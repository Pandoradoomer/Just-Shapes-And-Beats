using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public float delayBetweenCycles;
    public virtual void OnStart() { }

    public virtual void OnUpdate() { }

    private void Start()
    {
        OnStart();
    }

    private void Update()
    {
        OnUpdate();
    }

}
