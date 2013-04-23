using System.Web.Mvc;
using EPiServer.Shell.Navigation;
using EPiServer.Shell.Web.Mvc;
using Solita.LanguageEditor.UI.Common;
using Solita.LanguageEditor.UI.Lang;
using Solita.LanguageEditor.UI.Models;
using EPiServer.Shell.Web.Mvc.Html;

namespace Solita.LanguageEditor.UI.Controllers
{
    [Authorize]
    public class LanguageEditorController : Controller
    {
        private readonly LocalizationPersister _persister;

        public LanguageEditorController()
        {
            _persister = new LocalizationPersister();
        }

        [MenuItem("/global/cms/localizations", TextResourceKey = "CmsMenuTitle", ResourceType = typeof(CmsMenuTitleProvider))]
        public ActionResult Index()
        {
            var model = new LanguageEditorViewModel
                {
                    Categories = _persister.GetLocalizations(),
                    Languages = _persister.GetEnabledLanguages(),
                    Versions = _persister.GetTranslationFileVersions(),
                    DevelopmentMode = Settings.AutoPopulated.DevelopmentMode
                };

            // For some reason EPiServer's add-on system doesn't check for .cshtml files, so the view path needs to be set manually
            var viewPath = ModuleUtil.PathTo("Views/LanguageEditor/Index.cshtml");
            return View(viewPath, model);
            
            //return View("Index", model);
        }

        [HttpPost]
        public ActionResult Save(LanguageEditorViewModel model)
        {
            _persister.SaveLocalizations(model);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult GetJsonLocalizations(string version)
        {
            return Json(_persister.GetJsonLocalizations(version));
        }
    }
}