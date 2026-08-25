//[cite: 1, 3]
const STORAGE_PREFIX = 'b2border:ui:';

// utility storage helpers
const storageSet = (key, value) => {
    try {
        localStorage.setItem(STORAGE_PREFIX + key, value);
    } catch (e) {
        // localStorage may be unavailable; silently ignore
    }
};

const storageGet = (key) => {
    try {
        return localStorage.getItem(STORAGE_PREFIX + key);
    } catch (e) {
        return null;
    }
};

// dropdown toggle with optional persistence
const toggleDropdown = (dropdown, menu, isOpen, persist = true) => {
    // keep compatibility with custom 'open' and bootstrap 'show'
    dropdown.classList.toggle("open", isOpen);
    dropdown.classList.toggle("show", isOpen);
    if (menu) {
        menu.classList.toggle("show", isOpen);
        menu.style.height = isOpen ? `${menu.scrollHeight}px` : 0;
    }

    if (persist) {
        // ensure dropdown has a stable data-key; fallback to index if not present
        let key = dropdown.getAttribute('data-key');
        if (!key) {
            key = 'dropdown-' + Array.from(document.querySelectorAll('.dropdown-container')).indexOf(dropdown);
            dropdown.setAttribute('data-key', key);
        }
        storageSet('dropdown:' + key, isOpen ? '1' : '0');
    }
}

// close all open dropdowns (and persist state)
const closeAllDropdowns = (persist = true) => {
    // handle both custom container and bootstrap dropdowns
    document.querySelectorAll(".dropdown-container.open, .dropdown.show").forEach(openDropdown => {
        const menu = openDropdown.querySelector(".dropdown-menu");
        // use toggleDropdown to keep persistence logic consistent when applicable
        toggleDropdown(openDropdown, menu, false, persist);
    });
}

// delegate click for dropdown toggles (single handler to avoid duplicate bindings)
document.addEventListener('click', (e) => {
    const toggle = e.target.closest('.dropdown-toggle');
    if (!toggle) return;

    // prevent default only for empty fragment hrefs
    if (toggle.getAttribute('href') === '#') e.preventDefault();

    const dropdown = toggle.closest(".dropdown-container, .dropdown");
    if (!dropdown) return; // defensive

    const menu = dropdown.querySelector(".dropdown-menu");
    if (!menu) return;

    const isOpen = dropdown.classList.contains("open") || dropdown.classList.contains("show");

    // close others and toggle current
    closeAllDropdowns();
    toggleDropdown(dropdown, menu, !isOpen);
});

const updateBodySidebarState = () => {
    const sidebar = document.querySelector(".sidebar");
    if (!sidebar) return;
    document.body.classList.toggle("sidebar-collapsed", sidebar.classList.contains("collapsed"));
};

// persist sidebar collapsed state
const setSidebarCollapsed = (collapsed) => {
    const sidebar = document.querySelector(".sidebar");
    if (!sidebar) return;
    sidebar.classList.toggle("collapsed", collapsed);
    updateBodySidebarState();
    storageSet('sidebarCollapsed', collapsed ? '1' : '0');
};

// restore sidebar collapsed from storage (if present)
const restoreSidebarState = () => {
    const val = storageGet('sidebarCollapsed');
    if (val === '1' || val === '0') {
        const collapsed = val === '1';
        const sidebar = document.querySelector(".sidebar");
        if (sidebar) {
            sidebar.classList.toggle("collapsed", collapsed);
            updateBodySidebarState();
        }
    }
};

document.querySelectorAll(".sidebar-toggler, .sidebar-menu-button").forEach(btn => {
    btn.addEventListener("click", () => {
        closeAllDropdowns();
        const sidebar = document.querySelector(".sidebar");
        if (!sidebar) return;
        const willBeCollapsed = !sidebar.classList.contains("collapsed");
        sidebar.classList.toggle("collapsed");
        // 同步更新 body 類別，讓 topbar/main-content 能透過 CSS 變數回應
        updateBodySidebarState();
        // persist
        storageSet('sidebarCollapsed', willBeCollapsed ? '1' : '0');
    });
});

