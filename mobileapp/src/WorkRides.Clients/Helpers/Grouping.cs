using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CarPool.Clients.Core.Helpers
{
    public class Grouping<K, T> : ObservableCollection<T>
    {
        public K Key { get; private set; }

        public Grouping(K key, T item)
        {
            Key = key;
            this.Items.Add(item);
        }
    }
}