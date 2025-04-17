USE master
GO

CREATE DATABASE SchoolSchedule
GO

USE SchoolSchedule
GO



CREATE TABLE [Group](
	Id		INT IDENTITY PRIMARY KEY,
	[Year]	INT NOT NULL,
	[Name] 	NCHAR(1) NOT NULL
)
ALTER TABLE [Group] ADD CONSTRAINT
GroupNameConstraint CHECK (LOWER([Name])>='а' AND LOWER([Name])<='е')
GO
--CREATE TRIGGER GroupUpperCaseNameInsertTrigger
--ON [Group]
--INSTEAD OF INSERT
--AS
--BEGIN
--	SET NOCOUNT ON;
--	
--	INSERT INTO [Group]
--		([Year],[Name])
--		SELECT [Year],UPPER([Name])
--		FROM inserted;
--END;
--GO
--CREATE TRIGGER GroupUpperCaseNameUpdateTrigger
--ON [Group]
--INSTEAD OF UPDATE
--AS
--BEGIN
--	SET NOCOUNT ON;
--	
--	UPDATE [Group]
--	SET
--		[Name]=UPPER(inserted.[Name]),
--		[Year]=inserted.[Year]
--	FROM
--		[Group]
--	INNER JOIN
--		inserted ON [Group].Id=inserted.Id
--END;
--GO


CREATE TABLE Student(
	Id			INT IDENTITY PRIMARY KEY,
	[Name]		NVARCHAR(30) NOT NULL,
	Surname		NVARCHAR(30) NOT NULL,
	Patronymic	NVARCHAR(30) NOT NULL,
	IdGroup		INT NOT NULL,
	Email		NVARCHAR(60),
	FOREIGN KEY (IdGroup)	REFERENCES [Group]
)
ALTER TABLE Student ADD CONSTRAINT
RussianStudentName CHECK (NOT [Name] like '%[a-zA-Z0-9]%')

ALTER TABLE Student ADD CONSTRAINT
RussianStudentPatronymic CHECK (NOT [Patronymic] like '%[a-zA-Z0-9]%')

ALTER TABLE Student ADD CONSTRAINT
RussianStudentSurname CHECK (NOT [Surname] like '%[a-zA-Z0-9]%')

ALTER TABLE Student ADD CONSTRAINT
StudentEmailCheck CHECK ([Email] IS NULL OR [Email] like '%@%.%')



CREATE TABLE Subject(
	Id			INT IDENTITY PRIMARY KEY,
	[Name]		NVARCHAR(70) NOT NULL,
)



CREATE TABLE Lesson(
	Id			INT IDENTITY PRIMARY KEY,
	IdSubject	INT NOT NULL,
	IdGroup		INT NOT NULL,
	[Number]	INT NOT NULL,
	FOREIGN KEY (IdSubject)	REFERENCES Subject,
	FOREIGN KEY	(IdGroup)	REFERENCES [Group]
)
ALTER TABLE Lesson ADD CONSTRAINT
LessonNumberCheck CHECK ([Number]>=1 AND [Number]<=8)


CREATE TABLE Teacher(
	Id			INT IDENTITY PRIMARY KEY,
	[Name]		NVARCHAR(30) NOT NULL,
	Surname		NVARCHAR(30) NOT NULL,
	Patronymic	NVARCHAR(30) NOT NULL,
)
ALTER TABLE Teacher ADD CONSTRAINT RussianTeacherName
CHECK (NOT [Name] like '%[a-zA-Z0-9]%')

ALTER TABLE Teacher ADD CONSTRAINT
RussianTeacherPatronymic CHECK (NOT [Patronymic] like '%[a-zA-Z0-9]%')

ALTER TABLE Teacher ADD CONSTRAINT 
RussianTeacherSurname CHECK (NOT [Surname] like '%[a-zA-Z0-9]%')



CREATE TABLE TeacherSubject(
	IdTeacher	INT,
	IdSubject	INT,
	FOREIGN KEY (IdTeacher)	REFERENCES Teacher,
	FOREIGN KEY (IdSubject)	REFERENCES Subject,
	PRIMARY KEY	(IdTeacher,IdSubject)
)



CREATE TABLE TeacherPhone(
	Id			INT IDENTITY PRIMARY KEY,
	IdTeacher	INT NOT NULL,
	PhoneNumber	NVARCHAR(16) NOT NULL,
	FOREIGN KEY	(IdTeacher)	REFERENCES Teacher
)
ALTER TABLE TeacherPhone ADD CONSTRAINT TeacherPhoneConstraint
CHECK ([PhoneNumber] like '+7 [0-9][0-9][0-9] [0-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9]')



CREATE TABLE Schedule(
	Id			INT IDENTITY PRIMARY KEY,
	IdLesson	INT NOT NULL,
	IdTeacher	INT NOT NULL,
	StartTime	TIME(7) NOT NULL,
	EndTime 	TIME(7) NOT NULL,
	[Date] 		DATE NOT NULL,
	FOREIGN KEY (IdLesson)	REFERENCES Lesson,
	FOREIGN KEY (IdTeacher)	REFERENCES Teacher
)
ALTER TABLE Schedule ADD CONSTRAINT
ScheduleCurrentTime CHECK ([StartTime]<[EndTime])



CREATE TABLE ClassTeacher(
	IdTeacher	INT,
	IdGroup		INT,
	FOREIGN KEY	(IdTeacher)	REFERENCES Teacher,
	FOREIGN KEY (IdGroup)	REFERENCES [Group],
	PRIMARY KEY	(IdTeacher,IdGroup)
)
GO


