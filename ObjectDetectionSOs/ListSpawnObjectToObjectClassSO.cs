using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ObjectDetectSpawner/ListSpawnObjectToClass")]
public class ListSpawnObjectToObjectClassSO : ScriptableObject
{
    public List<SpawnObjectToObjectClassSO> _SpawnObjectToObjectClassSos;
}