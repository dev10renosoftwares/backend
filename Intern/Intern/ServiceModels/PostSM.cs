namespace Intern.ServiceModels
{
    public class PostSM
    {

        public int Id { get; set; }
        public string PostName { get; set; }
        public string Description { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedOnUtc { get; set; }
        public string? LastModifiedBy { get; set; }
    }


    public class AddPostSM
    {
        public string? PostName { get; set; }
        public string? Description { get; set; }
    }
}
