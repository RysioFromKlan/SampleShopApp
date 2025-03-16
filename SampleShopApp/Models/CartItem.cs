namespace SampleShopApp.Models
{
    /// <summary>
    /// Dane wysyłane dla frontendu
    /// Data sent for frontend
    /// </summary>
    public class CartItem
    {
        public int ProductId { get; set; }
        public required string ProductName {  get; set; }
        public required float Price { get; set; }
        public int Quantity { get; set; }
    }
}
    

