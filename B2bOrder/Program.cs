using B2bOrder.Data;
using B2bOrder.Data.ShareCore;
using B2bOrder.Hubs;
using B2bOrder.Models;
using B2bOrder.Repositories;
using B2bOrder.Responsitories;
using B2bOrder.Services;
using B2bOrder.Services.Common;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using System.Data;
using System.Reflection;
using static B2bOrder.Resources.DatabaseSql;


bool fAlwaysSwagger = true; // 強制在任何環境都啟用 Swagger（開發或生產）
bool LockToSwagger  = false;

var builder = WebApplication.CreateBuilder(args);

// ====== 修改：啟動時檢查 FirebaseApp 是否已初始化，若未初始化則從設定檔讀取憑證路徑並建立 FirebaseApp 實例 ======
// 讀取 Firebase 憑證路徑
var credentialsPath = builder.Configuration["Firebase:CredentialsPath"]
                      ?? "key/firebase-service-account.json";

var fullPath = Path.Combine(builder.Environment.ContentRootPath, credentialsPath);

if (File.Exists(fullPath))
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(fullPath)
    });
}
else
{
    // 提醒：請確保金鑰檔案存在於指定路徑
    Console.WriteLine($"[Warning] Firebase 憑證檔案不存在：{fullPath}");
}

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 💡 新增這行：註冊 HttpContextAccessor 服務，解決 InvalidOperationException 錯誤
builder.Services.AddHttpContextAccessor();

// 讀取版本：優先使用 appsettings.json 的 App:Version，否則退回到 assembly 資訊
var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
var version = builder.Configuration.GetValue<string>("App:Version")
              ?? assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
              ?? assembly.GetName().Version?.ToString()
              ?? "0.0.0";

// 把版本注入 DI，方便 Razor 頁面或 Controller 使用
builder.Services.AddSingleton(new AppInfo { Version = version });

// 註冊 Swagger -> Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "B2bOrder API",
        Version = "v1",
        Description = "API for member management and notifications"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// 讀取 ConnectionString
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var masterConnectionString = builder.Configuration.GetConnectionString("MasterConnection");
var mdmConnectionString = builder.Configuration.GetConnectionString("MdmConnection");

// 1. 業務 Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. MasterDB Context
builder.Services.AddDbContext<MasterDbContext>(options =>
    options.UseMySql(masterConnectionString, ServerVersion.AutoDetect(masterConnectionString)));

// 3. MDMDB Context
builder.Services.AddDbContext<MdmDbContext>(options =>
    options.UseMySql(mdmConnectionString, ServerVersion.AutoDetect(mdmConnectionString)));

// 宣告資料庫健康狀態狀態物件
var dbHealth = new DatabaseHealth { IsDatabaseConnectOK = true, IsDatabaseAvailable = true };
ServerVersion serverVersion = null;

try
{
    // 嘗試偵測資料庫版本（如果 DB 沒開，這裡會直接拋出例外）
    serverVersion = ServerVersion.AutoDetect(connectionString);
}
catch (Exception ex)
{
    using var loggerFactory = LoggerFactory.Create(lb => lb.AddConsole());
    var logger = loggerFactory.CreateLogger("Startup");
    logger.LogWarning(ex, "MySQL connection auto-detect failed於啟動階段。將於頁面提示資料庫不可用。");

    dbHealth.IsDatabaseConnectOK = false;

    // Fallback: 假定一個預設版本以利 DbContext 註冊，避免 DI 容器在建立時崩潰，但實際上不啟用 InMemory DB 混淆行為
    serverVersion = new MySqlServerVersion(new Version(8, 0, 35));
}

// 註冊 ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

// 註冊 DatabaseHealth 供 middleware 與頁面檢查
builder.Services.AddSingleton(dbHealth);

// Authentication (Cookie)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Member/Login";
        options.LogoutPath = "/Member/Logout";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// progress update with SignalR
builder.Services.AddSignalR();

// Repositories & Services
// --- repositories ---
builder.Services.AddScoped<IDataMemberRepository, DataMemberRepository>();
builder.Services.AddScoped<ISysUserRepository, SysUserRepository>();
builder.Services.AddScoped<IDataMemberFileRepository, DataMemberFileRepository>();
// 註冊 SysConfig Repository
builder.Services.AddScoped<ISysConfigRepository, SysConfigRepository>();
// 系統公告服務
builder.Services.AddScoped<INoticeRepository, NoticeRepository>();
builder.Services.AddScoped<IDataNoticeReadRepository, DataNoticeReadRepository>();
// 註冊待辦行事曆 Repository
builder.Services.AddScoped<ITodoRepository, TodoRepository>();

