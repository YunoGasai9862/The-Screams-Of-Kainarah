using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SubjectAttribute : Attribute
    {
        public Type SubjectType { get; set; }

        public Type ContextType { get; set; }

    }
}
