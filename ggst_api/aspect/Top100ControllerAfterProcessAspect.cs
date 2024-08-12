using ggst_api.entity;
using ggst_api.Services;
using PostSharp.Aspects;
using PostSharp.Serialization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ggst_api.aspect
{

    public class Update2kafkaFilter : IActionFilter
    {

        private readonly ResultUpdate _resultUpdate;

        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        private readonly ILogger<Update2kafkaFilter> _logger;

        public Update2kafkaFilter(ResultUpdate resultUpdate,IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,ILogger<Update2kafkaFilter> logger) { 

            _resultUpdate=resultUpdate;
            _actionDescriptorCollectionProvider=actionDescriptorCollectionProvider;
            _logger=logger;

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var methodName = actionDescriptor.ActionName;
            _logger.LogInformation(methodName);
            if ("getTop100info".Equals(methodName))
            {
                _resultUpdate.sendTop100();
            }
            else {
                var result = (ObjectResult)context.Result;
                Console.WriteLine($"-----context.mes: {result.Value}");
                List<PlayerInfoEntity> resultlist = (List<PlayerInfoEntity>)result.Value;
                //调用方法
                _resultUpdate.sendSync(resultlist);
            }
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }

    }
}
