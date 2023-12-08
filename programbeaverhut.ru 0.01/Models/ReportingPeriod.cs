using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class ReportingPeriod
    {
        // Отчетный период
        public int Id { get; set; }

        [Required(ErrorMessage = "  • Укажите название группы клиентов")]
        public string NameReportingPeriod { get; set; }
        
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "  • Пароли не совпадают")]
        public string PasswordVeri { get; set; }

        public string AppliedPassword { get; set; } // Пароль ввода

        public string VerificationPassword { get; set; } //Проверка пароля (Сравнивающий пароль)
        public string OldPassword { get; set; } //Проверка пароля (Старый пароль)
        public string NewPassword { get; set; } //Проверка пароля (Новый пароль)

        public int ColorId { get; set; }
        public string NameColor { get; set; }
        public ColorSelection ColorSelection { get; set; }
        public string UserName { get; set; }
        public string UserId1 { get; set; }
        public User User { get; set; }
    }
}
