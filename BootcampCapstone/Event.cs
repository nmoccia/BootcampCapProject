//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BootcampCapstone
{
    using System;
    using System.Collections.Generic;
    [System.ComponentModel.DataAnnotations.MetadataType(typeof(EventMetaData))]
    public partial class Event
    {
        public Event()
        {
            this.Registrations = new HashSet<Registration>();
        }
    
        public int eventID { get; set; }
        public string title { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        public Nullable<int> categoryID { get; set; }
        public Nullable<int> typeID { get; set; }
        public string eventDescription { get; set; }
        public Nullable<int> ownerID { get; set; }
        public string logoPath { get; set; }
        public string location { get; set; }
        public string eventStatus { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Type Type { get; set; }
        public virtual ICollection<Registration> Registrations { get; set; }
    }
}
