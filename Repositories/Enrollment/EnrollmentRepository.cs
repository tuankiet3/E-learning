using E_learning.DAL.Enrollment;
using E_learning.Model.Enrollment;

namespace E_learning.Repositories.Enrollment
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly EnrollmentDAL _enrollmentDAL;
        public EnrollmentRepository(EnrollmentDAL enrollmentDAL)
        {
            _enrollmentDAL = enrollmentDAL;
        }
        public Task<bool> InsertEnrollment(EnrollmentModel enrollment) => _enrollmentDAL.InsertEnrollment(enrollment);
        public Task<List<EnrollmentModel>> GetAllEnrollments() => _enrollmentDAL.GetAllEnrollments();
        public Task<List<EnrollmentModel>> getEnrollbyUserID(string userID) => _enrollmentDAL.getEnrollbyUserID(userID);
    }
}
