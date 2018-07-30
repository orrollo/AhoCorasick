using System;
using System.Collections.Generic;
using System.Text;

namespace AhoCorasick.Core
{
    public class HashSet<T> : IDisposable
    {
        private int maskSize = 4;
        private Dictionary<int,List<T>> _items = new Dictionary<int, List<T>>();

        public bool Contains(T obj)
        {
            if (obj == null) throw new ArgumentException();
            var hashGroup = GetHashGroup(obj);
            if (!_items.ContainsKey(hashGroup)) return false;
            return _items[hashGroup].Contains(obj);
        }

        private int GetHashGroup(T obj)
        {
            return obj.GetHashCode() & ((1 << maskSize) - 1);
        }

        protected int BSearch(List<T> list, T obj)
        {
            if (list.Count == 0) return -1;
            var hash = obj.GetHashCode();
            if (list[0].GetHashCode() > hash) return 0;
            if (list[list.Count - 1].GetHashCode() < hash) return -1;
            //
            int l = 0, r = list.Count - 1;
            while ((r - l) > 1)
            {
                int m = (r + l) >> 1, hh = list[m].GetHashCode();
                if (hh < hash) 
                    l = m;
                else 
                    r = m;
            }
            for (int idx = r; (idx < list.Count) && (list[idx].GetHashCode() == hash); idx++) if (list[idx].Equals(obj)) return idx;
            return r;
        }

        public void Remove(T obj)
        {
            if (obj == null) throw new ArgumentException();
            var hashGroup = GetHashGroup(obj);
            if (!_items.ContainsKey(hashGroup)) return;
            //
            var list = _items[hashGroup];
            var index = BSearch(list, obj);
            if (index == -1 || !list[index].Equals(obj)) return;
            list.RemoveAt(index);
        }

        public void Add(T obj)
        {
            if (obj == null) throw new ArgumentException();
            var hashGroup = GetHashGroup(obj);
            if (!_items.ContainsKey(hashGroup)) _items[hashGroup] = new List<T>();
            var list = _items[hashGroup];
            var index = BSearch(list, obj);
            if (index != -1)
            {
                if (list[index].Equals(obj)) return;
                list.Insert(index, obj);
            }
            else
            {
                list.Add(obj);
            }
            if (_items.Count >= 65536) return;
            if (list.Count <= (1 << maskSize)) return;
            IncreaseMask();
        }

        private void IncreaseMask()
        {
            maskSize++;
            var ret = new Dictionary<int, List<T>>();
            foreach (var pair in _items)
            {
                foreach (var obj in pair.Value)
                {
                    var hashGroup = GetHashGroup(obj);
                    if (!ret.ContainsKey(hashGroup)) ret[hashGroup] = new List<T>();
                    ret[hashGroup].Add(obj);
                }
            }
            foreach (var pair in _items) pair.Value.Sort((a,b)=>a.GetHashCode().CompareTo(b.GetHashCode()));
        }

        public void Dispose()
        {
            foreach (var pair in _items) pair.Value.Clear();
            _items.Clear();
        }
    }
}
