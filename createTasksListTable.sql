CREATE TABLE [dbo].[TaskList] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Description]   NVARCHAR (20)  NOT NULL,
    [DueDate]       DATE           NOT NULL,
    [Complete]      BIT            NOT NULL,
    [UserId]        NVARCHAR (450) NOT NULL,
    PRIMARY KEY     CLUSTERED ([Id] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);