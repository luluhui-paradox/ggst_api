using ggst_api;
using ggst_api.config;
using ggst_api.Controllers;
using ggst_api.entity;
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
    }
}