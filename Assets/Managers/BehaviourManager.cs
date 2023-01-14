using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Geometries.Behaviours;

public enum BehaviourType
{
    SHOOT_ALL,
    ROTATE,
    ROTATE_SEQUENCE,
    PULSE,
    SPAWN,
    DESPAWN

}

public enum Status
{
    NOT_STARTED,
    ONGOING,
    FINISHED
}

public class Behaviour
{
    public BehaviourType type;
    public Status status;
    public BaseBehaviour behaviour;
}

public class LifeTime
{
    public float lifeTime;
    public float spawnTime;
}

public class BehaviourManager : MonoBehaviour
{
    // Start is called before the first frame update

    public Dictionary<Transform, List<Behaviour>> behaviours;
    public Dictionary<Transform, LifeTime> lifeTimes;
    public List<(Transform t, BaseBehaviour b)> runningCoroutines;
    float innerTime = 0.0f;
    int coroutinesRun = 0;

    void Start()
    {
        behaviours = new Dictionary<Transform, List<Behaviour>>();
        lifeTimes = new Dictionary<Transform, LifeTime>();
        runningCoroutines = new List<(Transform t, BaseBehaviour b)>();
    }

    // Update is called once per frame
    void Update()
    {
        innerTime += Time.deltaTime;
        ManageBehaviours();
        if(innerTime - Mathf.Floor(innerTime) < 0.005f)
        {
            //PrintBehaviours();
            //Debug.Log(coroutinesRun);
        }
    }

    public void AddBehaviour(Transform key, Behaviour value)
    {
        if(behaviours.ContainsKey(key))
        {
            if (behaviours[key].Any(x => x.type == value.type))
                return;
            else
            {
                behaviours[key].Add(value);
            }
        }
        else
        {
            behaviours.Add(key, new List<Behaviour>());
            behaviours[key].Add(value);
        }
    }

    #region Print Functions
    public void PrintBehaviours()
    {
        foreach(KeyValuePair<Transform, List<Behaviour>> kvp in behaviours)
        {
            if(kvp.Key != null)
            {
                Debug.Log($"Transform with {kvp.Key.name} has {kvp.Value.Count} behaviours attached");
                foreach (Behaviour b in behaviours[kvp.Key])
                {
                    PrintBehaviour(b);
                }
            }
        }
    }

    void PrintBehaviour(Behaviour b)
    {
        string printOut = $"Type: {b.type} | Status: {b.status}";
        switch(b.type)
        {
            case BehaviourType.ROTATE:
                {
                    RotateBehaviour rotBehaviour = b.behaviour as RotateBehaviour;
                    printOut += $"\n Cycle Frequency: {rotBehaviour.delayBetweenCycles} | Rotation Angle: {rotBehaviour.rotationAngle}";
                    break;
                }
            case BehaviourType.SPAWN:
                {
                    SpawnBehaviour spawnBehaviour = b.behaviour as SpawnBehaviour;
                    printOut += $"\n Spawn Time: {spawnBehaviour.delayBetweenCycles} | Spawn Type: {spawnBehaviour.spawnType}";
                    break;
                }
        }
        Debug.Log(printOut);
    }
    #endregion

    #region Core Loop
    void ManageBehaviours()
    {
        //DeleteAllBehavioursWithNullTransforms();
        TriggerAllExpiredTransforms();
        ChangeBehaviourStatus();
        ParseAndEffectuateBehaviours();
    }

    public void DeleteAllBehavioursWithNullTransforms()
    {
        var toRemove1 = lifeTimes.Where(pair => pair.Key == null)
                                .Select(pair => pair.Key)
                                .ToList();
        foreach(var key in toRemove1)
        {
            lifeTimes.Remove(key);
        }

        var toRemove2 = behaviours.Where(pair => pair.Key == null)
                                  .Select(pair => pair.Key)
                                  .ToList();
        foreach(var key in toRemove2)
        {
            behaviours.Remove(key);
        }
    }

    void TriggerAllExpiredTransforms()
    {
        /// 1. Cycle through the lifeTimes
        /// 2. Find all transforms where innerTime > spawnTime + lifeTime
        /// 3. Communicate that the object has reached the end of its lifecycle to the ChangeBehaviourStatus
        /// 4. Remove them from the lifeTimes dictionary
        var keys = lifeTimes.Keys.ToArray();
        for (int i = 0; i < lifeTimes.Count; i++)
        {
            var key = keys[i];
            LifeTime currLifeTime = lifeTimes[key];
            if (innerTime > currLifeTime.spawnTime + currLifeTime.lifeTime)
            {
                ChangeBehaviourStatus(true, key);
                lifeTimes.Remove(key);
            }
        }
    }

