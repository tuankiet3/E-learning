using E_learning.DAL.Course;
using E_learning.Model.Courses;

namespace E_learning.Repositories
{
    public class CourseRepository : ICourseRepository
    {
         
        private readonly CoursesDAL _coursesDAL;
        private readonly LessonDAL _lessonDAL;
        private readonly QuizDAL _quizDAL;
        private readonly ChoiceDAL _choiceDAL;

        public CourseRepository(
            CoursesDAL coursesDAL,
            LessonDAL lessonDAL,
            QuizDAL quizDAL,
            ChoiceDAL choiceDAL)
        {
            _coursesDAL = coursesDAL;
            _lessonDAL = lessonDAL;
            _quizDAL = quizDAL;
            _choiceDAL = choiceDAL;
        }
        // Course methods
        public Task<List<CoursesModel>> GetAllCourses() => _coursesDAL.getAllCourse();
        public Task<CoursesModel> GetCourseByID(string courseID) => _coursesDAL.getCourseByID(courseID);
        public Task<bool> InsertCourse(CoursesModel course) => _coursesDAL.InsertCourse(course);
        public Task<bool> DeleteCourse(string courseID) => _coursesDAL.deleteCourse(courseID);
        // Lesson methods
        public Task<List<LessonModel>> GetLessonsByCourseID(string courseID) => _lessonDAL.GetLessonByCourseID(courseID);
        public Task<bool> DeleteLessons(string lessionID) => _lessonDAL.deleteLesson(lessionID);
        public Task<bool> InsertLesson(LessonModel lesson) => _lessonDAL.insertLesson(lesson);
        // Quiz methods
        public Task<List<QuizModel>> GetQuizzesByCourseID(string courseID) => _quizDAL.GetQuizByCourseID(courseID);
        public Task<bool> DeleteQuiz(string quizID) => _quizDAL.DeleteQuiz(quizID);
        public Task<bool> InsertQuiz(QuizModel quiz) => _quizDAL.insertQuiz(quiz);
        // Choice methods
        public Task<List<ChoiceModel>> GetChoicesByQuizID(string quizID) => _choiceDAL.GetChoicesByQuizID(quizID);
        public Task<bool> DeleteChoice(string choiceID) => _choiceDAL.deleteChoice(choiceID);
        public Task<bool> InsertChoice(ChoiceModel choice) => _choiceDAL.InsertChoice(choice);
    }
}

