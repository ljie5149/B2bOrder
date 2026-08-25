using B2bOrder.Data;
using B2bOrder.Models.api;
using B2bOrder.Models.Common;
using B2bOrder.Services.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardApiController : BaseApiController
    {
        private readonly ApplicationDbContext _db;


        public DashboardApiController(ApplicationDbContext db, IJwtService jwtService, IEncryptionService encryptionService) : base(jwtService, encryptionService)
        {
            _db = db;
        }

        /// <summary>
        /// 取得會員成長圖表數據 (優化版：使用記憶體內搜尋，免除 N+1 遞迴)
        /// </summary>
        /// <param name="months">1: 週數據, 6: 6個月, 12: 12個月</param>
        [HttpGet("growth/{months:int}")]
        public async Task<IActionResult> GetMemberGrowthData(int months)
        {
            try
            {
                // 取得 RawToken，這是從 BaseApiController 繼承而來的屬性
                var token = RawToken;

                // 1. 調用基底類別方法，取得驗證後的 memberSid
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                // 1. 檢查目前登入會員是否存在
                var loginMemberExists = await _db.DataMembers.AnyAsync(x => x.Sid == memberSid && x.Avalible == "Y");
                if (!loginMemberExists)
                {
                    return NotFound(ApiResponse<object>.Fail("會員不存在或已被停用", "0x0404"));
                }

                // 2. 一次性撈出所有可用成員 (與 Index 的高效能優化邏輯一致)
                var allTeamMembers = await _db.DataMembers
                    .Where(x => x.Avalible == "Y")
                    .Select(x => new { x.Sid, x.ParentSid, x.Role, x.JoinDate })
                    .ToListAsync();

                var lookup = allTeamMembers.ToLookup(x => x.ParentSid);

                // 3. BFS 找出所有下線成員
                var currentLevelSids = new List<string> { memberSid };
                var subMembers = new List<dynamic>();

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
                            }
                        }
                    }
                    currentLevelSids = nextLevelSids;
                }

                // 4. 根據參數計算時間區間並分流資料
                var labels = new List<string>();
                var generalData = new List<int>();
                var distributorData = new List<int>();
                DateTime now = DateTime.Today;

                if (months == 1)
                {
                    labels = new List<string> { "第一週", "第二週", "第三週", "第四週" };

                    for (int i = 3; i >= 0; i--)
                    {
                        DateTime start = now.AddDays(-(i + 1) * 7);
                        DateTime end = now.AddDays(-i * 7);

                        var weekMembers = subMembers.Where(x => x.JoinDate >= start && x.JoinDate < end).ToList();

                        generalData.Add(weekMembers.Count(x => x.Role == null || !((string)x.Role).Contains("Smn")));
                        distributorData.Add(weekMembers.Count(x => x.Role != null && ((string)x.Role).Contains("Smn")));
                    }
                }
                else
                {
                    int limitMonths = (months == 6) ? 6 : 12;

                    for (int i = limitMonths - 1; i >= 0; i--)
                    {
                        DateTime targetMonth = now.AddMonths(-i);
                        labels.Add($"{targetMonth:MM}月");

                        var monthMembers = subMembers.Where(x => x.JoinDate != null &&
                                                             ((DateTime)x.JoinDate).Year == targetMonth.Year &&
                                                             ((DateTime)x.JoinDate).Month == targetMonth.Month).ToList();

                        generalData.Add(monthMembers.Count(x => x.Role == null || !((string)x.Role).Contains("Smn")));
                        distributorData.Add(monthMembers.Count(x => x.Role != null && ((string)x.Role).Contains("Smn")));
                    }
                }

                // 5. 包裝成圖表專用的資料結構物件，傳入 Success 的 Data 中
                var chartData = new
                {
                    labels = labels,
                    generalData = generalData,
                    distributorData = distributorData
                };

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(chartData), "取得會員成長數據成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(chartData, "取得會員成長數據成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }
    }
}