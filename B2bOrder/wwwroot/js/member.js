function toggleNode(btn) {
    // 找到最外層的節點外殼
    const nodeWrapper = btn.closest('.node-wrapper');
    if (!nodeWrapper) return;

    // 直接切換外殼的收合樣式狀態 (is-collapsed)
    const isCollapsing = nodeWrapper.classList.toggle('is-collapsed');

    // 同步切換按鈕文字
    if (isCollapsing) {
        btn.innerHTML = '＋';
    } else {
        btn.innerHTML = '-';
    }
}

// 一般會員警告 modal
let warningModal;
let createSid = '';
let createMid = '';
document.addEventListener(
    "DOMContentLoaded",
    function () {

        warningModal =
            new bootstrap.Modal(
                document.getElementById(
                    "normalMemberWarningModal"
                )
            );
    }
);
function showCreateMemberModal(
    sid,
    mid,
    role
) {
    createSid = sid;
    createMid = mid;
    if (role === "Stc") {
        warningModal.show();

        return;
    }

    continueCreateMember()
}
function continueCreateMember() {
    warningModal.hide();

    openCreateMemberModal(
        createSid,
        createMid
    );
}

// 顯示新增會員 modal
let createMemberModal;
document.addEventListener(
    "DOMContentLoaded",
    function () {

        createMemberModal =
            new bootstrap.Modal(
                document.getElementById(
                    "createMemberModal"
                )
            );
    }
);
function openCreateMemberModal(
    sid,
    mid
) {
    document.getElementById(
        "CreateMember_ParentSid"
    ).value = sid;

    document.getElementById(
        "CreateMember_ParentMid"
    ).value = mid;

    document.getElementById(
        "CreateMember_Mid"
    ).value = "";

    document.getElementById(
        "CreateMember_Name"
    ).value = "";

    document.getElementById(
        "CreateMember_Mobile"
    ).value = "";

    document.getElementById(
        "CreateMember_Email"
    ).value = "";

    createMemberModal.show();
}
async function createMember() {
    document
        .querySelectorAll(
            '#createMemberModal .is-invalid'
        )
        .forEach(x =>
            x.classList.remove('is-invalid')
        );

    document
        .querySelectorAll(
            '#createMemberModal .invalid-feedback'
        )
        .forEach(x =>
            x.innerHTML = ''
        );

    const data = {

        parentSid:
            document.getElementById(
                "CreateMember_ParentSid"
            ).value,

        mid:
            document.getElementById(
                "CreateMember_Mid"
            ).value,

        name:
            document.getElementById(
                "CreateMember_Name"
            ).value,

        mobile:
            document.getElementById(
                "CreateMember_Mobile"
            ).value,

        email:
            document.getElementById(
                "CreateMember_Email"
            ).value,

        idNumber:
            document.getElementById(
                "CreateMember_IdNumber"
            ).value,

        joinDate:
            document.getElementById(
                "CreateMember_JoinDate"
            ).value || null,

        continueDate:
            document.getElementById(
                "CreateMember_ContinueDate"
            ).value || null,

        role:
            document.getElementById(
                "CreateMember_Role"
            ).value
    };

    // console.log(data);
    const response =
        await fetch(
            '/Member/CreateMember',
            {
                method: 'POST',
                headers: {
                    'Content-Type':
                        'application/json'
                },
                body:
                    JSON.stringify(data)
            });

    const result =
        await response.json();
    // console.log(result);

    if (!result.success) {
        if (result.errors) {
            Object.keys(result.errors)
                .forEach(key => {
                    const input =
                        document.getElementById(
                            "CreateMember_" + key
                        );

                    if (!input) {
                        console.log(
                            "找不到欄位:",
                            key
                        );
                        return;
                    }

                    input.classList.add(
                        "is-invalid"
                    );

                    const feedback =
                        input.closest('.mb-3')
                            ?.querySelector(
                                '.invalid-feedback'
                            );

                    if (feedback) {
                        feedback.innerText =
                            result.errors[key];
                    }
                });
        }
        else {
            showMessage(
                "資料驗證失敗",
                result.message || "資料驗證失敗",
                false
            );
        }

        return;
    }

    createMemberModal.hide();

    showMessage(
        "新增會員成功",
        "新增會員已完成",
        true,
        function () {
            location.reload();
        }
    );
}
function renewCreateMember() {
    const today = getTodayDate();

    document.getElementById(
        "CreateMember_ContinueDate"
    ).value = today;
}

