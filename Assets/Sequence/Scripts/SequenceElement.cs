using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Geometries.Scripts;
using System;
public enum SequenceType
{
    FLASH_DYNAMIC,
    FLASH_DYNAMIC_SEQUENCE,
    FLASH_DIRECTION_SEQUENCE,
    FLASH_SINE_CURVE,
    SPAWN,
    WAYPOINT,
    CYCLICAL_WAYPOINT
}
[Serializable]
public class SequenceElement : ScriptableObject
{
    public SequenceType Type { get; protected set; }
    [Tooltip("If WaitUntilEnd is set, pause is how much is waited after the sequence is finished. Otherwise, it's how long until the next sequence element starts")]
    public bool WaitUntilEnd = true;
    public float pause = 0;
}


public class FlashSequenceElement : SequenceElement
{
    //TODO: add delay, every flash sequence would need a delay between elements to be set, even if it has 1 element only
    [Header("Flash Sequence")]
    public FlashGeometry geometry;
}

