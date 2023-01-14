using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Geometries.Behaviours
{

    [RequireComponent(typeof(SpawnBehaviour))]
    public class PulseBehaviour : BaseBehaviour
    {
        [Tooltip("Value in interval (0,1] that defines the proportion by which the object will grow and shrink")]
        public float scaleDiff = 0.25f;

        void Start()
        {
            base.OnStart();
            Singleton.Instance().BehaviourManager.AddBehaviour(this.transform,
                new Behaviour
                {
                    type = BehaviourType.PULSE,
                    status = Status.NOT_STARTED,
                    behaviour = this
                });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
