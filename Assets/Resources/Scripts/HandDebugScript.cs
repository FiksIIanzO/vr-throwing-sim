using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDebugScript : MonoBehaviour
{
    public GameObject rightHand;
    public GameObject leftHand;
    HandGrabBehaviour rightHandBehaviour;
    HandGrabBehaviour leftHandBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        rightHandBehaviour = rightHand.GetComponent<HandGrabBehaviour>();
        leftHandBehaviour = leftHand.GetComponent<HandGrabBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
