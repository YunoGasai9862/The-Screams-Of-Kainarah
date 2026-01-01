using System;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SubjectAttribute : Attribute
    {
        public Type SubjectType { get; set; }

        public Type ContextType { get; set; }

        public SubjectAttribute() { }

        public SubjectAttribute(Type subjectType, Type contextType)
        {
            SubjectType = subjectType;
            ContextType = contextType;
        }
    }
}