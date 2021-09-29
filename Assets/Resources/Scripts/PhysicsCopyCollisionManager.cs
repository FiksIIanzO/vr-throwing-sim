using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCopyCollisionManager : MonoBehaviour
{
    private Vector3 CollisionPoint;
    [SerializeField]
    private bool Colliding;

    // Start is called before the first frame update
    void Start()
    {
        Colliding = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Colliding = true;
        CollisionPoint = this.transform.position;
        //GetComponent<Material>().color = Color.black;
    }

    private void OnCollisionExit(Collision collision)
    {
        Colliding = false;
        //GetComponent<Material>().color = Color.white;
    }

    public bool IsColliding()
    {
        return Colliding;
    }


}
