using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class EditUserViewModel
    {
        // Для Управление пользователями (Админка UsersController : Controller)
        public string Id { get; set; }
        public string Email { get; set; }
        public int Year { get; set; }
    }
}
