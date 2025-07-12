using E_learning.Model.Enrollment;

namespace E_learning.Repositories.Enrollment
{
    public interface IEnrollmentRepository
    {
        Task<List<EnrollmentModel>> getEnrollbyUserID(string userID);
        Task<bool> InsertEnrollment(EnrollmentModel enrollment);
        Task<List<EnrollmentModel>> GetAllEnrollments(int offset, int fetchnext);
        Task<List<string>> GetAllEnrollmentsID();
    }
}
