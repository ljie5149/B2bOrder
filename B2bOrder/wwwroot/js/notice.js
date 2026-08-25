/**
 * B2bOrder - 公告管理系統前端邏輯
 */

// 1. 輔助函式：取得 Anti-Forgery Token
function getRequestVerificationToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
}

// 2. 輔助函式：清除模組內的驗證紅框與錯誤訊息
function clearModalValidation(modalElement) {
    if (!modalElement) return;
    modalElement.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
    modalElement.querySelectorAll('.invalid-feedback').forEach(el => el.innerText = '');
}

/**
 * =========================================================================
 * 1. 新增公告模組 (Create)
 * =========================================================================
 */

// 💡 修正：明確將此全域方法宣告出來，防止網頁發生 "ReferenceError: showCreateAnnouncementModal is not defined"
window.openCreateAnnouncementModal = window.showCreateAnnouncementModal = function () {
    const modalEl = document.getElementById('createAnnouncementModal');
    if (modalEl) {
        clearModalValidation(modalEl); // 開啟前清除上一次殘留的驗證錯誤
        var myModal = new bootstrap.Modal(modalEl);
        myModal.show();
    }
};

function validateCreateForm() {
    let isValid = true;
    const fields = ['Title', 'Content', 'StartDate', 'EndDate'];
    fields.forEach(field => {
        const el = document.getElementById('CreateAnnouncement_' + field);
        if (el) {
            if (!el.value.trim()) {
                el.classList.add('is-invalid');
                isValid = false;
            } else {
                el.classList.remove('is-invalid');
            }
        }
    });
    return isValid;
}

function createAnnouncement() {
    if (!validateCreateForm()) return alert('請完整填寫必填欄位');

    const isTop = document.getElementById('CreateAnnouncement_IsTop').checked;
    const sendPush = document.getElementById('CreateAnnouncement_SendPushNotification')?.checked || false;

    // 💡 依據 #2 規格：若包含置頂或發送推播，先彈出警告 Modal 提醒會覆蓋舊置頂
    if (isTop || sendPush) {
        var createModal = bootstrap.Modal.getInstance(document.getElementById('createAnnouncementModal'));
        if (createModal) createModal.hide();

        var warningModal = new bootstrap.Modal(document.getElementById('announcementWarningModal'));
        warningModal.show();
    } else {
        submitCreateAnnouncementData();
    }
}

// 💡 修正：補上警告 Modal 中點擊「仍然發布」時觸發的連動函式
window.continuePublishAnnouncement = function () {
    var warningModal = bootstrap.Modal.getInstance(document.getElementById('announcementWarningModal'));
    if (warningModal) warningModal.hide();
    submitCreateAnnouncementData();
};

function submitCreateAnnouncementData() {
    const payload = {
        CreatorSid: document.getElementById('CreateAnnouncement_CreatorSid')?.value || '',
        PublishUnit: document.getElementById('CreateAnnouncement_PublishUnit')?.value || '系統管理部',
        Category: document.getElementById('CreateAnnouncement_Category')?.value || 'General', // 前端預設 Value 英文
        Title: document.getElementById('CreateAnnouncement_Title').value,
        Content: document.getElementById('CreateAnnouncement_Content').value,
        StartDate: document.getElementById('CreateAnnouncement_StartDate').value,
        EndDate: document.getElementById('CreateAnnouncement_EndDate').value,
        IsTop: document.getElementById('CreateAnnouncement_IsTop').checked,
        SendPushNotification: document.getElementById('CreateAnnouncement_SendPushNotification')?.checked || false
    };

    fetch('/Notice/Create', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getRequestVerificationToken() },
        body: JSON.stringify(payload)
    })
        .then(r => r.json())
        .then(res => {
            if (res.success) {
                alert('公告發布成功');
                location.reload();
            } else {
                alert(res.message || '發布失敗');
            }
        });
}

/**
 * =========================================================================
 * 2. 編輯公告模組 (Edit)
 * =========================================================================
 */
