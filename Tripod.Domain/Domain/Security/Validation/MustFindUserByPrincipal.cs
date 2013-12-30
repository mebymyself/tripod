﻿using System;
using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class MustFindUserByPrincipal : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindUserByPrincipal(IProcessQueries queries)
            : base(() => Resources.Validation_DoesNotExist)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var principal = (IPrincipal)context.PropertyValue;
            if (principal != null)
            {
                var userId = principal.Identity.GetUserId();
                if (userId != null)
                {
                    var entity = _queries.Execute(new UserBy(int.Parse(userId))).Result;
                    if (entity != null) return true;
                }
            }

            context.MessageFormatter.AppendArgument("PropertyValue", principal != null ? principal.Identity.Name : "");
            return false;
        }
    }

    public static class MustFindUserByPrincipalExtensions
    {
        public static IRuleBuilderOptions<T, IPrincipal> MustFindUserByPrincipal<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindUserByPrincipal(queries));
        }
    }
}
