using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    TargetManager Manager;
    public GameObject Copy;
    // Start is called before the first frame update
    void Start()
    {
        Manager = GameObject.Find("OVRCameraRig").GetComponent<TargetManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Manager.DespawnTarget(this.gameObject);
    }
}
