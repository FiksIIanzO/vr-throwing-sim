using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcManager : MonoBehaviour
{
    public Scene predictionScene;
    public PhysicsScene predictionPhysicsScene;
    private Scene currentScene;
    private PhysicsScene currentPhysicsScene;
    public GameObject Arc;
    public GameObject CollisionSprite;
    public List<GameObject> ObstacleCopies;
    public List<GameObject> Throwables;

    public int maxIterations = 1000;
    public float resolution = 2;
    public int baseVelocity = 10;
    public Vector3 aimOffset = new Vector3(0, -0.5f, 0);
    public Vector3 handSpecificAimOffset = new Vector3(0.2f, 0, 0);
    
    public double LowThrowVelocityModifier = 1;
    public double MidThrowVelocityModifier = 0.8;
    public double HighThrowVelocityModifier = 1.5;
    
    public float forwardTrackingThreshold = 0.2f;
    public float backwardTrackingThreshold = 0;
    public float lowTrackingThreshold = -0.2f;
    public float highTrackingThreshold = 0f;
    public float lowVelocityThreshold = 0.2f;
    public float highVelocityThreshold = 0.1f;
    
    public Material LowAimMaterial, MidAimMaterial, HighAimMaterial, ThrowMaterial, GrabMaterial;
    // Start is called before the first frame update
    void Start()
    {
        Physics.autoSimulation = false;
        CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        predictionScene = SceneManager.CreateScene("Prediction", parameters);
        Debug.Log("Created prediction scene " + predictionScene);
        ObstacleCopies = new List<GameObject>();
        foreach (var _o in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            ObstacleCopies.Add(CreatePhysicsCopy(_o));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (currentPhysicsScene.IsValid())
        {
            currentPhysicsScene.Simulate(Time.fixedDeltaTime);
        }
    }

    public GameObject CreatePhysicsCopy(GameObject _o)
    {
        GameObject Copy = Instantiate(_o.gameObject);
        if (Copy.tag == "Throwable")
        {
            Copy.AddComponent<PhysicsCopyCollisionManager>();
        }
        Debug.Log("Instantiated " + _o);
        SceneManager.MoveGameObjectToScene(Copy, predictionScene);
        Debug.Log(" to scene " + predictionScene);
        return Copy;
    }

    public void ResetThrowables()
    {
        foreach (var _t in Throwables)
        {
            ThrowableParameters throwableParameters = _t.GetComponent<ThrowableParameters>();
            throwableParameters.Reset();
        }
    }

    public GameObject AddObjectTracking(GameObject _object)
    {
        GameObject copy = CreatePhysicsCopy(_object);
        ObstacleCopies.Add(copy);
        return copy;
    }

    public bool RemoveObjectTracking(GameObject _object)
    {
        var ok = ObstacleCopies.Remove(_object);
        Destroy(_object);
        return ok;
    }
}
