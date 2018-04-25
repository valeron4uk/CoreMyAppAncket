using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreMyAppAncket.Data;
using CoreMyAppAncket.Models.AncketVievModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using CoreMyAppAncket.Models;
using Microsoft.AspNetCore.Authorization;
using PagedList.Core;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using CoreMyAppAncket.Services;

namespace CoreMyAppAncket.Controllers
{
    [RequireHttps]
    [Authorize]
    public class AncketController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public AncketController(ApplicationDbContext context,
                                UserManager<ApplicationUser> manager,
                                IConfiguration configuration,
                                ILogger<AncketController> logger)
        {
            db = context;
            _userManager = manager;
            _configuration = configuration;
            _logger = logger;
        }
        // GET: Ancket
        [Authorize]
        public ActionResult Index()
        {
            List<Ancket> anckets = new List<Ancket>();
            Dictionary<int, int> results = new Dictionary<int, int>();
            var userId = _userManager.GetUserId(HttpContext.User);
            foreach (var item in db.Anckets.Where(t => t.UserId == userId).ToList())
            {
                Ancket ancket = item;
                ancket.AncketForms = db.AncketForms.Where(t => t.AncketId == item.Id).ToList();
                anckets.Add(ancket);
                results.Add(ancket.Id, db.AncketResults.Where(t => t.AncketId == ancket.Id).Count());

            }

            ViewBag.Results = results;
            return View(anckets);
        }


