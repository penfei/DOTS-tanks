using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(TransformSystemGroup))]
public class TankFiringSystem : JobComponentSystem
{
    struct SpawnShellsJob : IJobForEachWithEntity<PlayerInputState, LocalToWorld, Rotation, TankAttackStats>
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public Entity ShellPrefab;
        public float DeltaTime;

        void FireShell(int jobIndex, [ReadOnly] ref LocalToWorld tankLocalToWorld, [ReadOnly] ref Rotation tankRotation, [ReadOnly] ref TankAttackStats attackStats)
        {
            var shellEntity = CommandBuffer.Instantiate(jobIndex, ShellPrefab);
            Debug.Log(shellEntity);
            CommandBuffer.SetComponent(jobIndex, shellEntity, new Translation
            {
                Value = math.transform(tankLocalToWorld.Value, attackStats.ShellSpawnPositionOffset),
            });
            CommandBuffer.SetComponent(jobIndex, shellEntity, new Rotation
            {
                Value = math.mul(tankRotation.Value, attackStats.ShellSpawnRotationOffset),
            });
            CommandBuffer.SetComponent(jobIndex, shellEntity, new PhysicsVelocity
            {
                // TODO: add tank's current velocity
                Linear = math.mul(attackStats.ShellSpawnRotationOffset, tankLocalToWorld.Forward) * attackStats.CurrentLaunchForce,
            });
            attackStats.IsCharging = 0;
        }

        public void Execute(Entity tankEntity, int jobIndex, [ReadOnly] ref PlayerInputState inputState,
            [ReadOnly] ref LocalToWorld tankLocalToWorld, [ReadOnly] ref Rotation tankRotation, ref TankAttackStats attackStats)
        {
            if (attackStats.CurrentLaunchForce >= attackStats.MaxLaunchForce && attackStats.IsCharging != 0) {
                attackStats.CurrentLaunchForce = attackStats.MaxLaunchForce;
                FireShell(jobIndex, ref tankLocalToWorld, ref tankRotation, ref attackStats);
            } else if (inputState.Firing != 0 && attackStats.CurrentLaunchForce == attackStats.MinLaunchForce && attackStats.IsCharging == 0) {
                attackStats.IsCharging = 1;
            } else if (attackStats.IsCharging == 0 && inputState.Firing == 0 && attackStats.CurrentLaunchForce > attackStats.MinLaunchForce) {
                attackStats.CurrentLaunchForce = attackStats.MinLaunchForce;
            } else if (inputState.Firing != 0 && attackStats.IsCharging != 0) {
                attackStats.CurrentLaunchForce += attackStats.ChargeSpeed * DeltaTime;
            } else if (inputState.Firing == 0 && attackStats.IsCharging != 0) {
                FireShell(jobIndex, ref tankLocalToWorld, ref tankRotation, ref attackStats);
                attackStats.CurrentLaunchForce = attackStats.MinLaunchForce;
            }
        }
    }

    private BeginInitializationEntityCommandBufferSystem beginInitEcbSystem;
    private Entity ShellPrefab = Entity.Null;
    protected override void OnCreate()
    {
        beginInitEcbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        this.GetEntityQueryForIJobForEach(typeof(SpawnShellsJob));
    }

    protected override void OnStartRunning()
    {
        var shellPrefabQuery = EntityManager.CreateEntityQuery(new EntityQueryDesc
        {
            All = new[] {ComponentType.ReadWrite<ShellStats>()},
            Options = EntityQueryOptions.IncludePrefab,
        });
        if (shellPrefabQuery.CalculateEntityCount() != 1)
        {
            throw new InvalidOperationException("No Shell prefab detected?");
        }
        var prefabEntities = shellPrefabQuery.ToEntityArray(Allocator.TempJob);
        ShellPrefab = prefabEntities[0];
        prefabEntities.Dispose();
        shellPrefabQuery.Dispose();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var spawnShellsJob = new SpawnShellsJob
        {
            CommandBuffer = beginInitEcbSystem.CreateCommandBuffer().ToConcurrent(),
            DeltaTime = Time.deltaTime,
            ShellPrefab = ShellPrefab,
        }.Schedule(this, inputDeps);
        beginInitEcbSystem.AddJobHandleForProducer(spawnShellsJob);
        return spawnShellsJob;
    }
}
    