    public void ChangeBehaviourStatus(bool endOfLife = false, Transform t = null)
    {
        /// The operations are as follows:
        /// 1. If there is a spawn behaviour marked as NOT_STARTED, start it and set all other behaviours to NOT_STARTED as well
        /// 2. If the spawn behaviour is marked as FINISHED, set all other behaviours (except for any despawn) to ONGOING
        /// 3. If the object has reached the end of its life cycle:
        ///     a. check if there is a despawn
        ///     b. if there is, set it to ongoing 
        ///     c. if there isn't, add an ongoing one that makes the object disintegrate in an instant

        if (!endOfLife)
        {

            var keys = behaviours.Keys.ToArray();
            for (int i = 0; i < behaviours.Count; i++)
            {
                var key = keys[i];
                List<Behaviour> totalBehaviours = behaviours[key];
                if (totalBehaviours.Any(b => b.type == BehaviourType.SPAWN))
                {
                    int spawnIndex = totalBehaviours.FindIndex(b => b.type == BehaviourType.SPAWN);
                    if (totalBehaviours[spawnIndex].status == Status.NOT_STARTED)
                    {
                        totalBehaviours[spawnIndex].status = Status.ONGOING;

                        for (int j = 0; j < totalBehaviours.Count; j++)
                        {
                            if (j != spawnIndex)
                                totalBehaviours[j].status = Status.NOT_STARTED;
                        }
                    }
                    else if (totalBehaviours[spawnIndex].status == Status.FINISHED)
                    {
                        for (int j = 0; j < totalBehaviours.Count; j++)
                        {
                            if (totalBehaviours[j].type != BehaviourType.DESPAWN && j != spawnIndex)
                            {
                                totalBehaviours[j].status = Status.ONGOING;
                            }
                        }
                    }
                }

            }
        }
        else
        {
            List<Behaviour> totalBehaviours = behaviours[t];
            if (totalBehaviours.Any(b => b.type == BehaviourType.DESPAWN))
            {
                int despawnIndex = totalBehaviours.FindIndex(b => b.type == BehaviourType.DESPAWN);
                totalBehaviours[despawnIndex].status = Status.ONGOING;
            }
            /// for code cohesion, if there isn't a despawn behaviour (we just want the object to disintegrate instantly), we add an appropriate
            /// behaviour to keep the code clean and consistent
            else
            {
                totalBehaviours.Add(new Behaviour
                {
                    type = BehaviourType.DESPAWN,
                    status = Status.ONGOING,
                    behaviour = new DespawnBehaviour()
                    {
                        delayBetweenCycles = 0.1f,
                        spawnType = SpawnType.DEFAULT
                    }
                });
            }
        }
    }
    public void AddLifeTime(Transform t, float lifeTime)
    {
        lifeTimes.Add(t, new LifeTime
        {
            lifeTime = lifeTime,
            spawnTime = innerTime
        });
        Debug.Log($"Added new object at {innerTime} with lifetime: {lifeTime}");
    }

    void ParseAndEffectuateBehaviours()
    {
        /// LIMIT: There can only be one behaviour of each type in any one transform's behaviour list
        foreach (var key in behaviours.Keys.ToArray())
        {
            foreach (Behaviour b in behaviours[key])
            {
                if (b.status == Status.ONGOING)
                {
                    StartCoroutine(ParseBehaviour(b, key));
                }
            }
        }
    }

