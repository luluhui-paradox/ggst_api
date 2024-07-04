using ggst_api.config;
using ggst_api.Controllers;
using ggst_api.entity;
using ggst_api.utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;


namespace ggst_api.ScheduleTask
{
    public class UpdateDbSchedule:IUpdateDbSchedule
    {
        private readonly ILogger<UpdateDbSchedule> _logger;

        private readonly RatingUpdateHttpUtil _ratingUpdateHttpUtil;

        private readonly SqlServerConnectDbcontext _dbContext;



        public UpdateDbSchedule(ILogger<UpdateDbSchedule> logger , SqlServerConnectDbcontext context, RatingUpdateHttpUtil ratingUpdateHttpUtil)
        {
            _logger = logger;
            _ratingUpdateHttpUtil = ratingUpdateHttpUtil;
            _dbContext = context;

        }

        public async Task<int> updateDb()
        {
            List<PlayerInfoEntity> sourceList=new List<PlayerInfoEntity>();
            int updateItemCount = 0;
            int failed_retry_count = 0;
            bool isSuccess=false;

            while (!isSuccess && failed_retry_count < 5) {
                try
                {
                    sourceList.AddRange(getAllDistinctFromDb());


                     isSuccess = true;
                }
                catch (Exception e)
                {

                    failed_retry_count++;

                }
             }

            if (isSuccess) { 
                var taskList=new List<Task<List<PlayerInfoEntity>>>();
                var updateList = new List<PlayerInfoEntity>();
                foreach (var item in sourceList) {
                    taskList.Add(getListFromRemote(item));
                }
                while (taskList.Count>0) {
                    
                    for (int idx=taskList.Count-1;idx>=0;idx--) {
                        var taskitem=taskList[idx];
                        if (taskitem.IsCompleted) {
                            if (taskitem.IsCompletedSuccessfully) { 
                                List<PlayerInfoEntity> taskres= taskitem.Result;
                                updateList.AddRange(taskres);
                            }
                            //remove task 
                            taskList.Remove(taskitem);
                        }
                    }
                }

                //update db
                foreach (var dbitem in updateList) {


                    try
                    {
                        var existingEntity = _dbContext.PlayerInfoEntities.FirstOrDefault(e => e.id.Equals(dbitem.id)&&e.character.Equals(dbitem.character));
                        if (existingEntity != null)
                        {

                            existingEntity.name = dbitem.name;
                            existingEntity.game_count = dbitem.game_count;
                            existingEntity.platform = dbitem.platform;
                            existingEntity.rating_value= dbitem.rating_value;
                            existingEntity.rating_deviation=dbitem.rating_deviation;
                            _dbContext.SaveChanges();
                        }
                        else {


                            PlayerInfoEntity entityForUpdate = dbitem.deepcopy();
                            _dbContext.PlayerInfoEntities.Add(entityForUpdate);
                            
                            _dbContext.SaveChanges();
                        }
                        updateItemCount++;
                    }
                    catch (Exception e)
                    {

                        _logger.LogError(e.Message);

                        if (e.InnerException != null) {

                        }
                        
                    }
                }
            }
            return updateItemCount;
        }

        public List<PlayerInfoEntity> getAllDistinctFromDb() {
            List<PlayerInfoEntity> sourceList = _dbContext.PlayerInfoEntities.AsNoTracking().GroupBy(item => item.id).Select(group => group.First()).ToList();
                
            return sourceList;
        }

        public async Task<List<PlayerInfoEntity>?> getListFromRemote(PlayerInfoEntity entity) {
            if (entity == null || entity.id == null || entity.name == null) {
                return new List<PlayerInfoEntity>();
            }
            var httpRequestMessage = $"/api/search_exact?name={entity.name}";

            var httpResponseMessage = await _ratingUpdateHttpUtil.sendHttpAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var loadStream = httpResponseMessage.Content.ReadAsStream();

                try
                {
                    List<PlayerInfoEntity> totalList = JsonSerializer.Deserialize<List<PlayerInfoEntity>>(loadStream);
                    List<PlayerInfoEntity> resList;
                    resList = (from item in totalList where item.id.Equals(entity.id) select item).ToList();
                    return resList == null ? new List<PlayerInfoEntity>() : resList;
                }
                catch (Exception e)
                {

                   
                    return new List<PlayerInfoEntity>();
                }

            }
            else
            {
                _logger.LogWarning("HTTP FAILED");
                return new List<PlayerInfoEntity>();
            }

        }
    }
}
