using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectsAroundObjectDetected : MonoBehaviour
{
    private List<GameObject> spawnedObjects = new();
    private Dictionary<GameObject, int> spawnedObjectCount = new();

    [SerializeField] private ListSpawnObjectToObjectClassSO _listSpawnObjectToObjectClassSo;


    private Camera mainCamera;
    private LayerMask meshLayer;


    [SerializeField] private float timer = 1f;
    private WaitForSeconds delay;

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
        
        StartCoroutine(SpawnObjectsAtDetectedPositionWithDelay(objectDetectedAt));
    }

    IEnumerator SpawnObjectsAtDetectedPositionWithDelay((string category, Vector2 rectPosition) objectDetectedAt)
    {
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

                    (Vector3 hitPoint, Vector3 hitNormal) =
                        ShootRaycastFromDetectedPosition(objectDetectedAt.rectPosition);


                    if (hitPoint != Vector3.zero && hitNormal != Vector3.zero)
                    {
                        SpawnObjectAtHitPosition(hitPoint, hitNormal, spawnObjectToObjectClass.objectToSpawn);
                    }

                }
            }
        }

        waitForNextSpawn = false;
    }

    void SpawnObjectAtHitPosition(Vector3 position, Vector3 normal, GameObject objectToSpawn)
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


    (Vector3 position, Vector3 normal) ShootRaycastFromDetectedPosition(Vector2 rectPosition)
    {
        Ray ray;
        Vector2 convertScreenToRay = new Vector2(rectPosition.x / 2, rectPosition.y / 2);

        ray = mainCamera.ScreenPointToRay(convertScreenToRay);

        RaycastHit[] raycastHits = Physics.RaycastAll(ray, 50f);

        if (raycastHits.Length > 0)
        {
            foreach (var raycastHit in raycastHits)
            {
                if (raycastHit.collider.gameObject.layer == meshLayer)
                {
                    return (raycastHit.point, raycastHit.normal);
                }
            }
        }
        return (Vector3.zero, Vector3.zero);
    }
}