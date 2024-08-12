using ggst_api.config;
using ggst_api.entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using ggst_api.utils;
using StackExchange.Redis;
using System.Text.RegularExpressions;
using Mysqlx.Crud;

namespace ggst_api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class LoginUserController : ControllerBase
    {
        private readonly SqlServerConnectDbcontext _dbContext;
        private readonly ILogger<LoginUserController> _logger;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly TokenGenUtil _tokenGenUtil;

        public LoginUserController(SqlServerConnectDbcontext sqlServerConnectDbcontext, ILogger<LoginUserController> logger,IConnectionMultiplexer redis_connection,TokenGenUtil tokenGenUtil) {
            _dbContext = sqlServerConnectDbcontext;
            _logger = logger;
            _redisConnection = redis_connection;
            _tokenGenUtil = tokenGenUtil;
        }


        [HttpGet("login")]
        public ActionResult<Dictionary<string, Object>> Login(string user_account,string password_md5) {
            //select from db
            TbLoginUser? playerInfo = _dbContext.TbLoginUsers.AsNoTracking().Where(tb1 =>  tb1.LoginUserAccount==user_account && tb1.LoginUserPassword==password_md5).FirstOrDefault();
            if (playerInfo != null)
            {
                //exist,create an token, save to redis,expire at 4 hours
                string token = _tokenGenUtil.genToken(user_account);
                var redisdb = _redisConnection.GetDatabase(1);
                //create a tokenset
                redisdb.StringSet(token, playerInfo.LoginUserRole ,expiry:TimeSpan.FromMinutes(240), When.NotExists);
                //make a dict
                var resultres=new Dictionary<string, object>();
                resultres.Add("token",token);
                return Ok(resultres);

            }
            else {
                return BadRequest(new Dictionary<string,Object>());
            }
        }

        [HttpPost("register")]
        public ActionResult<Dictionary<string, Object>> Register(string user_account,string user_name,string user_pass_md5) {
            //use regex check user_account is phonenumer or email
            // 定义正则表达式
            string phonePattern = @"^(\+?86)?1[3-9]\d{9}$"; // 中国手机号格式
            string emailPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"; // 简单的邮箱格式

            bool isPhoneNumber = Regex.IsMatch(user_account, phonePattern);
            bool isEmail = Regex.IsMatch(user_account, emailPattern);

            if (!isPhoneNumber && !isEmail)
            {
                return BadRequest(new Dictionary<string, object>
            {
                { "error", "Invalid user account. It must be either a valid phone number or a valid email address." }
            });
            }

            //select if user_account is exist
            TbLoginUser? tbLoginUser=_dbContext.TbLoginUsers.AsNoTracking().Where(p=>p.LoginUserAccount==user_account).FirstOrDefault();
            if (tbLoginUser == null)
            {
                //insert
                try
                {
                    var InsertPO = new TbLoginUser { LoginUserAccount = user_account, LoginUserName = user_name, LoginUserPassword = user_pass_md5 };
                    _dbContext.TbLoginUsers.Add(InsertPO);
                    _dbContext.SaveChanges();
                    _dbContext.Entry(InsertPO).State = EntityState.Detached;
                    return Ok(new Dictionary<string, Object> {
                        { "success",user_account}
                    });
                }
                catch (Exception e)
                {

                    _logger.LogError(e.Message);
                    return Ok(new Dictionary<string, Object> { { "error", "cannot insert into db" } });
                }
            }
            else {
                return Ok(new Dictionary<string, Object> { { "error", "user already exist" } });
            }


        }
    }
}
