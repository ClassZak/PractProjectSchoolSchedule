USE master
GO

CREATE DATABASE SchoolSchedule
GO

USE SchoolSchedule
GO




CREATE TABLE BellScheduleType(
	Id		INT IDENTITY PRIMARY KEY,
	Name	NVARCHAR(50) NOT NULL
)
ALTER TABLE BellScheduleType ADD CONSTRAINT
RussianBellScheduleTypeConstraint CHECK (NOT Name LIKE '%[a-zA-Z]%')


CREATE TABLE BellSchedule(
	Id					INT IDENTITY PRIMARY KEY,
	IdBellScheduleType	INT NOT NULL,
	LessonNumber		INT NOT NULL,
	StartTime			TIME(7) NOT NULL,
	EndTime				TIME(7) NOT NULL,
	FOREIGN KEY			(IdBellScheduleType) REFERENCES BellScheduleType
)
ALTER TABLE BellSchedule ADD CONSTRAINT 
UniquesBellScheduleTypeAndLessonNumberConstraint UNIQUE (IdBellScheduleType, LessonNumber)
ALTER TABLE BellSchedule ADD CONSTRAINT
BellScheduleCurrentTime CHECK ([StartTime]<[EndTime])
ALTER TABLE BellSchedule ADD CONSTRAINT UniqueStartEndTime UNIQUE (IdBellScheduleType, StartTime, EndTime);




CREATE TABLE [Group](
	Id		INT IDENTITY PRIMARY KEY,
	[Year]	INT NOT NULL,
	Name	NCHAR(1) NOT NULL
)
ALTER TABLE [Group] ADD CONSTRAINT
GroupNameConstraint CHECK (LOWER(Name) >= 'а' AND LOWER(Name) <= 'е')
ALTER TABLE [Group] ADD CONSTRAINT
GroupYearConstraint CHECK ([Year] BETWEEN 1 AND 11)



GO
CREATE TRIGGER GroupUpperCaseNameTrigger
ON [Group]
AFTER INSERT, UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE [Group]
	SET
		[Name]=UPPER([Name])
END;
GO




CREATE TABLE Subject(
	Id		INT IDENTITY PRIMARY KEY,
	Name	NVARCHAR(70) NOT NULL
)
ALTER TABLE Subject ADD CONSTRAINT RussianSubjectConstraint
CHECK (NOT [Name] LIKE '%[a-zA-Z]%')



CREATE TABLE Teacher(
	Id			INT IDENTITY PRIMARY KEY,
	Surname		NVARCHAR(30)	NOT NULL,
	Name		NVARCHAR(30)	NOT NULL,
	Patronymic	NVARCHAR(30)	NOT NULL,
	Gender		NCHAR(1)		NOT NULL DEFAULT 'М',
	BirthDay	DATE			NOT	NULL DEFAULT '1970-01-01'
)
ALTER TABLE Teacher ADD CONSTRAINT RussianTeacherName
CHECK (NOT Name LIKE '%[a-zA-Z0-9]%')
ALTER TABLE Teacher ADD CONSTRAINT RussianTeacherPatronymic
CHECK (NOT Patronymic LIKE '%[a-zA-Z0-9]%')
ALTER TABLE Teacher ADD CONSTRAINT RussianTeacherSurname
CHECK (NOT Surname LIKE '%[a-zA-Z0-9]%')
ALTER TABLE Teacher ADD CONSTRAINT TeacherGenderCheck
CHECK (Gender IN ('М', 'Ж'))
ALTER TABLE Teacher ADD CONSTRAINT TeacherBirthDayCheck
CHECK (BirthDay BETWEEN '1900-01-01' AND GETDATE());



CREATE TABLE TeacherSubject(
	IdTeacher	INT,
	IdSubject	INT,
	PRIMARY KEY	(IdTeacher, IdSubject),
	FOREIGN KEY	(IdTeacher) REFERENCES Teacher,
	FOREIGN KEY	(IdSubject) REFERENCES Subject
)



