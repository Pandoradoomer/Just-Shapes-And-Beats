using Assets.Geometries.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Sequence Element", menuName = "Sequence Element/Spawn Sequence Element")]
public class SpawnSequenceElement : SequenceElement
{
    public SpawnGeometry spawnGeometry;
    public float lifeTime;

    private void OnValidate()
    {
        Type = SequenceType.SPAWN;
    }
}
