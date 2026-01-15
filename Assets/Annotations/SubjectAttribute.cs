using System;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SubjectAttribute : Attribute
    {
        public Asset AssetType { get; set; }
        public Type SubjectType { get; set; }

        public Type ContextType { get; set; }

        public SubjectAttribute() { }

        public SubjectAttribute(Asset assetType, Type subjectType, Type contextType)
        {
            AssetType = assetType;
            SubjectType = subjectType;
            ContextType = contextType;
        }
    }
}