using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class Chat1
    {
        public int Chat1Id { get; set; }
        public string Chat1Date { get; set; }
        public string NameUser { get; set; }
        public string Message { get; set; }

        public string UserName { get; set; }
        public string UserId1 { get; set; }
        public User User { get; set; }

        public int ReportingPeriodId { get; set; }
        public ReportingPeriod ReportingPeriod { get; set; }
    }
}
