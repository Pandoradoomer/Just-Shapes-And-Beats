using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlashVerticalSequence", menuName = "Sequence Element/Flash Vertical Sequence")]
public class FlashVerticalSequenceElement : FlashSequenceElement
{
    [Header("Vertical Sequence")]
    [Tooltip("xDeviation set to 0 by default means it starts at the far end of the screen")]
    public float xDeviation = 0;
    [Tooltip("yDeviation set to 0 means spawning with the centre in the middle of the screen")]
    public float yDeviation = 0;
    public float elementSpace;
    [Tooltip("Default is left to right; reverseDirection true makes it right to left")]
    public bool reverseDirection;
    public float delay;

    public void OnValidate()
    {
        //Type = SequenceType.FLASH_VERTICAL_SEQUENCE;
    }

}
