using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Reflection;
using TestDataEcptDcpt.Attribute;
using TestDataEcptDcpt.Common;
using TestDataEcptDcpt.Models;

namespace TestDataEcptDcpt.Helpers
{
    public class ModelBinder : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            var subclasses = new[] { typeof(UserModel) };
            var binders = new Dictionary<Type, (ModelMetadata, IModelBinder)>();

            foreach (var type in subclasses)
            {
                var modelMetadata = context.MetadataProvider.GetMetadataForType(type);
                binders[type] = (modelMetadata, context.CreateBinder(modelMetadata));
            }

            return new Binder(binders);
        }
    }

    public class Binder : IModelBinder
    {
        private Dictionary<Type, (ModelMetadata, IModelBinder)> binders;

        public Binder(Dictionary<Type, (ModelMetadata, IModelBinder)> binders)
        {
            this.binders = binders;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelType.Name;

            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            string valueFromBody = string.Empty;
            using (var sr = new StreamReader(bindingContext.HttpContext.Request.Body))
            {
                if (modelName == nameof(UserModel))
                {
                    var result = JsonConvert.DeserializeObject<UserModel>(sr.ReadToEndAsync().Result);
                    PropertyInfo[] props = result.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        //object[] attrs = prop.GetCustomAttributes<EncryptionDecryptionAttr>().ToArray();
                        if (prop.GetCustomAttributes().Any(t => t.GetType() == typeof(EncryptionDecryptionAttr)))
                        {
                            prop.SetValue(result, EncryptDecrypt.Encrypt(prop.GetValue(result, null) + "-" + new Random().Next(5555555)), null);
                        }
                    }
                    valueFromBody = JsonConvert.SerializeObject(result);
                    bindingContext.Result = ModelBindingResult.Success(result);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }
}
