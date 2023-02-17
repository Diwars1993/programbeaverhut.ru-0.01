using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class Contract
    {
        public int ContractId { get; set; } // Обрати внимание в отличии от класса модели где нет ссылки сдесь не просто Id
        public string Name { get; set; } // Название
        public string TitleName { get; set; } // Загаловок
        public string Address { get; set; } // Адрес заключения договора
        public string ItemOne { get; set; } // Все остальное в пункте 1
        public string ItemTwo { get; set; }// Все остальное в пункте 2
        public string ItemThree { get; set; } // пункте 3
        public string ItemFour { get; set; } // пункте 4
        public string ItemFive { get; set; } // пункте 5
        public string ItemSix { get; set; } // пункте 6
        public string ItemSeven { get; set; } // пункте 7
        public string ItemEight { get; set; } // пункте 8
        public string ItemNine { get; set; } // пункте 9
        public string ItemTen { get; set; } // пункте 10

        public string TermsUse { get; set; } // ПРАВИЛА ПОЛЬЗОВАНИЯ
        public string AcceptanceCertificate { get; set; } // АКТ ПРИЕМА-ПЕРЕДАЧИ

        public string NameReportingPeriod { get; set; }
        public int ReportingPeriodId { get; set; }
        public ReportingPeriod ReportingPeriod { get; set; }
        public string UserId1 { get; set; }
        public User User { get; set; }

    }
}
