using Confluent.Kafka;

namespace ggst_api.kafkaUtils
{
    public class KafkaConfig
    {
        private IConfiguration _configuration;

        private ProducerConfig _producerConfig;

        private ConsumerConfig _consumerConfig;

        //for test
        public KafkaConfig(string conn) {
            //provider
            ProducerConfig produceConfig = new ProducerConfig();
            produceConfig.BootstrapServers = conn;
            produceConfig.Acks = Acks.Leader;
            _producerConfig = produceConfig;
            //consumer
            ConsumerConfig consumerConfig = new ConsumerConfig();
            consumerConfig.BootstrapServers = conn;
            consumerConfig.GroupId = "luluhui_consumer";
            consumerConfig.AutoOffsetReset=AutoOffsetReset.Earliest;
            _consumerConfig = consumerConfig;
        }

        public ProducerBuilder<TKey, TValue> GetProducerBuilder<TKey, TValue>() {
            return new ProducerBuilder<TKey, TValue>(_producerConfig);
        }

        public ConsumerBuilder<TKey, TValue> GetConsumerBuilder<TKey, TValue>() { 
            return new ConsumerBuilder<TKey, TValue>(_consumerConfig);
        }

        public void produceSendMessage<Tkey, Tvalue>(string topic,int partition,Tkey key, Tvalue value) {
            var producer = GetProducerBuilder<Tkey, Tvalue>().Build();
            TopicPartition topicPartition = new TopicPartition(topic, new Partition(partition));
            Message<Tkey, Tvalue> message = new Message<Tkey, Tvalue>() {Key=key, Value = value };
            producer.ProduceAsync(topicPartition, message);
        }

        




    }
}
