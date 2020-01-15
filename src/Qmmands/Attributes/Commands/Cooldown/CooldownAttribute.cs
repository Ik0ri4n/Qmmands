using System;

namespace Qmmands
{
    /// <summary>
    ///     Applies a <see cref="Cooldown"/> to the <see cref="Command"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CooldownAttribute : Attribute
    {
        /// <summary>
        ///     Gets the amount of times the <see cref="Command"/> can be used in the given time window.
        /// </summary>
        public int Amount { get; }

        /// <summary>
        ///     Gets the time window of this cooldown.
        /// </summary>
        public TimeSpan Per { get; }

        /// <summary>
        ///     Gets the <see langword="enum"/> bucket type to use with the <see cref="Delegates.CooldownBucketKeyGeneratorDelegate"/>.
        /// </summary>
        public Enum BucketType { get; }

        /// <summary>
        ///     Gets whether to measure the <see cref="Cooldown"/> before or after the <see cref="Command"/> execution.
        /// </summary>
        public bool MeasuredBeforeExecution { get; }

        /// <summary>
        ///     Initialises a new <see cref="CooldownAttribute"/> with the specified <see cref="Cooldown"/> properties 
        ///     and <see cref="MeasuredBeforeExecution"/> set to <see langword="true"/>.
        /// </summary>
        /// <param name="amount"> The amount of uses per given window. </param>
        /// <param name="per"> The bucket time window. </param>
        /// <param name="cooldownMeasure"> The unit of time of the given window. </param>
        /// <param name="bucketType"> The bucket type. Has to be an <see langword="enum"/>. </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Not a valid cooldown measure.
        /// </exception>
        public CooldownAttribute(int amount, double per, CooldownMeasure cooldownMeasure, object bucketType)
            : this(amount, per, cooldownMeasure, bucketType, true)
        { }

        /// <summary>
        ///     Initialises a new <see cref="CooldownAttribute"/> with the specified <see cref="Cooldown"/> properties.
        /// </summary>
        /// <param name="amount"> The amount of uses per given window. </param>
        /// <param name="per"> The bucket time window. </param>
        /// <param name="cooldownMeasure"> The unit of time of the given window. </param>
        /// <param name="bucketType"> The bucket type. Has to be an <see langword="enum"/>. </param>
        /// <param name="measuredBeforeExecution"> Whether the <see cref="Cooldown"/> shall be measured before or after the 
        ///     <see cref="Command"/> execution. </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Not a valid cooldown measure.
        /// </exception>
        public CooldownAttribute(int amount, double per, CooldownMeasure cooldownMeasure, object bucketType, bool measuredBeforeExecution)
        {
            if (!(bucketType is Enum enumBucketType))
                throw new ArgumentException("Bucket type must be an enum.", nameof(bucketType));

            Amount = amount;
            Per = cooldownMeasure switch
            {
                CooldownMeasure.Milliseconds => TimeSpan.FromMilliseconds(per),
                CooldownMeasure.Seconds => TimeSpan.FromSeconds(per),
                CooldownMeasure.Minutes => TimeSpan.FromMinutes(per),
                CooldownMeasure.Hours => TimeSpan.FromHours(per),
                CooldownMeasure.Days => TimeSpan.FromDays(per),
                _ => throw new ArgumentOutOfRangeException(nameof(cooldownMeasure), "Invalid cooldown measure."),
            };
            BucketType = enumBucketType;
            MeasuredBeforeExecution = measuredBeforeExecution;
        }
    }
}
