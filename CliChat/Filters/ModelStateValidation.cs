using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;
using System.Web.Http.Controllers;
//using System.Web.Http.Filters;

namespace CliChat.Filters
{
    public class ModelStateValidation : ActionFilterAttribute, IFilterMetadata
    {
        //Idk why this does not work
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                // TODO customize returned error 
                throw new ValidationException();
            }
            base.OnActionExecuting(context);
        }
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                // TODO customize returned error 
                throw new ValidationException();
            }
            return base.OnActionExecutionAsync(context, next);
        }
    }
}
