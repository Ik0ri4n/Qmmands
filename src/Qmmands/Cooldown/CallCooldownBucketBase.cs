using System.Threading;

namespace Qmmands
{
    internal abstract class CallCooldownBucketBase : CooldownBucketBase
    {
        public int Pending => Volatile.Read(ref _pending);
        protected int _pending;

        protected CallCooldownBucketBase(Cooldown cooldown) : base(cooldown)
        {
            _pending = Cooldown.Amount;
        }

        public abstract void DecrementPending();
    }
}
