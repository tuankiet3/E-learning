namespace E_learning.Model.Courses
{
    public class LessonModel
    {
        private string lessonID { get; set; }
        private string lessonTitle { get; set; }
        private string lessonURL { get; set; }
        private string courseID { get; set; }

        // Public properties to expose the fields
        public string LessonID => lessonID;
        public string LessonTitle => lessonTitle;
        public string LessonURL => lessonURL;
        public string CourseID => courseID;
        // Getter/Setter methods
        public string GetLessonID()
        {
            return lessonID;
        }
        public void SetLessonID(string lessonID)
        {
            this.lessonID = lessonID;
        }

        public string GetLessonTitle()
        {
            return lessonTitle;
        }
        public void SetLessonTitle(string lessonTitle)
        {
            this.lessonTitle = lessonTitle;
        }
        public string GetLessonURL()
        {
            return lessonURL;
        }
        public void SetLessonURL(string lessonURL)
        {
            this.lessonURL = lessonURL;
        }
        public string GetCourseID()
        {
            return courseID;
        }
        public void SetCourseID(string courseID)
        {
            this.courseID = courseID;
        }
        public LessonModel(string lessonID, string lessonTitle, string lessonURL, string courseID)
        {
            this.lessonID = lessonID;
            this.lessonTitle = lessonTitle;
            this.lessonURL = lessonURL;
            this.courseID = courseID;
        }
    }
}
