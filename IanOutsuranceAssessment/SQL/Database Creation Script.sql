USE [master]
GO
/****** Object:  Database [IanOutsuranceAssessment]    Script Date: 2017-10-19 01:35:02 ******/
CREATE DATABASE [IanOutsuranceAssessment]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'IanOutsuranceAssessment', FILENAME = N'G:\Databases\MSSQL13.BYTEBASE\MSSQL\DATA\IanOutsuranceAssessment.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'IanOutsuranceAssessment_log', FILENAME = N'G:\Databases\MSSQL13.BYTEBASE\MSSQL\DATA\IanOutsuranceAssessment_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [IanOutsuranceAssessment] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [IanOutsuranceAssessment].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [IanOutsuranceAssessment] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET ARITHABORT OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET  DISABLE_BROKER 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET RECOVERY FULL 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET  MULTI_USER 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [IanOutsuranceAssessment] SET DB_CHAINING OFF 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [IanOutsuranceAssessment] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'IanOutsuranceAssessment', N'ON'
GO
ALTER DATABASE [IanOutsuranceAssessment] SET QUERY_STORE = OFF
GO
USE [IanOutsuranceAssessment]
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [IanOutsuranceAssessment]
GO
/****** Object:  Table [dbo].[ClientsWorkbench]    Script Date: 2017-10-19 01:35:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClientsWorkbench](
	[FirstName] [nvarchar](150) NULL,
	[LastName] [nvarchar](150) NULL,
	[Address] [nvarchar](250) NULL,
	[PhoneNumber] [varchar](50) NULL
) ON [PRIMARY]

GO
/****** Object:  StoredProcedure [dbo].[spQuestion1]    Script Date: 2017-10-19 01:35:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spQuestion1]
(
	@FileName VARCHAR(255)
)

AS

--DECLARE @FileName VARCHAR(255) = 'C:\IanOutsuranceAssessment\Data\data.csv'

EXEC 
(
'TRUNCATE TABLE [ClientsWorkbench]

BULK INSERT [ClientsWorkbench]

FROM ''' + @FileName + ''' 
WITH
(
	FIELDTERMINATOR = '','',
	ROWTERMINATOR = ''0x0a'',
	FIRSTROW = 2
)'

)

--SELECT * FROM [ClientsWorkbench]

-------------------------------------------------------------------------------------------------------
-- We can use a table-variable for our intents and purposes, because we'll not be processing
-- thousands of rows.
-- In such a case, we will have to use a #Temp table and drop the table at the very end

DECLARE @NameAndSurnameOccurrences TABLE
(
	[NameOrSurname] NVARCHAR(150),
	[Occurrences] SMALLINT
)


-- First, insert the first names and the number of times each occur:
INSERT INTO @NameAndSurnameOccurrences
(
	[NameOrSurname],
	[Occurrences]
)

SELECT
	[FirstName],
	COUNT(*)

FROM [ClientsWorkbench]

GROUP BY [FirstName]

-- Then, insert the surnames and the number of times each occur:
INSERT INTO @NameAndSurnameOccurrences
(
	[NameOrSurname],
	[Occurrences]
)

SELECT
	[LastName],
	COUNT(*)

FROM [ClientsWorkbench]

GROUP BY [LastName]

SELECT * FROM @NameAndSurnameOccurrences
ORDER BY [Occurrences] DESC, [NameOrSurname]


-------------------------------------------------------------------------------------------------------
--DECLARE @ResultingFileContent NVARCHAR(MAX) = ''

--SELECT @ResultingFileContent = @ResultingFileContent + [NameOrSurname] + '	' + CONVERT(VARCHAR(10), [Occurrences]) + CHAR(13) + CHAR(10)
--FROM @NameAndSurnameOccurrences
--ORDER BY [Occurrences] DESC, [NameOrSurname]

--SELECT @ResultingFileContent AS [ResultingFileContent]

--SELECT * FROM @NameAndSurnameOccurrences
--ORDER BY [Occurrences] DESC, [NameOrSurname]


GO
/****** Object:  StoredProcedure [dbo].[spQuestion2]    Script Date: 2017-10-19 01:35:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spQuestion2]
(
	@FileName VARCHAR(255)
)

AS

--DECLARE @FileName VARCHAR(255) = 'C:\IanOutsuranceAssessment\Data\data.csv'

EXEC 
(
'TRUNCATE TABLE [ClientsWorkbench]

BULK INSERT [ClientsWorkbench]

FROM ''' + @FileName + ''' 
WITH
(
	FIELDTERMINATOR = '','',
	ROWTERMINATOR = ''0x0a'',
	FIRSTROW = 2
)'

)

--SELECT * FROM [ClientsWorkbench]

-------------------------------------------------------------------------------------------------------
-- We can use a table-variable for our intents and purposes, because we'll not be processing
-- thousands of rows.
-- In such a case, we will have to use a #Temp table and drop the table at the very end

DECLARE @StreetNumberAndStreetName TABLE
(
	[StreetNumber] NVARCHAR(10),
	[StreetName] NVARCHAR(150)
)


-- Split the street number and -name and insert this split address into the table variable
INSERT INTO @StreetNumberAndStreetName
(
	[StreetNumber],
	[StreetName]
)

SELECT
	LTRIM(RTRIM(SUBSTRING([Address], 1, CHARINDEX(' ', [Address], 1)))),
	LTRIM(RTRIM(REPLACE([Address], LTRIM(RTRIM(SUBSTRING([Address], 1, CHARINDEX(' ', [Address], 1)))), '')))

FROM [ClientsWorkbench]

SELECT * FROM @StreetNumberAndStreetName
ORDER BY [StreetName], [StreetNumber]

-------------------------------------------------------------------------------------------------------



GO
USE [master]
GO
ALTER DATABASE [IanOutsuranceAssessment] SET  READ_WRITE 
GO
