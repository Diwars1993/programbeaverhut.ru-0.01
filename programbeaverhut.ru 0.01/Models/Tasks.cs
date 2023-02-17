using System;

namespace programbeaverhut.ru.Models
{
    // Задачи
    public class Tasks
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Employees { get; set; } // Заголовок или имя
        public string Description { get; set; } // Описание

        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        public string UserName { get; set; }
        public string UserId1 { get; set; }
        public User User { get; set; }

        public string TaskGroupHiName { get; set; }
        public int TaskGroupHiId { get; set; }
        public TaskGroupHi TaskGroupHi { get; set; }
    }
}
