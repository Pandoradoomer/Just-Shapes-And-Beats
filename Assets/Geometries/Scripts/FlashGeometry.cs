using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Geometries.Scripts
{

    [CreateAssetMenu]
    [Serializable]
    public class FlashGeometry : BasicGeometry
    {
        public float buildUpTime;
        public float stayAliveTime;
        public float fadeOutTime;
    }
}
