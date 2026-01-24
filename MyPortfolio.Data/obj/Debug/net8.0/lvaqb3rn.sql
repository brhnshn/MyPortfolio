IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Abouts] (
    [Id] int NOT NULL IDENTITY,
    [FullName] nvarchar(100) NOT NULL,
    [Title] nvarchar(150) NOT NULL,
    [Bio] nvarchar(max) NOT NULL,
    [ImageUrl] nvarchar(250) NOT NULL,
    [CvUrl] nvarchar(250) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_Abouts] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AdminUsers] (
    [Id] int NOT NULL IDENTITY,
    [UserName] nvarchar(50) NOT NULL,
    [PasswordHash] nvarchar(250) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_AdminUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Projects] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(150) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [ImageUrl] nvarchar(250) NOT NULL,
    [GithubLink] nvarchar(250) NULL,
    [LiveLink] nvarchar(250) NULL,
    [DisplayOrder] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_Projects] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SiteSettings] (
    [Id] int NOT NULL IDENTITY,
    [SiteTitle] nvarchar(100) NOT NULL,
    [MetaDescription] nvarchar(500) NOT NULL,
    [FooterText] nvarchar(200) NOT NULL,
    [LogoUrl] nvarchar(250) NOT NULL,
    [FaviconUrl] nvarchar(250) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [Phone] nvarchar(20) NOT NULL,
    [FormspreeId] nvarchar(100) NOT NULL,
    [GithubUrl] nvarchar(250) NULL,
    [LinkedinUrl] nvarchar(250) NULL,
    [XUrl] nvarchar(250) NULL,
    [InstagramUrl] nvarchar(250) NULL,
    [ActiveTemplate] nvarchar(50) NOT NULL,
    [PrimaryColor] nvarchar(7) NOT NULL,
    [SecondaryColor] nvarchar(7) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_SiteSettings] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Skills] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(50) NOT NULL,
    [Percentage] tinyint NOT NULL,
    [Category] nvarchar(50) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_Skills] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260111210920_InitialCreate', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Abouts].[Bio]', N'Description', N'COLUMN';
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Abouts]') AND [c].[name] = N'Title');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Abouts] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Abouts] ALTER COLUMN [Title] nvarchar(max) NOT NULL;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Abouts]') AND [c].[name] = N'ImageUrl');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Abouts] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Abouts] ALTER COLUMN [ImageUrl] nvarchar(max) NULL;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Abouts]') AND [c].[name] = N'FullName');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Abouts] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Abouts] ALTER COLUMN [FullName] nvarchar(max) NULL;
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Abouts]') AND [c].[name] = N'CvUrl');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Abouts] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Abouts] ALTER COLUMN [CvUrl] nvarchar(max) NULL;
GO

ALTER TABLE [Abouts] ADD [Address] nvarchar(max) NULL;
GO

ALTER TABLE [Abouts] ADD [Details] nvarchar(max) NULL;
GO

ALTER TABLE [Abouts] ADD [Email] nvarchar(max) NULL;
GO

ALTER TABLE [Abouts] ADD [Phone] nvarchar(max) NULL;
GO

