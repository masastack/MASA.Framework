// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.BuildingBlocks.Caching
{
    public class CacheEntryOptions
    {
        private TimeSpan? _absoluteExpirationRelativeToNow;
        private TimeSpan? _slidingExpiration;

        /// <summary>
        /// Gets or sets an absolute expiration date for the cache entry.
        /// When coexisting with AbsoluteExpirationRelativeToNow, use AbsoluteExpirationRelativeToNow first
        /// </summary>
        public DateTimeOffset? AbsoluteExpiration { get; set; }

        /// <summary>
        /// Gets or sets an absolute expiration time, relative to now.
        /// When coexisting with AbsoluteExpiration, use AbsoluteExpirationRelativeToNow first
        /// </summary>
        public TimeSpan? AbsoluteExpirationRelativeToNow
        {
            get => _absoluteExpirationRelativeToNow;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(AbsoluteExpirationRelativeToNow),
                        value,
                        "The relative expiration value must be positive.");
                }

                _absoluteExpirationRelativeToNow = value;
            }
        }

        /// <summary>
        /// Gets or sets how long a cache entry can be inactive (e.g. not accessed) before it will be removed.
        /// This will not extend the entry lifetime beyond the absolute expiration (if set).
        /// </summary>
        public TimeSpan? SlidingExpiration
        {
            get => _slidingExpiration;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(SlidingExpiration),
                        value,
                        "The sliding expiration value must be positive.");
                }
                _slidingExpiration = value;
            }
        }

        public CacheEntryOptions() { }

        public CacheEntryOptions(DateTimeOffset? absoluteExpiration)
            => AbsoluteExpiration = absoluteExpiration;

        public CacheEntryOptions(TimeSpan? absoluteExpirationRelativeToNow)
            => AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
    }

    public class CacheEntryOptions<T> : CacheEntryOptions
    {
        public Action<T?>? ValueChanged { get; set; }

        public CacheEntryOptions()
        {
        }

        public CacheEntryOptions(DateTimeOffset? absoluteExpiration) : base(absoluteExpiration)
        {
        }

        public CacheEntryOptions(TimeSpan? absoluteExpirationRelativeToNow) : base(absoluteExpirationRelativeToNow)
        {

        }
    }
}
