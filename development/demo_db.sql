USE [master]
GO
/****** Object:  Database [touch_for_food]    Script Date: 03/31/2013 15:46:28 ******/
CREATE DATABASE [touch_for_food] ON  PRIMARY 
( NAME = N'touch_for_food', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\touch_for_food.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'touch_for_food_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\touch_for_food_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [touch_for_food] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [touch_for_food].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [touch_for_food] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [touch_for_food] SET ANSI_NULLS OFF
GO
ALTER DATABASE [touch_for_food] SET ANSI_PADDING OFF
GO
ALTER DATABASE [touch_for_food] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [touch_for_food] SET ARITHABORT OFF
GO
ALTER DATABASE [touch_for_food] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [touch_for_food] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [touch_for_food] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [touch_for_food] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [touch_for_food] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [touch_for_food] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [touch_for_food] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [touch_for_food] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [touch_for_food] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [touch_for_food] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [touch_for_food] SET  DISABLE_BROKER
GO
ALTER DATABASE [touch_for_food] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [touch_for_food] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [touch_for_food] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [touch_for_food] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [touch_for_food] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [touch_for_food] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [touch_for_food] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [touch_for_food] SET  READ_WRITE
GO
ALTER DATABASE [touch_for_food] SET RECOVERY SIMPLE
GO
ALTER DATABASE [touch_for_food] SET  MULTI_USER
GO
ALTER DATABASE [touch_for_food] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [touch_for_food] SET DB_CHAINING OFF
GO
USE [touch_for_food]
GO
/****** Object:  Table [dbo].[restaurant]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[restaurant](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](35) NULL,
	[address] [nvarchar](50) NULL,
	[postal_code] [nvarchar](10) NULL,
	[city] [nvarchar](35) NULL,
	[rating] [decimal](2, 1) NULL,
	[version] [int] NOT NULL,
	[is_deleted] [bit] NOT NULL,
 CONSTRAINT [PK__restaura__3213E83F1DE57479] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[restaurant] ON
INSERT [dbo].[restaurant] ([id], [name], [address], [postal_code], [city], [rating], [version], [is_deleted]) VALUES (166, N'CloudNine Resto', N'1515 St. Catherine W.', N'H3G 1M8', N'Montreal', NULL, 1, 0)
SET IDENTITY_INSERT [dbo].[restaurant] OFF
/****** Object:  Table [dbo].[category]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[category](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](55) NULL,
	[version] [int] NOT NULL,
 CONSTRAINT [PK__category__3213E83F0EA330E9] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[category] ON
INSERT [dbo].[category] ([id], [name], [version]) VALUES (95, N'Snacks', 1)
INSERT [dbo].[category] ([id], [name], [version]) VALUES (96, N'Deserts', 1)
SET IDENTITY_INSERT [dbo].[category] OFF
/****** Object:  Table [dbo].[table]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[table](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) NULL,
	[restaurant_id] [int] NULL,
 CONSTRAINT [PK_table] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[table] ON
INSERT [dbo].[table] ([id], [name], [restaurant_id]) VALUES (163, N'Hum''s Table', 166)
INSERT [dbo].[table] ([id], [name], [restaurant_id]) VALUES (164, N'Asenjo''s Table', 166)
INSERT [dbo].[table] ([id], [name], [restaurant_id]) VALUES (165, N'Daher''s Table', 166)
INSERT [dbo].[table] ([id], [name], [restaurant_id]) VALUES (166, N'Levkovsky''s Table', 166)
SET IDENTITY_INSERT [dbo].[table] OFF
/****** Object:  Table [dbo].[menu]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[menu](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[resto_id] [int] NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[is_active] [bit] NOT NULL,
	[is_deleted] [bit] NOT NULL,
	[version] [int] NOT NULL,
 CONSTRAINT [PK__menu__3213E83F29572725] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[menu] ON
INSERT [dbo].[menu] ([id], [resto_id], [name], [is_active], [is_deleted], [version]) VALUES (107, 166, N'Demo Menu', 1, 0, 2)
SET IDENTITY_INSERT [dbo].[menu] OFF
/****** Object:  Table [dbo].[item]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[item](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](55) NULL,
	[description] [text] NULL,
	[metadata] [text] NULL,
	[category_id] [int] NULL,
	[image_url] [nvarchar](255) NULL,
	[version] [int] NOT NULL,
 CONSTRAINT [PK__menu_ite__3213E83F0AD2A005] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[item] ON
INSERT [dbo].[item] ([id], [name], [description], [metadata], [category_id], [image_url], [version]) VALUES (88, N'Chiftele (Romanian Meatballs)', N'Fried mini pork meatballs mixed with potatoes. Served on a bed of lettuce.', N'meatballs potato pork', 95, N'/Content/images/food/meatballs.jpg', 2)
INSERT [dbo].[item] ([id], [name], [description], [metadata], [category_id], [image_url], [version]) VALUES (89, N'Prosciutto Wrapped Melon', N'Cantaloupe and freshly shaved Parmesan wrapped in prosciutto. Drizzled with balsamic reduction.', N'cantaloupe melon prosciutto meat cheese', 95, N'/Content/images/food/melonballs.jpg', 3)
INSERT [dbo].[item] ([id], [name], [description], [metadata], [category_id], [image_url], [version]) VALUES (90, N'Mexican Style Pinwheel Sandwiches', N'Monterey Jack and smoked turkey rolled up in a salsa tortilla.', N'cheese salsa turkey sandwich tortilla mexican', 95, N'/Content/images/food/pinwheels.jpg', 1)
INSERT [dbo].[item] ([id], [name], [description], [metadata], [category_id], [image_url], [version]) VALUES (91, N'Carrot Cake', N'Mini two-layer carrot cake with cream cheese icing.', N'carrot cake desert mini cream cheese', 96, N'/Content/images/food/carrot.jpg', 1)
INSERT [dbo].[item] ([id], [name], [description], [metadata], [category_id], [image_url], [version]) VALUES (92, N'Chocolate Eclair', N'A choux paste shell filled with homemade pastry cream and glazed with chocolate ganache.', N'pastry desert cream custard fancy words homemade', 96, N'/Content/images/food/eclairs.jpg', 1)
SET IDENTITY_INSERT [dbo].[item] OFF
/****** Object:  Table [dbo].[waiter]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[waiter](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[first_name] [nvarchar](35) NULL,
	[last_name] [nvarchar](35) NULL,
	[resto_id] [int] NULL,
	[version] [int] NOT NULL,
 CONSTRAINT [PK__waiter__3213E83F25869641] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](35) NOT NULL,
	[password] [nvarchar](255) NOT NULL,
	[first_name] [nvarchar](35) NOT NULL,
	[last_name] [nvarchar](35) NOT NULL,
	[email] [nvarchar](35) NOT NULL,
	[image_url] [nvarchar](max) NULL,
	[current_table_id] [int] NULL,
	[version] [int] NOT NULL,
	[user_role] [int] NULL,
 CONSTRAINT [PK__user__3213E83F21B6055D] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [email_is_unique] UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [username_is_unique] UNIQUE NONCLUSTERED 
(
	[username] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Email must be unique' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'user', @level2type=N'CONSTRAINT',@level2name=N'email_is_unique'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Usernames must be unique' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'user', @level2type=N'CONSTRAINT',@level2name=N'username_is_unique'
GO
SET IDENTITY_INSERT [dbo].[user] ON
INSERT [dbo].[user] ([id], [username], [password], [first_name], [last_name], [email], [image_url], [current_table_id], [version], [user_role]) VALUES (250, N'developer', N'052184051173122045182002089053137200071181132074', N'Dev', N'Eloper', N'dev@email.com', NULL, NULL, 1, 8)
INSERT [dbo].[user] ([id], [username], [password], [first_name], [last_name], [email], [image_url], [current_table_id], [version], [user_role]) VALUES (251, N'restaurant', N'052184051173122045182002089053137200071181132074', N'Resto', N'Rant', N'resto@email.com', NULL, NULL, 2, 4)
INSERT [dbo].[user] ([id], [username], [password], [first_name], [last_name], [email], [image_url], [current_table_id], [version], [user_role]) VALUES (252, N'josh', N'052184051173122045182002089053137200071181132074', N'Josh', N'Hum', N'josh@email.com', NULL, 163, 1, 2)
INSERT [dbo].[user] ([id], [username], [password], [first_name], [last_name], [email], [image_url], [current_table_id], [version], [user_role]) VALUES (253, N'cristian', N'052184051173122045182002089053137200071181132074', N'Cristian', N'Asenjo', N'cristian@email.com', NULL, 164, 1, 2)
INSERT [dbo].[user] ([id], [username], [password], [first_name], [last_name], [email], [image_url], [current_table_id], [version], [user_role]) VALUES (254, N'mikhail', N'052184051173122045182002089053137200071181132074', N'Mikhail', N'Levkovsky', N'mikhail@email.com', NULL, 166, 1, 2)
INSERT [dbo].[user] ([id], [username], [password], [first_name], [last_name], [email], [image_url], [current_table_id], [version], [user_role]) VALUES (255, N'daher', N'052184051173122045182002089053137200071181132074', N'Chris', N'Daher', N'chris@email.com', NULL, 165, 1, 2)
SET IDENTITY_INSERT [dbo].[user] OFF
/****** Object:  Table [dbo].[menu_category]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[menu_category](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[category_id] [int] NOT NULL,
	[menu_id] [int] NOT NULL,
	[is_deleted] [bit] NOT NULL,
	[is_active] [bit] NOT NULL,
	[version] [int] NOT NULL,
 CONSTRAINT [PK_menu_category] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[menu_category] ON
INSERT [dbo].[menu_category] ([id], [category_id], [menu_id], [is_deleted], [is_active], [version]) VALUES (287, 95, 107, 0, 1, 2)
INSERT [dbo].[menu_category] ([id], [category_id], [menu_id], [is_deleted], [is_active], [version]) VALUES (288, 96, 107, 0, 1, 2)
SET IDENTITY_INSERT [dbo].[menu_category] OFF
/****** Object:  Table [dbo].[service_request]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[service_request](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[note] [text] NULL,
	[status] [int] NULL,
	[created] [datetime2](7) NULL,
	[version] [int] NOT NULL,
	[table_id] [int] NULL,
 CONSTRAINT [PK_service_request] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sides]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sides](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[menu_category_id] [int] NULL,
	[name] [nvarchar](55) NOT NULL,
	[price] [decimal](10, 2) NULL,
	[is_active] [bit] NOT NULL,
	[is_deleted] [bit] NOT NULL,
	[version] [int] NOT NULL,
 CONSTRAINT [PK_sides] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[restaurant_user]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[restaurant_user](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[restaurant_id] [int] NULL,
 CONSTRAINT [PK_restaurant_user] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[restaurant_user] ON
INSERT [dbo].[restaurant_user] ([id], [user_id], [restaurant_id]) VALUES (65, 251, 166)
SET IDENTITY_INSERT [dbo].[restaurant_user] OFF
/****** Object:  Table [dbo].[order]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[order](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[timestamp] [datetime2](7) NOT NULL,
	[total] [decimal](10, 2) NULL,
	[order_status] [int] NULL,
	[user_id] [int] NULL,
	[waiter_id] [int] NULL,
	[version] [int] NOT NULL,
	[table_id] [int] NULL,
 CONSTRAINT [PK__order__3213E83F1273C1CD] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[menu_item]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[menu_item](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[item_id] [int] NOT NULL,
	[menu_category_id] [int] NOT NULL,
	[price] [decimal](10, 2) NOT NULL,
	[is_deleted] [bit] NOT NULL,
	[is_active] [bit] NOT NULL,
	[version] [int] NOT NULL,
 CONSTRAINT [PK_menu_item_instance] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[menu_item] ON
INSERT [dbo].[menu_item] ([id], [item_id], [menu_category_id], [price], [is_deleted], [is_active], [version]) VALUES (170, 88, 287, CAST(0.00 AS Decimal(10, 2)), 1, 1, 3)
INSERT [dbo].[menu_item] ([id], [item_id], [menu_category_id], [price], [is_deleted], [is_active], [version]) VALUES (171, 89, 287, CAST(0.00 AS Decimal(10, 2)), 1, 1, 3)
INSERT [dbo].[menu_item] ([id], [item_id], [menu_category_id], [price], [is_deleted], [is_active], [version]) VALUES (172, 89, 287, CAST(0.00 AS Decimal(10, 2)), 1, 1, 3)
INSERT [dbo].[menu_item] ([id], [item_id], [menu_category_id], [price], [is_deleted], [is_active], [version]) VALUES (173, 90, 287, CAST(0.00 AS Decimal(10, 2)), 0, 1, 2)
INSERT [dbo].[menu_item] ([id], [item_id], [menu_category_id], [price], [is_deleted], [is_active], [version]) VALUES (174, 91, 288, CAST(0.00 AS Decimal(10, 2)), 0, 1, 2)
INSERT [dbo].[menu_item] ([id], [item_id], [menu_category_id], [price], [is_deleted], [is_active], [version]) VALUES (175, 92, 288, CAST(0.00 AS Decimal(10, 2)), 0, 1, 2)
INSERT [dbo].[menu_item] ([id], [item_id], [menu_category_id], [price], [is_deleted], [is_active], [version]) VALUES (176, 88, 287, CAST(0.00 AS Decimal(10, 2)), 0, 1, 1)
INSERT [dbo].[menu_item] ([id], [item_id], [menu_category_id], [price], [is_deleted], [is_active], [version]) VALUES (177, 89, 287, CAST(0.00 AS Decimal(10, 2)), 0, 1, 1)
SET IDENTITY_INSERT [dbo].[menu_item] OFF
/****** Object:  Table [dbo].[friendship]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[friendship](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[first_user] [int] NULL,
	[second_user] [int] NULL,
 CONSTRAINT [PK_friendship] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[bill]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bill](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[order_id] [int] NULL,
	[tvq] [decimal](18, 2) NULL,
	[tps] [decimal](18, 2) NULL,
	[total] [decimal](18, 2) NULL,
	[timestamp] [datetime] NULL,
	[paid] [bit] NULL,
	[is_deleted] [bit] NULL,
	[version] [int] NOT NULL,
 CONSTRAINT [PK_bill] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[review]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[review](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[rating] [decimal](2, 1) NOT NULL,
	[is_anonymous] [bit] NOT NULL,
	[order_id] [int] NOT NULL,
	[restaurant_id] [int] NOT NULL,
 CONSTRAINT [PK_review] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Trigger [update_average]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[update_average]
   ON  [dbo].[review] 
   AFTER INSERT
AS 
BEGIN
	DECLARE @restaurantIDToUpdate int
	DECLARE @newReviewValue decimal(2,1)
	SELECT @restaurantIDToUpdate = restaurant_id FROM inserted;
	SELECT @newReviewValue = AVG(rating) FROM review WHERE review.restaurant_id=@restaurantIDToUpdate;
	
	UPDATE restaurant SET rating = @newReviewValue WHERE restaurant.id=@restaurantIDToUpdate;

END
GO
/****** Object:  Table [dbo].[order_item]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[order_item](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[order_id] [int] NULL,
	[menu_item_id] [int] NULL,
	[note] [text] NULL,
	[order_item_status] [int] NULL,
	[last_changed] [datetime2](7) NULL,
	[version] [int] NOT NULL,
	[bill_id] [int] NULL,
	[sides_id] [int] NULL,
 CONSTRAINT [PK__order_it__3213E83F164452B1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[review_order_item]    Script Date: 03/31/2013 15:46:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[review_order_item](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[rating] [int] NULL,
	[review_text] [nvarchar](max) NULL,
	[order_item_id] [int] NOT NULL,
	[review_id] [int] NOT NULL,
	[submitted_on] [date] NOT NULL,
 CONSTRAINT [PK_review_order_item] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Default [DF_restaurant_version]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[restaurant] ADD  CONSTRAINT [DF_restaurant_version]  DEFAULT ((1)) FOR [version]
GO
/****** Object:  Default [DF_restaurant_is_deleted]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[restaurant] ADD  CONSTRAINT [DF_restaurant_is_deleted]  DEFAULT ((0)) FOR [is_deleted]
GO
/****** Object:  Default [DF_category_version]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[category] ADD  CONSTRAINT [DF_category_version]  DEFAULT ((1)) FOR [version]
GO
/****** Object:  Default [DF_menu_is_deleted]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[menu] ADD  CONSTRAINT [DF_menu_is_deleted]  DEFAULT ((0)) FOR [is_active]
GO
/****** Object:  Default [DF_menu_is_delete]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[menu] ADD  CONSTRAINT [DF_menu_is_delete]  DEFAULT ((0)) FOR [is_deleted]
GO
/****** Object:  Default [DF_menu_version]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[menu] ADD  CONSTRAINT [DF_menu_version]  DEFAULT ((1)) FOR [version]
GO
/****** Object:  Default [DF_item_version]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[item] ADD  CONSTRAINT [DF_item_version]  DEFAULT ((1)) FOR [version]
GO
/****** Object:  Default [DF_waiter_version]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[waiter] ADD  CONSTRAINT [DF_waiter_version]  DEFAULT ((1)) FOR [version]
GO
/****** Object:  Default [DF_menu_category_is_deleted]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[menu_category] ADD  CONSTRAINT [DF_menu_category_is_deleted]  DEFAULT ((0)) FOR [is_deleted]
GO
/****** Object:  Default [DF_menu_category_is_active]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[menu_category] ADD  CONSTRAINT [DF_menu_category_is_active]  DEFAULT ((1)) FOR [is_active]
GO
/****** Object:  Default [DF_menu_category_version]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[menu_category] ADD  CONSTRAINT [DF_menu_category_version]  DEFAULT ((1)) FOR [version]
GO
/****** Object:  Default [DF_user_version]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[user] ADD  CONSTRAINT [DF_user_version]  DEFAULT ((1)) FOR [version]
GO
/****** Object:  Default [DF_service_request_version]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[service_request] ADD  CONSTRAINT [DF_service_request_version]  DEFAULT ((1)) FOR [version]
GO
/****** Object:  Default [DF_order_version]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[order] ADD  CONSTRAINT [DF_order_version]  DEFAULT ((1)) FOR [version]
GO
/****** Object:  Default [DF_menu_item_is_deleted]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[menu_item] ADD  CONSTRAINT [DF_menu_item_is_deleted]  DEFAULT ((0)) FOR [is_deleted]
GO
/****** Object:  Default [DF_menu_item_is_active]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[menu_item] ADD  CONSTRAINT [DF_menu_item_is_active]  DEFAULT ((1)) FOR [is_active]
GO
/****** Object:  Default [DF_menu_item_version]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[menu_item] ADD  CONSTRAINT [DF_menu_item_version]  DEFAULT ((1)) FOR [version]
GO
/****** Object:  Default [DF_bill_version]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[bill] ADD  CONSTRAINT [DF_bill_version]  DEFAULT ((1)) FOR [version]
GO
/****** Object:  Default [DF_order_item_version]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[order_item] ADD  CONSTRAINT [DF_order_item_version]  DEFAULT ((1)) FOR [version]
GO
/****** Object:  ForeignKey [FK_menu_restaurant]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[menu]  WITH CHECK ADD  CONSTRAINT [FK_menu_restaurant] FOREIGN KEY([resto_id])
REFERENCES [dbo].[restaurant] ([id])
GO
ALTER TABLE [dbo].[menu] CHECK CONSTRAINT [FK_menu_restaurant]
GO
/****** Object:  ForeignKey [FK_item_category]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[item]  WITH CHECK ADD  CONSTRAINT [FK_item_category] FOREIGN KEY([category_id])
REFERENCES [dbo].[category] ([id])
GO
ALTER TABLE [dbo].[item] CHECK CONSTRAINT [FK_item_category]
GO
/****** Object:  ForeignKey [FK_table_restaurant]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[table]  WITH CHECK ADD  CONSTRAINT [FK_table_restaurant] FOREIGN KEY([restaurant_id])
REFERENCES [dbo].[restaurant] ([id])
GO
ALTER TABLE [dbo].[table] CHECK CONSTRAINT [FK_table_restaurant]
GO
/****** Object:  ForeignKey [FK_waiter_restaurant]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[waiter]  WITH CHECK ADD  CONSTRAINT [FK_waiter_restaurant] FOREIGN KEY([resto_id])
REFERENCES [dbo].[restaurant] ([id])
GO
ALTER TABLE [dbo].[waiter] CHECK CONSTRAINT [FK_waiter_restaurant]
GO
/****** Object:  ForeignKey [FK_menu_category_category]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[menu_category]  WITH CHECK ADD  CONSTRAINT [FK_menu_category_category] FOREIGN KEY([category_id])
REFERENCES [dbo].[category] ([id])
GO
ALTER TABLE [dbo].[menu_category] CHECK CONSTRAINT [FK_menu_category_category]
GO
/****** Object:  ForeignKey [FK_menu_category_menu]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[menu_category]  WITH CHECK ADD  CONSTRAINT [FK_menu_category_menu] FOREIGN KEY([menu_id])
REFERENCES [dbo].[menu] ([id])
GO
ALTER TABLE [dbo].[menu_category] CHECK CONSTRAINT [FK_menu_category_menu]
GO
/****** Object:  ForeignKey [FK_user_table]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[user]  WITH CHECK ADD  CONSTRAINT [FK_user_table] FOREIGN KEY([current_table_id])
REFERENCES [dbo].[table] ([id])
GO
ALTER TABLE [dbo].[user] CHECK CONSTRAINT [FK_user_table]
GO
/****** Object:  ForeignKey [FK_service_request_table1]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[service_request]  WITH CHECK ADD  CONSTRAINT [FK_service_request_table1] FOREIGN KEY([table_id])
REFERENCES [dbo].[table] ([id])
GO
ALTER TABLE [dbo].[service_request] CHECK CONSTRAINT [FK_service_request_table1]
GO
/****** Object:  ForeignKey [FK_restaurant_user_restaurant]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[restaurant_user]  WITH CHECK ADD  CONSTRAINT [FK_restaurant_user_restaurant] FOREIGN KEY([restaurant_id])
REFERENCES [dbo].[restaurant] ([id])
GO
ALTER TABLE [dbo].[restaurant_user] CHECK CONSTRAINT [FK_restaurant_user_restaurant]
GO
/****** Object:  ForeignKey [FK_restaurant_user_user]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[restaurant_user]  WITH CHECK ADD  CONSTRAINT [FK_restaurant_user_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
GO
ALTER TABLE [dbo].[restaurant_user] CHECK CONSTRAINT [FK_restaurant_user_user]
GO
/****** Object:  ForeignKey [FK_friendship_user]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[friendship]  WITH CHECK ADD  CONSTRAINT [FK_friendship_user] FOREIGN KEY([first_user])
REFERENCES [dbo].[user] ([id])
GO
ALTER TABLE [dbo].[friendship] CHECK CONSTRAINT [FK_friendship_user]
GO
/****** Object:  ForeignKey [FK_friendship_user1]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[friendship]  WITH CHECK ADD  CONSTRAINT [FK_friendship_user1] FOREIGN KEY([second_user])
REFERENCES [dbo].[user] ([id])
GO
ALTER TABLE [dbo].[friendship] CHECK CONSTRAINT [FK_friendship_user1]
GO
/****** Object:  ForeignKey [FK_sides_menu_category]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[sides]  WITH CHECK ADD  CONSTRAINT [FK_sides_menu_category] FOREIGN KEY([menu_category_id])
REFERENCES [dbo].[menu_category] ([id])
GO
ALTER TABLE [dbo].[sides] CHECK CONSTRAINT [FK_sides_menu_category]
GO
/****** Object:  ForeignKey [FK_order_table]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[order]  WITH CHECK ADD  CONSTRAINT [FK_order_table] FOREIGN KEY([table_id])
REFERENCES [dbo].[table] ([id])
GO
ALTER TABLE [dbo].[order] CHECK CONSTRAINT [FK_order_table]
GO
/****** Object:  ForeignKey [FK_order_user]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[order]  WITH CHECK ADD  CONSTRAINT [FK_order_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
GO
ALTER TABLE [dbo].[order] CHECK CONSTRAINT [FK_order_user]
GO
/****** Object:  ForeignKey [FK_order_waiter]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[order]  WITH CHECK ADD  CONSTRAINT [FK_order_waiter] FOREIGN KEY([waiter_id])
REFERENCES [dbo].[waiter] ([id])
GO
ALTER TABLE [dbo].[order] CHECK CONSTRAINT [FK_order_waiter]
GO
/****** Object:  ForeignKey [FK_menu_item_instance_menu_item]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[menu_item]  WITH CHECK ADD  CONSTRAINT [FK_menu_item_instance_menu_item] FOREIGN KEY([item_id])
REFERENCES [dbo].[item] ([id])
GO
ALTER TABLE [dbo].[menu_item] CHECK CONSTRAINT [FK_menu_item_instance_menu_item]
GO
/****** Object:  ForeignKey [FK_menu_item_menu_category]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[menu_item]  WITH CHECK ADD  CONSTRAINT [FK_menu_item_menu_category] FOREIGN KEY([menu_category_id])
REFERENCES [dbo].[menu_category] ([id])
GO
ALTER TABLE [dbo].[menu_item] CHECK CONSTRAINT [FK_menu_item_menu_category]
GO
/****** Object:  ForeignKey [FK_review_order]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[review]  WITH CHECK ADD  CONSTRAINT [FK_review_order] FOREIGN KEY([order_id])
REFERENCES [dbo].[order] ([id])
GO
ALTER TABLE [dbo].[review] CHECK CONSTRAINT [FK_review_order]
GO
/****** Object:  ForeignKey [FK_review_restaurant]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[review]  WITH CHECK ADD  CONSTRAINT [FK_review_restaurant] FOREIGN KEY([restaurant_id])
REFERENCES [dbo].[restaurant] ([id])
GO
ALTER TABLE [dbo].[review] CHECK CONSTRAINT [FK_review_restaurant]
GO
/****** Object:  ForeignKey [FK_review_user]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[review]  WITH CHECK ADD  CONSTRAINT [FK_review_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
ON UPDATE SET NULL
GO
ALTER TABLE [dbo].[review] CHECK CONSTRAINT [FK_review_user]
GO
/****** Object:  ForeignKey [FK_bill_order]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[bill]  WITH CHECK ADD  CONSTRAINT [FK_bill_order] FOREIGN KEY([order_id])
REFERENCES [dbo].[order] ([id])
GO
ALTER TABLE [dbo].[bill] CHECK CONSTRAINT [FK_bill_order]
GO
/****** Object:  ForeignKey [FK_order_item_bill]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[order_item]  WITH CHECK ADD  CONSTRAINT [FK_order_item_bill] FOREIGN KEY([bill_id])
REFERENCES [dbo].[bill] ([id])
GO
ALTER TABLE [dbo].[order_item] CHECK CONSTRAINT [FK_order_item_bill]
GO
/****** Object:  ForeignKey [FK_order_item_menu_item]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[order_item]  WITH CHECK ADD  CONSTRAINT [FK_order_item_menu_item] FOREIGN KEY([menu_item_id])
REFERENCES [dbo].[menu_item] ([id])
GO
ALTER TABLE [dbo].[order_item] CHECK CONSTRAINT [FK_order_item_menu_item]
GO
/****** Object:  ForeignKey [FK_order_item_order]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[order_item]  WITH CHECK ADD  CONSTRAINT [FK_order_item_order] FOREIGN KEY([order_id])
REFERENCES [dbo].[order] ([id])
GO
ALTER TABLE [dbo].[order_item] CHECK CONSTRAINT [FK_order_item_order]
GO
/****** Object:  ForeignKey [FK_order_item_sides]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[order_item]  WITH CHECK ADD  CONSTRAINT [FK_order_item_sides] FOREIGN KEY([sides_id])
REFERENCES [dbo].[sides] ([id])
GO
ALTER TABLE [dbo].[order_item] CHECK CONSTRAINT [FK_order_item_sides]
GO
/****** Object:  ForeignKey [FK_review_order_item_order_item]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[review_order_item]  WITH CHECK ADD  CONSTRAINT [FK_review_order_item_order_item] FOREIGN KEY([order_item_id])
REFERENCES [dbo].[order_item] ([id])
GO
ALTER TABLE [dbo].[review_order_item] CHECK CONSTRAINT [FK_review_order_item_order_item]
GO
/****** Object:  ForeignKey [FK_review_order_item_review]    Script Date: 03/31/2013 15:46:30 ******/
ALTER TABLE [dbo].[review_order_item]  WITH CHECK ADD  CONSTRAINT [FK_review_order_item_review] FOREIGN KEY([review_id])
REFERENCES [dbo].[review] ([id])
GO
ALTER TABLE [dbo].[review_order_item] CHECK CONSTRAINT [FK_review_order_item_review]
GO
