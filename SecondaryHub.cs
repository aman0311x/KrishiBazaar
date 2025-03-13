using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KrishiBazaarProject.Models
{
    public class SecondaryHub
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SecondaryHubID { get; set; }

        public int? PrimaryHubID { get; set; }
        [ForeignKey("PrimaryHubID")]
        public PrimaryHub PrimaryHub { get; set; }

        public int LocationID { get; set; }
        [ForeignKey("LocationID")]
        public Location Location { get; set; }

        [Required]
        public int Capacity { get; set; }
    }
}
