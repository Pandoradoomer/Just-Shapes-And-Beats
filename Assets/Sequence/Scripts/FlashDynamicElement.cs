using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flash Dynamic Object", menuName = "Sequence Element/Flash Dynamic")]
public class FlashDynamicElement : FlashSequenceElement
{
    [Header("Dynamic Element")]
    public int repetitions;
    public float delayBetweenRepetitions;

    public void OnValidate()
    {
        Type = SequenceType.FLASH_DYNAMIC;
    }
}

