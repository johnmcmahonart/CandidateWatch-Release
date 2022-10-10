using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RESTApi.Controllers
{
    //strings are submitted from rtkquery in format "candidateId1","candidateId2",etc
    //this binder handles spliting the string by seperator and converting to list
    public class StringArrayBinder : IModelBinder
    {
        private ILogger<StringArrayBinder> _logger;

        public StringArrayBinder(ILogger<StringArrayBinder> logger)
        {
            _logger = logger;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.FieldName).FirstValue;
            if (value == null || string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            try
            {
                List<string> result = value.Split(',').ToList();
                List<string> resultTrimmed = new();
                foreach (var candidate in result)
                {
                    resultTrimmed.Add(candidate.Trim('"'));
                }
                //var deserializedJson = JsonSerializer.Deserialize(value, bindingContext.ModelType, new JsonSerializerOptions(JsonSerializerDefaults.Web)

                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to bind '{FieldName}'", bindingContext.FieldName, ex);
            }
            return Task.CompletedTask;
        }
    }
}