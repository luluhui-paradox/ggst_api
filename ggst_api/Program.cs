using ggst_api.config;
using ggst_api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using ggst_api.utils;
using StackExchange.Redis;
using ggst_api.ScheduleTask;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add services to the container.
builder.Services.AddHttpClient();


//DI
builder.Services.AddScoped<ITop100Getter,Top100GetService>();
builder.Services.AddScoped<RatingUpdateHttpUtil>();
builder.Services.AddScoped<IUpdateDbSchedule, UpdateDbSchedule>();

//sqlserver context
builder.Services.AddDbContextFactory<SqlServerConnectDbcontext>(
    option => {
        if (builder.Environment.IsEnvironment("Release"))
        {
            option.UseMySQL(builder.Configuration.GetSection("ConnectionStrings")["GGST_DB"]);
        }
        else {
            option.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["GGST_DB"]);
        }
        //option.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["GGST_DB"]);
        option.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
);

//redis
builder.Services.AddSingleton<IConnectionMultiplexer>(
    provider =>
    {
        var config = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("REDIS_LOCAL"));
        return ConnectionMultiplexer.Connect(config);
    }
 );

//redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("REDIS_LOCAL");
    options.InstanceName = "luluhuiInstance"; // 可选的实例名称
});



builder.Services.AddCors(
    i=>i.AddPolicy("luluhui_policy",v=>v.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())    
);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsEnvironment("Release"))
{
    app.UseSwagger();
    //app.UseSwaggerUI();
}

//cors
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
