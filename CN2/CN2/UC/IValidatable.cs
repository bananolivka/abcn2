using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CN2.Core.DataStructures
{
    public interface IValidatable
    {
        bool IsValid { get; }
    }
}
