using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.Intrinsics.X86;

namespace E_learning.Services
{
    public class Normalize
    {
        public static string RemoveDiacritics(string text)
        {
            string normalized = text.Normalize(NormalizationForm.FormD);
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            return regex.Replace(normalized, "")
                        .Replace('đ', 'd')
                        .Replace('Đ', 'D');
        }
    }
}


//use E_Learning;

//CREATE TABLE Users (
//    UserID nvarchar(25) PRIMARY KEY,
//    Username nvarchar(25),
//    Password nvarchar(255),
//    Email nvarchar(25),
//    FirstName nvarchar(25),
//    LastName nvarchar(25),
//    FullName nvarchar(25),
//    UserRole nvarchar(25),
//    CreateAt datetime DEFAULT GETDATE()
//);

//CREATE TABLE Admin (
//    UserID nvarchar(25) PRIMARY KEY,
//    FOREIGN KEY (UserID) REFERENCES Users(UserID)
//);

//CREATE TABLE Lecturers (
//    UserID nvarchar(25) PRIMARY KEY,
//    FOREIGN KEY (UserID) REFERENCES Users(UserID)
//);

//CREATE TABLE Students (
//    UserID nvarchar(25) PRIMARY KEY,
//    FOREIGN KEY (UserID) REFERENCES Users(UserID)
//);

//CREATE TABLE Courses (
//    CourseID nvarchar(25) PRIMARY KEY,
//    CourseName nvarchar(25),
//    CoursePrice decimal,
//    CourseDescription nvarchar(25),
//    AuthorID nvarchar(25),
//    CreateAt datetime DEFAULT GETDATE(),
//    FOREIGN KEY (AuthorID) REFERENCES Lecturers(UserID)
//);

//CREATE TABLE Enrollments (
//    EnrolmentID nvarchar(25) PRIMARY KEY,
//    StudentID nvarchar(25),
//    CourseID nvarchar(25),
//    CreateAt datetime DEFAULT GETDATE(),
//    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID),
//    FOREIGN KEY (StudentID) REFERENCES Students(UserID)
//);

//CREATE TABLE Lessons (
//    LessonID nvarchar(25) PRIMARY KEY,
//    LessonTitle nvarchar(25),
//    LessonURL nvarchar(255),
//    CreateAt datetime DEFAULT GETDATE(),
//    CourseID nvarchar(25),
//    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
//);

//CREATE TABLE Quiz (
//    QuizID nvarchar(25) PRIMARY KEY,
//    QuizTitle nvarchar(25),
//    CreateAt datetime DEFAULT GETDATE(),
//    CourseID nvarchar(25),
//    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
//);

//CREATE TABLE Question (
//    QuestionID nvarchar(25) PRIMARY KEY,
//    QuestionContent nvarchar(255),
//    CreateAt datetime DEFAULT GETDATE(),
//    QuizID nvarchar(25),
//    FOREIGN KEY (QuizID) REFERENCES Quiz(QuizID)
//);

//CREATE TABLE Choice (
//    ChoiceID nvarchar(25) PRIMARY KEY,
//    ChoiceText nvarchar(25),
//    isCorrect bit,
//    QuestionID nvarchar(25),
//    CreateAt datetime DEFAULT GETDATE(),
//    FOREIGN KEY (QuestionID) REFERENCES Question(QuestionID)
//);

//CREATE TABLE Payment (
//    PaymentID nvarchar(25) PRIMARY KEY,
//    Price decimal,
//    PaymentStatus nvarchar(25),
//    BuyerID nvarchar(25),
//    CourseID nvarchar(25),
//    CreateAt datetime DEFAULT GETDATE(),
//    FOREIGN KEY (BuyerID) REFERENCES Users(UserID),
//    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
//);

//CREATE TABLE MeetingRoom (
//    RoomID nvarchar(25) PRIMARY KEY,
//    RoomName nvarchar(25),
//    RoomURL nvarchar(255),
//    CreateAt datetime DEFAULT GETDATE(),
//    CreatorID nvarchar(25),
//    FOREIGN KEY (CreatorID) REFERENCES Users(UserID)
//);
