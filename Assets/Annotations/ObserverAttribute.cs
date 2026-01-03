using System;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ObserverAttribute : Attribute
    {
        public Context Context { get; set; }

        public Type SubjectType { get; set; }

        public Type DataType { get; set; }

        public ObserverAttribute() { }

        public ObserverAttribute(Context context, Type subjectType, Type contextType)
        {
            Context = context;
            SubjectType = subjectType;
            DataType = contextType;
        }
    }
}
