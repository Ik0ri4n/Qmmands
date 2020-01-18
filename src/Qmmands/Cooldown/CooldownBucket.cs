using System;

namespace Qmmands
{
    internal sealed class CooldownBucket : CooldownBucketBase
    {
        public DateTimeOffset Window { get; private set; }

        public override bool HoldsInformation => DateTimeOffset.UtcNow <= _lastCall + Cooldown.Per;
        private DateTimeOffset _lastCall;

        public CooldownBucket(Cooldown cooldown) : base(cooldown) { }

        public override bool IsRateLimited(out TimeSpan retryAfter)
        {
            var now = DateTimeOffset.UtcNow;

            lock (_lock)
            {
                _lastCall = now;

                if (Remaining == Cooldown.Amount)
                    Window = now;

                if (now > Window + Cooldown.Per)
                {
                    _remaining = Cooldown.Amount;
                    Window = now;
                }

                if (Remaining == 0)
                {
                    retryAfter = Cooldown.Per - (now - Window);
                    return true;
                }

                retryAfter = default;
                return false;
            }
        }

        public override void Decrement()
        {
            var now = DateTimeOffset.UtcNow;
            lock (_lock)
            {
                _remaining--;

                if (Remaining == 0)
                    Window = now;
            }
        }

        public override void Reset()
        {
            lock (_lock)
            {
                _remaining = Cooldown.Amount;
                _lastCall = default;
                Window = default;
            }
        }
    }
}