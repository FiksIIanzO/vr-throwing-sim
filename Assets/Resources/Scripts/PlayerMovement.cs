using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController controller;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void FixedUpdate()
    {
        Vector2 movement = new Vector2();
        movement = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        movement = movement * speed;
        controller.Move(Quaternion.Euler(0, GameObject.FindGameObjectWithTag("MainCamera").transform.eulerAngles.y, 0) * new Vector3(movement.x, 0, movement.y));
    }
}
