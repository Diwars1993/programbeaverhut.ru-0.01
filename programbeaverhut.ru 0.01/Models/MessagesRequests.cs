using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class MessagesRequests // Это сообщения в запросах к модераторам
    {
        public int MessagesRequestsId { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }

        public string RequestHiName { get; set; }
        public int RequestHiId1 { get; set; }
        public RequestHi RequestHi { get; set; }

        public string UserName { get; set; }
        public string UserId1 { get; set; }
        public User User { get; set; }
    }
}