CREATE   PROC ShowLessonsAtDayForTeacher
@surname	NCHAR(30),
@name		NCHAR(30),
@patronymic	NCHAR(30),
@date		DATE
AS
BEGIN
	SELECT [Subject].[Name] AS 'Предмет',Lesson.Number AS 'Номер урока',FORMAT(Schedule.StartTime,N'hh\:mm') AS 'Время начала урока',FORMAT(Schedule.EndTime,N'hh\:mm') AS 'Время завершения урока'
	FROM Schedule, Teacher, [Subject], Lesson
	WHERE 
		Teacher.Id		=	Schedule.IdTeacher	AND
		Lesson.Id		=	Schedule.IdLesson	AND
		[Subject].Id	=	Lesson.IdSubject	AND
		Schedule.[Date]		=	@date		AND
		Teacher.[Name]		=	@name		AND
		Teacher.Surname		=	@surname	AND
		Teacher.Patronymic	=	@patronymic
	ORDER BY Lesson.Number


	IF @@ROWCOUNT = 0
	BEGIN
		PRINT CONCAT('Уроки для преподавателя ',@surname,' ',@name,' ',@patronymic,' на ',@date, 'не найдены');
	END
	ELSE
	BEGIN
		DECLARE @sql NVARCHAR(MAX)
		SET @sql=N'
		CREATE OR ALTER VIEW LessonsAtDayForTeacher AS
			SELECT [Subject].[Name] AS ''Предмет'',Lesson.Number AS ''Номер урока'',Schedule.StartTime AS ''Время начала урока'',Schedule.EndTime AS ''Время завершения урока''
			FROM Schedule, Teacher, [Subject], Lesson
			WHERE 
				Teacher.Id		=	Schedule.IdTeacher	AND
				Lesson.Id		=	Schedule.IdLesson	AND
				[Subject].Id	=	Lesson.IdSubject	AND
				Schedule.[Date]		=	'''+CONVERT(NVARCHAR(MAX), @date, 23) +'''		AND
				Teacher.[Name]		=	'''+@name+'''		AND
				Teacher.Surname		=	'''+@surname+'''	AND
				Teacher.Patronymic	=	'''+@patronymic+''''


		EXEC sp_executesql @sql
	END;
END;
GO

CREATE PROC ShowStudentsByGroup
@year		INT,
@name		NCHAR(1)
AS
BEGIN
	DECLARE @groupId INT;
	SELECT @groupId = Id
	FROM [Group]
	WHERE [Group].[Year]=@year AND [Group].[Name]=UPPER(@name)
	
	DECLARE @sql NVARCHAR(MAX);
	
	IF @groupId IS NULL
	BEGIN
		PRINT CONCAT('Класс ',@year,UPPER(@name),' не найден');
		
		
		SET @sql = N'CREATE OR ALTER VIEW GroupView AS
			SELECT [Name] AS Имя,
			Surname AS Фамилия,
			Patronymic AS Отчество
			FROM Student
			WHERE IdGroup IS NULL';
		EXEC sp_executesql @sql;
	END;
	ELSE
	BEGIN
		SELECT [Name] AS 'Имя', Surname AS 'Фамилия', Patronymic AS 'Отчество'
		FROM Student
		WHERE IdGroup=@groupId

		SET @sql = N'CREATE OR ALTER VIEW GroupView AS
			SELECT [Name] AS Имя,
			Surname AS Фамилия,
			Patronymic AS Отчество
			FROM Student
			WHERE IdGroup = ' + CAST(@groupId AS NVARCHAR(10));

		EXEC sp_executesql @sql;
	END;
END;
GO







--Ввод данных
INSERT INTO [Group] ([Year], [Name]) VALUES 
(1, 'А'),
(1, 'Б'),
(1, 'В'),
(2, 'А'),
(2, 'Б'),
(2, 'В')
GO


INSERT INTO Student ([Name], Surname, Patronymic, IdGroup, Email) VALUES 
('Иван', 'Свофорд', 'Кузьмич', 1, 'zvzov@mail.ru'),
('Сигма', 'Сигма', 'Сигманович', 5, '228777XXXzxcSigma@yandex.ru'),
('Гой', 'Путин', 'Петрович', 3, NULL)
GO


INSERT INTO Subject ([Name]) VALUES 
('Математика'),
('ОБЖ'),
('Информатика'),
('Алгебра')
GO


INSERT INTO Lesson (IdSubject, IdGroup, [Number]) VALUES 
(2, 1, 1),
(2, 1, 2),
(2, 1, 3),
(2, 1, 4),
(2, 1, 5),
(2, 1, 6),
(2, 1, 7)
GO

INSERT INTO Teacher ([Name], Surname, Patronymic) VALUES 
('Марина', 'Погорелова', 'Николаевна'),
('Сергей', 'Дегтярёв', 'Николаевич'),
('Владимир', 'Косицкий', 'Александрович'),
('Иван', 'Грахов', 'Вадимович')
GO


INSERT INTO Schedule (IdLesson, IdTeacher, StartTime, EndTime, [Date]) VALUES 
(1, 2, CAST('08:00:00' AS Time), CAST('08:40:00' AS Time), CAST('2025-10-04' AS Date)),
(2, 2, CAST('08:50:00' AS Time), CAST('09:30:00' AS Time), CAST('2025-10-04' AS Date)),
(3, 2, CAST('09:40:00' AS Time), CAST('10:20:00' AS Time), CAST('2025-10-04' AS Date))
GO




INSERT INTO  TeacherPhone (IdTeacher, PhoneNumber) VALUES 
(1, '+7 123 456-78-90'),
(1, '+7 342 555-77-90'),
(2, '+7 245 222-33-22'),
(3, '+7 890 374-85-82'),
(4, '+7 452 214-53-51')
GO









USE [master]
