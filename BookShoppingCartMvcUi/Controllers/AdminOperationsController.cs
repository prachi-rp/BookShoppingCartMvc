using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BookShoppingCartMvcUi.Controllers
{
   // [Authorize(Roles = nameof(Roles.Admin))]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminOperationsController : ControllerBase
    {
        private readonly IUserOrderRepository _userOrderRepository;

        public AdminOperationsController(IUserOrderRepository userOrderRepository)
        {
            _userOrderRepository = userOrderRepository;
        }

        [HttpGet("AllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _userOrderRepository.UserOrders(true);
            return Ok(orders); // Returns a JSON response
        }

        [HttpPost("TogglePaymentStatus/{orderId}")]
        public async Task<IActionResult> TogglePaymentStatus(int orderId)
        {
            try
            {
                await _userOrderRepository.TogglePaymentStatus(orderId);
                return Ok(new { Message = "Payment status toggled successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("Order/{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _userOrderRepository.GetOrderById(orderId);
            if (order == null)
                return NotFound(new { Message = $"Order with ID {orderId} not found" });

            return Ok(order);
        }

        [HttpGet("OrderStatuses")]
        public async Task<IActionResult> GetOrderStatuses()
        {
            var statuses = await _userOrderRepository.GetOrderStatuses();
            return Ok(statuses);
        }

        [HttpPost("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusModel data)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid data provided" });

            try
            {
                await _userOrderRepository.ChangeOrderStatus(data);
                return Ok(new { Message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}









//using BookShoppingCartMvcUi.Constants;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;

//namespace BookShoppingCartMvcUi.Controllers;
//[Authorize(Roles =nameof(Roles.Admin))]

//public class AdminOperationsController : Controller
//{
//    private readonly IUserOrderRepository _userOrderRepository;

//    public AdminOperationsController(IUserOrderRepository userOrderRepository)
//    {
//        _userOrderRepository = userOrderRepository;
//    }
//    public async Task<IActionResult> AllOrders()
//    {
//        var orders = await _userOrderRepository.UserOrders(true);
//        return View(orders);
//    }
//    public async Task<IActionResult> TogglePaymentStatus(int orderId)
//    {
//        try
//        {
//            await _userOrderRepository.TogglePaymentStatus(orderId);
//        }
//        catch (Exception ex) 
//        { 

//        }
//        return RedirectToAction(nameof(AllOrders));
//    }

//    public async Task<IActionResult> UpdateOrderStatus(int orderId)
//    {
//        var order = await _userOrderRepository.GetOrderById(orderId);
//        if (order == null)
//        {
//            throw new InvalidOperationException($"Order with id:{orderId} does not found");
//        }
//        var orderStatusList = (await _userOrderRepository.GetOrderStatuses()).Select(orderStatus => {
//            return new SelectListItem
//            {
//                Value = orderStatus.Id.ToString(),
//                Text = orderStatus.StatusName,
//                Selected = order.OrderStatusId == orderStatus.Id
//            };
//        });
//        var data = new UpdateOrderStatusModel
//        {
//            OrderId = orderId,
//            OrderStatusId = order.OrderStatusId,
//            OrderStatusList = orderStatusList
//        };
//        return View(data);
//    }

//    [HttpPost]
//    public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel data)
//    {
//        try
//        {
//            if (!ModelState.IsValid)
//            {
//                data.OrderStatusList = (await _userOrderRepository.GetOrderStatuses()).Select(orderStatus =>
//                {
//                    return new SelectListItem
//                    {
//                        Value =
//                        orderStatus.Id.ToString(),
//                        Text =
//                        orderStatus.StatusName,
//                        Selected = orderStatus.Id == orderStatus.Id
//                    };
//                });
//                return View(data);
//            }
//            await _userOrderRepository.ChangeOrderStatus(data);
//            TempData["msg"] = "Updated Successfully";
//        }
//        catch (Exception ex) 
//        {
//            TempData["msg"] = "Something went wrong";
//        }
//        return RedirectToAction(nameof(UpdateOrderStatus), new { orderId = data.OrderId });
//    }

//    public IActionResult Dashboard()
//    {
//        return View();
//    }

//}
