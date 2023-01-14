using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Projectile
{
    public Transform obj;
    public Vector3 Direction;
    public float speed;
}

public class ProjectileManager : MonoBehaviour
{

    public List<Projectile> projectiles;

    private void Start()
    {
        projectiles = new List<Projectile>();
    }

    private void Update()
    {
        UpdateProjectiles();
        DeleteFarProjectiles();
    }

    void UpdateProjectiles()
    {
        foreach (Projectile p in projectiles)
        {
            p.obj.transform.position = p.obj.transform.position + (Vector3)(p.Direction * p.speed * Time.deltaTime);
        }
    }

    void DeleteFarProjectiles()
    {

        List<Projectile> toRemove = projectiles.FindAll(x => Mathf.Abs(x.obj.transform.position.x) > 10.0f || Mathf.Abs(x.obj.transform.position.y) > 6.0f);
        projectiles.RemoveAll(x => Mathf.Abs(x.obj.transform.position.x) > 10.0f || Mathf.Abs(x.obj.transform.position.y) > 6.0f);
        foreach (Projectile p in toRemove)
        {
            Singleton.Instance().ProjectilePool.ReturnObject(p.obj.gameObject);
        }

    }

}
