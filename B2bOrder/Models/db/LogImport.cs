using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.Db
{
    [Table("log_import")]
    public class LogImport
    {
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("create_sid")]
        public string CreateSid { get; set; } = string.Empty;

        [Column("file_name")]
        public string FileName { get; set; } = string.Empty;

        [Column("file_type")]
        public string FileType { get; set; } = string.Empty;

        [Column("total_count")]
        public int TotalCount { get; set; }

        [Column("success_count")]
        public int SuccessCount { get; set; }

        [Column("fail_count")]
        public int FailCount { get; set; }

        [Column("remark")]
        public string? Remark { get; set; }
    }
}