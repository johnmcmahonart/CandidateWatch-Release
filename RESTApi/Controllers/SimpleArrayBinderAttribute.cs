using Microsoft.AspNetCore.Mvc;

namespace RESTApi.Controllers
{
    public class StringArrayBinderAttribute:ModelBinderAttribute
    {
    public StringArrayBinderAttribute()
        {
            BinderType=typeof(StringArrayBinder);
        }

    }
}
