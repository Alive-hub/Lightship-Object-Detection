using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ObjectDetectSpawner/SpawnObjectToClass")]
public class SpawnObjectToObjectClassSO : ScriptableObject
{
    public string objectName;
    public string detectionClass;
    public GameObject objectToSpawn;
    public int maxSpawn; 
}