ALTER TABLE [Abouts] ADD [SubDescription] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260111225303_UpdateAboutColumns', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260111225608_AddServiceTable', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Services] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [IconUrl] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_Services] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Testimonials] (
    [Id] int NOT NULL IDENTITY,
    [ClientName] nvarchar(max) NOT NULL,
    [Company] nvarchar(max) NOT NULL,
    [Comment] nvarchar(max) NOT NULL,
    [ImageUrl] nvarchar(max) NOT NULL,
    [Title] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_Testimonials] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260111231148_AddTestimonialsTable', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Messages] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Subject] nvarchar(max) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [IsRead] bit NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260111232125_AddMessageTable', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Features] (
    [Id] int NOT NULL IDENTITY,
    [Header] nvarchar(max) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_Features] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260112000409_AddFeatureTable', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Projects] ADD [Platform] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Projects] ADD [Price] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [Projects] ADD [ProjectUrl] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260112002444_ProjectTableFixed', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Projects]') AND [c].[name] = N'Price');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Projects] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [Projects] DROP COLUMN [Price];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260112003219_RemovePriceField', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Projects]') AND [c].[name] = N'LiveLink');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Projects] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [Projects] DROP COLUMN [LiveLink];
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Projects]') AND [c].[name] = N'ProjectUrl');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Projects] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [Projects] ALTER COLUMN [ProjectUrl] nvarchar(250) NULL;
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Projects]') AND [c].[name] = N'Platform');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Projects] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [Projects] ALTER COLUMN [Platform] nvarchar(100) NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260112124226_fixprojectfinal', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP TABLE [AdminUsers];
GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Testimonials]') AND [c].[name] = N'ImageUrl');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Testimonials] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [Testimonials] ALTER COLUMN [ImageUrl] nvarchar(max) NULL;
GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Testimonials]') AND [c].[name] = N'Company');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Testimonials] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [Testimonials] ALTER COLUMN [Company] nvarchar(max) NULL;
GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Testimonials]') AND [c].[name] = N'Comment');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Testimonials] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [Testimonials] ALTER COLUMN [Comment] nvarchar(max) NULL;
GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Testimonials]') AND [c].[name] = N'ClientName');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Testimonials] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [Testimonials] ALTER COLUMN [ClientName] nvarchar(max) NULL;
GO

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SiteSettings]') AND [c].[name] = N'Phone');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [SiteSettings] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [SiteSettings] ALTER COLUMN [Phone] nvarchar(20) NULL;
GO

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SiteSettings]') AND [c].[name] = N'MetaDescription');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [SiteSettings] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [SiteSettings] ALTER COLUMN [MetaDescription] nvarchar(500) NULL;
GO

DECLARE @var14 sysname;
SELECT @var14 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SiteSettings]') AND [c].[name] = N'LogoUrl');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [SiteSettings] DROP CONSTRAINT [' + @var14 + '];');
ALTER TABLE [SiteSettings] ALTER COLUMN [LogoUrl] nvarchar(250) NULL;
GO

DECLARE @var15 sysname;
SELECT @var15 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SiteSettings]') AND [c].[name] = N'FormspreeId');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [SiteSettings] DROP CONSTRAINT [' + @var15 + '];');
ALTER TABLE [SiteSettings] ALTER COLUMN [FormspreeId] nvarchar(100) NULL;
GO

DECLARE @var16 sysname;
SELECT @var16 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SiteSettings]') AND [c].[name] = N'FooterText');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [SiteSettings] DROP CONSTRAINT [' + @var16 + '];');
ALTER TABLE [SiteSettings] ALTER COLUMN [FooterText] nvarchar(200) NULL;
GO

DECLARE @var17 sysname;
SELECT @var17 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SiteSettings]') AND [c].[name] = N'FaviconUrl');
IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [SiteSettings] DROP CONSTRAINT [' + @var17 + '];');
ALTER TABLE [SiteSettings] ALTER COLUMN [FaviconUrl] nvarchar(250) NULL;
GO

DECLARE @var18 sysname;
SELECT @var18 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SiteSettings]') AND [c].[name] = N'Email');
IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [SiteSettings] DROP CONSTRAINT [' + @var18 + '];');
ALTER TABLE [SiteSettings] ALTER COLUMN [Email] nvarchar(100) NULL;
GO

DECLARE @var19 sysname;
SELECT @var19 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Services]') AND [c].[name] = N'Title');
IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [Services] DROP CONSTRAINT [' + @var19 + '];');
ALTER TABLE [Services] ALTER COLUMN [Title] nvarchar(max) NULL;
GO

DECLARE @var20 sysname;
SELECT @var20 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Services]') AND [c].[name] = N'IconUrl');
IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [Services] DROP CONSTRAINT [' + @var20 + '];');
ALTER TABLE [Services] ALTER COLUMN [IconUrl] nvarchar(max) NULL;
GO

DECLARE @var21 sysname;
SELECT @var21 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Services]') AND [c].[name] = N'Description');
IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [Services] DROP CONSTRAINT [' + @var21 + '];');
ALTER TABLE [Services] ALTER COLUMN [Description] nvarchar(max) NULL;
GO

