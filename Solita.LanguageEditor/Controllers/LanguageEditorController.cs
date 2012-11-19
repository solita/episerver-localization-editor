using System.Web.Mvc;
using EPiServer.Shell;
using EPiServer.Shell.Navigation;
using Solita.LanguageEditor.Models;

namespace Solita.LanguageEditor.Controllers
{
//    [Authorize(Roles = "LanguageEditors")]
    public class LanguageEditorController : Controller
    {
        private readonly LocalizationPersister _persister;

        public LanguageEditorController()
        {
            _persister = new LocalizationPersister();
        }

        [MenuItem("/global/cms/localizations", Text = "Language editor")]
        public ActionResult Index()
        {
            // For some reason EPiServer's add-on system doesn't check for .cshtml files, so the view path needs to be set manually
            var viewPath = Paths.ToResource(GetType(), "Views/LanguageEditor/Index.cshtml");
            return View(viewPath, _persister.GetLocalizations());
            
            //return View("Index", _persister.GetLocalizations());
        }

        public ActionResult Save(LanguageEditorViewModel model)
        {
            _persister.SaveLocalizations(model);

            return RedirectToAction("Index");
        }
    }
}