window.openEditAnnouncementModal = window.showEditAnnouncementModal = function (sid) {
    console.log("[Debug] 開始讀取編輯資料，SID:", sid);

    // 💡 呼叫 NoticeController 的 GetDetail，不需要點閱數增加 (incrementClick = false)
    fetch(`/Notice/GetDetail?sid=${sid}&incrementClick=false`, { method: 'GET' })
        .then(r => r.json())
        .then(res => {
            if (res.success && res.data) {
                const item = res.data;

                // 填入欄位值 (對齊首字小寫 camelCase 機制)
                document.getElementById('EditAnnouncement_Sid').value = item.sid || '';
                document.getElementById('EditAnnouncement_PublishUnit').value = item.publishUnit || '系統管理部';

                // 💡 【核心修正：公告分類自動選取轉換】
                // 建立對照表：同時防護資料庫存「中文」或「全小寫英文」的狀況
                const categoryMap = {
                    'general': 'General', '一般公告': 'General',
                    'activity': 'Activity', '活動通知': 'Activity',
                    'system': 'System', '系統維護': 'System',
                    'urgent': 'Urgent', '緊急通報': 'Urgent'
                };

                // 取得資料庫傳回的值並轉小寫（如果是英文的話）
                const rawCategory = item.category ? item.category.toString().trim() : '';
                const lookupKey = rawCategory.toLowerCase();

                // 如果對照表有找到，就塞對應的標準首字大寫值；找不到就沿用原值
                const finalCategoryValue = categoryMap[lookupKey] || rawCategory;

                // 明確指派給 select 控制項，瀏覽器就會自動選中對應的 option
                document.getElementById('EditAnnouncement_Category').value = finalCategoryValue;
                console.log(`[Debug] 分類原始值: ${rawCategory} -> 自動選中 Value: ${finalCategoryValue}`);

                document.getElementById('EditAnnouncement_Title').value = item.title || '';
                document.getElementById('EditAnnouncement_Content').value = item.content || '';

                if (item.startDate) document.getElementById('EditAnnouncement_StartDate').value = item.startDate.substring(0, 10);
                if (item.endDate) document.getElementById('EditAnnouncement_EndDate').value = item.endDate.substring(0, 10);

                document.getElementById('EditAnnouncement_IsTop').checked = (item.isTop === 1 || item.isTop === true);

                // 顯示彈窗
                const modalEl = document.getElementById('editAnnouncementModal');
                clearModalValidation(modalEl);

                let myModal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                myModal.show();
            } else {
                alert(res.message || '無法取得公告明細資料');
            }
        })
        .catch(err => console.error("Fetch 錯誤:", err));
};

function saveAnnouncement() {
    const payload = {
        Sid: document.getElementById('EditAnnouncement_Sid').value,
        PublishUnit: document.getElementById('EditAnnouncement_PublishUnit').value,
        Category: document.getElementById('EditAnnouncement_Category').value,
        Title: document.getElementById('EditAnnouncement_Title').value,
        Content: document.getElementById('EditAnnouncement_Content').value,
        StartDate: document.getElementById('EditAnnouncement_StartDate').value,
        EndDate: document.getElementById('EditAnnouncement_EndDate').value,
        IsTop: document.getElementById('EditAnnouncement_IsTop').checked
    };

    fetch('/Notice/Edit', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getRequestVerificationToken() },
        body: JSON.stringify(payload)
    })
        .then(r => r.json())
        .then(res => {
            if (res.success) {
                alert('資料更新成功');
                location.reload();
            } else {
                alert(res.message || '更新失敗');
            }
        });
}

/**
 * =========================================================================
 * 3. 檢視與刪除模組 (View & Delete)
 * =========================================================================
 */
window.openViewAnnouncementModal = window.showViewAnnouncementModal = function (sid) {
    console.log("[Debug] 開始檢視公告明細，SID:", sid);

    // 💡 點開檢視，將 incrementClick 設為 true 觸發後端點閱數自動 +1
    fetch(`/Notice/GetDetail?sid=${sid}&incrementClick=true`, { method: 'GET' })
        .then(r => r.json())
        .then(res => {
            if (res.success && res.data) {
                const item = res.data;

                document.getElementById('ViewAnnouncement_Title').value = item.title || '';
                document.getElementById('ViewAnnouncement_PublishUnit').value = item.publishUnit || '系統管理部';
                document.getElementById('ViewAnnouncement_Category').value = item.category || '一般公告';
                document.getElementById('ViewAnnouncement_Content').value = item.content || '';

                // 即時更新網頁上的點閱數顯示
                const clickCountEl = document.getElementById('ViewAnnouncement_ClickCount');
                if (clickCountEl) clickCountEl.innerText = item.clickCount || 0;

                const modalEl = document.getElementById('viewAnnouncementModal');
                let myModal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                myModal.show();
            } else {
                alert('無法讀取公告明細');
            }
        })
        .catch(err => console.error("Fetch 錯誤:", err));
};

function executeDeleteAction(sid, actionType) {
    // 💡 修正：串接後端 Controller 的 Delete Payload 結構 (Sid 與 ActionType)
    fetch('/Notice/Delete', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getRequestVerificationToken() },
        body: JSON.stringify({ Sid: sid, ActionType: actionType })
    })
        .then(r => r.json())
        .then(res => {
            if (res.success) {
                location.reload();
            } else {
                alert(res.message || '操作執行失敗');
            }
        });
}

