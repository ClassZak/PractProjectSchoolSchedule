USE master
GO

CREATE DATABASE SchoolSchedule
GO

USE SchoolSchedule
GO

CREATE TABLE BellScheduleType(
    Id      INT IDENTITY PRIMARY KEY,
    Name    NVARCHAR(50) NOT NULL
)
GO

CREATE TABLE BellSchedule(
    Id                  INT IDENTITY PRIMARY KEY,
    IdBellScheduleType  INT NOT NULL,
    LessonNumber        INT NOT NULL,
    StartTime           TIME(7) NOT NULL,
    EndTime            TIME(7) NOT NULL
)
GO

ALTER TABLE BellSchedule ADD CONSTRAINT
FK_BellSchedule_BellScheduleType FOREIGN KEY (IdBellScheduleType) REFERENCES BellScheduleType
GO

CREATE TABLE [Group](
    Id      INT IDENTITY PRIMARY KEY,
    Year    INT NOT NULL,
    Name    NCHAR(1) NOT NULL
)
GO

ALTER TABLE [Group] ADD CONSTRAINT
GroupNameConstraint CHECK (LOWER(Name) >= 'а' AND LOWER(Name) <= 'е')
GO

CREATE TABLE Subject(
    Id      INT IDENTITY PRIMARY KEY,
    Name    NVARCHAR(70) NOT NULL
)
GO

CREATE TABLE Teacher(
    Id          INT IDENTITY PRIMARY KEY,
    Name        NVARCHAR(30) NOT NULL,
    Surname     NVARCHAR(30) NOT NULL,
    Patronymic  NVARCHAR(30) NOT NULL
)
GO

ALTER TABLE Teacher ADD CONSTRAINT RussianTeacherName
CHECK (NOT Name LIKE '%[a-zA-Z0-9]%')

ALTER TABLE Teacher ADD CONSTRAINT RussianTeacherPatronymic
CHECK (NOT Patronymic LIKE '%[a-zA-Z0-9]%')

ALTER TABLE Teacher ADD CONSTRAINT RussianTeacherSurname
CHECK (NOT Surname LIKE '%[a-zA-Z0-9]%')
GO

CREATE TABLE TeacherSubject(
    IdTeacher   INT,
    IdSubject   INT,
    PRIMARY KEY (IdTeacher, IdSubject)
)
GO

ALTER TABLE TeacherSubject ADD FOREIGN KEY (IdTeacher) REFERENCES Teacher
ALTER TABLE TeacherSubject ADD FOREIGN KEY (IdSubject) REFERENCES Subject
GO

CREATE TABLE TeacherPhone(
    Id          INT IDENTITY PRIMARY KEY,
    IdTeacher   INT NOT NULL,
    PhoneNumber NVARCHAR(16) NOT NULL
)
GO

ALTER TABLE TeacherPhone ADD CONSTRAINT TeacherPhoneConstraint
CHECK (PhoneNumber LIKE '+7 [0-9][0-9][0-9] [0-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9]')

ALTER TABLE TeacherPhone ADD FOREIGN KEY (IdTeacher) REFERENCES Teacher
GO

CREATE TABLE Student(
    Id          INT IDENTITY PRIMARY KEY,
    Name        NVARCHAR(30) NOT NULL,
    Surname     NVARCHAR(30) NOT NULL,
    Patronymic  NVARCHAR(30) NOT NULL,
    IdGroup     INT NOT NULL,
    Email       NVARCHAR(60)
)
GO

ALTER TABLE Student ADD CONSTRAINT RussianStudentName
CHECK (NOT Name LIKE '%[a-zA-Z0-9]%')

ALTER TABLE Student ADD CONSTRAINT RussianStudentSurname
CHECK (NOT Surname LIKE '%[a-zA-Z0-9]%')

ALTER TABLE Student ADD CONSTRAINT RussianStudentPatronymic
CHECK (NOT Patronymic LIKE '%[a-zA-Z0-9]%')

ALTER TABLE Student ADD CONSTRAINT StudentEmailCheck
CHECK (Email IS NULL OR Email LIKE '%@%.%')

ALTER TABLE Student ADD FOREIGN KEY (IdGroup) REFERENCES [Group]
GO

CREATE TABLE Schedule(
    Id              INT IDENTITY PRIMARY KEY,
    IdSubject       INT NOT NULL,
    IdGroup         INT NOT NULL,
    IdTeacher       INT NOT NULL,
    IdBellSchedule  INT NOT NULL,
    DayOfTheWeek    INT NOT NULL,
    ClassRoom       INT NOT NULL
)
GO

ALTER TABLE Schedule ADD FOREIGN KEY (IdSubject) REFERENCES Subject
ALTER TABLE Schedule ADD FOREIGN KEY (IdGroup) REFERENCES [Group]
ALTER TABLE Schedule ADD FOREIGN KEY (IdTeacher) REFERENCES Teacher
ALTER TABLE Schedule ADD FOREIGN KEY (IdBellSchedule) REFERENCES BellSchedule
GO

CREATE TABLE LessonSubsitutionSchedule(
    Id          	INT IDENTITY PRIMARY KEY,
    Date        	DATE NOT NULL,
    IdSubject   	INT,
    IdGroup     	INT,
    IdTeacher   	INT,
    ClassRoom   	INT,
	LessonNumber	INT
)
GO

ALTER TABLE LessonSubsitutionSchedule ADD FOREIGN KEY (IdSubject) REFERENCES Subject
ALTER TABLE LessonSubsitutionSchedule ADD FOREIGN KEY (IdGroup) REFERENCES [Group]
ALTER TABLE LessonSubsitutionSchedule ADD FOREIGN KEY (IdTeacher) REFERENCES Teacher
GO

CREATE TABLE ClassTeacher(
    IdTeacher   INT,
    IdGroup     INT,
    PRIMARY KEY (IdTeacher, IdGroup)
)
GO

ALTER TABLE ClassTeacher ADD FOREIGN KEY (IdTeacher) REFERENCES Teacher
ALTER TABLE ClassTeacher ADD FOREIGN KEY (IdGroup) REFERENCES [Group]
GO