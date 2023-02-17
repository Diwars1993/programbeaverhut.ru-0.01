using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class ColorSelection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }

        public List<Client> Clients { get; set; }
        public ColorSelection()
        {
            Clients = new List<Client>();
        }
    }
}
