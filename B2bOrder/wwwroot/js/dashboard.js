document.addEventListener("DOMContentLoaded", function () {
    // 1. 先安全地取得 Canvas 元素
    const canvasElement = document.getElementById('memberGrowthChart');

    // 🔴 關鍵防呆：如果畫面上找不到這個元件，直接收工不報錯！
    if (!canvasElement) {
        console.log("當前介面未渲染 Canvas 圖表元件，跳過 Chart.js 初始化。");
        return;
    }

    const ctx = canvasElement.getContext('2d');
    let memberChart = null; // 用來儲存 Chart 實例，方便後續 update

    // 🟢 核心：向 Controller 抓取資料並更新/建立圖表
    function updateChartFromServer(months) {
        // 對應你的 Controller Action 路由與參數
        const url = `/Home/GetMemberGrowthData?months=${months}`;

        fetch(url)
            .then(response => {
                if (!response.ok) throw new Error("網路請求失敗");
                return response.json();
            })
            .then(data => {
                // 如果圖表已經初始化過，直接更新數據（會有平滑動畫效果）
                if (memberChart) {
                    memberChart.data.labels = data.labels;
                    memberChart.data.datasets[0].data = data.generalData;
                    memberChart.data.datasets[1].data = data.distributorData;
                    memberChart.update();
                } else {
                    // 第一次載入，初始化 Chart.js
                    initChart(data.labels, data.generalData, data.distributorData);
                }
            })
            .catch(error => console.error("抓取會員成長資料失敗:", error));
    }

    // 🟢 封裝 Chart.js 初始化設定
    function initChart(labels, generalData, distributorData) {
        memberChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: '一般會員',
                        data: generalData,
                        backgroundColor: '#cbd5e1',
                        borderRadius: 6,
                        borderSkipped: false
                    },
                    {
                        label: '直銷商',
                        data: distributorData,
                        backgroundColor: '#0d6efd',
                        borderRadius: 6,
                        borderSkipped: false
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'top',
                        labels: {
                            boxWidth: 12,
                            font: { size: 12, weight: '500' }
                        }
                    },
                    tooltip: {
                        padding: 10,
                        cornerRadius: 6
                    }
                },
                scales: {
                    x: { grid: { display: false } },
                    y: {
                        beginAtZero: true,
                        grid: { color: '#f1f5f9' }
                    }
                }
            }
        });
    }

    // 🚀 【觸發點 1】網頁第一次載入時
    // 考慮到 else 區塊可能沒有 id，我們改用屬性或類別選擇器做相容
    // 如果你有把 else 的 select 加上 id="trendTimeRange"，這裡用 getElementById 即可
    const timeRangeSelect = document.getElementById('trendTimeRange') || document.querySelector('.card select');
    const defaultMonths = timeRangeSelect ? timeRangeSelect.value : 1;

    // 執行第一次撈取
    updateChartFromServer(defaultMonths);

    // 🚀 【觸發點 2】下拉選單切換時
    if (timeRangeSelect) {
        timeRangeSelect.addEventListener('change', function () {
            const value = this.value;
            console.log("切換時間範圍至: " + value + " 個月");

            // 動態跟後端要新範圍的資料
            updateChartFromServer(value);
        });
    }
});