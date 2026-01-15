using Assets.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.ObserverPattern.interfaces
{
    public interface IObserverBundle
    {
        public ObserverAttribute GetObserverAttribute();
    }
}
