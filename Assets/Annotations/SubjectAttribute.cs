using Annotations.Enums;
using System;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SubjectAttribute : Attribute
    {
        public Asset AssetType { get; set; }

        public Type EntityType { get; set; }

        public Type ContextType { get; set; }

        public Type UnityEventType { get; set; }

        public SubjectAttribute() { }

        public SubjectAttribute(Asset assetType, Type entityType, Type contextType)
        {
            AssetType = assetType;
            EntityType = entityType;
            ContextType = contextType;
        }

        public override string ToString()
        {
           return $"AssetType: {AssetType}, SubjectType: {EntityType}, ContextType: {ContextType}, UnityEventType: {UnityEventType}";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}