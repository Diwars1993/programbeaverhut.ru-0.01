using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class Staff
    {
        // Сотрудники 
        public int Id { get; set; }
        public string StaffName { get; set; }
        public string StaffTelephone { get; set; }

        public string UserName { get; set; }
        public string UserId1 { get; set; }
        public User User { get; set; }


        public List<Tasks> Tasks1 { get; set; }
        public Staff()
        {
            Tasks1 = new List<Tasks>();
        }
    }
}
