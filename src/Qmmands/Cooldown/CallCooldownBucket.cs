using System;

namespace Qmmands
{
    internal sealed class CallCooldownBucket : CallCooldownBucketBase
    {
        public override bool HoldsInformation => Remaining < Pending;

        public CallCooldownBucket(Cooldown cooldown) : base(cooldown) { }

        public override bool IsRateLimited(out TimeSpan retryAfter)
        {
            lock (_lock)
            {
                if (Remaining != Cooldown.Amount && Remaining == Pending)
                {
                    _remaining = Cooldown.Amount;
                    _pending = Cooldown.Amount;
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

        public override void DecrementPending()
        {
            lock (_lock)
            {
                _pending--;
            }
        }

        public override void Reset()
        {
            lock (_lock)
            {
                _remaining = Cooldown.Amount;
                _pending = Cooldown.Amount;
            }
        }
    }
}
