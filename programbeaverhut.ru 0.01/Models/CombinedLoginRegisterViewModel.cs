using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    // // Это все нужно для того чтобы было Несколько моделей в одном представлении "Настройки" в MVC https://translated.turbopages.org/proxy_u/en-ru.ru.79cbbd31-62a37310-6f89da8b-74722d776562/https/www.c-sharpcorner.com/uploadfile/ff2f08/multiple-models-in-single-view-in-mvc/
    public class CombinedLoginRegisterViewModel
    {
        public IEnumerable<Product> Product1 { get; set; }
        public IEnumerable<Category> Categorys1 { get; set; }
        public IEnumerable<Offices> Officess1 { get; set; }
        public IEnumerable<Client> Clients1 { get; set; }
        public IEnumerable<ServiceName> ServiceName1 { get; set; }
        public IEnumerable<Service> Service1 { get; set; }
        public IEnumerable<ReportingPeriod> ReportingPeriod1 { get; set; }
        public IEnumerable<LegalEntity> LegalEntity1 { get; set; }
        public IEnumerable<Contract> Contract1 { get; set; }
        
        public IEnumerable<TaskGroupHi> TaskGroupHi1 { get; set; }
        public IEnumerable<Chat1> Chat11 { get; set; }
        public IEnumerable<RequestHi> RequestHi1 { get; set; }
        public IEnumerable<MessagesRequests> MessagesRequests1 { get; set; }
        public IEnumerable<User> User1 { get; set; }
        public IEnumerable<Staff> Staff1 { get; set; }

        // Фильтрация по сотрудниу
        public IEnumerable<Tasks> Tasks1 { get; set; }
        public SelectList Staffss { get; set; } 
        public string Name { get; set; }
    }
}

