using Assets.Geometries.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Cyclical Waypoint Sequence Element", menuName = "Sequence Element/Cyclical Waypoint Sequence Element")]
public class SequenceCyclicalWaypointElement : SequenceWaypointElement
{
    [Header("Cycles")]
    public int numCycles = 1;
    public int cycleStart = 0;
    public int cycleEnd = 1;

    [Header("Inspector Variables")]
    public bool showOnlyCycle = false;

    private void OnValidate()
    {
        Type = SequenceType.CYCLICAL_WAYPOINT;
        if(cycleEnd >= waypoints.Count)
            cycleEnd = waypoints.Count - 1;
        if (cycleEnd <= 0)
            cycleEnd = 1;
        if (cycleStart < 0)
            cycleStart = 0;
        if (cycleStart >= cycleEnd)
            cycleStart = cycleEnd - 1;
    }

}