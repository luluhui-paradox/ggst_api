using Confluent.Kafka;
using ggst_api.entity;
using ggst_api.kafkaUtils;
using ggst_api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace TestProject1
{
    public class KafkaTest
    {
        private readonly KafkaConfig _kafkaConfig;

        private readonly ITestOutputHelper _logger;

        private readonly ResultUpdate _resultUpdate;

        public KafkaTest(KafkaConfig kafkaConfig, ITestOutputHelper logger,ResultUpdate resultUpdate) { 
            _kafkaConfig = kafkaConfig;
            _logger = logger;
            _resultUpdate = resultUpdate;
        }

        [Fact]
        public void kafkaProduceTest() {
            var producer = _kafkaConfig.GetProducerBuilder<string, string>().Build();
            TopicPartition topicPartition = new TopicPartition("test_topic",new Partition(0));
            Message<string, string> message = new Message<string, string>() { Key="luluhui_server" ,Value="hello world"};
            producer.ProduceAsync(topicPartition, message);
        }

        [Fact]
        public void kafkaConsumeTest() { 
            var consumer=_kafkaConfig.GetConsumerBuilder<string, string>().Build();
            consumer.Subscribe("test_topic");
            
            try
            {
                
                    var message = consumer.Consume();
                    _logger.WriteLine($"key:{message.Key}----Received message: {message.Value}");
                    // 在这里处理消息

                    // 提交偏移量
                    consumer.Commit(message);
                
            }
            catch (OperationCanceledException)
            {
                // 捕获到 OperationCanceledException 时，表示程序即将退出
            }
            finally
            {
                consumer.Close();
            }
        }

        [Fact]
        public void kafkaProduceAndConsumeTest() {
            _kafkaConfig.produceSendMessage<string, string>("test_topic",0,"luluhui","helloworld");
            kafkaConsumeTest();
        }

        [Fact]
        public void resultUpdateTest() { 
            var player=new PlayerInfoEntity();
            player.id = "000";
            player.name = "gj";
            List<PlayerInfoEntity> playerInfoEntities = new List<PlayerInfoEntity>();
            playerInfoEntities.Add(player);
            _resultUpdate.sendSync(playerInfoEntities);
        }

        [Fact]
        public void gettop100Test() { 
            _resultUpdate.sendTop100();
            kafkaConsumeTest();
        }
    }
}
