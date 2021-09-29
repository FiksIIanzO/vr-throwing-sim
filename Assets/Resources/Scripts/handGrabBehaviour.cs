using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrabBehaviour : MonoBehaviour
{
    private ArcManager arcManager;

    public bool IsGrabbing;
    protected bool HoldingObject;
    public GameObject HandModel;
    public GameObject SelectedThrowable;
    public GameObject ThrowableInstance;
    public OVRInput.Controller controller;
    public bool resettable;

    public GameObject ReferenceHand;
    public GameObject OtherHand;
    protected GameObject ThrowingArc;
    protected ThrowingManager objectManager;
    protected Vector3 AimOffset;
    //Debug parameters
    public bool IsDebug;
    public GameObject DebugGrenade;

    protected virtual void Start()
    {
        IsGrabbing = false;
        controller = OVRInput.Controller.Touch;
        arcManager = GameObject.Find("OVRCameraRig").GetComponent<ArcManager>();
    }

    protected virtual float checkForGrabbing()
    {
        return 0;
    }

    protected virtual bool checkForGrabbingDebug()
    {
        return false;
    }

    protected void Update()
    {
        if (checkForGrabbing() > 0.5 || checkForGrabbingDebug())
        {
            IsGrabbing = true;
        }
        else IsGrabbing = false;
        if (IsGrabbing && !HoldingObject)
        {
            if (IsDebug) CreateGrenade();
            else if (SelectedThrowable != null)
            {
                GrabThrowable();
                GetComponent<LineRenderer>().enabled = false;
            }
        }
        else if (!IsGrabbing && HoldingObject)
        {
            LetGoOfGrenade();
            GetComponent<LineRenderer>().enabled = true;
        }
        if (resettable && OVRInput.Get(OVRInput.RawButton.A) || Input.GetKey(KeyCode.R))
        {
            arcManager.ResetThrowables();
            if (SelectedThrowable != null)
            {
                ThrowableParameters throwableParameters = SelectedThrowable.GetComponent<ThrowableParameters>();
                throwableParameters.SwitchMaterial(false);
                SelectedThrowable = null;
            }
        }
    }

    protected void CreateGrenade()
    {
        HandModel.SetActive(false);
        ThrowableInstance = Instantiate(DebugGrenade, this.transform, false);
        Debug.Log("Grenade created");
        objectManager = ThrowableInstance.GetComponent<ThrowingManager>();
        Debug.Log("OM: " + objectManager);
        if (objectManager == null)
        {
            Debug.Log("Trying to create OM");
            objectManager = ThrowableInstance.AddComponent<ThrowingManager>();
            Debug.Log("OM2: " + objectManager);
            objectManager.SetupThrow(ThrowableInstance, gameObject, OtherHand, AimOffset);
        }
        else objectManager.enabled = true;
        Debug.Log("Manager status: " + objectManager.enabled);
        ThrowableInstance.GetComponent<Rigidbody>().isKinematic = true;
        HoldingObject = true;
    }

    //Grabs currently selected throwable
    protected void GrabThrowable()
    {
        HandModel.SetActive(false);
        ThrowableInstance = SelectedThrowable;
        ThrowableInstance.transform.SetParent(transform, false);
        ThrowableInstance.transform.position = transform.position;
        Debug.Log("Throwable grabbed");
        objectManager = ThrowableInstance.GetComponent<ThrowingManager>();
        Debug.Log("OM: " + objectManager);
        if (objectManager == null)
        {
            Debug.Log("Trying to create OM");
            objectManager = ThrowableInstance.AddComponent<ThrowingManager>();
            Debug.Log("OM2: " + objectManager);
        }
        else objectManager.enabled = true;
        objectManager.SetupThrow(ThrowableInstance, gameObject, OtherHand, AimOffset);
        Debug.Log("Manager status: " + objectManager.enabled);
        ThrowableInstance.GetComponent<Rigidbody>().isKinematic = true;
        HoldingObject = true;
    }

    protected void LetGoOfGrenade()
    {
        objectManager.CompleteThrow();
        ThrowableInstance.transform.parent = null;
        HandModel.SetActive(true);
        HoldingObject = false;
        Debug.Log("Grenade dropped");
    }

    protected void FixedUpdate()
    {
        if (!IsGrabbing) SelectThrowable();
    }

    protected void SelectThrowable()
    {
        Debug.DrawRay(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("ThrowableGrabCollider"), QueryTriggerInteraction.Collide) &&
            !IsGrabbing)
        {
            ThrowableParameters throwableParameters;
            if (SelectedThrowable != null)
            {
                throwableParameters = SelectedThrowable.GetComponent<ThrowableParameters>();
                throwableParameters.SwitchMaterial(false);
            }

            SelectedThrowable = hit.collider.gameObject.transform.parent.gameObject;
            throwableParameters = SelectedThrowable.GetComponent<ThrowableParameters>();
            throwableParameters.SwitchMaterial(true);
        }
        else if (SelectedThrowable != null)
        {
            ThrowableParameters throwableParameters = SelectedThrowable.GetComponent<ThrowableParameters>();
            throwableParameters.SwitchMaterial(false);
            SelectedThrowable = null;
        }
        LineRenderer aimLine = GetComponent<LineRenderer>();
        aimLine.positionCount = 100;
        Vector3 aimLinePoint = transform.position;
        for (int i = 0; i < 100; i++)
        {
            aimLine.SetPosition(i, aimLinePoint);
            aimLinePoint += transform.forward;
        }

        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("BigRedButton"), QueryTriggerInteraction.Collide))
        {
            resettable = true;
        }
        else resettable = false;
    }

    protected void OnTriggerEnter(Collision other)
    {
        if (other.gameObject.tag == "BigRedButton")
        {
            arcManager.ResetThrowables();
            ThrowableParameters throwableParameters = SelectedThrowable.GetComponent<ThrowableParameters>();
            throwableParameters.SwitchMaterial(false);
            SelectedThrowable = null;
        }
    }
}
