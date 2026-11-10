namespace UniversityCafeteria8.Models
{
    public class Product // display product details in the menu and also used in the supply request
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
