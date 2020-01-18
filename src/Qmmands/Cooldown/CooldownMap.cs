using System;
using System.Collections.Concurrent;

namespace Qmmands
{
    internal sealed class CooldownMap
    {
        private readonly ConcurrentDictionary<object, CooldownBucketBase> _buckets;

        private readonly Command _command;

        internal CooldownMap(Command command)
        {
            _command = command;
            _buckets = new ConcurrentDictionary<object, CooldownBucketBase>();
        }

        public void Update()
        {
            var buckets = _buckets.ToArray();
            for (var i = 0; i < buckets.Length; i++)
            {
                var bucket = buckets[i];
                if (!bucket.Value.HoldsInformation)
                    _buckets.TryRemove(bucket.Key, out _);
            }
        }

        public void Clear()
            => _buckets.Clear();

        public CooldownBucketBase GetBucket(Cooldown cooldown, CommandContext context)
        {
            var key = _command.Service.CooldownBucketKeyGenerator(cooldown.BucketType, context);
            return key == null ? null : _buckets.GetOrAdd(key, cooldown.MeasuredBeforeExecution
                ? new CooldownBucket(cooldown)
                : cooldown.Per == TimeSpan.Zero
                    ? new CallCooldownBucket(cooldown)
                    : (CooldownBucketBase) new PostExecutionCooldownBucket(cooldown));
        }
    }
}
