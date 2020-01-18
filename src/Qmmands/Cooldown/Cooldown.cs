﻿using System;

namespace Qmmands
{
    /// <summary>
    ///     Represents a <see cref="Command"/> cooldown.
    /// </summary>
    public sealed class Cooldown
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
        ///     Initialises a new <see cref="Cooldown"/> with the specified properties and 
        ///     <see cref="MeasuredBeforeExecution"/> set to <see langword="true"/>.
        /// </summary>
        /// <param name="amount"> The amount of uses per given window. </param>
        /// <param name="per"> The bucket time window. </param>
        /// <param name="bucketType"> The <see langword="enum"/> bucket type. </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Amount and per must be positive values.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Bucket type must not be <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Bucket type must be an <see langword="enum"/>.
        /// </exception>
        public Cooldown(int amount, TimeSpan per, Enum bucketType)
            : this(amount, per, bucketType, true)
        { }

        /// <summary>
        ///     Initialises a new <see cref="Cooldown"/> with the specified properties.
        /// </summary>
        /// <param name="amount"> The amount of uses per given window. </param>
        /// <param name="per"> The bucket time window. </param>
        /// <param name="bucketType"> The <see langword="enum"/> bucket type. </param>
        /// <param name="measuredBeforeExecution"> Whether the <see cref="Cooldown"/> shall be measured before or after the 
        ///     <see cref="Command"/> execution. </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Amount and per must be positive values.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Bucket type must not be <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Bucket type must be an <see langword="enum"/>.
        /// </exception>
        public Cooldown(int amount, TimeSpan per, Enum bucketType, bool measuredBeforeExecution)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be a positive integer.");

            if (measuredBeforeExecution)
            {
                if (per <= TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException(nameof(per), "Per must be a positive time span.");
            }
            else
            {
                if (per < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException(nameof(per), "Per must be a positive or zero time span.");
            }

            if (bucketType == null)
                throw new ArgumentNullException(nameof(bucketType), "Bucket type must not be null.");

            Amount = amount;
            Per = per;
            BucketType = bucketType;
            MeasuredBeforeExecution = measuredBeforeExecution;
        }
    }
}
