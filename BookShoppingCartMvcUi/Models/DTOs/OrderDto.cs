namespace BookShoppingCartMvcUi.Models.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public string OrderStatus { get; set; }
        public IEnumerable<OrderDetailDto> OrderDetails { get; set; }
    }
    public class OrderDetailDto
    {
        public string BookName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Image { get; set; }
        public string GenreName { get; set; }
    }
}
