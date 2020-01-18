using System;
using System.Threading;

namespace Qmmands
{
    internal abstract class CooldownBucketBase
    {
        public Cooldown Cooldown { get; }

        public int Remaining => Volatile.Read(ref _remaining);
        protected int _remaining;

        public abstract bool HoldsInformation { get; }

        protected readonly object _lock = new object();

        protected CooldownBucketBase(Cooldown cooldown)
        {
            Cooldown = cooldown;
            _remaining = Cooldown.Amount;
        }

        public abstract bool IsRateLimited(out TimeSpan retryAfter);

        public abstract void Decrement();

        public abstract void Reset();
    }
}