// 顯示編輯會員 modal
let editMemberModal;
document.addEventListener(
    "DOMContentLoaded",
    function () {

        editMemberModal =
            new bootstrap.Modal(
                document.getElementById(
                    "editMemberModal"
                )
            );
    });
async function showEditMemberModal(sid) {
    const response =
        await fetch(
            '/Member/EditMember?sid=' + sid
        );

    const data =
        await response.json();

    document.getElementById(
        'EditMember_Sid'
    ).value = data.sid;

    document.getElementById(
        'EditMember_Mid'
    ).value = data.mid;

    document.getElementById(
        'EditMember_ParentMid'
    ).value = data.parentMid;

    document.getElementById(
        'EditMember_Name'
    ).value = data.name ?? '';

    document.getElementById(
        'EditMember_Mobile'
    ).value = data.mobile ?? '';

    document.getElementById(
        'EditMember_Email'
    ).value = data.email ?? '';

    document.getElementById(
        'EditMember_IdNumber'
    ).value = data.idNumber ?? '';

    document.getElementById(
        'EditMember_Role'
    ).value = data.role;

    document.getElementById(
        'EditMember_JoinDate'
    ).value = data.joinDate ?? '';

    document.getElementById(
        'EditMember_ContinueDate'
    ).value = data.continueDate ?? '';

    editMemberModal.show();
}
async function saveMember() {
    document
        .querySelectorAll(
            "#editMemberModal .is-invalid"
        )
        .forEach(x =>
            x.classList.remove(
                "is-invalid"
            )
        );

    const model =
    {
        sid:
            document.getElementById(
                'EditMember_Sid'
            ).value,

        name:
            document.getElementById(
                'EditMember_Name'
            ).value,

        mobile:
            document.getElementById(
                'EditMember_Mobile'
            ).value,

        email:
            document.getElementById(
                'EditMember_Email'
            ).value,

        idNumber:
            document.getElementById(
                'EditMember_IdNumber'
            ).value,

        role:
            document.getElementById(
                'EditMember_Role'
            ).value,

        joinDate:
            document.getElementById(
                'EditMember_JoinDate'
            ).value || null,

        ContinueDate:
            document.getElementById(
                'EditMember_ContinueDate'
            ).value || null
    };
    // console.log(model);

    const response =
        await fetch(
            '/Member/EditMember',
            {
                method: 'POST',
                headers:
                {
                    'Content-Type':
                        'application/json'
                },
                body:
                    JSON.stringify(model)
            });

    const result =
        await response.json();

    console.log(result);
    if (!result.success) {
        if (result.errors) {
            Object.keys(result.errors)
                .forEach(key => {
                    const input =
                        document.getElementById(
                            "EditMember_" + key
                        );

                    if (!input)
                        return;

                    input.classList.add(
                        "is-invalid"
                    );

                    const feedback =
                        input.closest('.mb-3')
                            ?.querySelector(
                                '.invalid-feedback'
                            );

                    if (feedback) {
                        feedback.innerText =
                            result.errors[key];
                    }
                });
        }

        return;
    }

    editMemberModal.hide();

    showMessage(
        "編輯會員成功",
        "編輯會員已完成",
        true,
        function () {
            location.reload();
        }
    );
}
function renewEditMember() {
    const today = getTodayDate();

    document.getElementById(
        "EditMember_ContinueDate"
    ).value = today;
}

// 顯示刪除會員 modal
let deleteModal;
document.addEventListener(
    "DOMContentLoaded",
    function () {

        deleteModal =
            new bootstrap.Modal(
                document.getElementById(
                    "deleteMemberModal"
                )
            );
    }
);
function showDeleteDialog(
    sid,
    mid
) {
    document.getElementById(
        "deleteSid"
    ).value = sid;

    document.getElementById(
        "deleteMemberName"
    ).innerHTML =
        "刪除會員：" + mid;

    deleteModal.show();
}

