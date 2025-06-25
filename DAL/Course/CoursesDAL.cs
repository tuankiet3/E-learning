namespace E_learning.DAL.Course
{
    public class CoursesDAL
    {
        private readonly string _connectionString;
        private readonly ILogger<CoursesDAL> _logger;
        public CoursesDAL(string connectionString, ILogger<CoursesDAL> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }
    }
}
