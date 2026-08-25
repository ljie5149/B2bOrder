namespace B2bOrder.Resources.ShareCore
{
    /// <summary>
    /// FileDB Core Schema
    /// 規範：
    /// 1. nid：資料庫內部主鍵與外鍵
    /// 2. sid：Web / APP / API 對外識別碼
    /// 3. avalible：Y可用；W停用；D刪除
    /// 4. 各業務系統以 reference_type + reference_sid 關聯檔案
    /// 5. 支援檔案版本、縮圖、掃毒、權限、分享與儲存供應商
    /// 6. 適用 MySQL 8.x / InnoDB / utf8mb4
    /// </summary>
    internal static class FileDB
    {
        public static readonly string CreateTables = @"
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================================
-- 01. 儲存供應商與儲存空間
-- =========================================================

CREATE TABLE IF NOT EXISTS fil_storage_provider (
    nid                     INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '儲存供應商序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    provider_code           VARCHAR(50)                         NOT NULL COMMENT '供應商代碼',
    provider_name           VARCHAR(150)                        NOT NULL COMMENT '供應商名稱',
    provider_type           VARCHAR(30)                         NOT NULL COMMENT 'LOCAL本機;S3;AZURE_BLOB;GCS;MINIO;FTP',
    endpoint_url            VARCHAR(500)                            NULL COMMENT '服務端點',
    credential_encrypted    LONGTEXT                                NULL COMMENT '加密後連線憑證',
    config_json             JSON                                    NULL COMMENT '供應商設定',
    default_bucket          VARCHAR(200)                            NULL COMMENT '預設Bucket或Container',
    public_base_url         VARCHAR(500)                            NULL COMMENT '公開檔案基礎網址',
    provider_status         VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;PAUSED暫停;DISABLED停用',
    priority                INT                                 NOT NULL DEFAULT 0 COMMENT '使用優先順序',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_fsp_provider_code UNIQUE (provider_code),
    INDEX idx_fsp_provider_type (provider_type),
    INDEX idx_fsp_status (provider_status),
    INDEX idx_fsp_priority (priority),
    INDEX idx_fsp_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='檔案儲存供應商';

CREATE TABLE IF NOT EXISTS fil_storage_space (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '儲存空間序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    provider_nid            INT UNSIGNED                        NOT NULL COMMENT '儲存供應商流水號',
    space_code              VARCHAR(80)                         NOT NULL COMMENT '儲存空間代碼',
    space_name              VARCHAR(150)                        NOT NULL COMMENT '儲存空間名稱',
    bucket_name             VARCHAR(200)                            NULL COMMENT 'Bucket或Container名稱',
    base_path               VARCHAR(500)                            NULL COMMENT '基礎路徑',
    access_type             VARCHAR(20)                         NOT NULL DEFAULT 'PRIVATE' COMMENT 'PRIVATE私有;PUBLIC公開;SIGNED_URL簽名網址',
    max_file_size           BIGINT UNSIGNED                         NULL COMMENT '單檔最大Bytes',
    total_quota             BIGINT UNSIGNED                         NULL COMMENT '空間總配額Bytes',
    used_size               BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '已使用容量Bytes',
    allowed_extensions      JSON                                    NULL COMMENT '允許副檔名',
    allowed_mime_types      JSON                                    NULL COMMENT '允許MIME類型',
    space_status            VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE啟用;READ_ONLY唯讀;DISABLED停用',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_fss_provider
        FOREIGN KEY (provider_nid) REFERENCES fil_storage_provider(nid),
    CONSTRAINT uk_fss_space_code UNIQUE (space_code),
    INDEX idx_fss_provider_nid (provider_nid),
    INDEX idx_fss_access_type (access_type),
    INDEX idx_fss_status (space_status),
    INDEX idx_fss_avalible (avalible),
    CHECK (used_size >= 0),
    CHECK (total_quota IS NULL OR total_quota >= used_size)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='檔案儲存空間';

-- =========================================================
-- 02. 資料夾
-- =========================================================

CREATE TABLE IF NOT EXISTS fil_folder (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '資料夾序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    storage_space_nid       BIGINT UNSIGNED                     NOT NULL COMMENT '儲存空間流水號',
    parent_nid              BIGINT UNSIGNED                         NULL COMMENT '上層資料夾流水號',
    folder_code             VARCHAR(100)                        NOT NULL COMMENT '資料夾代碼',
    folder_name             VARCHAR(255)                        NOT NULL COMMENT '資料夾名稱',
    physical_path           VARCHAR(1000)                       NOT NULL COMMENT '實體儲存路徑',
    tree_path               VARCHAR(2000)                           NULL COMMENT '樹狀路徑',
    folder_level            INT                                 NOT NULL DEFAULT 1 COMMENT '資料夾層級',
    owner_type              VARCHAR(30)                             NULL COMMENT 'SYSTEM系統;USER使用者;STORE商店;SERVICE服務',
    owner_sid               VARCHAR(32)                             NULL COMMENT '擁有者序號',
    access_type             VARCHAR(20)                         NOT NULL DEFAULT 'PRIVATE' COMMENT 'PRIVATE;PUBLIC;INHERIT',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_ff_storage_space
        FOREIGN KEY (storage_space_nid) REFERENCES fil_storage_space(nid),
    CONSTRAINT fk_ff_parent
        FOREIGN KEY (parent_nid) REFERENCES fil_folder(nid),
    CONSTRAINT uk_ff_space_path UNIQUE (storage_space_nid, physical_path(191)),
    INDEX idx_ff_storage_space_nid (storage_space_nid),
    INDEX idx_ff_parent_nid (parent_nid),
    INDEX idx_ff_owner (owner_type, owner_sid),
    INDEX idx_ff_access_type (access_type),
    INDEX idx_ff_avalible (avalible),
    CHECK (folder_level > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='檔案資料夾';

-- =========================================================
-- 03. 檔案主檔與版本
-- =========================================================

CREATE TABLE IF NOT EXISTS fil_file (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '檔案序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    folder_nid              BIGINT UNSIGNED                         NULL COMMENT '資料夾流水號',
    storage_space_nid       BIGINT UNSIGNED                     NOT NULL COMMENT '儲存空間流水號',
    file_no                 VARCHAR(80)                         NOT NULL COMMENT '檔案編號',
    original_name           VARCHAR(500)                        NOT NULL COMMENT '原始檔名',
    stored_name             VARCHAR(500)                        NOT NULL COMMENT '實際儲存檔名',
    file_extension          VARCHAR(30)                             NULL COMMENT '副檔名',
    mime_type               VARCHAR(150)                            NULL COMMENT 'MIME類型',
    file_size               BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '檔案大小Bytes',
    storage_path            VARCHAR(2000)                       NOT NULL COMMENT '儲存路徑',
    public_url              VARCHAR(2000)                           NULL COMMENT '公開網址',
    checksum_type           VARCHAR(20)                         NOT NULL DEFAULT 'SHA256' COMMENT 'MD5;SHA1;SHA256',
    checksum_value          VARCHAR(128)                        NOT NULL COMMENT '檔案Checksum',
    current_version         INT                                 NOT NULL DEFAULT 1 COMMENT '目前版本',
    file_category           VARCHAR(50)                             NULL COMMENT 'IMAGE圖片;DOCUMENT文件;VIDEO影片;AUDIO音訊;ARCHIVE壓縮檔;OTHER其他',
    upload_source           VARCHAR(30)                         NOT NULL DEFAULT 'USER' COMMENT 'USER使用者;SYSTEM系統;IMPORT匯入;API介接',
    uploader_sid            VARCHAR(32)                             NULL COMMENT '上傳人員序號',
    access_type             VARCHAR(20)                         NOT NULL DEFAULT 'PRIVATE' COMMENT 'PRIVATE;PUBLIC;SIGNED_URL',
    virus_scan_status       VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待掃描;CLEAN安全;INFECTED感染;FAILED失敗',
    process_status          VARCHAR(20)                         NOT NULL DEFAULT 'READY' COMMENT 'UPLOADING上傳中;READY可用;PROCESSING處理中;FAILED失敗;DELETED刪除',
    retention_until         DATETIME                                NULL COMMENT '保留期限',
    legal_hold              TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否法律保全禁止刪除',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_fff_folder
        FOREIGN KEY (folder_nid) REFERENCES fil_folder(nid),
    CONSTRAINT fk_fff_storage_space
        FOREIGN KEY (storage_space_nid) REFERENCES fil_storage_space(nid),
    CONSTRAINT uk_fff_file_no UNIQUE (file_no),
    INDEX idx_fff_folder_nid (folder_nid),
    INDEX idx_fff_storage_space_nid (storage_space_nid),
    INDEX idx_fff_checksum (checksum_type, checksum_value),
    INDEX idx_fff_file_category (file_category),
    INDEX idx_fff_uploader_sid (uploader_sid),
    INDEX idx_fff_virus_scan_status (virus_scan_status),
    INDEX idx_fff_process_status (process_status),
    INDEX idx_fff_retention_until (retention_until),
    INDEX idx_fff_avalible (avalible),
    CHECK (file_size >= 0),
    CHECK (current_version > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='檔案主檔';

CREATE TABLE IF NOT EXISTS fil_file_version (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '檔案版本序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    file_nid                BIGINT UNSIGNED                     NOT NULL COMMENT '檔案流水號',
    version_no              INT                                 NOT NULL COMMENT '版本號',
    stored_name             VARCHAR(500)                        NOT NULL COMMENT '實際儲存檔名',
    storage_path            VARCHAR(2000)                       NOT NULL COMMENT '版本儲存路徑',
    file_size               BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '檔案大小Bytes',
    checksum_value          VARCHAR(128)                        NOT NULL COMMENT 'Checksum',
    mime_type               VARCHAR(150)                            NULL COMMENT 'MIME類型',
    uploader_sid            VARCHAR(32)                             NULL COMMENT '上傳人員序號',
    change_reason           VARCHAR(500)                            NULL COMMENT '版本變更原因',
    virus_scan_status       VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING;CLEAN;INFECTED;FAILED',
    version_status          VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;ARCHIVED封存;DELETED刪除',
    CONSTRAINT fk_ffv_file
        FOREIGN KEY (file_nid) REFERENCES fil_file(nid),
    CONSTRAINT uk_ffv_file_version UNIQUE (file_nid, version_no),
    INDEX idx_ffv_file_nid (file_nid),
    INDEX idx_ffv_version_no (version_no),
    INDEX idx_ffv_virus_scan_status (virus_scan_status),
    INDEX idx_ffv_status (version_status),
    CHECK (version_no > 0),
    CHECK (file_size >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='檔案版本歷程';

-- =========================================================
-- 04. 業務附件關聯
-- =========================================================

CREATE TABLE IF NOT EXISTS fil_reference (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '檔案關聯序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    file_nid                BIGINT UNSIGNED                     NOT NULL COMMENT '檔案流水號',
    reference_service       VARCHAR(80)                         NOT NULL COMMENT '來源服務代碼',
    reference_type          VARCHAR(100)                        NOT NULL COMMENT '來源資料類型',
    reference_sid           VARCHAR(32)                         NOT NULL COMMENT '來源資料序號',
    reference_item_sid      VARCHAR(32)                             NULL COMMENT '來源明細序號',
    attachment_type         VARCHAR(50)                         NOT NULL DEFAULT 'ATTACHMENT' COMMENT 'ATTACHMENT附件;COVER封面;IMAGE圖片;CONTRACT合約;REPORT報表',
    title                   VARCHAR(500)                            NULL COMMENT '附件標題',
    description             TEXT                                    NULL COMMENT '附件說明',
    sort_no                 INT                                 NOT NULL DEFAULT 0 COMMENT '排序',
    is_primary              TINYINT(1)                          NOT NULL DEFAULT 0 COMMENT '是否主要檔案',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    CONSTRAINT fk_fr_file
        FOREIGN KEY (file_nid) REFERENCES fil_file(nid),
    CONSTRAINT uk_fr_reference UNIQUE (file_nid, reference_service, reference_type, reference_sid, reference_item_sid),
    INDEX idx_fr_file_nid (file_nid),
    INDEX idx_fr_reference (reference_service, reference_type, reference_sid),
    INDEX idx_fr_reference_item_sid (reference_item_sid),
    INDEX idx_fr_attachment_type (attachment_type),
    INDEX idx_fr_primary (is_primary),
    INDEX idx_fr_avalible (avalible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='業務資料與檔案關聯';

-- =========================================================
-- 05. 縮圖與衍生檔
-- =========================================================

CREATE TABLE IF NOT EXISTS fil_derivative (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '衍生檔序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    file_nid                BIGINT UNSIGNED                     NOT NULL COMMENT '原始檔案流水號',
    derivative_type         VARCHAR(30)                         NOT NULL COMMENT 'THUMBNAIL縮圖;PREVIEW預覽;WATERMARK浮水印;TRANSCODE轉檔;PDF_PREVIEW PDF預覽',
    width                   INT                                     NULL COMMENT '寬度',
    height                  INT                                     NULL COMMENT '高度',
    mime_type               VARCHAR(150)                            NULL COMMENT 'MIME類型',
    file_size               BIGINT UNSIGNED                     NOT NULL DEFAULT 0 COMMENT '檔案大小Bytes',
    storage_path            VARCHAR(2000)                       NOT NULL COMMENT '衍生檔儲存路徑',
    public_url              VARCHAR(2000)                           NULL COMMENT '公開網址',
    process_status          VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;PROCESSING處理中;SUCCESS成功;FAILED失敗',
    error_message           TEXT                                    NULL COMMENT '錯誤訊息',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    CONSTRAINT fk_fd_file
        FOREIGN KEY (file_nid) REFERENCES fil_file(nid),
    INDEX idx_fd_file_nid (file_nid),
    INDEX idx_fd_derivative_type (derivative_type),
    INDEX idx_fd_dimension (width, height),
    INDEX idx_fd_status (process_status),
    CHECK (file_size >= 0),
    CHECK (width IS NULL OR width > 0),
    CHECK (height IS NULL OR height > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='縮圖、預覽與衍生檔';

CREATE TABLE IF NOT EXISTS fil_processing_job (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '檔案處理工作序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    file_nid                BIGINT UNSIGNED                     NOT NULL COMMENT '檔案流水號',
    job_type                VARCHAR(30)                         NOT NULL COMMENT 'VIRUS_SCAN掃毒;THUMBNAIL縮圖;OCR文字辨識;TRANSCODE轉檔;METADATA中繼資料',
    job_config              JSON                                    NULL COMMENT '工作設定',
    job_status              VARCHAR(20)                         NOT NULL DEFAULT 'PENDING' COMMENT 'PENDING待處理;PROCESSING處理中;SUCCESS成功;FAILED失敗;DEAD死信',
    retry_count             INT                                 NOT NULL DEFAULT 0 COMMENT '重試次數',
    max_retry_count         INT                                 NOT NULL DEFAULT 3 COMMENT '最大重試次數',
    next_retry_date         DATETIME                                NULL COMMENT '下次重試時間',
    start_date              DATETIME                                NULL COMMENT '開始時間',
    completed_date          DATETIME                                NULL COMMENT '完成時間',
    result_data             JSON                                    NULL COMMENT '處理結果',
    error_message           LONGTEXT                                NULL COMMENT '錯誤訊息',
    CONSTRAINT fk_fpj_file
        FOREIGN KEY (file_nid) REFERENCES fil_file(nid),
    INDEX idx_fpj_file_nid (file_nid),
    INDEX idx_fpj_job_type (job_type),
    INDEX idx_fpj_status (job_status),
    INDEX idx_fpj_next_retry_date (next_retry_date),
    CHECK (retry_count >= 0),
    CHECK (max_retry_count >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='檔案背景處理工作';

-- =========================================================
-- 06. 檔案權限與分享
-- =========================================================

CREATE TABLE IF NOT EXISTS fil_permission (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '檔案權限序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    target_type             VARCHAR(20)                         NOT NULL COMMENT 'FILE檔案;FOLDER資料夾',
    target_sid              VARCHAR(32)                         NOT NULL COMMENT '檔案或資料夾序號',
    principal_type          VARCHAR(30)                         NOT NULL COMMENT 'USER使用者;ROLE角色;STORE商店;SERVICE服務;PUBLIC公開',
    principal_sid           VARCHAR(32)                             NULL COMMENT '被授權對象序號',
    permission_type         VARCHAR(20)                         NOT NULL COMMENT 'READ讀取;WRITE修改;DELETE刪除;SHARE分享;OWNER擁有者',
    start_date              DATETIME                                NULL COMMENT '生效時間',
    end_date                DATETIME                                NULL COMMENT '失效時間',
    granted_user_sid        VARCHAR(32)                             NULL COMMENT '授權人員序號',
    avalible                VARCHAR(2)                          NOT NULL DEFAULT 'Y' COMMENT 'Y可用;D刪除;W停用',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT uk_fp_permission UNIQUE (target_type, target_sid, principal_type, principal_sid, permission_type),
    INDEX idx_fp_target (target_type, target_sid),
    INDEX idx_fp_principal (principal_type, principal_sid),
    INDEX idx_fp_permission_type (permission_type),
    INDEX idx_fp_date (start_date, end_date),
    INDEX idx_fp_avalible (avalible),
    CHECK (end_date IS NULL OR start_date IS NULL OR end_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='檔案與資料夾權限';

CREATE TABLE IF NOT EXISTS fil_share_link (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '分享連結序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    file_nid                BIGINT UNSIGNED                         NULL COMMENT '檔案流水號',
    folder_nid              BIGINT UNSIGNED                         NULL COMMENT '資料夾流水號',
    share_token_hash        VARCHAR(255)                        NOT NULL COMMENT '分享Token雜湊',
    password_hash           VARCHAR(255)                            NULL COMMENT '分享密碼雜湊',
    start_date              DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '分享開始時間',
    expiry_date             DATETIME                                NULL COMMENT '分享失效時間',
    max_download_count      INT                                     NULL COMMENT '最大下載次數',
    download_count          INT                                 NOT NULL DEFAULT 0 COMMENT '已下載次數',
    allow_preview           TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否允許預覽',
    allow_download          TINYINT(1)                          NOT NULL DEFAULT 1 COMMENT '是否允許下載',
    share_status            VARCHAR(20)                         NOT NULL DEFAULT 'ACTIVE' COMMENT 'ACTIVE有效;EXPIRED過期;REVOKED撤銷',
    create_user_sid         VARCHAR(32)                             NULL COMMENT '建立人員序號',
    remark                  TEXT                                    NULL COMMENT '備註',
    CONSTRAINT fk_fsl_file
        FOREIGN KEY (file_nid) REFERENCES fil_file(nid),
    CONSTRAINT fk_fsl_folder
        FOREIGN KEY (folder_nid) REFERENCES fil_folder(nid),
    UNIQUE KEY uk_fsl_token_hash (share_token_hash),
    INDEX idx_fsl_file_nid (file_nid),
    INDEX idx_fsl_folder_nid (folder_nid),
    INDEX idx_fsl_expiry_date (expiry_date),
    INDEX idx_fsl_status (share_status),
    CHECK (file_nid IS NOT NULL OR folder_nid IS NOT NULL),
    CHECK (max_download_count IS NULL OR max_download_count > 0),
    CHECK (download_count >= 0),
    CHECK (expiry_date IS NULL OR expiry_date >= start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='檔案分享連結';

CREATE TABLE IF NOT EXISTS fil_access_log (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '檔案存取紀錄序號',
    access_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '存取時間',
    file_sid                VARCHAR(32)                             NULL COMMENT '檔案序號',
    folder_sid              VARCHAR(32)                             NULL COMMENT '資料夾序號',
    share_link_sid          VARCHAR(32)                             NULL COMMENT '分享連結序號',
    actor_type              VARCHAR(30)                         NOT NULL COMMENT 'USER使用者;SERVICE服務;ANONYMOUS匿名',
    actor_sid               VARCHAR(32)                             NULL COMMENT '操作對象序號',
    action_type             VARCHAR(20)                         NOT NULL COMMENT 'VIEW預覽;DOWNLOAD下載;UPLOAD上傳;UPDATE修改;DELETE刪除;SHARE分享',
    result                  VARCHAR(20)                         NOT NULL DEFAULT 'SUCCESS' COMMENT 'SUCCESS成功;DENIED拒絕;FAILED失敗',
    ip_address              VARCHAR(50)                             NULL COMMENT 'IP位址',
    user_agent              VARCHAR(1000)                           NULL COMMENT '瀏覽器或裝置資訊',
    request_id              VARCHAR(100)                            NULL COMMENT '請求識別碼',
    remark                  TEXT                                    NULL COMMENT '備註',
    INDEX idx_fal_file_sid (file_sid),
    INDEX idx_fal_folder_sid (folder_sid),
    INDEX idx_fal_share_link_sid (share_link_sid),
    INDEX idx_fal_actor (actor_type, actor_sid),
    INDEX idx_fal_action_type (action_type),
    INDEX idx_fal_access_date (access_date),
    INDEX idx_fal_result (result),
    INDEX idx_fal_request_id (request_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='檔案存取稽核紀錄';

-- =========================================================
-- 07. 分段上傳
-- =========================================================

CREATE TABLE IF NOT EXISTS fil_upload_session (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '上傳工作階段序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    modify_date             DATETIME                                NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '修改日期',
    upload_id               VARCHAR(150)                        NOT NULL COMMENT '上傳識別碼',
    storage_space_nid       BIGINT UNSIGNED                     NOT NULL COMMENT '儲存空間流水號',
    folder_nid              BIGINT UNSIGNED                         NULL COMMENT '目標資料夾流水號',
    original_name           VARCHAR(500)                        NOT NULL COMMENT '原始檔名',
    mime_type               VARCHAR(150)                            NULL COMMENT 'MIME類型',
    total_size              BIGINT UNSIGNED                     NOT NULL COMMENT '檔案總大小Bytes',
    chunk_size              BIGINT UNSIGNED                     NOT NULL COMMENT '分段大小Bytes',
    total_chunks            INT                                 NOT NULL COMMENT '總分段數',
    uploaded_chunks         INT                                 NOT NULL DEFAULT 0 COMMENT '已上傳分段數',
    uploader_sid            VARCHAR(32)                             NULL COMMENT '上傳人員序號',
    upload_status           VARCHAR(20)                         NOT NULL DEFAULT 'UPLOADING' COMMENT 'UPLOADING上傳中;COMPLETING合併中;SUCCESS成功;FAILED失敗;CANCELLED取消;EXPIRED過期',
    expiry_date             DATETIME                            NOT NULL COMMENT '工作階段到期時間',
    completed_file_sid      VARCHAR(32)                             NULL COMMENT '完成後檔案序號',
    error_message           TEXT                                    NULL COMMENT '錯誤訊息',
    CONSTRAINT fk_fus_storage_space
        FOREIGN KEY (storage_space_nid) REFERENCES fil_storage_space(nid),
    CONSTRAINT fk_fus_folder
        FOREIGN KEY (folder_nid) REFERENCES fil_folder(nid),
    CONSTRAINT uk_fus_upload_id UNIQUE (upload_id),
    INDEX idx_fus_storage_space_nid (storage_space_nid),
    INDEX idx_fus_folder_nid (folder_nid),
    INDEX idx_fus_uploader_sid (uploader_sid),
    INDEX idx_fus_status (upload_status),
    INDEX idx_fus_expiry_date (expiry_date),
    CHECK (total_size > 0),
    CHECK (chunk_size > 0),
    CHECK (total_chunks > 0),
    CHECK (uploaded_chunks >= 0),
    CHECK (uploaded_chunks <= total_chunks),
    CHECK (expiry_date > create_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='檔案分段上傳工作階段';

CREATE TABLE IF NOT EXISTS fil_upload_chunk (
    nid                     BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '流水號',
    sid                     VARCHAR(32)                         NOT NULL UNIQUE COMMENT '上傳分段序號',
    create_date             DATETIME                            NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '建立日期',
    upload_session_nid      BIGINT UNSIGNED                     NOT NULL COMMENT '上傳工作階段流水號',
    chunk_no                INT                                 NOT NULL COMMENT '分段編號',
    chunk_size              BIGINT UNSIGNED                     NOT NULL COMMENT '分段大小Bytes',
    checksum_value          VARCHAR(128)                            NULL COMMENT '分段Checksum',
    storage_path            VARCHAR(2000)                       NOT NULL COMMENT '暫存路徑',
    chunk_status            VARCHAR(20)                         NOT NULL DEFAULT 'UPLOADED' COMMENT 'UPLOADING上傳中;UPLOADED已上傳;VERIFIED已驗證;FAILED失敗',
    CONSTRAINT fk_fuc_upload_session
        FOREIGN KEY (upload_session_nid) REFERENCES fil_upload_session(nid),
    CONSTRAINT uk_fuc_session_chunk UNIQUE (upload_session_nid, chunk_no),
    INDEX idx_fuc_upload_session_nid (upload_session_nid),
    INDEX idx_fuc_status (chunk_status),
    CHECK (chunk_no >= 0),
    CHECK (chunk_size > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='檔案分段上傳明細';

SET FOREIGN_KEY_CHECKS = 1;
";
    }
}
