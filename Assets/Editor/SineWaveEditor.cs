using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


[CustomEditor(typeof(FlashDynamicSineElement))]
public class FlashDynamicSineElementEditor : Editor
{
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        FlashDynamicSineElement element = (FlashDynamicSineElement)target;

        Vector2 startPoint = element.startPoint;
        Vector2 endPoint = element.endPoint;
        EditorGUI.BeginChangeCheck();

        Handles.color = Color.yellow;
        Handles.Label(startPoint, "Start");
        Handles.Label(endPoint, "End");

        Vector2 newStartPoint = Handles.PositionHandle(startPoint, Quaternion.identity);
        Vector2 newEndPoint = Handles.PositionHandle(endPoint, Quaternion.identity);
        int numDivisions = 100;
        for(int i = 0; i < numDivisions - 1; i++)
        {
            Vector2 p1 = Vector2.Lerp(startPoint, endPoint, i / (float)numDivisions);
            Vector2 p2 = Vector2.Lerp(startPoint, endPoint, (i+1) / (float)numDivisions);
            if(!element.UseYAxis)
                Handles.DrawLine(new Vector2(p1.x, element.functionMultiplier * Mathf.Sin(element.sineMultiplier * p1.x) + p1.y),
                                new Vector2(p2.x, element.functionMultiplier * Mathf.Sin(element.sineMultiplier * p2.x) + p2.y));
            else
            {
                Handles.DrawLine(new Vector2(p1.x + element.functionMultiplier * Mathf.Sin(element.sineMultiplier * p1.y), p1.y),
                                new Vector2(p2.x + element.functionMultiplier * Mathf.Sin(element.sineMultiplier * p2.y), p2.y));
            }
        
        }

        if(!element.UseYAxis)
        {
            if (startPoint.x > endPoint.x)
            {
                var aux = endPoint;
                endPoint = startPoint;
                startPoint = aux;
            }
        }
        else
        {
            if (startPoint.y > endPoint.y)
            {
                var aux = endPoint;
                endPoint = startPoint;
                startPoint = aux;
            }
        }

        if(!element.UseYAxis)
        {
            for (float i = startPoint.x; i < endPoint.x; i += element.distance + 1)
            {
                float yDeviation = Vector2.Lerp(startPoint, endPoint, (i - startPoint.x) / (endPoint.x - startPoint.x)).y;
                Handles.DrawWireDisc(new Vector2(i, element.functionMultiplier * Mathf.Sin(element.sineMultiplier * i) + yDeviation), Vector3.forward, 0.5f);
            }
        }
        else
        {
            for (float i = startPoint.y; i < endPoint.y; i += element.distance + 1)
            {
                float xDeviation = Vector2.Lerp(startPoint, endPoint, (i - startPoint.y) / (endPoint.y - startPoint.y)).x;
                Handles.DrawWireDisc(new Vector2(element.functionMultiplier * Mathf.Sin(element.sineMultiplier * i) + xDeviation, i), Vector3.forward, 0.5f);
            }
        }

        if(EditorGUI.EndChangeCheck())
        {
            newStartPoint = new Vector2(Mathf.Round(newStartPoint.x * 4) / 4.0f, Mathf.Round(newStartPoint.y * 4) / 4.0f);
            newEndPoint = new Vector2(Mathf.Round(newEndPoint.x * 4) / 4.0f, Mathf.Round(newEndPoint.y * 4) / 4.0f);

            element.startPoint = newStartPoint;
            element.endPoint = newEndPoint;
        }

    }
}