// --- services ---
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ISysUserService, SysUserService>();
builder.Services.AddScoped<IDataMemberFileService, DataMemberFileService>();
builder.Services.AddScoped<IMemberExpireNoticeService, MemberExpireNoticeService>();
builder.Services.AddScoped<ISysConfigService, SysConfigService>();
builder.Services.AddScoped<ISysNoticeSettingService, SysNoticeSettingService>();
builder.Services.AddScoped<ILogPushService, LogPushService>();
builder.Services.AddScoped<ILogImportService, LogImportService>();
builder.Services.AddScoped<ILogExportService, LogExportService>();
builder.Services.AddScoped<ILogActionService, LogActionService>();
builder.Services.AddScoped<IFCMService, FCMService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ILineService, LineService>();
// 註冊 SysConfig Service
builder.Services.AddScoped<ISysConfigService, SysConfigService>();
// 系統公告服務
builder.Services.AddScoped<INoticeService, NoticeService>();
// 註冊 Service
builder.Services.AddScoped<ITodoService, TodoService>();

// 註冊 JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

// 註冊密碼雜湊器 (用於無 ASP.NET Identity 的情況)
builder.Services.AddSingleton<IPasswordHasher<Member>, PasswordHasher<Member>>();
builder.Services.AddSingleton<IPasswordHasher<B2bOrder.Models.Db.SysUser>, PasswordHasher<B2bOrder.Models.Db.SysUser>>();

// 【新增】註冊加解密服務
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();

//// 取得台灣時間服務
//builder.Services.AddScoped<ITimeZoneService, TimeZoneService>();

// HttpClient for FCM
builder.Services.AddHttpClient();

// Hosted service for expiry notifications
builder.Services.AddHostedService<NotificationHostedService>();

var app = builder.Build();

// progress update with SignalR
app.MapHub<MemberImportHub>("/memberImportHub");

