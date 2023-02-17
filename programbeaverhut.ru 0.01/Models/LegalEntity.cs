using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    // Компания в настройках
    public class LegalEntity
    {
        public int Id { get; set; }
        public string LegalEntityName { get; set; }
        public string LegalEntityINN { get; set; }
        public string LegalEntityOGRIP { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }

        public string UserName { get; set; }
        public string UserId1 { get; set; }
        public User User { get; set; }

        public List<Client> Clients { get; set; }
        public LegalEntity()
        {
            Clients = new List<Client>();
        }
    }
}
