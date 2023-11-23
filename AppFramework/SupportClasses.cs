using System;
using System.Collections;
using System.Collections.Generic;

namespace AppFramework
{
    //public class AllotmentDetail
    //{
    //    public string Rollno { get; set; }
    //    public string Instcd { get; set; }
    //    public string Brcd { get; set; }
    //    public int OptNo { get; set; }
    //    public string Sequence { get; set; }
    //    public int IsRetained { get; set; }

    //    public AllotmentDetail(string rollno, string instcd, string brcd, string sequence, int optNo, int isRetained)
    //    {
    //        Rollno = rollno;
    //        Instcd = instcd;
    //        Brcd = brcd;
    //        OptNo = optNo;
    //        Sequence = sequence;
    //        IsRetained = isRetained;
    //    }

    //}

    public class WaitListNode
    {
        public string rollNo;
        public double rank;
        public int isRetained;
        public WaitListNode(string RollNo, double Rank)
        {
            rollNo = RollNo;
            rank = Rank;
            isRetained = 0;
        }

        public WaitListNode(string RollNo, double Rank, int isRetained)
        {
            rollNo = RollNo;
            rank = Rank;
            this.isRetained = isRetained;
        }

    }

    public class WaitList : IEnumerable
    {
        private List<WaitListNode> _retained = new List<WaitListNode>();
        private List<WaitListNode> _nodes = new List<WaitListNode>();
        public void Enqueue(WaitListNode node)
        {
            if (node.isRetained == 1)
            {
                _retained.Add(node);
            }
            else
            {
                _nodes.Add(node);
                swim(_nodes.Count - 1);
            }
            
        }

        public WaitListNode dequeue()
        {
            if (isEmpty())
            {
                throw new NullReferenceException();
            }
            WaitListNode result = _nodes[0];
            if (_nodes.Count > 1)
                _nodes[0] = _nodes[_nodes.Count - 1];
            _nodes.RemoveAt(_nodes.Count - 1);
            sink(0);
            return result;
        }

        private void swim(int index)
        {
            if (index == 0)
            {
                return;
            }
            int parent = (index - 1) / 2;
            if (_nodes[index].rank > _nodes[parent].rank)
            {
                swap(index, parent);
                swim(parent);
            }
        }

        private void swap(int index1, int index2)
        {
            WaitListNode temp = _nodes[index1];
            _nodes[index1] = _nodes[index2];
            _nodes[index2] = temp;
        }

        private void sink(int index)
        {
            int left = index * 2 + 1;
            int right = index * 2 + 2;
            if (left >= _nodes.Count)
            {
                return;
            }
            int largestChild = left;
            if (right < _nodes.Count)
            {
                if (_nodes[left].rank < _nodes[right].rank)
                {
                    largestChild = right;
                }
            }
            if (_nodes[index].rank < _nodes[largestChild].rank)
            {
                swap(index, largestChild);
                sink(largestChild);
            }
        }


        public IEnumerator GetEnumerator()
        {
            foreach (WaitListNode o in _retained)
            {
                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return o;
            }

            foreach (WaitListNode o in _nodes)
            {
                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return o;
            }
        }

        public void clear()
        {
            _nodes.Clear();
        }
        public int size()
        {
            return _nodes.Count+_retained.Count;
        }
        public bool isEmpty()
        {
            return _nodes.Count == 0;
        }



        public double GetMaxRank()
        {

            if (isEmpty())
            {
                throw new NullReferenceException();
            }
            return _nodes[0].rank;
        }
    }

}