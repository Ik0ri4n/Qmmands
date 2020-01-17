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

        protected object _lock = new object();

        public CooldownBucket(Cooldown cooldown)
        {
            Cooldown = cooldown;
            _remaining = Cooldown.Amount;
        }

        public virtual bool IsRateLimited(out TimeSpan retryAfter)
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

                _remaining--;

                if (Remaining == 0)
                    Window = now;

                retryAfter = default;
                return false;
            }
        }

        public virtual void Reset()
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