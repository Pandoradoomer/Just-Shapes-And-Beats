using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Geometries.Behaviours
{
    [RequireComponent(typeof(SpawnBehaviour))]
    internal class RotateBehaviour : BaseBehaviour
    {
        float innerTime = 0;
        public float rotationAngle = 45;
        public override void OnStart()
        {
            base.OnStart();
            Singleton.Instance().BehaviourManager.AddBehaviour(this.transform,
                new Behaviour
                {
                    type = BehaviourType.ROTATE,
                    status = Status.ONGOING,
                    behaviour = this
                });
        }

        public override void OnUpdate()
        {
            innerTime += Time.deltaTime;
            if(innerTime > delayBetweenCycles)
            {
                innerTime = 0;
            }
            
            base.OnUpdate();
        }


    }
}
