using System;
using System.Runtime.Caching;

namespace CachingSystem
{
    /// <summary>
    /// Represents a class providing simple caching operations
    /// </summary>
    public class Cache
    {
        #region Fields
        /// <summary>
        /// Cache holder
        /// </summary>
        private ObjectCache _cache;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Cache"/> class with new memory cache
        /// </summary>
        public Cache()
        {
            _cache = new MemoryCache("New Memory Cache");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache"/> class with specified cache
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when cache parameter is not initialized.</exception>
        public Cache(ObjectCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException($"{nameof(cache)}");
        }
        #endregion

        #region Public API
        /// <summary>
        /// Adds new object to the cache
        /// </summary>
        /// <typeparam name="T">Type of cached object</typeparam>
        /// <param name="key">A unique identifier for the cache entry</param>
        /// <param name="cachedObject">The object to insert in the cache</param>
        /// <param name="cacheTimeInSeconds">How long to store object in the cache</param>
        /// <exception cref="ArgumentNullException">Thrown when key string parameter is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when cache time is less or equal to zero.</exception>
        /// <exception cref="ArgumentException">Thrown when cache entry with the same key already exists.</exception>
        public void Add<T>(string key, T cachedObject, long cacheTimeInSeconds)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException($"{nameof(key)}");
            }

            if (cacheTimeInSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(cacheTimeInSeconds)}");
            }

            try
            {
                AddObjectToCache<T>(key, cachedObject, cacheTimeInSeconds);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("An object with the same key already exists.");
            }
        }

        /// <summary>
        /// Gets object from the cache
        /// </summary>
        /// <typeparam name="T">Type of cached object</typeparam>
        /// <param name="key">A unique identifier for the cache entry to get</param>
        /// <returns>The cache entry that is identified by key parameter</returns>
        /// <exception cref="ArgumentNullException">Thrown when key string parameter is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when cache entry with specified key doesn't exist.</exception>
        public T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException($"{nameof(key)}");
            }

            try
            {
                return GetObjectFromCache<T>(key);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("An object with specified key doesn't exist.");
            }
        }

        /// <summary>
        /// Removes object from the cache
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry to remove</param>
        /// <exception cref="ArgumentNullException">Thrown when key string parameter is null or empty.</exception>
        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException($"{nameof(key)}");
            }

            RemoveObjectFromCache(key);
        }

        #endregion

        #region Private API

        private void AddObjectToCache<T>(string key, T cachedObject, long cacheTimeInSeconds)
        {
            if (IsExist(key))
            {
                throw new ArgumentException();
            }

            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(cacheTimeInSeconds);

            _cache.Set(key, cachedObject, cacheItemPolicy);
        }

        private T GetObjectFromCache<T>(string key)
        {
            var cachedObject = (T)_cache[key];
            if (cachedObject == null)
            {
                throw new ArgumentException();
            }

            return cachedObject;
        }

        private void RemoveObjectFromCache(string key)
        {
            if (IsExist(key))
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// Checks if a cache entry with specified key exists
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry to check</param>
        /// <returns>True if a cache entry exists. False otherwise</returns>
        private bool IsExist(string key)
        {
            var cachedObject = _cache[key];

            if (cachedObject == null)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