    IEnumerator ParseBehaviour(Behaviour b, Transform t)
    {
        switch (b.type)
        {
            case BehaviourType.SPAWN:
                {
                    SpawnBehaviour spawnBehaviour = b.behaviour as SpawnBehaviour;
                    if (!runningCoroutines.Contains((t, spawnBehaviour)))
                    {
                        runningCoroutines.Add((t, spawnBehaviour));
                        yield return StartCoroutine(Spawn(spawnBehaviour, t));
                    }
                    break;
                }
            case BehaviourType.ROTATE:
                {
                    RotateBehaviour rotateBehaviour = b.behaviour as RotateBehaviour;
                    if (!runningCoroutines.Contains((t, rotateBehaviour)))
                    {
                        runningCoroutines.Add((t, rotateBehaviour));
                        yield return StartCoroutine(Rotate(rotateBehaviour, t));
                    }
                    break;
                }
            case BehaviourType.ROTATE_SEQUENCE:
                {
                    RotateSequenceBehaviour rotateSeqBehaviour = b.behaviour as RotateSequenceBehaviour;
                    if(!runningCoroutines.Contains((t,rotateSeqBehaviour)))
                    {
                        runningCoroutines.Add((t, rotateSeqBehaviour));
                        yield return StartCoroutine(RotateSequence(rotateSeqBehaviour, t));
                    }
                    break;
                }
            case BehaviourType.DESPAWN:
                {
                    DespawnBehaviour despawnBehaviour = b.behaviour as DespawnBehaviour;
                    if (!runningCoroutines.Contains((t, despawnBehaviour)))
                    {
                        runningCoroutines.Add((t, despawnBehaviour));
                        yield return StartCoroutine(Despawn(despawnBehaviour, t));
                    }
                    break;
                }
            case BehaviourType.SHOOT_ALL:
                {
                    ShootAllBehaviour shootBehaviour = b.behaviour as ShootAllBehaviour;
                    if(!runningCoroutines.Contains((t, shootBehaviour)))
                    {
                        runningCoroutines.Add((t, shootBehaviour));
                        yield return StartCoroutine(Shoot(shootBehaviour, t));
                    }
                    break;
                }
            case BehaviourType.PULSE:
                {
                    PulseBehaviour pulseBehaviour = b.behaviour as PulseBehaviour;
                    if(!runningCoroutines.Contains((t,pulseBehaviour)))
                    {
                        runningCoroutines.Add((t, pulseBehaviour));
                        yield return StartCoroutine(Pulse(pulseBehaviour, t));
                    }
                    break;
                }
        }
        yield return null;
    }
    #endregion

    #region Shoot Functions

    IEnumerator Shoot(ShootAllBehaviour b, Transform t)
    {
        SpriteRenderer sr = t.GetComponentInChildren<SpriteRenderer>();
        foreach (ProjectileData sp in b.spawnPoints.spawningPoints)
        {
            Quaternion rot = Quaternion.AngleAxis(t.eulerAngles.z, Vector3.forward);
            Vector2 pos = rot * sp.Pos;
            //Debug.Log($"{pos} / {sp.Pos} / {transform.eulerAngles.z}");
            Transform go = Instantiate(b.Projectile, new Vector2(t.position.x + pos.x, t.position.y + pos.y), Quaternion.identity);
            Color c = sr.color;
            c.a = 1.0f;
            go.GetComponent<SpriteRenderer>().color = c;
            Singleton.Instance().ProjectileManager.projectiles.Add(new global::Projectile
            {
                obj = go,
                Direction = rot * sp.Direction,
                speed = b.speed
            });
        }
        yield return new WaitForSeconds(b.delayBetweenCycles);
        runningCoroutines.Remove((t, b));
    }

    #endregion

