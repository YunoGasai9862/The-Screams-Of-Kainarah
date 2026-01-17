using Assets.Annotations;
using System.Collections.Generic;

namespace Assets.Scripts.ObserverPattern.interfaces
{
    public interface IObserverBundle
    {
        List<INotify> Observers { get; set; }
        public ObserverAttribute ObserverAttribute { get; }
    }

    public interface IObserverBundle<T>
    {
        List<INotify<T>> Observers { get; set; }
        public ObserverAttribute ObserverAttribute { get; }
    }
}
