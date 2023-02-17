using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class Service
    {
        public int ServiceId { get; set; } // Обрати внимание в отличии от класса модели где нет ссылки сдесь не просто Id, а ProductId ЭТО ВАЖНО!
        public string ServiceDescription { get; set; } // Описание
        public decimal ServicePrice { get; set; }// Цена


        public int ClientId { get; set; } // ссылка на клиента
        public Client Client { get; set; }
    }
}
