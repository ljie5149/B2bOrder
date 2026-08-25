using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Models.Db
{
    [Table("data_member_file")]
    public class DataMemberFile
    {
        [Key]
        [Column("nid")]
        public int Nid { get; set; }

        [Column("sid")]
        public string Sid { get; set; } = string.Empty;

        [Column("member_sid")]
        public string MemberSid { get; set; } = string.Empty;

        [Column("file_type")]
        public string FileType { get; set; } = string.Empty;

        [Column("file_name")]
        public string FileName { get; set; } = string.Empty;

        [Column("save_file_name")]
        public string SaveFileName { get; set; } = string.Empty;

        [Column("file_path")]
        public string FilePath { get; set; } = string.Empty;

        [Column("file_size")]
        public long? FileSize { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Column("avalible")]
        public string? Avalible { get; set; }

        [Column("remark")]
        public string? Remark { get; set; }

        [Column("member_nid")]
        public int MemberNid { get; set; }

        [ForeignKey("MemberNid")]
        public virtual DataMember? Member { get; set; }
    }
}