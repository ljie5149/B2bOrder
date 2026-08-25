namespace B2bOrder.Models.Tree
{
    public class DeleteMemberViewModel
    {
        public string Sid { get; set; }

        public string HandleType { get; set; }

        public string? TargetSid { get; set; }

        public string? TargetMid { get; set; }
    }
}