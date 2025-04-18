USE [master]
GO
/****** Object:  Database [Catalog]    Script Date: 3/24/2025 6:26:27 PM ******/
CREATE DATABASE [Catalog]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Catalog', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\Catalog.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Catalog_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\Catalog_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [Catalog] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Catalog].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Catalog] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Catalog] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Catalog] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Catalog] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Catalog] SET ARITHABORT OFF 
GO
ALTER DATABASE [Catalog] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Catalog] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Catalog] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Catalog] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Catalog] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Catalog] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Catalog] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Catalog] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Catalog] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Catalog] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Catalog] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Catalog] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Catalog] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Catalog] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Catalog] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Catalog] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Catalog] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Catalog] SET RECOVERY FULL 
GO
ALTER DATABASE [Catalog] SET  MULTI_USER 
GO
ALTER DATABASE [Catalog] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Catalog] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Catalog] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Catalog] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Catalog] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Catalog] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'Catalog', N'ON'
GO
ALTER DATABASE [Catalog] SET QUERY_STORE = ON
GO
ALTER DATABASE [Catalog] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [Catalog]
GO
/****** Object:  Table [dbo].[Courses]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Courses](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[courseName] [nvarchar](100) NOT NULL,
	[teacherId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Grades]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Grades](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[studentId] [int] NOT NULL,
	[courseId] [int] NOT NULL,
	[value] [decimal](5, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentCourses]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentCourses](
	[studentId] [int] NOT NULL,
	[courseId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[studentId] ASC,
	[courseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](50) NOT NULL,
	[password] [nvarchar](255) NOT NULL,
	[firstName] [nvarchar](50) NOT NULL,
	[lastName] [nvarchar](50) NOT NULL,
	[userRole] [nvarchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Courses] ON 

INSERT [dbo].[Courses] ([id], [courseName], [teacherId]) VALUES (1, N'Mathematics', 3)
INSERT [dbo].[Courses] ([id], [courseName], [teacherId]) VALUES (2, N'Physics', 3)
INSERT [dbo].[Courses] ([id], [courseName], [teacherId]) VALUES (3, N'History', 4)
INSERT [dbo].[Courses] ([id], [courseName], [teacherId]) VALUES (4, N'Biology', 4)
SET IDENTITY_INSERT [dbo].[Courses] OFF
GO
SET IDENTITY_INSERT [dbo].[Grades] ON 

INSERT [dbo].[Grades] ([id], [studentId], [courseId], [value]) VALUES (1, 5, 1, CAST(8.50 AS Decimal(5, 2)))
INSERT [dbo].[Grades] ([id], [studentId], [courseId], [value]) VALUES (2, 5, 2, CAST(7.80 AS Decimal(5, 2)))
INSERT [dbo].[Grades] ([id], [studentId], [courseId], [value]) VALUES (3, 6, 1, CAST(9.20 AS Decimal(5, 2)))
INSERT [dbo].[Grades] ([id], [studentId], [courseId], [value]) VALUES (4, 6, 3, CAST(8.85 AS Decimal(5, 2)))
INSERT [dbo].[Grades] ([id], [studentId], [courseId], [value]) VALUES (5, 7, 4, CAST(7.40 AS Decimal(5, 2)))
INSERT [dbo].[Grades] ([id], [studentId], [courseId], [value]) VALUES (6, 7, 2, CAST(8.10 AS Decimal(5, 2)))
INSERT [dbo].[Grades] ([id], [studentId], [courseId], [value]) VALUES (7, 8, 3, CAST(9.50 AS Decimal(5, 2)))
SET IDENTITY_INSERT [dbo].[Grades] OFF
GO
INSERT [dbo].[StudentCourses] ([studentId], [courseId]) VALUES (5, 1)
INSERT [dbo].[StudentCourses] ([studentId], [courseId]) VALUES (5, 2)
INSERT [dbo].[StudentCourses] ([studentId], [courseId]) VALUES (6, 1)
INSERT [dbo].[StudentCourses] ([studentId], [courseId]) VALUES (6, 3)
INSERT [dbo].[StudentCourses] ([studentId], [courseId]) VALUES (7, 2)
INSERT [dbo].[StudentCourses] ([studentId], [courseId]) VALUES (7, 4)
INSERT [dbo].[StudentCourses] ([studentId], [courseId]) VALUES (8, 3)
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([id], [username], [password], [firstName], [lastName], [userRole]) VALUES (1, N'admin1', N'password123', N'Alice', N'Admin', N'admin')
INSERT [dbo].[Users] ([id], [username], [password], [firstName], [lastName], [userRole]) VALUES (2, N'admin2', N'password123', N'Bob', N'Manager', N'admin')
INSERT [dbo].[Users] ([id], [username], [password], [firstName], [lastName], [userRole]) VALUES (3, N'teacher1', N'password123', N'John', N'Doe', N'teacher')
INSERT [dbo].[Users] ([id], [username], [password], [firstName], [lastName], [userRole]) VALUES (4, N'teacher2', N'password123', N'Emma', N'Smith', N'teacher')
INSERT [dbo].[Users] ([id], [username], [password], [firstName], [lastName], [userRole]) VALUES (5, N'student1', N'password123', N'Michael', N'Brown', N'student')
INSERT [dbo].[Users] ([id], [username], [password], [firstName], [lastName], [userRole]) VALUES (6, N'student2', N'password123', N'Sophia', N'Wilson', N'student')
INSERT [dbo].[Users] ([id], [username], [password], [firstName], [lastName], [userRole]) VALUES (7, N'student3', N'password123', N'David', N'Johnson', N'student')
INSERT [dbo].[Users] ([id], [username], [password], [firstName], [lastName], [userRole]) VALUES (8, N'student4', N'password123', N'Olivia', N'Martinez', N'student')
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__F3DBC572421E2DF9]    Script Date: 3/24/2025 6:26:27 PM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Courses]  WITH CHECK ADD  CONSTRAINT [FK_Courses_Teacher] FOREIGN KEY([teacherId])
REFERENCES [dbo].[Users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Courses] CHECK CONSTRAINT [FK_Courses_Teacher]
GO
ALTER TABLE [dbo].[Grades]  WITH CHECK ADD  CONSTRAINT [FK_Grades_Course] FOREIGN KEY([courseId])
REFERENCES [dbo].[Courses] ([id])
GO
ALTER TABLE [dbo].[Grades] CHECK CONSTRAINT [FK_Grades_Course]
GO
ALTER TABLE [dbo].[Grades]  WITH CHECK ADD  CONSTRAINT [FK_Grades_Student] FOREIGN KEY([studentId])
REFERENCES [dbo].[Users] ([id])
GO
ALTER TABLE [dbo].[Grades] CHECK CONSTRAINT [FK_Grades_Student]
GO
ALTER TABLE [dbo].[StudentCourses]  WITH CHECK ADD  CONSTRAINT [FK_StudentCourses_Course] FOREIGN KEY([courseId])
REFERENCES [dbo].[Courses] ([id])
GO
ALTER TABLE [dbo].[StudentCourses] CHECK CONSTRAINT [FK_StudentCourses_Course]
GO
ALTER TABLE [dbo].[StudentCourses]  WITH CHECK ADD  CONSTRAINT [FK_StudentCourses_Student] FOREIGN KEY([studentId])
REFERENCES [dbo].[Users] ([id])
GO
ALTER TABLE [dbo].[StudentCourses] CHECK CONSTRAINT [FK_StudentCourses_Student]
GO
ALTER TABLE [dbo].[Grades]  WITH CHECK ADD CHECK  (([value]>=(0) AND [value]<=(100)))
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD CHECK  (([userRole]='admin' OR [userRole]='teacher' OR [userRole]='student'))
GO
/****** Object:  StoredProcedure [dbo].[AddGrade]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =====================================
-- Grades Table CRUD Procedures
-- =====================================

-- Add Grade
CREATE PROCEDURE [dbo].[AddGrade]
    @studentId INT,
    @courseId INT,
    @value DECIMAL(5,2)
AS
BEGIN
    INSERT INTO Grades (studentId, courseId, value) VALUES (@studentId, @courseId, @value);
END;
GO
/****** Object:  StoredProcedure [dbo].[CreateCourse]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =====================================
-- Courses Table CRUD Procedures
-- =====================================

-- Create Course
CREATE PROCEDURE [dbo].[CreateCourse]
    @courseName NVARCHAR(100),
    @teacherId INT
AS
BEGIN
    INSERT INTO Courses (courseName, teacherId) VALUES (@courseName, @teacherId);
END;
GO
/****** Object:  StoredProcedure [dbo].[CreateUser]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =====================================
-- Users Table CRUD Procedures
-- =====================================

-- Create User
CREATE PROCEDURE [dbo].[CreateUser]
    @username NVARCHAR(50),
    @password NVARCHAR(255),
    @firstName NVARCHAR(50),
    @lastName NVARCHAR(50),
    @userRole NVARCHAR(10)
AS
BEGIN
    INSERT INTO Users (username, password, firstName, lastName, userRole)
    VALUES (@username, @password, @firstName, @lastName, @userRole);
END;
GO
/****** Object:  StoredProcedure [dbo].[DeleteCourse]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Delete Course
CREATE PROCEDURE [dbo].[DeleteCourse]
    @id INT
AS
BEGIN
    DELETE FROM Courses WHERE id = @id;
END;
GO
/****** Object:  StoredProcedure [dbo].[DeleteGrade]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Delete Grade
CREATE PROCEDURE [dbo].[DeleteGrade]
    @id INT
AS
BEGIN
    DELETE FROM Grades WHERE id = @id;
END;
GO
/****** Object:  StoredProcedure [dbo].[DeleteUser]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Delete User
CREATE PROCEDURE [dbo].[DeleteUser]
    @id INT
AS
BEGIN
    DELETE FROM Users WHERE id = @id;
END;
GO
/****** Object:  StoredProcedure [dbo].[EnrollStudent]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =====================================
-- StudentCourses (Enrollments) Table CRUD Procedures
-- =====================================

-- Enroll Student in Course
CREATE PROCEDURE [dbo].[EnrollStudent]
    @studentId INT,
    @courseId INT
AS
BEGIN
    INSERT INTO StudentCourses (studentId, courseId) VALUES (@studentId, @courseId);
END;
GO
/****** Object:  StoredProcedure [dbo].[GetAverageGradeByStudent]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAverageGradeByStudent]
    @studentId INT
AS
BEGIN
    SELECT AVG(value) AS averageGrade
    FROM Grades
    WHERE studentId = @studentId;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetCourseAverageGrade]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetCourseAverageGrade]
    @courseId INT
AS
BEGIN
    SELECT c.courseName, AVG(g.value) AS averageGrade
    FROM Grades g
    JOIN Courses c ON g.courseId = c.id
    WHERE g.courseId = @courseId
    GROUP BY c.courseName;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetCourses]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Read Courses
CREATE PROCEDURE [dbo].[GetCourses]
AS
BEGIN
    SELECT * FROM Courses;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetCoursesByStudent]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetCoursesByStudent]
    @studentId INT
AS
BEGIN
    SELECT c.id AS courseId, c.courseName
    FROM StudentCourses sc
    JOIN Courses c ON sc.courseId = c.id
    WHERE sc.studentId = @studentId;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetCoursesByTeacher]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetCoursesByTeacher]
    @teacherId INT
AS
BEGIN
    SELECT id AS courseId, courseName
    FROM Courses
    WHERE teacherId = @teacherId;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetEnrollments]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get Enrollments
CREATE PROCEDURE [dbo].[GetEnrollments]
AS
BEGIN
    SELECT sc.studentId, u.firstName, u.lastName, sc.courseId, c.courseName
    FROM StudentCourses sc
    JOIN Users u ON sc.studentId = u.id
    JOIN Courses c ON sc.courseId = c.id;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetGrades]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Get Grades
CREATE PROCEDURE [dbo].[GetGrades]
AS
BEGIN
    SELECT g.id, u.firstName, u.lastName, c.courseName, g.value
    FROM Grades g
    JOIN Users u ON g.studentId = u.id
    JOIN Courses c ON g.courseId = c.id;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetGradesByStudent]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetGradesByStudent]
    @studentId INT
AS
BEGIN
    SELECT c.courseName, g.value AS grade
    FROM Grades g
    JOIN Courses c ON g.courseId = c.id
    WHERE g.studentId = @studentId;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetStudentsByCourse]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetStudentsByCourse]
    @courseId INT
AS
BEGIN
    SELECT u.id AS studentId, u.firstName, u.lastName
    FROM StudentCourses sc
    JOIN Users u ON sc.studentId = u.id
    WHERE sc.courseId = @courseId;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetTopStudentsInCourse]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetTopStudentsInCourse]
    @courseId INT,
    @topN INT
AS
BEGIN
    SELECT TOP (@topN) u.firstName, u.lastName, g.value AS grade
    FROM Grades g
    JOIN Users u ON g.studentId = u.id
    WHERE g.courseId = @courseId
    ORDER BY g.value DESC;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetUsers]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Read Users
CREATE PROCEDURE [dbo].[GetUsers]
AS
BEGIN
    SELECT * FROM Users;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetUsersByRole]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetUsersByRole]
    @userRole NVARCHAR(10)
AS
BEGIN
    SELECT id, username, firstName, lastName
    FROM Users
    WHERE userRole = @userRole;
END;
GO
/****** Object:  StoredProcedure [dbo].[RemoveEnrollment]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Remove Student from Course
CREATE PROCEDURE [dbo].[RemoveEnrollment]
    @studentId INT,
    @courseId INT
AS
BEGIN
    DELETE FROM StudentCourses WHERE studentId = @studentId AND courseId = @courseId;
END;
GO
/****** Object:  StoredProcedure [dbo].[RemoveGradesByStudent]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[RemoveGradesByStudent]
    @studentId INT
AS
BEGIN
    DELETE FROM Grades WHERE studentId = @studentId;
END;
GO
/****** Object:  StoredProcedure [dbo].[RemoveStudentFromAllCourses]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[RemoveStudentFromAllCourses]
    @studentId INT
AS
BEGIN
    DELETE FROM StudentCourses WHERE studentId = @studentId;
END;
GO
/****** Object:  StoredProcedure [dbo].[UpdateCourse]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Update Course
CREATE PROCEDURE [dbo].[UpdateCourse]
    @id INT,
    @courseName NVARCHAR(100),
    @teacherId INT
AS
BEGIN
    UPDATE Courses
    SET courseName = @courseName, teacherId = @teacherId
    WHERE id = @id;
END;
GO
/****** Object:  StoredProcedure [dbo].[UpdateGrade]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Update Grade
CREATE PROCEDURE [dbo].[UpdateGrade]
    @id INT,
    @value DECIMAL(5,2)
AS
BEGIN
    UPDATE Grades
    SET value = @value
    WHERE id = @id;
END;
GO
/****** Object:  StoredProcedure [dbo].[UpdateUser]    Script Date: 3/24/2025 6:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Update User
CREATE PROCEDURE [dbo].[UpdateUser]
    @id INT,
    @username NVARCHAR(50),
    @password NVARCHAR(255),
    @firstName NVARCHAR(50),
    @lastName NVARCHAR(50),
    @userRole NVARCHAR(10)
AS
BEGIN
    UPDATE Users
    SET username = @username, password = @password, firstName = @firstName,
        lastName = @lastName, userRole = @userRole
    WHERE id = @id;
END;
GO
USE [master]
GO
ALTER DATABASE [Catalog] SET  READ_WRITE 
GO
