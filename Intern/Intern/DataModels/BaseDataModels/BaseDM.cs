using System.ComponentModel.DataAnnotations;

namespace Intern.DataModels.BaseDataModels
{
    public class BaseDM
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime CreatedOnUtc { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedOnUtc { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}  
