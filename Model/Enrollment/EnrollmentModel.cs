namespace E_learning.Model.Enrollment
{
    public class EnrollmentModel
    {
        private string enrollmenID { get; set; }
        private string userID { get; set; }
        private string courseID { get; set; }

        // Public properties to expose data
        public string EnrollmentID => enrollmenID;
        public string UserID => userID;
        public string CourseID => courseID;
        // Getter/Setter methods
        public string GetEnrollmentID()
        {
            return enrollmenID;
        }
        public void SetEnrollmentID(string enrollmentID)
        {
            this.enrollmenID = enrollmentID;
        }
        public string GetUserID()
        {
            return userID;
        }
        public void SetUserID(string userID)
        {
            this.userID = userID;
        }
        public string GetCourseID()
        {
            return courseID;
        }
        public void SetCourseID(string courseID)
        {
            this.courseID = courseID;
        }
        public EnrollmentModel(string enrollmentID, string userID, string courseID)
        {
            this.enrollmenID = enrollmentID;
            this.userID = userID;
            this.courseID = courseID;
        }
    }
}
