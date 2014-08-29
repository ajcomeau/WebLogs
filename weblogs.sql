USE [Sample]
GO

/****** Object:  Table [dbo].[weblogs]    Script Date: 8/29/2014 6:29:15 PM 
 Sample script for creating weblogs table to go with WebLogsImport application ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[weblogs](
	[date] [date] NULL,
	[time] [nvarchar](25) NULL,
	[s-sitename] [nvarchar](1024) NULL,
	[s-computername] [nvarchar](1024) NULL,
	[s-ip] [nvarchar](1024) NULL,
	[cs-method] [nvarchar](1024) NULL,
	[cs-uri-stem] [nvarchar](1024) NULL,
	[cs-uri-query] [nvarchar](1024) NULL,
	[s-port] [nvarchar](1024) NULL,
	[cs-username] [nvarchar](1024) NULL,
	[c-ip] [nvarchar](1024) NULL,
	[cs-version] [nvarchar](1024) NULL,
	[cs(user-agent)] [nvarchar](1024) NULL,
	[cs(cookie)] [varchar](max) NULL,
	[cs(referrer)] [nvarchar](1024) NULL,
	[cs-host] [nvarchar](1024) NULL,
	[sc-status] [nvarchar](1024) NULL,
	[sc-substatus] [nvarchar](1024) NULL,
	[sc-win32-status] [nvarchar](1024) NULL,
	[sc-bytes] [bigint] NULL,
	[cs-bytes] [bigint] NULL,
	[time-taken] [bigint] NULL,
	[site-name] [nvarchar](1024) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


