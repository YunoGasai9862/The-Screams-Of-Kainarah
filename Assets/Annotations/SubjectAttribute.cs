using Annotations.Enums;
using Assets.Annotations.Interfaces;
using System;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SubjectAttribute : Attribute, IData
    {
        public Asset AssetType { get; set; }

        public Type EntityType { get; set; }

        public Type ContextType { get; set; }

        public Type UnityEventType { get; set; }

        public SubjectAttribute() { }

        public override string ToString()
        {
           return $"AssetType: {AssetType}, EntityType: {EntityType}, ContextType: {ContextType}, UnityEventType: {UnityEventType}";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}