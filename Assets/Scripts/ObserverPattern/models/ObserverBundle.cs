using Assets.Annotations;
using Assets.Scripts.ObserverPattern.interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.ObserverPattern.models
{

    public class ObserverBundle<T> : IObserverBundle<T>
    {
        public INotify<T> Observer { get; set; }

        public ObserverAttribute ObserverAttribute { get; set; }

        public override string ToString()
        {
            return $"ObserverAttribute: {ObserverAttribute}, ObserverIntances : {Observer}";
        }
    }

    public class ObserverBundle: IObserverBundle
    {
        public INotify Observer { get; set; }

        public ObserverAttribute ObserverAttribute { get; set; }

        public override string ToString()
        {
            return $"ObserverAttribute: {ObserverAttribute}, ObserverIntances : {Observer}";
        }
    }
}
