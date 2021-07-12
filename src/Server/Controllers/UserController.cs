using BlazingChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlazingChat.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace BlazingChat.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly BlazingChatContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(ILogger<UserController> logger, BlazingChatContext context, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this._context = context;
            this._configuration = configuration;
            this._httpClientFactory = httpClientFactory;
        }

        [HttpGet("getcontacts")]
        public List<User> GetContacts()
        {
            return _context.Users.ToList();
        }

        //Authentication Methods
        [HttpPost("loginuser")]
        public async Task<ActionResult<User>> LoginUser(User user, bool isPersistent)
        {
            user.Password = Utility.Encrypt(user.Password);
            User loggedInUser = await _context.Users
                                    .Where(u => u.EmailAddress == user.EmailAddress && u.Password == user.Password)
                                    .FirstOrDefaultAsync();

            if (loggedInUser != null)
            {
                //create a claim
                var claimEmail = new Claim(ClaimTypes.Email, loggedInUser.EmailAddress);
                var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, loggedInUser.UserId.ToString());
                //create claimsIdentity
                var claimsIdentity = new ClaimsIdentity(new[] { claimEmail, claimNameIdentifier }, "serverAuth");
                //create claimsPrincipal
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                //Sign In User
                await HttpContext.SignInAsync(claimsPrincipal, GetAuthenticationProperties(isPersistent));
            }
            return await Task.FromResult(loggedInUser);
        }

        [HttpGet("getcurrentuser")]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            User currentUser = new User();

            if (User.Identity.IsAuthenticated)
            {
                currentUser.EmailAddress = User.FindFirstValue(ClaimTypes.Email);
                currentUser = await _context.Users.Where(u => u.EmailAddress == currentUser.EmailAddress).FirstOrDefaultAsync();

                if (currentUser == null)
                {
                    currentUser = new User();
                    currentUser.UserId = _context.Users.Max(user => user.UserId) + 1;
                    currentUser.EmailAddress = User.FindFirstValue(ClaimTypes.Email);
                    currentUser.Password = Utility.Encrypt(currentUser.EmailAddress);
                    currentUser.Source = "EXTL";

                    _context.Users.Add(currentUser);
                    await _context.SaveChangesAsync();
                }
            }
            return await Task.FromResult(currentUser);
        }

        [HttpPost("registeruser")]
        public async Task<ActionResult> RegisterUser(User user)
        {
            //in this method you should only create a user record and not authenticate the user
            var emailAddressExists = _context.Users.Where(u => u.EmailAddress == user.EmailAddress).FirstOrDefault();
            if (emailAddressExists == null)
            {
                user.Password = Utility.Encrypt(user.Password);
                user.Source = "APPL";
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpGet("logoutuser")]
        public async Task<ActionResult<String>> LogOutUser()
        {
            await HttpContext.SignOutAsync();
            return "Success";
        }

        [HttpGet("TwitterSignIn")]
        public async Task TwitterSignIn(bool isPersistent)
        {
            await HttpContext.ChallengeAsync(TwitterDefaults.AuthenticationScheme,
                GetAuthenticationProperties(isPersistent));
        }

        [HttpGet("FacebookSignIn")]
        public async Task FacebookSignIn(bool isPersistent)
        {
            await HttpContext.ChallengeAsync(FacebookDefaults.AuthenticationScheme,
                GetAuthenticationProperties(isPersistent));
        }

        [HttpGet("GoogleSignIn")]
        public async Task GoogleSignIn(bool isPersistent)
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                GetAuthenticationProperties(isPersistent));
        }

        public AuthenticationProperties GetAuthenticationProperties(bool isPersistent = false)
        {
            return new AuthenticationProperties()
            {
                IsPersistent = isPersistent,
                //ExpiresUtc = DateTime.UtcNow.AddMinutes(10),
                RedirectUri = "/profile"
            };
        }
        [HttpGet("notauthorized")]
        public IActionResult NotAuthorized()
        {
            return Unauthorized();
        }

        //Migrating to JWT Authorization...
        private string GenerateJwtToken(User user)
        {
            //getting the secret key
            string secretKey = _configuration["JWTSettings:SecretKey"];
            var key = Encoding.ASCII.GetBytes(secretKey);

            //create claims
            var claimEmail = new Claim(ClaimTypes.Email, user.EmailAddress);
            var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString());

            //create claimsIdentity
            var claimsIdentity = new ClaimsIdentity(new[] { claimEmail, claimNameIdentifier }, "serverAuth");

            // generate token that is valid for 7 days
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            //creating a token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //returning the token back
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("authenticatejwt")]
        public async Task<ActionResult<AuthenticationResponse>> AuthenticateJWT(AuthenticationRequest authenticationRequest)
        {
            string token = string.Empty;

            //checking if the user exists in the database
            authenticationRequest.Password = Utility.Encrypt(authenticationRequest.Password);
            User loggedInUser = await _context.Users
                        .Where(u => u.EmailAddress == authenticationRequest.EmailAddress && u.Password == authenticationRequest.Password)
                        .FirstOrDefaultAsync();

            if (loggedInUser != null)
            {
                //generating the token
                token = GenerateJwtToken(loggedInUser);
            }
            return await Task.FromResult(new AuthenticationResponse() { Token = token });
        }

        [HttpPost("getuserbyjwt")]
        public async Task<ActionResult<User>> GetUserByJWT([FromBody] string jwtToken)
        {
            try
            {
                //getting the secret key
                string secretKey = _configuration["JWTSettings:SecretKey"];
                var key = Encoding.ASCII.GetBytes(secretKey);

                //preparing the validation parameters
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;

                //validating the token
                var principle = tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out securityToken);
                var jwtSecurityToken = (JwtSecurityToken)securityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    //returning the user if found
                    var userId = principle.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    return await _context.Users.Where(u => u.UserId == Convert.ToInt64(userId)).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                //logging the error and returning null
                Console.WriteLine("Exception : " + ex.Message);
                return null;
            }
            //returning null if token is not validated
            return null;
        }

        // Facebook Authentication using JWT
        [HttpGet("getfacebookappid")]
        public ActionResult<string> GetFacebookAppID()
        {
            return _configuration["Authentication:Facebook:AppId"];
        }

        [HttpPost("getfacebookjwt")]
        public async Task<ActionResult<AuthenticationResponse>> GetFacebookJWT([FromBody] FacebookAuthRequest facebookAuthRequest)
        {
            // 1.create a token and an http client
            string token = string.Empty;
            var httpClient = _httpClientFactory.CreateClient();

            // 2.get AppId and AppSecrete
            string appId = _configuration["Authentication:Facebook:AppId"];
            string appSecrete = _configuration["Authentication:Facebook:AppSecrete"];
            Console.WriteLine("\nApp Id : " + appId);
            Console.WriteLine("Secrete Id : " + appSecrete + "\n");

            // 3. generate an app access token
            var appAccessRequest = $"https://graph.facebook.com/oauth/access_token?client_id={appId}&client_secret={appSecrete}&grant_type=client_credentials";
            var appAccessTokenResponse = await httpClient.GetFromJsonAsync<FacebookAppAccessToken>(appAccessRequest);
            Console.WriteLine("App Access Token : " + appAccessTokenResponse.Access_Token);
            Console.WriteLine("Auth Request Access Token : " + facebookAuthRequest.AccessToken + "\n");

            // 4. validate the user access token
            var userAccessValidationRequest = $"https://graph.facebook.com/debug_token?input_token={facebookAuthRequest.AccessToken}&access_token={appAccessTokenResponse.Access_Token}";
            var userAccessTokenValidationResponse = await httpClient.GetFromJsonAsync<FacebookUserAccessTokenValidation>(userAccessValidationRequest);
            Console.WriteLine("Is Token Valid : " + userAccessTokenValidationResponse.Data?.Is_Valid + "\n");
            
            if (!userAccessTokenValidationResponse.Data.Is_Valid) 
                return BadRequest();

            // 5. we've got a valid token so we can request user data from facebook
            var userDataRequest = $"https://graph.facebook.com/v11.0/me?fields=id,email,first_name,last_name,name,gender,locale,birthday,picture&access_token={facebookAuthRequest.AccessToken}";
            var facebookUserData = await httpClient.GetFromJsonAsync<FacebookUserData>(userDataRequest);
            Console.WriteLine("Facebook Email Address : " + facebookUserData.Email + "\n");

            //6. try to find the user in the database or create a new account
            var loggedInUser = await _context.Users.Where(user => user.EmailAddress == facebookUserData.Email).FirstOrDefaultAsync();

            //7. generate the token
            if(loggedInUser == null)
            {
                loggedInUser = new User();
                loggedInUser.UserId = _context.Users.Max(user => user.UserId) + 1;
                loggedInUser.EmailAddress = User.FindFirstValue(ClaimTypes.Email);
                loggedInUser.Password = Utility.Encrypt(loggedInUser.EmailAddress);
                loggedInUser.Source = "EXTL";

                _context.Users.Add(loggedInUser);
                await _context.SaveChangesAsync();
            }

            token = GenerateJwtToken(loggedInUser);
            Console.WriteLine("JWT : " + token + "\n");
            
            httpClient.Dispose();
            
            return await Task.FromResult(new AuthenticationResponse() { Token = token });
        }
    
        //Twitter Authentication using JWT
        [HttpGet("gettwitteroauthtoken")]
        public async Task<ActionResult<string>> GetTwitterOAuthTokenAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            
            //Step 1: Encode consumer key and secret
            var consumerKey = _configuration["Authentication:Twitter:ConsumerKey"];
            var consumerSecrete = _configuration["Authentication:Twitter:ConsumerSecrete"];
            var accessToken = _configuration["Authentication:Twitter:AccessToken"];
            var accessTokenSecrete = _configuration["Authentication:Twitter:AccessTokenSecrete"];
            var callbackUrl = "https://localhost:5001/signin-twitter";
            var requestTokenUrl = "https://api.twitter.com/oauth/request_token";
            var nonce = GetNonce();
            var timeStamp = GetCurrentTimeStamp();

            //Colleting parameters
            var parameters = $"oauth_callback={Uri.EscapeDataString(callbackUrl)}";
            parameters += $"&oauth_consumer_key={consumerKey}";
            parameters += $"&oauth_nonce={nonce}";
            parameters += $"&oauth_signature_method=HMAC-SHA1";
            parameters += $"&oauth_timestamp={timeStamp}";
            parameters += $"&oauth_token={accessToken}";
            parameters += $"&oauth_version=1.0";

            //Creating base signature string
            var baseSignatureString = $"POST";
            baseSignatureString += $"&{Uri.EscapeDataString(requestTokenUrl)}";
            baseSignatureString += $"&{Uri.EscapeDataString(parameters)}";

            //Creating Signing Key
            var signingKey = $"{consumerSecrete}&{accessTokenSecrete}";

            //Generating the signature
            Byte[] secretBytes = UTF8Encoding.UTF8.GetBytes(signingKey);
            HMACSHA1 hMACSHA1 = new HMACSHA1(secretBytes);

            Byte[] dataBytes = UTF8Encoding.UTF8.GetBytes(baseSignatureString);
            Byte[] calcHash = hMACSHA1.ComputeHash(dataBytes);
            String oAuthSignature = Convert.ToBase64String(calcHash);
            
            string authHeader = string.Empty;
            authHeader += "OAuth ";
            authHeader += "oauth_nonce=\"" + nonce  + "\", ";
            authHeader += "oauth_callback=\"" + Uri.EscapeDataString(callbackUrl) + "\", ";
            authHeader += "oauth_signature_method=\"HMAC-SHA1\", ";
            authHeader += "oauth_timestamp=\"" + timeStamp + "\", ";
            authHeader += "oauth_token=\"" + accessToken + "\", ";
            authHeader += "oauth_consumer_key=\"" + consumerKey + "\", ";
            authHeader += "oauth_signature=\"" + Uri.EscapeDataString(oAuthSignature) + "\", ";
            authHeader += "oauth_version=\"1.0\"";

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"https://api.twitter.com/oauth/request_token");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Authorization", authHeader);

            var response = await httpClient.SendAsync(requestMessage);

            var responseStatusCode = response.StatusCode;
            var oAuthTokenResponse = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Parameters : " + parameters + "\n");
            Console.WriteLine("Base Signature String : " + baseSignatureString + "\n");
            Console.WriteLine("signingKey : " + signingKey + "\n");
            Console.WriteLine("oAuthSignature : " + oAuthSignature + "\n");
            Console.WriteLine("Authorization Header : " + authHeader + "\n");
            Console.WriteLine("oAuth Token : " + oAuthTokenResponse + "\n");

            //returning the oAuth token if found
            return oAuthTokenResponse;
        }

        private string GetNonce()
        {
            Random random = new Random();
            int length = 16;
            var randomString = string.Empty;
            for (var i = 0; i < length; i++)
                randomString += ((char)(random.Next(1, 26) + 64)).ToString().ToLower();
            
            var bytes = Encoding.UTF8.GetBytes(randomString);
            var encodedString = Convert.ToBase64String(bytes);

            return new String(encodedString.Where(c => Char.IsLetterOrDigit(c)).ToArray());
        }
        private string GetCurrentTimeStamp()
        {
            var epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var now = DateTime.Now;
            return (now - epochDateTime).TotalSeconds.ToString().Split('.')[0];
        }
    }
}
            //var request = $"https://api.twitter.com/oauth/request_token?oauth_consumer_key={consumerKey}&oauth_signature_method=HMAC-SHA1&oauth_timestamp={GetCurrentTimeStamp()}&oauth_nonce={GetNonce()}&oauth_version=1.0&oauth_callback={Uri.EscapeDataString("https://localhost:5001/signin-twitter")}&oauth_signature={Uri.EscapeDataString(oAuthSignature)}";
            //var response2 = httpClient.GetStringAsync(request);

            //Console.WriteLine("Request : " + request + "\n");
            //Console.WriteLine("Response : " + response2 + "\n");