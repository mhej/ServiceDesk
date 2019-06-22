using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Data
{
    /// <summary>Requires Initialize method.</summary>
    public interface IDBInitializer
    {
        void Initialize();
    }
}
