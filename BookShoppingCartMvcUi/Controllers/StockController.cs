
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCartApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = nameof(Roles.Admin))]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepo;

        public StockController(IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetStocks([FromQuery] string sTerm = "")
        {
            try
            {
                var stocks = await _stockRepo.GetStocks(sTerm);
                return Ok(stocks); // Return stocks as JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetStockByBookId(int bookId)
        {
            try
            {
                var stock = await _stockRepo.GetStockByBookId(bookId);
                if (stock == null)
                    return NotFound("Stock not found for the given Book ID");
                return Ok(stock);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageStock([FromBody] StockDTO stock)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Return validation errors

            try
            {
                await _stockRepo.ManageStock(stock);
                return Ok(new { Message = "Stock updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong!");
            }
        }
    }
}








//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace BookShoppingCartMvcUi.Controllers
//{
//    //[Authorize(Roles=nameof(Roles.Admin))]
//    public class StockController : Controller
//    {
//        private readonly IStockRepository _stockRepo;

//        public StockController(IStockRepository stockRepo)
//        {
//            _stockRepo = stockRepo;
//        }
//        public async Task<IActionResult> Index(string sTerm="")
//        {
//            var stocks = await _stockRepo.GetStocks(sTerm);
//            return View(stocks);
//        }

//        public async Task<IActionResult> ManageStock(int bookId)
//        {
//            var existingStock = await _stockRepo.GetStockByBookId(bookId);
//            var stock = new StockDTO
//            {
//                BookId = bookId,
//                Quantity = existingStock != null
//                ? existingStock.Quantity : 0
//            };
//            return View(stock);
//        }

//        [HttpPost]
//        public async Task<IActionResult> ManageStock(StockDTO stock)
//        {
//            if(!ModelState.IsValid) 
//                return View(stock);
//            try
//            {
//                await _stockRepo.ManageStock(stock);
//                TempData["successMessage"] = "Stock is updated successfully.";
//            }
//            catch(Exception ex)
//            {
//                TempData["errorMessage"] = "Something went wrong!!";
//            }
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
