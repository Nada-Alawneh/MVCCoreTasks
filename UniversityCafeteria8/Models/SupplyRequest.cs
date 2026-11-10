using Microsoft.AspNetCore.Identity;

namespace UniversityCafeteria8.Models
{
    public class SupplyRequest // display request details from the user and also used to manage the supply request
    {
        public int Id { get; set; }
        public DateTime RequestDate { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; }


        // Foreign key to the User who made the request
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        //one item can have many request items
        public List<RequestItem> RequestItems { get; set; }

    }
}