DECLARE @var22 sysname;
SELECT @var22 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Projects]') AND [c].[name] = N'Platform');
IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [Projects] DROP CONSTRAINT [' + @var22 + '];');
ALTER TABLE [Projects] ALTER COLUMN [Platform] nvarchar(100) NULL;
GO

DECLARE @var23 sysname;
SELECT @var23 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Projects]') AND [c].[name] = N'ImageUrl');
IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [Projects] DROP CONSTRAINT [' + @var23 + '];');
ALTER TABLE [Projects] ALTER COLUMN [ImageUrl] nvarchar(250) NULL;
GO

DECLARE @var24 sysname;
SELECT @var24 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Messages]') AND [c].[name] = N'Subject');
IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [Messages] DROP CONSTRAINT [' + @var24 + '];');
ALTER TABLE [Messages] ALTER COLUMN [Subject] nvarchar(max) NULL;
GO

DECLARE @var25 sysname;
SELECT @var25 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Messages]') AND [c].[name] = N'Name');
IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [Messages] DROP CONSTRAINT [' + @var25 + '];');
ALTER TABLE [Messages] ALTER COLUMN [Name] nvarchar(max) NULL;
GO

DECLARE @var26 sysname;
SELECT @var26 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Messages]') AND [c].[name] = N'Email');
IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [Messages] DROP CONSTRAINT [' + @var26 + '];');
ALTER TABLE [Messages] ALTER COLUMN [Email] nvarchar(max) NULL;
GO

DECLARE @var27 sysname;
SELECT @var27 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Messages]') AND [c].[name] = N'Content');
IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [Messages] DROP CONSTRAINT [' + @var27 + '];');
ALTER TABLE [Messages] ALTER COLUMN [Content] nvarchar(max) NULL;
GO

DECLARE @var28 sysname;
SELECT @var28 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Features]') AND [c].[name] = N'Title');
IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [Features] DROP CONSTRAINT [' + @var28 + '];');
ALTER TABLE [Features] ALTER COLUMN [Title] nvarchar(max) NULL;
GO

DECLARE @var29 sysname;
SELECT @var29 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Features]') AND [c].[name] = N'Name');
IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [Features] DROP CONSTRAINT [' + @var29 + '];');
ALTER TABLE [Features] ALTER COLUMN [Name] nvarchar(max) NULL;
GO

DECLARE @var30 sysname;
SELECT @var30 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Features]') AND [c].[name] = N'Header');
IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [Features] DROP CONSTRAINT [' + @var30 + '];');
ALTER TABLE [Features] ALTER COLUMN [Header] nvarchar(max) NULL;
GO

DECLARE @var31 sysname;
SELECT @var31 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Abouts]') AND [c].[name] = N'Title');
IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [Abouts] DROP CONSTRAINT [' + @var31 + '];');
ALTER TABLE [Abouts] ALTER COLUMN [Title] nvarchar(max) NULL;
GO

DECLARE @var32 sysname;
SELECT @var32 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Abouts]') AND [c].[name] = N'Description');
IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [Abouts] DROP CONSTRAINT [' + @var32 + '];');
ALTER TABLE [Abouts] ALTER COLUMN [Description] nvarchar(max) NULL;
GO

CREATE TABLE [AspNetRoles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Surname] nvarchar(max) NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] int NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] int NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260112125245_AddIdentitySetup', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var33 sysname;
SELECT @var33 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Features]') AND [c].[name] = N'CreatedDate');
IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [Features] DROP CONSTRAINT [' + @var33 + '];');
ALTER TABLE [Features] DROP COLUMN [CreatedDate];
GO

DECLARE @var34 sysname;
SELECT @var34 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Features]') AND [c].[name] = N'UpdatedDate');
IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [Features] DROP CONSTRAINT [' + @var34 + '];');
ALTER TABLE [Features] DROP COLUMN [UpdatedDate];
GO

ALTER TABLE [Features] ADD [ImageUrl] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260112173929_FeatureImageAdded', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Abouts].[Email]', N'Mail', N'COLUMN';
GO

ALTER TABLE [Abouts] ADD [Age] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260112183310_updatefiles', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Abouts].[Mail]', N'ProjectCount', N'COLUMN';
GO

