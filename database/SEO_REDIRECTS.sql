START TRANSACTION;

ALTER TABLE `PracticeAreas` ADD `Keywords` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `PracticeAreas` ADD `MetaDescription` varchar(320) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `PracticeAreas` ADD `MetaTitle` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `PracticeAreas` ADD `OpenGraphDescription` varchar(320) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `PracticeAreas` ADD `OpenGraphMediaId` bigint NULL;

ALTER TABLE `PracticeAreas` ADD `OpenGraphTitle` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

CREATE TABLE `RedirectRules` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `SourcePath` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `DestinationPath` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NULL,
    `IsActive` tinyint(1) NOT NULL DEFAULT FALSE,
    `IsDeleted` tinyint(1) NOT NULL DEFAULT FALSE,
    CONSTRAINT `PK_RedirectRules` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE INDEX `IX_PracticeAreas_OpenGraphMediaId` ON `PracticeAreas` (`OpenGraphMediaId`);

CREATE UNIQUE INDEX `IX_RedirectRules_SourcePath` ON `RedirectRules` (`SourcePath`);

ALTER TABLE `PracticeAreas` ADD CONSTRAINT `FK_PracticeAreas_MediaAssets_OpenGraphMediaId` FOREIGN KEY (`OpenGraphMediaId`) REFERENCES `MediaAssets` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261009221754_SeoRedirects', '8.0.29');

COMMIT;
