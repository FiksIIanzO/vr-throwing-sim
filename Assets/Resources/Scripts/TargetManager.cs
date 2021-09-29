using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private ArcManager arcManager;

    private List<GameObject> SpawnedTargets;
    private List<GameObject> SpawnedCopies;
    [Range(0, 30)]
    public int TargetAmount;
    public float MinimumY;
    [Min(1)]
    public int MaxSpawnAttempts;
    public bool RespawnTargets;
    public int DespawnedTargets;
    public GameObject TargetBlueprint;
    // Start is called before the first frame update
    void Start()
    {
        arcManager = GameObject.Find("OVRCameraRig").GetComponent<ArcManager>();
        SpawnedTargets = new List<GameObject>();
        for (int i = 0; i < TargetAmount; i++)
        {
            GameObject SpawnedTarget = SpawnTarget();
            if (SpawnedTarget != null)
            {
                SpawnedTargets.Add(SpawnedTarget);
            }
            else Debug.LogError("Failed to create target");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnedTargets.Count < TargetAmount && RespawnTargets)
        {
            GameObject SpawnedTarget = SpawnTarget();
            if (SpawnedTarget != null)
            {
                SpawnedTargets.Add(SpawnedTarget);
            }
        }
    }

    public GameObject SpawnTarget()
    {
        Vector3 position = RandomizePosition();
        bool spawnPosValid = false;
        for (int i = 0; i < MaxSpawnAttempts; i++)
        {
            foreach (var _t in SpawnedTargets)
            {
                if ((position - _t.transform.position).magnitude < _t.GetComponent<SphereCollider>().radius * 2)
                {
                    position = RandomizePosition();
                    break;
                }
            }
            spawnPosValid = true;
        }
        if (spawnPosValid)
        {
            GameObject Target = Instantiate(TargetBlueprint, position, Quaternion.identity);
            TargetController targetController = Target.GetComponent<TargetController>();
            targetController.Copy = arcManager.AddObjectTracking(Target);
            return Target;
        }
        else return null;
    }

    private Vector3 RandomizePosition()
    {
        Vector3 random = new Vector3();
        random.y = MinimumY - 1;
        while (random.y < MinimumY)
        {
            random = Random.insideUnitSphere * 12;
        }
        return random;
    }

    public bool DespawnTarget(GameObject _target)
    {
        bool despawned = SpawnedTargets.Remove(_target);
        if (despawned)
        {
            arcManager.RemoveObjectTracking(_target.GetComponent<TargetController>().Copy);
            Destroy(_target);
            DespawnedTargets++;
        }
        return despawned;
    }
}
