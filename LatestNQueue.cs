using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkits {
    public class LatestNQueue<T> {
        private Queue<T> _Container;
        public int MaxSize { get; private set; }

        public LatestNQueue(int maxSize) {
            _Container = new Queue<T>();
            MaxSize = maxSize;
        }

        public T Enqueue(T newOne) {
            T oldOne = default(T);
            if (_Container.Count == MaxSize - 1)
                oldOne = _Container.Dequeue();
            _Container.Enqueue(newOne);

            return oldOne;
        }

        public List<T> ToList() {
            return _Container.ToList();
        }
    }
}
