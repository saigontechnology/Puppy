using System;
using System.Collections.Generic;
using System.Linq;

namespace Puppy.DataTable
{
    public class LengthMenuVm : List<Tuple<string, int>>
    {
        public override string ToString()
        {
            return "[[" + string.Join(", ", this.Select(pair => pair.Item2)) + "],[\"" + string.Join("\", \"", this.Select(pair => pair.Item1.Replace("\"", "\"\""))) + "\"]]";
        }
    }
}