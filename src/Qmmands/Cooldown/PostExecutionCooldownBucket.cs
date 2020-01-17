using System;
using System.Threading;

namespace Qmmands
{
    internal sealed class PostExecutionCooldownBucket : CooldownBucket
    {
        public int Pending => Volatile.Read(ref _pending);
        private int _pending;

        public override bool HoldsInformation => Remaining < Pending || base.HoldsInformation;

        public PostExecutionCooldownBucket(Cooldown cooldown)
            : base(cooldown)
        {
            _pending = cooldown.Amount;
        }

        public override bool IsRateLimited(out TimeSpan retryAfter)
        {
            var now = DateTimeOffset.UtcNow;

            lock (_lock)
            {
                if (Remaining != Cooldown.Amount && Remaining == Pending && now > Window + Cooldown.Per)
                {
                    _remaining = Cooldown.Amount;
                    _pending = Cooldown.Amount;
                }

                if (Pending == 0)
                {
                    retryAfter = Cooldown.Per - (now - Window);
                    return true;
                }

                retryAfter = default;

                return Remaining == 0;
            }
        }

        public override void Decrement()
        {
            lock (_lock)
            {
                _remaining--;
            }
        }

        public void DecrementPending()
        {
            var now = DateTimeOffset.UtcNow;

            lock (_lock)
            {
                _lastCall = now;

                if (Pending == Cooldown.Amount)
                    Window = now;

                if (now > Window + Cooldown.Per)
                {
                    _remaining = Cooldown.Amount;
                    _pending = Cooldown.Amount;
                    Window = now;
                }

                _pending--;

                if (Pending == 0)
                    Window = now;
            }
        }

        public override void Reset()
        {
            lock (_lock)
            {
                _remaining = Cooldown.Amount;
                _pending = Cooldown.Amount;
                _lastCall = default;
                Window = default;
            }
        }
    }
}