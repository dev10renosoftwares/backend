namespace Intern.ServiceModels.Exams
{
    public class DepartmentWithPostsSM
    {
        public DepartmentSM Department {  get; set; }

        public List<PostSM> Posts { get; set; }
    }
}
