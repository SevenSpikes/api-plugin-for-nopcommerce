using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nop.Plugin.Api.Extensions
{
    internal static class QueryParamsExtensions
    {
        internal static Dictionary<string, string> EnsureSearchQueryIsValid(this string query, Func<string, Dictionary<string, string>> parseSearchQuery)
        {
            if (!string.IsNullOrEmpty(query))
            {
                return parseSearchQuery(query);
            }

            return null;
        }

        internal static Dictionary<string, string> ParseSearchQuery(this string query)
        {
            var parsedQuery = new Dictionary<string, string>();

            var splitPattern = @"(\w+):";

            var fieldValueList = Regex.Split(query, splitPattern).Where(s => s != String.Empty).ToList();

            if (fieldValueList.Count < 2)
            {
                return parsedQuery;
            }

            for (var i = 0; i < fieldValueList.Count; i += 2)
            {
                var field = fieldValueList[i];
                var value = fieldValueList[i + 1];

                if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(value))
                {
                    field = field.Replace("_", string.Empty);
                    parsedQuery.Add(field.Trim(), value.Trim());
                }
            }

            return parsedQuery;
        }
    }
}
