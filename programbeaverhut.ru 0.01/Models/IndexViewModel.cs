using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Client> Clients { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public IEnumerable<ReportingPeriod> ReportingPeriods { get; set; }
        public IEnumerable<TaskGroupHi> TaskGroupHi1 { get; set; }
        public IEnumerable<Chat1> Chat11 { get; set; }
        public IEnumerable<Tasks> Tasks1 { get; set; }
        public IEnumerable<FileModel> FileModel1 { get; set; }
        public IEnumerable<FilesClient> FilesClient1 { get; set; }
    }
}
