using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandGrabBehaviour : HandGrabBehaviour
{
    public bool isThrowingDebug;

    protected override void Start()
    {
        base.Start();
        controller = OVRInput.Controller.LTouch;
        ReferenceHand = GameObject.FindGameObjectWithTag("Left Hand");
        OtherHand = GameObject.FindGameObjectWithTag("Right Hand");
    }

    protected override float checkForGrabbing()
    {
        return OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger);
    }

    protected override bool checkForGrabbingDebug()
    {
        return isThrowingDebug;
    }
}