// 顯示確認刪除會員 modal
let deleteConfirmModal;
document.addEventListener(
    "DOMContentLoaded",
    function () {
        deleteConfirmModal =
            new bootstrap.Modal(
                document.getElementById(
                    "deleteConfirmModal"
                )
            );
    }
);
function showDeleteConfirmModal() {
    const memberName =
        document.getElementById(
            "deleteMemberName"
        ).innerText;

    document.getElementById(
        "confirmDeleteMemberName"
    ).innerText = memberName;

    deleteModal.hide();
    deleteConfirmModal.show();
}
async function deleteMemberConfirm() {
    // 1. 取得刪除目標的 sid (加上 ?. 防呆)
    const sid = document.getElementById("deleteSid")?.value || "";

    // 2. 取得 Radio 選取的刪除類型 (加上 ?. 防呆，未選則預設空字串)
    const type = document.querySelector('input[name="deleteType"]:checked')?.value || "";

    // 3. 取得轉移目標的 targetSid 與 targetMid (加上 ?. 防呆)
    const targetSid = document.getElementById("targetSid")?.value || "";
    const targetMid = document.getElementById("targetMid")?.value || "";

    // 簡單的客戶端驗證：如果連 sid 或刪除類型都抓不到，直接中斷執行
    if (!sid) {
        showMessage("錯誤", "無法取得欲刪除的會員編號", false);
        return;
    }

    if (!type) {
        showMessage("提示", "請選擇刪除處置方式", false);
        return;
    }

    const response = await fetch('/Member/Delete', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            sid: sid,
            handleType: type,
            targetSid: targetSid,
            targetMid: targetMid
        })
    });

    const result = await response.json();

    if (!result.success) {
        showMessage(
            "刪除會員失敗",
            result.message || "刪除會員失敗",
            false
        );
        return;
    }

    location.reload();
}

// 會員續約 modal
let renewMemberModal;
document.addEventListener(
    "DOMContentLoaded",
    function () {
        renewMemberModal =
            new bootstrap.Modal(
                document.getElementById(
                    "renewMemberModal"
                )
            );
    }
);
function renewMember(
    sid,
    mid
) {
    document.getElementById(
        "renewSid"
    ).value = sid;

    document.getElementById(
        "renewMid"
    ).innerText = mid;

    const nextYear =
        new Date();

    nextYear.setFullYear(
        nextYear.getFullYear() + 1
    );

    document.getElementById(
        "renewDatePreview"
    ).innerText =
        nextYear.toLocaleDateString();

    renewMemberModal.show();
}
async function renewMemberConfirm() {
    const sid =
        document.getElementById(
            "renewSid"
        ).value;

    const response =
        await fetch(
            '/Member/RenewMember',
            {
                method: 'POST',
                headers:
                {
                    'Content-Type':
                        'application/json'
                },
                body:
                    JSON.stringify(
                        {
                            sid: sid
                        })
            });

    const result =
        await response.json();

    if (!result.success) {
        showMessage(
            "續約失敗",
            result.message || "續約失敗",
            false
        );
        return;
    }

    renewMemberModal.hide();
    showMessage(
        "會員續約成功",
        "會員續約已完成",
        true,
        function () {
            location.reload();
        }
    );
}

// 系統訊息 modal
let messageModal;
document.addEventListener(
    "DOMContentLoaded",
    function () {
        messageModal =
            new bootstrap.Modal(
                document.getElementById(
                    "messageModal"
                )
            );
    });
function showMessage(
    title,
    message,
    isSuccess = false,
    callback = null
) {
    messageCallback = callback;

    document.getElementById(
        "messageTitle"
    ).innerText = title;

    document.getElementById(
        "messageText"
    ).innerText = message;

    document.getElementById(
        "messageIcon"
    ).innerHTML =
        isSuccess
            ? `<i class="bi bi-check-circle-fill text-success"
                     style="font-size:60px;"></i>`
            : `<i class="bi bi-exclamation-triangle-fill text-danger"
                     style="font-size:60px;"></i>`;

    messageModal.show();
}
function closeMessageModal() {
    messageModal.hide();

    if (messageCallback) {
        messageCallback();
        messageCallback = null;
    }
}

// 匯出 Export Modal
let exportModal;
const exportModalEl =
    document.getElementById(
        "exportModal"
    );

