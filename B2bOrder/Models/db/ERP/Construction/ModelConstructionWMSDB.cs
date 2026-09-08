using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2bOrder.Resources.ERP.Construction.Models
{
    #region 01. 工地區域與現場儲位主檔 / Site Yard & Storage Zone Master
    /// <summary>
    /// 工地區域與現場儲位主檔 / Site Yard & Storage Zone Master
    /// </summary>
    [Table("wms_site_zone_master")]
    public class SiteZoneMaster
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("project_sid")]
        public string ProjectSid { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Column("zone_code")]
        public string ZoneCode { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [Column("zone_name")]
        public string ZoneName { get; set; } = null!;

        [Required]
        [StringLength(30)]
        [Column("zone_type")]
        public string ZoneType { get; set; } = "YARD";

        [Required]
        [StringLength(20)]
        [Column("floor_level")]
        public string FloorLevel { get; set; } = "1F";

        [Column("max_weight_capacity_kg", TypeName = "decimal(12,2)")]
        public decimal MaxWeightCapacityKg { get; set; } = 0.00m;

        [Required]
        [StringLength(2)]
        [Column("is_crane_accessible")]
        public string IsCraneAccessible { get; set; } = "Y";

        [Required]
        [StringLength(20)]
        [Column("zone_status")]
        public string ZoneStatus { get; set; } = "ACTIVE";

        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 02. 進場交貨與卸貨時段預約檔 / Delivery Slot Appointment
    /// <summary>
    /// 進場交貨與卸貨時段預約檔 / Delivery Slot Appointment
    /// </summary>
    [Table("wms_delivery_appointment")]
    public class DeliveryAppointment
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("project_sid")]
        public string ProjectSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("zone_sid")]
        public string ZoneSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("vendor_sid")]
        public string VendorSid { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Column("appointment_no")]
        public string AppointmentNo { get; set; } = null!;

        [Column("expected_arrival_time")]
        public DateTime ExpectedArrivalTime { get; set; }

        [Column("estimated_duration_mins")]
        public int EstimatedDurationMins { get; set; } = 60;

        [Required]
        [StringLength(50)]
        [Column("truck_plate_number")]
        public string TruckPlateNumber { get; set; } = null!;

        [StringLength(100)]
        [Column("driver_name")]
        public string? DriverName { get; set; }

        [StringLength(50)]
        [Column("driver_phone")]
        public string? DriverPhone { get; set; }

        [Required]
        [StringLength(200)]
        [Column("cargo_description")]
        public string CargoDescription { get; set; } = null!;

        [Required]
        [StringLength(2)]
        [Column("is_heavy_lifting_needed")]
        public string IsHeavyLiftingNeeded { get; set; } = "N";

        [Required]
        [StringLength(20)]
        [Column("appointment_status")]
        public string AppointmentStatus { get; set; } = "BOOKED";

        [Column("actual_arrival_time")]
        public DateTime? ActualArrivalTime { get; set; }

        [Column("actual_departure_time")]
        public DateTime? ActualDepartureTime { get; set; }

        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 03. 車輛過磅與現場簽收點收單 / Weighbridge & Delivery Ticket
    /// <summary>
    /// 車輛過磅與現場簽收點收單 / Weighbridge & Delivery Ticket
    /// </summary>
    [Table("wms_weighbridge_ticket")]
    public class WeighbridgeTicket
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("project_sid")]
        public string ProjectSid { get; set; } = null!;

        [StringLength(32)]
        [Column("appointment_sid")]
        public string? AppointmentSid { get; set; }

        [StringLength(32)]
        [Column("ref_po_sid")]
        public string? RefPoSid { get; set; }

        [Required]
        [StringLength(50)]
        [Column("ticket_no")]
        public string TicketNo { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Column("truck_plate_number")]
        public string TruckPlateNumber { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("material_sid")]
        public string MaterialSid { get; set; } = null!;

        [Column("gross_weight_kg", TypeName = "decimal(12,2)")]
        public decimal GrossWeightKg { get; set; } = 0.00m;

        [Column("tare_weight_kg", TypeName = "decimal(12,2)")]
        public decimal TareWeightKg { get; set; } = 0.00m;

        [Column("net_weight_kg", TypeName = "decimal(12,2)")]
        public decimal NetWeightKg { get; set; } = 0.00m;

        [Required]
        [StringLength(32)]
        [Column("inspector_user_sid")]
        public string InspectorUserSid { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [Column("inspection_result")]
        public string InspectionResult { get; set; } = "PASSED";

        [StringLength(32)]
        [Column("weighbridge_photo_sid")]
        public string? WeighbridgePhotoSid { get; set; }

        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion

    #region 04. 跨工地材料與設備調撥主檔 / Site-to-Site Transfer Order
    /// <summary>
    /// 跨工地材料與設備調撥主檔 / Site-to-Site Transfer Order
    /// </summary>
    [Table("wms_transfer_order")]
    public class TransferOrder
    {
        [Key]
        [Column("nid")]
        public ulong Nid { get; set; }

        [Required]
        [StringLength(32)]
        [Column("sid")]
        public string Sid { get; set; } = null!;

        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("modify_date")]
        public DateTime? ModifyDate { get; set; }

        [Required]
        [StringLength(32)]
        [Column("company_sid")]
        public string CompanySid { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Column("transfer_no")]
        public string TransferNo { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("from_project_sid")]
        public string FromProjectSid { get; set; } = null!;

        [Required]
        [StringLength(32)]
        [Column("to_project_sid")]
        public string ToProjectSid { get; set; } = null!;

        [StringLength(32)]
        [Column("carrier_vendor_sid")]
        public string? CarrierVendorSid { get; set; }

        [Column("dispatch_date", TypeName = "date")]
        public DateTime DispatchDate { get; set; }

        [Column("actual_received_date")]
        public DateTime? ActualReceivedDate { get; set; }

        [Required]
        [StringLength(20)]
        [Column("transfer_status")]
        public string TransferStatus { get; set; } = "REQUESTED";

        [Required]
        [StringLength(32)]
        [Column("requested_by_user_sid")]
        public string RequestedByUserSid { get; set; } = null!;

        [StringLength(32)]
        [Column("received_by_user_sid")]
        public string? ReceivedByUserSid { get; set; }

        [Column("version_no")]
        public ulong VersionNo { get; set; } = 0;

        [Required]
        [StringLength(2)]
        [Column("avalible")]
        public string Avalible { get; set; } = "Y";

        [Column("remark", TypeName = "text")]
        public string? Remark { get; set; }
    }
    #endregion
}