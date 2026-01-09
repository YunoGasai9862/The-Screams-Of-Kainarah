using Assets.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ObserverPattern.models
{
    public class ObserverBundle
    {
        public List<ObserverContext> ObserverContexts { get; set; } = new List<ObserverContext>(); 

        public ObserverAttribute ObserverAttribute { get; set; }

        public override string ToString()
        {
            return $"ObserverAttribute: {ObserverAttribute}, ObserverIntances : {string.Join(",", ObserverContexts.Select(val => val.ToString()))}";
        }
    }
}
