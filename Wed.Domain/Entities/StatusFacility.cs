

using System.ComponentModel;

namespace Wed.Domain.Entities
{
    public enum StatusFacility
    {
        [Description("Available")]
        Available = 1,

        [Description("Under MainTenance")]
        MainTenance = 2,

        [Description("Closed")]
        Closed = 3
    }
}
