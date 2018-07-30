using System;
using System.Collections.Generic;

namespace AhoCorasick.Core
{
    public partial class SearchFsa<TItem>
    {
        protected List<Node> Nodes;
        protected Node Root;

        public SearchFsa()
        {
            ResetNodes();
        }

        private Node CreateNode()
        {
            return RegisterNode(new Node());
        }

        private Node CreateNode(Node parent, TItem income)
        {
            return RegisterNode(new Node(parent, income));
        }

        private Node RegisterNode(Node node)
        {
            node.Id = Nodes.Count;
            Nodes.Add(node);
            return node;
        }

        public bool IsPrepared { get; protected set; }

        public void ResetNodes()
        {
            lock (Locker)
            {
                Nodes = new List<Node>();
                IsPrepared = false;
                Root = CreateNode();
                Root.Parent = Root;
            }
        }

        private Node GetSuffLink(Node node)
        {
            if (node.SuffixLink == null)
            {
                if (node == Root || node.Parent == Root)
                {
                    node.SuffixLink = Root;
                }
                else
                {
                    node.SuffixLink = GetLink(GetSuffLink(node.Parent), node.IncomeValue);
                }
            }
            return node.SuffixLink;
        }

        private Node GetLink(Node node, TItem income)
        {
            if (!node.Jumps.ContainsKey(income))
            {
                node.Jumps[income] = node == Root ? Root : GetLink(GetSuffLink(node), income);
            }
            return node.Jumps[income];
        }

        public void Add(IEnumerable<TItem> data, object context = null)
        {
            if (data == null) return;

            lock (Locker)
            {
                var count = 0;
                var currentNode = Root;
                foreach (var ch in data)
                {
                    if (!currentNode.Jumps.ContainsKey(ch)) currentNode.Jumps[ch] = CreateNode(currentNode, ch);
                    currentNode = currentNode.Jumps[ch];
                    count++;
                }
                if (currentNode == Root) return;

                IsPrepared = false;

                currentNode.Final = true;
                currentNode.FinalContext = context ?? data;
                currentNode.DataLength = count;
            }

        }

        protected void Recurse(Node node, Action<Node> fn, HashSet<Node> processed = null)
        {
            if (processed == null) processed = new HashSet<Node>();
            if (!processed.Contains(node))
            {
                processed.Add(node);
                fn(node);
                var keys = new List<TItem>();
                foreach (var key in node.Jumps.Keys) keys.Add(key);
                foreach (var key in keys) Recurse(node.Jumps[key], fn, processed);
            }
        }

        protected object Locker = new object();

        // prepare and search

        public void Prepare()
        {
            if (IsPrepared) return;
            lock (Locker)
            {
                if (IsPrepared) return;
                Recurse(Root, ResetHasSuffix);
                Recurse(Root, SetSuffixLink);
                Recurse(Root, RemoveRootLinks);
                Recurse(Root, CalculateHasSuffix);
                IsPrepared = true;
            }
        }

        private void CalculateHasSuffix(Node node)
        {
            var suffux = node.SuffixLink;
            node.HasFinalSuffix = suffux.Final || suffux.HasFinalSuffix;
        }

        private void ResetHasSuffix(Node node)
        {
            node.HasFinalSuffix = false;
        }

        private void RemoveRootLinks(Node node)
        {
            var ret = new List<TItem>();
            foreach (var pair in node.Jumps) if (pair.Value == Root) ret.Add(pair.Key);
            foreach (var key in ret) node.Jumps.Remove(key);
        }

        private void SetSuffixLink(Node node)
        {
            node.SuffixLink = GetSuffLink(node);
        }

        public delegate void FoundDelegate(int position, object context);

        protected List<EndPoint> EndPoints = new List<EndPoint>();

        public EndPoint GetEndPoint(FoundDelegate action = null)
        {
            lock (Locker)
            {
                if (!IsPrepared) throw new ArgumentException();
                var endPoint = new EndPoint(this, action);
                EndPoints.Add(endPoint);
                return endPoint;
            }
        }

        private void UnregisterEndPoint(EndPoint endPoint)
        {
            lock (Locker)
            {
                if (EndPoints.Contains(endPoint)) EndPoints.Remove(endPoint);
            }
        }
    }
}

