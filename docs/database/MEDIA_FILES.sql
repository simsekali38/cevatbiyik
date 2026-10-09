START TRANSACTION;

CREATE TABLE `PrivateDocuments` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Title` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `StoragePath` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Sha256` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Bytes` bigint NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NULL,
    `IsActive` tinyint(1) NOT NULL DEFAULT FALSE,
    `IsDeleted` tinyint(1) NOT NULL DEFAULT FALSE,
    CONSTRAINT `PK_PrivateDocuments` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE UNIQUE INDEX `IX_PrivateDocuments_StoragePath` ON `PrivateDocuments` (`StoragePath`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261009210341_MediaFiles', '8.0.29');

COMMIT;

