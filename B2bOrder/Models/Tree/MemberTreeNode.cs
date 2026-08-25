namespace B2bOrder.Models.Tree
{
    public class MemberTreeNode
    {
        public int Level { get; set; }
        public string Sid { get; set; }

        public string Mid { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }

        public int TeamCount { get; set; }

        public bool IsRoot { get; set; } = false;

        public bool Expanded { get; set; } = true;

        public List<MemberTreeNode> Children { get; set; } = new();
    }
    public class MemberTreePageViewModel
    {
        public MemberTreeNode Tree { get; set; }

        public CreateMemberViewModel CreateMember { get; set; }

        public EditMemberViewModel EditMember { get; set; }
    }
}
