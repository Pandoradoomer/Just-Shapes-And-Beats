using Assets.Geometries.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SequenceCyclicalWaypointElement))]
public class SequenceCyclicalWaypointElementEditor : Editor
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
        SequenceCyclicalWaypointElement element = (SequenceCyclicalWaypointElement)target;

        //shorthand for class variables
        Vector2 spawnPoint = element.spawnPoint;
        Vector2 endPoint = element.endPoint;
        List<Vector2> waypoints = element.waypoints;

        //shorthand for new position variables
        Vector2 newSpawnPoint;
        Vector2 newEndPoint;
        List<Vector2> newWaypoints = new List<Vector2>();

        EditorGUI.BeginChangeCheck();
        Handles.color = Color.yellow;

        newSpawnPoint = Handles.PositionHandle(spawnPoint, Quaternion.identity);
        newEndPoint = Handles.PositionHandle(endPoint, Quaternion.identity);

        if (!element.showOnlyCycle)
        {
            Handles.Label(spawnPoint, "Spawn");
            Handles.Label(endPoint, "End");
            if (waypoints.Count > 0)
            {
                Handles.DrawLine(spawnPoint, waypoints[0]);
                for (int i = 0; i < waypoints.Count - 1; i++)
                {
                    Handles.Label(waypoints[i], $"Waypoint {i}");
                    newWaypoints.Add(Handles.PositionHandle(waypoints[i], Quaternion.identity));
                    Handles.DrawLine(waypoints[i], waypoints[i + 1]);
                }
                if (waypoints.Count >= 1)
                {
                    Handles.Label(waypoints[waypoints.Count - 1], $"Waypoint {waypoints.Count - 1}");
                    newWaypoints.Add(Handles.PositionHandle(waypoints[waypoints.Count - 1], Quaternion.identity));
                }
                Handles.DrawLine(waypoints[waypoints.Count - 1], endPoint);
            }
            else
            {
                Handles.DrawLine(spawnPoint, endPoint);
            }
        }
        if(element.showOnlyCycle)
        {
            int count = 1;
            for(int i = element.cycleStart; i <=element.cycleEnd; i++)
            {
                Handles.Label(waypoints[i], $"Cycle Waypoint {count++}");
            }
        }
        for(int i = element.cycleStart; i < element.cycleEnd; i++)
        {
            Handles.DrawLine(waypoints[i], waypoints[i + 1]);
        }
        Handles.DrawLine(waypoints[element.cycleEnd], waypoints[element.cycleStart]);
        if (EditorGUI.EndChangeCheck())
        {
            newSpawnPoint = new Vector2(Mathf.Round(newSpawnPoint.x * 4) / 4.0f, Mathf.Round(newSpawnPoint.y * 4) / 4.0f);
            newEndPoint = new Vector2(Mathf.Round(newEndPoint.x * 4) / 4.0f, Mathf.Round(newEndPoint.y * 4) / 4.0f);

            for (int i = 0; i < newWaypoints.Count; i++)
            {
                newWaypoints[i] = new Vector2(Mathf.Round(newWaypoints[i].x * 4) / 4.0f, Mathf.Round(newWaypoints[i].y * 4) / 4.0f);
            }

            element.spawnPoint = newSpawnPoint;
            element.endPoint = newEndPoint;
            element.waypoints = newWaypoints;
        }
    }
}
