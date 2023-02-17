using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class FilesClient
    {
        public int FilesClientId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public string UserId1 { get; set; }
        public User User { get; set; }

        public int? ClientId { get; set; }
        public Client Client { get; set; }
    }
}
