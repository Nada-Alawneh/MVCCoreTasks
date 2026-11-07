namespace AcademyGraduationCeremony.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SupplierName { get; set; }
        public string OfferName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }
}