if (exportModalEl) {
    exportModal =
        new bootstrap.Modal(
            exportModalEl
        );
}
function showExportModal() {
    exportModal.show();
}
function exportMembers() {
    const type =
        document.getElementById(
            "exportType"
        ).value;

    const scope =
        document.getElementById(
            "exportScope"
        ).value;

    const keyword =
        document.querySelector(
            'input[name="keyword"]'
        )?.value ?? '';

    const status =
        document.querySelector(
            'select[name="status"]'
        )?.value ?? '';

    window.location =
        `/Member/Export?type=${type}`
        + `&scope=${scope}`
        + `&keyword=${encodeURIComponent(keyword)}`
        + `&status=${status}`;
}

// 匯入 Import Modal
let importModal;
const importModalEl =
    document.getElementById(
        "importModal"
    );

if (importModalEl) {
    importModal =
        new bootstrap.Modal(
            importModalEl
        );
}
function showImportModal() {
    document.getElementById(
        "importFile"
    ).value = "";

    importModal.show();
}
async function importMembers() {

    document.getElementById(
        "importProgressBar"
    ).style.width = "0%";

    document.getElementById(
        "importProgressBar"
    ).innerHTML = "0%";

    document.getElementById(
        "importProgressText"
    ).innerHTML = "0 / 0";

    document.getElementById(
        "importProgressArea"
    ).style.display = "";

    document.getElementById(
        "importResult"
    ).innerHTML = "";

    document.getElementById(
        "btnImportSubmit"
    ).disabled = true;

    document.getElementById(
        "btnImportCancel"
    ).disabled = true;

    try {
        const file =
            document.getElementById(
                "importFile"
            ).files[0];

        if (!file) {
            showMessage(
                "錯誤",
                "請選擇檔案"
            );
            return;
        }

        const formData =
            new FormData();

        formData.append(
            "file",
            file
        );

        const response =
            await fetch(
                "/Member/Import",
                {
                    method: "POST",
                    body: formData
                });

        const result =
            await response.json();
        //console.log(result);

        if (!result.success) {
            showMessage(
                "錯誤",
                result.message ?? "匯入失敗"
            );
            return;
        }

        let html = "";

        html += `
            <div>
                <b>新增：</b>${result.insertCount} 筆
            </div>

            <div>
                <b>更新：</b>${result.updateCount} 筆
            </div>

            <div>
                <b>失敗：</b>${result.skipCount} 筆
            </div>
        `;

        if (result.errors &&
            result.errors.length > 0) {
            html += `
                <hr>
                <div class="text-danger fw-bold">
                    失敗明細
                </div>
                <ul class="mt-2">
            `;

            result.errors.forEach(x => {
                html += `<li>${x}</li>`;
            });

            html += "</ul>";
        }

        document.getElementById(
            "importResultArea"
        ).style.display = "";

        document.getElementById(
            "importResult"
        ).innerHTML = html;
    }
    catch (ex) {
        showMessage(
            "錯誤",
            ex.message
        );
    }
    finally {
        document.getElementById(
            "btnImportSubmit"
        ).disabled = false;

        document.getElementById(
            "btnImportCancel"
        ).disabled = false;

        document.getElementById(
            "importProgressArea"
        ).style.display = "none";
    }
}


// ========================
// SignalR 匯入進度
// ========================

let connection = null;

document.addEventListener(
    "DOMContentLoaded",
    async function () {
        if (typeof signalR === "undefined") {
            console.log(
                "SignalR js 未載入"
            );
            return;
        }

        connection =
            new signalR.HubConnectionBuilder()
                .withUrl("/memberImportHub")
                .build();

        connection.on(
            "ImportProgress",
            function (data) {
                console.log(data);
                const percent =
                    Math.floor(
                        data.current * 100 /
                        data.total
                    );

                document.getElementById(
                    "importProgressBar"
                ).style.width =
                    percent + "%";

                document.getElementById(
                    "importProgressBar"
                ).innerHTML =
                    percent + "%";

                document.getElementById(
                    "importProgressText"
                ).innerHTML =
                    `${data.current} / ${data.total}`;
            });

        try {
            await connection.start();

            console.log(
                "SignalR Connected"
            );
        }
        catch (err) {
            console.error(err);
        }
    });



