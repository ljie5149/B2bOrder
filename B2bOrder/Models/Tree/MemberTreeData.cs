using B2bOrder.Models.Db;

namespace B2bOrder.Models.Tree
{
    public class MemberTreeData
    {
        public DataMember? Member { get; set; }

        public List<DataMember> Children { get; set; } = new();
    }
}
