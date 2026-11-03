using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ChildAssetAttribute: Attribute
    {
        public int InstantiationOrder { get; set; }

        public float InstantiationDelay { get; set; }

        ChildAssetAttribute(int instantiationOrder, float instantiationDelay)
        {
            InstantiationOrder = instantiationOrder;
            InstantiationDelay = instantiationDelay;
        }
    }
}
