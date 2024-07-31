using ggst_api.config;
using ggst_api.ScheduleTask;
using ggst_api.Services;
using ggst_api.utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using ggst_api.kafkaUtils;

namespace TestProject1
{
    public class Startup
    {
        public readonly string sql_connect_str;
        public readonly string redis_connect_str;
        public readonly string kafka_bootstrapserver;

        public Startup() {
            // json config
            string currentDirectory = Directory.GetCurrentDirectory();

            // 构建 JSON 文件的完整路径
            string filePath = Path.Combine(currentDirectory, "testJsonConfigRelease.json");
            using (var fileStream = File.OpenRead(filePath))
            {
                var jsonDocument = JsonDocument.Parse(fileStream);

                // 获取根元素
                var root = jsonDocument.RootElement;

                // 获取 ConnectionStrings 对象
                var connectionStrings = root.GetProperty("ConnectionStrings");

                // 获取 GGST_DB 的值
                sql_connect_str = connectionStrings.GetProperty("GGST_DB").GetString();
                

                // 获取 REDIS_LOCAL 的值
                redis_connect_str = connectionStrings.GetProperty("REDIS_LOCAL").GetString();

                kafka_bootstrapserver = root.GetProperty("kafka_config").GetProperty("BootstrapServers").GetString();


                // 记得释放资源
                jsonDocument.Dispose();
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {

            // 服务注册
            services.AddScoped<ITop100Getter,Top100GetService>();
            services.AddScoped<IRatingUpdateHttpUtil, RatingUpdateHttpUtil>();
            services.AddScoped<RatingUpdateHttpUtil>();
            services.AddScoped<IUpdateDbSchedule, UpdateDbSchedule>();
            services.AddScoped<ResultUpdate>();
            services.AddHttpClient();
            services.AddControllers();

            //kafka service
            services.AddSingleton<KafkaConfig>(
                provider => {
                    return new KafkaConfig(kafka_bootstrapserver);
                }    
            );



            //sqlserver context
            services.AddDbContextFactory<SqlServerConnectDbcontext>(
                option => {

                    option.UseMySQL(sql_connect_str);
                    option.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                }
            );

            //redis
            services.AddSingleton<IConnectionMultiplexer>(
                provider =>
                {
                    var config = ConfigurationOptions.Parse(redis_connect_str);
                    return ConnectionMultiplexer.Connect(config);
                }
             );

            //redis cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redis_connect_str;
                options.InstanceName = "luluhuiInstance"; // 可选的实例名称
            });

        }
    }

}