CREATE TABLE TeacherPhone(
	Id			INT IDENTITY PRIMARY KEY,
	IdTeacher	INT NOT NULL,
	PhoneNumber	NVARCHAR(16) NOT NULL,
	FOREIGN KEY	(IdTeacher) REFERENCES Teacher
)
ALTER TABLE TeacherPhone ADD CONSTRAINT TeacherPhoneConstraint
CHECK (PhoneNumber LIKE '+7 [0-9][0-9][0-9] [0-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9]')
ALTER TABLE TeacherPhone ADD CONSTRAINT UniqueTeacherPhone UNIQUE (PhoneNumber);



CREATE TABLE Student(
	Id			INT IDENTITY PRIMARY KEY,
	Surname		NVARCHAR(30)	NOT NULL,
	Name		NVARCHAR(30)	NOT NULL,
	Patronymic	NVARCHAR(30)	NOT NULL,
	IdGroup		INT NOT NULL,
	Email		NVARCHAR(60),
	Gender		NCHAR(1)		NOT NULL DEFAULT 'М',
	BirthDay	DATE			NOT	NULL DEFAULT '2005-01-01',
	FOREIGN KEY	(IdGroup) REFERENCES [Group]
)
ALTER TABLE Student ADD CONSTRAINT StudentGenderCheck
CHECK (Gender IN ('М', 'Ж'))
ALTER TABLE Student ADD CONSTRAINT StudentBirthYearCheck
CHECK (BirthDay BETWEEN '2005-01-01' AND GETDATE());
ALTER TABLE Student ADD CONSTRAINT RussianStudentName
CHECK (NOT Name LIKE '%[a-zA-Z0-9]%')
ALTER TABLE Student ADD CONSTRAINT RussianStudentSurname
CHECK (NOT Surname LIKE '%[a-zA-Z0-9]%')
ALTER TABLE Student ADD CONSTRAINT RussianStudentPatronymic
CHECK (NOT Patronymic LIKE '%[a-zA-Z0-9]%')
ALTER TABLE Student ADD CONSTRAINT StudentEmailCheck
CHECK (Email IS NULL OR Email LIKE '%_@%_.__%')
ALTER TABLE Student ADD CONSTRAINT UniqueStudentEmail UNIQUE (Email);
GO

CREATE TRIGGER StudentLowerCaseEmailTrigger
ON Student
AFTER INSERT, UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE Student
		SET email = LOWER(i.email)
	FROM Student s
	INNER JOIN inserted i ON s.id = i.id
	WHERE s.Email IS NOT NULL AND s.Email <> LOWER(i.Email);
END;
GO





















