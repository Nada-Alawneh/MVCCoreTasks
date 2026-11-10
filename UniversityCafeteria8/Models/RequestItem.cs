namespace UniversityCafeteria8.Models
{
    public class RequestItem
    {
        public int Id { get; set; }

       //ربط العنصر بالطلب الرئيسي
        public int SupplyRequestId { get; set; }
        public SupplyRequest SupplyRequest { get; set; }
        //ربط العنصر بالمنتج
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

    }
}
