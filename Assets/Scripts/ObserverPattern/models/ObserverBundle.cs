using Assets.Annotations;
using Assets.Scripts.ObserverPattern.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ObserverPattern.models
{
    public class ObserverBundle<T>: IObserverBundle
    {
        public List<INotify<T>> Observers { get; set; } = new List<INotify<T>>(); 

        public ObserverAttribute ObserverAttribute { get; set; }

        public override string ToString()
        {
            return $"ObserverAttribute: {ObserverAttribute}, ObserverIntances : {string.Join(",", Observers.Select(val => val.ToString()))}";
        }

        ObserverAttribute IObserverBundle.GetObserverAttribute()
        {
            return ObserverAttribute;
        }
    }
}
