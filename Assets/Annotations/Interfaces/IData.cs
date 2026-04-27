using Annotations.Enums;
using System;

namespace Assets.Annotations.Interfaces
{
    public interface IData
    {
        public Asset AssetType { get; set; }

        public Type EntityType { get; set; }

        public Type ContextType { get; set; }
    }
}
