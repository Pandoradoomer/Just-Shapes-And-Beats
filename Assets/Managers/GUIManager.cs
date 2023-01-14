using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField]
    Sequence LaserSequence;
    [SerializeField]
    Sequence WaveSequence;
    [SerializeField]
    Sequence WaypointSequence;
    [SerializeField]
    Sequence SpawnerSequence;

    private void OnGUI()
    {

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();

        if(GUILayout.Button("Laser 1 - Horizontal"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.ParseSequenceElement(LaserSequence.sequence[0]));
        }
        if(GUILayout.Button("Laser 2 - Vertical"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.ParseSequenceElement(LaserSequence.sequence[1]));
        }
        if(GUILayout.Button("Laser 3 - Dynamic"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.ParseSequenceElement(LaserSequence.sequence[2]));
        }
        if(GUILayout.Button("Wave 1 - Horizontal"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.StartSequence(WaveSequence.sequence.Take(3).ToList()));
        }
        if (GUILayout.Button("Wave 2 - Vertical"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.StartSequence(WaveSequence.sequence.Skip(3).Take(4).ToList()));
        }
        if(GUILayout.Button("Wave 3 - Sines"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.StartSequence(WaveSequence.sequence.Skip(7).Take(4).ToList()));
        }
        if(GUILayout.Button("Wave 4 - Onslaught"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.ParseSequenceElement(WaveSequence.sequence[11]));  
        }
        if(GUILayout.Button("Wave 5 - Follower"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.ParseSequenceElement(WaveSequence.sequence[12]));
        }
        if(GUILayout.Button("Waypoints 1 - Peaks"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.ParseSequenceElement(WaypointSequence.sequence[0]));
        }
        if (GUILayout.Button("Waypoints 2 - Sweeps"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.ParseSequenceElement(WaypointSequence.sequence[1]));
        }
        if (GUILayout.Button("Waypoints 3 - Square Cycle"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.ParseSequenceElement(WaypointSequence.sequence[2]));
        }
        if(GUILayout.Button("Waypoints 4 - Intersect Cycle"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.ParseSequenceElement(WaypointSequence.sequence[3]));
        }
        if(GUILayout.Button("Spawner 1 - Mother"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.ParseSequenceElement(SpawnerSequence.sequence[0]));
        }
        if (GUILayout.Button("Spawner 2 - Spammer"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.ParseSequenceElement(SpawnerSequence.sequence[1]));
        }
        if (GUILayout.Button("Spawner 3 - Jiggler"))
        {
            StartCoroutine(Singleton.Instance().MasterScript.ParseSequenceElement(SpawnerSequence.sequence[2]));
        }
        Singleton.Instance()._cameraShake = GUILayout.Toggle(Singleton.Instance()._cameraShake, "Camera Shake");

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}