---------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------
CREATE TABLE Schedule(
	Id				INT IDENTITY PRIMARY KEY,
	IdSubject		INT NOT NULL,
	IdGroup			INT NOT NULL,
	IdTeacher		INT NOT NULL,
	IdBellSchedule	INT NOT NULL,
	DayOfTheWeek	INT NOT NULL,
	ClassRoom		INT NOT NULL,
	FOREIGN KEY		(IdSubject)			REFERENCES Subject,
	FOREIGN KEY		(IdGroup)			REFERENCES [Group],
	FOREIGN KEY		(IdTeacher)			REFERENCES Teacher,
	FOREIGN KEY		(IdBellSchedule)	REFERENCES BellSchedule
)
ALTER TABLE Schedule ADD CONSTRAINT DayOfTheWeekRangeCheck CHECK (DayOfTheWeek BETWEEN 1 AND 7)
ALTER TABLE Schedule ADD CONSTRAINT ClassRoomNotZeroCheck CHECK (ClassRoom <> 0)
GO
CREATE OR ALTER TRIGGER CheckTeacherAvailability
ON Schedule
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @TeacherName NVARCHAR(100),
        @DayOfWeekNumber INT,
        @DayOfWeekName NVARCHAR(20),
        @ExistingLessonNumber INT,
        @ErrorMsg NVARCHAR(500);

    -- Получаем данные о конфликте
    SELECT TOP 1
        @TeacherName = t.Surname + ' ' + t.[Name] + ' ' + t.Patronymic ,-- Имя преподавателя
        @DayOfWeekNumber = i.DayOfTheWeek,         -- Число 1-7
        @ExistingLessonNumber = bs2.LessonNumber   -- Номер урока
    FROM inserted i
    JOIN Schedule s ON i.IdTeacher = s.IdTeacher
    JOIN BellSchedule bs1 ON i.IdBellSchedule = bs1.Id
    JOIN BellSchedule bs2 ON s.IdBellSchedule = bs2.Id
    JOIN Teacher t ON s.IdTeacher = t.Id           -- Предполагается таблица Teacher
    WHERE 
        i.Id <> s.Id
        AND i.DayOfTheWeek = s.DayOfTheWeek
        AND (bs1.StartTime < bs2.EndTime AND bs1.EndTime > bs2.StartTime);

    IF @@ROWCOUNT > 0
    BEGIN
        -- Преобразуем номер дня в название
        SET @DayOfWeekName = CASE @DayOfWeekNumber
            WHEN 1 THEN 'понедельник'
            WHEN 2 THEN 'вторник'
            WHEN 3 THEN 'среду'
            WHEN 4 THEN 'четверг'
            WHEN 5 THEN 'пятницу'
            WHEN 6 THEN 'субботу'
            WHEN 7 THEN 'воскресенье'
            ELSE 'неизвестный день'
        END;

        -- Формируем сообщение
        SET @ErrorMsg = FORMATMESSAGE(
            N'Учитель %s уже проводит занятие в %s на %d уроке.', 
            @TeacherName,
            @DayOfWeekName,
            @ExistingLessonNumber
        );

        RAISERROR(@ErrorMsg, 16, 1);
        ROLLBACK TRANSACTION;
    END;
END;
GO
CREATE OR ALTER TRIGGER CheckClassRoomAvailability
ON Schedule
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @ClassRoom NVARCHAR(10),
        @DayOfWeekNumber INT,
        @DayOfWeekName NVARCHAR(20),
        @LessonNumber INT,
        @ErrorMsg NVARCHAR(500);

    -- Получаем данные о конфликте
    SELECT TOP 1
        @ClassRoom = i.ClassRoom,
        @DayOfWeekNumber = i.DayOfTheWeek,
        @LessonNumber = bs_s.LessonNumber
    FROM inserted i
    JOIN Schedule s 
        ON i.ClassRoom = s.ClassRoom
        AND i.DayOfTheWeek = s.DayOfTheWeek
        AND i.Id <> s.Id
    JOIN BellSchedule bs_i 
        ON i.IdBellSchedule = bs_i.Id
    JOIN BellSchedule bs_s 
        ON s.IdBellSchedule = bs_s.Id
    WHERE 
        bs_i.StartTime < bs_s.EndTime 
        AND bs_i.EndTime > bs_s.StartTime;

    IF @@ROWCOUNT > 0
    BEGIN
        -- Определяем название дня недели в винительном падеже
        SET @DayOfWeekName = CASE @DayOfWeekNumber
            WHEN 1 THEN 'понедельник'
            WHEN 2 THEN 'вторник'
            WHEN 3 THEN 'среду'
            WHEN 4 THEN 'четверг'
            WHEN 5 THEN 'пятницу'
            WHEN 6 THEN 'субботу'
            WHEN 7 THEN 'воскресенье'
            ELSE 'неизвестный день'
        END;

        -- Формируем сообщение
        SET @ErrorMsg = FORMATMESSAGE(
            N'Кабинет %s уже занят в %s на %d уроке.', 
            @ClassRoom,
            @DayOfWeekName,
            @LessonNumber
        );

        RAISERROR(@ErrorMsg, 16, 1);
        ROLLBACK TRANSACTION;
    END;
