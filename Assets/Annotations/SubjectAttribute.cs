using System;
using UnityEngine.Events;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SubjectAttribute : Attribute
    {
        public Asset AssetType { get; set; }

        public Type SubjectType { get; set; }

        public Type ContextType { get; set; }

        public Type UnityEventType { get; set; }

        public SubjectAttribute() { }

        public SubjectAttribute(Asset assetType, Type subjectType, Type contextType)
        {
            AssetType = assetType;
            SubjectType = subjectType;
            ContextType = contextType;
        }

        public override string ToString()
        {
           return $"AssetType: {AssetType}, SubjectType: {SubjectType}, ContextType: {ContextType}, UnityEventType: {UnityEventType}";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}