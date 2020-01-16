using System;
using System.Threading;

namespace Qmmands
{
    internal class CooldownBucket
    {
        public Cooldown Cooldown { get; }

        public int Remaining => Volatile.Read(ref _remaining);
        protected int _remaining;

        public DateTimeOffset Window { get; protected set; }

        public virtual bool HoldsInformation => DateTimeOffset.UtcNow <= _lastCall + Cooldown.Per;
        protected DateTimeOffset _lastCall;

        public CooldownBucket(Cooldown cooldown)
        {
            Cooldown = cooldown;
            _remaining = Cooldown.Amount;
        }

        public virtual bool IsRateLimited(out TimeSpan retryAfter)
        {
            var now = DateTimeOffset.UtcNow;
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

        public virtual void Decrement()
        {
            var now = DateTimeOffset.UtcNow;
            Interlocked.Decrement(ref _remaining);

            if (Remaining == 0)
                Window = now;
        }

        public virtual void Reset()
        {
            _remaining = Cooldown.Amount;
            _lastCall = default;
            Window = default;
        }
    }
}