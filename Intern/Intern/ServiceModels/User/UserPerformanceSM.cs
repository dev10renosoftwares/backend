namespace Intern.ServiceModels.User
{
    public class UserPerformanceSM : ClientUserSM
    {   
            public int TotalTestsTaken { get; set; }
            public int TotalRemainingTests { get; set; }
            public double AverageMarks { get; set; }

            public TopPerformanceSM TopPerformance { get; set; }
     
        public class TopPerformanceSM
        {
            public PostSM Post { get; set; }   
            public double Marks { get; set; }  
        }

       
    }
}
