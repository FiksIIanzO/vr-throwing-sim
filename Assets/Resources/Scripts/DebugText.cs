using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    public Vector3 LeftHandRelativePos;
    public Vector3 RightHandRelativePos;
    public ThrowingManager.ThrowStages LeftHandThrowStage;
    public ThrowingManager.ThrowTypes LeftHandThrowType;
    public ThrowingManager.ThrowStages RightHandThrowStage;
    public ThrowingManager.ThrowTypes RightHandThrowType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        string text = "";
        var turnTowardsCamera = Quaternion.FromToRotation(Vector3.forward, new Vector3(0, GameObject.FindGameObjectWithTag("MainCamera").transform.forward.y));
        LeftHandRelativePos = GameObject.FindGameObjectWithTag("Left Hand").transform.position - GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        LeftHandRelativePos = Quaternion.Euler(0, -GameObject.FindGameObjectWithTag("MainCamera").transform.eulerAngles.y, 0) * LeftHandRelativePos;
        text += "Lhand pos: " + LeftHandRelativePos + "vel: " + Quaternion.Euler(0, -GameObject.FindGameObjectWithTag("MainCamera").transform.eulerAngles.y, 0) * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        RightHandRelativePos = GameObject.FindGameObjectWithTag("Right Hand").transform.position - GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        RightHandRelativePos = Quaternion.Euler(0, -GameObject.FindGameObjectWithTag("MainCamera").transform.eulerAngles.y, 0) * RightHandRelativePos;
        text += "\nRhand pos: " + RightHandRelativePos + "vel: " + Quaternion.Euler(0, -GameObject.FindGameObjectWithTag("MainCamera").transform.eulerAngles.y, 0) * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
        text += "\nParent pos: " + GameObject.Find("OVRCameraRig").transform.position;
        Debug.DrawRay(GameObject.FindGameObjectWithTag("MainCamera").transform.position, GameObject.FindGameObjectWithTag("MainCamera").transform.forward);
        ThrowingManager leftGrenadeInstance = null;
        try
        {
            leftGrenadeInstance = GameObject.FindGameObjectWithTag("Left Hand").GetComponent<HandGrabBehaviour>().ThrowableInstance.GetComponent<ThrowingManager>();
        }
        catch { }
        if (leftGrenadeInstance != null)
        {
            LeftHandThrowStage = leftGrenadeInstance.ThrowStage;
            LeftHandThrowType = leftGrenadeInstance.ThrowType;
            text += "\nLhand stage: " + LeftHandThrowStage + "; Lhand type: " + LeftHandThrowType;
        }
        else text += "\n Left hand is not holding a grenade";
        ThrowingManager rightGrenadeInstance = null;
        try
        {
            rightGrenadeInstance = GameObject.FindGameObjectWithTag("Right Hand").GetComponent<HandGrabBehaviour>().ThrowableInstance.GetComponent<ThrowingManager>();
        }
        catch { }
        if (rightGrenadeInstance != null)
        {
            RightHandThrowStage = rightGrenadeInstance.ThrowStage;
            RightHandThrowType = rightGrenadeInstance.ThrowType;
            text += "\nRhand stage: " + RightHandThrowStage + "; Rhand type: " + RightHandThrowType;
        }
        else text += "\n Right hand is not holding a grenade";
        text += "\nMovement axis: " + OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        text += "\nDespawned targets: " + GameObject.Find("OVRCameraRig").GetComponent<TargetManager>().DespawnedTargets;
        GetComponent<Text>().text = text;
    }
}
