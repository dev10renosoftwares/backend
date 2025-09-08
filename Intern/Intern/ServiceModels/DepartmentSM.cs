namespace Intern.ServiceModels
{
    public class DepartmentSM
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedOnUtc { get; set; }
        public string? LastModifiedBy { get; set; }
    }



}
