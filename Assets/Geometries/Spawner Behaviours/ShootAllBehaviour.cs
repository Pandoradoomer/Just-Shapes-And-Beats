using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShootAllBehaviour : BaseBehaviour
{
    // Start is called before the first frame update
    float innerTime = 0;
    public float speed = 5;

    public SpawnPoints spawnPoints;
    public Transform Projectile;

    public override void OnStart()
    {
        base.OnStart();
        Singleton.Instance().BehaviourManager.AddBehaviour(this.transform,
            new Behaviour
            {
                type = BehaviourType.SHOOT_ALL,
                status = Status.NOT_STARTED,
                behaviour = this
            });
    }

    public override void OnUpdate()
    {
        //innerTime += Time.deltaTime;
        //if (innerTime > delayBetweenCycles)
        //{
        //    innerTime = 0;
        //    //Shoot();
        //}
        base.OnUpdate();
    }

    protected void Shoot()
    {
        foreach (ProjectileData sp in spawnPoints.spawningPoints)
        {
            Quaternion rot = Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward);
            Vector2 pos = rot * sp.Pos;
            GameObject go = Singleton.Instance().ProjectilePool.GetObject();
            go.transform.position = new Vector2(transform.position.x + pos.x, transform.position.y + pos.y);
            go.transform.rotation = Quaternion.identity;
            Singleton.Instance().ProjectileManager.projectiles.Add(new global::Projectile
            {
                obj = go.transform,
                Direction = rot * sp.Direction,
                speed = speed
            });
        }
    }
    
}