// restore dropdown open states from storage
const restoreDropdownStates = () => {
    const dropdowns = Array.from(document.querySelectorAll('.dropdown-container'));
    dropdowns.forEach((dropdown, idx) => {
        // determine key (use existing data-key or assign fallback)
        let key = dropdown.getAttribute('data-key');
        if (!key) {
            key = 'dropdown-' + idx;
            dropdown.setAttribute('data-key', key);
        }

        const stored = storageGet('dropdown:' + key);
        const defaultOpen = dropdown.getAttribute('data-default-open') === 'true';
        const shouldOpen = stored === '1' ? true : (stored === '0' ? false : defaultOpen);

        const menu = dropdown.querySelector('.dropdown-menu');
        if (!menu) return;

        // open/close after layout so scrollHeight is correct
        if (shouldOpen) {
            // requestAnimationFrame to ensure CSS/layout ready
            requestAnimationFrame(() => toggleDropdown(dropdown, menu, true, /*persist*/ false));
        } else {
            // ensure closed
            toggleDropdown(dropdown, menu, false, /*persist*/ false);
        }
    });
};

// 如果頁面載入時側邊欄已經是 collapsed（例如伺服端預設），確保 body 狀態同步，並從 localStorage 還原使用者設定
document.addEventListener("DOMContentLoaded", () => {
    // restore sidebar first
    restoreSidebarState();

    // then restore dropdowns
    restoreDropdownStates();

    // 如果沒有 localStorage 設定但 server-side 設了 data-default-open，保留原有行為（舊程式碼相容）
    updateBodySidebarState();
});

// 阻止點擊 remember label 切換 checkbox（使用 pointerdown capture，在更早階段攔截）
// 修改：只在 label 明確標記為要被阻止時才攔截；預設讓標籤觸發 checkbox（例如登入頁的「記住我」）
(function () {
    const getRelatedInput = (lbl) => {
        if (!lbl) return null;
        // 1) 若 label 有 for，使用該 id 找 input
        const forId = lbl.getAttribute && lbl.getAttribute('for');
        if (forId) {
            return document.getElementById(forId);
        }
        // 2) 否則嘗試找到同一 .form-check 區塊中的 input
        const formCheck = lbl.closest && lbl.closest('.form-check');
        if (formCheck) {
            return formCheck.querySelector('input[type="checkbox"], input[type="radio"]');
        }
        // 3) fallback: 找附近的 input
        return (lbl.querySelector && lbl.querySelector('input[type="checkbox"], input[type="radio"]')) || null;
    };

    // 在 capture 階段攔截 pointerdown，僅當 label 被明確標記要阻止時，才阻止 label activation
    // 使用方式：在不想讓 label 切換 input 的 label 上加上 class="prevent-label-toggle" 或 data-prevent-toggle="true"
    document.addEventListener('pointerdown', function (e) {
        const targetEl = e.target instanceof Element ? e.target : null;
        const lbl = targetEl ? targetEl.closest('.form-check-label') : null;
        if (!lbl) return;

        const relatedInput = getRelatedInput(lbl);
        if (!relatedInput) return;

        const wantsPrevent = lbl.classList.contains('prevent-label-toggle') || lbl.getAttribute('data-prevent-toggle') === 'true';
        if (!wantsPrevent) {
            // 不攔截，讓瀏覽器預設行為執行（點擊 label 會切換 checkbox）
            return;
        }

        // 若標記為要阻止，才阻止預設與後續事件
        e.preventDefault();
        e.stopImmediatePropagation();
        e.stopPropagation();

        // 若需要，仍可手動控制 input 的切換並觸發 change（目前保留為註解）
        // relatedInput.checked = !relatedInput.checked;
        // relatedInput.dispatchEvent(new Event('change', { bubbles: true }));
    }, true);
})();

function getTodayDate() {
    const today = new Date();

    const yyyy =
        today.getFullYear();

    const mm =
        String(
            today.getMonth() + 1
        ).padStart(2, '0');

    const dd =
        String(
            today.getDate()
        ).padStart(2, '0');

    return `${yyyy}-${mm}-${dd}`;
}