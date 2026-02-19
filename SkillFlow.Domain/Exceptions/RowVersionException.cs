using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Exceptions
{
    public class MissingRowVersionException : DomainException
    {
        public MissingRowVersionException() : base("RowVersion is required") { }
    }
}
