using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public string ContractNumber { get; set; } // Номер договора
       
        public string SNM { get; set; } // ФИО клиента
        public string Address { get; set; } // Адресс 
        public string Telephone { get; set; } // Телефон 
        public string Date { get; set; } // Дата 
        public string PassportData { get; set; } // Паспортные данные
        public string ClientINN { get; set; } // ИНН
        public string ClientOGRIP { get; set; } // ОГРИП
        public decimal PayGoods { get; set; } // Оплата товара
        public decimal RemainingСostGoods { get; set; } // Остаток стоимости за товар
        public decimal AmountGoods { get; set; } // Сумма стоимости за товар
        public decimal PayService { get; set; } // Оплата услуг
        public decimal RemainingСostService { get; set; } // Остаток стоимости за услуги
        public decimal AmountService { get; set; } // Сумма стоимости за услуги
        public string OrderAssemblyStage { get; set; } // Этап сборки

        public int ColorId { get; set; }
        public string NameColor { get; set; }
        public ColorSelection ColorSelection { get; set; }
        public int LegalEntityId { get; set; }
        public string NameLegalEntity { get; set; }
        public LegalEntity LegalEntity { get; set; }
        public int CategoryId { get; set; }
        public string NameCategory { get; set; }
        public Category Category { get; set; }
        public int OfficesId { get; set; }
        public string Name { get; set; }
        public Offices Offices { get; set; }
        public string UserName { get; set; }
        public string UserId1 { get; set; }
        public User User { get; set; }

        public string Manager { get; set; } // Манеджер
        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        public int ReportingPeriodId { get; set; }
        public ReportingPeriod ReportingPeriod { get; set; }
    }
}
