using Unity.Entities;
using UnityEngine;

namespace Unity.Physics.Authoring
{
    [UpdateAfter(typeof(PhysicsBodyConversionSystem))]
    [UpdateAfter(typeof(LegacyRigidbodyConversionSystem))]
    public class TanksConversionSystem : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((SetInertiaInverseBehaviour behaviour) => { behaviour.Convert(GetPrimaryEntity(behaviour), DstEntityManager, this); });
        }
    }
}
