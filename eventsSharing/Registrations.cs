//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace eventsSharing
{
    using System;
    using System.Collections.Generic;
    
    public partial class Registrations
    {
        public int eventId { get; set; }
        public int userId { get; set; }
        public System.DateTime date { get; set; }
    
        public virtual Events Events { get; set; }
        public virtual User User { get; set; }
    }
}