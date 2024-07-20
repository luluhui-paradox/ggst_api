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




    }
}
