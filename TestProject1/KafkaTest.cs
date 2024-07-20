using Confluent.Kafka;
using ggst_api.kafkaUtils;
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


        public KafkaTest(KafkaConfig kafkaConfig, ITestOutputHelper logger) { 
            _kafkaConfig = kafkaConfig;
            _logger = logger;
        }

        [Fact]
        public void kafkaProduceTest() {
            var producer = _kafkaConfig.GetProducerBuilder<string, string>().Build();
            TopicPartition topicPartition = new TopicPartition("test_topic",new Partition(0));
            Message<string, string> message = new Message<string, string>() { Value="hello world"};
            producer.Produce(topicPartition, message);
        }

        [Fact]
        public void kafkaConsumeTest() { 
            var consumer=_kafkaConfig.GetConsumerBuilder<string, string>().Build();
            consumer.Subscribe("test_topic");
            try
            {
                
                    var message = consumer.Consume();
                    _logger.WriteLine($"Received message: {message.Value}");
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
    }
}
