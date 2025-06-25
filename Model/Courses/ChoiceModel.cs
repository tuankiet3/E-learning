namespace E_learning.Model.Courses
{
    public class ChoiceModel
    {
        private string ChoiceID { get; set; }
        private string ChoiceText { get; set; }
        private bool IsCorrect { get; set; }
        private string QuizID { get; set; }

        public string GetChoiceID()
        {
            return ChoiceID;
        }
        public void SetChoiceID(string choiceID)
        {
            ChoiceID = choiceID;
        }
        public string GetChoiceText()
        {
            return ChoiceText;
        }
        public void SetChoiceText(string choiceText)
        {
            ChoiceText = choiceText;
        }
        public bool GetIsCorrect()
        {
            return IsCorrect;
        }
        public void SetIsCorrect(bool isCorrect)
        {
            IsCorrect = isCorrect;
        }
        public string GetQuizID()
        {
            return QuizID;
        }
        public void SetQuizID(string quizID)
        {
            QuizID = quizID;
        }
        public ChoiceModel(string choiceID, string choiceText, bool isCorrect, string quizID)
        {
            ChoiceID = choiceID;
            ChoiceText = choiceText;
            IsCorrect = isCorrect;
            QuizID = quizID;
        }

    }
}
