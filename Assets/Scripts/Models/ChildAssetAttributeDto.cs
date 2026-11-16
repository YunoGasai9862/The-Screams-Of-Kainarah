using Assets.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class ChildAssetAttributeDto
    {
        public GameObject GameObject { get; set; }

        //the location of the parent (that's where all the childs will be regardless of their own position - v1 design)
        public Vector3 Position { get; set; }
        
        public List<ChildAssetAttribute> ChildAssetAttributes { get; set; }
    }
}
