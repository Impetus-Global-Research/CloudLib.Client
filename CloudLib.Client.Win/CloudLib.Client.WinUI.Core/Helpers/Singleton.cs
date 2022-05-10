using System;
using System.Collections.Concurrent;

namespace CloudLib.Client.WinUI.Core.Helpers
{
    public static class Singleton<T>
        where T : new()
    {
        private static readonly ConcurrentDictionary<Type, Lazy<T>> Instances = new();

        public static T Instance
        {
            get
            {
                return Instances.GetOrAdd(typeof(T), t => new Lazy<T>(() => new T())).Value;
            }
        }
    }
}
