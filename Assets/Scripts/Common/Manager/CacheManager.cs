using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleFramework
{
    public class CacheManager : Singleton<CacheManager>
    {

        private Dictionary<object, List<object>> mapCaches = new Dictionary<object, List<object>>();
        public T CreateData<T>(object key) where T : class, new()
        {
            if (key == null)
            {
                return null;
            }
            T res = null;
            if (mapCaches.ContainsKey(key))
            {
                if (mapCaches[key] != null && mapCaches[key].Count > 0)
                {
                    res = mapCaches[key][0] as T;
                    mapCaches[key].RemoveAt(0);
                }
            }
            if (res == null)
            {
                res = default(T);
            }
            
            return res;
        }



    }

}

