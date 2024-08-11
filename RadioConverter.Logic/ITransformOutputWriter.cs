using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioConverter.Logic
{
    public interface ITransformOutputWriter
    {
        Task WriteToAsync(IReadOnlyList<string> keys, IReadOnlyList<IReadOnlyDictionary<string, string>> values, Stream output);
    }
}
