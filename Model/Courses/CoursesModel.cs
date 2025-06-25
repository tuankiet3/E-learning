namespace E_learning.Model.Courses
{
    public class CoursesModel
    {
        private string courseID { get; set; }
        private string courseName { get; set; }
        private decimal coursePrice { get; set; }
        private string authorID { get; set; }

        public string GetCourseID()
        {
            return courseID;
        }
        public void SetCourseID(string courseID)
        {
            this.courseID = courseID;
        }
        public string GetCourseName()
        {
            return courseName;
        }
        public void SetCourseName(string courseName)
        {
            this.courseName = courseName;
        }
        public decimal GetCoursePrice()
        {
            return coursePrice;
        }
        public void SetCoursePrice(decimal coursePrice)
        {
            this.coursePrice = coursePrice;
        }
        public string GetAuthorID()
        {
            return authorID;
        }
        public void SetAuthorID(string authorID)
        {
            this.authorID = authorID;
        }
        public CoursesModel(string courseID, string courseName, decimal coursePrice, string authorID)
        {
            this.courseID = courseID;
            this.courseName = courseName;
            this.coursePrice = coursePrice;
            this.authorID = authorID;
        }
    }
}
