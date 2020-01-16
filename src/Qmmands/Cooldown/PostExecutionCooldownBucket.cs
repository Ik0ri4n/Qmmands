using System;
using System.Threading;

namespace Qmmands
{
    internal sealed class PostExecutionCooldownBucket : CooldownBucket
    {
        public int Executed => Volatile.Read(ref _executed);
        private int _executed;

        public override bool HoldsInformation => Remaining > Executed || base.HoldsInformation;

        public PostExecutionCooldownBucket(Cooldown cooldown)
            : base(cooldown)
        {
            _executed = 0;
        }

        public override bool IsRateLimited(out TimeSpan retryAfter)
        {
            var now = DateTimeOffset.UtcNow;

            if (Executed == 0)
                Window = now;

            if (now > Window + Cooldown.Per)
            {
                _remaining = Cooldown.Amount;
                _executed = 0;
                Window = now;
            }

            if (Executed == Cooldown.Amount)
            {
                retryAfter = Cooldown.Per - (now - Window);
                return true;
            }

            retryAfter = default;
            return Remaining == 0;
        }

        public override void Decrement()
            => Interlocked.Decrement(ref _remaining);

        public void IncrementExecuted()
        {
            var now = DateTimeOffset.UtcNow;
            _lastCall = now;

            Interlocked.Increment(ref _executed);

            if (Executed == Cooldown.Amount)
                Window = now;
        }

        public override void Reset()
        {
            base.Reset();
            _executed = 0;
        }
    }
}