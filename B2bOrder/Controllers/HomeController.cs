using B2bOrder.Data;
using B2bOrder.Models;
using B2bOrder.Models.Db;
using B2bOrder.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace B2bOrder.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db; // 註：您的 DbContext 命名較特別，維持原本注入與使用

        public HomeController(
            ILogger<HomeController> logger,
            IMemberService memberService,
            ApplicationDbContext db)
        {
            _memberService = memberService;
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // 需登入才可看到儀表板
            if (User?.Identity?.IsAuthenticated != true)
            {
                var returnUrl = Url.Action(nameof(Index), "Home");
                return RedirectToAction("Login", "Member", new { returnUrl });
            }

            try
            {
                var vm = new HomeDashboardViewModel();
                var memberSid = User.FindFirst("member_sid")?.Value;
                var login_member = await _db.DataMembers.Where(x => x.Sid == memberSid && x.Avalible == "Y").ToListAsync();

                if (login_member == null || !login_member.Any())
                {
                    return RedirectToAction("Login", "Member");
                }
                vm.MemberName = login_member[0].Name ?? "";

                // 💡 【新增】從後台資料表撈取最新公告 (範例預設抓最新 3 筆，依建立或發布時間倒序排列)
                // 註：欄位名稱（如 DataNotices、Avalible、PublishDate）請根據您實作的實體模型 (Entity) 做對應調整
                if (_db.DataNotices != null)
                {
                    vm.LatestNotices = await _db.DataNotices
                        .Where(x => x.Avalible == "Y") // 僅撈取啟用的公告
                        .OrderByDescending(x => x.PublishDate) // 依據發布日期倒序
                        .Take(3) // 畫面上顯示最新 3 筆
                        .Select(x => new ViewAnnouncementModel
                        {
                            Sid = x.Sid,
                            Title = x.Title ?? "",
                            Content = x.Content ?? "",
                            Category = x.Category ?? "一般",
                            PublishDate = x.PublishDate,
                            PublishUnit = x.PublishUnit ?? "系統管理端"
                        })
                        .ToListAsync();
                }

                // 1. 一次性撈出該使用者旗下的所有可用下線（包含自己），避免迴圈查資料庫
                var allTeamMembers = await _db.DataMembers
                    .Where(x => x.Avalible == "Y")
                    .Select(x => new { x.Sid, x.ParentSid, x.Role, x.JoinDate, x.ContinueDate, x.Name, x.Mid, x.HintDays })
                    .ToListAsync();

                // 2. 建立查表字典 (Key: ParentSid, Value: 下線清單)，加速記憶體內搜尋
                var lookup = allTeamMembers.ToLookup(x => x.ParentSid);

                // 3. 計算團隊總人數與篩選 (排除自己，僅計算從自己延伸出去的組織)
                var currentLevelSids = new List<string> { memberSid };
                var levelIndex = 1;

                // 存放所有下線的資料清單
                var subMembers = new List<dynamic>();

                // 儲存各層人數
                int l1 = 0, l2 = 0, l3 = 0, l4 = 0, l5Plus = 0;

                // 利用廣度優先搜尋層層遞迴
                while (currentLevelSids.Any())
                {
                    var nextLevelSids = new List<string>();
                    foreach (var parentSid in currentLevelSids)
                    {
                        if (lookup.Contains(parentSid))
                        {
                            foreach (var child in lookup[parentSid])
                            {
                                nextLevelSids.Add(child.Sid);
                                subMembers.Add(child);

                                // 依據目前所在的代數分流計數
                                switch (levelIndex)
                                {
                                    case 1: l1++; break;
                                    case 2: l2++; break;
                                    case 3: l3++; break;
                                    case 4: l4++; break;
                                    default: l5Plus++; break; // 5層含以上
                                }
                            }
                        }
                    }
                    currentLevelSids = nextLevelSids;
                    levelIndex++;
                }

                // 將計算好的層級分佈數據塞入 ViewModel
                vm.Level1Count = l1;
                vm.Level2Count = l2;
                vm.Level3Count = l3;
                vm.Level4Count = l4;
                vm.Level5PlusCount = l5Plus;


                // 將登入者自己也納入統計標的（若需要含登入者本身）
                var targetMembers = new List<dynamic>(subMembers);
                var selfObj = allTeamMembers.FirstOrDefault(x => x.Sid == memberSid);
                if (selfObj != null)
                {
                    targetMembers.Add(selfObj);
                }

                // 3. 計算即將到期與已到期數量
                var today = DateTime.Today;

                // 3. 統計轄下所有 1~n 層會員的到期數量
                try
                {
                    vm.ExpireSoonCount = targetMembers.Count(x =>
                        x.ContinueDate != null &&
                        (x.ContinueDate - today).Days >= 0 &&
                        (x.ContinueDate - today).Days <= x.HintDays);
                }
                catch { }
                try
                {
                    vm.ExpiredCount = targetMembers.Count(x =>
                        x.ContinueDate != null &&
                        (x.ContinueDate - today).Days < 0);
                }
                catch { }


                // 4. 計算首頁其他卡片數值
                vm.TotalMembers = subMembers.Count;
                vm.DirectMembers = subMembers.Count(x => x.Role == "Smn");
                vm.RegularMembers = subMembers.Count(x => x.Role == "Stc");

                // 5. 最新加入會員 (前 10 筆)
                vm.LatestMembers = subMembers
                    .OrderByDescending(x => x.JoinDate)
                    .Take(10)
                    .Select(x => new HomeDashboardViewModel.LatestMemberDto
                    {
                        Name = x.Name ?? "",
                        Mid = x.Mid,
                        Role = x.Role == "Smn" ? "直銷商" : "一般會員",
                        JoinDate = x.JoinDate
                    })
                    .Cast<HomeDashboardViewModel.LatestMemberDto>()
                    .ToList();

                // 6. 最近 5 個月會員成長
                var growth = new List<HomeDashboardViewModel.MemberGrowthPoint>();
                for (int i = 4; i >= 0; i--)
                {
                    var date = DateTime.Today.AddMonths(-i);
                    var count = subMembers.Count(x =>
                        x.JoinDate != null &&
                        ((DateTime)x.JoinDate).Year == date.Year &&
                        ((DateTime)x.JoinDate).Month == date.Month);

                    growth.Add(new HomeDashboardViewModel.MemberGrowthPoint
                    {
                        Label = $"{date.Month}月",
                        Count = count
                    });
                }
                vm.MemberGrowth = growth;
                vm.ShowSalesAndBonus = false;

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Index() - 讀取儀表板資料失敗");
                return RedirectToAction(nameof(DatabaseUnavailable));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // 當資料庫不可用時轉到說明頁
        public IActionResult DatabaseUnavailable()
        {
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Title = "資料庫無法連線",
                Message = "系統目前無法連線到資料庫，請稍後再試或聯絡系統管理員。"
            };

            return View("Error", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> GetMemberGrowthData(int months)
        {
            var labels = new List<string>();
            var generalData = new List<int>();
            var distributorData = new List<int>();

            var memberSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrEmpty(memberSid))
            {
                return Json(new { success = false, message = "未登入" });
            }

            var login_member = await _memberService.GetBySidAsync(memberSid);
            if (login_member == null)
            {
                return Json(new { success = false, message = "會員不存在" });
            }

            var allMembers = new List<DataMember>();
            await GetAllDescendantsAsync(memberSid, allMembers);

            DateTime now = DateTime.Today;

            if (months == 1)
            {
                labels = new List<string> { "第一週", "第二週", "第三週", "第四週" };

                for (int i = 3; i >= 0; i--)
                {
                    DateTime start = now.AddDays(-(i + 1) * 7);
                    DateTime end = now.AddDays(-i * 7);

                    var weekMembers = allMembers.Where(x => x.JoinDate >= start && x.JoinDate < end).ToList();

                    generalData.Add(weekMembers.Count(x => x.Role == null || !x.Role.Contains("Smn")));
                    distributorData.Add(weekMembers.Count(x => x.Role != null && x.Role.Contains("Smn")));
                }
            }
            else
            {
                int limitMonths = (months == 6) ? 6 : 12;

                for (int i = limitMonths - 1; i >= 0; i--)
                {
                    DateTime targetMonth = now.AddMonths(-i);
                    labels.Add($"{targetMonth:MM}月");

                    var monthMembers = allMembers.Where(x => x.JoinDate != null &&
                                                             x.JoinDate.Value.Year == targetMonth.Year &&
                                                             x.JoinDate.Value.Month == targetMonth.Month).ToList();

                    generalData.Add(monthMembers.Count(x => x.Role == null || !x.Role.Contains("Smn")));
                    distributorData.Add(monthMembers.Count(x => x.Role != null && x.Role.Contains("Smn")));
                }
            }

            return Json(new
            {
                labels = labels,
                generalData = generalData,
                distributorData = distributorData
            });
        }

        private async Task GetAllDescendantsAsync(string parentSid, List<DataMember> resultList)
        {
            var directChildren = await _memberService.GetChildrenAsync(parentSid);
            if (directChildren != null && directChildren.Any())
            {
                foreach (var child in directChildren.Cast<DataMember>())
                {
                    resultList.Add(child);
                    await GetAllDescendantsAsync(child.Sid, resultList);
                }
            }
        }
    }
}