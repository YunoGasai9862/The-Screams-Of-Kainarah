using System;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ObserverAttribute : Attribute
    {
        public Type ObserverType { get; set; }

        public Type SubjectType { get; set; }

        public Type DataType { get; set; }

        public ObserverAttribute() { }

        public ObserverAttribute(Type observerType, Type subjectType, Type contextType)
        {
            ObserverType = observerType;
            SubjectType = subjectType;
            DataType = contextType;
        }
    }
}
