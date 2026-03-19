using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.Base;

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
