using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Geometries.Behaviours
{
    [RequireComponent(typeof(SpawnBehaviour))]
    public class RotateSequenceBehaviour : BaseBehaviour
    {
        public float[] rotationAngles;
        public int currentIndex = 0;
        public override void OnStart()
        {
            base.OnStart();
            Singleton.Instance().BehaviourManager.AddBehaviour(this.transform,
                new Behaviour
                {
                    type = BehaviourType.ROTATE_SEQUENCE,
                    status = Status.ONGOING,
                    behaviour = this
                });
        }
    }
}
