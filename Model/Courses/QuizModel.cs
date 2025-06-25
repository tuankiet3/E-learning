namespace E_learning.Model.Courses
{
    public class QuizModel
    {
        private string quizID { get; set; }
        private string quizTitle { get; set; }
        private string courseID { get; set; }
        public string getQuizID()
        {
            return quizID;
        }
        public void setQuizID(string quizID)
        {
            this.quizID = quizID;
        }
        public string getQuizTitle()
        {
            return quizTitle;
        }
        public void setQuizTitle(string quizTitle)
        {
            this.quizTitle = quizTitle;
        }
        public string getCourseID()
        {
            return courseID;
        }
        public void setCourseID(string courseID)
        {
            this.courseID = courseID;
        }
        public QuizModel(string quizID, string quizTitle, string courseID)
        {
            this.quizID = quizID;
            this.quizTitle = quizTitle;
            this.courseID = courseID;
        }
    }
}
