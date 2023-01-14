using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rescaler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    List<Transform> itemsToRescale;

    private 
    void Start()
    {
        //Rescale();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        //if(gameObject.activeSelf == false)
        //    Unscale();
    }


    public void Rescale()
    {
        Vector3 parentScale = transform.localScale;
        float scale = parentScale.x >= parentScale.y ? parentScale.y : parentScale.x;
        foreach (Transform t in itemsToRescale)
        {
            Vector3 currScale = t.localScale;
            Vector3 pos = t.localPosition;
            t.localScale = new Vector3(scale * currScale.x / parentScale.x, scale * currScale.y / parentScale.y, currScale.z / parentScale.z);
            t.localPosition = new Vector3(scale * pos.x / parentScale.x, scale * pos.y / parentScale.y, pos.z / parentScale.z);
        }
    }
    public void Unscale()
    {
        Vector3 parentScale = transform.localScale;
        float scale = parentScale.x >= parentScale.y ? parentScale.y : parentScale.x;
        foreach (Transform t in itemsToRescale)
        {
            Vector3 currScale = t.localScale;
            Vector3 pos = t.localPosition;
            t.localScale = new Vector3(parentScale.x * currScale.x / scale, parentScale.y * currScale.y / scale, 1);
            t.localPosition = new Vector3(parentScale.x * pos.x / scale, parentScale.y * pos.y / scale, pos.z);
        }
    }
}
