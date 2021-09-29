using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThrowingManager : MonoBehaviour
{
    public GameObject thisHand;
    public GameObject aimHand;
    public GameObject Head;
    public LineRenderer arcRenderer;
    public GameObject PhysicsCopy;
    public PhysicsCopyCollisionManager CopyCollisionData;
    public GameObject CollisionPointSprite;
    public GameObject Arc;
    public List<GameObject> ObstacleCopies;
    bool Setup = false;
    public float forwardTrackingThreshold;
    public float backwardTrackingThreshold;
    public float lowTrackingThreshold;
    public float highTrackingThreshold;
    public float lowVelocityThreshold;
    public float highVelocityThreshold;
    public ThrowTypes ThrowType;
    public ThrowStages ThrowStage;

    public Scene predictionScene;
    private PhysicsScene predictionPhysicsScene;
    private Scene currentScene;
    private PhysicsScene currentPhysicsScene;
    public int maxIterations;

    public Vector3 appliedForce;
    public Vector3 copyAppliedForce;
    private int baseVelocity;
    private float resolution;
    private Vector3 aimOffset;
    private ArcManager arcManager;

    public bool isSwingingDebug;
    public float throwHeight;
    public Vector3 lastCalculatedPoint;
    public Vector3 LastLaunchPosition;
    public Vector3 LastAppliedForce;

    public Material LowAimMaterial, MidAimMaterial, HighAimMaterial, ThrowMaterial;

    private void Start()
    {
        arcManager = GameObject.Find("OVRCameraRig").GetComponent<ArcManager>();
    }

    private void FixedUpdate()
    {
        if (Setup)
        {
            HandGrabBehaviour thisHandGrabBehaviour = thisHand.GetComponent<HandGrabBehaviour>();
            SetThrowStage(ThrowStage);
            if (ThrowStage == ThrowStages.None)
            {
                appliedForce = OVRInput.GetLocalControllerVelocity(thisHandGrabBehaviour.controller);
                arcRenderer.enabled = false;
            }   
            else if (ThrowStage == ThrowStages.Aim)
            {
                PhysicsCopy.transform.position = transform.position;
                LastLaunchPosition = transform.position;
                Vector3 Direction = aimHand.transform.position - Head.transform.position/* + aimOffset*/; //TODO: add hand specific offset
                appliedForce = OVRInput.GetLocalControllerVelocity(thisHandGrabBehaviour.controller);
                copyAppliedForce = Direction.normalized * baseVelocity;
                LastAppliedForce = copyAppliedForce;
                ThrowableParameters PhysicsCopyParameters = PhysicsCopy.GetComponent<ThrowableParameters>();
                PhysicsCopyParameters.Throw(ThrowType, copyAppliedForce);
                arcRenderer.enabled = true;
                arcRenderer.positionCount = maxIterations;
                for (int i = 0; i < maxIterations; i++)
                {
                    PhysicsCopyParameters.ApplyCustomGravity();
                    predictionPhysicsScene.Simulate(Time.fixedDeltaTime * resolution);
                    PhysicsCopyCollisionManager physicsManager = PhysicsCopy.GetComponent<PhysicsCopyCollisionManager>();
                    if (physicsManager.IsColliding())
                    {
                        CollisionPointSprite.SetActive(true);
                        CollisionPointSprite.transform.position = PhysicsCopy.transform.position;
                        arcRenderer.SetPosition(i, PhysicsCopy.transform.position);
                        arcRenderer.positionCount = i + 1;
                        break;
                    }
                    else
                    {
                        CollisionPointSprite.SetActive(false);
                        arcRenderer.SetPosition(i, PhysicsCopy.transform.position);
                        lastCalculatedPoint = PhysicsCopy.transform.position;
                    }
                    if (throwHeight < PhysicsCopy.transform.position.y) throwHeight = PhysicsCopy.transform.position.y;
                }
                Debug.Log(maxIterations + " iterations calculated");
                if (ThrowType == ThrowTypes.Low) arcRenderer.material = LowAimMaterial;
                else if (ThrowType == ThrowTypes.Mid) arcRenderer.material = MidAimMaterial;
                else arcRenderer.material = HighAimMaterial;
            } else
            {
                PhysicsCopy.transform.position = transform.position;
                ThrowableParameters PhysicsCopyParameters = PhysicsCopy.GetComponent<ThrowableParameters>();
                var TrajectoryCorrection = CorrectTrajectory(LastLaunchPosition, transform.position, lastCalculatedPoint);
                copyAppliedForce = LastAppliedForce;
                copyAppliedForce.x *= TrajectoryCorrection.x;
                copyAppliedForce.z *= TrajectoryCorrection.z;
                PhysicsCopyParameters.Throw(ThrowType, copyAppliedForce);
                arcRenderer.enabled = true;
                arcRenderer.positionCount = maxIterations;
                for (int i = 0; i < maxIterations; i++)
                {
                    PhysicsCopyParameters.ApplyCustomGravity();
                    predictionPhysicsScene.Simulate(Time.fixedDeltaTime * resolution);
                    PhysicsCopyCollisionManager physicsManager = PhysicsCopy.GetComponent<PhysicsCopyCollisionManager>();
                    if (physicsManager.IsColliding())
                    {
                        arcRenderer.positionCount = i;
                        break;
                    }
                    else
                    {
                        arcRenderer.SetPosition(i, PhysicsCopy.transform.position);
                    }
                }
                appliedForce = copyAppliedForce;
                arcRenderer.material = ThrowMaterial;
            }
        }
    }

    public Vector3 CompleteThrow()
    {
        Destroy(Arc);
        Destroy(PhysicsCopy);
        Destroy(CollisionPointSprite);
        Debug.Log("Cleaned up throwing arc");
        this.enabled = false;
        ThrowableParameters throwableParameters = gameObject.GetComponent<ThrowableParameters>();
        throwableParameters.Throw(ThrowType, appliedForce);
        return appliedForce;
    }

    public ThrowingManager SetupThrow(GameObject Throwable, GameObject ThrowingHand, GameObject AimHand, Vector3 _AimOffset)
    {
        predictionScene = arcManager.predictionScene;
        thisHand = ThrowingHand;
        aimHand = AimHand;
        aimOffset = _AimOffset;
        Head = GameObject.Find("CenterEyeAnchor");
        PhysicsCopy = arcManager.CreatePhysicsCopy(Throwable);
        PhysicsCopy.GetComponent<MeshRenderer>().enabled = false;
        Debug.Log("PhysicsCopy " + PhysicsCopy);
        if (PhysicsCopy != null)
        {
            Setup = true;
            Debug.Log("Throwing arc set up");
        }
        maxIterations = arcManager.maxIterations;
        predictionPhysicsScene = predictionScene.GetPhysicsScene();
        Arc = Instantiate(arcManager.Arc, transform);
        arcRenderer = Arc.GetComponent<LineRenderer>();
        baseVelocity = arcManager.baseVelocity;
        resolution = arcManager.resolution;
        CollisionPointSprite = Instantiate(arcManager.CollisionSprite);
        CollisionPointSprite.SetActive(false);

        forwardTrackingThreshold = arcManager.forwardTrackingThreshold;
        backwardTrackingThreshold = arcManager.backwardTrackingThreshold;
        lowTrackingThreshold = arcManager.lowTrackingThreshold;
        highTrackingThreshold = arcManager.highTrackingThreshold;
        lowVelocityThreshold = arcManager.lowVelocityThreshold;
        highVelocityThreshold = arcManager.highVelocityThreshold;

        LowAimMaterial = arcManager.LowAimMaterial;
        MidAimMaterial = arcManager.MidAimMaterial;
        HighAimMaterial = arcManager.HighAimMaterial;
        ThrowMaterial = arcManager.ThrowMaterial;
        return this;
    }

    //ћетод, устанавливающий текущие стадии броска в зависимости от относительных положений головы и рук
    public void SetThrowStage(ThrowStages _stage)
    {
        if (isSwingingDebug)
        {
            ThrowStage = ThrowStages.Swing;
            return;
        }
        Vector3 LocalPositionThisHand = thisHand.transform.position - Head.transform.position; //ќтносительное положение бросающей руки
        Vector3 LocalPositionAimHand = aimHand.transform.position - Head.transform.position; //ќтносительное положение прицельной руки
        //ќтносительные положени€ с базисом поворота головы
        Vector3 RelativeToHeadPositionThisHand = Quaternion.Euler(0, -Head.transform.eulerAngles.y, 0) * LocalPositionThisHand;
        Vector3 RelativeToHeadPositionAimHand = Quaternion.Euler(0, -Head.transform.eulerAngles.y, 0) * LocalPositionAimHand;
        HandGrabBehaviour thisHandBehaviour = thisHand.GetComponent<HandGrabBehaviour>();
        Vector3 LocalVelocityThisHand = OVRInput.GetLocalControllerVelocity(thisHandBehaviour.controller);
        Vector3 RelativeToHeadVelocityThisHand = Quaternion.Euler(-Head.transform.eulerAngles) * LocalVelocityThisHand;
        if (_stage == ThrowStages.None) //ѕри нулевой стадии можно перейти только на стадию прицеливани€
        {
            SetThrowTypeAim(RelativeToHeadPositionThisHand, RelativeToHeadPositionAimHand);
        } else if (_stage == ThrowStages.Swing)
        {
            if (LocalVelocityThisHand.magnitude < lowVelocityThreshold)
            {
                SetThrowTypeAim(RelativeToHeadPositionThisHand, RelativeToHeadPositionAimHand);
            }
            else ThrowStage = ThrowStages.Swing;
        }
        else //if (ThrowStage == Aim)
        {
            if (RelativeToHeadVelocityThisHand.z > highVelocityThreshold)
                ThrowStage = ThrowStages.Swing;
            else
            {
                SetThrowTypeAim(RelativeToHeadPositionThisHand, RelativeToHeadPositionAimHand);
            }
        }
    }

    //ћетод, определ€ющий тип броска по высоте бросающей руки на стадии прицеливани€
    private void SetThrowTypeAim(Vector3 RelativeToHeadPositionThisHand, Vector3 RelativeToHeadPositionAimHand)
    {
        if (RelativeToHeadPositionThisHand.z <= backwardTrackingThreshold && RelativeToHeadPositionAimHand.z >= forwardTrackingThreshold)
        {
            if (RelativeToHeadPositionThisHand.y <= lowTrackingThreshold)
                ThrowType = ThrowTypes.Low;
            else if (RelativeToHeadPositionThisHand.y < highTrackingThreshold)
                ThrowType = ThrowTypes.Mid;
            else ThrowType = ThrowTypes.High;
            ThrowStage = ThrowStages.Aim;
        }
        else
        {
            ThrowStage = ThrowStages.None;
            ThrowType = ThrowTypes.None;
        }
    }

    public enum ThrowTypes
    {
        None = 0, //Ѕросок не происходит
        Low = 1,
        Mid = 2,
        High = 3
    }

    public enum ThrowStages
    {
        None = 0, //Ѕросок не происходит
        Aim = 1, //—тади€ прицеливани€
        Swing = 2 //—тади€ замаха
    }

    /*
    Vector3 Launch(GameObject _projectile, Vector3 _target, Vector3 _launchVector, float _gravity)
    {
        // think of it as top-down view of vectors: 
        //   we don't care about the y-component(height) of the initial and target position.
        Vector3 projectileXZPos = new Vector3(_projectile.transform.position.x, 0.0f, _projectile.transform.position.z);
        Vector3 targetXZPos = new Vector3(_target.x, 0.0f, _target.z);

        // rotate the object to face the target
        _projectile.transform.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = _gravity;
        float buf = Vector3.Angle(new Vector3(_launchVector.x, 0, _launchVector.z), _launchVector);
        float tanAlpha = Mathf.Tan(buf * Mathf.Deg2Rad);
        float H = _target.y - _projectile.transform.position.y;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = _projectile.transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        return globalVelocity;
    } */

        //ћетод, корректирующий траекторию по x и z во врем€ стадии замаха
    public Vector3 CorrectTrajectory(Vector3 InitialLaunchPosition, Vector3 LaunchPosition, Vector3 TargetPosition)
    {
        float InitialXDistance = TargetPosition.x - InitialLaunchPosition.x;
        float InitialZDistance = TargetPosition.z - InitialLaunchPosition.z;
        float RealXDistance = TargetPosition.x - LaunchPosition.x;
        float RealZDistance = TargetPosition.z - LaunchPosition.z;
        if (InitialXDistance == 0) InitialXDistance = 0.00001f;
        if (InitialZDistance == 0) InitialZDistance = 0.00001f;
        float XRatio = RealXDistance / InitialXDistance;
        float ZRatio = RealZDistance / InitialZDistance;
        return new Vector3(XRatio, 0, ZRatio);
    }

}