END;
GO
CREATE OR ALTER TRIGGER CheckGroupAvailability
ON Schedule
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @GroupYear INT,
        @GroupName NVARCHAR(1),
        @DayOfWeekNumber INT,
        @DayOfWeekName NVARCHAR(20),
        @LessonNumber INT,
        @ErrorMsg NVARCHAR(500);

    -- Получаем данные о конфликте
    SELECT TOP 1
        @GroupYear = g.[Year],               -- Год обучения
        @GroupName = g.[Name],           -- Буква класса (А, Б и т.д.)
        @DayOfWeekNumber = i.DayOfTheWeek, -- День недели (1-7)
        @LessonNumber = bs_s.LessonNumber  -- Номер урока
    FROM inserted i
    JOIN Schedule s 
        ON i.IdGroup = s.IdGroup
        AND i.DayOfTheWeek = s.DayOfTheWeek
        AND i.Id <> s.Id
    JOIN BellSchedule bs_i 
        ON i.IdBellSchedule = bs_i.Id
    JOIN BellSchedule bs_s 
        ON s.IdBellSchedule = bs_s.Id
    JOIN [Group] g 
        ON s.IdGroup = g.Id
    WHERE 
        bs_i.StartTime < bs_s.EndTime 
        AND bs_i.EndTime > bs_s.StartTime;

    IF @@ROWCOUNT > 0
    BEGIN
        -- Определяем день недели
        SET @DayOfWeekName = CASE @DayOfWeekNumber
            WHEN 1 THEN 'понедельник'
            WHEN 2 THEN 'вторник'
            WHEN 3 THEN 'среду'
            WHEN 4 THEN 'четверг'
            WHEN 5 THEN 'пятницу'
            WHEN 6 THEN 'субботу'
            WHEN 7 THEN 'воскресенье'
            ELSE 'неизвестный день'
        END;

        -- Формируем сообщение
        SET @ErrorMsg = FORMATMESSAGE(
            N'Группа %d-%s уже имеет занятие в %s на %d уроке.', 
            @GroupYear,
            @GroupName,
            @DayOfWeekName,
            @LessonNumber
        );

        RAISERROR(@ErrorMsg, 16, 1);
        ROLLBACK TRANSACTION;
    END;
END;
GO
CREATE TRIGGER PositiveClassRoomTrigger
ON Schedule
AFTER INSERT, UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE Schedule
	SET 
		ClassRoom=ABS(ClassRoom)
END
GO
---------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------













CREATE TABLE LessonSubsitutionSchedule(
	Id				INT IDENTITY PRIMARY KEY,
	Date			DATE NOT NULL,
	IdSubject		INT,
	IdGroup			INT,
	IdTeacher		INT,
	ClassRoom		INT,
	LessonNumber	INT NOT NULL,
	FOREIGN KEY		(IdSubject)	REFERENCES Subject,
	FOREIGN KEY		(IdGroup)	REFERENCES [Group],
	FOREIGN KEY		(IdTeacher)	REFERENCES Teacher
)



CREATE TABLE ClassTeacher(
	IdTeacher	INT,
	IdGroup		INT,
	PRIMARY KEY	(IdTeacher, IdGroup),
	FOREIGN KEY	(IdTeacher) REFERENCES Teacher,
	FOREIGN KEY	(IdGroup) 	REFERENCES [Group]
)
ALTER TABLE ClassTeacher ADD CONSTRAINT UniqueGroupConstraint UNIQUE (IdGroup)






GO
CREATE OR ALTER PROC ShowLessonsAtDayForTeacher
	@surname			NVARCHAR(30),
	@name				NVARCHAR(30),
	@patronymic			NVARCHAR(30),
	@date				DATE,
	@idBellScheduleType	INT
