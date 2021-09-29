using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigRedButton : MonoBehaviour
{
    private ArcManager arcManager;
    // Start is called before the first frame update
    void Start()
    {
        arcManager = GameObject.Find("OVRCameraRig").GetComponent<ArcManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Left Hand" || other.gameObject.tag == "Right Hand")
        {
            arcManager.ResetThrowables();
        }
    }
}
