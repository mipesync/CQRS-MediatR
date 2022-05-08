using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CQRS_MediatR.API.Controllers
{
    [ApiController]
    [Route("error")]
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View("~/API/Views/Shared/PageNotFound.cshtml");
        }
    }
}
