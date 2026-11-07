using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AcademyGraduationCeremony.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public string OfferName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }

        public int SupplierId { get; set; }

        [JsonIgnore]
        public Supplier Supplier { get; set; }

        [NotMapped]
        public int CategoryId => Supplier != null ? Supplier.CategoryId : 0;

        [NotMapped]
        public string CategoryName => Supplier != null && Supplier.Category != null ? Supplier.Category.Name : "";

        [NotMapped]
        public string SupplierName => Supplier != null ? Supplier.Name : "";
    }
}