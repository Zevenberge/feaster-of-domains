using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace FeasterOfDomains.Users.Web
{
    public class IdentityException<Type> : Exception
    {
        public IdentityException(string name, IEnumerable<IdentityError> errors)
            : base($"Failure when trying to register {typeof(Type).Name} {name}:\n"
            + string.Join("\n", errors.Select(x => $"{x.Code} :: {x.Description}")))
        {
        }
    }
}