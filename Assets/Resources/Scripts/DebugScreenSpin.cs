using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScreenSpin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.SetPositionAndRotation(Vector3.zero, GameObject.FindGameObjectWithTag("MainCamera").transform.rotation);
        //transform.Rotate(new Vector3(0, GameObject.FindGameObjectWithTag("MainCamera").transform.rotation.eulerAngles.y));
        transform.rotation = Quaternion.Euler(new Vector3(0, GameObject.FindGameObjectWithTag("MainCamera").transform.rotation.eulerAngles.y));
    }
}
