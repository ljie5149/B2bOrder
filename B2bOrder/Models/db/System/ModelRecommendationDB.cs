using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.System.Models
{
    public class RecommendationDBModel
    {
        // 此類別作為容器或可依專案需求拆分，以下為各資料表 Model 與中英文 Region
    }

    #region 會員推薦偏好與特徵檔 (Customer Profile & Preferences)
    [Table("rec_customer_preference")]
    public class RecCustomerPreferenceModel
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("customer_sid")]
        public string CustomerSid { get; set; } = null!;

        [Column("preferred_categories_json", TypeName = "json")]
        public string? PreferredCategoriesJson { get; set; }

        [Column("preferred_brands_json", TypeName = "json")]
        public string? PreferredBrandsJson { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("price_sensitivity_level")]
        public string PriceSensitivityLevel { get; set; } = "MEDIUM";

        [Column("last_model_updated_at")]
        public DateTime? LastModelUpdatedAt { get; set; }

        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 商品關聯與相似度矩陣檔 (Item-to-Item Similarity)
    [Table("rec_item_similarity")]
    public class RecItemSimilarityModel
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("source_item_sid")]
        public string SourceItemSid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("target_item_sid")]
        public string TargetItemSid { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [Column("relation_type")]
        public string RelationType { get; set; } = "SIMILAR";

        [Column("similarity_score", TypeName = "decimal(8,6)")]
        public decimal SimilarityScore { get; set; } = 0.000000m;

        [Column("rank_order")]
        public int RankOrder { get; set; } = 1;

        [Required]
        [MaxLength(50)]
        [Column("model_version")]
        public string ModelVersion { get; set; } = "v1.0";

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 個人化推薦結果與快取檔 (Personalized Recommendations)
    [Table("rec_personal_recommendation")]
    public class RecPersonalRecommendationModel
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("customer_sid")]
        public string CustomerSid { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("scene_code")]
        public string SceneCode { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("recommended_item_sid")]
        public string RecommendedItemSid { get; set; } = null!;

        [Column("recommend_score", TypeName = "decimal(8,6)")]
        public decimal RecommendScore { get; set; } = 0.000000m;

        [Column("display_rank")]
        public int DisplayRank { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("algorithm_code")]
        public string AlgorithmCode { get; set; } = "CF_HYBRID";

        [Column("expired_at")]
        public DateTime ExpiredAt { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 推薦系統人工干預與調控規則檔 (Boost, Bury & Block Rules)
    [Table("rec_rule_setting")]
    public class RecRuleSettingModel
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        [Column("rule_name")]
        public string RuleName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("scene_code")]
        public string SceneCode { get; set; } = "ALL";

        [Required]
        [MaxLength(20)]
        [Column("action_type")]
        public string ActionType { get; set; } = "BOOST";

        [Required]
        [MaxLength(30)]
        [Column("target_entity_type")]
        public string TargetEntityType { get; set; } = "ITEM";

        [Required]
        [MaxLength(32)]
        [Column("target_entity_sid")]
        public string TargetEntitySid { get; set; } = null!;

        [Column("boost_weight", TypeName = "decimal(5,2)")]
        public decimal BoostWeight { get; set; } = 1.00m;

        [Column("fixed_pin_position")]
        public int? FixedPinPosition { get; set; }

        [Column("start_time")]
        public DateTime? StartTime { get; set; }

        [Column("end_time")]
        public DateTime? EndTime { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("rule_status")]
        public string RuleStatus { get; set; } = "ACTIVE";

        [Column("version_no")]
        public ulong VersionNo { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 推薦版位曝光、點擊與轉換歷程檔 (Recommendation Analytics)
    [Table("rec_impression_log")]
    public class RecImpressionLogModel
    {
        [Key]
        [Column("nid")]
        public long Nid { get; set; }

        [Required]
        [MaxLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [MaxLength(32)]
        [Column("customer_sid")]
        public string? CustomerSid { get; set; }

        [MaxLength(100)]
        [Column("session_id")]
        public string? SessionId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("scene_code")]
        public string SceneCode { get; set; } = null!;

        [Required]
        [MaxLength(32)]
        [Column("recommended_item_sid")]
        public string RecommendedItemSid { get; set; } = null!;

        [Column("display_position")]
        public int DisplayPosition { get; set; }

        [MaxLength(50)]
        [Column("algorithm_code")]
        public string? AlgorithmCode { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("is_clicked")]
        public string IsClicked { get; set; } = "N";

        [Column("click_time")]
        public DateTime? ClickTime { get; set; }

        [Required]
        [MaxLength(2)]
        [Column("converted_to_order")]
        public string ConvertedToOrder { get; set; } = "N";

        [MaxLength(32)]
        [Column("sales_order_sid")]
        public string? SalesOrderSid { get; set; }

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion
}