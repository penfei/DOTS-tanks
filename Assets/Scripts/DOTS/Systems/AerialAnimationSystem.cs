﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class AerialAnimationSystem : JobComponentSystem
{
    protected struct AnimateAerialsJob : IJobForEach<Aerial, PlayerInputState>
    {
        [NativeDisableParallelForRestriction]
        public ComponentDataFromEntity<Rotation> RotationFromEntity;
        
        public float DeltaTime;
        
        [BurstCompile]
        public void Execute(ref Aerial aerial, [ReadOnly] ref PlayerInputState playerInput)
        {
            if(playerInput.Move.y != 0)
            {
                aerial.BendAngle = playerInput.Move.y;
                aerial.BendSpeed = 0;
            }
            else
            {
                aerial.BendSpeed -= aerial.BendAngle + 0.025f * aerial.BendSpeed;
                aerial.BendAngle += aerial.BendSpeed * DeltaTime;
            }

            var delta = quaternion.EulerXYZ(0, 0, 0.6f * aerial.BendAngle);

            var data1 = RotationFromEntity[aerial.Entity1];
            data1.Value = math.mul(aerial.ReferenceRotation1, delta);
            RotationFromEntity[aerial.Entity1] = data1;

            var data2 = RotationFromEntity[aerial.Entity2];
            data2.Value = math.mul(aerial.ReferenceRotation2, delta);
            RotationFromEntity[aerial.Entity2] = data2;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new AnimateAerialsJob
        {
            RotationFromEntity = GetComponentDataFromEntity<Rotation>(),
            DeltaTime = UnityEngine.Time.deltaTime
        }.Schedule(this, inputDeps);
        return job;
    }
}