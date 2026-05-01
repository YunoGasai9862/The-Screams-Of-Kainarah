
using System;

namespace Assets.Scripts.Interfaces.Registry
{
    public interface IRegistry
    {
        public void Decommission(Int32 instanceId);

        public void Register<T>(T value);
    }
}
