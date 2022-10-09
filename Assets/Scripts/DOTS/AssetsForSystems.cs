using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AssetsForSystems : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject ShellPrefab;
    public GameObject ShellExplosionPrefab;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(ShellPrefab);
        referencedPrefabs.Add(ShellExplosionPrefab);
    }
}
