namespace AcademyGraduationCeremony.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // 1 cat __ M supp
        public ICollection<Supplier> Suppliers { get; set; }
    }
}