using STPresenceControl.Enums;
using System;

namespace STPresenceControl.Models
{
    public class PresenceControlEntry
    {
        public PresenceControlEntryTypeEnum? Type { get; set; }
        public DateTime Date { get; set; }
    }
}
