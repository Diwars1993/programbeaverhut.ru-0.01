using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// (Регистрация) Добавление Identity в проект с нуля https://metanit.com/sharp/aspnet5/16.2.php
namespace programbeaverhut.ru.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }

        public int UnreadUser { get; set; }
    }
}
