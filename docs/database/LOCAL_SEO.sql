START TRANSACTION;

CREATE TABLE `Locations` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Slug` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Kind` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `ParentId` bigint NULL,
    `Summary` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Body` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `LocalDetails` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `IsReviewed` tinyint(1) NOT NULL,
    `FactsVerified` tinyint(1) NOT NULL,
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
    CONSTRAINT `PK_Locations` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Locations_Locations_ParentId` FOREIGN KEY (`ParentId`) REFERENCES `Locations` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Locations_MediaAssets_OpenGraphMediaId` FOREIGN KEY (`OpenGraphMediaId`) REFERENCES `MediaAssets` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `LocationArticles` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `LocationId` bigint NOT NULL,
    `ArticleId` bigint NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NULL,
    `IsActive` tinyint(1) NOT NULL DEFAULT FALSE,
    `IsDeleted` tinyint(1) NOT NULL DEFAULT FALSE,
    CONSTRAINT `PK_LocationArticles` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_LocationArticles_Articles_ArticleId` FOREIGN KEY (`ArticleId`) REFERENCES `Articles` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_LocationArticles_Locations_LocationId` FOREIGN KEY (`LocationId`) REFERENCES `Locations` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `LocationPracticeAreas` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `LocationId` bigint NOT NULL,
    `PracticeAreaId` bigint NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NULL,
    `IsActive` tinyint(1) NOT NULL DEFAULT FALSE,
    `IsDeleted` tinyint(1) NOT NULL DEFAULT FALSE,
    CONSTRAINT `PK_LocationPracticeAreas` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_LocationPracticeAreas_Locations_LocationId` FOREIGN KEY (`LocationId`) REFERENCES `Locations` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_LocationPracticeAreas_PracticeAreas_PracticeAreaId` FOREIGN KEY (`PracticeAreaId`) REFERENCES `PracticeAreas` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE INDEX `IX_LocationArticles_ArticleId` ON `LocationArticles` (`ArticleId`);

CREATE UNIQUE INDEX `IX_LocationArticles_LocationId_ArticleId` ON `LocationArticles` (`LocationId`, `ArticleId`);

CREATE UNIQUE INDEX `IX_LocationPracticeAreas_LocationId_PracticeAreaId` ON `LocationPracticeAreas` (`LocationId`, `PracticeAreaId`);

CREATE INDEX `IX_LocationPracticeAreas_PracticeAreaId` ON `LocationPracticeAreas` (`PracticeAreaId`);

CREATE INDEX `IX_Locations_IsActive_IsReviewed_FactsVerified` ON `Locations` (`IsActive`, `IsReviewed`, `FactsVerified`);

CREATE INDEX `IX_Locations_OpenGraphMediaId` ON `Locations` (`OpenGraphMediaId`);

CREATE INDEX `IX_Locations_ParentId` ON `Locations` (`ParentId`);

CREATE UNIQUE INDEX `IX_Locations_Slug` ON `Locations` (`Slug`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261009224849_LocalSeo', '8.0.29');

COMMIT;
