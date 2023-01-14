using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Geometries.Scripts
{
    /// <remarks>
    /// Decide whether it's better to have the Geometries as non-scriptables
    /// For every custom sequence, there would need to be a special scriptable object defined with rigid,
    /// unmodifiable properties that would ultimately defeat the purpose of having scriptable objects in the first place
    /// At the same time, everything would overload the sequence element, which would require the creation of multiple
    /// sequence elements;
    /// 
    /// At the end of the day, a sequence would require a huge number of config data entries that would be encapsulated by
    /// either 
    /// a combination of bespoke scriptable geometries and bespoke sequence elements (limited, but easy to define)
    /// OR
    /// a combination of adaptable scriptable geometries and bespoke sequence elements (much more adaptable, but repetitive to define)
    /// 
    /// </remarks>
    public class Geometry
    {
        public Vector3 Scale = Vector3.one;
        public Color _Color = Color.white;
    }
    public class BasicGeometry : Geometry
    {
        //public Sprite _Sprite;
        public Transform _Transform;
    }
}

