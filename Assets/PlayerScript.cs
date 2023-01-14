using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    float speed = 10.0f;
    Rigidbody2D body;
    [SerializeField]
    Camera mainCamera;
    Vector3 originalPosition;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        originalPosition = mainCamera.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = Vector3.zero;
           
        if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            velocity.x = -speed;
        }
        else if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            velocity.x = speed;
        }

        if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            velocity.y = speed;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            velocity.y = -speed;
        }

        body.velocity = velocity;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Geometry" && Singleton.Instance()._cameraShake)
        {
            Debug.Log("Bang!");
            StartCoroutine(Shake(originalPosition));
        }
    }

    //take out and put in singleton?
    //(?)create a playermanager script that will manage
    //sprite rendering based on how much hp is left
    //movement
    //collision
    IEnumerator Shake(Vector3 originalPos)
    {
        for(float i = 0; i < 1; i += Time.deltaTime)
        {
            mainCamera.transform.localPosition = originalPos + Random.insideUnitSphere * 0.4f;
            yield return null;
        }
        mainCamera.transform.localPosition = originalPos;

    }
}