// 顯示檢視會員 modal
let viewMemberModal;
document.addEventListener("DOMContentLoaded", function () {
    // 初始化 Bootstrap Modal 實例
    const modalElement = document.getElementById("viewMemberModal");
    if (modalElement) {
        viewMemberModal = new bootstrap.Modal(modalElement);
    }
});
async function viewMember(sid) {
    if (!sid) {
        console.error("無效的會員識別碼 (sid)");
        return;
    }

    try {
        // 1. 發送非同步請求到後端 Controller 取得會員詳細資料
        const response = await fetch(`/Member/GetMemberDetail?sid=${encodeURIComponent(sid)}`, {
            method: 'GET',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP 錯誤！狀態碼: ${response.status}`);
        }

        const result = await response.json();

        // 2. 判斷後端回傳結果是否成功
        if (result.success && result.data) {
            const data = result.data;

            // 3. 將資料動態填入 viewMemberForm 內的各個唯讀欄位
            document.querySelector('#viewMemberForm [name="ViewMember.Mid"]').value = data.mid || '';
            document.querySelector('#viewMemberForm [name="ViewMember.ParentMid"]').value = data.parentMid || '';
            document.querySelector('#viewMemberForm [name="ViewMember.Name"]').value = data.name || '';
            document.querySelector('#viewMemberForm [name="ViewMember.Mobile"]').value = data.mobile || '';
            document.querySelector('#viewMemberForm [name="ViewMember.Email"]').value = data.email || '';
            document.querySelector('#viewMemberForm [name="ViewMember.IdNumber"]').value = data.idNumber || '';

            // 處理日期格式 (將 yyyy-MM-ddTHH:mm:ss 切出前 10 碼 yyyy-MM-dd 以符合 HTML5 date 格式)
            if (data.joinDate) {
                document.querySelector('#viewMemberForm [name="ViewMember.JoinDate"]').value = data.joinDate.split('T')[0];
            } else {
                document.querySelector('#viewMemberForm [name="ViewMember.JoinDate"]').value = '';
            }

            // --- 處理到期日與背景顏色動態調整 ---
            const continueDateInput = document.querySelector('#viewMemberForm [name="ViewMember.ContinueDate"]');

            // 先清除上一次可能殘留的背景與文字顏色設定
            continueDateInput.style.backgroundColor = '';
            continueDateInput.style.color = '';

            if (data.continueDate) {
                const dateString = data.continueDate.split('T')[0];
                continueDateInput.value = dateString;

                // 計算天數差 (到期日 - 今天)
                const today = new Date();
                today.setHours(0, 0, 0, 0); // 將時間去除，只比對日期

                const expireDate = new Date(dateString);
                expireDate.setHours(0, 0, 0, 0);

                // 計算兩者相差的毫秒數並換算成天數
                const diffTime = expireDate.getTime() - today.getTime();
                const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

                // 根據天數差判斷底色邏輯：
                if (diffDays <= 0) {
                    // 當天或已過期 (>=今天則為紅色底)
                    continueDateInput.style.backgroundColor = '#dc3545'; // Bootstrap text-danger 紅
                    continueDateInput.style.color = '#ffffff';            // 白色字
                } else if (diffDays <= 3) {
                    // 距離今天 3 天內 (包含第3天) 則為橘色底
                    continueDateInput.style.backgroundColor = '#fd7e14'; // Bootstrap orange 橘
                    continueDateInput.style.color = '#ffffff';            // 白色字
                } else if (diffDays <= 30) {
                    // 距離今天 30 天內 (包含第30天) 則為黃色底
                    continueDateInput.style.backgroundColor = '#ffc107'; // Bootstrap text-warning 黃
                    continueDateInput.style.color = '#212529';            // 深色字
                }
                // 超過 30 天則維持原本 input 預設的白底黑字

            } else {
                continueDateInput.value = '';
            }

            // 填入會員角色中文名稱
            if (data.roleName) {
                document.querySelector('#viewMemberForm [name="ViewMember.Role"]').value = data.roleName;
            } else {
                document.querySelector('#viewMemberForm [name="ViewMember.Role"]').value = '';
            }

            // 4. 開啟檢視視窗
            if (viewMemberModal) {
                viewMemberModal.show();
            }
        } else {
            alert(result.message || "無法取得會員詳細資料");
        }
    } catch (error) {
        console.error("讀取會員資料時發生異常:", error);
        alert("系統發生錯誤，請稍後再試。");
    }
}