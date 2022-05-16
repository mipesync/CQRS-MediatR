using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using System.Text.RegularExpressions;

namespace CQRS_MediatR.API.Filters
{
    public class IdVerifyAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// Позволяет проверить валидность id паттерну
        /// </summary>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var id = context.HttpContext.Request.RouteValues.Values.Last()?.ToString();

            var pattern = @"^\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";

            if (!Regex.IsMatch(id!, pattern))
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new BadRequestObjectResult(new { message = "The Id is incorrect" });
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Нет реализации
        }
    }
}