AS
BEGIN
	-- Определяем день недели для фильтрации
	DECLARE @dayOfWeek INT = DATEPART(WEEKDAY, @date);

	-- Основной запрос с учетом замен и типа расписания
	SELECT 
		COALESCE(sub.Name, s.Name) AS 'Предмет',
		COALESCE(sub.LessonNumber, bs.LessonNumber) AS 'Номер урока',
		FORMAT(bs.StartTime, N'hh\:mm') AS 'Время начала',
		FORMAT(bs.EndTime, N'hh\:mm') AS 'Время завершения',
		COALESCE(sub.ClassRoom, sch.ClassRoom) AS 'Кабинет'
	FROM Schedule sch
	JOIN Teacher t ON sch.IdTeacher = t.Id
	JOIN Subject s ON sch.IdSubject = s.Id
	JOIN BellSchedule bs ON sch.IdBellSchedule = bs.Id
	LEFT JOIN (
		SELECT 
			lss.[Date],
			lss.IdTeacher,
			lss.LessonNumber,
			lss.IdSubject,
			lss.ClassRoom,
			subj.Name
		FROM LessonSubsitutionSchedule lss
		JOIN Subject subj ON lss.IdSubject = subj.Id
		WHERE lss.[Date] = @date -- Фильтр замен по дате
	) sub 
		ON sch.IdTeacher = sub.IdTeacher 
		AND bs.LessonNumber = sub.LessonNumber
	WHERE 
		t.Surname = @surname
		AND t.Name = @name
		AND t.Patronymic = @patronymic
		AND sch.DayOfTheWeek = @dayOfWeek OR DATEPART(WEEKDAY, sub.[Date])=@dayOfWeek-- День недели из расписания
		AND bs.IdBellScheduleType = @idBellScheduleType -- Фильтр по типу расписания
	ORDER BY bs.LessonNumber;

	-- Динамическое создание VIEW
	DECLARE @sql NVARCHAR(MAX) = N'
	CREATE OR ALTER VIEW LessonsAtDayForTeacher AS
	SELECT 
		COALESCE(sub.Name, s.Name) AS Предмет,
		COALESCE(sub.LessonNumber, bs.LessonNumber) AS [Номер урока],
		FORMAT(bs.StartTime, N''hh\:mm'') AS [Начало],
		FORMAT(bs.EndTime, N''hh\:mm'') AS [Конец],
		COALESCE(sub.ClassRoom, sch.ClassRoom) AS Кабинет
	FROM Schedule sch
	JOIN Teacher t ON sch.IdTeacher = t.Id
	JOIN Subject s ON sch.IdSubject = s.Id
	JOIN BellSchedule bs ON sch.IdBellSchedule = bs.Id
	LEFT JOIN (
		SELECT 
			lss.[Date],
			lss.IdTeacher,
			lss.LessonNumber,
			lss.IdSubject,
			lss.ClassRoom,
			subj.Name
		FROM LessonSubsitutionSchedule lss
		JOIN Subject subj ON lss.IdSubject = subj.Id
		WHERE lss.Date = ''' + CONVERT(NVARCHAR(10), @date, 23) + '''
	) sub 
		ON sch.IdTeacher = sub.IdTeacher 
		AND bs.LessonNumber = sub.LessonNumber
	WHERE 
		t.Surname = ''' + @surname + '''
		AND t.Name = ''' + @name + '''
		AND t.Patronymic = ''' + @patronymic + '''
		AND sch.DayOfTheWeek = ' + CAST(@dayOfWeek AS NVARCHAR(2)) + 'OR DATEPART(WEEKDAY, sub.[Date])='+CAST(@dayOfWeek AS NVARCHAR(2)) +'
		AND bs.IdBellScheduleType = ' + CAST(@idBellScheduleType AS NVARCHAR(10));
	
	EXEC sp_executesql @sql;
END;
GO
CREATE OR ALTER PROC ShowLessonsAtDayForTeacherByIdTeacher
	@idTeacher			INT,
	@date				DATE,
	@idBellScheduleType	INT
