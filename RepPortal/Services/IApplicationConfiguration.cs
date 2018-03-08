using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepPortal.Services
{
    public interface IApplicationConfiguration
    {
        string GoogleAPIKey { get; set; }
    }
}
