-- COMPLETE DEMO DATABASE: cevodbtest ONLY. Never production.
-- Fills all 25 application/Identity tables; existing EF history stays untouched.
-- Published/draft/scheduled CMS states and synthetic review flags are test simulations.
-- Companion media/PDF files: prod-sample-assets.zip (SQL alone cannot write files).
-- Demo Identity users are inactive, permanently locked and have no disclosed password.
-- NotificationPending=0 everywhere: this seed must never send fake messages.
-- Data writes INSERT-only. Routine DDL outside the data transaction.
-- Requires CREATE ROUTINE, ALTER ROUTINE, EXECUTE, SELECT, INSERT.
-- Run whole file, DELIMITER-aware client, stop on first error; never --force.
-- A conflict aborts without overwriting any existing row; no migrations are fabricated.
USE `cevodbtest`;
SET NAMES utf8mb4 COLLATE utf8mb4_unicode_ci;
DELIMITER $$
CREATE PROCEDURE `cevodbtest`.`seed_prod_sample_v2`()
SQL SECURITY INVOKER
BEGIN
    DECLARE v_old_mode TEXT;
    DECLARE v_now DATETIME(6);
    DECLARE x_2_0 BIGINT;
    DECLARE x_2_1 BIGINT;
    DECLARE x_2_2 BIGINT;
    DECLARE x_2_3 BIGINT;
    DECLARE x_2_4 BIGINT;
    DECLARE x_2_5 BIGINT;
    DECLARE x_2_6 BIGINT;
    DECLARE x_2_7 BIGINT;
    DECLARE x_14_0 BIGINT;
    DECLARE x_14_1 BIGINT;
    DECLARE x_14_2 BIGINT;
    DECLARE x_0_0 BIGINT;
    DECLARE x_3_0 BIGINT;
    DECLARE x_1_0 BIGINT;
    DECLARE x_1_1 BIGINT;
    DECLARE x_0_1 BIGINT;
    DECLARE x_3_1 BIGINT;
    DECLARE x_1_2 BIGINT;
    DECLARE x_1_3 BIGINT;
    DECLARE x_0_2 BIGINT;
    DECLARE x_3_2 BIGINT;
    DECLARE x_1_4 BIGINT;
    DECLARE x_1_5 BIGINT;
    DECLARE x_0_3 BIGINT;
    DECLARE x_3_3 BIGINT;
    DECLARE x_1_6 BIGINT;
    DECLARE x_1_7 BIGINT;
    DECLARE x_0_4 BIGINT;
    DECLARE x_3_4 BIGINT;
    DECLARE x_1_8 BIGINT;
    DECLARE x_1_9 BIGINT;
    DECLARE x_0_5 BIGINT;
    DECLARE x_3_5 BIGINT;
    DECLARE x_1_10 BIGINT;
    DECLARE x_1_11 BIGINT;
    DECLARE x_4_0 BIGINT;
    DECLARE x_4_1 BIGINT;
    DECLARE x_4_2 BIGINT;
    DECLARE x_4_3 BIGINT;
    DECLARE x_4_4 BIGINT;
    DECLARE x_4_5 BIGINT;
    DECLARE x_4_6 BIGINT;
    DECLARE x_4_7 BIGINT;
    DECLARE x_4_8 BIGINT;
    DECLARE x_4_9 BIGINT;
    DECLARE x_4_10 BIGINT;
    DECLARE x_4_11 BIGINT;
    DECLARE x_4_12 BIGINT;
    DECLARE x_4_13 BIGINT;
    DECLARE x_4_14 BIGINT;
    DECLARE x_4_15 BIGINT;
    DECLARE x_4_16 BIGINT;
    DECLARE x_4_17 BIGINT;
    DECLARE x_4_18 BIGINT;
    DECLARE x_4_19 BIGINT;
    DECLARE x_4_20 BIGINT;
    DECLARE x_4_21 BIGINT;
    DECLARE x_4_22 BIGINT;
    DECLARE x_4_23 BIGINT;
    DECLARE x_6_0 BIGINT;
    DECLARE x_6_1 BIGINT;
    DECLARE x_6_2 BIGINT;
    DECLARE x_6_3 BIGINT;
    DECLARE x_6_4 BIGINT;
    DECLARE x_6_5 BIGINT;
    DECLARE x_6_6 BIGINT;
    DECLARE x_6_7 BIGINT;
    DECLARE x_6_8 BIGINT;
    DECLARE x_6_9 BIGINT;
    DECLARE x_6_10 BIGINT;
    DECLARE x_6_11 BIGINT;
    DECLARE x_7_0 BIGINT;
    DECLARE x_7_1 BIGINT;
    DECLARE x_8_0 BIGINT;
    DECLARE x_8_1 BIGINT;
    DECLARE x_8_2 BIGINT;
    DECLARE x_8_3 BIGINT;
    DECLARE x_9_0 BIGINT;
    DECLARE x_9_1 BIGINT;
    DECLARE x_9_2 BIGINT;
    DECLARE x_9_3 BIGINT;
    DECLARE x_9_4 BIGINT;
    DECLARE x_9_5 BIGINT;
    DECLARE x_9_6 BIGINT;
    DECLARE x_9_7 BIGINT;
    DECLARE x_9_8 BIGINT;
    DECLARE x_9_9 BIGINT;
    DECLARE x_9_10 BIGINT;
    DECLARE x_9_11 BIGINT;
    DECLARE x_10_0 BIGINT;
    DECLARE x_10_1 BIGINT;
    DECLARE x_10_2 BIGINT;
    DECLARE x_10_3 BIGINT;
    DECLARE x_10_4 BIGINT;
    DECLARE x_10_5 BIGINT;
    DECLARE x_10_6 BIGINT;
    DECLARE x_10_7 BIGINT;
    DECLARE x_10_8 BIGINT;
    DECLARE x_10_9 BIGINT;
    DECLARE x_10_10 BIGINT;
    DECLARE x_10_11 BIGINT;
    DECLARE x_11_0 BIGINT;
    DECLARE x_11_1 BIGINT;
    DECLARE x_11_2 BIGINT;
    DECLARE x_11_3 BIGINT;
    DECLARE x_11_4 BIGINT;
    DECLARE x_11_5 BIGINT;
    DECLARE x_12_0 BIGINT;
    DECLARE x_12_1 BIGINT;
    DECLARE x_12_2 BIGINT;
    DECLARE x_11_6 BIGINT;
    DECLARE x_11_7 BIGINT;
    DECLARE x_12_3 BIGINT;
    DECLARE x_12_4 BIGINT;
    DECLARE x_12_5 BIGINT;
    DECLARE x_11_8 BIGINT;
    DECLARE x_11_9 BIGINT;
    DECLARE x_12_6 BIGINT;
    DECLARE x_12_7 BIGINT;
    DECLARE x_12_8 BIGINT;
    DECLARE x_11_10 BIGINT;
    DECLARE x_11_11 BIGINT;
    DECLARE x_12_9 BIGINT;
    DECLARE x_12_10 BIGINT;
    DECLARE x_12_11 BIGINT;
    DECLARE x_11_12 BIGINT;
    DECLARE x_11_13 BIGINT;
    DECLARE x_12_12 BIGINT;
    DECLARE x_12_13 BIGINT;
    DECLARE x_12_14 BIGINT;
    DECLARE x_11_14 BIGINT;
    DECLARE x_11_15 BIGINT;
    DECLARE x_12_15 BIGINT;
    DECLARE x_12_16 BIGINT;
    DECLARE x_12_17 BIGINT;
    DECLARE x_11_16 BIGINT;
    DECLARE x_11_17 BIGINT;
    DECLARE x_12_18 BIGINT;
    DECLARE x_12_19 BIGINT;
    DECLARE x_12_20 BIGINT;
    DECLARE x_11_18 BIGINT;
    DECLARE x_11_19 BIGINT;
    DECLARE x_12_21 BIGINT;
    DECLARE x_12_22 BIGINT;
    DECLARE x_12_23 BIGINT;
    DECLARE x_11_20 BIGINT;
    DECLARE x_11_21 BIGINT;
    DECLARE x_12_24 BIGINT;
    DECLARE x_12_25 BIGINT;
    DECLARE x_12_26 BIGINT;
    DECLARE x_11_22 BIGINT;
    DECLARE x_11_23 BIGINT;
    DECLARE x_12_27 BIGINT;
    DECLARE x_12_28 BIGINT;
    DECLARE x_12_29 BIGINT;
    DECLARE x_11_24 BIGINT;
    DECLARE x_11_25 BIGINT;
    DECLARE x_12_30 BIGINT;
    DECLARE x_12_31 BIGINT;
    DECLARE x_12_32 BIGINT;
    DECLARE x_11_26 BIGINT;
    DECLARE x_11_27 BIGINT;
    DECLARE x_12_33 BIGINT;
    DECLARE x_12_34 BIGINT;
    DECLARE x_12_35 BIGINT;
    DECLARE x_13_0 BIGINT;
    DECLARE x_13_1 BIGINT;
    DECLARE x_13_2 BIGINT;
    DECLARE x_13_3 BIGINT;
    DECLARE x_13_4 BIGINT;
    DECLARE x_13_5 BIGINT;
    DECLARE x_13_6 BIGINT;
    DECLARE x_13_7 BIGINT;
    DECLARE x_13_8 BIGINT;
    DECLARE x_13_9 BIGINT;
    DECLARE x_13_10 BIGINT;
    DECLARE x_13_11 BIGINT;
    DECLARE x_15_0 BIGINT;
    DECLARE x_15_1 BIGINT;
    DECLARE x_15_2 BIGINT;
    DECLARE x_15_3 BIGINT;
    DECLARE x_15_4 BIGINT;
    DECLARE x_18_0 VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
    DECLARE x_20_0 BIGINT;
    DECLARE x_19_0 VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
    DECLARE x_21_0 BIGINT;
    DECLARE x_21_1 BIGINT;
    DECLARE x_18_1 VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
    DECLARE x_20_1 BIGINT;
    DECLARE x_19_1 VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
    DECLARE x_21_2 BIGINT;
    DECLARE x_21_3 BIGINT;
    DECLARE x_16_0 BIGINT;
    DECLARE x_16_1 BIGINT;
    DECLARE x_16_2 BIGINT;
    DECLARE x_16_3 BIGINT;
    DECLARE x_16_4 BIGINT;
    DECLARE x_16_5 BIGINT;
    DECLARE x_16_6 BIGINT;
    DECLARE x_16_7 BIGINT;
    DECLARE x_17_0 BIGINT;
    DECLARE x_17_1 BIGINT;
    DECLARE x_17_2 BIGINT;
    DECLARE x_17_3 BIGINT;
    DECLARE x_17_4 BIGINT;
    DECLARE x_17_5 BIGINT;
    DECLARE x_17_6 BIGINT;
    DECLARE x_17_7 BIGINT;
    DECLARE x_17_8 BIGINT;
    DECLARE x_17_9 BIGINT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET FOREIGN_KEY_CHECKS = 1;
        SET SESSION sql_mode = v_old_mode;
        RESIGNAL;
    END;
    SET v_old_mode = @@SESSION.sql_mode;
    SET SESSION sql_mode = CONCAT_WS(',', NULLIF(v_old_mode, ''), 'STRICT_TRANS_TABLES');
    SET v_now = UTC_TIMESTAMP(6);
    IF DATABASE() <> 'cevodbtest' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Only cevodbtest is permitted';
    END IF;
    IF (SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA = 'cevodbtest' AND ENGINE = 'InnoDB'
        AND LOWER(TABLE_NAME) IN ('articlecategories', 'articletags', 'mediaassets', 'practiceareas', 'articles', 'articletaglinks', 'faqitems', 'lawyerprofiles', 'pagecontents', 'pageseo', 'locations', 'locationpracticeareas', 'locationarticles', 'contactmessages', 'privatedocuments', 'redirectrules', 'auditlogs', 'sitesettings', 'aspnetroles', 'aspnetusers', 'aspnetroleclaims', 'aspnetuserclaims', 'aspnetuserlogins', 'aspnetuserroles', 'aspnetusertokens')) <> 25 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'All 25 transactional application tables are required';
    END IF;
    START TRANSACTION;
    IF EXISTS (SELECT 1 FROM `cevodbtest`.`SiteSettings` WHERE `Key` = 'TestData.ProdSample.v2.Manifest') THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Complete sample already exists; nothing overwritten';
    END IF;
    SET FOREIGN_KEY_CHECKS = 0;
    INSERT INTO `cevodbtest`.`MediaAssets` (`StoragePath`, `AltText`, `ContentType`, `Width`, `Height`, `Bytes`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('images/ded00000000000000000000000000000.webp', 'DEMO — Ceza Hukuku kapak görseli', 'image/webp', 1200, 630, 14508, v_now, NULL, 1, 0);

    SET x_2_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`MediaAssets` (`StoragePath`, `AltText`, `ContentType`, `Width`, `Height`, `Bytes`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('images/ded00000000000000000000000000001.webp', 'DEMO — Gayrimenkul Hukuku kapak görseli', 'image/webp', 1200, 630, 15596, v_now, NULL, 1, 0);

    SET x_2_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`MediaAssets` (`StoragePath`, `AltText`, `ContentType`, `Width`, `Height`, `Bytes`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('images/ded00000000000000000000000000002.webp', 'DEMO — İş Hukuku kapak görseli', 'image/webp', 1200, 630, 13840, v_now, NULL, 1, 0);

    SET x_2_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`MediaAssets` (`StoragePath`, `AltText`, `ContentType`, `Width`, `Height`, `Bytes`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('images/ded00000000000000000000000000003.webp', 'DEMO — Ticaret Hukuku kapak görseli', 'image/webp', 1200, 630, 14706, v_now, NULL, 1, 0);

    SET x_2_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`MediaAssets` (`StoragePath`, `AltText`, `ContentType`, `Width`, `Height`, `Bytes`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('images/ded00000000000000000000000000004.webp', 'DEMO — İcra-İflas Hukuku kapak görseli', 'image/webp', 1200, 630, 15048, v_now, NULL, 1, 0);

    SET x_2_4 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`MediaAssets` (`StoragePath`, `AltText`, `ContentType`, `Width`, `Height`, `Bytes`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('images/ded00000000000000000000000000005.webp', 'DEMO — İdare Hukuku kapak görseli', 'image/webp', 1200, 630, 14460, v_now, NULL, 1, 0);

    SET x_2_5 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`MediaAssets` (`StoragePath`, `AltText`, `ContentType`, `Width`, `Height`, `Bytes`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('images/ded00000000000000000000000000006.webp', 'DEMO — Ceza Hukuku kapak görseli', 'image/webp', 1200, 630, 15568, v_now, NULL, 0, 0);

    SET x_2_6 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`MediaAssets` (`StoragePath`, `AltText`, `ContentType`, `Width`, `Height`, `Bytes`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('images/ded00000000000000000000000000007.webp', 'DEMO — Gayrimenkul Hukuku kapak görseli', 'image/webp', 1200, 630, 15568, v_now, NULL, 0, 1);

    SET x_2_7 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PrivateDocuments` (`Title`, `StoragePath`, `Sha256`, `Bytes`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('DEMO belge kontrol listesi', 'documents/ded10000000000000000000000000000.pdf', '6A621C6CA37E35ABAA2C5308A375757CEE8881C7625D61B3F2818AAE16ABC92A', 614, v_now, NULL, 1, 0);

    SET x_14_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PrivateDocuments` (`Title`, `StoragePath`, `Sha256`, `Bytes`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('DEMO görüşme notu', 'documents/ded10000000000000000000000000001.pdf', '8FF17D9374EDA7B7B6C21305A4F5280F9E4F5B10867491C33AEF435771438BF1', 614, v_now, NULL, 1, 0);

    SET x_14_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PrivateDocuments` (`Title`, `StoragePath`, `Sha256`, `Bytes`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('DEMO silinmiş belge', 'documents/ded10000000000000000000000000002.pdf', '5A6AC46041C608589E1A0AE041D38629B7700E4DD1D13BBFD05998BE7F7BEC4B', 614, v_now, NULL, 0, 1);

    SET x_14_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleCategories` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('DEMO — Ceza Hukuku', 'demo-ceza-hukuku', v_now, NULL, 1, 0);

    SET x_0_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PracticeAreas` (`Name`, `Slug`, `ShortDescription`, `Content`, `SortOrder`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ceza Hukuku — Demo', 'demo-ceza-hukuku', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi

Olayın tarih sırası, ifade tutanakları ve bildirim kayıtları birlikte incelenir. Belgede yer alan bilgi ile kişisel yorum ayrı tutulur.

Görüşmeye hazırlık

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 10, 'Ceza Hukuku | Demo çalışma alanı', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi. Test ortamı için hazırlanmış belge ve süreç inceleme örneği.', 'Ceza Hukuku, belge hazırlığı, demo', 'Ceza Hukuku — Demo', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi', x_2_0, v_now, NULL, 1, 0);

    SET x_3_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTags` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ceza Hukuku / Belge hazırlığı', 'demo-ceza-hukuku-belge', v_now, NULL, 1, 0);

    SET x_1_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTags` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ceza Hukuku / Süreç değerlendirmesi', 'demo-ceza-hukuku-surec', v_now, NULL, 1, 0);

    SET x_1_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleCategories` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('DEMO — Gayrimenkul Hukuku', 'demo-gayrimenkul-hukuku', v_now, NULL, 1, 0);

    SET x_0_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PracticeAreas` (`Name`, `Slug`, `ShortDescription`, `Content`, `SortOrder`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Gayrimenkul Hukuku — Demo', 'demo-gayrimenkul-hukuku', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı

Tapu kayıtları, sözleşme ekleri ve ödeme belgeleri bir araya getirilir. Kullanım biçimi ve yazılı kayıtlar ayrı başlıklarla değerlendirilir.

Görüşmeye hazırlık

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 20, 'Gayrimenkul Hukuku | Demo çalışma alanı', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı. Test ortamı için hazırlanmış belge ve süreç inceleme örneği.', 'Gayrimenkul Hukuku, belge hazırlığı, demo', 'Gayrimenkul Hukuku — Demo', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı', x_2_1, v_now, NULL, 1, 0);

    SET x_3_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTags` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Gayrimenkul Hukuku / Belge hazırlığı', 'demo-gayrimenkul-hukuku-belge', v_now, NULL, 1, 0);

    SET x_1_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTags` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Gayrimenkul Hukuku / Süreç değerlendirmesi', 'demo-gayrimenkul-hukuku-surec', v_now, NULL, 1, 0);

    SET x_1_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleCategories` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('DEMO — İş Hukuku', 'demo-is-hukuku', v_now, NULL, 1, 0);

    SET x_0_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PracticeAreas` (`Name`, `Slug`, `ShortDescription`, `Content`, `SortOrder`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İş Hukuku — Demo', 'demo-is-hukuku', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi

İş sözleşmesi, bordro, görev tanımı ve yazışmalar tarih sırasına konur. Eksik kayıtlar ve belgeler arasındaki farklar not edilir.

Görüşmeye hazırlık

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 30, 'İş Hukuku | Demo çalışma alanı', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi. Test ortamı için hazırlanmış belge ve süreç inceleme örneği.', 'İş Hukuku, belge hazırlığı, demo', 'İş Hukuku — Demo', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi', x_2_2, v_now, NULL, 1, 0);

    SET x_3_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTags` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İş Hukuku / Belge hazırlığı', 'demo-is-hukuku-belge', v_now, NULL, 1, 0);

    SET x_1_4 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTags` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İş Hukuku / Süreç değerlendirmesi', 'demo-is-hukuku-surec', v_now, NULL, 1, 0);

    SET x_1_5 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleCategories` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('DEMO — Ticaret Hukuku', 'demo-ticaret-hukuku', v_now, NULL, 1, 0);

    SET x_0_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PracticeAreas` (`Name`, `Slug`, `ShortDescription`, `Content`, `SortOrder`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ticaret Hukuku — Demo', 'demo-ticaret-hukuku', 'Ticari sözleşme, sipariş ve teslim kayıtları', 'Ticari sözleşme, sipariş ve teslim kayıtları

Sipariş, sözleşme, fatura, teslim ve ödeme belgeleri arasındaki bağlantı incelenir. Temsil bilgileri belge üzerinden ayrıca doğrulanır.

Görüşmeye hazırlık

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 40, 'Ticaret Hukuku | Demo çalışma alanı', 'Ticari sözleşme, sipariş ve teslim kayıtları. Test ortamı için hazırlanmış belge ve süreç inceleme örneği.', 'Ticaret Hukuku, belge hazırlığı, demo', 'Ticaret Hukuku — Demo', 'Ticari sözleşme, sipariş ve teslim kayıtları', x_2_3, v_now, NULL, 1, 0);

    SET x_3_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTags` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ticaret Hukuku / Belge hazırlığı', 'demo-ticaret-hukuku-belge', v_now, NULL, 1, 0);

    SET x_1_6 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTags` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ticaret Hukuku / Süreç değerlendirmesi', 'demo-ticaret-hukuku-surec', v_now, NULL, 1, 0);

    SET x_1_7 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleCategories` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('DEMO — İcra-İflas Hukuku', 'demo-icra-iflas-hukuku', v_now, NULL, 1, 0);

    SET x_0_4 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PracticeAreas` (`Name`, `Slug`, `ShortDescription`, `Content`, `SortOrder`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İcra-İflas Hukuku — Demo', 'demo-icra-iflas-hukuku', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi

Takip evrakı, dayanak kayıtlar ve ödeme belgeleri birlikte saklanır. Teslim ve işlem tarihleri ayrı kaydedilir.

Görüşmeye hazırlık

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 50, 'İcra-İflas Hukuku | Demo çalışma alanı', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi. Test ortamı için hazırlanmış belge ve süreç inceleme örneği.', 'İcra-İflas Hukuku, belge hazırlığı, demo', 'İcra-İflas Hukuku — Demo', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi', x_2_4, v_now, NULL, 1, 0);

    SET x_3_4 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTags` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İcra-İflas Hukuku / Belge hazırlığı', 'demo-icra-iflas-hukuku-belge', v_now, NULL, 1, 0);

    SET x_1_8 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTags` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İcra-İflas Hukuku / Süreç değerlendirmesi', 'demo-icra-iflas-hukuku-surec', v_now, NULL, 1, 0);

    SET x_1_9 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleCategories` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('DEMO — İdare Hukuku', 'demo-idare-hukuku', v_now, NULL, 1, 0);

    SET x_0_5 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PracticeAreas` (`Name`, `Slug`, `ShortDescription`, `Content`, `SortOrder`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İdare Hukuku — Demo', 'demo-idare-hukuku', 'İdari işlem ve başvuru dosyasının düzenlenmesi', 'İdari işlem ve başvuru dosyasının düzenlenmesi

İşlem metni, ekleri, bildirim kayıtları ve önceki başvurular bir araya getirilir. Kurum adı tek başına yetkili merci hakkında sonuç oluşturmaz.

Görüşmeye hazırlık

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 60, 'İdare Hukuku | Demo çalışma alanı', 'İdari işlem ve başvuru dosyasının düzenlenmesi. Test ortamı için hazırlanmış belge ve süreç inceleme örneği.', 'İdare Hukuku, belge hazırlığı, demo', 'İdare Hukuku — Demo', 'İdari işlem ve başvuru dosyasının düzenlenmesi', x_2_5, v_now, NULL, 1, 0);

    SET x_3_5 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTags` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İdare Hukuku / Belge hazırlığı', 'demo-idare-hukuku-belge', v_now, NULL, 1, 0);

    SET x_1_10 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTags` (`Name`, `Slug`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İdare Hukuku / Süreç değerlendirmesi', 'demo-idare-hukuku-surec', v_now, NULL, 1, 0);

    SET x_1_11 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ceza Hukuku: belge hazırlığı ve ilk görüşme — Demo', 'demo-ceza-hukuku-belge-hazirligi', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'Ceza Hukuku: belge hazırlığı ve ilk görüşme

Olayın tarih sırası, ifade tutanakları ve bildirim kayıtları birlikte incelenir. Belgede yer alan bilgi ile kişisel yorum ayrı tutulur.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 2 DAY), x_3_0, x_0_0, x_2_0, 'Ceza Hukuku: belge hazırlığı ve ilk görüşme', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'Ceza Hukuku, kayıt düzeni, demo', 'Ceza Hukuku: belge hazırlığı ve ilk görüşme', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi', x_2_0, v_now, NULL, 1, 0);

    SET x_4_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_0, x_1_0);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_0, x_1_1);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Gayrimenkul Hukuku: belge hazırlığı ve ilk görüşme — Demo', 'demo-gayrimenkul-hukuku-belge-hazirligi', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'Gayrimenkul Hukuku: belge hazırlığı ve ilk görüşme

Tapu kayıtları, sözleşme ekleri ve ödeme belgeleri bir araya getirilir. Kullanım biçimi ve yazılı kayıtlar ayrı başlıklarla değerlendirilir.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 3 DAY), x_3_1, x_0_1, x_2_1, 'Gayrimenkul Hukuku: belge hazırlığı ve ilk görüşme', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'Gayrimenkul Hukuku, kayıt düzeni, demo', 'Gayrimenkul Hukuku: belge hazırlığı ve ilk görüşme', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı', x_2_1, v_now, NULL, 1, 0);

    SET x_4_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_1, x_1_2);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_1, x_1_3);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İş Hukuku: belge hazırlığı ve ilk görüşme — Demo', 'demo-is-hukuku-belge-hazirligi', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'İş Hukuku: belge hazırlığı ve ilk görüşme

İş sözleşmesi, bordro, görev tanımı ve yazışmalar tarih sırasına konur. Eksik kayıtlar ve belgeler arasındaki farklar not edilir.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 4 DAY), x_3_2, x_0_2, x_2_2, 'İş Hukuku: belge hazırlığı ve ilk görüşme', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'İş Hukuku, kayıt düzeni, demo', 'İş Hukuku: belge hazırlığı ve ilk görüşme', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi', x_2_2, v_now, NULL, 1, 0);

    SET x_4_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_2, x_1_4);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_2, x_1_5);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ticaret Hukuku: belge hazırlığı ve ilk görüşme — Demo', 'demo-ticaret-hukuku-belge-hazirligi', 'Ticari sözleşme, sipariş ve teslim kayıtları. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'Ticaret Hukuku: belge hazırlığı ve ilk görüşme

Sipariş, sözleşme, fatura, teslim ve ödeme belgeleri arasındaki bağlantı incelenir. Temsil bilgileri belge üzerinden ayrıca doğrulanır.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 5 DAY), x_3_3, x_0_3, x_2_3, 'Ticaret Hukuku: belge hazırlığı ve ilk görüşme', 'Ticari sözleşme, sipariş ve teslim kayıtları. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'Ticaret Hukuku, kayıt düzeni, demo', 'Ticaret Hukuku: belge hazırlığı ve ilk görüşme', 'Ticari sözleşme, sipariş ve teslim kayıtları', x_2_3, v_now, NULL, 1, 0);

    SET x_4_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_3, x_1_6);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_3, x_1_7);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İcra-İflas Hukuku: belge hazırlığı ve ilk görüşme — Demo', 'demo-icra-iflas-hukuku-belge-hazirligi', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'İcra-İflas Hukuku: belge hazırlığı ve ilk görüşme

Takip evrakı, dayanak kayıtlar ve ödeme belgeleri birlikte saklanır. Teslim ve işlem tarihleri ayrı kaydedilir.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 6 DAY), x_3_4, x_0_4, x_2_4, 'İcra-İflas Hukuku: belge hazırlığı ve ilk görüşme', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'İcra-İflas Hukuku, kayıt düzeni, demo', 'İcra-İflas Hukuku: belge hazırlığı ve ilk görüşme', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi', x_2_4, v_now, NULL, 1, 0);

    SET x_4_4 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_4, x_1_8);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_4, x_1_9);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İdare Hukuku: belge hazırlığı ve ilk görüşme — Demo', 'demo-idare-hukuku-belge-hazirligi', 'İdari işlem ve başvuru dosyasının düzenlenmesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'İdare Hukuku: belge hazırlığı ve ilk görüşme

İşlem metni, ekleri, bildirim kayıtları ve önceki başvurular bir araya getirilir. Kurum adı tek başına yetkili merci hakkında sonuç oluşturmaz.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 7 DAY), x_3_5, x_0_5, x_2_5, 'İdare Hukuku: belge hazırlığı ve ilk görüşme', 'İdari işlem ve başvuru dosyasının düzenlenmesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'İdare Hukuku, kayıt düzeni, demo', 'İdare Hukuku: belge hazırlığı ve ilk görüşme', 'İdari işlem ve başvuru dosyasının düzenlenmesi', x_2_5, v_now, NULL, 1, 0);

    SET x_4_5 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_5, x_1_10);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_5, x_1_11);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ceza Hukuku: kronoloji ve kayıt düzeni — Demo', 'demo-ceza-hukuku-kayit-duzeni', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'Ceza Hukuku: kronoloji ve kayıt düzeni

Olayın tarih sırası, ifade tutanakları ve bildirim kayıtları birlikte incelenir. Belgede yer alan bilgi ile kişisel yorum ayrı tutulur.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 8 DAY), x_3_0, x_0_0, x_2_0, 'Ceza Hukuku: kronoloji ve kayıt düzeni', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'Ceza Hukuku, kayıt düzeni, demo', 'Ceza Hukuku: kronoloji ve kayıt düzeni', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi', x_2_0, v_now, NULL, 1, 0);

    SET x_4_6 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_6, x_1_0);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_6, x_1_1);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Gayrimenkul Hukuku: kronoloji ve kayıt düzeni — Demo', 'demo-gayrimenkul-hukuku-kayit-duzeni', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'Gayrimenkul Hukuku: kronoloji ve kayıt düzeni

Tapu kayıtları, sözleşme ekleri ve ödeme belgeleri bir araya getirilir. Kullanım biçimi ve yazılı kayıtlar ayrı başlıklarla değerlendirilir.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 9 DAY), x_3_1, x_0_1, x_2_1, 'Gayrimenkul Hukuku: kronoloji ve kayıt düzeni', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'Gayrimenkul Hukuku, kayıt düzeni, demo', 'Gayrimenkul Hukuku: kronoloji ve kayıt düzeni', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı', x_2_1, v_now, NULL, 1, 0);

    SET x_4_7 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_7, x_1_2);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_7, x_1_3);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İş Hukuku: kronoloji ve kayıt düzeni — Demo', 'demo-is-hukuku-kayit-duzeni', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'İş Hukuku: kronoloji ve kayıt düzeni

İş sözleşmesi, bordro, görev tanımı ve yazışmalar tarih sırasına konur. Eksik kayıtlar ve belgeler arasındaki farklar not edilir.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 10 DAY), x_3_2, x_0_2, x_2_2, 'İş Hukuku: kronoloji ve kayıt düzeni', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'İş Hukuku, kayıt düzeni, demo', 'İş Hukuku: kronoloji ve kayıt düzeni', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi', x_2_2, v_now, NULL, 1, 0);

    SET x_4_8 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_8, x_1_4);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_8, x_1_5);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ticaret Hukuku: kronoloji ve kayıt düzeni — Demo', 'demo-ticaret-hukuku-kayit-duzeni', 'Ticari sözleşme, sipariş ve teslim kayıtları. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'Ticaret Hukuku: kronoloji ve kayıt düzeni

Sipariş, sözleşme, fatura, teslim ve ödeme belgeleri arasındaki bağlantı incelenir. Temsil bilgileri belge üzerinden ayrıca doğrulanır.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 11 DAY), x_3_3, x_0_3, x_2_3, 'Ticaret Hukuku: kronoloji ve kayıt düzeni', 'Ticari sözleşme, sipariş ve teslim kayıtları. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'Ticaret Hukuku, kayıt düzeni, demo', 'Ticaret Hukuku: kronoloji ve kayıt düzeni', 'Ticari sözleşme, sipariş ve teslim kayıtları', x_2_3, v_now, NULL, 1, 0);

    SET x_4_9 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_9, x_1_6);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_9, x_1_7);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İcra-İflas Hukuku: kronoloji ve kayıt düzeni — Demo', 'demo-icra-iflas-hukuku-kayit-duzeni', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'İcra-İflas Hukuku: kronoloji ve kayıt düzeni

Takip evrakı, dayanak kayıtlar ve ödeme belgeleri birlikte saklanır. Teslim ve işlem tarihleri ayrı kaydedilir.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 12 DAY), x_3_4, x_0_4, x_2_4, 'İcra-İflas Hukuku: kronoloji ve kayıt düzeni', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'İcra-İflas Hukuku, kayıt düzeni, demo', 'İcra-İflas Hukuku: kronoloji ve kayıt düzeni', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi', x_2_4, v_now, NULL, 1, 0);

    SET x_4_10 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_10, x_1_8);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_10, x_1_9);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İdare Hukuku: kronoloji ve kayıt düzeni — Demo', 'demo-idare-hukuku-kayit-duzeni', 'İdari işlem ve başvuru dosyasının düzenlenmesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'İdare Hukuku: kronoloji ve kayıt düzeni

İşlem metni, ekleri, bildirim kayıtları ve önceki başvurular bir araya getirilir. Kurum adı tek başına yetkili merci hakkında sonuç oluşturmaz.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 13 DAY), x_3_5, x_0_5, x_2_5, 'İdare Hukuku: kronoloji ve kayıt düzeni', 'İdari işlem ve başvuru dosyasının düzenlenmesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'İdare Hukuku, kayıt düzeni, demo', 'İdare Hukuku: kronoloji ve kayıt düzeni', 'İdari işlem ve başvuru dosyasının düzenlenmesi', x_2_5, v_now, NULL, 1, 0);

    SET x_4_11 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_11, x_1_10);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_11, x_1_11);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ceza Hukuku: değerlendirme soruları — Demo', 'demo-ceza-hukuku-degerlendirme', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'Ceza Hukuku: değerlendirme soruları

Olayın tarih sırası, ifade tutanakları ve bildirim kayıtları birlikte incelenir. Belgede yer alan bilgi ile kişisel yorum ayrı tutulur.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 14 DAY), x_3_0, x_0_0, x_2_0, 'Ceza Hukuku: değerlendirme soruları', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'Ceza Hukuku, kayıt düzeni, demo', 'Ceza Hukuku: değerlendirme soruları', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi', x_2_0, v_now, NULL, 1, 0);

    SET x_4_12 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_12, x_1_0);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_12, x_1_1);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Gayrimenkul Hukuku: değerlendirme soruları — Demo', 'demo-gayrimenkul-hukuku-degerlendirme', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'Gayrimenkul Hukuku: değerlendirme soruları

Tapu kayıtları, sözleşme ekleri ve ödeme belgeleri bir araya getirilir. Kullanım biçimi ve yazılı kayıtlar ayrı başlıklarla değerlendirilir.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 15 DAY), x_3_1, x_0_1, x_2_1, 'Gayrimenkul Hukuku: değerlendirme soruları', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'Gayrimenkul Hukuku, kayıt düzeni, demo', 'Gayrimenkul Hukuku: değerlendirme soruları', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı', x_2_1, v_now, NULL, 1, 0);

    SET x_4_13 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_13, x_1_2);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_13, x_1_3);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İş Hukuku: değerlendirme soruları — Demo', 'demo-is-hukuku-degerlendirme', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'İş Hukuku: değerlendirme soruları

İş sözleşmesi, bordro, görev tanımı ve yazışmalar tarih sırasına konur. Eksik kayıtlar ve belgeler arasındaki farklar not edilir.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 16 DAY), x_3_2, x_0_2, x_2_2, 'İş Hukuku: değerlendirme soruları', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'İş Hukuku, kayıt düzeni, demo', 'İş Hukuku: değerlendirme soruları', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi', x_2_2, v_now, NULL, 1, 0);

    SET x_4_14 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_14, x_1_4);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_14, x_1_5);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ticaret Hukuku: değerlendirme soruları — Demo', 'demo-ticaret-hukuku-degerlendirme', 'Ticari sözleşme, sipariş ve teslim kayıtları. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'Ticaret Hukuku: değerlendirme soruları

Sipariş, sözleşme, fatura, teslim ve ödeme belgeleri arasındaki bağlantı incelenir. Temsil bilgileri belge üzerinden ayrıca doğrulanır.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 17 DAY), x_3_3, x_0_3, x_2_3, 'Ticaret Hukuku: değerlendirme soruları', 'Ticari sözleşme, sipariş ve teslim kayıtları. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'Ticaret Hukuku, kayıt düzeni, demo', 'Ticaret Hukuku: değerlendirme soruları', 'Ticari sözleşme, sipariş ve teslim kayıtları', x_2_3, v_now, NULL, 1, 0);

    SET x_4_15 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_15, x_1_6);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_15, x_1_7);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İcra-İflas Hukuku: değerlendirme soruları — Demo', 'demo-icra-iflas-hukuku-degerlendirme', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'İcra-İflas Hukuku: değerlendirme soruları

Takip evrakı, dayanak kayıtlar ve ödeme belgeleri birlikte saklanır. Teslim ve işlem tarihleri ayrı kaydedilir.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 18 DAY), x_3_4, x_0_4, x_2_4, 'İcra-İflas Hukuku: değerlendirme soruları', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'İcra-İflas Hukuku, kayıt düzeni, demo', 'İcra-İflas Hukuku: değerlendirme soruları', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi', x_2_4, v_now, NULL, 1, 0);

    SET x_4_16 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_16, x_1_8);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_16, x_1_9);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İdare Hukuku: değerlendirme soruları — Demo', 'demo-idare-hukuku-degerlendirme', 'İdari işlem ve başvuru dosyasının düzenlenmesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'İdare Hukuku: değerlendirme soruları

İşlem metni, ekleri, bildirim kayıtları ve önceki başvurular bir araya getirilir. Kurum adı tek başına yetkili merci hakkında sonuç oluşturmaz.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_SUB(v_now, INTERVAL 19 DAY), x_3_5, x_0_5, x_2_5, 'İdare Hukuku: değerlendirme soruları', 'İdari işlem ve başvuru dosyasının düzenlenmesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'İdare Hukuku, kayıt düzeni, demo', 'İdare Hukuku: değerlendirme soruları', 'İdari işlem ve başvuru dosyasının düzenlenmesi', x_2_5, v_now, NULL, 1, 0);

    SET x_4_17 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_17, x_1_10);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_17, x_1_11);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ceza Hukuku: güncel belge kontrolü — Demo', 'demo-ceza-hukuku-belge-kontrolu', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'Ceza Hukuku: güncel belge kontrolü

Olayın tarih sırası, ifade tutanakları ve bildirim kayıtları birlikte incelenir. Belgede yer alan bilgi ile kişisel yorum ayrı tutulur.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', NULL, x_3_0, x_0_0, x_2_0, 'Ceza Hukuku: güncel belge kontrolü', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'Ceza Hukuku, kayıt düzeni, demo', 'Ceza Hukuku: güncel belge kontrolü', 'İfade, tutanak ve olay kronolojisinin düzenlenmesi', x_2_0, v_now, NULL, 0, 0);

    SET x_4_18 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_18, x_1_0);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_18, x_1_1);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Gayrimenkul Hukuku: güncel belge kontrolü — Demo', 'demo-gayrimenkul-hukuku-belge-kontrolu', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'Gayrimenkul Hukuku: güncel belge kontrolü

Tapu kayıtları, sözleşme ekleri ve ödeme belgeleri bir araya getirilir. Kullanım biçimi ve yazılı kayıtlar ayrı başlıklarla değerlendirilir.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', NULL, x_3_1, x_0_1, x_2_1, 'Gayrimenkul Hukuku: güncel belge kontrolü', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'Gayrimenkul Hukuku, kayıt düzeni, demo', 'Gayrimenkul Hukuku: güncel belge kontrolü', 'Taşınmaz, tapu ve sözleşme belgelerinin hazırlığı', x_2_1, v_now, NULL, 0, 0);

    SET x_4_19 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_19, x_1_2);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_19, x_1_3);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İş Hukuku: güncel belge kontrolü — Demo', 'demo-is-hukuku-belge-kontrolu', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'İş Hukuku: güncel belge kontrolü

İş sözleşmesi, bordro, görev tanımı ve yazışmalar tarih sırasına konur. Eksik kayıtlar ve belgeler arasındaki farklar not edilir.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', NULL, x_3_2, x_0_2, x_2_2, 'İş Hukuku: güncel belge kontrolü', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'İş Hukuku, kayıt düzeni, demo', 'İş Hukuku: güncel belge kontrolü', 'Çalışma ilişkisi ve ödeme kayıtlarının düzenlenmesi', x_2_2, v_now, NULL, 0, 0);

    SET x_4_20 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_20, x_1_4);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_20, x_1_5);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ticaret Hukuku: güncel belge kontrolü — Demo', 'demo-ticaret-hukuku-belge-kontrolu', 'Ticari sözleşme, sipariş ve teslim kayıtları. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'Ticaret Hukuku: güncel belge kontrolü

Sipariş, sözleşme, fatura, teslim ve ödeme belgeleri arasındaki bağlantı incelenir. Temsil bilgileri belge üzerinden ayrıca doğrulanır.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_ADD(v_now, INTERVAL 7 DAY), x_3_3, x_0_3, x_2_3, 'Ticaret Hukuku: güncel belge kontrolü', 'Ticari sözleşme, sipariş ve teslim kayıtları. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'Ticaret Hukuku, kayıt düzeni, demo', 'Ticaret Hukuku: güncel belge kontrolü', 'Ticari sözleşme, sipariş ve teslim kayıtları', x_2_3, v_now, NULL, 1, 0);

    SET x_4_21 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_21, x_1_6);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_21, x_1_7);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İcra-İflas Hukuku: güncel belge kontrolü — Demo', 'demo-icra-iflas-hukuku-belge-kontrolu', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'İcra-İflas Hukuku: güncel belge kontrolü

Takip evrakı, dayanak kayıtlar ve ödeme belgeleri birlikte saklanır. Teslim ve işlem tarihleri ayrı kaydedilir.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_ADD(v_now, INTERVAL 7 DAY), x_3_4, x_0_4, x_2_4, 'İcra-İflas Hukuku: güncel belge kontrolü', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'İcra-İflas Hukuku, kayıt düzeni, demo', 'İcra-İflas Hukuku: güncel belge kontrolü', 'Takip evrakı ve ödeme bilgilerinin ön incelemesi', x_2_4, v_now, NULL, 1, 0);

    SET x_4_22 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_22, x_1_8);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_22, x_1_9);

    INSERT INTO `cevodbtest`.`Articles` (`Title`, `Slug`, `Excerpt`, `Content`, `PublishedAt`, `PracticeAreaId`, `CategoryId`, `CoverMediaId`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İdare Hukuku: güncel belge kontrolü — Demo', 'demo-idare-hukuku-belge-kontrolu', 'İdari işlem ve başvuru dosyasının düzenlenmesi. Kurgusal içerik üzerinden belge düzeni ve görüşme hazırlığı.', 'İdare Hukuku: güncel belge kontrolü

İşlem metni, ekleri, bildirim kayıtları ve önceki başvurular bir araya getirilir. Kurum adı tek başına yetkili merci hakkında sonuç oluşturmaz.

Hazırlık adımları

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

Kontrol soruları

Hangi kayıt eksik? Tarih bilgisi hangi belgeye dayanıyor? Açıklama gerektiren noktalar neler? Her başlık dosya özelinde ele alınır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', DATE_ADD(v_now, INTERVAL 7 DAY), x_3_5, x_0_5, x_2_5, 'İdare Hukuku: güncel belge kontrolü', 'İdari işlem ve başvuru dosyasının düzenlenmesi. Kronoloji, kayıt kaynağı ve eksik evrak değerlendirmesine ilişkin demo içerik.', 'İdare Hukuku, kayıt düzeni, demo', 'İdare Hukuku: güncel belge kontrolü', 'İdari işlem ve başvuru dosyasının düzenlenmesi', x_2_5, v_now, NULL, 1, 0);

    SET x_4_23 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_23, x_1_10);

    INSERT INTO `cevodbtest`.`ArticleTagLinks` (`ArticleId`, `TagId`)
    VALUES (x_4_23, x_1_11);

    INSERT INTO `cevodbtest`.`FaqItems` (`Question`, `Answer`, `SortOrder`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ceza Hukuku görüşmesine nasıl hazırlanılır? (Demo)', 'Olayın tarih sırası, ifade tutanakları ve bildirim kayıtları birlikte incelenir. Belgede yer alan bilgi ile kişisel yorum ayrı tutulur.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

İlgili çalışma alanı: /uzmanlik-alanlari/demo-ceza-hukuku

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 0, 1, v_now, NULL, 1, 0);

    SET x_6_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`FaqItems` (`Question`, `Answer`, `SortOrder`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ceza Hukuku belgeleri nasıl düzenlenebilir? (Demo)', 'Olayın tarih sırası, ifade tutanakları ve bildirim kayıtları birlikte incelenir. Belgede yer alan bilgi ile kişisel yorum ayrı tutulur.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

İlgili çalışma alanı: /uzmanlik-alanlari/demo-ceza-hukuku

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 10, 1, v_now, NULL, 1, 0);

    SET x_6_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`FaqItems` (`Question`, `Answer`, `SortOrder`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Gayrimenkul Hukuku görüşmesine nasıl hazırlanılır? (Demo)', 'Tapu kayıtları, sözleşme ekleri ve ödeme belgeleri bir araya getirilir. Kullanım biçimi ve yazılı kayıtlar ayrı başlıklarla değerlendirilir.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

İlgili çalışma alanı: /uzmanlik-alanlari/demo-gayrimenkul-hukuku

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 20, 1, v_now, NULL, 1, 0);

    SET x_6_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`FaqItems` (`Question`, `Answer`, `SortOrder`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Gayrimenkul Hukuku belgeleri nasıl düzenlenebilir? (Demo)', 'Tapu kayıtları, sözleşme ekleri ve ödeme belgeleri bir araya getirilir. Kullanım biçimi ve yazılı kayıtlar ayrı başlıklarla değerlendirilir.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

İlgili çalışma alanı: /uzmanlik-alanlari/demo-gayrimenkul-hukuku

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 30, 1, v_now, NULL, 1, 0);

    SET x_6_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`FaqItems` (`Question`, `Answer`, `SortOrder`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İş Hukuku görüşmesine nasıl hazırlanılır? (Demo)', 'İş sözleşmesi, bordro, görev tanımı ve yazışmalar tarih sırasına konur. Eksik kayıtlar ve belgeler arasındaki farklar not edilir.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

İlgili çalışma alanı: /uzmanlik-alanlari/demo-is-hukuku

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 40, 1, v_now, NULL, 1, 0);

    SET x_6_4 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`FaqItems` (`Question`, `Answer`, `SortOrder`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İş Hukuku belgeleri nasıl düzenlenebilir? (Demo)', 'İş sözleşmesi, bordro, görev tanımı ve yazışmalar tarih sırasına konur. Eksik kayıtlar ve belgeler arasındaki farklar not edilir.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

İlgili çalışma alanı: /uzmanlik-alanlari/demo-is-hukuku

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 50, 1, v_now, NULL, 1, 0);

    SET x_6_5 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`FaqItems` (`Question`, `Answer`, `SortOrder`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ticaret Hukuku görüşmesine nasıl hazırlanılır? (Demo)', 'Sipariş, sözleşme, fatura, teslim ve ödeme belgeleri arasındaki bağlantı incelenir. Temsil bilgileri belge üzerinden ayrıca doğrulanır.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

İlgili çalışma alanı: /uzmanlik-alanlari/demo-ticaret-hukuku

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 60, 1, v_now, NULL, 1, 0);

    SET x_6_6 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`FaqItems` (`Question`, `Answer`, `SortOrder`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ticaret Hukuku belgeleri nasıl düzenlenebilir? (Demo)', 'Sipariş, sözleşme, fatura, teslim ve ödeme belgeleri arasındaki bağlantı incelenir. Temsil bilgileri belge üzerinden ayrıca doğrulanır.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

İlgili çalışma alanı: /uzmanlik-alanlari/demo-ticaret-hukuku

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 70, 1, v_now, NULL, 1, 0);

    SET x_6_7 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`FaqItems` (`Question`, `Answer`, `SortOrder`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İcra-İflas Hukuku görüşmesine nasıl hazırlanılır? (Demo)', 'Takip evrakı, dayanak kayıtlar ve ödeme belgeleri birlikte saklanır. Teslim ve işlem tarihleri ayrı kaydedilir.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

İlgili çalışma alanı: /uzmanlik-alanlari/demo-icra-iflas-hukuku

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 80, 1, v_now, NULL, 1, 0);

    SET x_6_8 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`FaqItems` (`Question`, `Answer`, `SortOrder`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İcra-İflas Hukuku belgeleri nasıl düzenlenebilir? (Demo)', 'Takip evrakı, dayanak kayıtlar ve ödeme belgeleri birlikte saklanır. Teslim ve işlem tarihleri ayrı kaydedilir.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

İlgili çalışma alanı: /uzmanlik-alanlari/demo-icra-iflas-hukuku

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 90, 1, v_now, NULL, 1, 0);

    SET x_6_9 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`FaqItems` (`Question`, `Answer`, `SortOrder`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İdare Hukuku görüşmesine nasıl hazırlanılır? (Demo)', 'İşlem metni, ekleri, bildirim kayıtları ve önceki başvurular bir araya getirilir. Kurum adı tek başına yetkili merci hakkında sonuç oluşturmaz.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

İlgili çalışma alanı: /uzmanlik-alanlari/demo-idare-hukuku

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 100, 1, v_now, NULL, 1, 0);

    SET x_6_10 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`FaqItems` (`Question`, `Answer`, `SortOrder`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İdare Hukuku belgeleri nasıl düzenlenebilir? (Demo)', 'İşlem metni, ekleri, bildirim kayıtları ve önceki başvurular bir araya getirilir. Kurum adı tek başına yetkili merci hakkında sonuç oluşturmaz.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

İlgili çalışma alanı: /uzmanlik-alanlari/demo-idare-hukuku

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 110, 1, v_now, NULL, 1, 0);

    SET x_6_11 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LawyerProfiles` (`Key`, `Biography`, `Vision`, `BarAssociation`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('primary', 'DEMO PROFİL — Bu kayıt, dolu bir profil ekranının denenmesi için hazırlanmıştır. Av. Cevat Bıyık hakkında eğitim, deneyim, baro sicili veya kişisel bilgi beyanı içermez.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.', 'DEMO VİZYON — Belgelerin düzenli sunulması, anlaşılır metin ve erişilebilir arayüz örneklerinin test edildiği kurgusal profil içeriğidir.', 'CONTENT_REQUIRED — gerçek baro bilgisi doğrulanmalıdır', 1, v_now, NULL, 1, 0);

    SET x_7_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LawyerProfiles` (`Key`, `Biography`, `Vision`, `BarAssociation`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('demo-draft', 'DEMO — yayın dışı profil düzenleme senaryosu.', 'DEMO — inceleme bekleyen profil senaryosu.', 'CONTENT_REQUIRED', 0, v_now, NULL, 0, 0);

    SET x_7_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageContents` (`Path`, `Title`, `Body`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/', 'Demo hukuk bürosu içerik merkezi', 'Bu test ortamında makaleler, çalışma alanları ve iletişim bileşenleri dolu veriyle incelenebilir. İçerikler gerçek danışmanlık hizmeti veya kişi bilgisi beyan etmez.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 1, v_now, NULL, 1, 0);

    SET x_8_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageContents` (`Path`, `Title`, `Body`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/kvkk', 'KVKK / Aydınlatma — Demo', 'Bu sayfa yalnızca aydınlatma ekranının görünümünü test eder. Formdaki kişiler ve iletişim kayıtları kurgusaldır. Gerçek veri sorumlusu, işleme amaçları, hukuki sebepler, aktarım ve başvuru kanalları üretim öncesinde ayrı hukuki incelemeyle belirlenmelidir.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 1, v_now, NULL, 1, 0);

    SET x_8_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageContents` (`Path`, `Title`, `Body`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/cerez-politikasi', 'Çerez Politikası — Demo', 'Çerez tercihleri ve izin arayüzü bu ortamda test edilir. Bu veri seti analitik hizmet, Google hesabı veya üçüncü taraf entegrasyonu etkinleştirmez. Gerçek çerez envanteri ve saklama süreleri yayından önce doğrulanmalıdır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 1, v_now, NULL, 1, 0);

    SET x_8_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageContents` (`Path`, `Title`, `Body`, `IsReviewed`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/yasal-uyari', 'Yasal Uyarı — Demo', 'Bu ortamda gösterilen içerikler kurgusal test verisidir. Gerçek dosya veya hukuki görüş yerine geçmez. Gerçek kişi, mahkeme kararı, başarı oranı ve mesleki sicil bilgisi kullanılmaz.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 1, v_now, NULL, 1, 0);

    SET x_8_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageSeo` (`Path`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/', 'Demo hukuk içerikleri | Ana sayfa', 'Dolu veriyle ziyaretçi, içerik ve SEO ekranlarının denendiği cevodbtest demo ortamı. Gerçek kişi veya hukuki hizmet iddiası içermez.', 'demo, test verisi, hukuk içerikleri', 'Demo içerik merkezi', 'Kurgusal verilerle arayüz ve SEO denemesi.', x_2_0, v_now, NULL, 1, 0);

    SET x_9_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageSeo` (`Path`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/hakkimda', 'Demo hukuk içerikleri | hakkimda', 'Dolu veriyle ziyaretçi, içerik ve SEO ekranlarının denendiği cevodbtest demo ortamı. Gerçek kişi veya hukuki hizmet iddiası içermez.', 'demo, test verisi, hukuk içerikleri', 'Demo içerik merkezi', 'Kurgusal verilerle arayüz ve SEO denemesi.', x_2_1, v_now, NULL, 1, 0);

    SET x_9_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageSeo` (`Path`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/makaleler', 'Demo hukuk içerikleri | makaleler', 'Dolu veriyle ziyaretçi, içerik ve SEO ekranlarının denendiği cevodbtest demo ortamı. Gerçek kişi veya hukuki hizmet iddiası içermez.', 'demo, test verisi, hukuk içerikleri', 'Demo içerik merkezi', 'Kurgusal verilerle arayüz ve SEO denemesi.', x_2_2, v_now, NULL, 1, 0);

    SET x_9_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageSeo` (`Path`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/uzmanlik-alanlari', 'Demo hukuk içerikleri | uzmanlik alanlari', 'Dolu veriyle ziyaretçi, içerik ve SEO ekranlarının denendiği cevodbtest demo ortamı. Gerçek kişi veya hukuki hizmet iddiası içermez.', 'demo, test verisi, hukuk içerikleri', 'Demo içerik merkezi', 'Kurgusal verilerle arayüz ve SEO denemesi.', x_2_3, v_now, NULL, 1, 0);

    SET x_9_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageSeo` (`Path`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/sss', 'Demo hukuk içerikleri | sss', 'Dolu veriyle ziyaretçi, içerik ve SEO ekranlarının denendiği cevodbtest demo ortamı. Gerçek kişi veya hukuki hizmet iddiası içermez.', 'demo, test verisi, hukuk içerikleri', 'Demo içerik merkezi', 'Kurgusal verilerle arayüz ve SEO denemesi.', x_2_4, v_now, NULL, 1, 0);

    SET x_9_4 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageSeo` (`Path`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/iletisim', 'Demo hukuk içerikleri | iletisim', 'Dolu veriyle ziyaretçi, içerik ve SEO ekranlarının denendiği cevodbtest demo ortamı. Gerçek kişi veya hukuki hizmet iddiası içermez.', 'demo, test verisi, hukuk içerikleri', 'Demo içerik merkezi', 'Kurgusal verilerle arayüz ve SEO denemesi.', x_2_5, v_now, NULL, 1, 0);

    SET x_9_5 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageSeo` (`Path`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/kvkk', 'Demo hukuk içerikleri | kvkk', 'Dolu veriyle ziyaretçi, içerik ve SEO ekranlarının denendiği cevodbtest demo ortamı. Gerçek kişi veya hukuki hizmet iddiası içermez.', 'demo, test verisi, hukuk içerikleri', 'Demo içerik merkezi', 'Kurgusal verilerle arayüz ve SEO denemesi.', x_2_0, v_now, NULL, 1, 0);

    SET x_9_6 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageSeo` (`Path`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/cerez-politikasi', 'Demo hukuk içerikleri | cerez politikasi', 'Dolu veriyle ziyaretçi, içerik ve SEO ekranlarının denendiği cevodbtest demo ortamı. Gerçek kişi veya hukuki hizmet iddiası içermez.', 'demo, test verisi, hukuk içerikleri', 'Demo içerik merkezi', 'Kurgusal verilerle arayüz ve SEO denemesi.', x_2_1, v_now, NULL, 1, 0);

    SET x_9_7 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageSeo` (`Path`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/yasal-uyari', 'Demo hukuk içerikleri | yasal uyari', 'Dolu veriyle ziyaretçi, içerik ve SEO ekranlarının denendiği cevodbtest demo ortamı. Gerçek kişi veya hukuki hizmet iddiası içermez.', 'demo, test verisi, hukuk içerikleri', 'Demo içerik merkezi', 'Kurgusal verilerle arayüz ve SEO denemesi.', x_2_2, v_now, NULL, 1, 0);

    SET x_9_8 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageSeo` (`Path`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/konumlar', 'Demo hukuk içerikleri | konumlar', 'Dolu veriyle ziyaretçi, içerik ve SEO ekranlarının denendiği cevodbtest demo ortamı. Gerçek kişi veya hukuki hizmet iddiası içermez.', 'demo, test verisi, hukuk içerikleri', 'Demo içerik merkezi', 'Kurgusal verilerle arayüz ve SEO denemesi.', x_2_3, v_now, NULL, 1, 0);

    SET x_9_9 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageSeo` (`Path`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/konumlar/demo-kayseri', 'Demo hukuk içerikleri | konumlar/demo kayseri', 'Dolu veriyle ziyaretçi, içerik ve SEO ekranlarının denendiği cevodbtest demo ortamı. Gerçek kişi veya hukuki hizmet iddiası içermez.', 'demo, test verisi, hukuk içerikleri', 'Demo içerik merkezi', 'Kurgusal verilerle arayüz ve SEO denemesi.', x_2_4, v_now, NULL, 1, 0);

    SET x_9_10 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`PageSeo` (`Path`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/konumlar/demo-talas', 'Demo hukuk içerikleri | konumlar/demo talas', 'Dolu veriyle ziyaretçi, içerik ve SEO ekranlarının denendiği cevodbtest demo ortamı. Gerçek kişi veya hukuki hizmet iddiası içermez.', 'demo, test verisi, hukuk içerikleri', 'Demo içerik merkezi', 'Kurgusal verilerle arayüz ve SEO denemesi.', x_2_5, v_now, NULL, 1, 0);

    SET x_9_11 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`Locations` (`Name`, `Slug`, `Kind`, `ParentId`, `Summary`, `Body`, `LocalDetails`, `IsReviewed`, `FactsVerified`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Kayseri', 'demo-kayseri', 'Province', NULL, 'Kayseri için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.', 'Kayseri için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 'Kayseri için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez. CONTENT_REQUIRED: özgün doğrulanmış yerel bilgiler.', 1, 0, 'Kayseri | Demo konum içerikleri', 'Kayseri konum hiyerarşisi ve içerik ilişkileri için kurgusal test sayfası. Gerçek yerel hizmet veya ofis iddiası içermez.', 'Kayseri, demo konum, belge hazırlığı', 'Kayseri — Demo konum', 'Konum ve içerik eşleşmeleri için test verisi.', x_2_0, v_now, NULL, 1, 0);

    SET x_10_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`Locations` (`Name`, `Slug`, `Kind`, `ParentId`, `Summary`, `Body`, `LocalDetails`, `IsReviewed`, `FactsVerified`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Melikgazi', 'demo-melikgazi', 'District', x_10_0, 'Melikgazi için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.', 'Melikgazi için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 'Melikgazi için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez. CONTENT_REQUIRED: özgün doğrulanmış yerel bilgiler.', 1, 0, 'Melikgazi | Demo konum içerikleri', 'Melikgazi konum hiyerarşisi ve içerik ilişkileri için kurgusal test sayfası. Gerçek yerel hizmet veya ofis iddiası içermez.', 'Melikgazi, demo konum, belge hazırlığı', 'Melikgazi — Demo konum', 'Konum ve içerik eşleşmeleri için test verisi.', x_2_1, v_now, NULL, 1, 0);

    SET x_10_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`Locations` (`Name`, `Slug`, `Kind`, `ParentId`, `Summary`, `Body`, `LocalDetails`, `IsReviewed`, `FactsVerified`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Kocasinan', 'demo-kocasinan', 'District', x_10_0, 'Kocasinan için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.', 'Kocasinan için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 'Kocasinan için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez. CONTENT_REQUIRED: özgün doğrulanmış yerel bilgiler.', 1, 0, 'Kocasinan | Demo konum içerikleri', 'Kocasinan konum hiyerarşisi ve içerik ilişkileri için kurgusal test sayfası. Gerçek yerel hizmet veya ofis iddiası içermez.', 'Kocasinan, demo konum, belge hazırlığı', 'Kocasinan — Demo konum', 'Konum ve içerik eşleşmeleri için test verisi.', x_2_2, v_now, NULL, 1, 0);

    SET x_10_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`Locations` (`Name`, `Slug`, `Kind`, `ParentId`, `Summary`, `Body`, `LocalDetails`, `IsReviewed`, `FactsVerified`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Talas', 'demo-talas', 'District', x_10_0, 'Talas için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.', 'Talas için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 'Talas için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez. CONTENT_REQUIRED: özgün doğrulanmış yerel bilgiler.', 1, 0, 'Talas | Demo konum içerikleri', 'Talas konum hiyerarşisi ve içerik ilişkileri için kurgusal test sayfası. Gerçek yerel hizmet veya ofis iddiası içermez.', 'Talas, demo konum, belge hazırlığı', 'Talas — Demo konum', 'Konum ve içerik eşleşmeleri için test verisi.', x_2_3, v_now, NULL, 1, 0);

    SET x_10_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`Locations` (`Name`, `Slug`, `Kind`, `ParentId`, `Summary`, `Body`, `LocalDetails`, `IsReviewed`, `FactsVerified`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Ankara', 'demo-ankara', 'Province', NULL, 'Ankara için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez. Yargıtay ve Danıştay yalnızca konu etiketidir; görev ve yetki beyanı değildir.', 'Ankara için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez. Yargıtay ve Danıştay yalnızca konu etiketidir; görev ve yetki beyanı değildir.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 'Ankara için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez. Yargıtay ve Danıştay yalnızca konu etiketidir; görev ve yetki beyanı değildir. CONTENT_REQUIRED: özgün doğrulanmış yerel bilgiler.', 1, 0, 'Ankara | Demo konum içerikleri', 'Ankara konum hiyerarşisi ve içerik ilişkileri için kurgusal test sayfası. Gerçek yerel hizmet veya ofis iddiası içermez.', 'Ankara, demo konum, belge hazırlığı', 'Ankara — Demo konum', 'Konum ve içerik eşleşmeleri için test verisi.', x_2_4, v_now, NULL, 1, 0);

    SET x_10_4 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`Locations` (`Name`, `Slug`, `Kind`, `ParentId`, `Summary`, `Body`, `LocalDetails`, `IsReviewed`, `FactsVerified`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Çankaya', 'demo-cankaya', 'District', x_10_4, 'Çankaya için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.', 'Çankaya için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 'Çankaya için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez. CONTENT_REQUIRED: özgün doğrulanmış yerel bilgiler.', 1, 0, 'Çankaya | Demo konum içerikleri', 'Çankaya konum hiyerarşisi ve içerik ilişkileri için kurgusal test sayfası. Gerçek yerel hizmet veya ofis iddiası içermez.', 'Çankaya, demo konum, belge hazırlığı', 'Çankaya — Demo konum', 'Konum ve içerik eşleşmeleri için test verisi.', x_2_5, v_now, NULL, 1, 0);

    SET x_10_5 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`Locations` (`Name`, `Slug`, `Kind`, `ParentId`, `Summary`, `Body`, `LocalDetails`, `IsReviewed`, `FactsVerified`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Yenimahalle', 'demo-yenimahalle', 'District', x_10_4, 'Yenimahalle için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.', 'Yenimahalle için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 'Yenimahalle için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez. CONTENT_REQUIRED: özgün doğrulanmış yerel bilgiler.', 1, 0, 'Yenimahalle | Demo konum içerikleri', 'Yenimahalle konum hiyerarşisi ve içerik ilişkileri için kurgusal test sayfası. Gerçek yerel hizmet veya ofis iddiası içermez.', 'Yenimahalle, demo konum, belge hazırlığı', 'Yenimahalle — Demo konum', 'Konum ve içerik eşleşmeleri için test verisi.', x_2_0, v_now, NULL, 1, 0);

    SET x_10_6 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`Locations` (`Name`, `Slug`, `Kind`, `ParentId`, `Summary`, `Body`, `LocalDetails`, `IsReviewed`, `FactsVerified`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('İstanbul', 'demo-istanbul', 'Province', NULL, 'İstanbul için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.', 'İstanbul için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 'İstanbul için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez. CONTENT_REQUIRED: özgün doğrulanmış yerel bilgiler.', 1, 0, 'İstanbul | Demo konum içerikleri', 'İstanbul konum hiyerarşisi ve içerik ilişkileri için kurgusal test sayfası. Gerçek yerel hizmet veya ofis iddiası içermez.', 'İstanbul, demo konum, belge hazırlığı', 'İstanbul — Demo konum', 'Konum ve içerik eşleşmeleri için test verisi.', x_2_1, v_now, NULL, 1, 0);

    SET x_10_7 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`Locations` (`Name`, `Slug`, `Kind`, `ParentId`, `Summary`, `Body`, `LocalDetails`, `IsReviewed`, `FactsVerified`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Şişli', 'demo-sisli', 'District', x_10_7, 'Şişli için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.', 'Şişli için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 'Şişli için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez. CONTENT_REQUIRED: özgün doğrulanmış yerel bilgiler.', 1, 0, 'Şişli | Demo konum içerikleri', 'Şişli konum hiyerarşisi ve içerik ilişkileri için kurgusal test sayfası. Gerçek yerel hizmet veya ofis iddiası içermez.', 'Şişli, demo konum, belge hazırlığı', 'Şişli — Demo konum', 'Konum ve içerik eşleşmeleri için test verisi.', x_2_2, v_now, NULL, 1, 0);

    SET x_10_8 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`Locations` (`Name`, `Slug`, `Kind`, `ParentId`, `Summary`, `Body`, `LocalDetails`, `IsReviewed`, `FactsVerified`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Çağlayan', 'demo-caglayan', 'Region', x_10_8, 'Çağlayan için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.', 'Çağlayan için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 'Çağlayan için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez. CONTENT_REQUIRED: özgün doğrulanmış yerel bilgiler.', 1, 0, 'Çağlayan | Demo konum içerikleri', 'Çağlayan konum hiyerarşisi ve içerik ilişkileri için kurgusal test sayfası. Gerçek yerel hizmet veya ofis iddiası içermez.', 'Çağlayan, demo konum, belge hazırlığı', 'Çağlayan — Demo konum', 'Konum ve içerik eşleşmeleri için test verisi.', x_2_3, v_now, NULL, 1, 0);

    SET x_10_9 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`Locations` (`Name`, `Slug`, `Kind`, `ParentId`, `Summary`, `Body`, `LocalDetails`, `IsReviewed`, `FactsVerified`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Nevşehir', 'demo-nevsehir', 'Province', NULL, 'Nevşehir için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.', 'Nevşehir için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 'Nevşehir için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez. CONTENT_REQUIRED: özgün doğrulanmış yerel bilgiler.', 1, 0, 'Nevşehir | Demo konum içerikleri', 'Nevşehir konum hiyerarşisi ve içerik ilişkileri için kurgusal test sayfası. Gerçek yerel hizmet veya ofis iddiası içermez.', 'Nevşehir, demo konum, belge hazırlığı', 'Nevşehir — Demo konum', 'Konum ve içerik eşleşmeleri için test verisi.', x_2_4, v_now, NULL, 1, 0);

    SET x_10_10 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`Locations` (`Name`, `Slug`, `Kind`, `ParentId`, `Summary`, `Body`, `LocalDetails`, `IsReviewed`, `FactsVerified`, `MetaTitle`, `MetaDescription`, `Keywords`, `OpenGraphTitle`, `OpenGraphDescription`, `OpenGraphMediaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Sivas', 'demo-sivas', 'Province', NULL, 'Sivas için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.', 'Sivas için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez.

Belgeler tarih sırasıyla düzenlenir. Kaynağı belirsiz bilgiler ayrıca işaretlenir. Görüşmede ele alınacak sorular, henüz bulunamayan evrak ve açıklama gerektiren noktalar ayrı listeler halinde hazırlanır. Kişisel veriler yalnızca gerekli kapsamda paylaşılır. Bu örnekte gerçek kimlik, iletişim veya mahkeme kararı bilgisi kullanılmamıştır.

DEMO VERİ — Bu metin cevodbtest arayüz ve veri ilişkisi denemesi için hazırlanmıştır. Gerçek dosya, kişi, avukat özgeçmişi veya hukuki görüş değildir. Süre, görev, yetki ve başvuru koşulları ancak somut dosya ve güncel mevzuat incelemesiyle değerlendirilir. Sonuç taahhüdü yoktur.', 'Sivas için kurgusal yerel içerik ekranı ve ilişki testi. Gerçek ofis, mahkeme adresi veya hizmet kapsamı bilgisi verilmez. CONTENT_REQUIRED: özgün doğrulanmış yerel bilgiler.', 1, 0, 'Sivas | Demo konum içerikleri', 'Sivas konum hiyerarşisi ve içerik ilişkileri için kurgusal test sayfası. Gerçek yerel hizmet veya ofis iddiası içermez.', 'Sivas, demo konum, belge hazırlığı', 'Sivas — Demo konum', 'Konum ve içerik eşleşmeleri için test verisi.', x_2_5, v_now, NULL, 1, 0);

    SET x_10_11 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_0, x_3_0, v_now, NULL, 1, 0);

    SET x_11_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_0, x_3_1, v_now, NULL, 1, 0);

    SET x_11_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_0, x_3_2, v_now, NULL, 1, 0);

    SET x_11_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_0, x_3_3, v_now, NULL, 1, 0);

    SET x_11_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_0, x_3_4, v_now, NULL, 1, 0);

    SET x_11_4 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_0, x_3_5, v_now, NULL, 1, 0);

    SET x_11_5 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_0, x_4_0, v_now, NULL, 1, 0);

    SET x_12_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_0, x_4_1, v_now, NULL, 1, 0);

    SET x_12_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_0, x_4_2, v_now, NULL, 1, 0);

    SET x_12_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_1, x_3_1, v_now, NULL, 1, 0);

    SET x_11_6 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_1, x_3_2, v_now, NULL, 1, 0);

    SET x_11_7 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_1, x_4_3, v_now, NULL, 1, 0);

    SET x_12_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_1, x_4_4, v_now, NULL, 1, 0);

    SET x_12_4 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_1, x_4_5, v_now, NULL, 1, 0);

    SET x_12_5 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_2, x_3_2, v_now, NULL, 1, 0);

    SET x_11_8 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_2, x_3_3, v_now, NULL, 1, 0);

    SET x_11_9 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_2, x_4_6, v_now, NULL, 1, 0);

    SET x_12_6 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_2, x_4_7, v_now, NULL, 1, 0);

    SET x_12_7 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_2, x_4_8, v_now, NULL, 1, 0);

    SET x_12_8 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_3, x_3_3, v_now, NULL, 1, 0);

    SET x_11_10 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_3, x_3_4, v_now, NULL, 1, 0);

    SET x_11_11 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_3, x_4_9, v_now, NULL, 1, 0);

    SET x_12_9 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_3, x_4_10, v_now, NULL, 1, 0);

    SET x_12_10 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_3, x_4_11, v_now, NULL, 1, 0);

    SET x_12_11 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_4, x_3_4, v_now, NULL, 1, 0);

    SET x_11_12 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_4, x_3_5, v_now, NULL, 1, 0);

    SET x_11_13 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_4, x_4_12, v_now, NULL, 1, 0);

    SET x_12_12 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_4, x_4_13, v_now, NULL, 1, 0);

    SET x_12_13 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_4, x_4_14, v_now, NULL, 1, 0);

    SET x_12_14 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_5, x_3_5, v_now, NULL, 1, 0);

    SET x_11_14 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_5, x_3_0, v_now, NULL, 1, 0);

    SET x_11_15 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_5, x_4_15, v_now, NULL, 1, 0);

    SET x_12_15 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_5, x_4_16, v_now, NULL, 1, 0);

    SET x_12_16 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_5, x_4_17, v_now, NULL, 1, 0);

    SET x_12_17 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_6, x_3_0, v_now, NULL, 1, 0);

    SET x_11_16 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_6, x_3_1, v_now, NULL, 1, 0);

    SET x_11_17 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_6, x_4_0, v_now, NULL, 1, 0);

    SET x_12_18 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_6, x_4_1, v_now, NULL, 1, 0);

    SET x_12_19 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_6, x_4_2, v_now, NULL, 1, 0);

    SET x_12_20 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_7, x_3_1, v_now, NULL, 1, 0);

    SET x_11_18 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_7, x_3_2, v_now, NULL, 1, 0);

    SET x_11_19 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_7, x_4_3, v_now, NULL, 1, 0);

    SET x_12_21 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_7, x_4_4, v_now, NULL, 1, 0);

    SET x_12_22 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_7, x_4_5, v_now, NULL, 1, 0);

    SET x_12_23 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_8, x_3_2, v_now, NULL, 1, 0);

    SET x_11_20 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_8, x_3_3, v_now, NULL, 1, 0);

    SET x_11_21 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_8, x_4_6, v_now, NULL, 1, 0);

    SET x_12_24 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_8, x_4_7, v_now, NULL, 1, 0);

    SET x_12_25 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_8, x_4_8, v_now, NULL, 1, 0);

    SET x_12_26 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_9, x_3_3, v_now, NULL, 1, 0);

    SET x_11_22 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_9, x_3_4, v_now, NULL, 1, 0);

    SET x_11_23 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_9, x_4_9, v_now, NULL, 1, 0);

    SET x_12_27 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_9, x_4_10, v_now, NULL, 1, 0);

    SET x_12_28 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_9, x_4_11, v_now, NULL, 1, 0);

    SET x_12_29 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_10, x_3_4, v_now, NULL, 1, 0);

    SET x_11_24 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_10, x_3_5, v_now, NULL, 1, 0);

    SET x_11_25 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_10, x_4_12, v_now, NULL, 1, 0);

    SET x_12_30 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_10, x_4_13, v_now, NULL, 1, 0);

    SET x_12_31 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_10, x_4_14, v_now, NULL, 1, 0);

    SET x_12_32 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_11, x_3_5, v_now, NULL, 1, 0);

    SET x_11_26 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationPracticeAreas` (`LocationId`, `PracticeAreaId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_11, x_3_0, v_now, NULL, 1, 0);

    SET x_11_27 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_11, x_4_15, v_now, NULL, 1, 0);

    SET x_12_33 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_11, x_4_16, v_now, NULL, 1, 0);

    SET x_12_34 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`LocationArticles` (`LocationId`, `ArticleId`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES (x_10_11, x_4_17, v_now, NULL, 1, 0);

    SET x_12_35 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ContactMessages` (`Name`, `Email`, `Phone`, `Subject`, `Message`, `Consent`, `IsRead`, `ProcessedAt`, `NotificationPending`, `NotificationAttempts`, `NotificationNextAttemptAt`, `NotificationSentAt`, `NotificationLeaseId`, `NotificationLeaseUntil`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Demo Ziyaretçi 01', 'demo.visitor01@example.invalid', 'DEMO — gerçek telefon yok', 'DEMO / Ceza Hukuku bilgi talebi', 'Kurgusal iletişim mesajı: yönetim panelinde okundu, okunmadı ve bildirim durumlarının denenmesi için oluşturuldu. Gerçek danışan veya dosya içeriği değildir.', 1, 0, NULL, 0, 0, NULL, NULL, NULL, NULL, DATE_SUB(v_now, INTERVAL 1 DAY), NULL, 1, 0);

    SET x_13_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ContactMessages` (`Name`, `Email`, `Phone`, `Subject`, `Message`, `Consent`, `IsRead`, `ProcessedAt`, `NotificationPending`, `NotificationAttempts`, `NotificationNextAttemptAt`, `NotificationSentAt`, `NotificationLeaseId`, `NotificationLeaseUntil`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Demo Ziyaretçi 02', 'demo.visitor02@example.invalid', 'DEMO — gerçek telefon yok', 'DEMO / Gayrimenkul Hukuku bilgi talebi', 'Kurgusal iletişim mesajı: yönetim panelinde okundu, okunmadı ve bildirim durumlarının denenmesi için oluşturuldu. Gerçek danışan veya dosya içeriği değildir.', 1, 0, NULL, 0, 0, NULL, NULL, NULL, NULL, DATE_SUB(v_now, INTERVAL 2 DAY), NULL, 1, 0);

    SET x_13_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ContactMessages` (`Name`, `Email`, `Phone`, `Subject`, `Message`, `Consent`, `IsRead`, `ProcessedAt`, `NotificationPending`, `NotificationAttempts`, `NotificationNextAttemptAt`, `NotificationSentAt`, `NotificationLeaseId`, `NotificationLeaseUntil`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Demo Ziyaretçi 03', 'demo.visitor03@example.invalid', 'DEMO — gerçek telefon yok', 'DEMO / İş Hukuku bilgi talebi', 'Kurgusal iletişim mesajı: yönetim panelinde okundu, okunmadı ve bildirim durumlarının denenmesi için oluşturuldu. Gerçek danışan veya dosya içeriği değildir.', 1, 0, NULL, 0, 0, NULL, NULL, NULL, NULL, DATE_SUB(v_now, INTERVAL 3 DAY), NULL, 1, 0);

    SET x_13_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ContactMessages` (`Name`, `Email`, `Phone`, `Subject`, `Message`, `Consent`, `IsRead`, `ProcessedAt`, `NotificationPending`, `NotificationAttempts`, `NotificationNextAttemptAt`, `NotificationSentAt`, `NotificationLeaseId`, `NotificationLeaseUntil`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Demo Ziyaretçi 04', 'demo.visitor04@example.invalid', 'DEMO — gerçek telefon yok', 'DEMO / Ticaret Hukuku bilgi talebi', 'Kurgusal iletişim mesajı: yönetim panelinde okundu, okunmadı ve bildirim durumlarının denenmesi için oluşturuldu. Gerçek danışan veya dosya içeriği değildir.', 1, 0, NULL, 0, 0, NULL, NULL, NULL, NULL, DATE_SUB(v_now, INTERVAL 4 DAY), NULL, 1, 0);

    SET x_13_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ContactMessages` (`Name`, `Email`, `Phone`, `Subject`, `Message`, `Consent`, `IsRead`, `ProcessedAt`, `NotificationPending`, `NotificationAttempts`, `NotificationNextAttemptAt`, `NotificationSentAt`, `NotificationLeaseId`, `NotificationLeaseUntil`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Demo Ziyaretçi 05', 'demo.visitor05@example.invalid', 'DEMO — gerçek telefon yok', 'DEMO / İcra-İflas Hukuku bilgi talebi', 'Kurgusal iletişim mesajı: yönetim panelinde okundu, okunmadı ve bildirim durumlarının denenmesi için oluşturuldu. Gerçek danışan veya dosya içeriği değildir.', 1, 0, NULL, 0, 0, NULL, NULL, NULL, NULL, DATE_SUB(v_now, INTERVAL 5 DAY), NULL, 1, 0);

    SET x_13_4 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ContactMessages` (`Name`, `Email`, `Phone`, `Subject`, `Message`, `Consent`, `IsRead`, `ProcessedAt`, `NotificationPending`, `NotificationAttempts`, `NotificationNextAttemptAt`, `NotificationSentAt`, `NotificationLeaseId`, `NotificationLeaseUntil`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Demo Ziyaretçi 06', 'demo.visitor06@example.invalid', 'DEMO — gerçek telefon yok', 'DEMO / İdare Hukuku bilgi talebi', 'Kurgusal iletişim mesajı: yönetim panelinde okundu, okunmadı ve bildirim durumlarının denenmesi için oluşturuldu. Gerçek danışan veya dosya içeriği değildir.', 1, 0, NULL, 0, 0, NULL, NULL, NULL, NULL, DATE_SUB(v_now, INTERVAL 6 DAY), NULL, 1, 0);

    SET x_13_5 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ContactMessages` (`Name`, `Email`, `Phone`, `Subject`, `Message`, `Consent`, `IsRead`, `ProcessedAt`, `NotificationPending`, `NotificationAttempts`, `NotificationNextAttemptAt`, `NotificationSentAt`, `NotificationLeaseId`, `NotificationLeaseUntil`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Demo Ziyaretçi 07', 'demo.visitor07@example.invalid', 'DEMO — gerçek telefon yok', 'DEMO / Ceza Hukuku bilgi talebi', 'Kurgusal iletişim mesajı: yönetim panelinde okundu, okunmadı ve bildirim durumlarının denenmesi için oluşturuldu. Gerçek danışan veya dosya içeriği değildir.', 1, 1, DATE_SUB(v_now, INTERVAL 1 DAY), 0, 1, NULL, DATE_SUB(v_now, INTERVAL 1 DAY), NULL, NULL, DATE_SUB(v_now, INTERVAL 7 DAY), NULL, 1, 0);

    SET x_13_6 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ContactMessages` (`Name`, `Email`, `Phone`, `Subject`, `Message`, `Consent`, `IsRead`, `ProcessedAt`, `NotificationPending`, `NotificationAttempts`, `NotificationNextAttemptAt`, `NotificationSentAt`, `NotificationLeaseId`, `NotificationLeaseUntil`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Demo Ziyaretçi 08', 'demo.visitor08@example.invalid', 'DEMO — gerçek telefon yok', 'DEMO / Gayrimenkul Hukuku bilgi talebi', 'Kurgusal iletişim mesajı: yönetim panelinde okundu, okunmadı ve bildirim durumlarının denenmesi için oluşturuldu. Gerçek danışan veya dosya içeriği değildir.', 1, 1, DATE_SUB(v_now, INTERVAL 1 DAY), 0, 1, NULL, DATE_SUB(v_now, INTERVAL 1 DAY), NULL, NULL, DATE_SUB(v_now, INTERVAL 8 DAY), NULL, 1, 0);

    SET x_13_7 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ContactMessages` (`Name`, `Email`, `Phone`, `Subject`, `Message`, `Consent`, `IsRead`, `ProcessedAt`, `NotificationPending`, `NotificationAttempts`, `NotificationNextAttemptAt`, `NotificationSentAt`, `NotificationLeaseId`, `NotificationLeaseUntil`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Demo Ziyaretçi 09', 'demo.visitor09@example.invalid', 'DEMO — gerçek telefon yok', 'DEMO / İş Hukuku bilgi talebi', 'Kurgusal iletişim mesajı: yönetim panelinde okundu, okunmadı ve bildirim durumlarının denenmesi için oluşturuldu. Gerçek danışan veya dosya içeriği değildir.', 1, 1, DATE_SUB(v_now, INTERVAL 1 DAY), 0, 1, NULL, DATE_SUB(v_now, INTERVAL 1 DAY), NULL, NULL, DATE_SUB(v_now, INTERVAL 9 DAY), NULL, 1, 0);

    SET x_13_8 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ContactMessages` (`Name`, `Email`, `Phone`, `Subject`, `Message`, `Consent`, `IsRead`, `ProcessedAt`, `NotificationPending`, `NotificationAttempts`, `NotificationNextAttemptAt`, `NotificationSentAt`, `NotificationLeaseId`, `NotificationLeaseUntil`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Demo Ziyaretçi 10', 'demo.visitor10@example.invalid', 'DEMO — gerçek telefon yok', 'DEMO / Ticaret Hukuku bilgi talebi', 'Kurgusal iletişim mesajı: yönetim panelinde okundu, okunmadı ve bildirim durumlarının denenmesi için oluşturuldu. Gerçek danışan veya dosya içeriği değildir.', 1, 1, DATE_SUB(v_now, INTERVAL 1 DAY), 0, 2, NULL, NULL, NULL, NULL, DATE_SUB(v_now, INTERVAL 10 DAY), NULL, 1, 0);

    SET x_13_9 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ContactMessages` (`Name`, `Email`, `Phone`, `Subject`, `Message`, `Consent`, `IsRead`, `ProcessedAt`, `NotificationPending`, `NotificationAttempts`, `NotificationNextAttemptAt`, `NotificationSentAt`, `NotificationLeaseId`, `NotificationLeaseUntil`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Demo Ziyaretçi 11', 'demo.visitor11@example.invalid', 'DEMO — gerçek telefon yok', 'DEMO / İcra-İflas Hukuku bilgi talebi', 'Kurgusal iletişim mesajı: yönetim panelinde okundu, okunmadı ve bildirim durumlarının denenmesi için oluşturuldu. Gerçek danışan veya dosya içeriği değildir.', 1, 1, DATE_SUB(v_now, INTERVAL 1 DAY), 0, 2, NULL, NULL, NULL, NULL, DATE_SUB(v_now, INTERVAL 11 DAY), NULL, 1, 0);

    SET x_13_10 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`ContactMessages` (`Name`, `Email`, `Phone`, `Subject`, `Message`, `Consent`, `IsRead`, `ProcessedAt`, `NotificationPending`, `NotificationAttempts`, `NotificationNextAttemptAt`, `NotificationSentAt`, `NotificationLeaseId`, `NotificationLeaseUntil`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Demo Ziyaretçi 12', 'demo.visitor12@example.invalid', 'DEMO — gerçek telefon yok', 'DEMO / İdare Hukuku bilgi talebi', 'Kurgusal iletişim mesajı: yönetim panelinde okundu, okunmadı ve bildirim durumlarının denenmesi için oluşturuldu. Gerçek danışan veya dosya içeriği değildir.', 1, 1, DATE_SUB(v_now, INTERVAL 1 DAY), 0, 0, NULL, NULL, NULL, NULL, DATE_SUB(v_now, INTERVAL 12 DAY), NULL, 1, 1);

    SET x_13_11 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`RedirectRules` (`SourcePath`, `DestinationPath`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/demo-eski-ceza', '/uzmanlik-alanlari/demo-ceza-hukuku', v_now, NULL, 1, 0);

    SET x_15_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`RedirectRules` (`SourcePath`, `DestinationPath`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/demo-eski-ticaret', '/uzmanlik-alanlari/demo-ticaret-hukuku', v_now, NULL, 1, 0);

    SET x_15_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`RedirectRules` (`SourcePath`, `DestinationPath`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/demo-eski-makale', '/makaleler/demo-is-hukuku-belge-hazirligi', v_now, NULL, 1, 0);

    SET x_15_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`RedirectRules` (`SourcePath`, `DestinationPath`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/demo-eski-sss', '/sss', v_now, NULL, 1, 0);

    SET x_15_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`RedirectRules` (`SourcePath`, `DestinationPath`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('/demo-kapali-kural', '/makaleler', v_now, NULL, 0, 0);

    SET x_15_4 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`AspNetRoles` (`Id`, `Name`, `NormalizedName`, `ConcurrencyStamp`)
    VALUES ('prod-sample-v2-role-0', 'Admin', 'ADMIN', 'a7facd2c-1823-45d4-8af0-8525780d6242');

    SET x_18_0 = 'prod-sample-v2-role-0';

    INSERT INTO `cevodbtest`.`AspNetRoleClaims` (`RoleId`, `ClaimType`, `ClaimValue`)
    VALUES (x_18_0, 'demo:fixture', 'test-only');

    SET x_20_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`AspNetUsers` (`Id`, `IsActive`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`, `EmailConfirmed`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`)
    VALUES ('prod-sample-v2-user-0', 0, 'demo-account-0', 'DEMO-ACCOUNT-0', 'demo.account0@example.invalid', 'DEMO.ACCOUNT0@EXAMPLE.INVALID', 1, 'AQAAAAIAAYagAAAAEAtGYFCIf2d0H5INDORwHh+NLf9LZqhPqF89exCiI20CDxzCmafKsPy8DI/doebBHw==', '7ffacb46-8610-44f2-b0b3-e4dc3bc2f591', '1d7bfd40-a268-4de3-a09b-8762fe711bb3', 'DEMO — gerçek telefon yok', 0, 0, '9999-12-31 00:00:00.000000', 1, 0);

    SET x_19_0 = 'prod-sample-v2-user-0';

    INSERT INTO `cevodbtest`.`AspNetUserRoles` (`UserId`, `RoleId`)
    VALUES (x_19_0, x_18_0);

    INSERT INTO `cevodbtest`.`AspNetUserClaims` (`UserId`, `ClaimType`, `ClaimValue`)
    VALUES (x_19_0, 'demo:dataset', 'prod-sample-v2');

    SET x_21_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`AspNetUserClaims` (`UserId`, `ClaimType`, `ClaimValue`)
    VALUES (x_19_0, 'demo:purpose', 'interface-test');

    SET x_21_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`AspNetUserLogins` (`LoginProvider`, `ProviderKey`, `ProviderDisplayName`, `UserId`)
    VALUES ('DemoFixtureProvider', 'demo-provider-key-0', 'DEMO — gerçek dış sağlayıcı değil', x_19_0);

    INSERT INTO `cevodbtest`.`AspNetUserTokens` (`UserId`, `LoginProvider`, `Name`, `Value`)
    VALUES (x_19_0, 'DemoFixtureProvider', 'FixtureState', 'DEMO_NOT_AN_AUTH_TOKEN');

    INSERT INTO `cevodbtest`.`AspNetRoles` (`Id`, `Name`, `NormalizedName`, `ConcurrencyStamp`)
    VALUES ('prod-sample-v2-role-1', 'DemoAuditor', 'DEMOAUDITOR', 'ae8a2167-17e8-4d07-a0d4-807d25cb8f86');

    SET x_18_1 = 'prod-sample-v2-role-1';

    INSERT INTO `cevodbtest`.`AspNetRoleClaims` (`RoleId`, `ClaimType`, `ClaimValue`)
    VALUES (x_18_1, 'demo:fixture', 'test-only');

    SET x_20_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`AspNetUsers` (`Id`, `IsActive`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`, `EmailConfirmed`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`)
    VALUES ('prod-sample-v2-user-1', 0, 'demo-account-1', 'DEMO-ACCOUNT-1', 'demo.account1@example.invalid', 'DEMO.ACCOUNT1@EXAMPLE.INVALID', 1, 'AQAAAAIAAYagAAAAEH9ewWE7/xXLibYwR8Hi6EUz5UY58RJXi6TWiNHx7RznCQd/njCVmhHV8DtYeyKB3A==', 'd43a8688-f554-4169-bf6e-3948aaeeb1c6', '605b6ef9-9a5d-4981-9b66-a7b1e67f98ed', 'DEMO — gerçek telefon yok', 0, 0, '9999-12-31 00:00:00.000000', 1, 0);

    SET x_19_1 = 'prod-sample-v2-user-1';

    INSERT INTO `cevodbtest`.`AspNetUserRoles` (`UserId`, `RoleId`)
    VALUES (x_19_1, x_18_1);

    INSERT INTO `cevodbtest`.`AspNetUserClaims` (`UserId`, `ClaimType`, `ClaimValue`)
    VALUES (x_19_1, 'demo:dataset', 'prod-sample-v2');

    SET x_21_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`AspNetUserClaims` (`UserId`, `ClaimType`, `ClaimValue`)
    VALUES (x_19_1, 'demo:purpose', 'interface-test');

    SET x_21_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`AspNetUserLogins` (`LoginProvider`, `ProviderKey`, `ProviderDisplayName`, `UserId`)
    VALUES ('DemoFixtureProvider', 'demo-provider-key-1', 'DEMO — gerçek dış sağlayıcı değil', x_19_1);

    INSERT INTO `cevodbtest`.`AspNetUserTokens` (`UserId`, `LoginProvider`, `Name`, `Value`)
    VALUES (x_19_1, 'DemoFixtureProvider', 'FixtureState', 'DEMO_NOT_AN_AUTH_TOKEN');

    INSERT INTO `cevodbtest`.`AuditLogs` (`CreatedAt`, `ActorId`, `Action`)
    VALUES (DATE_SUB(v_now, INTERVAL 0 HOUR), x_19_0, 'Demo.Seed');

    SET x_16_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`AuditLogs` (`CreatedAt`, `ActorId`, `Action`)
    VALUES (DATE_SUB(v_now, INTERVAL 1 HOUR), x_19_1, 'Demo.LoginFailed');

    SET x_16_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`AuditLogs` (`CreatedAt`, `ActorId`, `Action`)
    VALUES (DATE_SUB(v_now, INTERVAL 2 HOUR), x_19_0, 'Demo.ArticleCreated');

    SET x_16_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`AuditLogs` (`CreatedAt`, `ActorId`, `Action`)
    VALUES (DATE_SUB(v_now, INTERVAL 3 HOUR), x_19_1, 'Demo.ArticleUpdated');

    SET x_16_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`AuditLogs` (`CreatedAt`, `ActorId`, `Action`)
    VALUES (DATE_SUB(v_now, INTERVAL 4 HOUR), x_19_0, 'Demo.MessageRead');

    SET x_16_4 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`AuditLogs` (`CreatedAt`, `ActorId`, `Action`)
    VALUES (DATE_SUB(v_now, INTERVAL 5 HOUR), x_19_1, 'Demo.DocumentViewed');

    SET x_16_5 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`AuditLogs` (`CreatedAt`, `ActorId`, `Action`)
    VALUES (DATE_SUB(v_now, INTERVAL 6 HOUR), x_19_0, 'Demo.RedirectCreated');

    SET x_16_6 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`AuditLogs` (`CreatedAt`, `ActorId`, `Action`)
    VALUES (DATE_SUB(v_now, INTERVAL 7 HOUR), x_19_1, 'Demo.Logout');

    SET x_16_7 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`SiteSettings` (`Key`, `Value`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Site.Name', 'DEMO — Cevat Bıyık test ortamı', v_now, NULL, 1, 0);

    SET x_17_0 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`SiteSettings` (`Key`, `Value`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Site.Description', 'Kurgusal verilerle dolu makale, çalışma alanı, profil ve yönetim ekranları.', v_now, NULL, 1, 0);

    SET x_17_1 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`SiteSettings` (`Key`, `Value`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Contact.Name', 'Demo İletişim Birimi — gerçek kişi değil', v_now, NULL, 1, 0);

    SET x_17_2 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`SiteSettings` (`Key`, `Value`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Contact.Email', 'demo.contact@example.invalid', v_now, NULL, 1, 0);

    SET x_17_3 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`SiteSettings` (`Key`, `Value`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Contact.Phone', 'DEMO — aranabilir telefon yok', v_now, NULL, 1, 0);

    SET x_17_4 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`SiteSettings` (`Key`, `Value`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Contact.Address', 'DEMO Test Sokak No: 0, Kayseri — gerçek adres değildir', v_now, NULL, 1, 0);

    SET x_17_5 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`SiteSettings` (`Key`, `Value`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Contact.WorkingHours', 'DEMO: Hafta içi 09:00–18:00 — gerçek çalışma saati beyanı değildir', v_now, NULL, 1, 0);

    SET x_17_6 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`SiteSettings` (`Key`, `Value`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('Contact.WhatsApp', 'DEMO — gerçek WhatsApp hesabı yok', v_now, NULL, 1, 0);

    SET x_17_7 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`SiteSettings` (`Key`, `Value`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('TestData.ProdSample.v2.Manifest', 'Complete test-only fixture; synthetic reviewed flags; no production use', v_now, NULL, 0, 0);

    SET x_17_8 = LAST_INSERT_ID();

    INSERT INTO `cevodbtest`.`SiteSettings` (`Key`, `Value`, `CreatedAt`, `UpdatedAt`, `IsActive`, `IsDeleted`)
    VALUES ('TestData.ProdSample.v2.Ids', JSON_OBJECT('ArticleCategories', JSON_ARRAY(x_0_0, x_0_1, x_0_2, x_0_3, x_0_4, x_0_5),
        'ArticleTags', JSON_ARRAY(x_1_0, x_1_1, x_1_2, x_1_3, x_1_4, x_1_5, x_1_6, x_1_7, x_1_8, x_1_9, x_1_10, x_1_11),
        'MediaAssets', JSON_ARRAY(x_2_0, x_2_1, x_2_2, x_2_3, x_2_4, x_2_5, x_2_6, x_2_7),
        'PracticeAreas', JSON_ARRAY(x_3_0, x_3_1, x_3_2, x_3_3, x_3_4, x_3_5),
        'Articles', JSON_ARRAY(x_4_0, x_4_1, x_4_2, x_4_3, x_4_4, x_4_5, x_4_6, x_4_7, x_4_8, x_4_9, x_4_10, x_4_11, x_4_12, x_4_13, x_4_14, x_4_15, x_4_16, x_4_17, x_4_18, x_4_19, x_4_20, x_4_21, x_4_22, x_4_23),
        'FaqItems', JSON_ARRAY(x_6_0, x_6_1, x_6_2, x_6_3, x_6_4, x_6_5, x_6_6, x_6_7, x_6_8, x_6_9, x_6_10, x_6_11),
        'LawyerProfiles', JSON_ARRAY(x_7_0, x_7_1),
        'PageContents', JSON_ARRAY(x_8_0, x_8_1, x_8_2, x_8_3),
        'PageSeo', JSON_ARRAY(x_9_0, x_9_1, x_9_2, x_9_3, x_9_4, x_9_5, x_9_6, x_9_7, x_9_8, x_9_9, x_9_10, x_9_11),
        'Locations', JSON_ARRAY(x_10_0, x_10_1, x_10_2, x_10_3, x_10_4, x_10_5, x_10_6, x_10_7, x_10_8, x_10_9, x_10_10, x_10_11),
        'LocationPracticeAreas', JSON_ARRAY(x_11_0, x_11_1, x_11_2, x_11_3, x_11_4, x_11_5, x_11_6, x_11_7, x_11_8, x_11_9, x_11_10, x_11_11, x_11_12, x_11_13, x_11_14, x_11_15, x_11_16, x_11_17, x_11_18, x_11_19, x_11_20, x_11_21, x_11_22, x_11_23, x_11_24, x_11_25, x_11_26, x_11_27),
        'LocationArticles', JSON_ARRAY(x_12_0, x_12_1, x_12_2, x_12_3, x_12_4, x_12_5, x_12_6, x_12_7, x_12_8, x_12_9, x_12_10, x_12_11, x_12_12, x_12_13, x_12_14, x_12_15, x_12_16, x_12_17, x_12_18, x_12_19, x_12_20, x_12_21, x_12_22, x_12_23, x_12_24, x_12_25, x_12_26, x_12_27, x_12_28, x_12_29, x_12_30, x_12_31, x_12_32, x_12_33, x_12_34, x_12_35),
        'ContactMessages', JSON_ARRAY(x_13_0, x_13_1, x_13_2, x_13_3, x_13_4, x_13_5, x_13_6, x_13_7, x_13_8, x_13_9, x_13_10, x_13_11),
        'PrivateDocuments', JSON_ARRAY(x_14_0, x_14_1, x_14_2),
        'RedirectRules', JSON_ARRAY(x_15_0, x_15_1, x_15_2, x_15_3, x_15_4),
        'AuditLogs', JSON_ARRAY(x_16_0, x_16_1, x_16_2, x_16_3, x_16_4, x_16_5, x_16_6, x_16_7),
        'SiteSettings', JSON_ARRAY(x_17_0, x_17_1, x_17_2, x_17_3, x_17_4, x_17_5, x_17_6, x_17_7, x_17_8),
        'AspNetRoles', JSON_ARRAY(x_18_0, x_18_1),
        'AspNetUsers', JSON_ARRAY(x_19_0, x_19_1),
        'AspNetRoleClaims', JSON_ARRAY(x_20_0, x_20_1),
        'AspNetUserClaims', JSON_ARRAY(x_21_0, x_21_1, x_21_2, x_21_3)), v_now, NULL, 0, 0);

    SET x_17_9 = LAST_INSERT_ID();

    IF EXISTS (SELECT 1 FROM `cevodbtest`.`Articles` c LEFT JOIN `cevodbtest`.`PracticeAreas` p ON p.Id = c.`PracticeAreaId` WHERE c.Id IN (x_4_0, x_4_1, x_4_2, x_4_3, x_4_4, x_4_5, x_4_6, x_4_7, x_4_8, x_4_9, x_4_10, x_4_11, x_4_12, x_4_13, x_4_14, x_4_15, x_4_16, x_4_17, x_4_18, x_4_19, x_4_20, x_4_21, x_4_22, x_4_23) AND c.`PracticeAreaId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`Articles` c LEFT JOIN `cevodbtest`.`ArticleCategories` p ON p.Id = c.`CategoryId` WHERE c.Id IN (x_4_0, x_4_1, x_4_2, x_4_3, x_4_4, x_4_5, x_4_6, x_4_7, x_4_8, x_4_9, x_4_10, x_4_11, x_4_12, x_4_13, x_4_14, x_4_15, x_4_16, x_4_17, x_4_18, x_4_19, x_4_20, x_4_21, x_4_22, x_4_23) AND c.`CategoryId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`Articles` c LEFT JOIN `cevodbtest`.`MediaAssets` p ON p.Id = c.`CoverMediaId` WHERE c.Id IN (x_4_0, x_4_1, x_4_2, x_4_3, x_4_4, x_4_5, x_4_6, x_4_7, x_4_8, x_4_9, x_4_10, x_4_11, x_4_12, x_4_13, x_4_14, x_4_15, x_4_16, x_4_17, x_4_18, x_4_19, x_4_20, x_4_21, x_4_22, x_4_23) AND c.`CoverMediaId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`Articles` c LEFT JOIN `cevodbtest`.`MediaAssets` p ON p.Id = c.`OpenGraphMediaId` WHERE c.Id IN (x_4_0, x_4_1, x_4_2, x_4_3, x_4_4, x_4_5, x_4_6, x_4_7, x_4_8, x_4_9, x_4_10, x_4_11, x_4_12, x_4_13, x_4_14, x_4_15, x_4_16, x_4_17, x_4_18, x_4_19, x_4_20, x_4_21, x_4_22, x_4_23) AND c.`OpenGraphMediaId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`PracticeAreas` c LEFT JOIN `cevodbtest`.`MediaAssets` p ON p.Id = c.`OpenGraphMediaId` WHERE c.Id IN (x_3_0, x_3_1, x_3_2, x_3_3, x_3_4, x_3_5) AND c.`OpenGraphMediaId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`PageSeo` c LEFT JOIN `cevodbtest`.`MediaAssets` p ON p.Id = c.`OpenGraphMediaId` WHERE c.Id IN (x_9_0, x_9_1, x_9_2, x_9_3, x_9_4, x_9_5, x_9_6, x_9_7, x_9_8, x_9_9, x_9_10, x_9_11) AND c.`OpenGraphMediaId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`Locations` c LEFT JOIN `cevodbtest`.`Locations` p ON p.Id = c.`ParentId` WHERE c.Id IN (x_10_0, x_10_1, x_10_2, x_10_3, x_10_4, x_10_5, x_10_6, x_10_7, x_10_8, x_10_9, x_10_10, x_10_11) AND c.`ParentId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`Locations` c LEFT JOIN `cevodbtest`.`MediaAssets` p ON p.Id = c.`OpenGraphMediaId` WHERE c.Id IN (x_10_0, x_10_1, x_10_2, x_10_3, x_10_4, x_10_5, x_10_6, x_10_7, x_10_8, x_10_9, x_10_10, x_10_11) AND c.`OpenGraphMediaId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`ArticleTagLinks` c LEFT JOIN `cevodbtest`.`Articles` p ON p.Id = c.`ArticleId` WHERE c.ArticleId IN (x_4_0, x_4_1, x_4_2, x_4_3, x_4_4, x_4_5, x_4_6, x_4_7, x_4_8, x_4_9, x_4_10, x_4_11, x_4_12, x_4_13, x_4_14, x_4_15, x_4_16, x_4_17, x_4_18, x_4_19, x_4_20, x_4_21, x_4_22, x_4_23) AND c.`ArticleId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`ArticleTagLinks` c LEFT JOIN `cevodbtest`.`ArticleTags` p ON p.Id = c.`TagId` WHERE c.ArticleId IN (x_4_0, x_4_1, x_4_2, x_4_3, x_4_4, x_4_5, x_4_6, x_4_7, x_4_8, x_4_9, x_4_10, x_4_11, x_4_12, x_4_13, x_4_14, x_4_15, x_4_16, x_4_17, x_4_18, x_4_19, x_4_20, x_4_21, x_4_22, x_4_23) AND c.`TagId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`LocationPracticeAreas` c LEFT JOIN `cevodbtest`.`Locations` p ON p.Id = c.`LocationId` WHERE c.Id IN (x_11_0, x_11_1, x_11_2, x_11_3, x_11_4, x_11_5, x_11_6, x_11_7, x_11_8, x_11_9, x_11_10, x_11_11, x_11_12, x_11_13, x_11_14, x_11_15, x_11_16, x_11_17, x_11_18, x_11_19, x_11_20, x_11_21, x_11_22, x_11_23, x_11_24, x_11_25, x_11_26, x_11_27) AND c.`LocationId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`LocationPracticeAreas` c LEFT JOIN `cevodbtest`.`PracticeAreas` p ON p.Id = c.`PracticeAreaId` WHERE c.Id IN (x_11_0, x_11_1, x_11_2, x_11_3, x_11_4, x_11_5, x_11_6, x_11_7, x_11_8, x_11_9, x_11_10, x_11_11, x_11_12, x_11_13, x_11_14, x_11_15, x_11_16, x_11_17, x_11_18, x_11_19, x_11_20, x_11_21, x_11_22, x_11_23, x_11_24, x_11_25, x_11_26, x_11_27) AND c.`PracticeAreaId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`LocationArticles` c LEFT JOIN `cevodbtest`.`Locations` p ON p.Id = c.`LocationId` WHERE c.Id IN (x_12_0, x_12_1, x_12_2, x_12_3, x_12_4, x_12_5, x_12_6, x_12_7, x_12_8, x_12_9, x_12_10, x_12_11, x_12_12, x_12_13, x_12_14, x_12_15, x_12_16, x_12_17, x_12_18, x_12_19, x_12_20, x_12_21, x_12_22, x_12_23, x_12_24, x_12_25, x_12_26, x_12_27, x_12_28, x_12_29, x_12_30, x_12_31, x_12_32, x_12_33, x_12_34, x_12_35) AND c.`LocationId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`LocationArticles` c LEFT JOIN `cevodbtest`.`Articles` p ON p.Id = c.`ArticleId` WHERE c.Id IN (x_12_0, x_12_1, x_12_2, x_12_3, x_12_4, x_12_5, x_12_6, x_12_7, x_12_8, x_12_9, x_12_10, x_12_11, x_12_12, x_12_13, x_12_14, x_12_15, x_12_16, x_12_17, x_12_18, x_12_19, x_12_20, x_12_21, x_12_22, x_12_23, x_12_24, x_12_25, x_12_26, x_12_27, x_12_28, x_12_29, x_12_30, x_12_31, x_12_32, x_12_33, x_12_34, x_12_35) AND c.`ArticleId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`AspNetRoleClaims` c LEFT JOIN `cevodbtest`.`AspNetRoles` p ON p.Id = c.`RoleId` WHERE c.Id IN (x_20_0, x_20_1) AND c.`RoleId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`AspNetUserClaims` c LEFT JOIN `cevodbtest`.`AspNetUsers` p ON p.Id = c.`UserId` WHERE c.Id IN (x_21_0, x_21_1, x_21_2, x_21_3) AND c.`UserId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`AspNetUserRoles` c LEFT JOIN `cevodbtest`.`AspNetUsers` p ON p.Id = c.`UserId` WHERE c.UserId IN (x_19_0, x_19_1) AND c.`UserId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`AspNetUserRoles` c LEFT JOIN `cevodbtest`.`AspNetRoles` p ON p.Id = c.`RoleId` WHERE c.UserId IN (x_19_0, x_19_1) AND c.`RoleId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`AspNetUserLogins` c LEFT JOIN `cevodbtest`.`AspNetUsers` p ON p.Id = c.`UserId` WHERE c.UserId IN (x_19_0, x_19_1) AND c.`UserId` IS NOT NULL AND p.Id IS NULL)
        OR EXISTS (SELECT 1 FROM `cevodbtest`.`AspNetUserTokens` c LEFT JOIN `cevodbtest`.`AspNetUsers` p ON p.Id = c.`UserId` WHERE c.UserId IN (x_19_0, x_19_1) AND c.`UserId` IS NOT NULL AND p.Id IS NULL) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Inserted FK validation failed';
    END IF;
    IF (SELECT COUNT(*) FROM `cevodbtest`.`ArticleCategories`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`ArticleTags`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`MediaAssets`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`PracticeAreas`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`Articles`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`ArticleTagLinks`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`FaqItems`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`LawyerProfiles`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`PageContents`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`PageSeo`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`Locations`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`LocationPracticeAreas`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`LocationArticles`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`ContactMessages`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`PrivateDocuments`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`RedirectRules`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`AuditLogs`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`SiteSettings`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`AspNetRoles`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`AspNetUsers`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`AspNetRoleClaims`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`AspNetUserClaims`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`AspNetUserLogins`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`AspNetUserRoles`) = 0
        OR (SELECT COUNT(*) FROM `cevodbtest`.`AspNetUserTokens`) = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'An application table remains empty';
    END IF;
    SET FOREIGN_KEY_CHECKS = 1;
    SET SESSION sql_mode = v_old_mode;
    COMMIT;
END$$
DELIMITER ;
CALL `cevodbtest`.`seed_prod_sample_v2`();
DROP PROCEDURE `cevodbtest`.`seed_prod_sample_v2`;

-- Read-only completion report: every application table must be non-empty.
SELECT 'ArticleCategories' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`ArticleCategories`
UNION ALL
SELECT 'ArticleTags' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`ArticleTags`
UNION ALL
SELECT 'MediaAssets' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`MediaAssets`
UNION ALL
SELECT 'PracticeAreas' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`PracticeAreas`
UNION ALL
SELECT 'Articles' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`Articles`
UNION ALL
SELECT 'ArticleTagLinks' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`ArticleTagLinks`
UNION ALL
SELECT 'FaqItems' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`FaqItems`
UNION ALL
SELECT 'LawyerProfiles' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`LawyerProfiles`
UNION ALL
SELECT 'PageContents' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`PageContents`
UNION ALL
SELECT 'PageSeo' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`PageSeo`
UNION ALL
SELECT 'Locations' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`Locations`
UNION ALL
SELECT 'LocationPracticeAreas' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`LocationPracticeAreas`
UNION ALL
SELECT 'LocationArticles' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`LocationArticles`
UNION ALL
SELECT 'ContactMessages' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`ContactMessages`
UNION ALL
SELECT 'PrivateDocuments' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`PrivateDocuments`
UNION ALL
SELECT 'RedirectRules' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`RedirectRules`
UNION ALL
SELECT 'AuditLogs' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`AuditLogs`
UNION ALL
SELECT 'SiteSettings' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`SiteSettings`
UNION ALL
SELECT 'AspNetRoles' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`AspNetRoles`
UNION ALL
SELECT 'AspNetUsers' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`AspNetUsers`
UNION ALL
SELECT 'AspNetRoleClaims' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`AspNetRoleClaims`
UNION ALL
SELECT 'AspNetUserClaims' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`AspNetUserClaims`
UNION ALL
SELECT 'AspNetUserLogins' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`AspNetUserLogins`
UNION ALL
SELECT 'AspNetUserRoles' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`AspNetUserRoles`
UNION ALL
SELECT 'AspNetUserTokens' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`AspNetUserTokens`
UNION ALL
SELECT '__EFMigrationsHistory' AS TableName, COUNT(*) AS TotalRows FROM `cevodbtest`.`__EFMigrationsHistory`;

-- Expected INSERT counts (migration history is not seeded):
-- ArticleCategories: 6
-- ArticleTags: 12
-- MediaAssets: 8
-- PracticeAreas: 6
-- Articles: 24
-- ArticleTagLinks: 48
-- FaqItems: 12
-- LawyerProfiles: 2
-- PageContents: 4
-- PageSeo: 12
-- Locations: 12
-- LocationPracticeAreas: 28
-- LocationArticles: 36
-- ContactMessages: 12
-- PrivateDocuments: 3
-- RedirectRules: 5
-- AuditLogs: 8
-- SiteSettings: 10
-- AspNetRoles: 2
-- AspNetUsers: 2
-- AspNetRoleClaims: 2
-- AspNetUserClaims: 4
-- AspNetUserLogins: 2
-- AspNetUserRoles: 2
-- AspNetUserTokens: 2
