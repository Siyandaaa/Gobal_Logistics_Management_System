using System.ComponentModel.DataAnnotations;

namespace Global_Logistics_Management_System.Models.Entities
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }
        [Required, StringLength(100)]
        public string? Name { get; set; }
        [Required, StringLength(200)]
        public string? ContactDetails { get; set; }
        [Required, StringLength(50)]
        public string? Region { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
