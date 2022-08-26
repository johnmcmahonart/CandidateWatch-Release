using System;
using System.Linq.Expressions;
namespace MDWatch.Utilities
{
    public static class General

    {
        public static string GetMemberName<T, TValue>(Expression<Func<T, TValue>> memberAccess)
        //https://stackoverflow.com/questions/7598968/getting-the-name-of-a-property-in-c-sharp

        {
            return ((MemberExpression)memberAccess.Body).Member.Name;
        }
        
    }
}