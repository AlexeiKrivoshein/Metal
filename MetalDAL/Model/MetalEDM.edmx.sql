
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/06/2021 15:19:26
-- Generated from EDMX file: D:\Projects\Metal\MetalDAL\Model\MetalEDM.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Metal];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_СustomerOrder]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderSet] DROP CONSTRAINT [FK_СustomerOrder];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderGroupOrder]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderSet] DROP CONSTRAINT [FK_OrderGroupOrder];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderOrderWork]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderOperationSet] DROP CONSTRAINT [FK_OrderOrderWork];
GO
IF OBJECT_ID(N'[dbo].[FK_OperationOrderOperation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderOperationSet] DROP CONSTRAINT [FK_OperationOrderOperation];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderLimitCard]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LimitCardMaterialSet] DROP CONSTRAINT [FK_OrderLimitCard];
GO
IF OBJECT_ID(N'[dbo].[FK_MaterialLimitCard]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LimitCardMaterialSet] DROP CONSTRAINT [FK_MaterialLimitCard];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderLimitCardWork]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LimitCardOperationSet] DROP CONSTRAINT [FK_OrderLimitCardWork];
GO
IF OBJECT_ID(N'[dbo].[FK_OperationLimitCardOperation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LimitCardOperationSet] DROP CONSTRAINT [FK_OperationLimitCardOperation];
GO
IF OBJECT_ID(N'[dbo].[FK_EmployeePost]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EmployeeSet] DROP CONSTRAINT [FK_EmployeePost];
GO
IF OBJECT_ID(N'[dbo].[FK_LimitCardMaterialLimitCardFactMaterial]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LimitCardFactMaterialSet] DROP CONSTRAINT [FK_LimitCardMaterialLimitCardFactMaterial];
GO
IF OBJECT_ID(N'[dbo].[FK_MaterialLimitCardFactMaterial]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LimitCardFactMaterialSet] DROP CONSTRAINT [FK_MaterialLimitCardFactMaterial];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderOperationEmployee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderOperationSet] DROP CONSTRAINT [FK_OrderOperationEmployee];
GO
IF OBJECT_ID(N'[dbo].[FK_MetalFileOrder]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MetalFileSet] DROP CONSTRAINT [FK_MetalFileOrder];
GO
IF OBJECT_ID(N'[dbo].[FK_EmployeeUserGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EmployeeSet] DROP CONSTRAINT [FK_EmployeeUserGroup];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[OrderSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderSet];
GO
IF OBJECT_ID(N'[dbo].[CustomerSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CustomerSet];
GO
IF OBJECT_ID(N'[dbo].[EmployeeSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EmployeeSet];
GO
IF OBJECT_ID(N'[dbo].[PostSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PostSet];
GO
IF OBJECT_ID(N'[dbo].[OperationSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OperationSet];
GO
IF OBJECT_ID(N'[dbo].[MaterialSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MaterialSet];
GO
IF OBJECT_ID(N'[dbo].[OrderGroupSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderGroupSet];
GO
IF OBJECT_ID(N'[dbo].[OrderOperationSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderOperationSet];
GO
IF OBJECT_ID(N'[dbo].[LimitCardMaterialSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LimitCardMaterialSet];
GO
IF OBJECT_ID(N'[dbo].[LimitCardOperationSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LimitCardOperationSet];
GO
IF OBJECT_ID(N'[dbo].[MetalFileSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MetalFileSet];
GO
IF OBJECT_ID(N'[dbo].[LimitCardFactMaterialSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LimitCardFactMaterialSet];
GO
IF OBJECT_ID(N'[dbo].[UserGroupSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserGroupSet];
GO
IF OBJECT_ID(N'[dbo].[SysTabSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SysTabSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'OrderSet'
CREATE TABLE [dbo].[OrderSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Number] int  NOT NULL,
    [Date] datetime  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Count] int  NOT NULL,
    [CustomerId] uniqueidentifier  NOT NULL,
    [ReadyDate] datetime  NOT NULL,
    [AcceptedDate] datetime  NOT NULL,
    [DrawingNumber] nvarchar(max)  NOT NULL,
    [DrawingState] int  NOT NULL,
    [IsCustomerMaterial] bit  NOT NULL,
    [CustomerMaterial] nvarchar(max)  NOT NULL,
    [CustomerReadyDate] datetime  NOT NULL,
    [MaterialAgreed] datetime  NOT NULL,
    [TechCalcPrice] float  NOT NULL,
    [TechCalcHour] int  NOT NULL,
    [TechCalcMinutes] int  NOT NULL,
    [TechCalcPriceDate] datetime  NOT NULL,
    [TechCalcMultiplier] float  NOT NULL,
    [TechMaterialReqDate] datetime  NOT NULL,
    [DirectorExpectedPrice] float  NOT NULL,
    [DirectorExpectedDay] int  NOT NULL,
    [DirectorExpectedDate] datetime  NOT NULL,
    [SalesMaterialAvailable] bit  NOT NULL,
    [SalesMaterialAvailableDate] datetime  NOT NULL,
    [SalesPrepaymentType] int  NOT NULL,
    [SalesPaymentType] int  NOT NULL,
    [SalesPrice] float  NOT NULL,
    [SalesPriceDate] datetime  NOT NULL,
    [SalesComOfferNumber] nvarchar(max)  NOT NULL,
    [SalesComOfferDate] datetime  NOT NULL,
    [AccSpecifNumber] nvarchar(max)  NOT NULL,
    [AccSpecifDate] datetime  NOT NULL,
    [AccBillNumber] nvarchar(max)  NOT NULL,
    [AccBillDate] datetime  NOT NULL,
    [AccPaymType] int  NOT NULL,
    [AccPaymDate] datetime  NOT NULL,
    [SalesMaterialOrderDate] datetime  NOT NULL,
    [SalesMaterialOrderReadyDate] datetime  NOT NULL,
    [DirectorOrderPlanedDate] datetime  NOT NULL,
    [OrderInManufactureDate] datetime  NOT NULL,
    [MaterialInManufactureDate] datetime  NOT NULL,
    [OrderInWorkDate] datetime  NOT NULL,
    [ManMadeDate] datetime  NOT NULL,
    [OTKProductGetDate] datetime  NOT NULL,
    [OTKProductDefectDate] datetime  NOT NULL,
    [OTKProductDefectInfo] nvarchar(max)  NOT NULL,
    [OTKProductCorrectDate] datetime  NOT NULL,
    [ManProductApplyDate] datetime  NOT NULL,
    [ManLimitCreateDate] datetime  NOT NULL,
    [AccCustomerInformedDate] datetime  NOT NULL,
    [AccOrderPaidDate] datetime  NOT NULL,
    [AccDocumentsToSendDate] datetime  NOT NULL,
    [SendOrderDeliveryType] int  NOT NULL,
    [SendDeliveryDate] datetime  NOT NULL,
    [OrderReadyType] int  NOT NULL,
    [OrderHoldDay] int  NOT NULL,
    [OrderCorruptReason] nvarchar(max)  NOT NULL,
    [OrderState] int  NOT NULL,
    [OrderGroupId] uniqueidentifier  NULL,
    [DateRec] datetime  NOT NULL,
    [Deleted] bit  NOT NULL,
    [Version] bigint  NOT NULL
);
GO

-- Creating table 'CustomerSet'
CREATE TABLE [dbo].[CustomerSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Employee] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NOT NULL,
    [Mail] nvarchar(max)  NOT NULL,
    [Fax] nvarchar(max)  NOT NULL,
    [Deleted] bit  NOT NULL,
    [Version] bigint  NOT NULL
);
GO

-- Creating table 'EmployeeSet'
CREATE TABLE [dbo].[EmployeeSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Secondname] nvarchar(max)  NOT NULL,
    [Patronymic] nvarchar(max)  NOT NULL,
    [PostId] uniqueidentifier  NULL,
    [UseForLogin] bit  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [Deleted] bit  NOT NULL,
    [Version] bigint  NOT NULL,
    [UserGroupId] uniqueidentifier  NULL
);
GO

-- Creating table 'PostSet'
CREATE TABLE [dbo].[PostSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Version] bigint  NOT NULL,
    [Deleted] bit  NOT NULL
);
GO

-- Creating table 'OperationSet'
CREATE TABLE [dbo].[OperationSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Deleted] bit  NOT NULL,
    [Version] bigint  NOT NULL
);
GO

-- Creating table 'MaterialSet'
CREATE TABLE [dbo].[MaterialSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Deleted] bit  NOT NULL,
    [Version] bigint  NOT NULL
);
GO

-- Creating table 'OrderGroupSet'
CREATE TABLE [dbo].[OrderGroupSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Deleted] bit  NOT NULL,
    [Version] bigint  NOT NULL
);
GO

-- Creating table 'OrderOperationSet'
CREATE TABLE [dbo].[OrderOperationSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Index] int  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OperationId] uniqueidentifier  NOT NULL,
    [Count] int  NOT NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NOT NULL,
    [Comment] nvarchar(max)  NOT NULL,
    [EmployeeId] uniqueidentifier  NULL
);
GO

-- Creating table 'LimitCardMaterialSet'
CREATE TABLE [dbo].[LimitCardMaterialSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Index] int  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [MaterialId] uniqueidentifier  NOT NULL,
    [Units] nvarchar(max)  NOT NULL,
    [UsagePerUnits] float  NOT NULL,
    [UsagePerOrder] float  NOT NULL,
    [Multiplicity] float  NOT NULL,
    [Price] float  NOT NULL
);
GO

-- Creating table 'LimitCardOperationSet'
CREATE TABLE [dbo].[LimitCardOperationSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Index] int  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OperationId] uniqueidentifier  NOT NULL,
    [ElapsedHours] smallint  NOT NULL,
    [ElapsedMinutes] smallint  NOT NULL,
    [PricePerHour] float  NOT NULL
);
GO

-- Creating table 'MetalFileSet'
CREATE TABLE [dbo].[MetalFileSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Path] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Index] int  NOT NULL,
    [ChunkCount] int  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'LimitCardFactMaterialSet'
CREATE TABLE [dbo].[LimitCardFactMaterialSet] (
    [Id] uniqueidentifier  NOT NULL,
    [LimitCardMaterialId] uniqueidentifier  NOT NULL,
    [MaterialId] uniqueidentifier  NOT NULL,
    [Count] float  NOT NULL,
    [Price] float  NOT NULL
);
GO

-- Creating table 'UserGroupSet'
CREATE TABLE [dbo].[UserGroupSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Rights] varbinary(max)  NOT NULL,
    [Version] bigint  NOT NULL,
    [Deleted] bit  NOT NULL
);
GO

-- Creating table 'SysTabSet'
CREATE TABLE [dbo].[SysTabSet] (
    [Id] uniqueidentifier  NOT NULL,
    [DataBaseVersion] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'OrderSet'
ALTER TABLE [dbo].[OrderSet]
ADD CONSTRAINT [PK_OrderSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CustomerSet'
ALTER TABLE [dbo].[CustomerSet]
ADD CONSTRAINT [PK_CustomerSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EmployeeSet'
ALTER TABLE [dbo].[EmployeeSet]
ADD CONSTRAINT [PK_EmployeeSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PostSet'
ALTER TABLE [dbo].[PostSet]
ADD CONSTRAINT [PK_PostSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OperationSet'
ALTER TABLE [dbo].[OperationSet]
ADD CONSTRAINT [PK_OperationSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MaterialSet'
ALTER TABLE [dbo].[MaterialSet]
ADD CONSTRAINT [PK_MaterialSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderGroupSet'
ALTER TABLE [dbo].[OrderGroupSet]
ADD CONSTRAINT [PK_OrderGroupSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderOperationSet'
ALTER TABLE [dbo].[OrderOperationSet]
ADD CONSTRAINT [PK_OrderOperationSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LimitCardMaterialSet'
ALTER TABLE [dbo].[LimitCardMaterialSet]
ADD CONSTRAINT [PK_LimitCardMaterialSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LimitCardOperationSet'
ALTER TABLE [dbo].[LimitCardOperationSet]
ADD CONSTRAINT [PK_LimitCardOperationSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MetalFileSet'
ALTER TABLE [dbo].[MetalFileSet]
ADD CONSTRAINT [PK_MetalFileSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LimitCardFactMaterialSet'
ALTER TABLE [dbo].[LimitCardFactMaterialSet]
ADD CONSTRAINT [PK_LimitCardFactMaterialSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserGroupSet'
ALTER TABLE [dbo].[UserGroupSet]
ADD CONSTRAINT [PK_UserGroupSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SysTabSet'
ALTER TABLE [dbo].[SysTabSet]
ADD CONSTRAINT [PK_SysTabSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [CustomerId] in table 'OrderSet'
ALTER TABLE [dbo].[OrderSet]
ADD CONSTRAINT [FK_СustomerOrder]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[CustomerSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_СustomerOrder'
CREATE INDEX [IX_FK_СustomerOrder]
ON [dbo].[OrderSet]
    ([CustomerId]);
GO

-- Creating foreign key on [OrderGroupId] in table 'OrderSet'
ALTER TABLE [dbo].[OrderSet]
ADD CONSTRAINT [FK_OrderGroupOrder]
    FOREIGN KEY ([OrderGroupId])
    REFERENCES [dbo].[OrderGroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderGroupOrder'
CREATE INDEX [IX_FK_OrderGroupOrder]
ON [dbo].[OrderSet]
    ([OrderGroupId]);
GO

-- Creating foreign key on [OrderId] in table 'OrderOperationSet'
ALTER TABLE [dbo].[OrderOperationSet]
ADD CONSTRAINT [FK_OrderOrderWork]
    FOREIGN KEY ([OrderId])
    REFERENCES [dbo].[OrderSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderOrderWork'
CREATE INDEX [IX_FK_OrderOrderWork]
ON [dbo].[OrderOperationSet]
    ([OrderId]);
GO

-- Creating foreign key on [OperationId] in table 'OrderOperationSet'
ALTER TABLE [dbo].[OrderOperationSet]
ADD CONSTRAINT [FK_OperationOrderOperation]
    FOREIGN KEY ([OperationId])
    REFERENCES [dbo].[OperationSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OperationOrderOperation'
CREATE INDEX [IX_FK_OperationOrderOperation]
ON [dbo].[OrderOperationSet]
    ([OperationId]);
GO

-- Creating foreign key on [OrderId] in table 'LimitCardMaterialSet'
ALTER TABLE [dbo].[LimitCardMaterialSet]
ADD CONSTRAINT [FK_OrderLimitCard]
    FOREIGN KEY ([OrderId])
    REFERENCES [dbo].[OrderSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderLimitCard'
CREATE INDEX [IX_FK_OrderLimitCard]
ON [dbo].[LimitCardMaterialSet]
    ([OrderId]);
GO

-- Creating foreign key on [MaterialId] in table 'LimitCardMaterialSet'
ALTER TABLE [dbo].[LimitCardMaterialSet]
ADD CONSTRAINT [FK_MaterialLimitCard]
    FOREIGN KEY ([MaterialId])
    REFERENCES [dbo].[MaterialSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MaterialLimitCard'
CREATE INDEX [IX_FK_MaterialLimitCard]
ON [dbo].[LimitCardMaterialSet]
    ([MaterialId]);
GO

-- Creating foreign key on [OrderId] in table 'LimitCardOperationSet'
ALTER TABLE [dbo].[LimitCardOperationSet]
ADD CONSTRAINT [FK_OrderLimitCardWork]
    FOREIGN KEY ([OrderId])
    REFERENCES [dbo].[OrderSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderLimitCardWork'
CREATE INDEX [IX_FK_OrderLimitCardWork]
ON [dbo].[LimitCardOperationSet]
    ([OrderId]);
GO

-- Creating foreign key on [OperationId] in table 'LimitCardOperationSet'
ALTER TABLE [dbo].[LimitCardOperationSet]
ADD CONSTRAINT [FK_OperationLimitCardOperation]
    FOREIGN KEY ([OperationId])
    REFERENCES [dbo].[OperationSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OperationLimitCardOperation'
CREATE INDEX [IX_FK_OperationLimitCardOperation]
ON [dbo].[LimitCardOperationSet]
    ([OperationId]);
GO

-- Creating foreign key on [PostId] in table 'EmployeeSet'
ALTER TABLE [dbo].[EmployeeSet]
ADD CONSTRAINT [FK_EmployeePost]
    FOREIGN KEY ([PostId])
    REFERENCES [dbo].[PostSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EmployeePost'
CREATE INDEX [IX_FK_EmployeePost]
ON [dbo].[EmployeeSet]
    ([PostId]);
GO

-- Creating foreign key on [LimitCardMaterialId] in table 'LimitCardFactMaterialSet'
ALTER TABLE [dbo].[LimitCardFactMaterialSet]
ADD CONSTRAINT [FK_LimitCardMaterialLimitCardFactMaterial]
    FOREIGN KEY ([LimitCardMaterialId])
    REFERENCES [dbo].[LimitCardMaterialSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LimitCardMaterialLimitCardFactMaterial'
CREATE INDEX [IX_FK_LimitCardMaterialLimitCardFactMaterial]
ON [dbo].[LimitCardFactMaterialSet]
    ([LimitCardMaterialId]);
GO

-- Creating foreign key on [MaterialId] in table 'LimitCardFactMaterialSet'
ALTER TABLE [dbo].[LimitCardFactMaterialSet]
ADD CONSTRAINT [FK_MaterialLimitCardFactMaterial]
    FOREIGN KEY ([MaterialId])
    REFERENCES [dbo].[MaterialSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MaterialLimitCardFactMaterial'
CREATE INDEX [IX_FK_MaterialLimitCardFactMaterial]
ON [dbo].[LimitCardFactMaterialSet]
    ([MaterialId]);
GO

-- Creating foreign key on [EmployeeId] in table 'OrderOperationSet'
ALTER TABLE [dbo].[OrderOperationSet]
ADD CONSTRAINT [FK_OrderOperationEmployee]
    FOREIGN KEY ([EmployeeId])
    REFERENCES [dbo].[EmployeeSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderOperationEmployee'
CREATE INDEX [IX_FK_OrderOperationEmployee]
ON [dbo].[OrderOperationSet]
    ([EmployeeId]);
GO

-- Creating foreign key on [OrderId] in table 'MetalFileSet'
ALTER TABLE [dbo].[MetalFileSet]
ADD CONSTRAINT [FK_MetalFileOrder]
    FOREIGN KEY ([OrderId])
    REFERENCES [dbo].[OrderSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MetalFileOrder'
CREATE INDEX [IX_FK_MetalFileOrder]
ON [dbo].[MetalFileSet]
    ([OrderId]);
GO

-- Creating foreign key on [UserGroupId] in table 'EmployeeSet'
ALTER TABLE [dbo].[EmployeeSet]
ADD CONSTRAINT [FK_EmployeeUserGroup]
    FOREIGN KEY ([UserGroupId])
    REFERENCES [dbo].[UserGroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EmployeeUserGroup'
CREATE INDEX [IX_FK_EmployeeUserGroup]
ON [dbo].[EmployeeSet]
    ([UserGroupId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------