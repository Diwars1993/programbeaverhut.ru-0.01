using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class TaskGroupHi
    {
        public int TaskGroupHiId { get; set; }
        public string NameTaskGroup { get; set; }

        public string UserName { get; set; }
        public string UserId1 { get; set; }
        public User User { get; set; }
    }
}
