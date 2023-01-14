using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Geometries.Behaviours
{
    public class DespawnBehaviour : SpawnBehaviour
    {

        public override void OnStart()
        {
            base.OnStart();
            Singleton.Instance().BehaviourManager.AddBehaviour(this.transform,
                new Behaviour
                {
                    type = BehaviourType.DESPAWN,
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
