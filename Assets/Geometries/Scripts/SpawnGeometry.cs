using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Geometries.Scripts
{
    [Serializable]
    public class SpawnGeometry : Geometry
    {
        public Vector2 Position;
        [Tooltip("The prefab for the spawner object")]
        public Transform Spawner;
    }
}

