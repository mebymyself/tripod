﻿using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotBeConfirmedEmailAddress : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotBeConfirmedEmailAddress(IProcessQueries queries)
            : base(() => Resources.Validation_EmailAddress_IsAlreadyConfirmed)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var email = (string)context.PropertyValue;
            var entity = _queries.Execute(new EmailAddressBy(email)).Result;
            if (entity == null || !entity.IsConfirmed) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustNotBeConfirmedEmailAddressExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotBeConfirmedEmailAddress<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBeConfirmedEmailAddress(queries));
        }
    }
}