AS
BEGIN
	-- Определяем день недели для фильтрации
	DECLARE @dayOfWeek INT = DATEPART(WEEKDAY, @date);

	-- Основной запрос с учетом замен и типа расписания
	SELECT 
		COALESCE(sub.Name, s.Name) AS 'Предмет',
		COALESCE(sub.LessonNumber, bs.LessonNumber) AS 'Номер_урока',
		FORMAT(bs.StartTime, N'hh\:mm') AS 'Время_начала',
		FORMAT(bs.EndTime, N'hh\:mm') AS 'Время_завершения',
		COALESCE(sub.ClassRoom, sch.ClassRoom) AS 'Кабинет'
	FROM Schedule sch
	JOIN Teacher t ON sch.IdTeacher = t.Id
	JOIN Subject s ON sch.IdSubject = s.Id
	JOIN BellSchedule bs ON sch.IdBellSchedule = bs.Id
	LEFT JOIN (
		SELECT 
			lss.[Date],
			lss.IdTeacher,
			lss.LessonNumber,
			lss.IdSubject,
			lss.ClassRoom,
			subj.Name
		FROM LessonSubsitutionSchedule lss
		JOIN Subject subj ON lss.IdSubject = subj.Id
		WHERE lss.[Date] = @date -- Фильтр замен по дате
	) sub 
		ON sch.IdTeacher = sub.IdTeacher 
		AND bs.LessonNumber = sub.LessonNumber
	WHERE 
		t.Id = @IdTeacher
		AND sch.DayOfTheWeek = @dayOfWeek OR DATEPART(WEEKDAY, sub.[Date])=@dayOfWeek-- День недели из расписания
		AND bs.IdBellScheduleType = @idBellScheduleType -- Фильтр по типу расписания
	ORDER BY bs.LessonNumber;

	-- Динамическое создание VIEW
	DECLARE @sql NVARCHAR(MAX) = N'
	CREATE OR ALTER VIEW LessonsAtDayForTeacher AS
	SELECT 
		COALESCE(sub.Name, s.Name) AS Предмет,
		COALESCE(sub.LessonNumber, bs.LessonNumber) AS [Номер урока],
		FORMAT(bs.StartTime, N''hh\:mm'') AS [Начало],
		FORMAT(bs.EndTime, N''hh\:mm'') AS [Конец],
		COALESCE(sub.ClassRoom, sch.ClassRoom) AS Кабинет
	FROM Schedule sch
	JOIN Teacher t ON sch.IdTeacher = t.Id
	JOIN Subject s ON sch.IdSubject = s.Id
	JOIN BellSchedule bs ON sch.IdBellSchedule = bs.Id
	LEFT JOIN (
		SELECT 
			lss.[Date],
			lss.IdTeacher,
			lss.LessonNumber,
			lss.IdSubject,
			lss.ClassRoom,
			subj.Name
		FROM LessonSubsitutionSchedule lss
		JOIN Subject subj ON lss.IdSubject = subj.Id
		WHERE lss.Date = ''' + CONVERT(NVARCHAR(10), @date, 23) + '''
	) sub 
		ON sch.IdTeacher = sub.IdTeacher 
		AND bs.LessonNumber = sub.LessonNumber
	WHERE 
		t.Id = ''' + CAST(@idTeacher AS NVARCHAR(10)) + '''
		AND sch.DayOfTheWeek = ' + CAST(@dayOfWeek AS NVARCHAR(2)) + 'OR DATEPART(WEEKDAY, sub.[Date])='+CAST(@dayOfWeek AS NVARCHAR(2)) +'
		AND bs.IdBellScheduleType = ' + CAST(@idBellScheduleType AS NVARCHAR(10));
	
	EXEC sp_executesql @sql;
END;
GO

-- Процедура: Студенты группы
CREATE OR ALTER PROC ShowStudentsByGroup
	@year INT,
	@name NCHAR(1)
AS
BEGIN
	DECLARE @groupId INT;
	SELECT @groupId = Id 
	FROM [Group] 
	WHERE [Year] = @year AND Name = @name;

	DECLARE @sql NVARCHAR(MAX);

	IF @groupId IS NULL
	BEGIN
		PRINT 'Группа не найдена';
		SET @sql = N'CREATE OR ALTER VIEW GroupView AS SELECT NULL AS [Нет данных]';
	END
	ELSE
	BEGIN
		SELECT 
			Surname,
			Name,
			Patronymic,
			Gender,
			BirthDay,
			Email
		FROM Student
		WHERE IdGroup = @groupId
		SET @sql = N'
		CREATE OR ALTER VIEW GroupView AS
		SELECT 
			Name AS Имя,
			Surname AS Фамилия,
			Patronymic AS Отчество,
			Gender,
			BirthDay,
			Email
		FROM Student
		WHERE IdGroup = ' + CAST(@groupId AS NVARCHAR(10));
	END;

	EXEC sp_executesql @sql;
