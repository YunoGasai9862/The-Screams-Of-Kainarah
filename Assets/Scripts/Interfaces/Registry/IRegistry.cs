using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Interfaces.Registry
{
    public interface IRegistry
    {
        void Register();

        void Decommission();
    }
}
