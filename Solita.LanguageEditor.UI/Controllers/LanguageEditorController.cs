using System.Web.Mvc;
using EPiServer.Shell.Navigation;
using Solita.LanguageEditor.UI.Common;
using Solita.LanguageEditor.UI.Lang;
using Solita.LanguageEditor.UI.Models;

namespace Solita.LanguageEditor.UI.Controllers
{
    [Authorize(Roles = "WebEditors, WebAdmins, Administrators")]
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

            return View(ModuleUtil.PathTo("Views/LanguageEditor/Index.cshtml"), model);
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