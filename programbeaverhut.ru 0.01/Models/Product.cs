using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class Product
    {
        public int ProductId { get; set; } // Обрати внимание в отличии от класса модели где нет ссылки сдесь не просто Id
        public string Description { get; set; } // Описание
        public string Colour { get; set; } // Цвет
        public string Glass { get; set; } // Стекло
        public decimal Quantity { get; set; } // Количество
        public decimal Price { get; set; }// Цена
        public decimal Amount { get; set; } // Cумма
        public decimal Discount { get; set; } // Скидка

        public int ClientId { get; set; } // ссылка на клиента
        public Client Client { get; set; }
    }
}
