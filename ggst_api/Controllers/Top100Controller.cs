﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text.Json;
using System.IO;
using Microsoft.AspNetCore.Cors;
using ggst_api.Services;
using Microsoft.EntityFrameworkCore;
using ggst_api.config;
using ggst_api.entity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;

namespace ggst_api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class Top100Controller : ControllerBase
    {
        private readonly ITop100Getter _top100Getter;
        private readonly DbContext _dbContext;
        private readonly ILogger<Top100Controller> _logger;
        private readonly IDistributedCache _distributedCache;

        public Top100Controller(ITop100Getter top100Getter,SqlServerConnectDbcontext sqlServerConnectDbcontext, ILogger<Top100Controller> logger,IDistributedCache distributedCache)
        {
            _top100Getter = top100Getter;
            _dbContext = sqlServerConnectDbcontext;
            _logger = logger;
            _distributedCache = distributedCache;
        }


        [HttpGet("getTop100")]
        [EnableCors("luluhui_policy")]
        public List<PlayerInfoEntity> getTop100info() {
            var cachedData = _distributedCache.GetString("getTop100info");

            if (!cachedData.IsNullOrEmpty())
            {
                try
                {
                    return PlayerInfoEntity.decodeFromString(cachedData);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    List<PlayerInfoEntity> res = _top100Getter.getTop100();
                    var options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache for 10 minutes
                    };
                    _distributedCache.SetStringAsync("getTop100info",PlayerInfoEntity.encodeFromList(res),options);
                    return res;
                }
            }
            else {
                List<PlayerInfoEntity> res = _top100Getter.getTop100();
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache for 10 minutes
                };
                _distributedCache.SetStringAsync("getTop100info", PlayerInfoEntity.encodeFromList(res), options);
                return res;
            }
            
        }

        [HttpGet("searchUsersByUsername")]
        [EnableCors("luluhui_policy")]
        public List<PlayerInfoEntity> searchUsersByUsername(string username) {
            //search from redis
            List<PlayerInfoEntity> res;
            var cachedData = _distributedCache.GetString($"_searchUsersByUsername_{username}");
            if (!cachedData.IsNullOrEmpty()) {
                try
                {
                    res = PlayerInfoEntity.decodeFromString(cachedData);
                    return res;
                }
                catch (Exception e)
                { 
                    _logger.LogError(e.Message);
                }
            }
            res= _top100Getter.searchUsersFromDB(username);
            if (res.IsNullOrEmpty()) {

                res = _top100Getter.searchUsersFromRemote(username);
                foreach (PlayerInfoEntity item in res)
                {
                    try
                    {
                        PlayerInfoEntity copyitem = item.deepcopy();
                        _dbContext.Add(copyitem);
                        _dbContext.SaveChanges();
                    }
                    catch (Exception)
                    {
                        _logger.LogError($"cannot insert item: id:{item.id},char_shot : {item.character_short}");
                        
                    }

                }
            }
            //insert into redis
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache for 10 minutes
            };
            _distributedCache.SetStringAsync($"_searchUsersByUsername_{username}", PlayerInfoEntity.encodeFromList(res), options);

            return res;
        }

        

        

        [HttpGet("searchUserExt")]
        [EnableCors("luluhui_policy")]
        public List<PlayerInfoEntity> searchUserExt(string username,string userid,string? char_short) {
            //search from redis
            List<PlayerInfoEntity> res;
            var cachedData = _distributedCache.GetString($"_searchUserExt_{username}_{userid}_{char_short}");
            if (!cachedData.IsNullOrEmpty())
            {
                try
                {
                    res = PlayerInfoEntity.decodeFromString(cachedData);
                    return res;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            }
            res = _top100Getter.searchUserExactFromDB( username,  userid,  char_short);
            if (res.IsNullOrEmpty() ){ 
                res=_top100Getter.searchUserExactFromRemote(username, userid, char_short);
                foreach (PlayerInfoEntity item in res) {
                    try
                    {
                        PlayerInfoEntity copyitem=item.deepcopy();
                        _dbContext.Add(copyitem);
                        _dbContext.SaveChanges();
                    }
                    catch (Exception)
                    {
                        _logger.LogError($"cannot insert item: id:{item.id},char_shot : {item.character_short}");
                       
                    }
                    
                }
                
            }
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache for 10 minutes
            };
            _distributedCache.SetStringAsync($"_searchUserExt_{username}_{userid}_{char_short}", PlayerInfoEntity.encodeFromList(res), options);
            return res;
        }

        

        





    }


}
