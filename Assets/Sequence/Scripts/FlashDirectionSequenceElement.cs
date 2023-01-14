using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "FlashDirectionSequence", menuName = "Sequence Element/Flash Direction Sequence")]
public class FlashDirectionSequenceElement : FlashSequenceElement
{
    [Header("Sequence Information")]
    public Vector2 startPoint;
    [Tooltip("Insert it as the point to go towards")]
    public Vector2 direction;
    [Header("Element Information")]
    public float elementSpace;
    public int numElements;
    public float delayBetweenElements;

    [Header("Behaviour Information")]
    [Tooltip("If it's set to true, the direction will be set to the player's location at runtime")]
    public bool isDynamic;
    [Tooltip("If isDynamic is not true, this is ignored; If this is true, the direction is recalculated after each element")]
    public bool shouldRecalculate;
    [Tooltip("If shouldRecalculate is not true, this is ignored; If this is true, the element uses the previous step as its reference")]
    public bool shouldStep;
    public bool shouldRotate;
    [Tooltip("By default, element space uses the X axis (sprite width) to space elements. Tick this to use the Y axis (sprite height)")]
    public bool useYAxisForCalculations;

    public new void OnValidate()
    {
        Type = SequenceType.FLASH_DIRECTION_SEQUENCE;
    }

}

[CustomEditor(typeof(FlashDirectionSequenceElement))]
public class FlashDirectionSequenceElementEditor : Editor
{
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    void OnSceneGUI(SceneView sceneView)
    {
        FlashDirectionSequenceElement element = (FlashDirectionSequenceElement)target;
        Vector2 startPoint = element.startPoint;
        Vector2 endPoint = element.direction;

        EditorGUI.BeginChangeCheck();
        Vector2 newStartPoint = Handles.PositionHandle(startPoint, Quaternion.identity);
        Vector2 newEndPoint = Handles.PositionHandle(endPoint, Quaternion.identity);

        Vector2 dir = (endPoint - startPoint).normalized;
        Quaternion rotation = Quaternion.identity;
        if(element.shouldRotate)
        {
            Vector2 reference;
            if (element.useYAxisForCalculations)
                reference = Vector2.up;
            else
                reference = Vector2.right;
            float rotationAngle = Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(dir, reference));
            if (dir.y < 0 && !element.useYAxisForCalculations)
                rotationAngle = -rotationAngle;
            if (dir.x > 0 && element.useYAxisForCalculations)
                rotationAngle = -rotationAngle;
            rotation = Quaternion.Euler(0, 0, rotationAngle);

        }

        Handles.color = Color.yellow;
        Handles.Label(startPoint, "Start");
        Handles.Label(endPoint, "Direction");
        Handles.DrawLine(startPoint, endPoint);
        for(int i =0 ; i < element.numElements; i++)
        {
            if(element.useYAxisForCalculations)
                Handles.matrix = Matrix4x4.TRS(startPoint + i * (element.elementSpace + element.geometry.Scale.y) * dir, rotation, element.geometry.Scale);
            else
                Handles.matrix = Matrix4x4.TRS(startPoint + i * (element.elementSpace + element.geometry.Scale.x) * dir, rotation, element.geometry.Scale);
            Handles.DrawWireCube(Vector3.zero, Vector3.one);
        }
        if (EditorGUI.EndChangeCheck())
        {
            newStartPoint = new Vector2(Mathf.Round(newStartPoint.x * 4) / 4.0f, Mathf.Round(newStartPoint.y * 4) / 4.0f);
            newEndPoint = new Vector2(Mathf.Round(newEndPoint.x * 4) / 4.0f, Mathf.Round(newEndPoint.y * 4) / 4.0f);
            
            element.startPoint = newStartPoint;
            element.direction = newEndPoint;
        }
    }
}
