using Annotations.Enums;
using System;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ObserverAttribute : Attribute
    {
        public Asset AssetType { get; set; }

        public Type ObserverType { get; set; }

        public Type SubjectType { get; set; }

        public Type ContextType { get; set; }

        public ObserverAttribute() { }

        public ObserverAttribute(Asset asset, Type observerType, Type subjectType, Type contextType)
        {
            AssetType = asset;
            ObserverType = observerType;
            SubjectType = subjectType;
            ContextType = contextType;
        }

        public override string ToString()
        {
            return $"ObserverType: {ObserverType}, SubjectType: {SubjectType}, ContextType: {ContextType}";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
