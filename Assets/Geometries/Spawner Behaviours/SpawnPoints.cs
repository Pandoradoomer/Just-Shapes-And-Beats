using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public struct ProjectileData
{
    public Vector2 Pos;
    public Vector2 Direction;
}

[CreateAssetMenu(fileName = "Spawn Points", menuName = "Spawning Points")]
public class SpawnPoints : ScriptableObject
{
    public List<ProjectileData> spawningPoints;
}

[CustomEditor(typeof(SpawnPoints))]
public class SpawnPointsEditor : Editor
{
    private void OnValidate()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
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
        SpawnPoints points = (SpawnPoints)target;

        List<ProjectileData> spawningPoints = points.spawningPoints;
        List<Vector2> newPositions = new List<Vector2>();
        List<Vector2> newDirections = new List<Vector2>();

        EditorGUI.BeginChangeCheck();

        Handles.color = Color.yellow;
        int count = 1;
        foreach(var sp in spawningPoints)
        {
            newPositions.Add(Handles.PositionHandle(sp.Pos, Quaternion.identity));
            newDirections.Add(Handles.PositionHandle(sp.Pos + sp.Direction, Quaternion.identity));
            Handles.Label(sp.Pos, $"Spawning Point {count++}");
            Handles.DrawWireDisc(sp.Pos, Vector3.forward, 0.1f);
            Handles.DrawLine(sp.Pos, sp.Pos + sp.Direction);

            
        }

        if(EditorGUI.EndChangeCheck())
        {
            for(int i = 0; i< newPositions.Count; i++)
            {
                newPositions[i] = new Vector2(Mathf.Round(newPositions[i].x * 4) / 4.0f, Mathf.Round(newPositions[i].y * 4) / 4.0f);
                newDirections[i] = new Vector2(Mathf.Round((newPositions[i].x + newDirections[i].x) * 4) / 4.0f, Mathf.Round(newPositions[i].y + newDirections[i].y * 4) / 4.0f);
            }
            List<ProjectileData> newSpawnPoints = new List<ProjectileData>();
            for(int i = 0; i < spawningPoints.Count; i++)
            {
                newSpawnPoints.Add(new ProjectileData
                {
                    Pos = newPositions[i],
                    Direction = newDirections[i].normalized
                });
            }
            points.spawningPoints = newSpawnPoints;
        }
    }
}
