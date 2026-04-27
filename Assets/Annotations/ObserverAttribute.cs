using Annotations.Enums;
using Assets.Annotations.Interfaces;
using System;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ObserverAttribute : Attribute, IData
    {
        public Asset AssetType { get; set; }

        public Type SubjectType { get; set; }

        public Type EntityType { get; set; }

        public Type ContextType { get; set; }

        public ObserverAttribute() { }

        public ObserverAttribute(Asset asset, Type subjectType, Type entityType, Type contextType)
        {
            AssetType = asset;
            SubjectType = subjectType;
            EntityType = entityType;
            ContextType = contextType;
        }

        public override string ToString()
        {
            return $"SubjectType: {SubjectType}, ObserverType: {EntityType}, ContextType: {ContextType}";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
