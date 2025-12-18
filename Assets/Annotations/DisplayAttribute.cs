
using System;

namespace Assets.Annotations
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DisplayAttribute: Attribute
	{
        public string Name { get; }
        public DisplayAttribute(string name)
        {
            Name = name;
        }
	}
}