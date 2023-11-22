using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using programbeaverhut.ru.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace programbeaverhut.ru.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // Для загурузки файлов https://metanit.com/sharp/aspnet5/21.3.php
        IWebHostEnvironment _appEnvironment;
        private PbhContext db;
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        IExcelDataReader reader;

        public HomeController(PbhContext context, IWebHostEnvironment appEnvironment, IWebHostEnvironment _environment, IConfiguration _configuration)
        {
            db = context;
            _appEnvironment = appEnvironment;
            Environment = _environment;
            Configuration = _configuration;
        }


        // Главная страница
        public IActionResult Index(int? staff, string name)
        {
            IQueryable<Tasks> tasks = db.Taskss.Include(p => p.Staff);
            if (staff != null && staff != 0)
            {
                tasks = tasks.Where(p => p.StaffId == staff);
            }
            if (!String.IsNullOrEmpty(name))
            {
                tasks = tasks.Where(p => p.TaskGroupHiName.Contains(name));
            }

            List<Staff> staffs = db.Staffs.ToList();
            // устанавливаем начальный элемент, который позволит выбрать всех
            staffs.Insert(0, new Staff { StaffName = "Все сотрудники", Id = 0 });


            // Это все нужно для того чтобы было Несколько моделей в одном представлении в MVC (Сдесь это может пригодиться)
            CombinedLoginRegisterViewModel mymodel = new CombinedLoginRegisterViewModel
            {
                Tasks1 = tasks.ToList(),
                // Тут кстати проверка на пользователя как if но через раширение линькью вроде....
                Staffss = new SelectList(staffs.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier) || o.UserId1 == null).ToList(), "Id", "StaffName"),
                Name = name
            };

            //Если админ то выводяться все клиенты 
            if (User.IsInRole("admin"))
            {
                mymodel.ReportingPeriod1 = db.ReportingPeriods;
            }
            // Если нет то только клиенты пользователя
            else
            {
                mymodel.ReportingPeriod1 = db.ReportingPeriods.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier) || o.UserId1 == null).ToList();
            }

            mymodel.TaskGroupHi1 = db.TaskGroupHis;
            mymodel.Chat11 = db.Chat1s;

            return View(mymodel);
        }
        [HttpPost]
        public async Task<IActionResult> Index(ReportingPeriod reportingPeriod, Chat1 chat1, IFormFile file)
        {

            if (reportingPeriod.Password == reportingPeriod.AppliedPassword)
            {
                // Это заполняеться поле UserName и UserId1 
                reportingPeriod.UserName = User.Identity.Name;
                reportingPeriod.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                foreach (ColorSelection m in db.ColorSelections)
                {
                    if (reportingPeriod.ColorId == m.Id)
                    {
                        reportingPeriod.NameColor = m.Name;
                    }
                }

                db.ReportingPeriods.Update(reportingPeriod);
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // Новая группа задач
        public IActionResult TaskGroupNew()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> TaskGroupNew(TaskGroupHi task)
        {
            // Это заполняеться поле UserName и UserId1 
            task.UserName = User.Identity.Name;
            task.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            db.TaskGroupHis.Add(task);
            await db.SaveChangesAsync();
            return LocalRedirect($"~/Home/TaskGroupHi");
        }

        // Группа задач страница
        public IActionResult TaskGroupHi()
        {
            // Это все нужно для того чтобы было Несколько моделей в одном представлении в MVC (Сдесь это может пригодиться)
            CombinedLoginRegisterViewModel mymodel = new CombinedLoginRegisterViewModel();
            mymodel.TaskGroupHi1 = db.TaskGroupHis;

            return View(mymodel);
        }

        // Нова группа клиента
        public IActionResult ClientGroup()
        {
            // Выпадпющий список выделение цветом
            IEnumerable<ColorSelection> colorSelections = db.ColorSelections;
            ViewBag.ColorSelections = new SelectList(colorSelections, "Id", "Name");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ClientGroup(ReportingPeriod reportingPeriod)
        {
            // Выпадпющий список выделение цветом
            IEnumerable<ColorSelection> colorSelections = db.ColorSelections;
            ViewBag.ColorSelections = new SelectList(colorSelections, "Id", "Name");

            if (ModelState.IsValid)
            {
                if (reportingPeriod.Password == reportingPeriod.PasswordVeri)
                {
                    // Это заполняеться поле UserName и UserId1 
                    reportingPeriod.UserName = User.Identity.Name;
                    reportingPeriod.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    foreach (ColorSelection m in db.ColorSelections)
                    {
                        if (reportingPeriod.ColorId == m.Id)
                        {
                            reportingPeriod.NameColor = m.Name;
                        }
                    }
                    db.ReportingPeriods.Add(reportingPeriod);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(reportingPeriod);
            }
            return RedirectToAction("Index");
        }

        // Старница запросов юзера
        public async Task<IActionResult> RequestPageHi(string id)
        {
            // Нужно что бы добыть имя пользователя к которому зашол админ в чат поодержки
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
                ViewBag.UserName = user.UserName;
            }

            ViewBag.UserId1 = id;
            // Это все нужно для того чтобы было Несколько моделей в одном представлении в MVC (Сдесь это может пригодиться)
            CombinedLoginRegisterViewModel mymodel = new CombinedLoginRegisterViewModel();
            mymodel.RequestHi1 = db.RequestHis;

            return View(mymodel);
        }

        // Создание нового запроса
        public IActionResult RequestHi()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RequestHi(RequestHi requestHi, string id)
        {
            // Это нужно для того, что бы если тему у пользователя создает админ создавался на пользователя
            if (User.IsInRole("admin"))
            {

                if (id != null & id != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    User user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
                    // Это заполняеться поле UserName и UserId1 
                    requestHi.UserName = user.UserName;
                    requestHi.UserId1 = user.Id;
                    db.RequestHis.Update(requestHi);
                    await db.SaveChangesAsync();
                    return LocalRedirect($"~/Home/RequestPageHi/{id}");
                }
                else
                {
                    // Это заполняеться поле UserName и UserId1 
                    requestHi.UserName = User.Identity.Name;
                    requestHi.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    db.RequestHis.Add(requestHi);
                    await db.SaveChangesAsync();
                    return LocalRedirect($"~/Home/RequestPageHi");
                }
            }
            // Это заполняеться поле UserName и UserId1 
            requestHi.UserName = User.Identity.Name;
            requestHi.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            db.RequestHis.Add(requestHi);
            await db.SaveChangesAsync();
            return LocalRedirect($"~/Home/RequestPageHi");
        }

        // Чат задач (Обращение к модераторам)
        [HttpGet]
        [ActionName("MessagesRequests")]
        public async Task<IActionResult> ConfirmMessagesRequests(int? id, SortState sortOrder = SortState.NameDesc)
        {
            //Расширение линькю заменяет цикл
            RequestHi request = await db.RequestHis.FirstOrDefaultAsync(p => p.Id == id);
            ViewBag.requestHiTopic = request.Topic;
            ViewBag.Date = DateTime.UtcNow;
            ViewBag.requestHiId = id;
            // Это все нужно для того чтобы было Несколько моделей в одном представлении в MVC https://translated.turbopages.org/proxy_u/en-ru.ru.79cbbd31-62a37310-6f89da8b-74722d776562/https/www.c-sharpcorner.com/uploadfile/ff2f08/multiple-models-in-single-view-in-mvc/
            CombinedLoginRegisterViewModel mymodel = new CombinedLoginRegisterViewModel();
            mymodel.RequestHi1 = db.RequestHis;

            // Обнуляем непрочитанные у пользователя 
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) == request.UserId1)
            {
                request.UnreadAdmin = 0;
                db.RequestHis.Update(request);
                await db.SaveChangesAsync();
            }
            // Обнуляем непрочитанные у админа
            if (User.IsInRole("admin"))
            {
                //Удалить непрочитаные cообщения в модели User
                //Удаляються только те непрочитанные сообщения которые в открытом чате
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id == request.UserId1);
                user.UnreadUser = user.UnreadUser - request.UnreadUser;
                db.Users.Update(user);
                await db.SaveChangesAsync();

                request.UnreadUser = 0;
                db.RequestHis.Update(request);
                await db.SaveChangesAsync();
            }

            // Нужно для фильтрация сообщейний по дате, а не по порядку создания
            IQueryable<MessagesRequests> tasks = db.MessagesRequestss.Include(x => x.User);
            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["AgeSort"] = sortOrder == SortState.AgeAsc ? SortState.AgeDesc : SortState.AgeAsc;
            ViewData["CompSort"] = sortOrder == SortState.CompanyAsc ? SortState.CompanyDesc : SortState.CompanyAsc;
            tasks = sortOrder switch
            {
                SortState.NameDesc => tasks.OrderByDescending(s => s.Date),
                _ => tasks.OrderBy(s => s.Date),
            };
            mymodel.MessagesRequests1 = tasks;

            return View(mymodel);
        }
        [HttpPost]
        public async Task<IActionResult> MessagesRequests(int? id, MessagesRequests messages)
        {
            if (User.IsInRole("admin"))
            {
                // Это заполняеться поле UserName и UserId1 
                messages.UserName = "Admin";
                messages.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                //Добовляет одно непрочитанное песьмо у пользователя в модели чата подержки
                RequestHi request = await db.RequestHis.FirstOrDefaultAsync(p => p.Id == id);
                request.UnreadAdmin = request.UnreadAdmin + 1;
                db.RequestHis.Update(request);
                await db.SaveChangesAsync();

                db.MessagesRequestss.Add(messages);
                await db.SaveChangesAsync();
                return LocalRedirect($"~/Home/MessagesRequests/{id}%20");
            }
            // Это заполняеться поле UserName и UserId1 
            messages.UserName = User.Identity.Name;
            messages.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Добовляет одно непрочитанное песьмо у админа в модели чата подержки
            RequestHi request1 = await db.RequestHis.FirstOrDefaultAsync(p => p.Id == id);
            request1.UnreadUser = request1.UnreadUser + 1;
            db.RequestHis.Update(request1);
            await db.SaveChangesAsync();

            //Добовляет одно непрочитанное песьмо в модели User
            User user = await db.Users.FirstOrDefaultAsync(p => p.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            user.UnreadUser = user.UnreadUser + 1;
            db.Users.Update(user);
            await db.SaveChangesAsync();

            db.MessagesRequestss.Add(messages);
            await db.SaveChangesAsync();
            return LocalRedirect($"~/Home/MessagesRequests/{id}%20");
        }

        // Стараница заказа клиента (Тут блять нужно все разобрать и почистить такую помойку нельзя остовлять!) 
        // Объединение сортировки, фильтрации и пагинации. https://metanit.com/sharp/aspnet5/12.9.php
        [HttpGet]
        [ActionName("ClientRegistration")]
        public async Task<IActionResult> ConfirmClientRegistration(int? id, int? company, string name, int page = 1, SortState sortOrder = SortState.NameDesc)
        {
            if (id == null)
                return RedirectToAction("Index");
            // ViewBag представляет такой объект, который позволяет определить любую переменную
            // и передать ей некоторое значение, а затем в представлении извлечь это значение
            ViewBag.reportingPeriodId = id;
            // Дата коментария в чате
            ViewBag.Date = DateTime.UtcNow;

            foreach (ReportingPeriod period in db.ReportingPeriods)
            {
                if (period.Id == id)
                {
                    ViewBag.NameReportingPeriod = period.NameReportingPeriod;
                }
            }

            // Обнуления пароля для группы клиентов
            ReportingPeriod reporting = await db.ReportingPeriods.FirstOrDefaultAsync(p => p.Id == id);
            reporting.AppliedPassword = null;
            db.ReportingPeriods.Update(reporting);
            await db.SaveChangesAsync();

            int pageSize = 10000;


            //фильтрация
            IQueryable<Client> users = db.Clients.Include(x => x.Offices);

            if (company != null && company != 0)
            {
                users = users.Where(p => p.OfficesId == company);
            }

            if (!String.IsNullOrEmpty(name))
            {
                users = users.Where(p => p.SNM.Contains(name));
            }

            // сортировка
            switch (sortOrder)
            {
                case SortState.NameAsc:
                    users = users.OrderBy(s => s.SNM);
                    break;
                case SortState.AgeAsc:
                    users = users.OrderBy(s => s.Date);
                    break;
                case SortState.AgeDesc:
                    users = users.OrderByDescending(s => s.Date);
                    break;
                case SortState.CompanyAsc:
                    users = users.OrderBy(s => s.Offices.Name);
                    break;
                case SortState.CompanyDesc:
                    users = users.OrderByDescending(s => s.Offices.Name);
                    break;
                default:
                    users = users.OrderByDescending(s => s.SNM);
                    break;
            }

            // пагинация
            var count = await users.CountAsync();
            var items = await users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();



            // формируем модель представления
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                // Выпадпющий список (ТУТ еще спомощью Where сделана фильтрация по UserName) 
                FilterViewModel = new FilterViewModel(db.Officess.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier) || o.UserId1 == null).ToList(), company, name),

            };

            // Нужно для фильтрация сообщейний по дате, а не по порядку создания
            IQueryable<Chat1> tasks = db.Chat1s.Include(x => x.User);
            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["AgeSort"] = sortOrder == SortState.AgeAsc ? SortState.AgeDesc : SortState.AgeAsc;
            ViewData["CompSort"] = sortOrder == SortState.CompanyAsc ? SortState.CompanyDesc : SortState.CompanyAsc;
            tasks = sortOrder switch
            {
                SortState.NameDesc => tasks.OrderByDescending(s => s.Chat1Date),
                _ => tasks.OrderBy(s => s.Chat1Date),
            };
            viewModel.Chat11 = tasks;
            viewModel.FileModel1 = db.FileModels;

            // ТУТ еще спомощью Where сделана фильтрация по UserName (что бы выводились клиенты только этого пользователя)
            if (User.IsInRole("admin"))
            {
                viewModel.Clients = items;
            }
            else
            {
                // ТУТ еще спомощью Where сделана фильтрация по UserName (что бы выводились клиенты только этого пользователя)
                viewModel.Clients = items.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier) || o.UserId1 == null).ToList();
            }

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ClientRegistration(Chat1 chat1, IFormFileCollection uploads, IFormFile file, int? id)
        {
            // ИМПОРТ КЛИЕНТОВ EXSEL
            if (file != null)
            {
                // Создайте каталог, если он не существует
                string dirPath = Path.Combine(Environment.WebRootPath, "ReceivedReports");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                // Убедитесь, что используется только файл Excel
                string dataFileName = Path.GetFileName(file.FileName);

                string extension = Path.GetExtension(dataFileName);

                string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };

                if (!allowedExtsnions.Contains(extension))
                    throw new Exception("Sorry! This file is not allowed, make sure that file having extension as either.xls or.xlsx is uploaded.");

                // Сделайте копию опубликованного файла из полученного HTTP-запроса
                string saveToPath = Path.Combine(dirPath, dataFileName);

                using (FileStream stream = new FileStream(saveToPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // USe this to handle Encodeing differences in .NET Core
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                // read the excel file
                using (var stream = new FileStream(saveToPath, FileMode.Open))
                {
                    if (extension == ".xls")
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    DataSet ds = new DataSet();
                    ds = reader.AsDataSet();
                    reader.Close();

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        // Read the the Table
                        DataTable serviceDetails = ds.Tables[0];
                        DataTable serviceDetails1 = ds.Tables[1];
                        DataTable serviceDetails2 = ds.Tables[2];


                        for (int i = 1; i < serviceDetails.Rows.Count; i++)
                        {

                            Client details2 = new Client();

                            details2.ContractNumber = serviceDetails.Rows[i][0].ToString();
                            details2.SNM = serviceDetails.Rows[i][1].ToString();
                            details2.Address = serviceDetails.Rows[i][2].ToString();
                            details2.Telephone = serviceDetails.Rows[i][3].ToString();
                            details2.Date = serviceDetails.Rows[i][4].ToString();
                            details2.PassportData = serviceDetails.Rows[i][5].ToString();
                            details2.ClientINN = serviceDetails.Rows[i][6].ToString();
                            details2.ClientOGRIP = serviceDetails.Rows[i][7].ToString();
                            details2.PayGoods = Convert.ToDecimal(serviceDetails.Rows[i][8].ToString());
                            details2.RemainingСostGoods = Convert.ToDecimal(serviceDetails.Rows[i][9].ToString());
                            details2.AmountGoods = Convert.ToDecimal(serviceDetails.Rows[i][10].ToString());
                            details2.PayService = Convert.ToDecimal(serviceDetails.Rows[i][11].ToString());
                            details2.RemainingСostService = Convert.ToDecimal(serviceDetails.Rows[i][12].ToString());
                            details2.AmountService = Convert.ToDecimal(serviceDetails.Rows[i][13].ToString());
                            details2.OrderAssemblyStage = serviceDetails.Rows[i][14].ToString();
                            details2.ColorId = Convert.ToInt32(serviceDetails.Rows[i][15].ToString());
                            details2.NameColor = serviceDetails.Rows[i][16].ToString();

                            // Проверка с циклом нужна, для того что-бы понять есть ли вообще такое в SQL то что накидали в EXSELE.
                            // Так, тут была ошибка в цикле foreach если ставишь IF и вместе Else он брал только последний элемент и таблицы бызы данных
                            foreach (LegalEntity m in db.LegalEntitys)
                            {
                                if (serviceDetails.Rows[i][18].ToString() == m.LegalEntityName && Convert.ToInt32(serviceDetails.Rows[i][17].ToString()) == m.Id)
                                {
                                    details2.LegalEntityId = Convert.ToInt32(serviceDetails.Rows[i][17].ToString());
                                    details2.NameLegalEntity = serviceDetails.Rows[i][18].ToString();
                                }
                            }
                            // Если все пошло по одному месту)
                            if (details2.NameLegalEntity == null)
                            {
                                details2.NameLegalEntity = "Компания была удалена";
                                details2.LegalEntityId = 1;
                            }

                            // Проверка с циклом нужна, для того что-бы понять есть ли вообще такое в SQL то что накидали в EXSELE.
                            // Так, тут была ошибка в цикле foreach если ставишь IF и вместе Else он брал только последний элемент и таблицы бызы данных
                            foreach (Category m in db.Categorys)
                            {
                                if (serviceDetails.Rows[i][20].ToString() == m.NameCategory && Convert.ToInt32(serviceDetails.Rows[i][19].ToString()) == m.Id)
                                {
                                    details2.CategoryId = Convert.ToInt32(serviceDetails.Rows[i][19].ToString());
                                    details2.NameCategory = serviceDetails.Rows[i][20].ToString();
                                }
                            }
                            // Если все пошло по одному месту)
                            if (details2.NameCategory == null)
                            {
                                details2.NameCategory = "Категория была удалена";
                                details2.CategoryId = 1;
                            }

                            // Проверка с циклом нужна, для того что-бы понять есть ли вообще такое в SQL то что накидали в EXSELE.
                            // Так, тут была ошибка в цикле foreach если ставишь IF и вместе Else он брал только последний элемент и таблицы бызы данных
                            foreach (Offices mgh in db.Officess)
                            {
                                if (Convert.ToInt32(serviceDetails.Rows[i][21].ToString()) == mgh.Id && serviceDetails.Rows[i][22].ToString() == mgh.Name)
                                {
                                    details2.OfficesId = Convert.ToInt32(serviceDetails.Rows[i][21].ToString());
                                    details2.Name = serviceDetails.Rows[i][22].ToString();
                                }
                            }
                            // Если все пошло по одному месту)
                            if (details2.Name == null)
                            {
                                details2.Name = "Офис был удален";
                                details2.OfficesId = 1;
                            }


                            // Тут проверки не требуется, ставиться скидывающий USER 
                            details2.UserName = User.Identity.Name;
                            details2.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                            // Проверка с циклом нужна, для того что-бы понять есть ли вообще такое в SQL то что накидали в EXSELE.
                            // Так, тут была ошибка в цикле foreach если ставишь IF и вместе Else он брал только последний элемент и таблицы бызы данных
                            foreach (Staff m in db.Staffs)
                            {
                                if (serviceDetails.Rows[i][24].ToString() == m.StaffName && Convert.ToInt32(serviceDetails.Rows[i][23].ToString()) == m.Id)
                                {
                                    details2.Manager = serviceDetails.Rows[i][24].ToString();
                                    details2.StaffId = Convert.ToInt32(serviceDetails.Rows[i][23].ToString());
                                }
                            }
                            // Если все пошло по одному месту)
                            if (details2.Manager == null)
                            {
                                details2.Manager = "Сотрудник был удален";
                                details2.StaffId = 1;
                            }

                            details2.ReportingPeriodId = (int)id;

                            // Add the record in Database
                            db.Clients.Add(details2);
                            await db.SaveChangesAsync();

                            for (int i1 = 1; i1 < serviceDetails1.Rows.Count; i1++)
                            {

                                if (Convert.ToInt32(serviceDetails.Rows[i][25].ToString()) == Convert.ToInt32(serviceDetails1.Rows[i1][7].ToString()))
                                {
                                    Product details3 = new Product();

                                    details3.Description = serviceDetails1.Rows[i1][0].ToString();
                                    details3.Colour = serviceDetails1.Rows[i1][1].ToString();
                                    details3.Glass = serviceDetails1.Rows[i1][2].ToString();
                                    details3.Quantity = Convert.ToDecimal(serviceDetails1.Rows[i1][3].ToString());
                                    details3.Price = Convert.ToDecimal(serviceDetails1.Rows[i1][4].ToString());
                                    details3.Amount = Convert.ToDecimal(serviceDetails1.Rows[i1][5].ToString());
                                    details3.Discount = Convert.ToDecimal(serviceDetails1.Rows[i1][6].ToString());

                                    details3.ClientId = details2.ClientId;

                                    // Add the record in Database
                                    db.Products.Add(details3);
                                    await db.SaveChangesAsync();
                                }

                            }

                            for (int i2 = 1; i2 < serviceDetails2.Rows.Count; i2++)
                            {

                                if (Convert.ToInt32(serviceDetails.Rows[i][25].ToString()) == Convert.ToInt32(serviceDetails2.Rows[i2][2].ToString()))
                                {
                                    Service details4 = new Service();

                                    details4.ServiceDescription = serviceDetails2.Rows[i2][0].ToString();
                                    details4.ServicePrice = Convert.ToDecimal(serviceDetails2.Rows[i2][1].ToString());

                                    details4.ClientId = details2.ClientId;

                                    // Add the record in Database
                                    db.Services.Add(details4);
                                    await db.SaveChangesAsync();
                                }

                            }

                        }

                    }
                }
                System.IO.File.Delete($"wwwroot/ReceivedReports/{file.FileName}");
                return LocalRedirect($"~/Home/ClientRegistration/{id}");
            }

            // Это заполняеться поле UserName и UserId1 
            chat1.UserName = User.Identity.Name;
            chat1.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Занести новую 
            db.Chat1s.Add(chat1);
            // сохраняем в бд все изменения
            await db.SaveChangesAsync();

            // Для загурузки файлов https://metanit.com/sharp/aspnet5/21.3.php
            foreach (var uploadedFile in uploads)
            {
                // Генерируем уникальное имя для файла
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedFile.FileName);
                // путь к папке Files
                string path = "/Files/" + fileName;
                // Получаем путь к папке для сохранения файла на сервере
                string filePath = Path.Combine(_appEnvironment.WebRootPath + path);
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                FileModel file1 = new FileModel { Name = uploadedFile.FileName, Path = path };
                file1.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                file1.Chat1Id = chat1.Chat1Id;
                db.FileModels.Add(file1);
            }
            db.SaveChanges();

            return LocalRedirect($"~/Home/ClientRegistration/{chat1.ReportingPeriodId}");
        }


        // Страница, новый клиент
        public IActionResult Create(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");
            // ViewBag представляет такой объект, который позволяет определить любую переменную
            // и передать ей некоторое значение, а затем в представлении извлечь это значение
            ViewBag.reportingPeriodId = id;

            // Выпадпющий список (ТУТ еще спомощью Where сделана фильтрация по UserName) 
            IEnumerable<Offices> offices = db.Officess;
            ViewBag.Offices = new SelectList(offices.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), "Id", "Name", "UserName");

            // Выпадпющий список (ТУТ еще спомощью Where сделана фильтрация по UserName) 
            IEnumerable<Category> categories = db.Categorys;
            ViewBag.Сategories = new SelectList(categories.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), "Id", "NameCategory", "UserName");

            // Выпадпющий список (ТУТ еще спомощью Where сделана фильтрация по UserName) 
            IEnumerable<LegalEntity> legals = db.LegalEntitys;
            ViewBag.LegalEntitys = new SelectList(legals.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), "Id", "LegalEntityName", "UserName");

            // Выпадпющий список (ТУТ еще спомощью Where сделана фильтрация по UserName) 
            IEnumerable<Staff> staff = db.Staffs;
            ViewBag.Staffs = new SelectList(staff.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), "Id", "StaffName", "UserName");

            // Выпадпющий список выделение цветом
            IEnumerable<ColorSelection> colorSelections = db.ColorSelections;
            ViewBag.ColorSelections = new SelectList(colorSelections, "Id", "Name");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Client client)
        {

            // Это заполняеться поле UserName и UserId1 
            client.UserName = User.Identity.Name;
            client.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Очень важный нюанс! Так как я не придумал не чего умнее...
            // Сдесь спомощью перебора и условного оператора добываем нужное имя офиса,категории и добовляем их в базу данных!
            // Это проверка нужна, что бы если нет, не одного офиса и выпадающий список пуст, клиент всеравно создовался с офисом по умолчанию.
            if (0 != client.OfficesId)
            {
                foreach (Offices j in db.Officess)
                {

                    if (client.OfficesId == j.Id)
                    {
                        client.Name = j.Name;
                    }
                }
            }
            else
            {
                client.OfficesId = 1;
                foreach (Offices j in db.Officess)
                {

                    if (client.OfficesId == j.Id)
                    {
                        client.Name = j.Name;
                    }
                }
            }

            // Это проверка нужна, что бы если нет, не одной категории и выпадающий список пуст, клиент всеравно создовался с категорией по умолчанию.
            if (0 != client.CategoryId)
            {
                foreach (Category j in db.Categorys)
                {
                    if (client.CategoryId == j.Id)
                    {
                        client.NameCategory = j.NameCategory;
                    }
                }
            }
            else
            {
                client.CategoryId = 1;
                foreach (Category j in db.Categorys)
                {
                    if (client.CategoryId == j.Id)
                    {
                        client.NameCategory = j.NameCategory;
                    }
                }
            }

            // Это проверка нужна, что бы если нет, не одной категории и выпадающий список пуст, клиент всеравно создовался с категорией по умолчанию.
            if (0 != client.LegalEntityId)
            {
                foreach (LegalEntity j in db.LegalEntitys)
                {
                    if (client.LegalEntityId == j.Id)
                    {
                        client.NameLegalEntity = j.LegalEntityName;
                    }
                }
            }
            else
            {
                client.LegalEntityId = 1;
                foreach (LegalEntity j in db.LegalEntitys)
                {
                    if (client.LegalEntityId == j.Id)
                    {
                        client.NameLegalEntity = j.LegalEntityName;
                    }
                }
            }
            // Это проверка нужна, что бы если нет, не одного сотрудника и выпадающий список пуст, клиент всеравно создовался с сотрудникам по умолчанию по умолчанию.
            if (0 != client.StaffId)
            {
                foreach (Staff j in db.Staffs)
                {
                    if (client.StaffId == j.Id)
                    {
                        client.Manager = j.StaffName;
                    }
                }
            }
            else
            {
                client.StaffId = 1;
                foreach (Staff j in db.Staffs)
                {
                    if (client.StaffId == j.Id)
                    {
                        client.Manager = j.StaffName;
                    }
                }
            }


            foreach (ColorSelection m in db.ColorSelections)
            {
                if (client.ColorId == m.Id)
                {
                    client.NameColor = m.Name;
                }
            }

            db.Clients.Add(client);
            await db.SaveChangesAsync();
            // j = номер ID отчотного периода
            int? k = client.ReportingPeriodId;
            return LocalRedirect($"~/Home/ClientRegistration/{k}");
        }


        // Страница, новый заказ
        // ДОЛГО ОБЬЯСНЯТЬ что и зачем поэтому вот: https://metanit.com/sharp/aspnet5/3.4.php
        [HttpGet]
        [ActionName("Buy")]
        public async Task<IActionResult> ConfirmBuy(int? id, int? id1)
        {
            // Выпадпющий список УСЛУГ (ТУТ еще спомощью Where сделана фильтрация по UserName) 
            IEnumerable<ServiceName> serviceName = db.ServiceNames;
            ViewBag.ServiceName = new SelectList(serviceName.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), "ServName", "ServName", "UserName");

            // Это все нужно для того чтобы было Несколько моделей в одном представлении в MVC https://translated.turbopages.org/proxy_u/en-ru.ru.79cbbd31-62a37310-6f89da8b-74722d776562/https/www.c-sharpcorner.com/uploadfile/ff2f08/multiple-models-in-single-view-in-mvc/
            CombinedLoginRegisterViewModel mymodel = new CombinedLoginRegisterViewModel();
            mymodel.Product1 = db.Products;
            mymodel.Service1 = db.Services;
            mymodel.ReportingPeriod1 = db.ReportingPeriods;
            mymodel.Clients1 = db.Clients;
            // Условный оператор IF услови если ID = null то возращаеться на главную
            if (id == null)
                return RedirectToAction("Index");
            // ViewBag представляет такой объект, который позволяет определить любую переменную
            // и передать ей некоторое значение, а затем в представлении извлечь это значение
            ViewBag.ClientId = id;
            ViewBag.ClientId1 = id1;

            return View(mymodel);
        }
        [HttpPost]
        public async Task<IActionResult> Buy(Product product, Service service, string color, int? id, int? id1, int color1)
        {
            Client user = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == id);

            // Это статус сборки клиента
            if (color != null)
            {
                user.OrderAssemblyStage = color;
            }

            // Это цветовое выделение
            if (0 != color1)
            {
                user.ColorId = color1;

                if (1 == color1)
                {
                    user.NameColor = "Без выделения";
                }
                if (2 == color1)
                {
                    user.NameColor = "Красный";
                }
                if (3 == color1)
                {
                    user.NameColor = "Желтый";
                }
                if (4 == color1)
                {
                    user.NameColor = "Зеленый";
                }
            }


            // j = номер ID клиента
            int? j = id;

            if (service.ServicePrice != 0)
            {
                if (color != null)
                {
                    user.OrderAssemblyStage = color;
                    db.Clients.Update(user);
                    await db.SaveChangesAsync();
                }
                // Занести новую услуга
                db.Services.Add(service);
                // сохраняем в бд все изменения
                db.SaveChanges();

                // Сумма стоимости услуги по умолчанию
                decimal b = 0;
                // Сумма всех услуг у клиента
                foreach (Service service1 in db.Services)
                {
                    if (id != null)
                    {
                        //Проверка на кого клиента заказ по ID
                        if (id == service1.ClientId)
                        {
                            // Общая стоимость услуг
                            b += +service1.ServicePrice;
                            user.AmountService = b;
                        }
                    }
                }

            }

            // Занести новый товар
            if (product.Description != null)
            {
                db.Products.Add(product);
                // сохраняем в бд все изменения
                db.SaveChanges();
            }

            // Сдесь вычсиляеться сумма этого продукта
            product.Amount = product.Price * product.Quantity;
            // Вычитаеться процент этого продукта
            product.Amount = product.Amount - (product.Amount * product.Discount) / 100;

            // Сумма стоимости продукции по умолчанию
            decimal d = 0;
            // Сумма всех товаров у клиента
            foreach (Product products in db.Products)
            {
                if (id != null)
                {
                    //Проверка на кого клиента заказ по ID
                    if (id == products.ClientId)
                    {
                        // Общая стоимость продукции
                        d += +products.Amount;
                        user.AmountGoods = d;
                    }
                }
            }

            // Сохранение всех изминений в клиента
            db.Clients.Update(user);
            await db.SaveChangesAsync();

            return RedirectPermanent($"/Home/Buy/{j}?id1={user.ReportingPeriodId}");
        }

        // Страница, настройки
        [HttpGet]
        public IActionResult Settings()
        {
            // Это все нужно для того чтобы было Несколько моделей в одном представлении в MVC https://translated.turbopages.org/proxy_u/en-ru.ru.79cbbd31-62a37310-6f89da8b-74722d776562/https/www.c-sharpcorner.com/uploadfile/ff2f08/multiple-models-in-single-view-in-mvc/
            CombinedLoginRegisterViewModel mymodel = new CombinedLoginRegisterViewModel();
            mymodel.Officess1 = db.Officess;
            mymodel.Categorys1 = db.Categorys;
            mymodel.ServiceName1 = db.ServiceNames;
            mymodel.LegalEntity1 = db.LegalEntitys;
            mymodel.Staff1 = db.Staffs;

            return View(mymodel);
        }
        [HttpPost]
        public IActionResult Settings(Offices offices, Category category, ServiceName serviceName, LegalEntity legalEntity, Staff staff)
        {
            // Тут стоит обратить свое внимание на проверку полей на null. Это нужно что бы сохранялся в БД только один а не оба.
            if (offices.Name != null)
            {
                // Это заполняеться поле NAMEUSER и UserId1 (Нужно для определения какие офисы какому пользователю выводить)
                offices.UserName = User.Identity.Name;
                offices.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                db.Officess.Add(offices);
                db.SaveChanges();
                return RedirectToAction("Settings");
            }
            // Тут стоит обратить свое внимание на проверку полей на null. Это нужно что бы сохранялся в БД только один а не оба.
            if (legalEntity.LegalEntityName != null)
            {
                // Это заполняеться поле NAMEUSER и UserId1 (Нужно для определения какие офисы какому пользователю выводить)
                legalEntity.UserName = User.Identity.Name;
                legalEntity.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                db.LegalEntitys.Add(legalEntity);
                db.SaveChanges();
                return RedirectToAction("Settings");
            }
            if (category.NameCategory != null)
            {
                // Это заполняеться поле NAMEUSER и UserId1 (Нужно для определения какие офисы какому пользователю выводить)
                category.UserName = User.Identity.Name;
                category.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                db.Categorys.Add(category);
                db.SaveChanges();
                return RedirectToAction("Settings");
            }
            if (serviceName.ServName != null)
            {
                // Это заполняеться поле NAMEUSER и UserId1 (Нужно для определения какие офисы какому пользователю выводить)
                serviceName.UserName = User.Identity.Name;
                serviceName.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                db.ServiceNames.Add(serviceName);
                db.SaveChanges();
                return RedirectToAction("Settings");
            }
            if (staff.StaffName != null)
            {
                // Это заполняеться поле NAMEUSER и UserId1 (Нужно для определения какие офисы какому пользователю выводить)
                staff.UserName = User.Identity.Name;
                staff.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                db.Staffs.Add(staff);
                db.SaveChanges();
                return RedirectToAction("Settings");
            }


            return RedirectToAction("Index");
        }

        // Страница, задачи
        public async Task<IActionResult> Tasks(int? staff, string name, int? id, SortState sortOrder = SortState.AgeAsc)
        {
            // ViewBag
            ViewBag.TaskGroupHiId = id;
            // Приведение Int в String
            string str = id.ToString();
            // ViewBag
            ViewBag.TaskGroupHiId2 = str;


            IQueryable<Tasks> tasks = db.Taskss.Include(p => p.Staff);
            if (staff != null && staff != 0)
            {
                tasks = tasks.Where(p => p.StaffId == staff);
            }
            if (!String.IsNullOrEmpty(name))
            {
                tasks = tasks.Where(p => p.Description.Contains(name));
            }

            List<Staff> staffs = db.Staffs.ToList();
            // устанавливаем начальный элемент, который позволит выбрать всех
            staffs.Insert(0, new Staff { StaffName = "Все сотрудники", Id = 0 });


            // Это все нужно для того чтобы было Несколько моделей в одном представлении в MVC (Сдесь это может пригодиться)
            CombinedLoginRegisterViewModel mymodel = new CombinedLoginRegisterViewModel
            {
                Tasks1 = tasks.ToList(),
                // Тут кстати проверка на пользователя как if но через раширение линькью вроде....
                Staffss = new SelectList(staffs.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier) || o.UserId1 == null).ToList(), "Id", "StaffName"),
                Name = name
            };
            mymodel.ReportingPeriod1 = db.ReportingPeriods;
            mymodel.TaskGroupHi1 = db.TaskGroupHis;
            mymodel.Chat11 = db.Chat1s;



            // Нужно для фильтрация задач по дате а не по порядку создания
            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["AgeSort"] = sortOrder == SortState.AgeAsc ? SortState.AgeDesc : SortState.AgeAsc;
            ViewData["CompSort"] = sortOrder == SortState.CompanyAsc ? SortState.CompanyDesc : SortState.CompanyAsc;
            tasks = sortOrder switch
            {
                SortState.NameDesc => tasks.OrderByDescending(s => s.Date),
                _ => tasks.OrderBy(s => s.Date),
            };

            mymodel.Tasks1 = tasks;

            return View(mymodel);
        }

        // Создание задачи
        [HttpGet]
        [ActionName("NewTask")]
        public async Task<IActionResult> ConfirmNewTask(int id1)
        {
            ViewBag.TasksHiId = id1;

            // Выпадпющий список (ТУТ еще спомощью Where сделана фильтрация по UserName) 
            IEnumerable<Staff> staffs = db.Staffs;
            ViewBag.Staff = new SelectList(staffs.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), "StaffName", "StaffName", "UserName");


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NewTask(Tasks tasks)
        {
            // Это заполняеться поле UserName и UserId1 
            tasks.UserName = User.Identity.Name;
            tasks.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Вставляет нужное имя группы задач
            foreach (var task in db.TaskGroupHis)
            {
                if (tasks.TaskGroupHiId == task.TaskGroupHiId)
                {
                    tasks.TaskGroupHiName = task.NameTaskGroup;
                }
            }

            // Вставляет нужное айди работника
            if (tasks.Employees != null)
            {
                foreach (var staff in db.Staffs)
                {
                    if (tasks.Employees == staff.StaffName)
                    {
                        tasks.StaffId = staff.Id;
                    }
                }
            }
            else
            {
                tasks.StaffId = 1;
            }

            db.Taskss.Add(tasks);
            await db.SaveChangesAsync();
            return LocalRedirect($"~/Home/Tasks/{ tasks.TaskGroupHiId}");
        }

        // Удаление группы задач
        [HttpGet]
        [ActionName("Delete9")]
        public async Task<IActionResult> ConfirmDelete9(int? id)
        {
            if (id != null)
            {
                TaskGroupHi task = await db.TaskGroupHis.FirstOrDefaultAsync(p => p.TaskGroupHiId == id);
                if (task != null)
                    return View(task);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete9(int? id)
        {
            if (id != null)
            {
                TaskGroupHi task = await db.TaskGroupHis.FirstOrDefaultAsync(p => p.TaskGroupHiId == id);
                if (task != null)
                {
                    db.TaskGroupHis.Remove(task);
                    await db.SaveChangesAsync();
                    return LocalRedirect($"~/Home/TaskGroupHi");
                }
            }
            return NotFound();
        }

        // Удаление задачи
        [HttpGet]
        [ActionName("Delete8")]
        public async Task<IActionResult> ConfirmDelete8(int? id)
        {
            if (id != null)
            {
                Tasks tasks = await db.Taskss.FirstOrDefaultAsync(p => p.Id == id);
                if (tasks != null)
                    return View(tasks);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete8(int? id)
        {

            if (id != null)
            {
                Tasks tasks = await db.Taskss.FirstOrDefaultAsync(p => p.Id == id);
                if (tasks != null)
                {

                    db.Taskss.Remove(tasks);
                    await db.SaveChangesAsync();
                    return LocalRedirect($"~/Home/Tasks/{tasks.TaskGroupHiId}");
                }
            }
            return NotFound();
        }

        // Удаление клиента
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Client user = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {

            // Это нужно что бы удалять несколько файлов подряд
            if (id != null)
            {
                var fileModel1 = db.FilesClients.Where(p => p.ClientId == id).FirstOrDefault();

                if (fileModel1 != null)
                {
                    foreach (FilesClient s in db.FilesClients)
                    {
                        // Что бы удалялись фацлы только этого клиента
                        if (s.ClientId == id)
                        {
                            if (System.IO.File.Exists($"wwwroot{s.Path}"))
                            {
                                System.IO.File.Delete($"wwwroot{s.Path}");
                            }
                        }
                    }

                }
                db.SaveChanges();
            }

            if (id != null)
            {
                Client user = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == id);
                if (user != null)
                {

                    db.Clients.Remove(user);
                    await db.SaveChangesAsync();
                    return LocalRedirect($"~/Home/ClientRegistration/{user.ReportingPeriodId}%20");
                }
            }
            return NotFound();
        }

        // Удаление Заказ
        [HttpGet]
        [ActionName("Delete1")]
        public async Task<IActionResult> ConfirmDelete1(int? id)
        {

            if (id != null)
            {
                Product user = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete1(int? id)
        {
            if (id != null)
            {

                Product user = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id);

                Client user2 = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == user.ClientId);
                if (user != null)
                {

                    // Общая стоимость продукции
                    decimal d = user2.AmountGoods - user.Amount;
                    user2.AmountGoods = d;

                    // Сохранение всех изминений в клиента
                    db.Clients.Update(user2);
                    await db.SaveChangesAsync();


                    // Это удаление
                    db.Products.Remove(user);
                    await db.SaveChangesAsync();
                }

                // В общем я хз но так можно, я думаю, тперь у нас j=ClientId
                int j = user.ClientId;

                return LocalRedirect($"~/Home/Buy/{j}?id1={user2.ReportingPeriodId}");

            }
            return NotFound();
        }

        // Удаление офиса
        [HttpGet]
        [ActionName("Delete2")]
        public async Task<IActionResult> ConfirmDelete2(int? id)
        {
            if (id != null)
            {
                Offices ofis = await db.Officess.FirstOrDefaultAsync(p => p.Id == id);
                if (ofis != null)
                    return View(ofis);
            }

            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete2(int? id)
        {
            // Присвоение клиенту дифолтного офиса при удалении 
            foreach (Client client1 in db.Clients)
            {
                Offices offices = db.Officess.Where(p => p.Id == client1.OfficesId).FirstOrDefault();
                if (offices != null) // add this
                {
                    if (client1.OfficesId == id) // add this
                    {
                        client1.OfficesId = 1;

                        // Это надо для того что бы менялся не только Id офиса но и имя в базе данных
                        foreach (Offices j in db.Officess)
                        {
                            if (client1.OfficesId == j.Id)
                            {
                                client1.Name = j.Name;
                            }
                        }
                    }
                }
            }
            db.SaveChanges();

            if (id != null)
            {
                Offices ofis = await db.Officess.FirstOrDefaultAsync(p => p.Id == id);
                if (ofis != null)
                {
                    db.Officess.Remove(ofis);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Settings");
                }
            }
            return NotFound();
        }

        // Удаление категории
        [HttpGet]
        [ActionName("Delete3")]
        public async Task<IActionResult> ConfirmDelete3(int? id)
        {
            if (id != null)
            {
                Category categ = await db.Categorys.FirstOrDefaultAsync(p => p.Id == id);
                if (categ != null)
                    return View(categ);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete3(int? id)
        {
            // Присвоение клиенту дифолтной категории при удалении 
            foreach (Client client1 in db.Clients)
            {
                Category category = db.Categorys.Where(p => p.Id == client1.CategoryId).FirstOrDefault();
                if (category != null) // add this
                {
                    if (client1.CategoryId == id) // add this
                    {
                        client1.CategoryId = 1;

                        // Это надо для того что бы менялся не только Id категории но и имя в базе данных
                        foreach (Category j in db.Categorys)
                        {
                            if (client1.CategoryId == j.Id)
                            {
                                client1.NameCategory = j.NameCategory;
                            }
                        }
                    }
                }
            }
            db.SaveChanges();

            if (id != null)
            {
                Category categ = await db.Categorys.FirstOrDefaultAsync(p => p.Id == id);
                if (categ != null)
                {
                    db.Categorys.Remove(categ);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Settings");
                }
            }
            return NotFound();
        }

        // Удаление услуги
        [HttpGet]
        [ActionName("Delete4")]
        public async Task<IActionResult> ConfirmDelete4(int? id)
        {
            if (id != null)
            {
                ServiceName serviceName = await db.ServiceNames.FirstOrDefaultAsync(p => p.Id == id);
                if (serviceName != null)
                    return View(serviceName);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete4(int? id)
        {

            if (id != null)
            {
                ServiceName serviceName = await db.ServiceNames.FirstOrDefaultAsync(p => p.Id == id);
                if (serviceName != null)
                {
                    db.ServiceNames.Remove(serviceName);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Settings");
                }
            }
            return NotFound();
        }

        // Удаление услуги в заказе
        [HttpGet]
        [ActionName("Delete5")]
        public async Task<IActionResult> ConfirmDelete5(int? id)
        {
            if (id != null)
            {
                Service service = await db.Services.FirstOrDefaultAsync(p => p.ServiceId == id);
                if (service != null)
                    return View(service);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete5(int? id)
        {

            if (id != null)
            {
                Service service = await db.Services.FirstOrDefaultAsync(p => p.ServiceId == id);
                if (service != null)
                {

                    Client user2 = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == service.ClientId);
                    // Общая стоимость услуги
                    decimal d = user2.AmountService - service.ServicePrice;
                    user2.AmountService = d;
                    // Сохранение всех изминений в клиента
                    db.Clients.Update(user2);
                    await db.SaveChangesAsync();

                    // j = номер ID клиента
                    int? j = service.ClientId;

                    db.Services.Remove(service);
                    await db.SaveChangesAsync();
                    return LocalRedirect($"~/Home/Buy/{j}?id1={user2.ReportingPeriodId}");
                }
            }
            return NotFound();
        }

        // Удаление учотного периода
        [HttpGet]
        [ActionName("Delete6")]
        public async Task<IActionResult> ConfirmDelete6(int? id)
        {
            if (id != null)
            {
                ReportingPeriod reportingPeriod = await db.ReportingPeriods.FirstOrDefaultAsync(p => p.Id == id);
                if (reportingPeriod != null)
                    return View(reportingPeriod);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete6(int? id)
        {

            if (id != null)
            {
                ReportingPeriod reportingPeriod = await db.ReportingPeriods.FirstOrDefaultAsync(p => p.Id == id);
                if (reportingPeriod != null)
                {
                    db.ReportingPeriods.Remove(reportingPeriod);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        // Удаление компания
        [HttpGet]
        [ActionName("Delete7")]
        public async Task<IActionResult> ConfirmDelete7(int? id)
        {
            if (id != null)
            {
                LegalEntity legal = await db.LegalEntitys.FirstOrDefaultAsync(p => p.Id == id);
                if (legal != null)
                    return View(legal);
            }

            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete7(int? id)
        {
            // Присвоение клиенту дифолтного юр лица при удалении 
            foreach (Client client1 in db.Clients)
            {
                LegalEntity legalEntity = db.LegalEntitys.Where(p => p.Id == client1.LegalEntityId).FirstOrDefault();
                if (legalEntity != null) // add this
                {
                    if (client1.LegalEntityId == id) // add this
                    {
                        client1.LegalEntityId = 1;

                        // Это надо для того что бы менялся не только Id юр лица но и имя в базе данных
                        foreach (LegalEntity j in db.LegalEntitys)
                        {
                            if (client1.LegalEntityId == j.Id)
                            {
                                client1.NameLegalEntity = j.LegalEntityName;
                            }
                        }
                    }
                }
            }
            db.SaveChanges();

            if (id != null)
            {
                LegalEntity legal = await db.LegalEntitys.FirstOrDefaultAsync(p => p.Id == id);
                if (legal != null)
                {
                    db.LegalEntitys.Remove(legal);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Settings");
                }
            }
            return NotFound();
        }

        // Удалить сообщение в чате группы клиентов
        public async Task<ActionResult> Delete22(int? id)
        {
            if (id != null)
            {
                var fileModel1 = db.FileModels.Where(p => p.Chat1Id == id).FirstOrDefault();

                if (fileModel1 != null)
                {
                    foreach (FileModel s in db.FileModels)
                    {
                        // ЧТо бы только этого сообщения файлы
                        if (s.Chat1Id == id)
                        {
                            if (System.IO.File.Exists($"wwwroot{s.Path}"))
                            {
                                System.IO.File.Delete($"wwwroot{s.Path}");
                            }
                        }
                    }
                }
                db.SaveChanges();
            }

            if (id != null)
            {
                Chat1 chat = await db.Chat1s.FirstOrDefaultAsync(p => p.Chat1Id == id);
                if (chat != null)
                {
                    db.Chat1s.Remove(chat);
                    await db.SaveChangesAsync();
                    return LocalRedirect($"~/Home/ClientRegistration/{chat.ReportingPeriodId}%20");
                }
            }
            return NotFound();
        }

        // Удалить Запросы пользователя
        public async Task<ActionResult> Delete23(int? id, string id1)
        {
            if (id != null)
            {
                RequestHi request = await db.RequestHis.FirstOrDefaultAsync(p => p.Id == id);

                // Обнуляем непрочитанные у пользователя 
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) == request.UserId1)
                {
                    request.UnreadAdmin = 0;
                    db.RequestHis.Update(request);
                    await db.SaveChangesAsync();
                }
                // Обнуляем непрочитанные у админа
                if (User.IsInRole("admin"))
                {
                    //Удалить непрочитаные cообщения в модели User
                    //Удаляються только те непрочитанные сообщения которые в открытом чате
                    User user = await db.Users.FirstOrDefaultAsync(p => p.Id == request.UserId1);
                    user.UnreadUser = user.UnreadUser - request.UnreadUser;
                    db.Users.Update(user);
                    await db.SaveChangesAsync();

                    request.UnreadUser = 0;
                    db.RequestHis.Update(request);
                    await db.SaveChangesAsync();
                }

                if (request != null)
                {
                    if (User.IsInRole("admin"))
                    {
                        db.RequestHis.Remove(request);
                        await db.SaveChangesAsync();
                        return LocalRedirect($"~/Home/RequestPageHi/{id1}");
                    }
                    db.RequestHis.Remove(request);
                    await db.SaveChangesAsync();
                    return LocalRedirect($"~/Home/RequestPageHi");
                }
            }
            return NotFound();
        }

        // Удалить сообщение в чате
        public async Task<ActionResult> Delete24(int? id)
        {
            if (id != null)
            {
                MessagesRequests request = await db.MessagesRequestss.FirstOrDefaultAsync(p => p.MessagesRequestsId == id);
                if (request != null)
                {
                    db.MessagesRequestss.Remove(request);
                    await db.SaveChangesAsync();
                    return LocalRedirect($"~/Home/MessagesRequests/{request.RequestHiId1}%20");
                }
            }
            return NotFound();
        }

        // Удаление сотрудника
        [HttpGet]
        [ActionName("Delete10")]
        public async Task<IActionResult> ConfirmDelete10(int? id)
        {
            if (id != null)
            {
                Staff staff = await db.Staffs.FirstOrDefaultAsync(p => p.Id == id);
                if (staff != null)
                    return View(staff);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete10(int? id)
        {

            // Присвоение клиенту дифолтной категории при удалении 
            foreach (Tasks client1 in db.Taskss)
            {
                Staff staff = db.Staffs.Where(p => p.Id == client1.StaffId).FirstOrDefault();
                if (staff != null) // add this
                {
                    if (client1.StaffId == id) // add this
                    {
                        client1.StaffId = 1;

                        // Это надо для того что бы менялся не только Id категории но и имя в базе данных
                        foreach (Staff j in db.Staffs)
                        {
                            if (client1.StaffId == j.Id)
                            {
                                client1.Employees = j.StaffName;
                            }
                        }
                    }
                }
            }
            db.SaveChanges();

            if (id != null)
            {
                Staff staff = await db.Staffs.FirstOrDefaultAsync(p => p.Id == id);
                if (staff != null)
                {
                    db.Staffs.Remove(staff);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Settings");
                }
            }
            return NotFound();
        }

        // Удалить фай (Фото у клиента)
        public async Task<ActionResult> Delete25(int? id, int? id1)
        {
            if (id != null)
            {
                foreach (FilesClient s in db.FilesClients)
                {
                    // ЧТо бы только этого сообщения файлы
                    if (s.FilesClientId == id)
                    {
                        if (System.IO.File.Exists($"wwwroot{s.Path}"))
                        {
                            System.IO.File.Delete($"wwwroot{s.Path}");
                        }
                    }
                }

                db.SaveChanges();

            }

            if (id != null)
            {
                FilesClient files = await db.FilesClients.FirstOrDefaultAsync(p => p.FilesClientId == id);
                if (files != null)
                {
                    db.FilesClients.Remove(files);
                    await db.SaveChangesAsync();

                    return LocalRedirect($"~/Home/FilesClient/{id1}");
                }
            }
            return NotFound();
        }

        // Удалить сообщение в чате группы клиентов
        public async Task<ActionResult> Delete23(int? id)
        {
            if (id != null)
            {
                var fileModel1 = db.FileModels.Where(p => p.Chat1Id == id).FirstOrDefault();

                if (fileModel1 != null)
                {
                    foreach (FileModel s in db.FileModels)
                    {
                        // ЧТо бы только этого сообщения файлы
                        if (s.Chat1Id == id)
                        {
                            if (System.IO.File.Exists($"wwwroot{s.Path}"))
                            {
                                System.IO.File.Delete($"wwwroot{s.Path}");
                            }
                        }
                    }
                }
                db.SaveChanges();
            }

            if (id != null)
            {
                Chat1 chat = await db.Chat1s.FirstOrDefaultAsync(p => p.Chat1Id == id);
                if (chat != null)
                {
                    db.Chat1s.Remove(chat);
                    await db.SaveChangesAsync();
                    return LocalRedirect($"~/Home/ClientRegistration/{chat.ReportingPeriodId}%20");
                }
            }
            return NotFound();
        }

        // Изменить клиента
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {

            // Выпадпющий список (ТУТ еще спомощью класса SelectList сделана фильтрация по UserName) 
            IEnumerable<Offices> offices = db.Officess;
            ViewBag.Offices = new SelectList(offices.Where(o => o.UserName == User.Identity.Name).ToList(), "Id", "Name", "UserName");

            // Выпадпющий список (ТУТ еще спомощью Where сделана фильтрация по UserName) 
            IEnumerable<Category> categories = db.Categorys;
            ViewBag.Сategories = new SelectList(categories.Where(o => o.UserName == User.Identity.Name).ToList(), "Id", "NameCategory", "UserName");

            // Выпадпющий список (ТУТ еще спомощью Where сделана фильтрация по UserName) 
            IEnumerable<LegalEntity> legals = db.LegalEntitys;
            ViewBag.LegalEntitys = new SelectList(legals.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), "Id", "LegalEntityName", "UserName");

            // Выпадпющий список (ТУТ еще спомощью Where сделана фильтрация по UserName) 
            IEnumerable<Staff> staff = db.Staffs;
            ViewBag.Staffs = new SelectList(staff.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), "Id", "StaffName", "UserName");

            // Выпадпющий список выделение цветом
            IEnumerable<ColorSelection> colorSelections = db.ColorSelections;
            ViewBag.ColorSelections = new SelectList(colorSelections, "Id", "Name");

            // Этап сборки
            foreach (Client client1 in db.Clients)
            {
                if (client1.ClientId == id)
                {
                    ViewBag.OrderAssemblyStage = client1.OrderAssemblyStage;
                }
            }
            // Сумма стоимости за товар
            foreach (Client client1 in db.Clients)
            {
                if (client1.ClientId == id)
                {
                    ViewBag.AmountGoods = client1.AmountGoods;
                }
            }
            // Сумма стоимости за услуги
            foreach (Client client1 in db.Clients)
            {
                if (client1.ClientId == id)
                {
                    ViewBag.AmountService = client1.AmountService;
                }
            }


            if (id != null)
            {
                Client client = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == id);
                if (client != null)
                    return View(client);


            }

            return NotFound();

        }
        [HttpPost]
        public async Task<IActionResult> Edit(Client client, string color)
        {

            // Это заполняеться поле NameUser и UserId1
            client.UserName = User.Identity.Name;
            client.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Очень важный нюанс! Так как я не придумал не чего умнее...
            // Сдесь спомощью перебора и условного оператора добываем нужное имя офиса,категории и добовляем их в базу данных!
            // Это проверка нужна, что бы если нет, не одного офиса и выпадающий список пуст, клиент всеравно создовался с офисом по умолчанию.
            if (0 != client.OfficesId)
            {
                foreach (Offices j in db.Officess)
                {

                    if (client.OfficesId == j.Id)
                    {
                        client.Name = j.Name;
                    }
                }
            }
            else
            {
                client.OfficesId = 1;
                foreach (Offices j in db.Officess)
                {

                    if (client.OfficesId == j.Id)
                    {
                        client.Name = j.Name;
                    }
                }
            }

            // Это проверка нужна, что бы если нет, не одной категории и выпадающий список пуст, клиент всеравно создовался с категорией по умолчанию.
            if (0 != client.CategoryId)
            {
                foreach (Category j in db.Categorys)
                {
                    if (client.CategoryId == j.Id)
                    {
                        client.NameCategory = j.NameCategory;
                    }
                }
            }
            else
            {
                client.CategoryId = 1;
                foreach (Category j in db.Categorys)
                {
                    if (client.CategoryId == j.Id)
                    {
                        client.NameCategory = j.NameCategory;
                    }
                }
            }

            // Это проверка нужна, что бы если нет, не одной категории и выпадающий список пуст, клиент всеравно создовался с категорией по умолчанию.
            if (0 != client.LegalEntityId)
            {
                foreach (LegalEntity j in db.LegalEntitys)
                {
                    if (client.LegalEntityId == j.Id)
                    {
                        client.NameLegalEntity = j.LegalEntityName;
                    }
                }
            }
            else
            {
                client.LegalEntityId = 1;
                foreach (LegalEntity j in db.LegalEntitys)
                {
                    if (client.LegalEntityId == j.Id)
                    {
                        client.NameLegalEntity = j.LegalEntityName;
                    }
                }
            }

            // Это проверка нужна, что бы если нет, не одного сотрудника и выпадающий список пуст, клиент всеравно создовался с сотрудникам по умолчанию по умолчанию.
            if (0 != client.StaffId)
            {
                foreach (Staff j in db.Staffs)
                {
                    if (client.StaffId == j.Id)
                    {
                        client.Manager = j.StaffName;
                    }
                }
            }
            else
            {
                client.StaffId = 1;
                foreach (Staff j in db.Staffs)
                {
                    if (client.StaffId == j.Id)
                    {
                        client.Manager = j.StaffName;
                    }
                }
            }

            foreach (ColorSelection m in db.ColorSelections)
            {
                if (client.ColorId == m.Id)
                {
                    client.NameColor = m.Name;
                }
            }

            db.Clients.Update(client);
            await db.SaveChangesAsync();
            int? k = client.ReportingPeriodId;
            return LocalRedirect($"~/Home/ClientRegistration/{k}");
        }

        // Изменить услугу
        public async Task<IActionResult> Edit1(int? id)
        {
            // Выпадпющий список УСЛУГ (ТУТ еще спомощью Where сделана фильтрация по UserName) 
            IEnumerable<ServiceName> serviceName = db.ServiceNames;
            ViewBag.ServiceName = new SelectList(serviceName.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), "ServName", "ServName", "UserName");

            if (id != null)
            {
                Service service = await db.Services.FirstOrDefaultAsync(p => p.ServiceId == id);
                if (service != null)
                    return View(service);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit1(Service service)
        {
            // j = номер ID клиента
            int j = service.ClientId;

            Client client = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == service.ClientId);

            db.Services.Update(service);
            await db.SaveChangesAsync();
            return RedirectPermanent($"/Home/Buy/{j}?id1={client.ReportingPeriodId}");
        }

        // Изменить заказа
        [HttpGet]
        [ActionName("Edit2")]
        public async Task<IActionResult> ConfirmEdit2(int? id)
        {
            if (id != null)
            {

                Product product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
                if (product != null)
                    return View(product);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit2(Product product, int? id)
        {

            // j = номер ID клиента
            int j = product.ClientId;
            // Сдесь сумма вычсиляеться
            product.Amount = product.Price * product.Quantity;
            // Вычитаеться процент
            product.Amount = product.Amount - (product.Amount * product.Discount) / 100;

            db.Products.Update(product);
            await db.SaveChangesAsync();

            // Сумма всех товаров у клиента
            Product product1 = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            Client user2 = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == product1.ClientId);
            // Сумма стоимости продукции по умолчанию
            decimal d = 0;
            foreach (Product products in db.Products)
            {
                if (id != null)
                {
                    //Проверка на кого клиента заказ по ID
                    if (product1.ClientId == products.ClientId)
                    {
                        // Общая стоимость продукции
                        d += +products.Amount;
                        user2.AmountGoods = d;
                    }
                }
            }
            // Сохранение всех изминений в клиента
            db.Clients.Update(user2);
            await db.SaveChangesAsync();

            return RedirectPermanent($"/Home/Buy/{j}?id1={user2.ReportingPeriodId}");
        }

        // Извинения группы клиентов
        public async Task<IActionResult> Edit3(int? id)
        {
            // Выпадпющий список выделение цветом
            IEnumerable<ColorSelection> colorSelections = db.ColorSelections;
            ViewBag.ColorSelections = new SelectList(colorSelections, "Id", "Name");


            if (id != null)
            {
                ReportingPeriod reportingPeriod = await db.ReportingPeriods.FirstOrDefaultAsync(p => p.Id == id);
                if (reportingPeriod != null)

                    reportingPeriod.VerificationPassword = null;

                return View(reportingPeriod);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit3(ReportingPeriod reportingPeriod)
        {
            // Выпадпющий список выделение цветом
            IEnumerable<ColorSelection> colorSelections = db.ColorSelections;
            ViewBag.ColorSelections = new SelectList(colorSelections, "Id", "Name");

            // Это все нужно, что бы вывести правельно пароли и проверить их
            if (reportingPeriod.Password != reportingPeriod.OldPassword & reportingPeriod.NewPassword == reportingPeriod.Password)
            {
                // Это заполняеться поле UserName и UserId1 
                reportingPeriod.UserName = User.Identity.Name;
                reportingPeriod.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                foreach (ColorSelection m in db.ColorSelections)
                {
                    if (reportingPeriod.ColorId == m.Id)
                    {
                        reportingPeriod.NameColor = m.Name;
                    }
                }

                reportingPeriod.VerificationPassword = "  • Cтарый пароль введен неверно";
                reportingPeriod.NewPassword = null;
                reportingPeriod.OldPassword = null;

                db.ReportingPeriods.Update(reportingPeriod);
                await db.SaveChangesAsync();
                return View(reportingPeriod);
            }
            if (reportingPeriod.Password != reportingPeriod.OldPassword & reportingPeriod.NewPassword != null)
            {
                // Это заполняеться поле UserName и UserId1 
                reportingPeriod.UserName = User.Identity.Name;
                reportingPeriod.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                foreach (ColorSelection m in db.ColorSelections)
                {
                    if (reportingPeriod.ColorId == m.Id)
                    {
                        reportingPeriod.NameColor = m.Name;
                    }
                }

                reportingPeriod.VerificationPassword = " • Cтарый пароль введен неверно";
                reportingPeriod.NewPassword = null;
                reportingPeriod.OldPassword = null;

                db.ReportingPeriods.Update(reportingPeriod);
                await db.SaveChangesAsync();
                return View(reportingPeriod);
            }
            if (reportingPeriod.Password != reportingPeriod.OldPassword & reportingPeriod.NewPassword == null & reportingPeriod.OldPassword != null)
            {
                // Это заполняеться поле UserName и UserId1 
                reportingPeriod.UserName = User.Identity.Name;
                reportingPeriod.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                foreach (ColorSelection m in db.ColorSelections)
                {
                    if (reportingPeriod.ColorId == m.Id)
                    {
                        reportingPeriod.NameColor = m.Name;
                    }
                }

                reportingPeriod.VerificationPassword = "  • Cтарый пароль введен неверно";
                reportingPeriod.NewPassword = null;
                reportingPeriod.OldPassword = null;

                db.ReportingPeriods.Update(reportingPeriod);
                await db.SaveChangesAsync();
                return View(reportingPeriod);
            }

            if (reportingPeriod.Password == reportingPeriod.OldPassword)
            {
                reportingPeriod.Password = reportingPeriod.NewPassword;
                reportingPeriod.PasswordVeri = reportingPeriod.NewPassword;
            }


            // Это заполняеться поле UserName и UserId1 
            reportingPeriod.UserName = User.Identity.Name;
            reportingPeriod.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (ColorSelection m in db.ColorSelections)
            {
                if (reportingPeriod.ColorId == m.Id)
                {
                    reportingPeriod.NameColor = m.Name;
                }
            }

            // Обнуление проверочных паролей (Это для того чтобы пароли и имя с цветом менялись отдельно)
            reportingPeriod.NewPassword = null;
            reportingPeriod.OldPassword = null;
            //reportingPeriod.VerificationPassword = null;


            db.ReportingPeriods.Update(reportingPeriod);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Изменить задачу
        [HttpGet]
        [ActionName("Edit4")]
        public async Task<IActionResult> ConfirmEdit4(int? id)
        {
            if (id != null)
            {
                // Выпадпющий список (ТУТ еще спомощью Where сделана фильтрация по UserName) 
                IEnumerable<Staff> staffs = db.Staffs;
                ViewBag.Staff = new SelectList(staffs.Where(o => o.UserId1 == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), "StaffName", "StaffName", "UserName");

                Tasks tasks = await db.Taskss.FirstOrDefaultAsync(p => p.Id == id);
                if (tasks != null)
                    return View(tasks);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit4(Tasks tasks, int? id)
        {

            // Вставляет нужное имя группы задач
            foreach (var task in db.TaskGroupHis)
            {
                if (tasks.TaskGroupHiId == task.TaskGroupHiId)
                {
                    tasks.TaskGroupHiName = task.NameTaskGroup;
                }
            }
            // Вставляет нужное айди работника
            if (tasks.Employees != null)
            {
                foreach (var staff in db.Staffs)
                {
                    if (tasks.Employees == staff.StaffName)
                    {
                        tasks.StaffId = staff.Id;
                    }
                }
            }
            else
            {
                tasks.StaffId = 1;
            }

            db.Taskss.Update(tasks);
            await db.SaveChangesAsync();
            return RedirectPermanent($"/Home/Tasks/{tasks.TaskGroupHiId}");
        }

        // Изменить группу задач
        [HttpGet]
        [ActionName("Edit5")]
        public async Task<IActionResult> ConfirmEdit5(int? id)
        {
            if (id != null)
            {

                TaskGroupHi tasks = await db.TaskGroupHis.FirstOrDefaultAsync(p => p.TaskGroupHiId == id);
                if (tasks != null)
                    return View(tasks);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit5(TaskGroupHi tasks, int? id)
        {
            db.TaskGroupHis.Update(tasks);
            await db.SaveChangesAsync();
            return RedirectPermanent($"/Home/TaskGroupHi");
        }

        // Изменить собщение в чате 
        public async Task<IActionResult> Edit6(int? id)
        {
            if (id != null)
            {
                Chat1 chat = await db.Chat1s.FirstOrDefaultAsync(p => p.Chat1Id == id);
                if (chat != null)
                    return View(chat);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit6(Chat1 chat)
        {
            db.Chat1s.Update(chat);
            await db.SaveChangesAsync();
            return RedirectPermanent($"/Home/ClientRegistration/{chat.ReportingPeriodId}%20");
        }

        // Редактировать запрос пользователя в поддержке
        [HttpGet]
        [ActionName("Edit7")]
        public async Task<IActionResult> ConfirmEdit7(int? id)
        {
            if (id != null)
            {

                RequestHi request = await db.RequestHis.FirstOrDefaultAsync(p => p.Id == id);
                if (request != null)
                    return View(request);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit7(RequestHi request, int? id)
        {
            db.RequestHis.Update(request);
            await db.SaveChangesAsync();
            if (User.IsInRole("admin"))
            {
                return RedirectPermanent($"~/Home/RequestPageHi/{request.UserId1}");
            }
            return RedirectPermanent($"~/Home/RequestPageHi");
        }

        // Изменить собщение в чате 
        public async Task<IActionResult> Edit8(int? id)
        {
            if (id != null)
            {
                MessagesRequests messages = await db.MessagesRequestss.FirstOrDefaultAsync(p => p.MessagesRequestsId == id);
                if (messages != null)
                    return View(messages);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit8(MessagesRequests messages)
        {
            db.MessagesRequestss.Update(messages);
            await db.SaveChangesAsync();
            return RedirectPermanent($"/Home/MessagesRequests/{messages.RequestHiId1}");
        }

        // Копировать товар
        [HttpGet]
        [ActionName("CopyProduct")]
        public async Task<IActionResult> ConfirmCopyProduct(int? id)
        {
            if (id != null)
            {
                Product product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
                if (product != null)
                    return View(product);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> CopyProduct(Product product, int? id)
        {
            // j = номер ID клиента
            int j = product.ClientId;
            // Сдесь сумма вычсиляеться
            product.Amount = product.Price * product.Quantity;
            // Вычитаеться процент
            product.Amount = product.Amount - (product.Amount * product.Discount) / 100;

            db.Products.Update(product);
            await db.SaveChangesAsync();

            // Сумма всех товаров у клиента
            Product product1 = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            Client user2 = await db.Clients.FirstOrDefaultAsync(p => p.ClientId == product1.ClientId);
            // Сумма стоимости продукции по умолчанию
            decimal d = 0;
            foreach (Product products in db.Products)
            {
                if (id != null)
                {
                    //Проверка на кого клиента заказ по ID
                    if (product1.ClientId == products.ClientId)
                    {
                        // Общая стоимость продукции
                        d += +products.Amount;
                        user2.AmountGoods = d;
                    }
                }
            }
            // Сохранение всех изминений в клиента
            db.Clients.Update(user2);
            await db.SaveChangesAsync();

            return RedirectPermanent($"/Home/Buy/{j}?id1={user2.ReportingPeriodId}");
        }

        // Добавить фото к клиенту
        [HttpGet]
        [ActionName("FilesClient")]
        public async Task<IActionResult> ConfirmFilesClient(int? id)
        {
            ViewBag.ClientId = id;
            IndexViewModel viewModel = new IndexViewModel();
            viewModel.FilesClient1 = db.FilesClients;
            viewModel.Clients = db.Clients;

            return View(viewModel);

        }
        [HttpPost]
        public async Task<IActionResult> FilesClient(IFormFileCollection uploads, int? id)
        {

            foreach (var uploadedFile in uploads)
            {

                // Генерируем уникальное имя для файла
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedFile.FileName);
                // путь к папке Files
                string path = "/Files/" + fileName;
                // Получаем путь к папке для сохранения файла на сервере
                string filePath = Path.Combine(_appEnvironment.WebRootPath + path);
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                FilesClient file = new FilesClient { NewName = fileName, Name = uploadedFile.FileName, Path = path };

                file.UserId1 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                file.ClientId = id;

                db.FilesClients.Add(file);
            }
            db.SaveChanges();

            return RedirectPermanent($"/Home/FilesClient/{id}");
        }

    }

}

