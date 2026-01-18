using Assets.Annotations;
using System.Collections.Generic;

namespace Assets.Scripts.ObserverPattern.interfaces
{
    public interface IObserverBundle
    {
        INotify Observer { get; set; }
        public ObserverAttribute ObserverAttribute { get; }
    }

    public interface IObserverBundle<T>
    {
        INotify<T> Observer { get; set; }
        public ObserverAttribute ObserverAttribute { get; }
    }
}
