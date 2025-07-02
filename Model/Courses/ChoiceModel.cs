namespace E_learning.Model.Courses
{
    public class ChoiceModel
    {
        private string ChoiceID { get; set; }
        private string ChoiceText { get; set; }
        private bool IsCorrect { get; set; }
        private string QuestionID { get; set; }

        public string ChoiceId => ChoiceID;
        public string Text => ChoiceText;
        public bool Correct => IsCorrect;
        public string QuestionId => QuestionID;
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
        public string getQuestionID()
        {
            return QuestionID;
        }
        public void setQuestionID(string questionID)
        {
            QuestionID = questionID;
        }
        public ChoiceModel(string choiceID, string choiceText, bool isCorrect, string questionID)
        {
            ChoiceID = choiceID;
            ChoiceText = choiceText;
            IsCorrect = isCorrect;
            QuestionID = questionID;
        }

    }
}
