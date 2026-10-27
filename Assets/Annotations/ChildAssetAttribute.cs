using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ChildAssetAttribute: Attribute
    {
        private int InstantiationOrder { get; set; }

        private float InstantiationDelay { get; set; }

        ChildAssetAttribute(int instantiationOrder, float instantiationDelay)
        {
            InstantiationOrder = instantiationOrder;
            InstantiationDelay = instantiationDelay;
        }
    }
}
