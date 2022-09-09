using System;
using System.Collections.Generic;
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
        
        public static IEnumerable<string> FixCandidateName(this string name)
        //fixes formatting of name from FEC Api
        {
            List<string> outString = new();
            var allLower = name.ToLower();
            //check for bad data in name
            if (name.Contains(","))
            {
                outString.Add(allLower.Split(',')[1]); //first name
                outString[0] = outString[0].Remove(0, 1); //remove space from begining of name
                outString.Add(allLower.Split(',')[0]); //last name
                                                       //capitalize first letter of each name
                outString[0] = char.ToUpper(outString[0][0]) + outString[0].Substring(1);
                outString[1] = char.ToUpper(outString[1][0]) + outString[1].Substring(1);
                return outString;
            }
            else
            {
                return new List<string>() { name };
            }
            

        }
    }
}