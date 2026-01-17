using Assets.Annotations;
using Assets.Scripts.ObserverPattern.interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.ObserverPattern.models
{

    public class ObserverBundle<T> : IObserverBundle<T>
    {
        public List<INotify<T>> Observers { get; set; } = new List<INotify<T>>();

        public ObserverAttribute ObserverAttribute { get; set; }

        public override string ToString()
        {
            return $"ObserverAttribute: {ObserverAttribute}, ObserverIntances : {string.Join(",", Observers.Select(val => val.ToString()))}";
        }
    }

    public class ObserverBundle: IObserverBundle
    {
        public List<INotify> Observers { get; set; } = new List<INotify>(); 

        public ObserverAttribute ObserverAttribute { get; set; }

        public override string ToString()
        {
            return $"ObserverAttribute: {ObserverAttribute}, ObserverIntances : {string.Join(",", Observers.Select(val => val.ToString()))}";
        }
    }
}
