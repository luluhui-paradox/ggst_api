using ggst_api.entity;
using ggst_api.Services;
using PostSharp.Aspects;
using PostSharp.Serialization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ggst_api.aspect
{

    public class Update2kafkaFilter : IActionFilter
    {

        private ResultUpdate _resultUpdate;

        public Update2kafkaFilter(ResultUpdate resultUpdate) { 
            _resultUpdate=resultUpdate;
        }
        
        public ResultUpdate resultUpdate {
            get { return _resultUpdate; }
            set { _resultUpdate = value; }
         }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var result=(ObjectResult)context.Result;
            Console.WriteLine($"-----context.mes: {result.Value}");
            List<PlayerInfoEntity> resultlist=(List<PlayerInfoEntity>) result.Value;
            //调用方法
            resultUpdate.sendSync(resultlist);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }

    }
}
