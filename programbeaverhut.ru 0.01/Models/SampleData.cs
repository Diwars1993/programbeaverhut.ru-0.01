using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Models
{
    public class SampleData
    {
        public static void Initialize(PbhContext context)
        {
            if (!context.ColorSelections.Any())
            {
                context.ColorSelections.AddRange(
                    new ColorSelection
                    {
                        Name = "Без выделения",
                        Number = "",

                    },
                    new ColorSelection
                    {
                        Name = "Красный",
                        Number = "#FA8072",
                        
                    },
                    new ColorSelection
                    {
                        Name = "Желтый",
                        Number = "#FFFF00",
                        
                    },
                    new ColorSelection
                    {
                        Name = "Зеленый",
                        Number = "#98FB98",
                        
                    }
                );
                context.SaveChanges();
            }
            if (!context.Officess.Any())
            {
                context.Officess.AddRange(
                    new Offices
                    {
                        Name = "Офис был удален",
                        Address = "Офис был удален",
                        Telephone = "Офис был удален",
                    }
                );
                context.SaveChanges();
            }
            if (!context.Categorys.Any())
            {
                context.Categorys.AddRange(
                    new Category
                    {
                        NameCategory = "Категория была удалена",
                    }
                );
                context.SaveChanges();
            }
            if (!context.ServiceNames.Any())
            {
                context.ServiceNames.AddRange(
                    new ServiceName
                    {
                        ServName = "Услуга была удалена",
                    }
                );
                context.SaveChanges();
            }
            if (!context.LegalEntitys.Any())
            {
                context.LegalEntitys.AddRange(
                    new LegalEntity
                    {
                       LegalEntityName = "Компания была удалена",
                       LegalEntityINN = "Компания была удалена",
                       LegalEntityOGRIP = "Компания была удалена",
                       Telephone = "Компания была удалена",
                       Address = "Компания была удалена",
                    }
                );
                context.SaveChanges();
            }
            if (!context.Staffs.Any())
            {
                context.Staffs.AddRange(
                    new Staff
                    {
                        StaffName = "Сотрудник был удален",
                        StaffTelephone = "Сотрудник был удален",
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
