using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Scene.Interface
{
    public interface IScene
    {
        void Broadcast<T>(T value);
    }
}
