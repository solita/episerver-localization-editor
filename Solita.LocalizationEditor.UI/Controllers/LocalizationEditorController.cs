using System.Web.Mvc;
using EPiServer.Shell.Navigation;
using Solita.LocalizationEditor.UI.Common;
using Solita.LocalizationEditor.UI.Lang;
using Solita.LocalizationEditor.UI.Models;

namespace Solita.LocalizationEditor.UI.Controllers
{
    public class LocalizationEditorController : Controller
    {
        private readonly LocalizationPersister _persister;

        public LocalizationEditorController()
        {
            _persister = new LocalizationPersister();
        }

        [MenuItem("/global/cms/localizations", TextResourceKey = "CmsMenuTitle", ResourceType = typeof(CmsMenuTitleProvider))]
        public ActionResult Index()
        {
            var model = new LocalizationEditorViewModel
            {
                Categories = _persister.GetLocalizations(),
                Languages = _persister.GetEnabledLanguages(),
                Versions = _persister.GetTranslationFileVersions(),
                PowertoolsMode = !Settings.AutoPopulated.DisablePowertoolsMode
            };

            return View(ModuleUtil.PathTo("Views/LocalizationEditor/Index.cshtml"), model);
        }

        [HttpPost]
        public ActionResult Save(LocalizationEditorViewModel model)
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