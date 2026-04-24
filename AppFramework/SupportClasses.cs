using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        public string seat;
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
        public WaitListNode(string RollNo, double Rank, int isRetained,string Seat)
        {
            rollNo = RollNo;
            rank = Rank;
            this.isRetained = isRetained;
            seat = Seat;
        }

    }

    //public class WaitList : IEnumerable
    //{
    //    private List<WaitListNode> _retained = new List<WaitListNode>();
    //    private List<WaitListNode> _nodes = new List<WaitListNode>();
    //    public void Enqueue(WaitListNode node)
    //    {
    //        if (node.isRetained == 1)
    //        {
    //            _retained.Add(node);
    //        }
    //        else
    //        {
    //            _nodes.Add(node);
    //            swim(_nodes.Count - 1);
    //        }

    //    }

    //    public WaitListNode dequeue()
    //    {
    //        if (isEmpty())
    //        {
    //            throw new NullReferenceException();
    //        }
    //        WaitListNode result = _nodes[0];
    //        if (_nodes.Count > 1)
    //            _nodes[0] = _nodes[_nodes.Count - 1];
    //        _nodes.RemoveAt(_nodes.Count - 1);
    //        sink(0);
    //        return result;
    //    }

    //    private void swim(int index)
    //    {
    //        if (index == 0)
    //        {
    //            return;
    //        }
    //        int parent = (index - 1) / 2;
    //        if (_nodes[index].rank > _nodes[parent].rank)
    //        {
    //            swap(index, parent);
    //            swim(parent);
    //        }
    //    }

    //    private void swap(int index1, int index2)
    //    {
    //        WaitListNode temp = _nodes[index1];
    //        _nodes[index1] = _nodes[index2];
    //        _nodes[index2] = temp;
    //    }

    //    private void sink(int index)
    //    {
    //        int left = index * 2 + 1;
    //        int right = index * 2 + 2;
    //        if (left >= _nodes.Count)
    //        {
    //            return;
    //        }
    //        int largestChild = left;
    //        if (right < _nodes.Count)
    //        {
    //            if (_nodes[left].rank < _nodes[right].rank)
    //            {
    //                largestChild = right;
    //            }
    //        }
    //        if (_nodes[index].rank < _nodes[largestChild].rank)
    //        {
    //            swap(index, largestChild);
    //            sink(largestChild);
    //        }
    //    }


    //    public IEnumerator GetEnumerator()
    //    {
    //        foreach (WaitListNode o in _retained)
    //        {
    //            // Return the current element and then on next function call 
    //            // resume from next element rather than starting all over again;
    //            yield return o;
    //        }

    //        foreach (WaitListNode o in _nodes)
    //        {
    //            // Return the current element and then on next function call 
    //            // resume from next element rather than starting all over again;
    //            yield return o;
    //        }
    //    }

    //    public void clear()
    //    {
    //        _nodes.Clear();
    //    }
    //    public int size()
    //    {
    //        return _nodes.Count + _retained.Count;
    //    }
    //    //public bool isEmpty()
    //    //{
    //    //    return _nodes.Count == 0;
    //    //}



    //    public double GetMaxRank()
    //    {
    //        if (_nodes.Count == 0)
    //            return double.MaxValue;

    //        return _nodes[0].rank; 
    //    }
    //    public WaitListNode RemoveWorst()
    //    {
    //        if (_nodes.Count == 0)
    //            return null;

    //        WaitListNode worst = _nodes[0];

    //        int lastIndex = _nodes.Count - 1;

    //        _nodes[0] = _nodes[lastIndex];
    //        _nodes.RemoveAt(lastIndex);

    //        if (_nodes.Count > 0)
    //            sink(0);

    //        return worst;
    //    }
    //    public bool isEmpty()
    //    {
    //        return (_nodes.Count + _retained.Count) == 0;
    //    }
    //}
    public class WaitList : IEnumerable<WaitListNode>
    {
        private List<WaitListNode> _nodes = new List<WaitListNode>();

    public IEnumerator<WaitListNode> GetEnumerator()
    {
        return _nodes.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return _nodes.GetEnumerator();
    }

        public void Enqueue(WaitListNode node)
        {
            _nodes.Add(node);
            Swim(_nodes.Count - 1);
        }
        public void EnqueueDummy(WaitListNode node)
        {
            _nodes.Add(node);
        }

        public int size()
        {
            return _nodes.Count;
        }

        public bool isEmpty()
        {
            return _nodes.Count == 0;
        }

        public double GetMaxRank()
        {
            if (_nodes.Count == 0)
                return double.MaxValue;

            return _nodes[0].rank;
        }

        public WaitListNode RemoveWorst()
        {
            if (_nodes.Count == 0)
                return null;

            WaitListNode worst = _nodes[0];

            int last = _nodes.Count - 1;
            _nodes[0] = _nodes[last];
            _nodes.RemoveAt(last);

            if (_nodes.Count > 0)
                Sink(0);

            return worst;
        }

        private void Swim(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (_nodes[i].rank <= _nodes[parent].rank)
                    break;

                Swap(i, parent);
                i = parent;
            }
        }

        private void Sink(int i)
        {
            int n = _nodes.Count;

            while (true)
            {
                int left = 2 * i + 1;
                int right = 2 * i + 2;
                int largest = i;

                if (left < n && _nodes[left].rank > _nodes[largest].rank)
                    largest = left;

                if (right < n && _nodes[right].rank > _nodes[largest].rank)
                    largest = right;

                if (largest == i)
                    break;

                Swap(i, largest);
                i = largest;
            }
        }

        private void Swap(int i, int j)
        {
            var temp = _nodes[i];
            _nodes[i] = _nodes[j];
            _nodes[j] = temp;
        }

        public void RemoveByRollNo(string rollNo)
        {
            _nodes = _nodes.Where(x => x.rollNo != rollNo).ToList();
        }
    }


}