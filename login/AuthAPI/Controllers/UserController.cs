using System.Collections.Immutable;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthAPI.Context;
using AuthAPI.Helpers;
using AuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace AuthAPI.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;

        public UserController(AppDbContext appDbContext, IConfiguration configuration)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _configuration = configuration;
        }

        [HttpPost("authentitace")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if(userObj == null)
                return BadRequest();

            var user = await _appDbContext.Users
            .FirstOrDefaultAsync( u => u.UserName == userObj.UserName);

            if(user == null)
                return NotFound(new {Message = "User Not Found"});

            if(!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new {Message = "Password is incorrect"});
            }

            user.Token = CreateJwt(user);

            return Ok(new {
                Token = user.Token,
                Message = "Login Succed"
                });
        }

        [HttpPost("resgister")]
        public async Task<IActionResult> ResgisterUser([FromBody] User userObj)
        {
            if(userObj == null)
                return BadRequest();

            //Check email
            if(await CheckUserNameExisAsync(userObj.UserName))
                return BadRequest( new {Message = "User allready exists"});

            //check username
            if(await CheckEmailNameExisAsync(userObj.Email))
                return BadRequest( new {Message = "Email allready exists"});

            //check password strength
            var pass = CheckPasswordStrength(userObj.Password);
            if(!string.IsNullOrEmpty(pass))
                return BadRequest(new {Message = pass.ToString()});

            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            userObj.Role = "User";
            userObj.Token = "";
            await _appDbContext.Users.AddAsync(userObj);
            await _appDbContext.SaveChangesAsync();

            return Ok( new {Message = "User registred"});
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers() 
        {
            return Ok(await _appDbContext.Users.ToListAsync());
        }

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();

            if(password.Length < 8)
                sb.Append("Minimum pasword length should be 8" + Environment.NewLine);
            if(!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]")
                 && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password should be alphanumeric" + Environment.NewLine);
            if(!Regex.IsMatch(password, "[<,>,@,!,#,&,$,/,(,),{,},:,;,.,~,=,\\,|,',^,*,_,?]"))
                sb.Append("Password should contain special characters" + Environment.NewLine);

            return sb.ToString();
            
        }

        private async Task<bool> CheckUserNameExisAsync(string username) => 
                        await _appDbContext.Users.AnyAsync(u => u.UserName == username);
        
        private async Task<bool> CheckEmailNameExisAsync(string email) => 
                        await _appDbContext.Users.AnyAsync(u => u.Email == email);

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            
            var identity = new ClaimsIdentity(new Claim[]
            {
                 new Claim(ClaimTypes.Role, user.Role),
                 new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            });

            var credentials = new SigningCredentials( key , SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddHours(10),
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

       
    }
}