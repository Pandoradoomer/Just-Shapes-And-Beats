using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Geometries.Behaviours
{
    public enum SpawnType
    {
        FADE_IN,
        GROW,
        DEFAULT
    }

    public class SpawnBehaviour : BaseBehaviour
    {
        public SpawnType spawnType;
        
        public override void OnStart()
        {
            base.OnStart();
            Singleton.Instance().BehaviourManager.AddBehaviour(this.transform,
                new Behaviour
                {
                    type = BehaviourType.SPAWN,
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