ALTER TABLE [Abouts] ADD [CustomerCount] nvarchar(max) NULL;
GO

ALTER TABLE [Abouts] ADD [Email] nvarchar(max) NULL;
GO

ALTER TABLE [Abouts] ADD [ExperienceYear] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260112190308_AddAboutStatsCols', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var35 sysname;
SELECT @var35 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Services]') AND [c].[name] = N'Title');
IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [Services] DROP CONSTRAINT [' + @var35 + '];');
UPDATE [Services] SET [Title] = N'' WHERE [Title] IS NULL;
ALTER TABLE [Services] ALTER COLUMN [Title] nvarchar(50) NOT NULL;
ALTER TABLE [Services] ADD DEFAULT N'' FOR [Title];
GO

DECLARE @var36 sysname;
SELECT @var36 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Services]') AND [c].[name] = N'Description');
IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [Services] DROP CONSTRAINT [' + @var36 + '];');
UPDATE [Services] SET [Description] = N'' WHERE [Description] IS NULL;
ALTER TABLE [Services] ALTER COLUMN [Description] nvarchar(225) NOT NULL;
ALTER TABLE [Services] ADD DEFAULT N'' FOR [Description];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260112195640_ServiceDescriptionLimit', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var37 sysname;
SELECT @var37 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Services]') AND [c].[name] = N'Title');
IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [Services] DROP CONSTRAINT [' + @var37 + '];');
ALTER TABLE [Services] ALTER COLUMN [Title] nvarchar(30) NOT NULL;
GO

DECLARE @var38 sysname;
SELECT @var38 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Services]') AND [c].[name] = N'Description');
IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [Services] DROP CONSTRAINT [' + @var38 + '];');
ALTER TABLE [Services] ALTER COLUMN [Description] nvarchar(100) NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260112201048_eklendi', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260112201221_eklendii', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var39 sysname;
SELECT @var39 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Messages]') AND [c].[name] = N'Subject');
IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [Messages] DROP CONSTRAINT [' + @var39 + '];');
UPDATE [Messages] SET [Subject] = N'' WHERE [Subject] IS NULL;
ALTER TABLE [Messages] ALTER COLUMN [Subject] nvarchar(max) NOT NULL;
ALTER TABLE [Messages] ADD DEFAULT N'' FOR [Subject];
GO

DECLARE @var40 sysname;
SELECT @var40 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Messages]') AND [c].[name] = N'Name');
IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [Messages] DROP CONSTRAINT [' + @var40 + '];');
UPDATE [Messages] SET [Name] = N'' WHERE [Name] IS NULL;
ALTER TABLE [Messages] ALTER COLUMN [Name] nvarchar(max) NOT NULL;
ALTER TABLE [Messages] ADD DEFAULT N'' FOR [Name];
GO

DECLARE @var41 sysname;
SELECT @var41 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Messages]') AND [c].[name] = N'Email');
IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [Messages] DROP CONSTRAINT [' + @var41 + '];');
UPDATE [Messages] SET [Email] = N'' WHERE [Email] IS NULL;
ALTER TABLE [Messages] ALTER COLUMN [Email] nvarchar(max) NOT NULL;
ALTER TABLE [Messages] ADD DEFAULT N'' FOR [Email];
GO

DECLARE @var42 sysname;
SELECT @var42 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Messages]') AND [c].[name] = N'Content');
IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [Messages] DROP CONSTRAINT [' + @var42 + '];');
UPDATE [Messages] SET [Content] = N'' WHERE [Content] IS NULL;
ALTER TABLE [Messages] ALTER COLUMN [Content] nvarchar(max) NOT NULL;
ALTER TABLE [Messages] ADD DEFAULT N'' FOR [Content];
GO

ALTER TABLE [Messages] ADD [Date] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260113151437_Messageupdate', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260113152012_updatecontact', N'8.0.22');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [ContactInfos] (
    [Id] int NOT NULL IDENTITY,
    [Address] nvarchar(250) NULL,
    [Phone] nvarchar(50) NULL,
    [Email] nvarchar(100) NULL,
    [MapUrl] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_ContactInfos] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260113152412_ContactInfoTableFix', N'8.0.22');
GO

COMMIT;
GO

