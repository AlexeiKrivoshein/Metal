using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetalDAL.Manager
{
    public class VersionManager
    {
        Dictionary<Type, ModelElementVersion> _versionMap = new Dictionary<Type, ModelElementVersion>();
        Dictionary<Type, VersionsCache> _versionCaches = new Dictionary<Type, VersionsCache>();

        //максимальные версии для типа
        public void SetVersion<T>(long version)
            where T: class
        {
            if (_versionMap.TryGetValue(typeof(T), out var _))
            {
                return;
            }

            _versionMap.Add(typeof(T), new ModelElementVersion(version));
        }

        public bool TryGetNext<T>(out long version)
            where T: class
        {
            if (_versionMap.TryGetValue(typeof(T), out var elementVersion))
            {
                version = elementVersion.GetNext();
                return true;
            }

            version = 0L;
            return false;
        }

        public bool TryGetActual<T>(out long version)
            where T: class
        {
            if (_versionMap.TryGetValue(typeof(T), out var elementVersion))
            {
                version = elementVersion.GetActual();
                return true;
            }

            version = 0L;
            return false;
        }

        //кеши для версий отдельных элементов
        public void AddVersionCache<T>() where T : class
        {
            if(! _versionCaches.ContainsKey(typeof(T)))
            {
                _versionCaches.Add(typeof(T), new VersionsCache());
            }
        }

        public bool TryGetVersionCache<T>(out VersionsCache cache) where T : class
        {
            if (_versionCaches.ContainsKey(typeof(T)))
            {
                cache = _versionCaches[typeof(T)];
                return true;
            }
            else
            {
                cache = null;
                return false;
            }
        }
    }
}
