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

namespace TestProject1
{
    public class Startup
    {
  
        public void ConfigureServices(IServiceCollection services)
        {

            // 服务注册
            services.AddTransient<ITop100Getter, Top100GetService>();
            services.AddScoped<IRatingUpdateHttpUtil, RatingUpdateHttpUtil>();
            services.AddScoped<RatingUpdateHttpUtil>();
            services.AddScoped<IUpdateDbSchedule, UpdateDbSchedule>();
            services.AddHttpClient();
            services.AddControllers();
           


        }
    }

}
