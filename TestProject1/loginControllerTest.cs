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
    public class LoginUserControllerTest : IClassFixture<LoginUserController>
    {
        private readonly ITop100Getter _top100Getter;

        private readonly SqlServerConnectDbcontext _dbcontext;

        private readonly ITestOutputHelper _logger;

        private readonly IConnectionMultiplexer _connection;

        private readonly LoginUserController _loginUserController;

        public LoginUserControllerTest(ITop100Getter top100GetService,SqlServerConnectDbcontext dbcontext,ITestOutputHelper logger,IConnectionMultiplexer connection, LoginUserController loginUserController) { 
            _top100Getter = top100GetService;
            _dbcontext = dbcontext;
            _logger = logger;
            _connection = connection;
            _loginUserController = loginUserController;
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
        public void controllerTest() {
            var res = _loginUserController.Register("15723174912","luluhui_alter","asdf");
            Assert.True(res != null && res.Value.ContainsKey("success"));
            
        }

        
    }
}