        // GET: Ancket/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id != null)
            {
                Ancket ancket = db.Anckets.Find(id);
                ancket.AncketForms = db.AncketForms.Where(x => x.AncketId == id).ToList();

                return View(ancket);
            }
            else
            {

                Ancket ancket = new Ancket()
                {
                    AncketForms = new List<AncketForm>()
                };
                return View(ancket);
            }

        }


        // POST: Ancket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(Ancket ancket, Dictionary<string, AncketForm> ancketForms)
        {
            try
            {
                if (ancket != null)
                {
                    var userId = _userManager.GetUserId(HttpContext.User);
                    Ancket a = new Ancket()
                    {
                        Id = ancket.Id,
                        Name = ancket.Name,
                        Date = DateTime.Now,
                        UserId = userId,
                        IsSendMail=ancket.IsSendMail
                    };
                    a.AncketForms = new List<AncketForm>();
                    foreach (var item in ancketForms)
                    {
                        a.AncketForms.Add(item.Value);
                    }

                    if (ancket.Id != 0)
                    {
                        var forms = db.AncketForms.Where(t => t.AncketId == ancket.Id).ToList();
                        db.AncketForms.RemoveRange(forms);
                        db.Anckets.Update(a);
                    }
                    else
                    {
                        db.Anckets.Add(a);
                    }

                    db.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return View(e);
            }
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ancket = await db.Anckets.FindAsync(id);
            if (ancket == null)
            {
                return NotFound();
            }

            return View(ancket);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var a = await db.Anckets.FindAsync(id);
                db.Anckets.Remove(a);
                await db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        public ActionResult ViewAncket(int? id, Ancket ancket, Dictionary<string, AncketForm> ancketForms)
        {
            if (id == null)
            {
                Ancket a = new Ancket()
                {
                    Id = ancket.Id,
                    Name = ancket.Name,
                    Date = DateTime.Now
                };
                a.AncketForms = new List<AncketForm>();
                foreach (var item in ancketForms)
                {
                    a.AncketForms.Add(item.Value);
                }

                return View(a);
            }
            else
            {
                Ancket aa = db.Anckets.Find(id);
                aa.AncketForms = db.AncketForms.Where(t => t.AncketId == id).ToList();
                return View(aa);
            }


        }


        public ActionResult AncketForm()
        {
            AncketForm ancketForm = new AncketForm();
            return View("AncketForm", new KeyValuePair<string, AncketForm>(
                            Guid.NewGuid().ToString("N"),
                            ancketForm));
        }

        public ActionResult FormData(string typeno, string key, AncketForm ancketForm)
        {

            ancketForm.FormType = (FormType)Enum.Parse(typeof(FormType), typeno);

            return View("FormData", new KeyValuePair<string, AncketForm>(key, ancketForm));
        }

        public ActionResult Results(int? id, int? page)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ancket = db.Anckets.Find(id);
            if (ancket == null)
            {
                return NotFound();
            }
            var results = db.AncketResults.Where(t => t.AncketId == id);
            foreach (var item in results)
            {
                item.AncketFormResults = db.AncketFormResults.Where(t => t.AncketResultId == item.Id).ToList();
            }

            int pageSize = 2;
            int pageNumber = page ?? 1;


            ViewBag.Ancket = ancket;
            return View(results.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AncketEnterData(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Ancket ancket = await db.Anckets.FindAsync(id);
            if (ancket == null)
            {
                return NotFound();
            }

            ancket.AncketForms = db.AncketForms.Where(t => t.AncketId == id).ToList();
            ViewData["ReCaptchaKey"] = _configuration.GetSection("Recaptcha:SiteKey").Value;
            return View(ancket);
        }

        //[ValidateRecaptcha]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AncketEnterDate(Ancket ancket, Dictionary<string, AncketForm> ancketForms)
        {
            
                try
                {

                    AncketResult ancketResult = new AncketResult()
                    {
                        AncketId = ancket.Id,
                        AncketName = ancket.Name,
                        AncketFormResults = new List<AncketFormResult>()
                    };

                

                foreach (var item in ancketForms)
                    {

                        if (item.Value.FormType.Equals(FormType.CheckBox))
                        {
                            ancketResult.AncketFormResults.Add(new AncketFormResult()
                            {
                                FormType = item.Value.FormType,
                                NameForm = item.Value.FormName,
                                CheckBoxData = item.Value.CheckBoxData
                            });
                        }
                        else if (item.Value.FormType.Equals(FormType.ComboBox) || item.Value.FormType.Equals(FormType.TextArea) || item.Value.FormType.Equals(FormType.TextBoxString))
                        {
                            ancketResult.AncketFormResults.Add(new AncketFormResult()
                            {
                                FormType = item.Value.FormType,
                                NameForm = item.Value.FormName,
                                StringData = item.Value.TextBoxStringData
                            });
                        }
                        else if (item.Value.FormType.Equals(FormType.Email))
                        {
                            ancketResult.AncketFormResults.Add(new AncketFormResult()
                            {
                                FormType = item.Value.FormType,
                                NameForm = item.Value.FormName
                            });

                        }
                        else if (item.Value.FormType.Equals(FormType.Phone) || item.Value.FormType.Equals(FormType.TextBoxInt))
                        {
                            ancketResult.AncketFormResults.Add(new AncketFormResult()
                            {
                                FormType = item.Value.FormType,
                                NameForm = item.Value.FormName,
                                IntData = item.Value.TextBoxIntData
                            });
                            //db.AncketFormResults.Add(new AncketFormResult()
                            //{
                            //    AncketFormId = item.Value.Id,
                            //    IntData = item.Value.TextBoxIntData
                            //});
                        }
                        

                    }
                    await db.AncketResults.AddAsync(ancketResult);
                    await db.SaveChangesAsync();

                if (ancket.IsSendMail == true)
                {
                    EmailService emailService = new EmailService();
                    var email = ancketResult.AncketFormResults.FirstOrDefault(t => t.FormType.Equals(FormType.Email)).NameForm;
                    var text = "TEST TEXT";
                    await emailService.SendEmailAsync(email, "Your form ", text);
                   
                }

                return RedirectToAction(nameof(Index), "Home");
                }
                catch (Exception e)
                {
                    return View(e);
                }
            
        }



        public static bool ReCaptchaPassed(string gRecaptchaResponse, string secret, ILogger logger)
        {
            HttpClient httpClient = new HttpClient();
            var res = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={gRecaptchaResponse}").Result;
            if (res.StatusCode != HttpStatusCode.OK)
            {
                logger.LogError("Error while sending request to ReCaptcha");
                return false;
            }

            string JSONres = res.Content.ReadAsStringAsync().Result;
            dynamic JSONdata = JObject.Parse(JSONres);
            if (JSONdata.success != "true")
            {
                return false;
            }

            return true;
        }

        //[HttpPost]
        [AllowAnonymous]
        public JsonResult Verify(string gRecaptchaResponse)
        {
            var res = ReCaptchaPassed(gRecaptchaResponse, _configuration.GetSection("Recaptcha:SecretKey").Value, _logger);

            return new JsonResult(res);
        }
    }
}