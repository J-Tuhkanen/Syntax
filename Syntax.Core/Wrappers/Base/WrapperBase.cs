using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syntax.Core.Wrappers.Base
{
    public class WrapperBase<T>
    {
        public T Model { get; }

        public WrapperBase(T model)
        {
            Model = model;
        }
    }
}
