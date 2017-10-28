using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTypeCreation.Contracts
{
    //dummy code
    public interface IPerson
    {
        string Name { get; set; }

        Person Person { get; set; }
    }
}
