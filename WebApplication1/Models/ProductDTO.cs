namespace WebApplication1.Models
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Image { get; set; } // This should be a URL to the image
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string ShortDescription { get; set; }
    }
}
