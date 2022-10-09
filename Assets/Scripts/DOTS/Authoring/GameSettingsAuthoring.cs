using System;
using Unity.Entities;
using UnityEngine;

public class GameSettingsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int ScoreToWin = 5;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameSettings
        {
            ScoreToWin = ScoreToWin,
        });
    }
}
