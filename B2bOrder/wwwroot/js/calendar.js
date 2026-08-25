/**
 * 待辦行事曆前端交互與後端 Ajax 同步邏輯
 */
// 宣告全域變數儲存 Bootstrap Modal 實例
var editTodoModal;

document.addEventListener('DOMContentLoaded', function () {
    var Calendar = FullCalendar.Calendar;
    var Draggable = FullCalendar.Draggable;

    var containerEl = document.getElementById('external-events');
    var calendarEl = document.getElementById('calendar');
    var calendar; // 將 calendar 提升為全域變數，以便動態更新

    // 取得「顯示已完成事項」的開關元件 [cite: 11]
    var showCompletedSwitch = document.getElementById('showCompleted');

    // 初始化編輯 Modal 實例
    const editModalEl = document.getElementById('editTodoModal');
    if (editModalEl) {
        editTodoModal = new bootstrap.Modal(editModalEl);
    }

    // 1. 初始化左側外部拖曳管理器
    new Draggable(containerEl, {
        itemSelector: '.todo-item',
        eventData: function (eventEl) {
            // 當未排程事項被拖進月曆時，FullCalendar 會讀取這些屬性來建立事件
            return {
                id: eventEl.getAttribute('data-id'),
                title: eventEl.getAttribute('data-title'),
                className: eventEl.getAttribute('data-classname')
            };
        }
    });

    // 2. 監聽「顯示已完成事項」開關切換邏輯
    if (showCompletedSwitch) {
        showCompletedSwitch.addEventListener('change', function () {
            // 當使用者切換開關時，重新載入並過濾清單
            loadTodoList();
        });
    }

    // 3. 從後端獲取最新待辦清單：/Calendar/GetTodoList
    function loadTodoList() {
        // 讀取當前開關狀態 (true = 顯示全部, false = 隱藏已完成)
        var showCompleted = showCompletedSwitch ? showCompletedSwitch.checked : true;

        fetch('/Calendar/GetTodoList')
            .then(response => response.json())
            .then(res => {
                if (res.success) {
                    // 清空左側容器
                    containerEl.innerHTML = '';

                    var scheduledEvents = [];
                    var unScheduledCount = 0; // 修正中文字元錯字

                    // 尋訪後端回傳的 DTO 清單
                    res.data.forEach(item => {
                        // 💡 核心過濾：如果該事項已完成，且使用者關閉了「顯示已完成事項」開關，則直接跳過不渲染
                        if (item.isDone && !showCompleted) {
                            return; 
                        }

                        // 不論原本是工作(黃)、生活(藍)，只要完成了，就強制疊加 event-completed 變成綠色
                        var finalClassName = item.className;
                        if (item.isDone) {
                            finalClassName += ' event-completed';
                        }

                        if (item.start) {
                            // A. 已排程事項：加入到 FullCalendar 的事件陣列中
                            scheduledEvents.push({
                                id: item.id,
                                title: item.title,
                                start: item.start,
                                end: item.end,
                                allDay: item.allDay,
                                className: finalClassName, // 使用處理後的樣式

                                // 💡 核心優化：如果該事項已完成，將 editable 設為 false，鎖定其在日曆上的拖拽與缩放
                                editable: !item.isDone
                            });
                        } else {
                            // B. 未排程事項：動態渲染至左側清單
                            unScheduledCount++;
                            renderLeftTodoItem(item);
                        }
                    });

                    // 更新左側的待辦計數標籤（#todo-count）
                    var countBadge = document.getElementById('todo-count');
                    if (countBadge) countBadge.innerText = unScheduledCount;

                    // 4. 載入資料完成後，初始化或重新整理 FullCalendar
                    initCalendar(scheduledEvents);
                } else {
                    console.error('載入待辦清單失敗：' + res.message);
                }
            })
            .catch(err => console.error('Error fetching todos:', err));
    }

    // 4. 動態生成左側未排程項目的 HTML 節點
    function renderLeftTodoItem(item) {
        var badgeClass = item.className === 'bg-work-event' ? 'badge-work' : (item.className === 'bg-life-event' ? 'badge-life' : 'badge-personal');
        var categoryText = item.className === 'bg-work-event' ? '工作事項' : (item.className === 'bg-life-event' ? '生活行程' : '個人專案');
        var dotColor = item.className === 'bg-work-event' ? 'text-danger' : (item.className === 'bg-life-event' ? 'text-warning' : 'text-success');

        var newItem = document.createElement('div');
        newItem.className = 'todo-item p-3 rounded d-flex align-items-center justify-content-between';
        
        // 💡 體驗優化：如果左側未排程事項是已完成狀態，加上覆蓋樣式與變暗
        var completedStyle = item.isDone ? 'opacity: 0.5; text-decoration: line-through;' : '';
        if (item.isDone) {
            newItem.classList.add('event-completed');
        }

        // 將後端待辦事項的 Sid 綁定至節點屬性，供 Draggable 操作抓取
        newItem.setAttribute('data-id', item.id);
        newItem.setAttribute('data-title', item.title);
        newItem.setAttribute('data-classname', item.className);

        // 統一體驗：點擊左側未排程右方的按鈕時，同樣彈出編輯 Modal
        // 若已完成則隱藏拖曳網格圖示，改顯示綠色勾勾
        newItem.innerHTML = `
            <div class="d-flex align-items-center gap-2 text-truncate" style="${completedStyle}">
                ${item.isDone ? '<i class="fa-solid fa-check text-success me-1"></i>' : '<i class="fa-solid fa-grip-vertical handle text-muted me-1"></i>'}
                <div class="text-truncate">
                    <div class="fw-bold text-dark mb-1" style="font-size: 13.5px;">
                        <span class="${dotColor} me-1">●</span>${item.title}
                    </div>
                    <span class="badge ${badgeClass} text-xs">${categoryText}</span>
                </div>
            </div>
            <div>
                <button class="btn btn-link text-muted p-1 small btn-trigger-modal">
                    <i class="fa-solid fa-ellipsis-vertical"></i>
                </button>
            </div>
        `;

        // 為內層按鈕單獨綁定點擊事件，避免影響外部 DOM 的 click 氣泡事件與拖拽
        newItem.querySelector('.btn-trigger-modal').addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            showEditTodoModal(item.id);
        });

        containerEl.appendChild(newItem);
    }

    // 5. 初始化或刷新 FullCalendar 實例
    function initCalendar(eventsList) {
        if (calendar) {
            // 修正關鍵 A：不要粗暴地去動 setOption('events')，否則會跟外部拖曳狀態衝突。
            // 我們直接透過 Event API 強力拔除目前畫面上渲染的所有事件節點，再逐一加入新事件。
            calendar.removeAllEvents();
            eventsList.forEach(function (ev) {
                calendar.addEvent(ev);
            });
            return;
        }

        calendar = new Calendar(calendarEl, {
            locale: 'zh-tw',
            initialView: 'dayGridMonth',
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay'
            },
            buttonText: {
                today: '今天', month: '月', week: '週', day: '日'
            },
            editable: true, // 全域允許編輯，但個別事件的 editable: false 會覆蓋此設定
            droppable: true,
            events: eventsList, // 第一次初始化的匿名事件源

            // 將 eventDidMount 改為 FullCalendar 原生的 eventClick (預設為滑鼠左鍵點擊)
            eventClick: function (info) {
                info.jsEvent.preventDefault(); // 阻擋可能存在的預設超連結行為
                var todoId = info.event.id;    // 取得該行程的 ID
                showEditTodoModal(todoId);     // 觸發撈取資料並開啟 Modal
            },

            // C. 當外部未排程事項（左側）成功拖曳丟入月曆中時觸發
            eventReceive: function (info) {
                var todoId = info.event.id;
                // 同步至後端資料庫
                updateEventTimeline(todoId, info.event.startStr, info.event.endStr, info.event.allDay);

                // 修正關鍵 B：將 info.revert(); 徹底移除！改用 info.event.remove(); 
                // 這會承認拖拽成功，但老子現在不需要這個前端臨時物件，直接把它從日曆中刪掉。
                // 這樣既不會觸發 FullCalendar 的狀態 Bug，又能保持畫面乾淨無殘影！
                info.event.remove();

                // 延遲重新整理列表，更新左側未排程數量與狀態
                setTimeout(loadTodoList, 300);
            },

            // D. 當行事曆內的事項被拖曳移動日期/時間時觸發
            eventDrop: function (info) {
                updateEventTimeline(info.event.id, info.event.startStr, info.event.endStr, info.event.allDay);
            },

            // E. 當行事曆內的事項被拉長或縮短行程時間時觸發
            eventResize: function (info) {
                updateEventTimeline(info.event.id, info.event.startStr, info.event.endStr, info.event.allDay);
            }
        });

        calendar.render();
    }

    // 6. 共用函式：發送 Ajax POST 到後端 /Calendar/UpdateTimeline 更新時間排程
    function updateEventTimeline(id, startIso, endIso, allDay) {
        var formData = new FormData();
        formData.append('id', id);
        formData.append('start', startIso || '');
        formData.append('end', endIso || '');
        formData.append('allDay', allDay);

        fetch('/Calendar/UpdateTimeline', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(res => {
                if (!res.success) alert('時間同步失敗：' + res.message);
            })
            .catch(err => console.error('Error updating timeline:', err));
    }

    // 7. 處理 Modal 視窗內「新增未排程事項」點擊事件
    document.getElementById('btnSaveTodo').addEventListener('click', function () {
        var title = document.getElementById('todoTitle').value.trim();
        var className = document.getElementById('todoCategory').value; // 取得像是 bg-work-event 的值

        if (!title) {
            alert('請輸入事項名稱');
            return;
        }

        var formData = new FormData();
        formData.append('title', title);
        formData.append('className', className);

        // 呼叫後端 API 新增
        fetch('/Calendar/Create', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(res => {
                if (res.success) {
                    // 表單清空並關閉視窗
                    document.getElementById('addTodoForm').reset();
                    var modalEl = document.getElementById('addTodoModal');
                    var modal = bootstrap.Modal.getInstance(modalEl);
                    modal.hide();

                    // 重新載入列表與行事曆
                    loadTodoList();
                } else {
                    alert('新增失敗：' + res.message);
                }
            })
            .catch(err => console.error('Error creating todo:', err));
    });

    /**
     * 9. 讀取特定待辦事項詳細資料並填入編輯 Modal (由左鍵點擊行事曆事件或左側按鈕觸發)
     */
    function showEditTodoModal(id) {
        fetch(`/Calendar/GetTodoDetail?id=${id}`)
            .then(response => response.json())
            .then(res => {
                if (res.success && res.data) {
                    // 將資料塞回編輯 Form 欄位
                    document.getElementById('editTodoId').value = res.data.id || res.data.sid;
                    document.getElementById('editTodoTitle').value = res.data.title;
                    document.getElementById('editTodoCategory').value = res.data.className;

                    // 判斷是否已完成 (兼顧 isDone 與 isCompleted 欄位名稱)
                    var isCompleted = res.data.isDone || res.data.isCompleted;

                    // 取得需要動態控制狀態的表單元件與按鈕
                    var inputTitle = document.getElementById('editTodoTitle');
                    var selectCategory = document.getElementById('editTodoCategory');
                    var btnUpdate = document.getElementById('btnUpdateTodo');
                    var btnComplete = document.getElementById('btnModalCompleteTodo');
                    var btnDelete = document.getElementById('btnModalDeleteTodo');
                    var modalTitle = document.querySelector('#editTodoModal .modal-title');

                    if (isCompleted) {
                        // 🔒 唯讀檢視模式：鎖定欄位
                        inputTitle.disabled = true;
                        selectCategory.disabled = true;

                        // 隱藏編輯、完成、刪除按鈕
                        btnUpdate.style.display = 'none';
                        btnComplete.style.display = 'none';
                        btnDelete.style.display = 'none';

                        // 變更 Modal 抬頭提示
                        modalTitle.innerHTML = '🔒 檢視待辦事項 (已完成)';
                    } else {
                        // 🔓 一般編輯模式：開放欄位
                        inputTitle.disabled = false;
                        selectCategory.disabled = false;

                        // 顯示操作按鈕
                        btnUpdate.style.display = 'inline-block';
                        btnComplete.style.display = 'inline-block';
                        btnDelete.style.display = 'inline-block';

                        // 還原標準抬頭
                        modalTitle.innerHTML = '✏️ 編輯待辦事項';
                    }

                    // 彈出 Bootstrap 編輯視窗
                    if (editTodoModal) {
                        editTodoModal.show();
                    }
                } else {
                    alert(res.message || '無法讀取該筆待辦事項資料');
                }
            })
            .catch(err => console.error('Error fetching single todo:', err));
    }

    /**
     * 10. 處理編輯 Modal 中的「儲存變更」按鈕點擊事件
     */
    document.getElementById('btnUpdateTodo').addEventListener('click', function () {
        // 安全防護：如果是唯讀狀態，阻擋點擊
        if (document.getElementById('editTodoTitle').disabled) return;

        var id = document.getElementById('editTodoId').value;
        var title = document.getElementById('editTodoTitle').value.trim();
        var className = document.getElementById('editTodoCategory').value;

        if (!title) {
            alert('請輸入事項名稱');
            return;
        }

        var formData = new FormData();
        formData.append('id', id);
        formData.append('title', title);
        formData.append('className', className);

        // 呼叫後端 API 儲存編輯內容
        fetch('/Calendar/Update', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(res => {
                if (res.success) {
                    // 關閉視窗
                    editTodoModal.hide();
                    // 重新整理行事曆與左側清單
                    loadTodoList();
                } else {
                    alert('更新失敗：' + res.message);
                }
            })
            .catch(err => console.error('Error updating todo:', err));
    });

    /**
     * 11. 處理編輯 Modal 中的「完成」按鈕點擊事件
     */
    document.getElementById('btnModalCompleteTodo').addEventListener('click', function () {
        if (document.getElementById('editTodoTitle').disabled) return;

        var id = document.getElementById('editTodoId').value;
        if (!id) return;

        var formData = new FormData();
        formData.append('id', id);
        formData.append('isDone', true);

        fetch('/Calendar/ToggleComplete', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(res => {
                if (res.success) {
                    editTodoModal.hide(); // 關閉 Modal
                    loadTodoList();       // 重新載入行事曆與清單 (自動套用目前開關的篩選狀態)
                } else {
                    alert('變更狀態失敗：' + res.message);
                }
            })
            .catch(err => console.error('Error toggling complete from modal:', err));
    });

    /**
     * 12. 處理編輯 Modal 中的「刪除」按鈕點擊事件
     */
    document.getElementById('btnModalDeleteTodo').addEventListener('click', function () {
        if (document.getElementById('editTodoTitle').disabled) return;

        var id = document.getElementById('editTodoId').value;
        if (!id) return;

        if (!confirm('確定要刪除此待辦事項嗎？')) return;

        var formData = new FormData();
        formData.append('id', id);

        fetch('/Calendar/Delete', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(res => {
                if (res.success) {
                    editTodoModal.hide(); // 關閉 Modal
                    loadTodoList();       // 重新載入行事曆與清單
                } else {
                    alert('刪除失敗：' + res.message);
                }
            })
            .catch(err => console.error('Error deleting todo from modal:', err));
    });

    // 頁面初始化載入
    loadTodoList();
});