using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string NameCategory { get; set; }

        public string UserName { get; set; }
        public string UserId1 { get; set; }
        public User User { get; set; }

        public List<Client> Clients { get; set; }
        public Category()
        {
            Clients = new List<Client>();
        }
    }
}
