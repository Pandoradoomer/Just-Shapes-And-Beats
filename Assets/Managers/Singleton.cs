using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton 
{
    private Singleton() 
    {
        _projectileManager = GameObject.FindObjectOfType<ProjectileManager>();
        _behaviourManager = GameObject.FindObjectOfType<BehaviourManager>();
        _masterScript = GameObject.FindObjectOfType<MasterScript>();


        ObjectPool[] pools = GameObject.FindObjectsOfType<ObjectPool>();
        foreach(ObjectPool pool in pools)
        {
            switch(pool.poolType)
            {
                case PoolType.CIRCLE: _circlePool = pool; break;
                case PoolType.SQUARE: _squarePool = pool; break;
                case PoolType.PROJECTILE: _projectilePool = pool; break;
            }
        }
    }
    static Singleton _instance;
    public static Singleton Instance()
    {
        if (_instance == null)
            _instance = new Singleton();
        return _instance;
    }

    private ProjectileManager _projectileManager;
    public ProjectileManager ProjectileManager { get => _projectileManager;  }

    private BehaviourManager _behaviourManager;
    public BehaviourManager BehaviourManager { get => _behaviourManager; }

    private MasterScript _masterScript;
    public MasterScript MasterScript { get => _masterScript; }

    private ObjectPool _projectilePool;
    public ObjectPool ProjectilePool { get => _projectilePool; }

    private ObjectPool _circlePool;
    public ObjectPool CirclePool { get => _circlePool; }

    private ObjectPool _squarePool;
    public ObjectPool SquarePool { get => _squarePool; }

    public bool _cameraShake = true;

}
