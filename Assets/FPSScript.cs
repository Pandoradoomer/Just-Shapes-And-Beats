using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    TextMeshProUGUI m_TextMeshPro;
    float time = 0.5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if(time < 0)
        {
            float current = 1f / Time.unscaledDeltaTime;
            m_TextMeshPro.text = $"FPS: {(int)current}";
            time = 0.5f;
        }
    }
}
