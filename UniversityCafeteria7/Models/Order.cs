namespace UniversityCafeteria7.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public Employee? employee { get; set; }
    }
}