END;
GO-- Процедура: Студенты группы
CREATE OR ALTER PROC ShowStudentsByGroupByIdGroup
	@idGroup	INT
AS
BEGIN
	SELECT 
		Surname,
		Name,
		Patronymic,
		Gender,
		BirthDay,
		Email
	FROM Student
	WHERE IdGroup = @idGroup
	
	DECLARE @sql NVARCHAR(MAX);
	SET @sql = N'
	CREATE OR ALTER VIEW GroupView AS
	SELECT 
		Name AS Имя,
		Surname AS Фамилия,
		Patronymic AS Отчество,
		Gender,
		BirthDay,
		Email
	FROM Student
	WHERE IdGroup = ' + CAST(@idGroup AS NVARCHAR(10));
	EXEC sp_executesql @sql;
END;
GO


CREATE OR ALTER FUNCTION dbo.CalculateAge
(
	@BirthDate DATE
)
RETURNS INT
AS
BEGIN
	DECLARE @Today DATE = GETDATE()
	RETURN 
		DATEDIFF(YEAR, @BirthDate, @Today) - 
		CASE 
			WHEN (MONTH(@BirthDate) > MONTH(@Today)) 
			  OR (MONTH(@BirthDate) = MONTH(@Today) AND DAY(@BirthDate) > DAY(@Today)) 
			THEN 1 
			ELSE 0 
		END
END
GO


INSERT INTO BellScheduleType ([Name]) VALUES ('Расписание звонков на каждую неделю')


INSERT INTO BellSchedule (IdBellScheduleType, LessonNumber, StartTime, EndTime) VALUES (1, 1, CAST('08:00:00' AS Time), CAST('08:40:00' AS Time))
INSERT INTO BellSchedule (IdBellScheduleType, LessonNumber, StartTime, EndTime) VALUES (1, 2, CAST('08:50:00' AS Time), CAST('09:30:00' AS Time))


INSERT INTO [Group] ([Year], [Name]) VALUES (1, 'В')


INSERT INTO Subject ([Name]) VALUES ('Математика')


INSERT INTO Teacher (Surname, [Name], Patronymic) VALUES ('Владимиров', 'Владимир', 'Владимирович')


INSERT INTO Schedule (IdSubject, IdGroup, IdTeacher, IdBellSchedule, DayOfTheWeek, ClassRoom) VALUES (1, 1, 1, 1, 4, 123)
INSERT INTO Schedule (IdSubject, IdGroup, IdTeacher, IdBellSchedule, DayOfTheWeek, ClassRoom) VALUES (1, 1, 1, 2, 4, 111)


INSERT INTO Student (Surname, [Name], Patronymic, IdGroup, Email) VALUES ('Иванов', 'Иван', 'Иванович', 1, NULL)


INSERT INTO LessonSubsitutionSchedule ([Date], IdSubject, IdGroup, IdTeacher, ClassRoom, LessonNumber) VALUES (CAST('2025-05-01' AS Date), 1, 1, 1, 345, 2)
INSERT INTO LessonSubsitutionSchedule ([Date], IdSubject, IdGroup, IdTeacher, ClassRoom, LessonNumber) VALUES (CAST('2025-05-01' AS Date), 1, NULL, NULL, NULL, 1)


INSERT INTO ClassTeacher ([IdTeacher], [IdGroup]) VALUES (1, 1)
GO









EXEC ShowLessonsAtDayForTeacherByIdTeacher 1, '01-05-2025', 1
EXEC ShowStudentsByGroupByIdGroup 1