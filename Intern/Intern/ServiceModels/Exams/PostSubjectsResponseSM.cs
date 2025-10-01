namespace Intern.ServiceModels.Exams
{
    public class PostSubjectsResponseSM
    {
        public PostSM Post { get; set; }
        public List<PostSubjectRelationSM> Subjects { get; set; }
    }

    public class PostSubjectRelationSM
    {
        public int SubjectPostId { get; set; }
        public SubjectSM Subject { get; set; }
    }

}
