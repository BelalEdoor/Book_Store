namespace BookShopping_Ecommerce.Models.DTOs
{
    public class OrderDetailModalDTO
    {
        public string DivId { get; set; }
        public IEnumerable<OrderDetails> OrderDetail { get; set; }
    }
}
