using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Exceptions
{
    public sealed class ConcurrencyException(Exception? innerException = null) : 
        DomainException("Someone just updated the session, please try again", innerException);
}
