using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class PbhContext : IdentityDbContext<User>
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Offices> Officess { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<ColorSelection> ColorSelections { get; set; }
        public DbSet<ServiceName> ServiceNames { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ReportingPeriod> ReportingPeriods { get; set; }
        public DbSet<LegalEntity> LegalEntitys { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Tasks> Taskss { get; set; }
        public DbSet<TaskGroupHi> TaskGroupHis { get; set; }
        public DbSet<Chat1> Chat1s { get; set; }
        public DbSet<RequestHi> RequestHis { get; set; }
        public DbSet<MessagesRequests> MessagesRequestss { get; set; }
        public DbSet<FileModel> FileModels { get; set; }
        public DbSet<FilesClient> FilesClients { get; set; }
        public DbSet<Staff> Staffs { get; set; }

        // По умолчанию у нас база данных отсутствуют.
        // Поэтому в конструктор MobileContext определен вызов Database.EnsureCreated(), который при отсутствии базы данных автоматически создает ее.
        // Если база данных уже есть, то ничего не происходит.
        public PbhContext(DbContextOptions<PbhContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
