﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Qmmands
{
    /// <summary>
    ///     Represents the command being on a cooldown.
    /// </summary>
    public sealed class CommandOnCooldownResult : FailedResult
    {
        /// <summary>
        ///     Gets the reason of this failed result.
        /// </summary>
        public override string Reason => _lazyReason.Value;
        private readonly Lazy<string> _lazyReason;

        /// <summary>
        ///     Gets the <see cref="Qmmands.Command"/> that is on cooldown.
        /// </summary>
        public Command Command { get; }

        /// <summary>
        ///     Gets the <see cref="Cooldown"/>s and <see cref="TimeSpan"/>s after which it is safe to retry. 
        ///     If RetryAfter is the default <see cref="TimeSpan"/> value the last call to the <see cref="Qmmands.Command"/> is still running.
        /// </summary>
        public IReadOnlyList<(Cooldown Cooldown, TimeSpan RetryAfter)> Cooldowns { get; }

        private static string GetRetryAddition(Cooldown cooldown, TimeSpan retryAfter)
            => retryAfter != default ? retryAfter.ToString() : "its execution and " + cooldown.Per.ToString();

        internal CommandOnCooldownResult(Command command, IReadOnlyList<(Cooldown Cooldown, TimeSpan RetryAfter)> cooldowns)
        {
            Command = command;
            Cooldowns = cooldowns;

            _lazyReason = new Lazy<string>(() => cooldowns.Count == 1
                ? $"Command {command} is on a '{cooldowns[0].Cooldown.BucketType}' cooldown. Retry after {GetRetryAddition(cooldowns[0].Cooldown, cooldowns[0].RetryAfter)}."
                : $"Command {command} is on multiple cooldowns: {string.Join(", ", cooldowns.Select(x => $"'{x.Cooldown.BucketType}' - retry after {GetRetryAddition(x.Cooldown, x.RetryAfter)}"))}", true);
        }
    }
}
