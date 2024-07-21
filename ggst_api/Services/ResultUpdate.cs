using ggst_api.entity;
using ggst_api.kafkaUtils;
using StackExchange.Redis;

namespace ggst_api.Services
{
    public class ResultUpdate
    {
        private readonly KafkaConfig _kafkaConfig;
        private readonly IConnectionMultiplexer _redisConnection;

        public ResultUpdate(KafkaConfig kafkaConfig, IConnectionMultiplexer connectionMultiplexer) { 
            _kafkaConfig = kafkaConfig;
            _redisConnection=connectionMultiplexer;
        }

        public void sendSync(List<PlayerInfoEntity> playerInfoEntities) {
            var db=_redisConnection.GetDatabase();
            foreach(var item in playerInfoEntities) {
                //setnx to redis
                //if updateSet Not exist,get redis lock and create Set with expire time
                while(!db.KeyExists("updateSet"))
                {
                    bool isSet = db.StringSet("update_lock", "1", TimeSpan.FromSeconds(5), When.NotExists);
                    if (isSet) {
                        db.SetAdd("updateSet", "xijinping");
                        //db.SetRemove("updateSet", "xijinping");
                        db.KeyExpire("updateSet", TimeSpan.FromMinutes(30));
                        //release lock
                        db.KeyDelete("update_lock");
                    }

                }
                var res = db.SetAdd("updateSet", item.name);
                if (res) {
                    //check if updateSet has expire
                    TimeSpan? expiration = db.KeyTimeToLive("updateSet");
                    if (!expiration.HasValue) {
                        db.SetAdd("updateSet", item.name);
                        db.KeyExpire("updateSet", TimeSpan.FromMinutes(30));
                    }
                    //produce to kafka
                    int retry_times = 0;bool success=false;
                    while (retry_times < 5 && success == false) {
                        try
                        {
                            _kafkaConfig.produceSendMessage<string, string>("test_topic", 0, "luluhui_consumer", item.name);
                            success = true;
                        }
                        catch (Exception)
                        {
                            retry_times++;
                        }
                    }
                }
                
            }
        }
    }
}
