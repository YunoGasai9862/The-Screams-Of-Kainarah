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

        public List<ChildAssetAttribute> ChildAssetAttributes { get; set; }
    }
}
