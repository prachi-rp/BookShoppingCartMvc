using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCartMvcUi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dash;

        public DashboardController(IDashboardRepository dash)
        {
            _dash = dash;
        }
        //stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetOrdersCount()
        {
            var CountOrders = await _dash.CountOrders();
            var TotalAmounts = await _dash.TotalAmounts();
            var CountBooks = await _dash.CountBooks();
            var CountAuthors = await _dash.CountAuthors();
            var CountGenres = await _dash.CountGenres();
            var CountCustomers = await _dash.CountCustomers();

            var dashboardDto = new
            {
                CountOrders,
                TotalAmounts,
                CountBooks,
                CountAuthors,
                CountGenres,
                CountCustomers
            };
            return Ok(dashboardDto);
        }

    }
}
