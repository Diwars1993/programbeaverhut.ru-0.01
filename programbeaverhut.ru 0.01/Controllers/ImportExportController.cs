using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using programbeaverhut.ru.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace programbeaverhut.ru.Controllers
{
    public class ImportExportController : Controller
    {

        private PbhContext db;
        private IHostEnvironment _hostingEnvironment;
        public ImportExportController(IHostEnvironment hostingEnvironment, PbhContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            db = context;
        }

        // Скачать документы
        public async Task<IActionResult> ImptExp(int? id, int? id1)
        {
            // Условный оператор IF условие если ID = null то возращаеться на главную
            if (id == null)
            return RedirectToAction("Index");

            ViewBag.ClientId1 = id1; // Группа клиентов

            foreach (Client client in db.Clients)
           {
                if (id == client.ClientId)
                {
                    // ViewBag представляет такой объект, который позволяет определить любую переменную
                    // и передать ей некоторое значение, а затем в представлении извлечь это значение
                    //ViewBag.ClientSNM = client.SNM;
                    ViewBag.ClientID = id;
                }
           }
            // Это все нужно для того чтобы было Несколько моделей в одном представлении в MVC (Сдесь это может пригодиться)
            CombinedLoginRegisterViewModel mymodel = new CombinedLoginRegisterViewModel();
            mymodel.Contract1 = db.Contracts;
            mymodel.ReportingPeriod1 = db.ReportingPeriods;
            mymodel.Clients1 = db.Clients;

            return View(mymodel);
        }


        // Конструктор договоров
        [HttpGet]
        [ActionName("AddContract")]
        public async Task<IActionResult> ConfirmAddContract(int? id)
        {

            if (id != null)
            {
                Client user = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == id);
                if (user != null)
                {
                    // ViewBag представляет такой объект, который позволяет определить любую переменную
                    // и передать ей некоторое значение, а затем в представлении извлечь это значение
                    //ViewBag.ClientSNM = client.SNM;
                    ViewBag.ContractNumber = user.ContractNumber;
                    ViewBag.Date = user.Date;
                    ViewBag.SNM = user.SNM;
                    ViewBag.NameLegalEntity = user.NameLegalEntity;
                    ViewBag.NameCategory = user.NameCategory;
                    ViewBag.AmountGoods = user.AmountGoods;
                    ViewBag.PassportData = user.PassportData;
                    ViewBag.ClientINN = user.ClientINN;
                    ViewBag.ClientOGRIP = user.ClientOGRIP;
                    ViewBag.Address1 = user.Address;
                    ViewBag.Telephone1 = user.Telephone;
                    ViewBag.UserId1 = user.UserId1;
                    ViewBag.ReportingPeriodId = user.ReportingPeriodId;

                    // Это мы пердоем имя ОТЧОТНОГО ПЕРИОДА
                    ReportingPeriod reportingPeriod = await db.ReportingPeriods.FirstOrDefaultAsync(p => p.Id == user.ReportingPeriodId);
                    ViewBag.NameReportingPeriod = reportingPeriod.NameReportingPeriod;
                    

                }
                LegalEntity legalEntity = await db.LegalEntitys.FirstOrDefaultAsync(p => p.Id == user.LegalEntityId);
                {
                    ViewBag.LegalEntityName = legalEntity.LegalEntityName;
                    ViewBag.LegalEntityINN = legalEntity.LegalEntityINN;
                    ViewBag.LegalEntityOGRIP = legalEntity.LegalEntityOGRIP;
                    ViewBag.Telephone = legalEntity.Telephone;
                    ViewBag.Address = legalEntity.Address;
                }
            }
                    return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddContract(Contract contract, int? id)
        {
            // Это заполняеться поле UserId1 
            //contract.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            Client user = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == id);

            db.Contracts.Add(contract);
            await db.SaveChangesAsync();
           
            return LocalRedirect($"~/ImportExport/ImptExp/{id}?id1={user.ReportingPeriodId}");
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { _hostingEnvironment.ContentRootPath };
        }


        // Бланк заказа
        public async Task<IActionResult> OnPostExport(int? id)
        {
            string sWebRootFolder = _hostingEnvironment.ContentRootPath;
            string sFileName = @"demo.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Demo");
                IRow row = excelSheet.CreateRow(0);

                // Выделение жирным
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints = 11;
                ICellStyle boldStyle = workbook.CreateCellStyle();
                boldStyle.SetFont(font);

                // Только обводка
                IFont font14 = workbook.CreateFont();
                ICellStyle boldStyle14 = workbook.CreateCellStyle();
                boldStyle14.BorderTop = NPOI.SS.UserModel.BorderStyle.Medium;
                boldStyle14.BorderBottom = NPOI.SS.UserModel.BorderStyle.Medium;
                boldStyle14.BorderLeft = NPOI.SS.UserModel.BorderStyle.Medium;
                boldStyle14.BorderRight = NPOI.SS.UserModel.BorderStyle.Medium;
                boldStyle14.SetFont(font14);

                // Обводка, выделение и по центру размещение
                IFont font15 = workbook.CreateFont();
                font15.IsBold = true;
                font15.FontHeightInPoints = 11;
                ICellStyle boldStyle15 = workbook.CreateCellStyle();
                boldStyle15.BorderTop = NPOI.SS.UserModel.BorderStyle.Medium;
                boldStyle15.BorderBottom = NPOI.SS.UserModel.BorderStyle.Medium;
                boldStyle15.BorderLeft = NPOI.SS.UserModel.BorderStyle.Medium;
                boldStyle15.BorderRight = NPOI.SS.UserModel.BorderStyle.Medium;
                // Выравнивание текста по горизонтали и вертикали
                boldStyle15.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                // Вертикальное выравнивание
                boldStyle15.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                boldStyle15.SetFont(font15);

                // Какая строка
                row = excelSheet.CreateRow(7);
                // Какую ячейку выделяем жирным
                ICell cel16 = row.CreateCell(0);
                ICell cel17 = row.CreateCell(1);
                ICell cel18 = row.CreateCell(2);
                ICell cel19 = row.CreateCell(3);
                ICell cel20 = row.CreateCell(4);
                ICell cel21 = row.CreateCell(5);
                ICell cel22 = row.CreateCell(6);
                row.CreateCell(0).SetCellValue("Описание наименования");
                row.CreateCell(1).SetCellValue("Цвет");
                row.CreateCell(2).SetCellValue("Стекло");
                row.CreateCell(3).SetCellValue("К-во");
                row.CreateCell(4).SetCellValue("Цена");
                row.CreateCell(5).SetCellValue("Сумма");
                row.CreateCell(6).SetCellValue("%");
                // Задавание стиля
                cel16.CellStyle = boldStyle15;
                cel17.CellStyle = boldStyle15;
                cel18.CellStyle = boldStyle15;
                cel19.CellStyle = boldStyle15;
                cel20.CellStyle = boldStyle15;
                cel21.CellStyle = boldStyle15;
                cel22.CellStyle = boldStyle15;

                // Создаем стиль ячейки
                ICellStyle cellStyle2 = workbook.CreateCellStyle();
                // Выравнивание текста по горизонтали и вертикали
                cellStyle2.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                // Вертикальное выравнивание
                cellStyle2.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                // Оборачивать
                //cellStyle.WrapText = true;
                // Уменьшаем шрифт и заполняем
                cellStyle2.ShrinkToFit = true;
                // Создаем ячейку 
                ICell Cell2 = excelSheet.CreateRow(6).CreateCell(0);
                // Придаем стиль ячейки
                Cell2.CellStyle = cellStyle2;
                // Устанавливаем значение в ячейку
                Cell2.SetCellValue("Лист Заказа №1");


                // Обьеденение ячеек
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 1, 7));
                // Обьеденение ячеек
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 3, 1, 7));
                // Обьеденение ячеек
                excelSheet.AddMergedRegion(new CellRangeAddress(4, 4, 1, 7));
                // Обьеденение ячеек
                excelSheet.AddMergedRegion(new CellRangeAddress(5, 5, 1, 7));
                // Обьеденение ячеек Лист Заказа №312045
                excelSheet.AddMergedRegion(new CellRangeAddress(6, 6, 0, 7));
                


                foreach (Client client in db.Clients)
                {
                 if (id != null)
                 {
                        //Проверка на кого клиента документы по ID
                   if (id == client.ClientId)
                   {
                        if (User.FindFirstValue(ClaimTypes.NameIdentifier) == client.UserId1)
                        {  
                            // Какая строка
                            row = excelSheet.CreateRow(2);
                            // Какую ячейку выделяем жирным
                            ICell cell = row.CreateCell(0);   
                            row.CreateCell(0).SetCellValue("Клиент: ");
                            row.CreateCell(1).SetCellValue(client.SNM);
                            // Выделение жирным
                            cell.CellStyle = boldStyle;
                            
                            // Какая строка
                            row = excelSheet.CreateRow(3);
                            // Какую ячейку выделяем жирным
                            ICell cel2 = row.CreateCell(0);
                            row.CreateCell(0).SetCellValue("Телефон клиента: ");
                            row.CreateCell(1).SetCellValue(client.Telephone);
                            // Выделение жирным
                            cel2.CellStyle = boldStyle;

                            // Какая строка
                            row = excelSheet.CreateRow(4);
                            // Какую ячейку выделяем жирным
                            ICell cel3 = row.CreateCell(0);
                            row.CreateCell(0).SetCellValue("Адрес клиента: ");
                            row.CreateCell(1).SetCellValue(client.Address);
                            // Выделение жирным
                            cel3.CellStyle = boldStyle;

                            // Какая строка
                            row = excelSheet.CreateRow(5);
                            // Какую ячейку выделяем жирным
                            ICell cel4 = row.CreateCell(0);
                            row.CreateCell(0).SetCellValue("Номер договора: ");
                            row.CreateCell(1).SetCellValue(client.ContractNumber);
                            // Выделение жирным
                            cel4.CellStyle = boldStyle;
                        }
                   }
                 }
                }

                // Общая стоимость продукции
                decimal d = 0;
                // Номер начальной строки
                int j = 8;

                // Выкладка товары
                foreach (Product product in db.Products)
                {
                    if (id != null)
                    {
                        //Проверка на кого клиента заказ по ID
                        if (id == product.ClientId)
                        {
                            
                            // Какая строка
                            row = excelSheet.CreateRow(j);
                            // Высота строки
                            row.HeightInPoints = 60;

                            // Выделение границ
                            ICell cel23 = row.CreateCell(0);
                            row.CreateCell(0).SetCellValue(product.Description);
                            cel23.CellStyle = boldStyle14;

                            ICell cel24 = row.CreateCell(1);
                            row.CreateCell(1).SetCellValue(product.Colour);
                            cel24.CellStyle = boldStyle14;

                            ICell cel25 = row.CreateCell(2);
                            row.CreateCell(2).SetCellValue(product.Glass);
                            cel25.CellStyle = boldStyle14;

                            ICell cel26 = row.CreateCell(3);
                            row.CreateCell(3).SetCellValue((double)product.Quantity);
                            cel26.CellStyle = boldStyle14;

                            ICell cel27 = row.CreateCell(4);
                            row.CreateCell(4).SetCellValue((double)product.Price);
                            cel27.CellStyle = boldStyle14;

                            ICell cel28 = row.CreateCell(5);
                            row.CreateCell(5).SetCellValue((double)product.Amount);
                            cel28.CellStyle = boldStyle14;

                            ICell cel29 = row.CreateCell(6);
                            row.CreateCell(6).SetCellValue((double)product.Discount);
                            cel29.CellStyle = boldStyle14;


                            // Общая стоимость продукции
                            d += +product.Amount;

                            j++;
                            
                        }
                    }    
                }

                
                // Общая стоимость товаров
                // Какая строка
                row = excelSheet.CreateRow(j);
                // Выделение обедененых ячеек
                ICell cel30 = row.CreateCell(0);
                ICell cel31 = row.CreateCell(1);
                ICell cel32 = row.CreateCell(2);
                ICell cel33 = row.CreateCell(3);
                ICell cel34 = row.CreateCell(4);
                ICell cel35 = row.CreateCell(5);
                ICell cel36 = row.CreateCell(6);

                row.CreateCell(5).SetCellValue($"{d}");
                row.CreateCell(0).SetCellValue("Общая стоимость продукции: ");
                // Обьеденение ячеек 
                excelSheet.AddMergedRegion(new CellRangeAddress(j, j, 0, 4));
                // Обьеденение ячеек 
                excelSheet.AddMergedRegion(new CellRangeAddress(j, j, 5, 6));

                cel30.CellStyle = boldStyle15;
                cel31.CellStyle = boldStyle15;
                cel32.CellStyle = boldStyle15;
                cel33.CellStyle = boldStyle15;
                cel34.CellStyle = boldStyle15;
                cel35.CellStyle = boldStyle15;
                cel36.CellStyle = boldStyle15;

                // Номер начальной строки
                int m = j+1;

                // Общая стоимость продукции
                decimal b = 0;

                // Выкладка услуг
                foreach (Service service in db.Services)
                {
                    if (id != null)
                    {
                        //Проверка на кого клиента услуга по ID
                        if (id == service.ClientId)
                        {
                            // Общая стоимость товаров
                            // Какая строка
                            row = excelSheet.CreateRow(m);
                            // Выделение обедененых ячеек
                            ICell cel37 = row.CreateCell(0);
                            ICell cel38 = row.CreateCell(1);
                            ICell cel39 = row.CreateCell(2);
                            ICell cel40 = row.CreateCell(3);
                            ICell cel41 = row.CreateCell(4);
                            ICell cel42 = row.CreateCell(5);
                            ICell cel43 = row.CreateCell(6);

                            row.CreateCell(5).SetCellValue((double)service.ServicePrice);
                            row.CreateCell(0).SetCellValue(service.ServiceDescription);
                            // Обьеденение ячеек 
                            excelSheet.AddMergedRegion(new CellRangeAddress(m, m, 0, 4));
                            // Обьеденение ячеек 
                            excelSheet.AddMergedRegion(new CellRangeAddress(m, m, 5, 6));

                            cel37.CellStyle = boldStyle15;
                            cel38.CellStyle = boldStyle15;
                            cel39.CellStyle = boldStyle15;
                            cel40.CellStyle = boldStyle15;
                            cel41.CellStyle = boldStyle15;
                            cel42.CellStyle = boldStyle15;
                            cel43.CellStyle = boldStyle15;

                            // Общая стоимость продукции
                            d += +service.ServicePrice;

                            m++;

                        }
                    }
                }

                // Общая всего
                // Какая строка
                row = excelSheet.CreateRow(m);
                // Выделение обедененых ячеек
                ICell cel44 = row.CreateCell(0);
                ICell cel45 = row.CreateCell(1);
                ICell cel46 = row.CreateCell(2);
                ICell cel47 = row.CreateCell(3);
                ICell cel48 = row.CreateCell(4);
                ICell cel49 = row.CreateCell(5);
                ICell cel50 = row.CreateCell(6);
                row.CreateCell(5).SetCellValue($"{d+b}");
                row.CreateCell(0).SetCellValue("Сумма вашего заказа: ");
                // Обьеденение ячеек 
                excelSheet.AddMergedRegion(new CellRangeAddress(m, m, 0, 4));
                // Обьеденение ячеек 
                excelSheet.AddMergedRegion(new CellRangeAddress(m, m, 5, 6));
                cel44.CellStyle = boldStyle15;
                cel45.CellStyle = boldStyle15;
                cel46.CellStyle = boldStyle15;
                cel47.CellStyle = boldStyle15;
                cel48.CellStyle = boldStyle15;
                cel49.CellStyle = boldStyle15;
                cel50.CellStyle = boldStyle15;

                
                foreach(Client client1 in db.Clients)
                {
                    if(client1.ClientId == id)
                    {
                        // Какая строка
                        row = excelSheet.CreateRow(m + 2);
                        row.CreateCell(0).SetCellValue($"Дата подписания: {client1.Date}");
                    }
                }

                // Какая строка
                row = excelSheet.CreateRow(m + 3);
                // Обьеденение ячеек 
                excelSheet.AddMergedRegion(new CellRangeAddress(m+3, m+3, 0, 6));
                row.CreateCell(0).SetCellValue("С ценой и описанием листа заказа №1 согласен (на) _________________");
                


                //Здесь мы автоматически изменяем размер первых 100 столбцов рабочего листа в соответствии с их содержимым:
                for (int i = 0; i <= 100; i++) excelSheet.AutoSizeColumn(i);
                
                workbook.Write(fs);
            }

            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }


        // Договор
        public async Task<IActionResult> ExportingContract(int? id, int? id2)
        {
            string sWebRootFolder = _hostingEnvironment.ContentRootPath;
            string sFileName = @"demo.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                if (id != null & id2 != null)
                {
                    Contract contract = await db.Contracts.FirstOrDefaultAsync(p => p.ContractId == id);
                    Client client = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == id2);


                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Demo");
                    IRow row = excelSheet.CreateRow(0);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle2 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle2.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle2.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle2.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell2 = excelSheet.CreateRow(0).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell2.CellStyle = cellStyle2;
                    // Устанавливаем значение в ячейку
                    Cell2.SetCellValue($"{contract.TitleName} №{client.ContractNumber}");
                    // Выделение жирным
                    IFont font = workbook.CreateFont();
                    font.IsBold = true;
                    font.FontHeightInPoints = 15;
                    cellStyle2.SetFont(font);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle3 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle3.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle3.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle3.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell3 = excelSheet.CreateRow(7).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell3.CellStyle = cellStyle3;
                    // Устанавливаем значение в ячейку
                    Cell3.SetCellValue("1.Предмет договора");
                    // Выделение жирным
                    IFont font1 = workbook.CreateFont();
                    font1.IsBold = true;
                    font1.FontHeightInPoints = 12;
                    cellStyle3.SetFont(font1);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle5 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle5.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle5.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle5.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell5 = excelSheet.CreateRow(16).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell5.CellStyle = cellStyle5;
                    // Устанавливаем значение в ячейку
                    Cell5.SetCellValue("2.Стоимость работ и порядок расчетов");
                    // Выделение жирным
                    IFont font5 = workbook.CreateFont();
                    font5.IsBold = true;
                    font5.FontHeightInPoints = 12;
                    cellStyle5.SetFont(font1);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle6 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle6.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle6.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle6.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell6 = excelSheet.CreateRow(27).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell6.CellStyle = cellStyle6;
                    // Устанавливаем значение в ячейку
                    Cell6.SetCellValue("3. Срок выполнения работ");
                    // Выделение жирным
                    IFont font6 = workbook.CreateFont();
                    font6.IsBold = true;
                    font6.FontHeightInPoints = 12;
                    cellStyle6.SetFont(font6);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle15 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle15.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle15.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle15.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell15 = excelSheet.CreateRow(41).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell15.CellStyle = cellStyle15;
                    // Устанавливаем значение в ячейку
                    Cell15.SetCellValue("4. Приемка работы");
                    // Выделение жирным
                    IFont font15 = workbook.CreateFont();
                    font15.IsBold = true;
                    font15.FontHeightInPoints = 12;
                    cellStyle15.SetFont(font15);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle16 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle16.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle16.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle16.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell16 = excelSheet.CreateRow(86).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell16.CellStyle = cellStyle16;
                    // Устанавливаем значение в ячейку
                    Cell16.SetCellValue("5. Права и обязанности сторон");
                    // Выделение жирным
                    IFont font16 = workbook.CreateFont();
                    font16.IsBold = true;
                    font16.FontHeightInPoints = 12;
                    cellStyle16.SetFont(font16);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle17 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle17.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle17.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle17.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell17 = excelSheet.CreateRow(111).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell17.CellStyle = cellStyle17;
                    // Устанавливаем значение в ячейку
                    Cell17.SetCellValue("6. Качество товара");
                    // Выделение жирным
                    IFont font17 = workbook.CreateFont();
                    font17.IsBold = true;
                    font17.FontHeightInPoints = 12;
                    cellStyle17.SetFont(font17);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle19 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle19.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle19.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle19.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell19 = excelSheet.CreateRow(121).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell19.CellStyle = cellStyle19;
                    // Устанавливаем значение в ячейку
                    Cell19.SetCellValue("7. Гарантийный срок");
                    // Выделение жирным
                    IFont font19 = workbook.CreateFont();
                    font19.IsBold = true;
                    font19.FontHeightInPoints = 12;
                    cellStyle19.SetFont(font19);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle22 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle22.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle22.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle22.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell22 = excelSheet.CreateRow(125).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell22.CellStyle = cellStyle22;
                    // Устанавливаем значение в ячейку
                    Cell22.SetCellValue("8. Ответственность сторон и порядок разрешения споров");
                    // Выделение жирным
                    IFont font22 = workbook.CreateFont();
                    font22.IsBold = true;
                    font22.FontHeightInPoints = 12;
                    cellStyle22.SetFont(font22);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle24 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle24.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle24.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle24.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell24 = excelSheet.CreateRow(133).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell24.CellStyle = cellStyle24;
                    // Устанавливаем значение в ячейку
                    Cell24.SetCellValue("9. Форс-мажор.");
                    // Выделение жирным
                    IFont font24 = workbook.CreateFont();
                    font24.IsBold = true;
                    font24.FontHeightInPoints = 12;
                    cellStyle24.SetFont(font24);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle26 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle26.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle26.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle26.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell26 = excelSheet.CreateRow(150).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell26.CellStyle = cellStyle26;
                    // Устанавливаем значение в ячейку
                    Cell26.SetCellValue("Пункт №10. Заключительные положения");
                    // Выделение жирным
                    IFont font26 = workbook.CreateFont();
                    font26.IsBold = true;
                    font26.FontHeightInPoints = 12;
                    cellStyle26.SetFont(font26);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle28 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle28.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle28.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle28.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell28 = excelSheet.CreateRow(162).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell28.CellStyle = cellStyle28;
                    // Устанавливаем значение в ячейку
                    Cell28.SetCellValue("Реквизиты и подписи сторон");
                    // Выделение жирным
                    IFont font28 = workbook.CreateFont();
                    font28.IsBold = true;
                    font28.FontHeightInPoints = 12;
                    cellStyle28.SetFont(font28);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle29 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle29.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle29.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle29.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell29 = excelSheet.CreateRow(180).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell29.CellStyle = cellStyle29;
                    // Устанавливаем значение в ячейку
                    Cell29.SetCellValue("СОГЛАСИЕ НА ОБРАБОТКУ ПЕРСОНАЛЬНЫХ ДАННЫХ");
                    // Выделение жирным
                    IFont font29 = workbook.CreateFont();
                    font29.IsBold = true;
                    font29.FontHeightInPoints = 12;
                    cellStyle29.SetFont(font29);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle4 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle4.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle4.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell4 = excelSheet.CreateRow(4).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell4.CellStyle = cellStyle4;
                    // Устанавливаем значение в ячейку
                    Cell4.SetCellValue($"{client.NameLegalEntity}, именуемый в дальнейшем Исполнитель, с одной стороны, {client.SNM}, именуемый в дальнейшем Заказчик, с другой стороны, заключили настоящий договор о нижеследующем:");

                    // Создаем стиль ячейки
                    ICellStyle cellStyle7 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle7.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle7.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell7 = excelSheet.CreateRow(8).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell7.CellStyle = cellStyle7;
                    // Устанавливаем значение в ячейку
                    Cell7.SetCellValue($"1.1 Заказчик поручает, а Исполнитель обязуется оказать услугу (и) (или) передать в собственность Заказчика товар, который (и) (или) которая в настоящем договоре именуется: {client.NameCategory}.");

                    // Создаем стиль ячейки
                    ICellStyle cellStyle8 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle8.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle8.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell8 = excelSheet.CreateRow(11).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell8.CellStyle = cellStyle8;
                    // Устанавливаем значение в ячейку
                    Cell8.SetCellValue($"1.2 Характеристика товара: ассортимент, точные размеры, количество, цена и другое, указываются в листе заказа № 1 от {client.Date} которые являются неотъемлемой частью договора.");

                    // Создаем стиль ячейки
                    ICellStyle cellStyle9 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle9.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle9.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell9 = excelSheet.CreateRow(14).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell9.CellStyle = cellStyle9;
                    // Устанавливаем значение в ячейку
                    Cell9.SetCellValue($"{contract.ItemOne}");

                    // Создаем стиль ячейки
                    ICellStyle cellStyle10 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle10.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle10.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell10 = excelSheet.CreateRow(17).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell10.CellStyle = cellStyle10;
                    // Устанавливаем значение в ячейку
                    Cell10.SetCellValue($"Стоимость договора без учота услуг, определяется сторонами в листе заказа №1 и составляет: {client.AmountGoods}");

                    // Создаем стиль ячейки
                    ICellStyle cellStyle11 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle11.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle11.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell11 = excelSheet.CreateRow(19).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell11.CellStyle = cellStyle11;
                    // Устанавливаем значение в ячейку
                    Cell11.SetCellValue($"{contract.ItemTwo}");

                    // Создаем стиль ячейки
                    ICellStyle cellStyle12 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle12.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle12.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell12 = excelSheet.CreateRow(28).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell12.CellStyle = cellStyle12;
                    // Устанавливаем значение в ячейку
                    Cell12.SetCellValue($"{contract.ItemThree}");

                    // Создаем стиль ячейки
                    ICellStyle cellStyle13 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle13.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle13.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell13 = excelSheet.CreateRow(42).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell13.CellStyle = cellStyle13;
                    // Устанавливаем значение в ячейку
                    Cell13.SetCellValue($"{contract.ItemFour}");

                    // Создаем стиль ячейки
                    ICellStyle cellStyle14 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle14.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle14.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell14 = excelSheet.CreateRow(87).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell14.CellStyle = cellStyle14;
                    // Устанавливаем значение в ячейку
                    Cell14.SetCellValue($"{contract.ItemFive}");

                    // Создаем стиль ячейки
                    ICellStyle cellStyle18 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle18.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle18.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell18 = excelSheet.CreateRow(112).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell18.CellStyle = cellStyle18;
                    // Устанавливаем значение в ячейку
                    Cell18.SetCellValue($"{contract.ItemSix}");

                    // Создаем стиль ячейки
                    ICellStyle cellStyle20 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle20.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle20.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell20 = excelSheet.CreateRow(122).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell20.CellStyle = cellStyle20;
                    // Устанавливаем значение в ячейку
                    Cell20.SetCellValue($"{contract.ItemSeven}");

                    // Создаем стиль ячейки
                    ICellStyle cellStyle23 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle23.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle23.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell23 = excelSheet.CreateRow(126).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell23.CellStyle = cellStyle23;
                    // Устанавливаем значение в ячейку
                    Cell23.SetCellValue($"{contract.ItemEight}");

                    // Создаем стиль ячейки
                    ICellStyle cellStyle25 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle25.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle25.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell25 = excelSheet.CreateRow(134).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell25.CellStyle = cellStyle25;
                    // Устанавливаем значение в ячейку
                    Cell25.SetCellValue($"{contract.ItemNine}");

                    // Создаем стиль ячейки
                    ICellStyle cellStyle27 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle27.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle27.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell27 = excelSheet.CreateRow(151).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell27.CellStyle = cellStyle27;
                    // Устанавливаем значение в ячейку
                    Cell27.SetCellValue($"{contract.ItemTen}");

                    // Создаем стиль ячейки
                    ICellStyle cellStyle30 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle30.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle30.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell30 = excelSheet.CreateRow(182).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell30.CellStyle = cellStyle30;
                    // Устанавливаем значение в ячейку
                    Cell30.SetCellValue($"Я, {client.SNM}, даю свое согласие на обработку моих персональных данных. Согласие касается фамилии, имени, отчества, данных о поле, дате рождении, гражданстве, типе документа, удостоверяющем личность (его серии, номере, дате и месте выдачи)." +
                        $"Я даю согласие на использование персональных данных исключительно в целях формирования документооборота предприятия, бухгалтерских операций и налоговых отчислений, а также на хранение всех вышеназванных данных на электронных носителях.Также данным согласием я разрешаю сбор моих персональных данных, " +
                        $"их хранение, систематизацию, обновление, использование(в т.ч.передачу третьим лицам для обмена информацией), а также осуществление любых иных действий, предусмотренных действующим законом Российской Федерации." +
                        $"До моего сведения доведено, что {client.NameLegalEntity} гарантирует обработку моих персональных данных в соответствии с действующим законодательством Российской Федерации.Срок действия данного согласия не ограничен.Согласие может быть отозвано в любой момент по моему  письменному заявлению." +
                        $"Подтверждаю, что, давая согласие, я действую без принуждения, по собственной воле и в своих интересах.");


                    // Прижать к правой стороне и переносить слова
                    IFont font0 = workbook.CreateFont();
                    ICellStyle boldStyle = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    boldStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    boldStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    boldStyle.SetFont(font0);

                    // Прижать к левой стороне
                    IFont font2 = workbook.CreateFont();
                    ICellStyle boldStyle2 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    boldStyle2.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                    boldStyle2.SetFont(font2);


                    // Дата и место оформеление договора
                    row = excelSheet.CreateRow(2);
                    // Автоматическое уминьшение (какой столбец)
                    ICell cel20 = row.CreateCell(7);
                    row.CreateCell(7).SetCellValue(client.Date);
                    // Автоматическое уминьшение
                    cel20.CellStyle = boldStyle2;
                    row.CreateCell(0).SetCellValue(contract.Address);

                    // Дата и место оформеление договора
                    row = excelSheet.CreateRow(164);
                    row.CreateCell(5).SetCellValue("Исполнитель");
                    row.CreateCell(0).SetCellValue("Заказчик");

                    // Дата и место оформеление договора
                    row = excelSheet.CreateRow(178);
                    row.CreateCell(5).SetCellValue("_________________/________________/");
                    row.CreateCell(0).SetCellValue("_________________/_______________________/");

                    // Согласие на обработку персональных данных
                    row = excelSheet.CreateRow(198);
                    row.CreateCell(0).SetCellValue($"_________________/{client.SNM}");


                    // Типчная проверка с циклами
                    foreach (LegalEntity legal in db.LegalEntitys)
                    {
                        foreach (Client client1 in db.Clients)
                        {
                            if (client1.ClientId == id2)
                            {
                                if (legal.Id == client1.LegalEntityId)
                                {
                                    //Реквезиты сторон 1
                                    row = excelSheet.CreateRow(166);
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel = row.CreateCell(5);
                                    row.CreateCell(5).SetCellValue("Наименование:");
                                    // Автоматическое уминьшение
                                    cel.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel11 = row.CreateCell(6);
                                    row.CreateCell(6).SetCellValue($"{legal.LegalEntityName}");
                                    // Автоматическое уминьшение
                                    cel11.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel2 = row.CreateCell(0);
                                    row.CreateCell(0).SetCellValue("Наименование:");
                                    // Автоматическое уминьшение
                                    cel2.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel12 = row.CreateCell(1);
                                    row.CreateCell(1).SetCellValue($"{client1.SNM}");
                                    // Автоматическое уминьшение
                                    cel12.CellStyle = boldStyle;

                                }
                            }
                        }
                    }

                    // Типчная проверка с циклами
                    foreach (LegalEntity legal in db.LegalEntitys)
                    {
                        foreach (Client client1 in db.Clients)
                        {
                            if (client1.ClientId == id2)
                            {
                                if (legal.Id == client1.LegalEntityId)
                                {
                                    //Реквезиты сторон 2
                                    row = excelSheet.CreateRow(168);
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel3 = row.CreateCell(5);
                                    row.CreateCell(5).SetCellValue("Адрес:");
                                    // Автоматическое уминьшение
                                    cel3.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel13 = row.CreateCell(6);
                                    row.CreateCell(6).SetCellValue($"{legal.Address}");
                                    // Автоматическое уминьшение
                                    cel13.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel4 = row.CreateCell(0);
                                    row.CreateCell(0).SetCellValue("Пасп. Данные:");
                                    // Автоматическое уминьшение
                                    cel4.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel14 = row.CreateCell(1);
                                    row.CreateCell(1).SetCellValue($"{client1.PassportData}");
                                    // Автоматическое уминьшение
                                    cel14.CellStyle = boldStyle;
                                }
                            }
                        }
                    }

                    // Типчная проверка с циклами
                    foreach (LegalEntity legal in db.LegalEntitys)
                    {
                        foreach (Client client1 in db.Clients)
                        {
                            if (client1.ClientId == id2)
                            {
                                if (legal.Id == client1.LegalEntityId)
                                {
                                    //Реквезиты сторон 3
                                    row = excelSheet.CreateRow(170);
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel5 = row.CreateCell(5);
                                    row.CreateCell(5).SetCellValue("ИНН:");
                                    // Автоматическое уминьшение
                                    cel5.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel15 = row.CreateCell(6);
                                    row.CreateCell(6).SetCellValue($"{legal.LegalEntityINN}");
                                    // Автоматическое уминьшение
                                    cel15.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel6 = row.CreateCell(0);
                                    row.CreateCell(0).SetCellValue("Адрес:");
                                    // Автоматическое уминьшение
                                    cel6.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel16 = row.CreateCell(1);
                                    row.CreateCell(1).SetCellValue($"{client1.Address}");
                                    // Автоматическое уминьшение
                                    cel16.CellStyle = boldStyle;
                                }
                            }
                        }
                    }

                    // Типчная проверка с циклами
                    foreach (LegalEntity legal in db.LegalEntitys)
                    {
                        foreach (Client client1 in db.Clients)
                        {
                            if (client1.ClientId == id2)
                            {
                                if (legal.Id == client1.LegalEntityId)
                                {
                                    //Реквезиты сторон 3
                                    row = excelSheet.CreateRow(172);
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel7 = row.CreateCell(5);
                                    row.CreateCell(5).SetCellValue("ОГРИП:");
                                    // Автоматическое уминьшение
                                    cel7.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel17 = row.CreateCell(6);
                                    row.CreateCell(6).SetCellValue($"{legal.LegalEntityOGRIP}");
                                    // Автоматическое уминьшение
                                    cel17.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel8 = row.CreateCell(0);
                                    row.CreateCell(0).SetCellValue("ИНН:");
                                    // Автоматическое уминьшение
                                    cel8.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel18 = row.CreateCell(1);
                                    row.CreateCell(1).SetCellValue($"{client1.ClientINN}");
                                    // Автоматическое уминьшение
                                    cel18.CellStyle = boldStyle;
                                }
                            }
                        }
                    }

                    // Типчная проверка с циклами
                    foreach (LegalEntity legal in db.LegalEntitys)
                    {
                        foreach (Client client1 in db.Clients)
                        {
                            if (client1.ClientId == id2)
                            {
                                if (legal.Id == client1.LegalEntityId)
                                {
                                    //Реквезиты сторон 4
                                    row = excelSheet.CreateRow(174);
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel9 = row.CreateCell(5);
                                    row.CreateCell(5).SetCellValue("Тел:");
                                    // Автоматическое уминьшение
                                    cel9.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel19 = row.CreateCell(6);
                                    row.CreateCell(6).SetCellValue($"{legal.Telephone}");
                                    // Автоматическое уминьшение
                                    cel19.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel10 = row.CreateCell(0);
                                    row.CreateCell(0).SetCellValue("ОГРИП:");
                                    // Автоматическое уминьшение
                                    cel10.CellStyle = boldStyle;
                                    // Автоматическое уминьшение (какой столбец)
                                    ICell cel21 = row.CreateCell(1);
                                    row.CreateCell(1).SetCellValue($"{client1.ClientOGRIP}");
                                    // Автоматическое уминьшение
                                    cel21.CellStyle = boldStyle;
                                }
                            }
                        }
                    }

                    // Обьеденение ячеек ДАТА
                    excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 7, 8));
                    // Обьеденение ячеек АДРЕС
                    excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 3));
                    // Обьеденение ячеек ПУНКТ №1 предмет договора
                    excelSheet.AddMergedRegion(new CellRangeAddress(7, 7, 0, 8));
                    // Обьеденение ячеек ПУНКТ №2. Стоимость работ и порядок расчетов
                    excelSheet.AddMergedRegion(new CellRangeAddress(16, 16, 0, 8));
                    // Обьеденение ячеек ПУНКТ №3. Срок выполнения работ
                    excelSheet.AddMergedRegion(new CellRangeAddress(27, 27, 0, 8));
                    // Обьеденение ячеек ПУНКТ №4. Приемка работы
                    excelSheet.AddMergedRegion(new CellRangeAddress(41, 41, 0, 8));
                    // Обьеденение ячеек ПУНКТ №5. Права и обязанности сторон
                    excelSheet.AddMergedRegion(new CellRangeAddress(86, 86, 0, 8));
                    // Обьеденение ячеек ПУНКТ №6. Качество товара
                    excelSheet.AddMergedRegion(new CellRangeAddress(111, 111, 0, 8));
                    // Обьеденение ячеек ПУНКТ №7. Гарантийный срок 
                    excelSheet.AddMergedRegion(new CellRangeAddress(121, 121, 0, 8));
                    // Обьеденение ячеек ПУНКТ №8. Ответственность сторон и порядок разрешения споров 
                    excelSheet.AddMergedRegion(new CellRangeAddress(125, 125, 0, 8));
                    // Обьеденение ячеек ПУНКТ №9. Форс-мажор.
                    excelSheet.AddMergedRegion(new CellRangeAddress(133, 133, 0, 8));
                    // Обьеденение ячеек ПУНКТ №10. Заключительные положения.
                    excelSheet.AddMergedRegion(new CellRangeAddress(150, 150, 0, 8));
                    // Обьеденение ячеек ПУНКТ Адреса и реквизиты сторон:
                    excelSheet.AddMergedRegion(new CellRangeAddress(162, 162, 0, 8));

                    // Обьеденение именуемый в дальнейшем Исполнитель
                    excelSheet.AddMergedRegion(new CellRangeAddress(4, 6, 0, 8));
                    
                    // Обьеденение ячеек #1.1
                    excelSheet.AddMergedRegion(new CellRangeAddress(8, 10, 0, 8));
                    // Обьеденение ячеек #1.2
                    excelSheet.AddMergedRegion(new CellRangeAddress(11, 13, 0, 8));
                    // Обьеденение ячеек #1.3
                    excelSheet.AddMergedRegion(new CellRangeAddress(14, 15, 0, 8));
                    // Обьеденение ячеек #2.1
                    excelSheet.AddMergedRegion(new CellRangeAddress(17, 18, 0, 8));
                    // Обьеденение ячеек #2. все остальное
                    excelSheet.AddMergedRegion(new CellRangeAddress(19, 26, 0, 8));
                    // Обьеденение ячеек #3. все
                    excelSheet.AddMergedRegion(new CellRangeAddress(28, 40, 0, 8));
                    // Обьеденение ячеек #4. все
                    excelSheet.AddMergedRegion(new CellRangeAddress(42, 85, 0, 8));
                    // Обьеденение ячеек #5. все
                    excelSheet.AddMergedRegion(new CellRangeAddress(87, 110, 0, 8));
                    // Обьеденение ячеек #6. все
                    excelSheet.AddMergedRegion(new CellRangeAddress(112, 120, 0, 8));
                    // Обьеденение ячеек #7. все
                    excelSheet.AddMergedRegion(new CellRangeAddress(122, 124, 0, 8));
                    // Обьеденение ячеек #8. все
                    excelSheet.AddMergedRegion(new CellRangeAddress(126, 132, 0, 8));
                    // Обьеденение ячеек #9. все
                    excelSheet.AddMergedRegion(new CellRangeAddress(134, 149, 0, 8));
                    // Обьеденение ячеек #10. все
                    excelSheet.AddMergedRegion(new CellRangeAddress(151, 161, 0, 8));
                    // Обьеденение ячеек СОГЛАСИЕ НА ОБРАБОТКУ ПЕРСОНАЛЬНЫХ ДАННЫХ
                    excelSheet.AddMergedRegion(new CellRangeAddress(182, 196, 0, 8));

                    // Обьеденение ячеек ЗАГОЛОВОК
                    excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 8));
                    // Обьеденение ячеек СОГЛАСИЕ НА ОБРАБОТКУ ПЕРСОНАЛЬНЫХ ДАННЫХ
                    excelSheet.AddMergedRegion(new CellRangeAddress(180, 180, 0, 8));
                    excelSheet.AddMergedRegion(new CellRangeAddress(198, 198, 0, 6));

                    // Исполнитель
                    excelSheet.AddMergedRegion(new CellRangeAddress(164, 164, 5, 8));
                    // Заказчик   
                    excelSheet.AddMergedRegion(new CellRangeAddress(164, 164, 0, 4));

                    // Описание исполнителя и заказсчика (Реквезиты сторон)
                    // строка 1
                    excelSheet.AddMergedRegion(new CellRangeAddress(166, 167, 1, 4));
                    excelSheet.AddMergedRegion(new CellRangeAddress(166, 167, 0, 0));
                    excelSheet.AddMergedRegion(new CellRangeAddress(166, 167, 6, 8));
                    excelSheet.AddMergedRegion(new CellRangeAddress(166, 167, 5, 5));
                    // Строка 2
                    excelSheet.AddMergedRegion(new CellRangeAddress(168, 169, 1, 4));
                    excelSheet.AddMergedRegion(new CellRangeAddress(168, 169, 0, 0));
                    excelSheet.AddMergedRegion(new CellRangeAddress(168, 169, 6, 8));
                    excelSheet.AddMergedRegion(new CellRangeAddress(168, 169, 5, 5));
                    // Строка 3
                    excelSheet.AddMergedRegion(new CellRangeAddress(170, 171, 1, 4));
                    excelSheet.AddMergedRegion(new CellRangeAddress(170, 171, 0, 0));
                    excelSheet.AddMergedRegion(new CellRangeAddress(170, 171, 6, 8));
                    excelSheet.AddMergedRegion(new CellRangeAddress(170, 171, 5, 5));
                    // Строка 4
                    excelSheet.AddMergedRegion(new CellRangeAddress(172, 173, 1, 4));
                    excelSheet.AddMergedRegion(new CellRangeAddress(172, 173, 0, 0));
                    excelSheet.AddMergedRegion(new CellRangeAddress(172, 173, 6, 8));
                    excelSheet.AddMergedRegion(new CellRangeAddress(172, 173, 5, 5));
                    // Строка 5
                    excelSheet.AddMergedRegion(new CellRangeAddress(174, 175, 1, 4));
                    excelSheet.AddMergedRegion(new CellRangeAddress(174, 175, 0, 0));
                    excelSheet.AddMergedRegion(new CellRangeAddress(174, 175, 6, 8));
                    excelSheet.AddMergedRegion(new CellRangeAddress(174, 175, 5, 5));
                    // Строка 6
                    excelSheet.AddMergedRegion(new CellRangeAddress(176, 177, 1, 4));
                    excelSheet.AddMergedRegion(new CellRangeAddress(176, 177, 0, 0));
                    excelSheet.AddMergedRegion(new CellRangeAddress(176, 177, 6, 8));
                    excelSheet.AddMergedRegion(new CellRangeAddress(176, 177, 5, 5));
                    // Подпись и расшифровка
                    excelSheet.AddMergedRegion(new CellRangeAddress(178, 178, 0, 4));
                    excelSheet.AddMergedRegion(new CellRangeAddress(178, 178, 5, 8));

                    workbook.Write(fs);
                }
            }


            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }

        // АКТЫ
        public async Task<IActionResult> TermsUse(int? id, int? id2)
        {
            string sWebRootFolder = _hostingEnvironment.ContentRootPath;
            string sFileName = @"demo.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                if (id != null & id2 != null)
                {
                    Contract contract = await db.Contracts.FirstOrDefaultAsync(p => p.ContractId == id);
                    Client client = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == id2);

                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Demo");
                    IRow row = excelSheet.CreateRow(0);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle3 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle3.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle3.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle3.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell3 = excelSheet.CreateRow(0).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell3.CellStyle = cellStyle3;
                    // Устанавливаем значение в ячейку
                    Cell3.SetCellValue("Правила пользования");
                    // Выделение жирным
                    IFont font1 = workbook.CreateFont();
                    font1.IsBold = true;
                    font1.FontHeightInPoints = 12;
                    cellStyle3.SetFont(font1);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle2 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle2.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle2.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell2 = excelSheet.CreateRow(2).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell2.CellStyle = cellStyle2;
                    // Устанавливаем значение в ячейку
                    Cell2.SetCellValue($"{contract.TermsUse}");                   

                    // Создаем стиль ячейки
                    ICellStyle cellStyle1 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle1.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    // Вертикальное выравнивание
                    cellStyle1.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    // Уменьшаем шрифт и заполняем
                    cellStyle1.ShrinkToFit = true;
                    // Создаем ячейку 
                    ICell Cell1 = excelSheet.CreateRow(50).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell1.CellStyle = cellStyle1;
                    // Устанавливаем значение в ячейку
                    Cell1.SetCellValue($"Акт приема-передачи по договору {client.ContractNumber} от {client.Date}");
                    // Выделение жирным
                    IFont font4 = workbook.CreateFont();
                    font4.IsBold = true;
                    font4.FontHeightInPoints = 12;
                    cellStyle1.SetFont(font4);

                    // Создаем стиль ячейки
                    ICellStyle cellStyle5 = workbook.CreateCellStyle();
                    // Выравнивание текста по горизонтали и вертикали
                    cellStyle5.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    // Вертикальное выравнивание
                    cellStyle5.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Distributed;
                    // Создаем ячейку 
                    ICell Cell5 = excelSheet.CreateRow(52).CreateCell(0);
                    // Придаем стиль ячейки
                    Cell5.CellStyle = cellStyle5;
                    // Устанавливаем значение в ячейку
                    Cell5.SetCellValue($"{contract.AcceptanceCertificate}");
                    

                    // Дата и место оформеление договора
                    row = excelSheet.CreateRow(45);
                    row.CreateCell(5).SetCellValue($"{client.NameLegalEntity}");
                    row.CreateCell(0).SetCellValue($"{client.SNM}");
                    row = excelSheet.CreateRow(46);
                    row.CreateCell(0).SetCellValue("С правилами пользования ознакомлен(а):");
                    row = excelSheet.CreateRow(48);
                    row.CreateCell(5).SetCellValue("_________________/________________/");
                    row.CreateCell(0).SetCellValue("_________________/_______________________/");

                    // Обьеденение ячеек ЗАГОЛОВОК Права пользования
                    excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 8));
                    excelSheet.AddMergedRegion(new CellRangeAddress(2, 43, 0, 8));
                    // Подпись и расшифровка
                    excelSheet.AddMergedRegion(new CellRangeAddress(45, 45, 0, 4));
                    excelSheet.AddMergedRegion(new CellRangeAddress(45, 45, 5, 8));
                    excelSheet.AddMergedRegion(new CellRangeAddress(46, 46, 0, 4));
                    excelSheet.AddMergedRegion(new CellRangeAddress(46, 46, 5, 8));
                    excelSheet.AddMergedRegion(new CellRangeAddress(48, 48, 0, 4));
                    excelSheet.AddMergedRegion(new CellRangeAddress(48, 48, 5, 8));


                    // Дата и место оформеление договора
                    row = excelSheet.CreateRow(95);
                    row.CreateCell(5).SetCellValue($"{client.NameLegalEntity}");
                    row.CreateCell(0).SetCellValue($"{client.SNM}");
                    row = excelSheet.CreateRow(96);
                    row.CreateCell(0).SetCellValue("Заказ принял(а):");
                    row.CreateCell(5).SetCellValue("От исполнителя заказ сдал:");
                    row = excelSheet.CreateRow(98);
                    row.CreateCell(5).SetCellValue("_________________/________________/");
                    row.CreateCell(0).SetCellValue("_________________/_______________________/");
                    row = excelSheet.CreateRow(99);
                    row.CreateCell(5).SetCellValue("_____  __________  ______г");
                    row.CreateCell(0).SetCellValue("_____  __________  ______г");

                    // Обьеденение ячеек ЗАГОЛОВОК Акт приемка передача
                    excelSheet.AddMergedRegion(new CellRangeAddress(50, 50, 0, 8));
                    excelSheet.AddMergedRegion(new CellRangeAddress(52, 93, 0, 8));
                    // Подпись и расшифровка
                    excelSheet.AddMergedRegion(new CellRangeAddress(95, 95, 0, 4));
                    excelSheet.AddMergedRegion(new CellRangeAddress(95, 95, 5, 8));
                    excelSheet.AddMergedRegion(new CellRangeAddress(96, 96, 0, 4));
                    excelSheet.AddMergedRegion(new CellRangeAddress(96, 96, 5, 8));
                    excelSheet.AddMergedRegion(new CellRangeAddress(98, 98, 0, 4));
                    excelSheet.AddMergedRegion(new CellRangeAddress(98, 98, 5, 8));
                    excelSheet.AddMergedRegion(new CellRangeAddress(99, 99, 0, 4));
                    excelSheet.AddMergedRegion(new CellRangeAddress(99, 99, 5, 8));

                    workbook.Write(fs);
                }

                using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
            }
        }

        // Удаление договора
        [HttpGet]
        [ActionName("Delete8")]
        public async Task<IActionResult> ConfirmDelete8(int? id, int? id2)
        {
            if (id != null)
            {
                Contract contract = await db.Contracts.FirstOrDefaultAsync(p => p.ContractId == id);
                if (contract != null)
                    return View(contract);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete8(int? id, int? id2)
        {
            Client user = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == id2);
            if (id != null)
            {
                Contract contract = await db.Contracts.FirstOrDefaultAsync(p => p.ContractId == id);
                if (contract != null)
                {
                    db.Contracts.Remove(contract);
                    await db.SaveChangesAsync();
                    return LocalRedirect($"~/ImportExport/ImptExp/{id2}?id1={user.ReportingPeriodId}");
                }
            }
            return NotFound();
        }

        // Изменить договор
        [HttpGet]
        [ActionName("Edit4")]
        public async Task<IActionResult> ConfirmEdit4(int? id, int? id2)
        {
            if (id != null)
            {
                Client user = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == id2);
                if (user != null)
                {
                    // ViewBag представляет такой объект, который позволяет определить любую переменную
                    // и передать ей некоторое значение, а затем в представлении извлечь это значение
                    //ViewBag.ClientSNM = client.SNM;
                    ViewBag.ContractNumber = user.ContractNumber;
                    ViewBag.Date = user.Date;
                    ViewBag.SNM = user.SNM;
                    ViewBag.NameLegalEntity = user.NameLegalEntity;
                    ViewBag.NameCategory = user.NameCategory;
                    ViewBag.AmountGoods = user.AmountGoods;
                    ViewBag.PassportData = user.PassportData;
                    ViewBag.ClientINN = user.ClientINN;
                    ViewBag.ClientOGRIP = user.ClientOGRIP;
                    ViewBag.Address1 = user.Address;
                    ViewBag.Telephone1 = user.Telephone;
                    ViewBag.UserId1 = user.UserId1;
                    

                    // Это мы пердоем имя ОТЧОТНОГО ПЕРИОДА
                    ReportingPeriod reportingPeriod = await db.ReportingPeriods.FirstOrDefaultAsync(p => p.Id == user.ReportingPeriodId);
                    ViewBag.NameReportingPeriod = reportingPeriod.NameReportingPeriod;


                }
                LegalEntity legalEntity = await db.LegalEntitys.FirstOrDefaultAsync(p => p.Id == user.LegalEntityId);
                {
                    ViewBag.LegalEntityName = legalEntity.LegalEntityName;
                    ViewBag.LegalEntityINN = legalEntity.LegalEntityINN;
                    ViewBag.LegalEntityOGRIP = legalEntity.LegalEntityOGRIP;
                    ViewBag.Telephone = legalEntity.Telephone;
                    ViewBag.Address = legalEntity.Address;
                }

                Contract contract = await db.Contracts.FirstOrDefaultAsync(p => p.ContractId == id);
                if (contract != null)
                    return View(contract);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit4(Contract contract, int? id, int? id2)
        {
            Client user = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == id2);
            db.Contracts.Update(contract);
            await db.SaveChangesAsync();
            return LocalRedirect($"~/ImportExport/ImptExp/{id2}?id1={user.ReportingPeriodId}");
        }

        // Копировать договор
        [HttpGet]
        [ActionName("CopyContract")]
        public async Task<IActionResult> ConfirmCopyContract(int? id, int? id2)
        {
            if (id != null)
            {
                Client user = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == id2);
                if (user != null)
                {
                    // ViewBag представляет такой объект, который позволяет определить любую переменную
                    // и передать ей некоторое значение, а затем в представлении извлечь это значение
                    //ViewBag.ClientSNM = client.SNM;
                    ViewBag.ContractNumber = user.ContractNumber;
                    ViewBag.Date = user.Date;
                    ViewBag.SNM = user.SNM;
                    ViewBag.NameLegalEntity = user.NameLegalEntity;
                    ViewBag.NameCategory = user.NameCategory;
                    ViewBag.AmountGoods = user.AmountGoods;
                    ViewBag.PassportData = user.PassportData;
                    ViewBag.ClientINN = user.ClientINN;
                    ViewBag.ClientOGRIP = user.ClientOGRIP;
                    ViewBag.Address1 = user.Address;
                    ViewBag.Telephone1 = user.Telephone;
                    ViewBag.UserId1 = user.UserId1;


                    // Это мы пердоем имя ОТЧОТНОГО ПЕРИОДА
                    ReportingPeriod reportingPeriod = await db.ReportingPeriods.FirstOrDefaultAsync(p => p.Id == user.ReportingPeriodId);
                    ViewBag.NameReportingPeriod = reportingPeriod.NameReportingPeriod;


                }
                LegalEntity legalEntity = await db.LegalEntitys.FirstOrDefaultAsync(p => p.Id == user.LegalEntityId);
                {
                    ViewBag.LegalEntityName = legalEntity.LegalEntityName;
                    ViewBag.LegalEntityINN = legalEntity.LegalEntityINN;
                    ViewBag.LegalEntityOGRIP = legalEntity.LegalEntityOGRIP;
                    ViewBag.Telephone = legalEntity.Telephone;
                    ViewBag.Address = legalEntity.Address;
                }

                Contract contract = await db.Contracts.FirstOrDefaultAsync(p => p.ContractId == id);
                if (contract != null)
                    return View(contract);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> CopyContract(Contract contract, int? id, int? id2)
        {
            Client user = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == id2);
            db.Contracts.Update(contract);
            await db.SaveChangesAsync();
            return LocalRedirect($"~/ImportExport/ImptExp/{id2}?id1={user.ReportingPeriodId}");
        }
    }
}

