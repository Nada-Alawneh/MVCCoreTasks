namespace CodeFirst.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}
