namespace AcademyGraduationCeremony.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }

        
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // 1 supp __ M offer
        public ICollection<Offer> Offers { get; set; }
    }
}
