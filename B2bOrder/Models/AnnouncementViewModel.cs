using System.ComponentModel.DataAnnotations;

namespace B2bOrder.Models
{
    public class AnnouncementViewModel
    {
        // 實例化新增公告所需要的模型
        public CreateAnnouncementModel CreateAnnouncement { get; set; } = new CreateAnnouncementModel();

        // 另外兩個 Modal 用的 Model 可以依此類推
        public EditAnnouncementModel EditAnnouncement { get; set; } = new EditAnnouncementModel();
        public ViewAnnouncementModel ViewAnnouncement { get; set; } = new ViewAnnouncementModel();
    }

    public class CreateAnnouncementModel
    {
        public string CreatorSid { get; set; } = string.Empty; 

        [Display(Name = "發布單位")]
        public string PublishUnit { get; set; } = string.Empty;

        [Required(ErrorMessage = "請選擇公告分類")]
        [Display(Name = "公告分類")]
        public string Category { get; set; } = "General";

        [Required(ErrorMessage = "請輸入公告標題")]
        [StringLength(100, ErrorMessage = "標題長度不能超過 100 個字")]
        [Display(Name = "公告標題")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入公告內容")]
        [Display(Name = "公告內容")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "請選擇上架日期")]
        [DataType(DataType.Date)]
        [Display(Name = "上架日期")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "請選擇下架日期")]
        [DataType(DataType.Date)]
        [Display(Name = "下架日期")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);

        [Display(Name = "是否置頂")]
        public bool IsTop { get; set; }

        [Display(Name = "同步發送推播")]
        public bool SendPushNotification { get; set; }
    }
    public class EditAnnouncementModel
    {
        [Required]
        public string Sid { get; set; } = string.Empty; // 用於辨識編輯的公告主鍵

        [Display(Name = "發布單位")]
        public string PublishUnit { get; set; } = string.Empty;

        [Required(ErrorMessage = "請選擇公告分類")]
        [Display(Name = "公告分類")]
        public string Category { get; set; } = "General";

        [Required(ErrorMessage = "請輸入公告標題")]
        [StringLength(100, ErrorMessage = "標題長度不能超過 100 個字")]
        [Display(Name = "公告標題")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入公告內容")]
        [Display(Name = "公告內容")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "請選擇上架日期")]
        [DataType(DataType.Date)]
        [Display(Name = "上架日期")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "請選擇下架日期")]
        [DataType(DataType.Date)]
        [Display(Name = "下架日期")]
        public DateTime EndDate { get; set; }

        [Display(Name = "是否置頂")]
        public bool IsTop { get; set; }
    }
    public class ViewAnnouncementModel
    {
        public string Sid { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "發布日期")]
        public DateTime PublishDate { get; set; }

        [Display(Name = "發布單位")]
        public string PublishUnit { get; set; } = string.Empty;

        [Display(Name = "公告分類")]
        public string Category { get; set; } = string.Empty; // 後端傳回時可直接對應中文（如：系統維護）

        [Display(Name = "公告標題")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "公告內容")]
        public string Content { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "上架日期")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "下架日期")]
        public DateTime EndDate { get; set; }

        [Display(Name = "點閱次數")]
        public int ClickCount { get; set; } = 0;
    }
}