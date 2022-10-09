using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
[RequiresEntityConversion]
public class HealthSliderAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int PlayerId;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new HealthSlider {PlayerId = PlayerId};

        dstManager.AddComponentData(entity, data);

    }
}
