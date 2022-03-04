using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MetalClient.Engine
{
    public class ObservableConcurrentDictionary<TKey, TValue>
        : ConcurrentDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs changeAction)
        {
            var eh = CollectionChanged;
            if (eh == null) return;

            eh(this, changeAction);

            OnPropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged()
        {
            var eh = PropertyChanged;
            if (eh == null) return;

            eh(this, new PropertyChangedEventArgs(null));
        }

        public ObservableConcurrentDictionary()
            : base()
        {
        }

        public new void Clear()
        {
            base.Clear();
            OnCollectionChanged(changeAction: new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory,
            Func<TKey, TValue, TValue> updateValueFactory)
        {
            bool isUpdated = false;
            TValue oldValue = default(TValue);

            TValue value = base.AddOrUpdate(key, addValueFactory, (k, v) =>
            {
                isUpdated = true;
                oldValue = v;
                return updateValueFactory(k, v);
            });

            if (isUpdated) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue));

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            return value;
        }

        public new TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            bool isUpdated = false;
            TValue oldValue = default(TValue);

            TValue value = base.AddOrUpdate(key, addValue, (k, v) =>
            {
                isUpdated = true;
                oldValue = v;
                return updateValueFactory(k, v);
            });

            if (isUpdated) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue));

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            return value;
        }


        public new TValue GetOrAdd(TKey key, Func<TKey, TValue> addValueFactory)
        {
            bool isAdded = false;

            TValue value = base.GetOrAdd(key, k =>
            {
                isAdded = true;
                return addValueFactory(k);
            });

            if (isAdded) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));

            return value;
        }

        public new TValue GetOrAdd(TKey key, TValue value)
        {
            return GetOrAdd(key, k => value);
        }

        public new bool TryAdd(TKey key, TValue value)
        {
            bool tryAdd = base.TryAdd(key, value);

            if (tryAdd) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));

            return tryAdd;
        }

        public new bool TryRemove(TKey key, out TValue value)
        {
            bool tryRemove = base.TryRemove(key, out value);

            if (tryRemove) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));

            return tryRemove;
        }

        public new bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            bool tryUpdate = base.TryUpdate(key, newValue, comparisonValue);

            if (tryUpdate) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, comparisonValue));

            return tryUpdate;
        }
    }
}