/**
 * =========================================================================
 * 4. 刪除/下架公告模組 (雙層 Modal 控制流)
 * =========================================================================
 */
// 💡 1. 點擊列表的刪除按鈕時觸發：打開第一個選擇視窗，並動態填入標題與 Sid
window.deleteAnnouncement = function (sid) {
    console.log("[Debug] 按下刪除按鈕，SID:", sid);
    if (!sid) return;

    // 先呼叫現有的 GetDetail 取得該公告的最新標題，用來在彈窗顯示
    fetch(`/Notice/GetDetail?sid=${sid}&incrementClick=false`, { method: 'GET' })
        .then(r => r.json())
        .then(res => {
            if (res.success && res.data) {
                // 將資料暫存到第一個 Modal 的隱藏控制項中
                document.getElementById('deleteAnnouncementSid').value = res.data.sid;
                document.getElementById('deleteAnnouncementTitle').innerText = `欲處置之公告：${res.data.title}`;

                // 重設第一個 Modal 的 Radio Button 預設選取 ARCHIVE
                const defaultRadio = document.querySelector('input[name="deleteType"][value="ARCHIVE"]');
                if (defaultRadio) defaultRadio.checked = true;

                // 打開第一個選單 Modal
                const firstModalEl = document.getElementById('deleteAnnouncementModal');
                let firstModal = bootstrap.Modal.getInstance(firstModalEl) || new bootstrap.Modal(firstModalEl);
                firstModal.show();
            } else {
                alert(res.message || "無法讀取公告基本資料");
            }
        })
        .catch(err => console.error("[Error] 讀取刪除明細失敗:", err));
};

// 💡 2. 第一個 Modal 按下「確認執行」時觸發：判斷要直接呼叫後端下架，還是要開啟第二層警告
window.showAnnouncementDeleteConfirmModal = function () {
    const sid = document.getElementById('deleteAnnouncementSid').value;
    // 取得選取的是 ARCHIVE 還是 PERMANENT
    const deleteType = document.querySelector('input[name="deleteType"]:checked').value;

    // 從第一個 Modal 抓取剛剛填好的標題文字 (去掉前綴字以便美化第二層顯示)
    const rawTitle = document.getElementById('deleteAnnouncementTitle').innerText.replace('欲處置之公告：', '');

    // 關閉第一個選擇 Modal
    const firstModalEl = document.getElementById('deleteAnnouncementModal');
    let firstModal = bootstrap.Modal.getInstance(firstModalEl);
    if (firstModal) firstModal.hide();

    if (deleteType === "PERMANENT") {
        // 【永久刪除流】：打開第二層「最終確認警告 Modal」
        document.getElementById('confirmDeleteAnnouncementTitle').innerText = rawTitle;

        const secondModalEl = document.getElementById('deleteConfirmModal');
        let secondModal = bootstrap.Modal.getInstance(secondModalEl) || new bootstrap.Modal(secondModalEl);
        secondModal.show();
    } else {
        // 【僅提前下架流】：不開第二層，直接打包 Payload 送往後端
        executeDeletePost(sid, "ARCHIVE");
    }
};

// 💡 3. 第二個 Modal（最終警告）按下「確定永久刪除」時觸發
window.deleteAnnouncementConfirm = function () {
    const sid = document.getElementById('deleteAnnouncementSid').value; // 序號依然在第一個 hidden input 中

    // 關閉第二層確認 Modal
    const secondModalEl = document.getElementById('deleteConfirmModal');
    let secondModal = bootstrap.Modal.getInstance(secondModalEl);
    if (secondModal) secondModal.hide();

    // 執行永久刪除請求
    executeDeletePost(sid, "PERMANENT");
};

// 💡 4. 內部封裝：精準對接 NoticeController.Delete 接收的 JSON 格式
function executeDeletePost(sid, actionType) {
    if (!sid) {
        alert("關鍵編號遺失，無法執行操作");
        return;
    }

    // 對齊後端 C# 的 DeleteActionPayload 結構 (大小寫皆可，JSON 會自動轉型)
    const payload = {
        Sid: sid,
        ActionType: actionType
    };

    fetch('/Notice/Delete', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
    })
        .then(r => r.json())
        .then(res => {
            if (res.success) {
                alert(res.message || "操作已成功執行！");
                location.reload(); // 重新整理頁面更新公告列表
            } else {
                alert(res.message || "處置執行失敗，請檢查系統紀錄");
            }
        })
        .catch(err => {
            console.error("[Error] 傳送刪除請求時發生通訊錯誤:", err);
            alert("與伺服器連線失敗，請檢查網路狀態。");
        });
}