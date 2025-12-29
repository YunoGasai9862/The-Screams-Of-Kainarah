using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ObserverAttribute : Attribute
    {
        public Type ObserverType { get; set; }

        public Type SubjectType { get; set; }

        public Type ContextType { get; set; }
    }
}
