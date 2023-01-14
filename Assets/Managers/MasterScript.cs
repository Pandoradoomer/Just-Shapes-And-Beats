using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Geometries.Scripts;
using System;
using System.Linq;
using static UnityEditor.Rendering.FilterWindow;

public class MasterScript : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    Sequence sequence;

    [SerializeField]
    GameObject player;

    [SerializeField]
    Camera camera;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartSequence(sequence.sequence));
        }
    }

    #region Base Functions
    Transform Create(BasicGeometry geometry, out ObjectPool objPool)
    {
        Collider2D coll = geometry._Transform.GetComponent<Collider2D>();
        objPool = null;
        Transform trans;
        if (coll is CircleCollider2D)
        {
            objPool = Singleton.Instance().CirclePool;
            trans = objPool.GetObject().transform;
        }
        else
        {
            objPool = Singleton.Instance().SquarePool;
            trans = objPool.GetObject().transform;
        }
        trans.localScale = geometry.Scale;
        trans.gameObject.SetActive(true);
        Rescaler rs = trans.GetComponent<Rescaler>();
        if (rs != null)
            rs.Rescale();
        SpriteRenderer sr = trans.GetComponent<SpriteRenderer>();
        sr.color = geometry._Color;

        return trans;

    }

    public IEnumerator StartSequence(List<SequenceElement> sequence)
    {
        foreach(SequenceElement se in sequence)
        {
            if(se.WaitUntilEnd)
            {
                yield return StartCoroutine(ParseSequenceElement(se));
            }
            else
            {
                StartCoroutine(ParseSequenceElement(se));
                yield return new WaitForSeconds(se.pause);
            }
            
        }
        yield return null;
    }
    public IEnumerator ParseSequenceElement(SequenceElement element)
    {
        switch (element.Type)
        {
            case SequenceType.FLASH_DYNAMIC:
                {
                    yield return FlashDynamic(element as FlashDynamicElement);
                    break;
                }
            case SequenceType.FLASH_SINE_CURVE:
                {
                    yield return FlashDynamicSine(element as FlashDynamicSineElement);
                    break;
                }
            case SequenceType.FLASH_DIRECTION_SEQUENCE:
                {
                    yield return StartCoroutine(FlashDirectionSequence(element as FlashDirectionSequenceElement));
                    break;
                }
            case SequenceType.SPAWN:
                {
                    yield return StartCoroutine(SpawnSequence(element as SpawnSequenceElement));
                    break;
                }
            case SequenceType.WAYPOINT:
                {
                    yield return StartCoroutine(WaypointSequence(element as SequenceWaypointElement));
                    break;
                }
            case SequenceType.CYCLICAL_WAYPOINT:
                {
                    yield return StartCoroutine(CyclicalWaypointSequence(element as SequenceCyclicalWaypointElement));
                    break;
                }
        }
        yield return null;
    }
    #endregion

    #region Sprite Functions
    void GetWorldParameters(out float worldHeight, out float worldWidth)
    {
        float aspect = (float)Screen.width / Screen.height;
        worldHeight = camera.orthographicSize * 2;
        worldWidth = worldHeight * aspect;
    }

    void GetPrefabParameters(Transform trans, out float height, out float width)
    {
        SpriteRenderer[] spriteRenderers = trans.GetComponentsInChildren<SpriteRenderer>();
        float minX = 10000, minY = 10000, maxX = -10000, maxY = -10000;
        float minWidth = 0, maxWidth = 0, minHeight = 0, maxHeight = 0;
        foreach(SpriteRenderer sr in spriteRenderers)
        {
            Transform tempTrans = sr.transform;
            if (tempTrans.position.x < minX)
            {
                minX = tempTrans.position.x;
                minWidth = sr.sprite.bounds.size.x;
            }
            if (tempTrans.position.x > maxX)
            {
                maxX = tempTrans.position.x;
                maxWidth = sr.sprite.bounds.size.x;
            }
            if (tempTrans.position.y < minY)
            {
                minY = tempTrans.position.y;
                minHeight = sr.sprite.bounds.size.y;
            }
            if (tempTrans.position.y > maxY)
            {
                maxY = tempTrans.position.y;
                maxHeight = sr.sprite.bounds.size.y;
            }

        }
        height = maxY - minY + maxHeight/2 + minHeight/2;
        width = maxX - minX + maxWidth/2 + minWidth/2;
    }
    #endregion

    #region Flash Functions
    void Flash(FlashGeometry geometry, Vector2 position, Quaternion? rotation =  null)
    {
        float buildUpTime = geometry.buildUpTime;
        float stayInTime = geometry.stayAliveTime;
        float fadeOutTime = geometry.fadeOutTime;
        ObjectPool objPool;
        Transform instance = Create(geometry, out objPool);
        instance.position = position;
        Quaternion effectiveRotation = rotation ?? Quaternion.identity;
        instance.rotation = effectiveRotation;
        StartCoroutine(Flash(instance, buildUpTime, stayInTime, fadeOutTime, objPool));
    }

    IEnumerator Flash(Transform obj, float buildUpTime, float stayInTime, float fadeOutTime, ObjectPool objPool)
    {
        SpriteRenderer[] spriteRenderers = obj.gameObject.GetComponentsInChildren<SpriteRenderer>();

        for (float i = 0; i < buildUpTime; i += Time.deltaTime)
        {
            foreach(SpriteRenderer sr in spriteRenderers)
            {
                Color c = sr.color;
                c.a = (i / buildUpTime) / 2.0f;
                sr.color = c;
            }
            yield return null;
        }

        Collider2D[] colls = obj.gameObject.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D coll in colls)
        {
            coll.enabled = true;
        }

        for (float i = 0; i < stayInTime; i += Time.deltaTime)
        {
            foreach(SpriteRenderer sr in spriteRenderers)
            {
                Color c = sr.color;
                c.a = 1.0f;
                sr.color = c;
            }
            yield return null;
        }
        foreach (Collider2D coll in colls)
        {
            coll.enabled = false;
        }

        for(float i = 0; i < fadeOutTime; i+= Time.deltaTime)
        {
            foreach(SpriteRenderer sr in spriteRenderers)
            {
                Color c = sr.color;
                c.a = (fadeOutTime - i) / fadeOutTime;
                sr.color = c;
            }
            yield return null;
        }
        Rescaler rs = obj.GetComponent<Rescaler>();
        if (rs != null)
            rs.Unscale();
        objPool.ReturnObject(obj.gameObject);

        yield return null;
    }

    IEnumerator FlashDynamic(FlashDynamicElement element)
    {
        int repetitions = element.repetitions;
        float delayBetweenRepetitions = element.delayBetweenRepetitions;
        FlashGeometry flashGeom = element.geometry;
        for (int i = 0; i < repetitions; i++)
        {
            Flash(flashGeom, player.transform.position);
            yield return new WaitForSeconds(delayBetweenRepetitions);
        }

        yield return element;

    }

    IEnumerator FlashDynamicSine(FlashDynamicSineElement element)
    {
        Vector2 startPoint = element.startPoint;
        Vector2 endPoint = element.endPoint;

        float elHeight, elWidth;
        GetPrefabParameters(element.geometry._Transform, out elHeight, out elWidth);
        if (!element.UseYAxis)
        {
            if (startPoint.x < endPoint.x)
            {
                for (float i = startPoint.x; i < endPoint.x; i += element.distance + elWidth * element.geometry.Scale.x)
                {
                    float yDeviation = Vector2.Lerp(startPoint, endPoint, (i - startPoint.x) / (endPoint.x - startPoint.x)).y;
                    Flash(element.geometry, new Vector2(i, element.functionMultiplier * Mathf.Sin(element.sineMultiplier * i) + yDeviation));
                    yield return new WaitForSeconds(element.delay);
                }
            }
            else
            {
                for (float i = startPoint.x; i > endPoint.x; i -= element.distance + elWidth * element.geometry.Scale.x)
                {
                    float yDeviation = Vector2.Lerp(startPoint, endPoint, (i - startPoint.x) / (endPoint.x - startPoint.x)).y;
                    Flash(element.geometry, new Vector2(i, element.functionMultiplier * Mathf.Sin(element.sineMultiplier * i) + yDeviation));
                    yield return new WaitForSeconds(element.delay);
                }
            }
        }
        else
        {
            if (startPoint.y < endPoint.y)
            {
                for (float i = startPoint.y; i < endPoint.y; i += element.distance + elHeight * element.geometry.Scale.y)
                {
                    float xDeviation = Vector2.Lerp(startPoint, endPoint, (i - startPoint.y) / (endPoint.y - startPoint.y)).x;
                    Flash(element.geometry, new Vector2(element.functionMultiplier * Mathf.Sin(element.sineMultiplier * i) + xDeviation, i));
                    yield return new WaitForSeconds(element.delay);
                }
            }
            else
            {
                for (float i = startPoint.y; i > endPoint.y; i -= element.distance + elHeight * element.geometry.Scale.y)
                {
                    float xDeviation = Vector2.Lerp(startPoint, endPoint, (i - startPoint.y) / (endPoint.y - startPoint.y)).x;
                    Flash(element.geometry, new Vector2(element.functionMultiplier * Mathf.Sin(element.sineMultiplier * i) + xDeviation, i));
                    yield return new WaitForSeconds(element.delay);
                }
            }
        }

        yield return element.pause;
    }

    void GetDirectionAndRotationFromPointToPoint(Vector2 startPoint, Vector2 endPoint, bool useY, out Vector2 dir, out Quaternion rotation)
    {
        //Vector2 playerPos = player.transform.position;
        dir = (endPoint - startPoint).normalized;
        Vector2 reference;
        if (useY)
            reference = Vector2.up;
        else
            reference = Vector2.right;

        float rotationAngle = Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(dir, reference));
        if (dir.y < 0 && !useY)
            rotationAngle = -rotationAngle;
        if (dir.x > 0 && useY)
            rotationAngle = -rotationAngle;
        rotation = Quaternion.Euler(0, 0, rotationAngle);
    }

    IEnumerator FlashDirectionSequence(FlashDirectionSequenceElement element)
    {
        Vector2 dir;
        Quaternion lookRotation;
        //isDynamic defines whether the rotations and calculations are made with the endpoint being the player's current position
        if(element.isDynamic)
        {
            GetDirectionAndRotationFromPointToPoint(element.startPoint, player.transform.position, element.useYAxisForCalculations, out dir, out lookRotation);
        }
        else
        {
            GetDirectionAndRotationFromPointToPoint(element.startPoint, element.direction, element.useYAxisForCalculations, out dir, out lookRotation);
        }
        lookRotation = element.shouldRotate ? lookRotation : Quaternion.identity;

        float prefabHeight, prefabWidth;
        GetPrefabParameters(element.geometry._Transform, out prefabHeight, out prefabWidth);
        Vector2 flashPos = element.startPoint;
        for (int i = 0; i < element.numElements; i++)
        {
            if (element.isDynamic && element.shouldRecalculate)
            {
                if (!element.shouldStep)
                    GetDirectionAndRotationFromPointToPoint(element.startPoint, player.transform.position, element.useYAxisForCalculations, out dir, out lookRotation);
                else
                    GetDirectionAndRotationFromPointToPoint(flashPos, player.transform.position, element.useYAxisForCalculations, out dir, out lookRotation);
            }
            if (!element.shouldStep)
            {
                flashPos = element.startPoint;
                if (element.useYAxisForCalculations)
                    flashPos += i * (prefabHeight * element.geometry.Scale.y + element.elementSpace) * dir;
                else
                    flashPos += i * (prefabWidth * element.geometry.Scale.x + element.elementSpace) * dir;
            }
            else
            {
                if (element.useYAxisForCalculations)
                    flashPos += (prefabHeight * element.geometry.Scale.y + element.elementSpace) * dir;
                else
                    flashPos += (prefabWidth * element.geometry.Scale.x + element.elementSpace) * dir;
            }
            Flash(element.geometry, flashPos, lookRotation);

            yield return new WaitForSeconds(element.delayBetweenElements);
        }

        yield return new WaitForSeconds(element.pause);
    }
    #endregion

    #region Spawn Functions
    IEnumerator SpawnSequence(SpawnSequenceElement element)
    {
        float lifeTime = element.lifeTime;
        SpawnGeometry geometry = element.spawnGeometry;

        Transform trans = Instantiate(geometry.Spawner, geometry.Position, Quaternion.identity);
        SpriteRenderer[] sprites = trans.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
        {
            Color c = sprite.color;
            c.a = 0;
            sprite.color = c;
        }
        Singleton.Instance().BehaviourManager.AddLifeTime(trans, lifeTime);

        yield return element.pause;

    }
    #endregion

    #region Waypoint Functions

    
    IEnumerator Travel(Transform trans, Vector2 to, float velocity, bool shouldRotate)
    {
        Vector2 pos = trans.position;
        Vector2 dir = (to - pos).normalized;
        float dist = (to - pos).magnitude;
        float distTravelled = 0;
        Vector3 movement = dir * 0.005f * velocity;
        if(shouldRotate)
        {
            //float angle = -Vector2.Angle(dir, Vector2.up);
            float angle = -Vector2.SignedAngle(dir, Vector2.up);
            trans.rotation = Quaternion.Euler(0, 0, angle);
        }
        while(distTravelled < dist)
        {
            trans.position += movement;
            distTravelled += movement.magnitude;
            yield return null;
        }

    }

    IEnumerator TravelBetweenWaypoints(Transform trans, List<Vector2> waypoints, Vector2 endPoint, float velocity, bool shouldRotate)
    {
        foreach (Vector2 waypoint in waypoints)
        {
            yield return StartCoroutine(Travel(trans, waypoint, velocity, shouldRotate));
        
        }
        yield return StartCoroutine(Travel(trans, endPoint, velocity, shouldRotate));
        //Destroy(trans.gameObject);
        Singleton.Instance().BehaviourManager.ChangeBehaviourStatus(true, trans);
        //Singleton.Instance().BehaviourManager.DeleteAllBehavioursWithNullTransforms();
    }
    IEnumerator WaypointSequence(SequenceWaypointElement element)
    {
        List<Transform> transforms = new List<Transform>();
        for(int i = 0; i < element.numberOfTravellers; i++)
        {
            transforms.Add(Instantiate(element.Traveller, element.spawnPoint, Quaternion.identity));
        }
        for(int i = 0; i < element.numberOfTravellers - 1; i++)
        {
            StartCoroutine(TravelBetweenWaypoints(transforms[i], element.waypoints, element.endPoint, element.velocity, element.shouldRotate));
            yield return new WaitForSeconds(element.delayBetweenTravellers);
        }
        yield return StartCoroutine(TravelBetweenWaypoints(transforms[element.numberOfTravellers - 1], element.waypoints, element.endPoint, element.velocity, element.shouldRotate));
        yield return element.pause;
        //Transform trans = Instantiate(element.Traveller, element.spawnPoint, Quaternion.identity);
        //
        //foreach(Vector2 waypoint in element.waypoints)
        //{
        //    yield return StartCoroutine(Travel(trans, waypoint, element.velocity, element.shouldRotate));
        //
        //}
        //yield return StartCoroutine(Travel(trans, element.endPoint, element.velocity, element.shouldRotate));
        //
        //Destroy(trans.gameObject);
        //yield return element.pause;
    }
    int ClosestToNextWaypointInCycle(List<Vector2> waypoints, Vector2 nextPoint, int cycleStart, int cycleEnd)
    {
        /// finds the index of the closest point to the next waypoint in the cycle and returns it
        /// this is useful in order to complete a 'half-cycle' so that the movement doesn't appear janky
        /// 

        // could use a regular distance for clarity, but this is slightly more efficient (even if it's not often calculated)
        float minSqDistance = 10000000;
        int index = -1;
        List<int> indicesOrder = new List<int>{ cycleEnd };
        for (int i = cycleStart; i < cycleEnd; i++)
            indicesOrder.Add(i);

        for(int i = 0; i < indicesOrder.Count; i++)
        {
            float sqDistance = (nextPoint - waypoints[indicesOrder[i]]).sqrMagnitude;
            ///Note: if there are equal distances later in the cycle, they aren't counted
            ///the idea is to perform the least amount of steps 
            if(sqDistance < minSqDistance)
            {
                minSqDistance = sqDistance;
                index = indicesOrder[i];
            }
        }
        

        return index;
    }
    IEnumerator CyclicalWaypointSequence(SequenceCyclicalWaypointElement element)
    {
        List<Transform> transforms = new List<Transform>();
        for (int i = 0; i < element.numberOfTravellers; i++)
            transforms.Add(Instantiate(element.Traveller, element.spawnPoint, Quaternion.identity));
        //Transform trans = Instantiate(element.Traveller, element.spawnPoint, Quaternion.identity);
        List<Vector2> waypoints = element.waypoints;
        int closestToNextWaypointInCycle = -1;
        if (element.cycleEnd == waypoints.Count - 1)
        {
            closestToNextWaypointInCycle = ClosestToNextWaypointInCycle(waypoints, element.endPoint, element.cycleStart, element.cycleEnd);
        }
        else
        {
            closestToNextWaypointInCycle = ClosestToNextWaypointInCycle(waypoints, waypoints[element.cycleEnd + 1], element.cycleStart, element.cycleEnd);
        }

        List<Vector2> waypointsBeforeCycle = waypoints.GetRange(0, element.cycleStart);
        List<Vector2> cycleWaypoints = waypoints.GetRange(element.cycleStart, element.cycleEnd - element.cycleStart + 1);
        List<Vector2> waypointsAfterCycle = new List<Vector2>();
        if(element.cycleEnd != waypoints.Count - 1)
        {
            waypointsAfterCycle = waypoints.GetRange(element.cycleEnd + 1, element.waypoints.Count - element.cycleEnd - 1);
        }

        List<Vector2> trueWaypoints = new List<Vector2>();
        trueWaypoints.AddRange(waypointsBeforeCycle);
        for (int i = 0; i < element.numCycles; i++)
            trueWaypoints.AddRange(cycleWaypoints);

        if(closestToNextWaypointInCycle != element.cycleEnd)
        {
            for(int i = element.cycleStart; i <= closestToNextWaypointInCycle; i++)
            {
                trueWaypoints.Add(waypoints[i]);
            }
        }

        trueWaypoints.AddRange(waypointsAfterCycle);

        for(int i = 0; i < element.numberOfTravellers - 1; i++)
        {
            StartCoroutine(TravelBetweenWaypoints(transforms[i], trueWaypoints, element.endPoint, element.velocity, element.shouldRotate));
            yield return new WaitForSeconds(element.delayBetweenTravellers);
        }
        yield return StartCoroutine(TravelBetweenWaypoints(transforms[element.numberOfTravellers-1], trueWaypoints,
                                                        element.endPoint, element.velocity, element.shouldRotate));
        yield return element.pause;
        //create a list of indices based on the cycle information and have a function that makes the travel through all of them;

        //for (int i = 0; i < element.cycleStart; i++)
        //{
        //    yield return StartCoroutine(Travel(transforms[0], waypoints[i], element.velocity, element.shouldRotate));
        //}
        //
        //for(int i = 0; i < element.numCycles; i++)
        //{
        //    for(int j = element.cycleStart; j <= element.cycleEnd; j++)
        //    {
        //        yield return StartCoroutine(Travel(transforms[0], waypoints[j], element.velocity, element.shouldRotate));
        //    }
        //}
        //
        //if(closestToNextWaypointInCycle != element.cycleEnd)
        //{
        //    for (int i = element.cycleStart; i <= closestToNextWaypointInCycle; i++)
        //    {
        //        yield return StartCoroutine(Travel(transforms[0], waypoints[i], element.velocity, element.shouldRotate));
        //    }
        //}
        //
        //if (element.cycleEnd != waypoints.Count - 1)
        //{
        //    for (int i = element.cycleEnd + 1; i < element.waypoints.Count; i++)
        //        yield return StartCoroutine(Travel(transforms[0], waypoints[i], element.velocity, element.shouldRotate));
        //}
        //
        //yield return StartCoroutine(Travel(transforms[0], element.endPoint, element.velocity, element.shouldRotate));
        //Destroy(transforms[0].gameObject);
        //yield return element.pause;
    }
    #endregion
}
