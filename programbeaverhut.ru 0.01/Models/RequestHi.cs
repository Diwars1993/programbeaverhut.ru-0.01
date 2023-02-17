using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class RequestHi
    {
        // Запрос в чате поддержки
        public int Id { get; set; }
        public string Topic { get; set; }
        public int UnreadUser { get; set; } // Непрочитанные сообщения пользователя администратором
        public int UnreadAdmin { get; set; } // Непрочитаные пользователям сообщения администратора

        public string UserName { get; set; }
        public string UserId1 { get; set; }
        public User User { get; set; }
    }
}
