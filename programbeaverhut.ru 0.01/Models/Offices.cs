using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class Offices
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }

        public string UserName { get; set; }
        public string UserId1 { get; set; }
        public User User { get; set; }

        public List<Client> Clients { get; set; }
        public Offices()
        {
            Clients = new List<Client>();
        }

    }
}
