﻿CREATE TABLE [dbo].[UserProfile] (
    [UserId]   INT            IDENTITY (1, 1) NOT NULL,
    [UserName] NVARCHAR (MAX) UNIQUE NOT NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC)
);

