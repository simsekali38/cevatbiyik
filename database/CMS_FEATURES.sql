START TRANSACTION;

ALTER TABLE `Articles` ADD `CategoryId` bigint NULL;

ALTER TABLE `Articles` ADD `CoverMediaId` bigint NULL;

ALTER TABLE `Articles` ADD `Keywords` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `Articles` ADD `MetaDescription` varchar(320) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `Articles` ADD `MetaTitle` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `Articles` ADD `OpenGraphDescription` varchar(320) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `Articles` ADD `OpenGraphMediaId` bigint NULL;

ALTER TABLE `Articles` ADD `OpenGraphTitle` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

CREATE TABLE `ArticleCategories` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Slug` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NULL,
    `IsActive` tinyint(1) NOT NULL DEFAULT FALSE,
    `IsDeleted` tinyint(1) NOT NULL DEFAULT FALSE,
    CONSTRAINT `PK_ArticleCategories` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `ArticleTags` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Slug` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NULL,
    `IsActive` tinyint(1) NOT NULL DEFAULT FALSE,
    `IsDeleted` tinyint(1) NOT NULL DEFAULT FALSE,
    CONSTRAINT `PK_ArticleTags` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `MediaAssets` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `StoragePath` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `AltText` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `ContentType` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Width` int NOT NULL,
    `Height` int NOT NULL,
    `Bytes` bigint NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NULL,
    `IsActive` tinyint(1) NOT NULL DEFAULT FALSE,
    `IsDeleted` tinyint(1) NOT NULL DEFAULT FALSE,
    CONSTRAINT `PK_MediaAssets` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `ArticleTagLinks` (
    `ArticleId` bigint NOT NULL,
    `TagId` bigint NOT NULL,
    CONSTRAINT `PK_ArticleTagLinks` PRIMARY KEY (`ArticleId`, `TagId`),
    CONSTRAINT `FK_ArticleTagLinks_ArticleTags_TagId` FOREIGN KEY (`TagId`) REFERENCES `ArticleTags` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ArticleTagLinks_Articles_ArticleId` FOREIGN KEY (`ArticleId`) REFERENCES `Articles` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `PageSeo` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Path` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `MetaTitle` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `MetaDescription` varchar(320) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `Keywords` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `OpenGraphTitle` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `OpenGraphDescription` varchar(320) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `OpenGraphMediaId` bigint NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NULL,
    `IsActive` tinyint(1) NOT NULL DEFAULT FALSE,
    `IsDeleted` tinyint(1) NOT NULL DEFAULT FALSE,
    CONSTRAINT `PK_PageSeo` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_PageSeo_MediaAssets_OpenGraphMediaId` FOREIGN KEY (`OpenGraphMediaId`) REFERENCES `MediaAssets` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE INDEX `IX_Articles_CategoryId` ON `Articles` (`CategoryId`);

CREATE INDEX `IX_Articles_CoverMediaId` ON `Articles` (`CoverMediaId`);

CREATE INDEX `IX_Articles_OpenGraphMediaId` ON `Articles` (`OpenGraphMediaId`);

CREATE UNIQUE INDEX `IX_ArticleCategories_Slug` ON `ArticleCategories` (`Slug`);

CREATE INDEX `IX_ArticleTagLinks_TagId` ON `ArticleTagLinks` (`TagId`);

CREATE UNIQUE INDEX `IX_ArticleTags_Slug` ON `ArticleTags` (`Slug`);

CREATE UNIQUE INDEX `IX_MediaAssets_StoragePath` ON `MediaAssets` (`StoragePath`);

CREATE INDEX `IX_PageSeo_OpenGraphMediaId` ON `PageSeo` (`OpenGraphMediaId`);

CREATE UNIQUE INDEX `IX_PageSeo_Path` ON `PageSeo` (`Path`);

ALTER TABLE `Articles` ADD CONSTRAINT `FK_Articles_ArticleCategories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `ArticleCategories` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `Articles` ADD CONSTRAINT `FK_Articles_MediaAssets_CoverMediaId` FOREIGN KEY (`CoverMediaId`) REFERENCES `MediaAssets` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `Articles` ADD CONSTRAINT `FK_Articles_MediaAssets_OpenGraphMediaId` FOREIGN KEY (`OpenGraphMediaId`) REFERENCES `MediaAssets` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261009202119_CmsFeatures', '8.0.29');

COMMIT;
