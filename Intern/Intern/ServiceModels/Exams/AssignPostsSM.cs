namespace Intern.ServiceModels.Exams
{
    public class AssignPostsSM
    {
        public int DepartmentId { get; set; }
        public List<int> PostIds { get; set; } = new List<int>();
    }
}
