//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SchoolSchedule
{
    using System;
    using System.Collections.Generic;
    
    public partial class Schedule
    {
        public int Id { get; set; }
        public int IdLesson { get; set; }
        public System.TimeSpan StartTime { get; set; }
        public System.TimeSpan EndTime { get; set; }
        public System.DateTime Date { get; set; }
        public int IdTeacher { get; set; }
    
        public virtual Lesson Lesson { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}
