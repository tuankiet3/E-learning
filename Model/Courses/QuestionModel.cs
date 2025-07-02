namespace E_learning.Model.Courses
{
    public class QuestionModel
    {
        private string QuestionID { get; set; }
        private string QuestionContent { get; set; }
        private string QuizID { get; set; }

        // ✅ Public properties chỉ để expose ra Web API
        public string questionId => QuestionID;
        public string questionContent => QuestionContent;
        public string quizId => QuizID;
        // ✅ Getter/Setter methods (nếu bạn vẫn muốn sử dụng trong code)
        public string GetQuestionID()
        {
            return QuestionID;
        }
        public void SetQuestionID(string questionID)
        {
            this.QuestionID = questionID;
        }

        public string GetQuestionContent()
        {
            return QuestionContent;
        }
        public void SetQuestionContent(string questionContent)
        {
            this.QuestionContent = questionContent;
        }
        public string GetQuizID()
        {
            return QuizID;
        }
        public void SetQuizID(string quizID)
        {
            this.QuizID = quizID;
        }
        public QuestionModel(string questionID, string questionContent, string quizID)
        {
            this.QuestionID = questionID;
            this.QuestionContent = questionContent;
            this.QuizID = quizID;
        }
    }
}
