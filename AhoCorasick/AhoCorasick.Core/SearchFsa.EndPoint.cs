using System;
using System.Collections.Generic;
using System.Text;

namespace AhoCorasick.Core
{
    public partial class SearchFsa<TItem>
    {
        public class EndPoint : IDisposable
        {
            protected SearchFsa<TItem> Fsa;
            protected FoundDelegate TotalFound;
            protected bool IsDisposed;

            public EndPoint(SearchFsa<TItem> fsa, FoundDelegate foundDelegate = null)
            {
                if (fsa == null || !fsa.IsPrepared) throw new ArgumentException();
                Fsa = fsa;
                TotalFound = foundDelegate;
                ResetState();
            }

            public void Dispose()
            {
                if (IsDisposed) return;
                if (Fsa != null) Fsa.UnregisterEndPoint(this);
                TotalFound = null;
                Fsa = null;
                IsDisposed = true;
            }

            protected int State;
            protected int Position;

            public void ResetState()
            {
                if (!Fsa.IsPrepared) throw new ArgumentException("not prepared fsa!");

                State = 0;
                Position = 0;
            }

            public void ProcessIncome(TItem income, FoundDelegate foundAction = null)
            {
                if (!Fsa.IsPrepared) throw new ArgumentException("not prepared fsa!");

                Position++;
                var currentNode = Fsa.Nodes[State];
                while (true)
                {
                    if (currentNode.Jumps.ContainsKey(income))
                    {
                        currentNode = currentNode.Jumps[income];
                        State = currentNode.Id;
                        ProcessFinalNode(currentNode, foundAction);
                        if (currentNode.HasFinalSuffix)
                        {
                            var suffixLink = currentNode.SuffixLink;
                            while (suffixLink != suffixLink.SuffixLink)
                            {
                                ProcessFinalNode(suffixLink, foundAction);
                                suffixLink = suffixLink.SuffixLink;
                            }
                        }
                        break;
                    }
                    var suffLink = currentNode.SuffixLink;
                    if (suffLink == currentNode) break;
                    currentNode = suffLink;
                    State = currentNode.Id;
                }
            }

            private void ProcessFinalNode(Node currentNode, FoundDelegate foundAction)
            {
                if (!currentNode.Final) return;
                var foundPosition = Position - currentNode.DataLength;
                if (foundAction != null) foundAction(foundPosition, currentNode.FinalContext);
                if (TotalFound != null) TotalFound(foundPosition, currentNode.FinalContext);
            }
        }
    }
}