    #region Rotate Functions
    //TODO: REFACTOR!!!
    IEnumerator Rotate(RotateBehaviour b, Transform t)
    {
        float initialAngle = t.eulerAngles.z;
        float step = b.rotationAngle / (b.delayBetweenCycles / 2);
        for (float i = 0; i < b.delayBetweenCycles / 2; i += Time.deltaTime)
        {
            if(t != null)
            t.Rotate(0, 0, step * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(b.delayBetweenCycles / 2);
        runningCoroutines.Remove((t, b));
    }

    IEnumerator RotateSequence(RotateSequenceBehaviour b, Transform t)
    {
        float initialAngle = t.eulerAngles.z;
        float step = b.rotationAngles[b.currentIndex] / (b.delayBetweenCycles / 2);
        float zAngle = b.rotationAngles[b.currentIndex] + t.eulerAngles.z;
        if (zAngle < -180)
            zAngle += 360;
        if (zAngle > 180)
            zAngle -= 360;
        for (float i = 0; i < b.delayBetweenCycles / 2; i += Time.deltaTime)
        {
            t.Rotate(0, 0, step * Time.deltaTime);
            yield return null;
        }
        t.eulerAngles = new Vector3(0, 0, zAngle);
        yield return new WaitForSeconds(b.delayBetweenCycles / 2);
        b.currentIndex++;
        if (b.currentIndex >= b.rotationAngles.Length)
            b.currentIndex = 0;
        runningCoroutines.Remove((t, b));

    }
    #endregion

    #region Despawn Coroutines
    IEnumerator Despawn(DespawnBehaviour b, Transform t)
    {
        switch(b.spawnType)
        {
            case SpawnType.FADE_IN:
                {
                    yield return StartCoroutine(FadeOut(b, t));
                    break;
                }
            case SpawnType.GROW:
                {
                    break;
                }
            case SpawnType.DEFAULT:
                {
                    yield return StartCoroutine(DefaultWait(b, t));
                    break;
                }
        }
        yield return null;
    }

    IEnumerator FadeOut(DespawnBehaviour b, Transform t)
    {
        SpriteRenderer[] sprites = t.GetComponentsInChildren<SpriteRenderer>();
        for(float i = 0; i < b.delayBetweenCycles; i += Time.deltaTime)
        {
            foreach(SpriteRenderer sprite in sprites)
            {
                Color c = sprite.color;
                c.a = 1 - (i / b.delayBetweenCycles);
                sprite.color = c;
            }
            yield return null;
        }
        Destroy(t.gameObject);
        behaviours.Remove(t);
        lifeTimes.Remove(t);
        runningCoroutines.Remove((t, b));
        yield return null;
    }

    IEnumerator DefaultWait(DespawnBehaviour b, Transform t)
    {
        for(float i = 0; i < b.delayBetweenCycles; i += Time.deltaTime)
        {
            yield return null;
        }
        Destroy(t.gameObject);
        behaviours.Remove(t);
        lifeTimes.Remove(t);
        runningCoroutines.Remove((t, b));
        yield return null;
    }
    #endregion

    #region Spawn Coroutines
    IEnumerator Spawn(SpawnBehaviour b, Transform t)
    {
        switch (b.spawnType)
        {
            case SpawnType.FADE_IN:
                {
                    yield return StartCoroutine(FadeIn(b, t));
                    break;
                }
            case SpawnType.GROW:
                {
                    yield return StartCoroutine(Grow(b, t));
                    break;
                }
        }
        yield return null;
    }

    IEnumerator FadeIn(SpawnBehaviour b, Transform t)
    {
        Collider2D[] colliders = t.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
            collider.enabled = false;

        SpriteRenderer[] sprites = t.GetComponentsInChildren<SpriteRenderer>();
        for (float i = 0; i < b.delayBetweenCycles; i += Time.deltaTime)
        {
            foreach (SpriteRenderer sprite in sprites)
            {
                Color c = sprite.color;
                c.a = i / b.delayBetweenCycles;
                sprite.color = c;
            }
            yield return null;
        }

        foreach (SpriteRenderer sprite in sprites)
        {
            Color c = sprite.color;
            c.a = 1;
            sprite.color = c;
        }

        foreach (Collider2D collider in colliders)
            collider.enabled = true;
        ChangeSpawnStatus(t);

        yield return null;
    }

    IEnumerator Grow(SpawnBehaviour b, Transform t)
    {
        Collider2D[] colliders = t.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
            collider.enabled = false;

        SpriteRenderer[] sprites = t.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer s in sprites)
            s.color = new Color(s.color.r, s.color.g, s.color.b, 1.0f);

        Vector3 initialScale = t.localScale;

        for(float i = 0; i < b.delayBetweenCycles; i += Time.deltaTime)
        {
            t.localScale = Vector3.Lerp(Vector3.zero, initialScale, i / b.delayBetweenCycles);
            yield return null;
        
        }
        t.localScale = initialScale;

        foreach (Collider2D collider in colliders)
            collider.enabled = true;
        ChangeSpawnStatus(t);

        yield return null;
    }

    void ChangeSpawnStatus(Transform t)
    {
        for(int i = 0; i < behaviours[t].Count; i++)
        {
            if (behaviours[t][i].type == BehaviourType.SPAWN)
            {
                behaviours[t][i].status = Status.FINISHED;
                return;
            }
        }
    }

    #endregion

    #region Pulse Functions
    IEnumerator Pulse(PulseBehaviour b, Transform t)
    {
        float scaleDiff = b.scaleDiff;

        /// The animation has 3 stages
        /// 1. The object shrinks to scale - scaleDiff
        /// 2. The object grows to scale + scaleDiff
        /// 3. The object shrinks to scale
        /// Each cycle happens in delay/3 
        /// 
        Vector3 initialScale = Vector3.one;
        if(t!= null)
            initialScale = t.localScale;

        //1
        for(float i = 0; i <= b.delayBetweenCycles/3; i += Time.deltaTime)
        {
            if (t == null)
                break;
            t.localScale = initialScale - (i/(b.delayBetweenCycles/3)) * scaleDiff * initialScale;
            yield return null;
        }

        //2
        for(float i = 0; i <= b.delayBetweenCycles/3; i += Time.deltaTime)
        {
            if (t == null)
                break;
            t.localScale = initialScale + (-0.5f + ((i / (b.delayBetweenCycles / 3)))) * 2 * scaleDiff * initialScale;
            yield return null;
        }

        //3
        for(float i = 0; i <= b.delayBetweenCycles/3; i += Time.deltaTime)
        {
            if (t == null)
                break;
            t.localScale = initialScale + (1 - i/(b.delayBetweenCycles/3)) * scaleDiff * initialScale;
            yield return null;
        }

        yield return null;
        if(t != null)
            runningCoroutines.Remove((t, b));
    }
    #endregion
}