bool fAutoCreateDataBase = true; // 自動建立資料庫（開發環境專用，生產環境請確保資料庫已存在）
// ====== 修改：啟動後立即檢查資料庫是否可連線，若不可且 fAutoCreateDataBase==true，嘗試建立資料庫與 data_member 表 ======
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetService<ILoggerFactory>();
    var logger = loggerFactory?.CreateLogger("Startup");

    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();

        if (db.Database.CanConnect()) dbHealth.IsDatabaseAvailable = true;
        if (!fAutoCreateDataBase)
        {
            dbHealth.IsDatabaseAvailable = false;
            logger?.LogWarning("資料庫不可連線。fAutoCreateDataBase=false，將顯示 DatabaseUnavailable 頁面。");
        }
        else
        {
            logger?.LogWarning("資料庫不可連線，fAutoCreateDataBase=true，開始嘗試建立資料庫與 data_member 表...");

            try
            {
                // 先以 MySqlConnectionStringBuilder 取得資料庫名稱並建立資料庫（若不存在）
                var csBuilder = new MySqlConnectionStringBuilder(connectionString);
                var dbName = csBuilder.Database ?? string.Empty;
                if (string.IsNullOrWhiteSpace(dbName))
                {
                    throw new InvalidOperationException("Connection string 未包含資料庫名稱（Database）。無法自動建立資料庫。");
                }

                // 以不帶 Database 的連線字串連接 MySQL 伺服器，建立 database
                csBuilder.Database = string.Empty;
                var serverOnlyCs = csBuilder.ConnectionString;

                using (var serverConn = new MySqlConnection(serverOnlyCs))
                {
                    serverConn.Open();
                    using var cmd = serverConn.CreateCommand();
                    cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS `{dbName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
                    cmd.ExecuteNonQuery();
                    logger?.LogInformation("資料庫 `{Db}` 已存在或建立完成。", dbName);
                }

                // 重新嘗試連線並套用 migration（若有）或直接建立 table
                try
                {
                    db.Database.Migrate();
                    logger?.LogInformation("套用 EF migration 成功。");
                    dbHealth.IsDatabaseAvailable = db.Database.CanConnect();

                    if (!String.IsNullOrEmpty(CreateTables))
                    {
                        // 如果 table 已存在則不執行整個 SQL 檔（避免 CREATE INDEX 重覆錯誤）
                        bool tableExists = TableExists(db, "data_member", connectionString, logger);
                        if (!tableExists)
                        {
                            db.Database.ExecuteSqlRaw(CreateTables);
                            logger?.LogInformation("SQL 建表檔案已執行");
                        }
                        else
                        {
                            logger?.LogInformation("資料表 data_member 已存在，已跳過執行建立資料表");
                        }
                    }
                    else
                    {
                        logger?.LogWarning("建表 SQL 為空指令。若需自動建立表請將 加入專案並設定。");
                    }
                }
                catch (Exception migEx)
                {
                    logger?.LogWarning(migEx, "套用 migration 失敗，改以執行建表 SQL 建立 data_member 表。");
                    logger?.LogInformation("data_member 表已建立或存在。");
                    dbHealth.IsDatabaseAvailable = db.Database.CanConnect();
                }
            }
            catch (Exception createEx)
            {
                dbHealth.IsDatabaseAvailable = false;
                logger?.LogError(createEx, "自動建立資料庫或 data_member 表時發生錯誤。");
            }
        }
    }
    catch (Exception ex)
    {
        dbHealth.IsDatabaseAvailable = false;
        logger?.LogError(ex, "檢查或建立資料庫時發生錯誤");
    }
}

// Helper: 檢查資料表是否存在（使用 information_schema.tables）
static bool TableExists(ApplicationDbContext db, string tableName, string connectionString, ILogger logger = null)
{
    try
    {
        var conn = db.Database.GetDbConnection();
        var shouldClose = conn.State == ConnectionState.Closed;
        if (shouldClose) conn.Open();

        using var cmd = conn.CreateCommand();
        var csBuilder = new MySqlConnectionStringBuilder(connectionString);
        var dbName = csBuilder.Database ?? string.Empty;

        cmd.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = @schema AND table_name = @table;";
        var pSchema = cmd.CreateParameter();
        pSchema.ParameterName = "@schema";
        pSchema.Value = dbName;
        cmd.Parameters.Add(pSchema);

        var pTable = cmd.CreateParameter();
        pTable.ParameterName = "@table";
        pTable.Value = tableName;
        cmd.Parameters.Add(pTable);

        var result = cmd.ExecuteScalar();
        var count = Convert.ToInt32(result ?? 0);

        if (shouldClose) conn.Close();

        return count > 0;
    }
    catch (Exception ex)
    {
        logger?.LogWarning(ex, "在檢查資料表存在性時發生錯誤，預設回傳 false: {Table}", tableName);
        return false;
    }
}
// =====================================================================================

// ==========================================
// Configure the HTTP request pipeline.
// ==========================================
if (app.Environment.IsDevelopment() && fAlwaysSwagger)
//if (fAlwaysSwagger)
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "B2bOrder API V1");
        c.RoutePrefix = "swagger";
    });

    // 新增：當存取根目錄 "/" 時，自動導向到 "/swagger"
    if (LockToSwagger)
    {
        app.MapGet("/", async context =>
        {
            context.Response.Redirect("/swagger");
            await Task.CompletedTask;
        });
    }
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 【Middleware 1】啟動時資料庫不可用的攔截（改為在每個 request 發生時短暫重試連線）
app.Use(async (context, next) =>
{
    var health = context.RequestServices.GetService<DatabaseHealth>();
    if (health != null && !health.IsDatabaseAvailable)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // 排除靜態資源、swagger、api、dbhealth、以及 DatabaseUnavailable 本身
        var isStatic = path.Contains('.') || path.StartsWith("/_framework") || path.StartsWith("/lib") ||
                       path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/images");
        var isSwagger = path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase);
        var isApi = path.StartsWith("/api", StringComparison.OrdinalIgnoreCase);
        var isDbHealth = path.Equals("/dbhealth", StringComparison.OrdinalIgnoreCase);
        var isTargetPage = path.StartsWith("/DatabaseUnavailable", StringComparison.OrdinalIgnoreCase)
                           || path.StartsWith("/Home/DatabaseUnavailable", StringComparison.OrdinalIgnoreCase);

        if (!isTargetPage && !isSwagger && !isApi && !isStatic && !isDbHealth)
        {
            // 嘗試再次短暫檢查資料庫（避免每次都做重試造成大量開銷）
            try
            {
                using var scope = context.RequestServices.CreateScope();
                var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                if (db != null)
                {
                    // 快速檢查一次，如果成功就更新全域狀態並放行
                    if (db.Database.CanConnect())
                    {
                        health.IsDatabaseAvailable = true;
                        await next();
                        return;
                    }
                }
            }
            catch
            {
                // 忽略例外，維持不可用狀態
            }

            // 尚不可用：導向 DatabaseUnavailable 頁面
            context.Response.Redirect("/Home/DatabaseUnavailable");
            return;
        }
    }

    await next();
});

// 【Middleware 2】執行期（Runtime）資料庫斷線攔截 (捕捉 Try-Catch 異常)
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex) when ((ex.GetType().FullName ?? string.Empty).Contains("MySql") 
                               || (ex.InnerException?.GetType().FullName ?? string.Empty).Contains("MySql"))
    {
        var logger = context.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("DatabaseMiddleware");
        logger?.LogError(ex, "Database connection failed at runtime.");

        var redirectPath = "/Home/DatabaseUnavailable";
        // 使用 StartsWithSegments 比較 PathString 的情況
        if (!context.Response.HasStarted && !context.Request.Path.StartsWithSegments(new PathString(redirectPath), StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Redirect(redirectPath);
            return;
        }

        throw;
    }
});

// 路由與身分驗證群組
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 支援 attribute routes（API controller）與傳統 MVC route
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// 簡單診斷 endpoint
app.MapGet("/dbhealth", (DatabaseHealth dh) => Results.Json(new { dh.IsDatabaseAvailable }));

app.Run();