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

namespace BlazingChat.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly BlazingChatContext _context;
        private readonly IConfiguration _configuration;

        public UserController(ILogger<UserController> logger, BlazingChatContext context, IConfiguration configuration)
        {
            this.logger = logger;
            this._context = context;
            this._configuration = configuration;
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
            if(emailAddressExists == null)
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
    }
}
