using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName ="Flash Sine Curve", menuName = "Sequence Element/Flash Sine Curve")]
public class FlashDynamicSineElement : FlashSequenceElement
{
    [Header("Sine Curve")]
    [Tooltip("sin(ax) where sineMultiplier is a")]
    public float sineMultiplier = 1;
    [Tooltip("a*sin(x) where functionMultiplier is a")]
    public float functionMultiplier = 1;
    public float xDeviation = 0;
    public float yDeviation;
    [Header("Element Properties")]
    public float distance;
    public float delay;
    public Vector2 startPoint;
    public Vector2 endPoint;
    [Header("Behaviour and Calculations")]
    public bool UseYAxis;

    public void OnValidate()
    {
        Type = SequenceType.FLASH_SINE_CURVE;
    }
}



