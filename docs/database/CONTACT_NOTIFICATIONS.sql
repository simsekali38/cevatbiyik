START TRANSACTION;

ALTER TABLE `ContactMessages` ADD `NotificationAttempts` int NOT NULL DEFAULT 0;

ALTER TABLE `ContactMessages` ADD `NotificationLeaseId` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `ContactMessages` ADD `NotificationLeaseUntil` datetime(6) NULL;

ALTER TABLE `ContactMessages` ADD `NotificationNextAttemptAt` datetime(6) NULL;

ALTER TABLE `ContactMessages` ADD `NotificationPending` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `ContactMessages` ADD `NotificationSentAt` datetime(6) NULL;

CREATE INDEX `IX_ContactMessages_IsDeleted_NotificationPending_NotificationNe~` ON `ContactMessages` (`IsDeleted`, `NotificationPending`, `NotificationNextAttemptAt`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261010200603_ContactNotifications', '8.0.29');

COMMIT;
