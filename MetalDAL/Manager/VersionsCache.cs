using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalDAL.Manager
{
    public class VersionsCache
    {
        private VersionComparer _comparer = new VersionComparer();

        private ConcurrentDictionary<Guid, long> _objectsVersions = new ConcurrentDictionary<Guid, long>();

        public bool TryGetObjectVersion(Guid id, out long version)
        {
            return _objectsVersions.TryGetValue(id, out version);
        }

        public void SetObjectVersion(Guid id, long version)
        {
            _objectsVersions.AddOrUpdate(id, version, (k, v) => version);
        }

        public void RemoveObjectVersion(Guid id)
        {
            _objectsVersions.TryRemove(id, out var _);
        }

        public void SetObjectsVersions(IDictionary<Guid, long> versions)
        {
            Parallel.ForEach(versions,
                new ParallelOptions() { MaxDegreeOfParallelism = 5 },
                version =>
                {
                    _objectsVersions.AddOrUpdate(version.Key, version.Value, (k, v) => version.Value);
                });
        }

        public List<Guid> GetUpdatedIds(IDictionary<Guid, long> versions)
        {
            return versions.Except(_objectsVersions, _comparer).Select(kv => kv.Key).ToList();
        }


        private class VersionComparer : IEqualityComparer<KeyValuePair<Guid, long>>
        {
            public bool Equals(KeyValuePair<Guid, long> x, KeyValuePair<Guid, long> y)
            {
                return x.Key == y.Key && x.Value == y.Value;
            }

            public int GetHashCode(KeyValuePair<Guid, long> obj)
            {
                return obj.Key.GetHashCode() * 17 + obj.Value.GetHashCode();
            }
        }
    }
}
