IF OBJECT_ID(N'dbo.QLNV_CongViecComments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.QLNV_CongViecComments (
        Id nvarchar(450) NOT NULL PRIMARY KEY,
        Id_CongViec nvarchar(450) NOT NULL,
        ParentCommentId nvarchar(450) NULL,
        NoiDung nvarchar(max) NOT NULL,
        CompanyId nvarchar(450) NOT NULL DEFAULT '',
        GroupId nvarchar(450) NOT NULL DEFAULT '',
        CreateAt datetime2 NOT NULL,
        CreateBy nvarchar(256) NOT NULL DEFAULT '',
        CreateByUserId nvarchar(450) NULL,
        UpdateAt datetime2 NULL,
        UpdateBy nvarchar(256) NULL,
        IsEdited int NOT NULL DEFAULT 0,
        IsActive int NOT NULL DEFAULT 1
    );
END;

IF OBJECT_ID(N'dbo.QLNV_CongViecMentions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.QLNV_CongViecMentions (
        Id nvarchar(450) NOT NULL PRIMARY KEY,
        Id_CongViec nvarchar(450) NOT NULL,
        CommentId nvarchar(450) NOT NULL,
        UserId nvarchar(450) NOT NULL,
        UserName nvarchar(256) NOT NULL DEFAULT '',
        GroupId nvarchar(450) NOT NULL DEFAULT '',
        CreateAt datetime2 NOT NULL,
        CreateBy nvarchar(256) NOT NULL DEFAULT '',
        IsActive int NOT NULL DEFAULT 1
    );
END;

IF OBJECT_ID(N'dbo.QLNV_CongViecWatchers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.QLNV_CongViecWatchers (
        Id nvarchar(450) NOT NULL PRIMARY KEY,
        Id_CongViec nvarchar(450) NOT NULL,
        UserId nvarchar(450) NOT NULL,
        UserName nvarchar(256) NOT NULL DEFAULT '',
        CompanyId nvarchar(450) NOT NULL DEFAULT '',
        GroupId nvarchar(450) NOT NULL DEFAULT '',
        CreateAt datetime2 NOT NULL,
        CreateBy nvarchar(256) NOT NULL DEFAULT '',
        IsActive int NOT NULL DEFAULT 1
    );
END;

IF OBJECT_ID(N'dbo.QLNV_CongViecActivities', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.QLNV_CongViecActivities (
        Id nvarchar(450) NOT NULL PRIMARY KEY,
        Id_CongViec nvarchar(450) NOT NULL,
        EventType nvarchar(80) NOT NULL DEFAULT '',
        FieldName nvarchar(100) NULL,
        OldValue nvarchar(max) NULL,
        NewValue nvarchar(max) NULL,
        Description nvarchar(max) NOT NULL DEFAULT '',
        ActorUserId nvarchar(450) NULL,
        ActorUserName nvarchar(256) NOT NULL DEFAULT '',
        MetadataJson nvarchar(max) NULL,
        CompanyId nvarchar(450) NOT NULL DEFAULT '',
        GroupId nvarchar(450) NOT NULL DEFAULT '',
        CreateAt datetime2 NOT NULL,
        IsActive int NOT NULL DEFAULT 1
    );
END;

IF OBJECT_ID(N'dbo.QLNV_CongViecEvents', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.QLNV_CongViecEvents (
        Id nvarchar(450) NOT NULL PRIMARY KEY,
        Id_CongViec nvarchar(450) NOT NULL,
        EventName nvarchar(120) NOT NULL DEFAULT '',
        PayloadJson nvarchar(max) NOT NULL DEFAULT '',
        TargetUserIdsJson nvarchar(max) NOT NULL DEFAULT '[]',
        IsPublished int NOT NULL DEFAULT 0,
        PublishedAt datetime2 NULL,
        RetryCount int NOT NULL DEFAULT 0,
        CompanyId nvarchar(450) NOT NULL DEFAULT '',
        GroupId nvarchar(450) NOT NULL DEFAULT '',
        CreateAt datetime2 NOT NULL,
        CreateBy nvarchar(256) NOT NULL DEFAULT '',
        IsActive int NOT NULL DEFAULT 1
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_QLNV_CongViecComments_Id_CongViec_CreateAt')
    CREATE INDEX IX_QLNV_CongViecComments_Id_CongViec_CreateAt ON dbo.QLNV_CongViecComments(Id_CongViec, CreateAt);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_QLNV_CongViecMentions_CommentId_UserId')
    CREATE INDEX IX_QLNV_CongViecMentions_CommentId_UserId ON dbo.QLNV_CongViecMentions(CommentId, UserId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_QLNV_CongViecWatchers_Id_CongViec_UserId')
    CREATE INDEX IX_QLNV_CongViecWatchers_Id_CongViec_UserId ON dbo.QLNV_CongViecWatchers(Id_CongViec, UserId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_QLNV_CongViecActivities_Id_CongViec_CreateAt')
    CREATE INDEX IX_QLNV_CongViecActivities_Id_CongViec_CreateAt ON dbo.QLNV_CongViecActivities(Id_CongViec, CreateAt);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_QLNV_CongViecEvents_Id_CongViec_CreateAt')
    CREATE INDEX IX_QLNV_CongViecEvents_Id_CongViec_CreateAt ON dbo.QLNV_CongViecEvents(Id_CongViec, CreateAt);
