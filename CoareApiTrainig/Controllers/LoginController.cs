using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoareApiTrainig.Model;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CoareApiTrainig.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserModel data)
        {

            UserModel login = new UserModel();
            login.UserID = data.UserID;
            login.Password = data.Password;
            IActionResult response = Unauthorized();

            var user = AuthenticateUser(login);
            if (user != null)
            {
                var tokenStr = GenerateJasonWebToken(user);
                response = Ok(new { token = tokenStr });
            }
            return response;


        }

        private string GenerateJasonWebToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserID),
                new Claim(JwtRegisteredClaimNames.GivenName,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddDays(10),
                signingCredentials: credentials);

            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }

        private UserModel AuthenticateUser(UserModel login)
        {
            string connstr = _config["ConnectionStrings:Default"].ToString();

            using (IDbConnection conn = new SqlConnection(connstr))
            {
                string query = "SELECT UserID, UserName, Email FROM AppUser Where UserID = @UserID AND Password = @Password ";
                UserModel usr = SqlMapper.Query<UserModel>(conn, query, new { UserID = login.UserID, Password= login.Password }).FirstOrDefault();

                return usr;
            }

        }
    }
}