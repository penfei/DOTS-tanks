using Unity.Entities;
using UnityEngine;
#if (TERRAFORM)
using Terraform.Component;
#endif

public class TankAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int PlayerId;
    public float MoveSpeed = 12f;
    public float TurnSpeed = 8;
    public Transform FireTransform;
    public float MinLaunchForce = 15.0f;
    public float MaxLaunchForce = 30.0f;
    public float MaxChargeTime = 0.75f;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TankPlayer
        {
            PlayerId = PlayerId,
            Score = 0,
        });
        dstManager.AddComponentData(entity, new PlayerHealth
        {
            Health = 100
        });
        dstManager.AddComponentData(entity, new TankMovementStats
        {
            MoveSpeed = MoveSpeed,
            TurnSpeed = TurnSpeed,
        });
        dstManager.AddComponentData(entity, new TankAttackStats
        {
            MinLaunchForce = MinLaunchForce,
            MaxLaunchForce = MaxLaunchForce,
            ChargeSpeed = (MaxLaunchForce - MinLaunchForce) / MaxChargeTime,
            CurrentLaunchForce = MinLaunchForce,
            IsCharging = 0,

            ShellSpawnPositionOffset = FireTransform.localPosition,
            ShellSpawnRotationOffset = FireTransform.localRotation,
        });
        dstManager.AddComponentData(entity, new PlayerInputState());
        dstManager.AddComponent(entity, typeof(CameraTarget));
    }
}
