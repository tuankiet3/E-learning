using System.Security.Cryptography;
namespace E_learning.Services
{
    public class GenerateID
    {
        // automatically generate unique IDs for Course
        public string generateCourseID() {
            byte[] bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            int number = Math.Abs(BitConverter.ToInt32(bytes, 0)) % 1000000;
            string numberString = number.ToString("D6");
            return $"CID{numberString}";
        }
        // automatically generate unique IDs for Lesson
        public string generateLessonID() {
            byte[] bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            int number = Math.Abs(BitConverter.ToInt32(bytes, 0)) % 1000000;
            string numberString = number.ToString("D6");
            return $"LID{numberString}";
        }
        // automatically generate unique IDs for Quiz
        public string generateQuizID() {
            byte[] bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            int number = Math.Abs(BitConverter.ToInt32(bytes, 0)) % 1000000;
            string numberString = number.ToString("D6");
            return $"QID{numberString}";
        }
        // automatically generate unique IDs for Choice
        public string generateChoiceID() {
            byte[] bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            int number = Math.Abs(BitConverter.ToInt32(bytes, 0)) % 1000000;
            string numberString = number.ToString("D6");
            return $"CHID{numberString}";
        }
    }
}
