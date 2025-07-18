﻿using E_learning.Model.Courses;

namespace E_learning.Repositories.Course
{
    public interface ICourseRepository
    {
       
            // Courses
            Task<List<CoursesModel>> GetAllCourses();
            Task<CoursesModel> GetCourseByID(string courseID);
            Task<bool> InsertCourse(CoursesModel course);
            Task<bool> DeleteCourse(string courseID);
       

        // Lessons
            Task<List<LessonModel>> GetLessonsByCourseID(string courseID);
            Task<bool> DeleteLessons(string courseID);
            Task<bool> InsertLesson(LessonModel lesson);
            Task<List<LessonModel>> GetAllLessons();
            Task<bool> checkBuyCourse(string userID, string lessonID);

        // Quizzes
        Task<List<QuizModel>> GetQuizzesByCourseID(string courseID);
            Task<bool> DeleteQuiz(string quizID);
            Task<bool> InsertQuiz(QuizModel quiz);
            Task<List<QuizModel>> GetAllQuiz();
        // Choices
        Task<List<ChoiceModel>> GetChoicesByQuizID(string quizID);
            Task<bool> DeleteChoice(string choiceID);
            Task<bool> InsertChoice(ChoiceModel choice);
            Task<List<ChoiceModel>> getAllChoice();
        // Questions
        Task<List<QuestionModel>> getALLQuestion();
        Task<bool> InsertQuestion(QuestionModel question);
        Task<bool> DeleteQuestion(string questionID);
        Task<List<QuestionModel>> GetQuestionsByQuizID(string quizID);

    }
}
