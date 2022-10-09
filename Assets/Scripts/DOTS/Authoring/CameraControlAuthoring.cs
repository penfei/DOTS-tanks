using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequiresEntityConversion]
[RequireComponent(typeof(Camera))]
public class CameraControlAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float DampTime = 0.2f;
    public float ScreenEdgeBuffer = 4f;
    public float MinSize = 6.5f;
    public float ZoomSpeed = 0.2f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var camera = GetComponent<Camera>();
        var data = new CameraControlComponent { DampTime = DampTime, MinSize = MinSize, ScreenEdgeBuffer = ScreenEdgeBuffer, CameraAspect = camera.aspect, CameraSize = camera.orthographicSize, ZoomSpeed = ZoomSpeed };
        dstManager.AddComponentData(entity, data);
    }
}
