using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace IPS_Editor
{
    public class IPSEntrySavedArgs : EventArgs
    {
        public IPSEntry Data { get; set; }
    }
}
