namespace E_learning.Model.Courses
{
    public class CoursesModel
    {
        // ✅ Private fields (được đóng gói)
        private string courseID { get; set; }
        private string courseName { get; set; }
        private decimal coursePrice { get; set; }
        private string courseDescription { get; set; }
        private string authorID { get; set; }

        // ✅ Public properties chỉ để expose ra Web API
        public string CourseID => courseID;
        public string CourseName => courseName;
        public decimal CoursePrice => coursePrice;
        public string CourseDescription => courseDescription;
        public string AuthorID => authorID;

        // ✅ Getter/Setter methods (nếu bạn vẫn muốn sử dụng trong code)
        public string GetCourseID() => courseID;
        public void SetCourseID(string value) => courseID = value;

        public string GetCourseName() => courseName;
        public void SetCourseName(string value) => courseName = value;

        public decimal GetCoursePrice() => coursePrice;
        public void SetCoursePrice(decimal value) => coursePrice = value;

        public string GetCourseDescription() => courseDescription;
        public void SetCourseDescription(string value) => courseDescription = value;

        public string GetAuthorID() => authorID;
        public void SetAuthorID(string value) => authorID = value;

        // ✅ Constructors
        public CoursesModel(string courseID, string courseName, decimal coursePrice, string courseDescription, string authorID)
        {
            this.courseID = courseID;
            this.courseName = courseName;
            this.coursePrice = coursePrice;
            this.courseDescription = courseDescription;
            this.authorID = authorID;
        }

        public CoursesModel() { }
    }
}
