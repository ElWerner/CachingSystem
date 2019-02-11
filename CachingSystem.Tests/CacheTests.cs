using System;
using System.Runtime.Caching;
using Moq;
using NUnit.Framework;
using CachingSystem;

namespace Tests
{
    [TestFixture]
    public class CacheTests
    {
        private Mock<ObjectCache> _objectCache;

        [SetUp]
        public void SetUp()
        {
            _objectCache = new Mock<ObjectCache>();
        }

        [Test]
        public void Add_ValidKeyAndTimeParametres_AddMethodCalled()
        {
            //Arrange
            Cache cache = new Cache(_objectCache.Object);

            string arrangedKey = "Key";
            string arrangedCachedObject = "Object";
            long arrangedCacheTime = 100;

            CacheItemPolicy arrangedCacheItemPolicy = new CacheItemPolicy();
            arrangedCacheItemPolicy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(arrangedCacheTime);

            //Action
            cache.Add(arrangedKey, arrangedCachedObject, arrangedCacheTime);

            //Assert
            _objectCache.Verify(c => c.Set(
                It.Is<string>(key => key == arrangedKey),
                It.Is<string>(cachedObject => cachedObject == arrangedCachedObject),
                It.Is<CacheItemPolicy>(policy => policy.AbsoluteExpiration.Second == arrangedCacheItemPolicy.AbsoluteExpiration.Second),
                null),
                Times.AtLeastOnce);
        }

        [Test]
        public void Get_ValidKeyAndTimeParametres_GetMethodCalled()
        {
            //Arrange
            _objectCache.SetupGet(c => c[It.IsAny<string>()]).Returns(new object());

            Cache cache = new Cache(_objectCache.Object);

            //Action
            cache.Get<object>("Key");

            //Assert
            _objectCache.Verify(c => c
                [It.Is<string>(key => key == "Key")],
                Times.AtLeastOnce);
        }

        [Test]
        public void Cache_NullObjectCache_ArgumentNullExceptionExpected()
        {
            //Arrange
            ObjectCache emptyCache = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() => new Cache(emptyCache));
        }

        [Test]
        public void Add_EmptyAndNullKey_ArgumentNullExceptionExpected()
        {
            //Arrange
            Cache cache = new Cache();

            string emptyKey = string.Empty;
            string nullKey = null;
            object cachedObject = new object();
            long cacheTime = 100;

            //Assert
            Assert.Throws<ArgumentNullException>(() => cache.Add(emptyKey, cachedObject, cacheTime));
            Assert.Throws<ArgumentNullException>(() => cache.Add(nullKey, cachedObject, cacheTime));
        }

        [Test]
        public void Add_NegativeAndZeroTime_ArgumentOutOfRangeExceptionExpected()
        {
            //Arrange
            Cache cache = new Cache();

            string key = "Key";
            object cachedObject = new object();
            long negativeCacheTime = -100;
            long zeroCacheTime = 0;

            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => cache.Add(key, cachedObject, negativeCacheTime));
            Assert.Throws<ArgumentOutOfRangeException>(() => cache.Add(key, cachedObject, zeroCacheTime));
        }

        [Test]
        public void Add_ExistingKey_ArgumentExceptionExpected()
        {
            //Arrange
            Cache cache = new Cache();

            string key = "Key";
            object cachedObject = new object();
            long cacheTime = 100;

            cache.Add(key, cachedObject, cacheTime);
            
            //Assert
            Assert.Throws<ArgumentException>(() => cache.Add(key, cachedObject, cacheTime));
        }

        [Test]
        public void Get_EmptyAndNullKey_ArgumentNullExceptionExpected()
        {
            //Arrange
            Cache cache = new Cache();

            string emptyKey = string.Empty;
            string nullKey = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() => cache.Get<object>(emptyKey));
            Assert.Throws<ArgumentNullException>(() => cache.Get<object>(nullKey));
        }

        [Test]
        public void Get_KeyStringWithNotExistingObject_ArgumentExceptionExpected()
        {
            //Arrange
            Cache cache = new Cache();

            string key = "Key";

            //Assert
            Assert.Throws<ArgumentException>(() => cache.Get<object>(key));
        }

        [Test]
        public void Get_ValidKeyAndTimeParametres_CachedObject()
        {
            //Arrange
            Cache cache = new Cache();

            string key = "Key";
            string cachedObject = "Object";
            long cacheTime = 100;

            cache.Add(key, cachedObject, cacheTime);

            //Action
            string actualCachedObject = cache.Get<string>(key);

            //Assert
            Assert.AreEqual(cachedObject, actualCachedObject);
        }
    }
}