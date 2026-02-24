using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using SkillFlow.Infrastructure.Caching;
using Xunit;

namespace SkillFlow.Tests.Infrastructure.Caching;

public sealed class CacheExtensionsTests
{
    [Fact]
    public async Task GetOrCreateAsync_Should_cache_value_for_same_key()
    {
        // Arrange
        using var cache = new MemoryCache(new MemoryCacheOptions());

        var factoryCalls = 0;

        Task<int> Factory()
        {
            factoryCalls++;
            return Task.FromResult(123);
        }

        var key = "attendees:search:john";
        var ttl = TimeSpan.FromMinutes(5);

        // Act
        var v1 = await cache.GetOrCreateAsync(key, ttl, Factory);
        var v2 = await cache.GetOrCreateAsync(key, ttl, Factory);

        // Assert
        v1.Should().Be(123);
        v2.Should().Be(123);
        factoryCalls.Should().Be(1, "factory ska bara köras vid cache miss");
    }

    [Fact]
    public async Task Versioned_cache_key_Should_be_invalidated_after_bump()
    {
        // Arrange
        using var cache = new MemoryCache(new MemoryCacheOptions());

        var buster = new FakeBuster();
        var factoryCalls = 0;

        Task<string> Factory()
        {
            factoryCalls++;
            return Task.FromResult($"value#{factoryCalls}");
        }

        var ttl = TimeSpan.FromMinutes(5);

        string V(string key) => CacheKey.V(buster.CurrentVersion, key);

        // Act 1: första versionen
        var k1 = V("attendees:all");
        var a1 = await cache.GetOrCreateAsync(k1, ttl, Factory);
        var a2 = await cache.GetOrCreateAsync(k1, ttl, Factory);

        // Bump → ny version → ny key
        buster.Bump();

        var k2 = V("attendees:all");
        var b1 = await cache.GetOrCreateAsync(k2, ttl, Factory);
        var b2 = await cache.GetOrCreateAsync(k2, ttl, Factory);

        // Assert
        factoryCalls.Should().Be(2, "factory ska köras igen efter Bump (ny version -> ny cache key)");
        a1.Should().Be("value#1");
        a2.Should().Be("value#1");
        b1.Should().Be("value#2");
        b2.Should().Be("value#2");
    }

    private sealed class FakeBuster
    {
        private int _version = 1;
        public int CurrentVersion => Volatile.Read(ref _version);
        public void Bump() => Interlocked.Increment(ref _version);
    }
}