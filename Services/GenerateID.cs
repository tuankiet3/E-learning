using System.Security.Cryptography;
namespace E_learning.Services
{
    public class GenerateID
    {
        // automatically generate unique IDs for Course
        public string generateID()
        {
            byte[] bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            int number = Math.Abs(BitConverter.ToInt32(bytes, 0)) % 1000000;
            string numberString = number.ToString("D6");
            return numberString;
        }
        public string generateCourseID() {
            string numberString = generateID();
            return $"CID{numberString}";
        }
        // automatically generate unique IDs for Lesson
        public string generateLessonID() {
            string numberString = generateID();
            return $"LID{numberString}";
        }
        // automatically generate unique IDs for Quiz
        public string generateQuizID() {
            string numberString = generateID();
            return $"QID{numberString}";
        }
        // automatically generate unique IDs for Choice
        public string generateChoiceID() {
            string numberString = generateID();
            return $"CHID{numberString}";
        }

        public string generateUserID()
        {
            string numberString = generateID();
            return $"UID{numberString}";
        }
        public string GenerateEnrollmentID()
        {
            string numberString = generateID();
            return $"ENID{numberString}";
        }
        public string GeneratePaymentID()
        {
            string numberString = generateID();
            return $"PID{numberString}";
        }

    }
}
