using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    // Загурузить файл в чате
    public class FileModel
    {
        public int FileModelId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public int Chat1Id { get; set; }
        public Chat1 Chat1 { get; set; }

        public string UserId1 { get; set; }
        public User User { get; set; }
    }
}
