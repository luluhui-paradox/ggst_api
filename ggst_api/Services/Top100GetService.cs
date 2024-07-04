using ggst_api.config;
using ggst_api.Controllers;
using ggst_api.entity;
using ggst_api.utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;

namespace ggst_api.Services
{
    public class Top100GetService:ITop100Getter
    {
        private readonly ILogger<Top100Controller> _logger;

        private readonly RatingUpdateHttpUtil _ratingUpdateHttpUtil;

        private readonly SqlServerConnectDbcontext _dbContext;

        public Top100GetService(ILogger<Top100Controller> logger , SqlServerConnectDbcontext context , RatingUpdateHttpUtil ratingUpdateHttpUtil) {
            _logger = logger;
            _ratingUpdateHttpUtil = ratingUpdateHttpUtil;
            _dbContext = context;
        }

        
        public  List<PlayerInfoEntity> getTop100()
        {
            //select items from sqlserver
            List<PlayerInfoEntity> reslist=(from item in _dbContext.PlayerInfoEntities orderby item.rating_value descending select item).Take(100).ToList();
            
            return reslist;
        }

        public List<PlayerInfoEntity> searchUserExactFromDB(string username, string userid, string? char_short)
        {
            var source = (from item in _dbContext.PlayerInfoEntities where item.name.Equals(username) && item.id.Equals(userid) select item);
            if (char_short != null && !char_short.Equals(""))
            {
                source.Where(item => item.character_short.Equals(char_short));
            }
            return source.ToList();
        }

        public List<PlayerInfoEntity> searchUserExactFromRemote(string username, string userid, string? char_short)
        {
            var httpRequestMessage = "/api/search_exact?name=" + username;

            var httpResponseMessage = _ratingUpdateHttpUtil.sendHttpAsync(httpRequestMessage).Result;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var loadStream = httpResponseMessage.Content.ReadAsStream();

                try
                {
                    List<PlayerInfoEntity> totalList = JsonSerializer.Deserialize<List<PlayerInfoEntity>>(loadStream);
                    List<PlayerInfoEntity> resList;
                    if (char_short.IsNullOrEmpty())
                    {
                        resList = (from item in totalList where item.id.Equals(userid) select item).ToList();
                    }
                    else {
                        resList = (from item in totalList where item.id.Equals(userid) && item.character_short.Equals(char_short) select item).ToList();
                    }
                    return resList == null ? new List<PlayerInfoEntity>() : resList;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return new List<PlayerInfoEntity>();
                }

            }
            else
            {
                _logger.LogWarning("HTTP FAILED");
                return new List<PlayerInfoEntity>();
            }
        }

        public List<PlayerInfoEntity> searchUsersFromDB(string username)
        {
            try
            {
                List<PlayerInfoEntity> res = _dbContext.PlayerInfoEntities.Where(item => item.name.Contains(username)).ToList();
                return res ?? new List<PlayerInfoEntity>();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new List<PlayerInfoEntity>();
            }
            
        }

        public  List<PlayerInfoEntity> searchUsersFromRemote(string username)
        {
            var httpRequestMessage = "/api/search?name=" + username;

            var httpResponseMessage = _ratingUpdateHttpUtil.sendHttpAsync(httpRequestMessage).Result;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var loadStream = httpResponseMessage.Content.ReadAsStream();

                try
                {
                    List<PlayerInfoEntity> resList = JsonSerializer.Deserialize<List<PlayerInfoEntity>>(loadStream);
                    return resList == null ? new List<PlayerInfoEntity>() : resList;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return new List<PlayerInfoEntity>();
                }

            }
            else
            {
                _logger.LogWarning("HTTP FAILED");
                return new List<PlayerInfoEntity>();
            }
        }



        public List<PlayerInfoEntity> selectUsersByNameAndRate(string username, int rating)
        {
            try
            {
                List<PlayerInfoEntity>? res = (from item in _dbContext.PlayerInfoEntities where (item.name.Contains(username) && item.rating_value > rating) select item).ToList();
                return res ?? new List<PlayerInfoEntity>();
            }
            catch (Exception)
            {
                return new List<PlayerInfoEntity>();
            }
        }





        
    }
}
