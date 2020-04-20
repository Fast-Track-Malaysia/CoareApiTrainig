using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoareApiTrainig.Model;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CoareApiTrainig.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingController : ControllerBase
    {
        private IConfiguration configuration;

        public TestingController(IConfiguration config)
        {
            configuration = config;
        }
        // GET: api/Testing
        [HttpGet("GetIdentity")]
        public IEnumerable<string> Get()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            return new string[] { claim[0].Value, claim[1].Value, claim[2].Value };
        }

        // GET: api/Testing/5
        [Authorize]
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(string id)
        {
            string connstr = configuration["ConnectionStrings:Default"];
            DataTable dt = new DataTable();
            IList<UserModel> users = new List<UserModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connstr))
                {
                    string query = "SELECT UserID, UserName, Email FROM AppUser ";
                    users = SqlMapper.Query<UserModel>(conn, query, null).ToList();

                    //using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                    //{
                    //    dat.SelectCommand.CommandText = "SELECT * FROM AppUser ";
                    //    dat.Fill(dt);
                    //    foreach(DataRow row in dt.Rows)
                    //    {
                    //        UserModel usr = new UserModel
                    //        {
                    //            UserID = row["UserID"].ToString(),
                    //            UserName = row["UserName"].ToString(),
                    //            Email = row["Email"].ToString()
                    //        };
                    //        users.Add(usr);
                    //    }
                    //}
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok( users);
        }

        // POST: api/Testing
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Testing/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
