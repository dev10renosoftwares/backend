using System.ComponentModel.DataAnnotations;

namespace Intern.ServiceModels.BaseServiceModels
{
    public class BaseSM
    {
        public int? Id { get; set; }
        public DateTime CreatedOnUtc { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedOnUtc { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
