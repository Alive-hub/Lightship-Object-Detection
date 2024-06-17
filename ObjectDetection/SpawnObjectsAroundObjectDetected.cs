using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnObjectsAroundObjectDetected : MonoBehaviour
{
    private List<GameObject> spawnedObjects = new();
    private Dictionary<GameObject, int> spawnedObjectCount = new();

    [SerializeField] private ListSpawnObjectToObjectClassSO _listSpawnObjectToObjectClassSo;

    [SerializeField] private Depth_ScreenToWorldPosition _screenToWorldPosition;

    private Camera mainCamera;
    private LayerMask meshLayer;
    private Ray debugRay;
    
    public static SpawnObjectsAroundObjectDetected Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void RemoveAllSpawnedObjects()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            Destroy(spawnedObjects[i]);
        }

        spawnedObjects = new();
        spawnedObjectCount = new();
    }

    public void ToggleSpawningOn()
    {
        preventSpawning = false;
    }

    public void ToggleSpawningOff()
    {
        preventSpawning = true;
    }

    [SerializeField] private float timer = 1f;
    private WaitForSeconds delay;

    private bool preventSpawning = true;
    private bool waitForNextSpawn = false;
    

    // Start is called before the first frame update
    void Start()
    {
        meshLayer = LayerMask.NameToLayer("ARMesh");
        mainCamera = FindObjectOfType<Camera>();
        
        ObjectDetectionSample.OnFoundItemAtPosition += ObjectDetectionSampleOnOnFoundItemAtPosition;
        delay = new WaitForSeconds(timer);
    }

    private void ObjectDetectionSampleOnOnFoundItemAtPosition((string category, Vector2 rectPosition) objectDetectedAt)
    {
        if(waitForNextSpawn) return;
        
        if(preventSpawning) return;
        
        StartCoroutine(SpawnObjectsAtDetectedPositionWithDelay(objectDetectedAt));
    }

    private Vector3 debugPoint;

    IEnumerator SpawnObjectsAtDetectedPositionWithDelay((string category, Vector2 rectPosition) objectDetectedAt)
    {
        Debug.Log("SpawnObjectsAtDetectedPositionWithDelay");
        
        waitForNextSpawn = true;
        yield return delay;

        if (_listSpawnObjectToObjectClassSo && _listSpawnObjectToObjectClassSo._SpawnObjectToObjectClassSos.Count > 0)
        {
            List<SpawnObjectToObjectClassSO> objectToObjectClassSos =
                _listSpawnObjectToObjectClassSo._SpawnObjectToObjectClassSos;

            foreach (var spawnObjectToObjectClass in objectToObjectClassSos)
            {
                if(spawnObjectToObjectClass.detectionClass == objectDetectedAt.category)
                {
                    GameObject objectToSpawn = spawnObjectToObjectClass.objectToSpawn;

                    if (spawnedObjectCount.ContainsKey(objectToSpawn) &&
                        spawnedObjectCount[objectToSpawn] >= spawnObjectToObjectClass.maxSpawn)
                    {
                        continue;
                    }

                    // (Vector3 hitPoint, Vector3 hitNormal) =
                    //     ShootRaycastFromDetectedPosition(objectDetectedAt.rectPosition);

                    Vector3 hitPointNew = _screenToWorldPosition
                            .ConvertScreenToWorldPosition(objectDetectedAt.rectPosition);
                    
                    Debug.Log("Converted Hit Point is: " + hitPointNew);
                    
                    debugPoint = hitPointNew;

                    if (hitPointNew != Vector3.zero)
                    {
                        SpawnObjectAtHitPosition(hitPointNew, spawnObjectToObjectClass.objectToSpawn);
                    }

                }
            }
        }

        waitForNextSpawn = false;
    }

    void SpawnObjectAtHitPosition(Vector3 position, GameObject objectToSpawn)
    {
        
        GameObject spawnedObject;

        spawnedObject = Instantiate(objectToSpawn, position, Quaternion.identity);

        if (spawnedObjectCount.ContainsKey(objectToSpawn))
        {
            spawnedObjectCount[objectToSpawn] += 1;
        }
        else
        {
            spawnedObjectCount.Add(objectToSpawn, 1);
        }
        
        Debug.Log($"Spawning Object {objectToSpawn.name} with total amount {spawnedObjectCount[objectToSpawn]}");
        spawnedObjects.Add(spawnedObject);
    }

    private Color[] randoms = new[]
    {
        Color.magenta,
        Color.green,
        Color.blue,
        Color.red,
        Color.yellow,
        Color.cyan,
    };
    private void OnDrawGizmos()
    {
        Gizmos.color =  randoms[Random.Range(0, randoms.Length)];
        Gizmos.DrawRay(debugRay);
        
        Gizmos.DrawWireSphere(debugPoint, .25f);
    }
}