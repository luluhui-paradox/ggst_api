using ggst_api;
using ggst_api.Controllers;
using ggst_api.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using Xunit.Abstractions;
using System.Diagnostics;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using Xunit;
using ggst_api.config;
using ggst_api.entity;
using System.Text;
using System.Security.Cryptography;
using ggst_api.utils;

namespace TestProject1
{
    public class UnitTest1: IClassFixture<Top100Controller>
    {
        private readonly ITop100Getter _top100Getter;

        private readonly SqlServerConnectDbcontext _dbcontext;

        private readonly ITestOutputHelper _logger;

        private readonly IConnectionMultiplexer _connection;

        private readonly Top100Controller _top100Controller;

        public UnitTest1(ITop100Getter top100GetService,SqlServerConnectDbcontext dbcontext,ITestOutputHelper logger,IConnectionMultiplexer connection,Top100Controller top100Controller) { 
            _top100Getter = top100GetService;
            _dbcontext = dbcontext;
            _logger = logger;
            _connection = connection;
            _top100Controller = top100Controller;
        }

        [Fact]
        public void redisTest() {
            var db = _connection.GetDatabase();
            db.StringSet("user", "jkl",expiry:TimeSpan.FromSeconds(30));
            var batch=db.CreateBatch();
            batch.StringSetAsync("user", "luluhui",TimeSpan.FromSeconds(100));
            batch.StringSetAsync("local", "dm", TimeSpan.FromSeconds(100));
            string? st=db.StringGet("user");
            _logger.WriteLine($"user value is {st}");
            batch.Execute();
            _logger.WriteLine($"user value is {db.StringGet("user")}");
        }

        [Fact]
        public void controllerTest1() {
            _top100Controller.searchUsersByUsername("umisho");
            _top100Controller.searchUserExt("umisho","dsadsa",null);
            _top100Controller.getTop100info();
        }

        [Fact]
        public void LoginUserAddTest() { 
            TbLoginUser loginUser = new TbLoginUser();
            loginUser.LoginUserAccount = "luluhui_counter";
            loginUser.LoginUserName = "luluhui";
            loginUser.LoginUserPassword = MD5Util.ComputeMd5Hash("luluhui");
            //insert
            _dbcontext.Add(loginUser);
            _dbcontext.SaveChanges();
            _dbcontext.Entry(loginUser).State=Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        [Fact]
        public void addConnectTest() { 
            //select tbplayerInfo.uuid
            PlayerInfoEntity? playerInfo = _dbcontext.PlayerInfoEntities.Where(p =>  p.name.Equals("UMISHO")).FirstOrDefault();
            _logger.WriteLine($"------------playerInfo is null ?? {playerInfo == null}-------------");
            TbLoginUser? tbLoginUser = _dbcontext.TbLoginUsers.Where(tbl => tbl.LoginUserName.Equals("luluhui")).FirstOrDefault();
            _logger.WriteLine($"------------tbLoginUser is null ?? {tbLoginUser == null}-------------");
            if (playerInfo != null && tbLoginUser != null) { 
                // make a new record
                TbUser2Character tbUser2Character = new TbUser2Character();
                
                tbUser2Character.UserId=tbLoginUser.LoginUserId;
                tbUser2Character.PlayUuid=playerInfo.id;
                //search
                
                TbUser2Character? serachres = _dbcontext.TbUser2Characters.Where(s => s.UserId==tbLoginUser.LoginUserId&&s.PlayUuid.Equals(playerInfo.id)).FirstOrDefault();
                if (serachres == null){
                    //insert
                    _dbcontext.Add(tbUser2Character);
                    _dbcontext.SaveChanges();
                    _dbcontext.Entry(tbUser2Character).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }
                
            }
        }
    }
}