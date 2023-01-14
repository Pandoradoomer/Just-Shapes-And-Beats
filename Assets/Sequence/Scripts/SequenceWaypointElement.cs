using Assets.Geometries.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "WaypointSequenceElement", menuName = "Sequence Element/Waypoint Sequence Element")]
public class SequenceWaypointElement : SequenceElement
{
    [Header("Geometry")]
    public Transform Traveller;
    [Header("Travel Elements")]
    public Vector2 spawnPoint;
    public List<Vector2> waypoints;
    public Vector2 endPoint;
    public float velocity;
    [Header("Traveller spawning")]
    [Tooltip("How many travellers should be spawned")]
    public int numberOfTravellers = 1;
    public float delayBetweenTravellers;
    [Header("Behaviour Elements")]
    [Tooltip("Whether the sprite should reorient itself to follow the direction or remain static")]
    public bool shouldRotate;

    private void OnValidate()
    {
        Type = SequenceType.WAYPOINT;
    }
}

[CustomEditor(typeof(SequenceWaypointElement))]
public class SequenceWaypointElementEditor : Editor
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
        SequenceWaypointElement element = (SequenceWaypointElement)target;

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
        if(EditorGUI.EndChangeCheck())
        {
            newSpawnPoint = new Vector2(Mathf.Round(newSpawnPoint.x * 4) / 4.0f, Mathf.Round(newSpawnPoint.y * 4) / 4.0f);
            newEndPoint = new Vector2(Mathf.Round(newEndPoint.x * 4) / 4.0f, Mathf.Round(newEndPoint.y * 4) / 4.0f);
            
            for(int i = 0; i < newWaypoints.Count; i++)
            {
                newWaypoints[i] = new Vector2(Mathf.Round(newWaypoints[i].x * 4) / 4.0f, Mathf.Round(newWaypoints[i].y * 4) / 4.0f);
            }
            
            element.spawnPoint = newSpawnPoint;
            element.endPoint = newEndPoint;
            element.waypoints = newWaypoints;
        }
    }
}
