using System;
using System.Collections.Generic;
using System.Text;

namespace Parser.Interface
{
    public interface IActionResolver
    {
        void Resolve(IList<string> files);
    }
}
