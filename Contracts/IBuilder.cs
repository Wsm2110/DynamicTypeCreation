using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTypeCreation.Contracts
{
   public interface IBuilder
    {
        IBuilder Extend<T>() where T : class;
        
        object Build();

    }
}
