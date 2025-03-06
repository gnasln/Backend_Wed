using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wed.Domain.Constants
{
    public enum FacilityStatus
    {
        [Description("Available")]
        Available = 1,

        [Description("Under Maintenance")]
        Maintenance = 2,

        [Description("Closed")]
        Closed = 3
    }
}
