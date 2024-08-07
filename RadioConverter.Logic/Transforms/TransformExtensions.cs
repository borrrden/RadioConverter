using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioConverter.Logic.Transforms
{
    internal static class TransformExtensions
    {
        public static IReadOnlyDictionary<string, object> ConvertUntypedDict(this IReadOnlyDictionary<string, object> source, string key)
        {
            var untypedDict = source[key] as IReadOnlyDictionary<object, object>;
            if (untypedDict == null)
            {
                throw new ApplicationException("Invalid source in ConvertUntypedDict");
            }

            return untypedDict.ToDictionary(k => (string)k.Key, v => v.Value);
        }

        public static IEnumerable<IReadOnlyDictionary<string, object>> ConvertUntypedListOfDict(this IReadOnlyDictionary<string, object> source, string key)
        {
            var untypedList = source[key] as IReadOnlyList<object>;
            if (untypedList == null)
            {
                throw new ApplicationException("Invalid source in ConvertUntypedListOfDict");
            }

            return untypedList.Where(x => x is IReadOnlyDictionary<object, object>).Select(x => (((IReadOnlyDictionary<object, object>)x).ToDictionary(k => (string)k.Key, v => v.Value)));
        }
    }
}
