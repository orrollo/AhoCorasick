using System.Collections.Generic;

namespace AhoCorasick.Core
{
    public partial class SearchFsa<TItem>
    {
        public class Node
        {
            public Node Parent { get; set; }
            public Node SuffixLink { get; set; }

            public TItem IncomeValue { get; set; }

            public int Id { get; set; }

            public bool Final { get; set; }
            public object FinalContext { get; set; }
            public int DataLength { get; set; }

            public bool HasFinalSuffix { get; set; }

            protected internal Dictionary<TItem, Node> Jumps;

            public Node()
            {
                Jumps = new Dictionary<TItem, Node>();
                SuffixLink = null;
                Parent = null;
            }

            public Node(Node parent)
                : this()
            {
                Parent = parent;
            }

            public Node(Node parent, TItem incomeValue)
                : this(parent)
            {
                IncomeValue = incomeValue;
            }
        }
    }
}
