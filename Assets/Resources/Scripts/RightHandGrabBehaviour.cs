using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandGrabBehaviour : HandGrabBehaviour
{
    protected override void Start()
    {
        base.Start();
        controller = OVRInput.Controller.RTouch;
        ReferenceHand = GameObject.FindGameObjectWithTag("Right Hand");
        OtherHand = GameObject.FindGameObjectWithTag("Left Hand");
    }

    protected override float checkForGrabbing()
    {
        return OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger);
    }
}
