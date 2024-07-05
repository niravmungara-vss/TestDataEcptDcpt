using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualBasic;
using System.Reflection;
using System.Web.Http.Filters;
using TestDataEcptDcpt.Attribute;
using TestDataEcptDcpt.Common;
using TestDataEcptDcpt.Models;
using IActionFilter = Microsoft.AspNetCore.Mvc.Filters.IActionFilter;

namespace TestDataEcptDcpt.Filters
{
    public class EncryptionDecryptionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var attrib = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes<EncryptionDecryptionAttr>().FirstOrDefault();
            if (attrib is null) return;

            if (context.Result is Microsoft.AspNetCore.Mvc.OkObjectResult)
            {
                var result = ((List<UserModel>)((Microsoft.AspNetCore.Mvc.OkObjectResult)context.Result).Value);
                foreach (var item in result)
                {
                    try
                    {
                        item.Email = EncryptDecrypt.Decrypt(item.Email);
                    }
                    catch
                    {
                    }
                }

            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var attrib = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes<EncryptionDecryptionAttr>().FirstOrDefault();
            if (attrib is null) return;

            var result = context.ActionArguments;
            if (result != null && result.Values != null && result.Values.Any(t => t.GetType() == typeof(UserModel)))
            {
                foreach (var item in result.Values)
                {
                    if (item.GetType() == typeof(UserModel))
                    {
                        var user = (UserModel)item;
                        ((UserModel)item).Email = EncryptDecrypt.Encrypt(user.Email + "-" + new Random().Next(5555555));
                    }
                }
            }
        }
    }
}
