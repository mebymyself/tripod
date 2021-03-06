﻿using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustBeVerifiedEmailSecretExtensions
    {
        /// <summary>
        /// Validates that this EmailVerification Secret matches that of the EmailVerification with
        /// the provided Ticket.
        /// </summary>
        /// <typeparam name="T">The command with the EmailVerification Ticket to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating EmailVerification by Ticket.</param>
        /// <param name="ticket">The Ticket of the EmailVerification to match the Secret against.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, string> MustBeVerifiedEmailSecret<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries, Func<T, string> ticket)
        {
            return ruleBuilder.SetValidator(new MustBeVerifiedEmailSecret<T>(queries, ticket));
        }
    }

    internal class MustBeVerifiedEmailSecret<T> : PropertyValidator
    {
        private readonly IProcessQueries _queries;
        private readonly Func<T, string> _ticket;

        internal MustBeVerifiedEmailSecret(IProcessQueries queries, Func<T, string> ticket)
            : base(() => Resources.Validation_EmailVerificationSecret_IsWrong)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            if (ticket == null) throw new ArgumentNullException("ticket");
            _queries = queries;
            _ticket = ticket;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var secret = (string)context.PropertyValue;
            var ticket = _ticket((T)context.Instance);
            if (string.IsNullOrWhiteSpace(secret)) return true;
            var entity = _queries.Execute(new EmailVerificationBy(ticket)).Result;
            if (entity == null) return true;
            if (entity.Secret == secret) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }
}
