using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookShoppingCartMvcUi.Controllers
{
    [ApiController]
    [Route("api/myapi")]
    [EnableCors("AllowReactApp")]
    public class MyAPIController : ControllerBase
    {
        [HttpGet]
        [Route("getdata")]
        public IActionResult GetData()
        {
            var data = new { message = "Hello from ASP.NET Core!" };
            return Ok(data);
        }

    }
}
