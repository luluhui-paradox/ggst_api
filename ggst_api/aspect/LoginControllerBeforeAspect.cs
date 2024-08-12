using ggst_api.entity;
using ggst_api.Services;
using PostSharp.Aspects;
using PostSharp.Serialization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using ggst_api.utils;

namespace ggst_api.aspect
{

    public class LoginControllerBeforeAspect : IActionFilter
    {

        private readonly ResultUpdate _resultUpdate;

        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        private readonly ILogger<LoginControllerBeforeAspect> _logger;

        private readonly IConnectionMultiplexer _redisConn;

        private readonly TokenGenUtil _tokenGenUtil;

        public LoginControllerBeforeAspect(ResultUpdate resultUpdate,IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,ILogger<LoginControllerBeforeAspect> logger,IConnectionMultiplexer connectionMultiplexer,TokenGenUtil tokenGenUtil) { 

            _resultUpdate=resultUpdate;
            _actionDescriptorCollectionProvider=actionDescriptorCollectionProvider;
            _logger=logger;
            _redisConn=connectionMultiplexer;
            _tokenGenUtil=tokenGenUtil;

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //// 从请求头中获取 token
            //if (!context.HttpContext.Request.Headers.TryGetValue("token", out var token))
            //{
            //    // 如果不存在 token，则返回 401 Unauthorized
            //    //context.Result = new UnauthorizedResult();

            //}
            //else
            //{
            //    // 这里可以添加其他逻辑来验证 token，例如：
            //    //此处获取

            //}
            //print header
            context.HttpContext.Request.Headers.TryGetValue("Authorization", out var auth);
            _logger.LogInformation($"---------------auth:{auth}--------------------");
            //进行jwt验证
            _logger.LogInformation($"-------------res:{_tokenGenUtil.ValidateToken(auth)}---------------");

        }

    }
}
