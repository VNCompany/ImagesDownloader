using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using HtmlParsing.Internal;

namespace HtmlParsing
{
    public class HtmlNodesCollection : IReadOnlyCollection<HtmlNode>
    {
        private readonly IReadOnlyList<HtmlNode> _nodes;
        private readonly int[] _indexes;

        internal HtmlNodesCollection(IReadOnlyList<HtmlNode> nodes, HtmlNode parent, int firstChildIndex, int lastChildIndex)
        {
            _nodes = nodes;

            lastChildIndex += 1;
            List<int> temp = new List<int>();
            for (int i = firstChildIndex; i != lastChildIndex; ++i)
            {
                if (_nodes[i].Parent == parent)
                    temp.Add(i);
            }

            _indexes = temp.ToArray();
        }

        public HtmlNode this[int index] => _nodes[_indexes[index]];
        public int Count => _indexes.Length;

        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<HtmlNode> IEnumerable<HtmlNode>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public class Enumerator : IEnumerator<HtmlNode>
        {
            private readonly HtmlNodesCollection _collection;
            private int _index;
            private HtmlNode? _current;

            public HtmlNode Current => _current ?? throw new NullReferenceException(nameof(Current));

            object IEnumerator.Current => Current;

            public Enumerator(HtmlNodesCollection collection)
            {
                _collection = collection;
                _index = 0;
                _current = null;
            }

            public bool MoveNext()
            {
                if (_index < _collection._indexes.Length)
                {
                    _current = _collection._nodes[_collection._indexes[_index++]];
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _current = null;
                _index = 0;
            }

            void IDisposable.Dispose() => GC.SuppressFinalize(this);
        }
    }
}