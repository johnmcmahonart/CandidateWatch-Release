using AutoMapper;

namespace RESTApi.Mapper
{
    internal static class MapperHelper
    {
        public static IEnumerable<Tout> MapIEnumerable<Tin, Tout>(IEnumerable<Tin> inputObj, IMapper mapper)
        {
            List<Tout> outObj = new();
            foreach (var item in inputObj)
            {
                outObj.Add(mapper.Map<Tout>(item));
            }
            return outObj;
        }
    }
}