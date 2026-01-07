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
        public List<GameObject> ObserverIntances { get; set; }

        public ObserverAttribute ObserverAttribute { get; set; }

        public override string ToString()
        {
            return $"ObserverAttribute: {ObserverAttribute}, ObserverIntances : {string.Join(",", ObserverIntances.Select(val => val.ToString()))}";
        }
    }
}
