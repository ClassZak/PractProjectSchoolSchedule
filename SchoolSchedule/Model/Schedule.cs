//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SchoolSchedule.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Schedule
    {
        public int Id { get; set; }
        public int IdSubject { get; set; }
        public int IdGroup { get; set; }
        public int IdTeacher { get; set; }
        public int IdBellSchedule { get; set; }
        public int DayOfTheWeek { get; set; }
        public int ClassRoom { get; set; }
    
        public virtual BellSchedule BellSchedule { get; set; }
        public virtual Group Group { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}
