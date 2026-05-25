using Annotations.Enums;
using Assets.Annotations.Interfaces;
using System;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ObserverAttribute : Attribute, IData
    {
        public Asset AssetType { get; set; }

        public Type EntityType { get; set; }

        public Type SubjectType { get; set; }

        public Type ContextType { get; set; }

        public ObserverAttribute() { }

        public override string ToString()
        {
            return $"AssetType: {AssetType}, EntityType: {EntityType}, SubjectType: {SubjectType}, ContextType: {ContextType}";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
