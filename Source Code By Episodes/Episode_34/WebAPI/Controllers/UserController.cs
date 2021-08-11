using BlazingChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlazingChat.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using RestSharp.Authenticators;
using RestSharp;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace BlazingChat.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly BlazingChatContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(ILogger<UserController> logger,
                                BlazingChatContext context,
                                IConfiguration configuration,
                                IHttpClientFactory httpClientFactory,
                                IWebHostEnvironment webHostEnvironment)
        {
            this._logger = logger;
            this._context = context;
            this._configuration = configuration;
            this._httpClientFactory = httpClientFactory;
            this._webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("getcontacts")]
        public List<User> GetContacts()
        {
            return _context.Users.ToList();
        }

        //Authentication Methods

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

        //Migrating to JWT Authorization...
       
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

                if (jwtSecurityToken != null
                    && jwtSecurityToken.ValidTo > DateTime.Now
                    && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    //returning the user if found
                    var userId = principle.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    return await GetUserByUserID(Convert.ToInt64(userId));
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
        [HttpGet("getfacebookappidandredirecturi")]
        public ActionResult<string> GetFacebookAppIDAndRedirectUri()
        {
            return _configuration["Authentication:Facebook:AppId"] + "&" + _configuration["Authentication:Facebook:RedirectUri"];
        }

        [HttpPost("getfacebookjwt")]
        public async Task<ActionResult<AuthenticationResponse>> GetFacebookJWT([FromBody] FacebookAuthRequest facebookAuthRequest)
        {
            // 1.create a token and an http client
            string token = string.Empty;
            var httpClient = _httpClientFactory.CreateClient();

            // 2.get AppId and AppSecrete
            // string appId = _configuration["Authentication:Facebook:AppId"];
            // string appSecrete = _configuration["Authentication:Facebook:AppSecrete"];
            // Console.WriteLine("\nApp Id : " + appId);
            // Console.WriteLine("Secrete Id : " + appSecrete + "\n");

            // // 3. generate an app access token
            // var appAccessRequest = $"https://graph.facebook.com/oauth/access_token?client_id={appId}&client_secret={appSecrete}&grant_type=client_credentials";
            // var appAccessTokenResponse = await httpClient.GetFromJsonAsync<FacebookAppAccessToken>(appAccessRequest);
            // Console.WriteLine("App Access Token : " + appAccessTokenResponse.Access_Token);
            // Console.WriteLine("Auth Request Access Token : " + facebookAuthRequest.AccessToken + "\n");

            // // 4. validate the user access token
            // var userAccessValidationRequest = $"https://graph.facebook.com/debug_token?input_token={facebookAuthRequest.AccessToken}&access_token={appAccessTokenResponse.Access_Token}";
            // var userAccessTokenValidationResponse = await httpClient.GetFromJsonAsync<FacebookUserAccessTokenValidation>(userAccessValidationRequest);
            // Console.WriteLine("Is Token Valid : " + userAccessTokenValidationResponse.Data?.Is_Valid + "\n");

            // if (!userAccessTokenValidationResponse.Data.Is_Valid) 
            //     return BadRequest();

            // 5. we've got a valid token so we can request user data from facebook
            var userDataRequest = $"https://graph.facebook.com/v11.0/me?fields=id,email,first_name,last_name,name,gender,locale,birthday,picture&access_token={facebookAuthRequest.AccessToken}";
            var facebookUserData = await httpClient.GetFromJsonAsync<FacebookUserData>(userDataRequest);
            Console.WriteLine("Facebook Email Address : " + facebookUserData.Email + "\n");

            //6. try to find the user in the database or create a new account
            var loggedInUser = await GetUserByEmailAddress(facebookUserData.Email);

            //7. generate the token
            if (loggedInUser == null) loggedInUser = await CreateExternalUser(facebookUserData.Email);

            token = GenerateJwtToken(loggedInUser);
            Console.WriteLine("JWT : " + token + "\n");

            httpClient.Dispose();

            return await Task.FromResult(new AuthenticationResponse() { Token = token });
        }

        //Twitter Authentication using JWT
        [HttpGet("gettwitteroauthtokenusingresharp")]
        public ActionResult<TwitterRequestTokenResponse> GetTwitterOAuthTokenUsingResharpAsync()
        {
            var consumerKey = _configuration["Authentication:Twitter:ConsumerKey"];
            var consumerSecrete = _configuration["Authentication:Twitter:ConsumerSecrete"];
            var callbackUrl = _configuration["Authentication:Twitter:CallbackUrl"];

            var client = new RestClient("https://api.twitter.com"); // Note NO /1

            client.Authenticator = OAuth1Authenticator.ForRequestToken(
                consumerKey,
                consumerSecrete,
                callbackUrl // Value for the oauth_callback parameter
            );

            var request = new RestRequest("/oauth/request_token", Method.POST);
            var response = client.Execute(request);

            var qs = HttpUtility.ParseQueryString(response.Content);

            var _token = qs["oauth_token"];
            var _tokenSecret = qs["oauth_token_secret"];
            var _callbackUrlConfirmed = qs["oauth_callback_confirmed"];

            return new TwitterRequestTokenResponse() { OAuthToken = _token, OAuthTokenSecrete = _tokenSecret, OAuthCallBackConfirmed = _callbackUrlConfirmed };
        }

        [HttpPost("gettwitterjwt")]
        public async Task<ActionResult<AuthenticationResponse>> GetTwitterJWT([FromBody] TwitterRequestTokenResponse twitterRequestTokenResponse)
        {
            // Step 1 : initializing variables
            string token = string.Empty;
            var httpClient = _httpClientFactory.CreateClient();

            // Step 2 : getting keys from appsettings.json
            var consumerKey = _configuration["Authentication:Twitter:ConsumerKey"];
            var consumerSecrete = _configuration["Authentication:Twitter:ConsumerSecrete"];

            // Step 3 : requesting oauth_token & oauth_token_secrete
            var client = new RestClient("https://api.twitter.com"); // Note NO /1

            client.Authenticator = OAuth1Authenticator.ForAccessToken(
                consumerKey,
                consumerSecrete,
                twitterRequestTokenResponse.OAuthToken,
                twitterRequestTokenResponse.OAuthTokenSecrete,
                twitterRequestTokenResponse.OAuthVerifier
            );

            var request = new RestRequest("/oauth/access_token", Method.POST);
            var response = client.Execute(request);

            var qs = HttpUtility.ParseQueryString(response.Content);
            var _token = qs["oauth_token"];
            var _tokenSecret = qs["oauth_token_secret"];

            // Step 4 : passing oauth_token & oauth_token_secrete to Twitter API to get email address of the user
            var emailAddress = await GetTwitterEmailAddress(_token, _tokenSecret);

            // Step 5 : try to find the user in the database or create a new account
            var loggedInUser = await GetUserByEmailAddress(emailAddress);

            // Step 6 : generate the token
            if (loggedInUser == null) loggedInUser = await CreateExternalUser(emailAddress);

            token = GenerateJwtToken(loggedInUser);
            httpClient.Dispose();

            // Step 7 : returning the token back to the client
            return await Task.FromResult(new AuthenticationResponse() { Token = token });
        }

        //Google Authentication using JWT
        [HttpGet("getgoogleclientidandredirecturi")]
        public ActionResult<string> GetGoogleClientIDAndRedirectUri()
        {
            return _configuration["Authentication:Google:ClientId"] + "&" + _configuration["Authentication:Google:RedirectUri"];
        }

        // Authorization Code
        [HttpGet("getuserswithoutrole")]
        public async Task<List<User>> GetUsersWithoutRole()
        {
            return await _context.Users.Select(user => new User() { UserId = user.UserId ,EmailAddress = user.EmailAddress } ).ToListAsync();
        }

        [HttpPut("assignrole")]
        public async Task<int> AssignRole([FromBody] User user)
        {
            User userToUpdate = await _context.Users.Where(u => u.UserId == user.UserId).FirstOrDefaultAsync();
            userToUpdate.Role = user.Role;
            int result = await _context.SaveChangesAsync();

            return result;
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("deleteuser/{userId}")]
        public async Task<int> DeleteUser(long userId)
        {
            _context.Users.Remove(new User() { UserId = userId });
            return await _context.SaveChangesAsync();
        }

        //Helper methods
        protected async Task<string> GetTwitterEmailAddress(string token, string tokenSecrete)
        {
            // Step 1 : initializing variables
            var httpClient = _httpClientFactory.CreateClient();
            var verifyCredentialsUrl = "https://api.twitter.com/1.1/account/verify_credentials.json";
            var nonce = GetNonce();
            var timeStamp = GetCurrentTimeStamp();

            // Step 2 : getting keys from appsettings.json
            var consumerKey = _configuration["Authentication:Twitter:ConsumerKey"];
            var consumerSecrete = _configuration["Authentication:Twitter:ConsumerSecrete"];
            var callbackUrl = _configuration["Authentication:Twitter:CallbackUrl"];

            // Step 3 : colleting parameters
            var parameters = $"include_email=true";
            parameters += $"&oauth_callback={Uri.EscapeDataString(callbackUrl)}";
            parameters += $"&oauth_consumer_key={consumerKey}";
            parameters += $"&oauth_nonce={nonce}";
            parameters += $"&oauth_signature_method=HMAC-SHA1";
            parameters += $"&oauth_timestamp={timeStamp}";
            parameters += $"&oauth_token={token}";
            parameters += $"&oauth_version=1.0";

            // Step 4 : creating base signature string
            var baseSignatureString = $"GET";
            baseSignatureString += $"&{Uri.EscapeDataString(verifyCredentialsUrl)}";
            baseSignatureString += $"&{Uri.EscapeDataString(parameters)}";

            // Step 5 : creating Signing Key
            var signingKey = $"{consumerSecrete}&{tokenSecrete}";

            // Step 6 : Generating the signature
            Byte[] secretBytes = UTF8Encoding.UTF8.GetBytes(signingKey);
            HMACSHA1 hMACSHA1 = new HMACSHA1(secretBytes);

            Byte[] dataBytes = UTF8Encoding.UTF8.GetBytes(baseSignatureString);
            Byte[] calcHash = hMACSHA1.ComputeHash(dataBytes);
            String oAuthSignature = Convert.ToBase64String(calcHash);

            // Step 6 : creating the verify credentials request url
            var verifyCredentialsRequest = $"https://api.twitter.com/1.1/account/verify_credentials.json?include_email=true";
            verifyCredentialsRequest += $"&oauth_consumer_key={consumerKey}";
            verifyCredentialsRequest += $"&oauth_token={token}";
            verifyCredentialsRequest += $"&oauth_signature_method=HMAC-SHA1";
            verifyCredentialsRequest += $"&oauth_timestamp={timeStamp}";
            verifyCredentialsRequest += $"&oauth_nonce={nonce}";
            verifyCredentialsRequest += $"&oauth_version=1.0";
            verifyCredentialsRequest += $"&oauth_callback={Uri.EscapeDataString(callbackUrl)}";
            verifyCredentialsRequest += $"&oauth_signature={Uri.EscapeDataString(oAuthSignature)}";

            // Step 7 : making the request
            var twitterUserData = await httpClient.GetFromJsonAsync<TwitterUserData>(verifyCredentialsRequest);

            // Step 8 :returning email address of the user
            return twitterUserData.Email;
        }
        protected string GetNonce()
        {
            Random random = new Random();
            int length = 32;
            var randomString = string.Empty;
            for (var i = 0; i < length; i++)
                randomString += ((char)(random.Next(1, 26) + 64)).ToString().ToLower();

            var bytes = Encoding.UTF8.GetBytes(randomString);
            var encodedString = Convert.ToBase64String(bytes);

            return new String(encodedString.Where(c => Char.IsLetterOrDigit(c)).ToArray());
        }
        protected string GetCurrentTimeStamp()
        {
            var epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var now = DateTime.Now;
            return (now - epochDateTime).TotalSeconds.ToString().Split('.')[0];
        }
        protected AuthenticationProperties GetAuthenticationProperties(bool isPersistent = false)
        {
            return new AuthenticationProperties()
            {
                IsPersistent = isPersistent,
                //ExpiresUtc = DateTime.UtcNow.AddMinutes(10),
                RedirectUri = "/profile"
            };
        }
        protected async Task<User> GetUserByUserID(long userId)
        {
            return await _context.Users.Select(user => new User
            {
                UserId = user.UserId,
                EmailAddress = user.EmailAddress,
                Role = user.Role,
                DarkTheme = user.DarkTheme
            }).Where(user => user.UserId == userId).FirstOrDefaultAsync();
        }
        protected async Task<User> GetUserByEmailAddress(string emailAddress)
        {
            return await _context.Users.Select(user => new User
            {
                UserId = user.UserId,
                EmailAddress = user.EmailAddress,
                Role = user.Role
            }).Where(user => user.EmailAddress == emailAddress).FirstOrDefaultAsync();
        }
        protected async Task<User> CreateExternalUser(string emailAddress)
        {
            var externalUser = new User();
            externalUser.UserId = _context.Users.Max(user => user.UserId) + 1;
            externalUser.EmailAddress = emailAddress;
            externalUser.Password = Utility.Encrypt(externalUser.EmailAddress);
            externalUser.Source = "EXTL";

            _context.Users.Add(externalUser);
            int result = await _context.SaveChangesAsync();

            if (result > 0) return externalUser;
            else return null;
        }
        protected string GenerateJwtToken(User user)
        {
            //getting the secret key
            string secretKey = _configuration["JWTSettings:SecretKey"];
            var key = Encoding.ASCII.GetBytes(secretKey);

            //create claims
            var claimEmail = new Claim(ClaimTypes.Email, user.EmailAddress);
            var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString());
            var claimRole = new Claim(ClaimTypes.Role, user.Role == null ? "" : user.Role);

            //create claimsIdentity
            var claimsIdentity = new ClaimsIdentity(new[] { claimEmail, claimNameIdentifier, claimRole }, "serverAuth");

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

    }
}