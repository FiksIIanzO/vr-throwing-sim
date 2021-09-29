using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableParameters : MonoBehaviour
{
    ArcManager arcManager;
    public double LowThrowGravity = 1;
    public double MidThrowGravity = 0.9;
    public double HighThrowGravity = 1;
    private double LowThrowVelocityModifier;
    private double MidThrowVelocityModifier;
    private double HighThrowVelocityModifier;
    public double ThrowMass = 1;
    public Vector3 Velocity;
    private Vector3 BaseGravity;
    public Vector3 Gravity;
    public ThrowingManager.ThrowTypes ThrowType;
    public bool Thrown;
    public bool GravityCorrected;
    public Rigidbody rb;
    public Material defaultMaterial;
    public Vector3 defaultTransformPos;
    public Quaternion defaultTransformRot;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        arcManager = GameObject.Find("OVRCameraRig").GetComponent<ArcManager>();
        LowThrowVelocityModifier = arcManager.LowThrowVelocityModifier;
        MidThrowVelocityModifier = arcManager.MidThrowVelocityModifier;
        HighThrowVelocityModifier = arcManager.HighThrowVelocityModifier;
        defaultMaterial = this.GetComponent<MeshRenderer>().material;
        defaultTransformPos = transform.position;
        defaultTransformRot = transform.rotation;
        Thrown = false;
        BaseGravity = Gravity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        ApplyCustomGravity();
    }

    public void Throw(ThrowingManager.ThrowTypes _type, Vector3 _velocity)
    {
        if (_type != ThrowingManager.ThrowTypes.None)
        {
            rb.useGravity = false;
        }
        rb.isKinematic = false;
        rb.velocity = _velocity;
        if (_type == ThrowingManager.ThrowTypes.Low)
        {
            rb.velocity = rb.velocity * (float)LowThrowVelocityModifier / (float)ThrowMass;
        } else if (_type == ThrowingManager.ThrowTypes.Mid)
        {
            rb.velocity = rb.velocity * (float)MidThrowVelocityModifier / (float)ThrowMass;
        } else if (_type == ThrowingManager.ThrowTypes.High)
        {
            rb.velocity = rb.velocity * (float)HighThrowVelocityModifier / (float)ThrowMass;
        }
        ThrowType = _type;
        Thrown = true;
    }

    public void Throw(ThrowingManager.ThrowTypes _type, Vector3 _velocity, float _gravity)
    {
        if (_type != ThrowingManager.ThrowTypes.None)
        {
            rb.useGravity = false;
        }
        rb.isKinematic = false;
        rb.velocity = _velocity;
        if (_type == ThrowingManager.ThrowTypes.Low)
        {
            rb.velocity = rb.velocity * (float)LowThrowVelocityModifier / (float)ThrowMass;
        }
        else if (_type == ThrowingManager.ThrowTypes.Mid)
        {
            rb.velocity = rb.velocity * (float)MidThrowVelocityModifier / (float)ThrowMass;
        }
        else if (_type == ThrowingManager.ThrowTypes.High)
        {
            rb.velocity = rb.velocity * (float)HighThrowVelocityModifier / (float)ThrowMass;
        }
        ThrowType = _type;
        Thrown = true;
        Gravity = new Vector3 (0, -_gravity, 0);
        GravityCorrected = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.useGravity = true;
        ThrowType = ThrowingManager.ThrowTypes.None;
        Thrown = false;
        if (GravityCorrected) Gravity = BaseGravity;
        GravityCorrected = false;
    }

    public void SetGravity(ThrowingManager.ThrowTypes _type)
    {
        if (_type != ThrowingManager.ThrowTypes.None)
        {
            rb.useGravity = false;
            Gravity = Physics.gravity;
            if (_type == ThrowingManager.ThrowTypes.Low)
            {
                Gravity.y = Gravity.y * (float)LowThrowGravity;
            }
            else if (_type == ThrowingManager.ThrowTypes.Mid)
            {
                Gravity.y = Gravity.y * (float)MidThrowGravity;
            }
            else if (_type == ThrowingManager.ThrowTypes.High)
            {
                Gravity.y = Gravity.y * (float)HighThrowGravity;
            }
        }
        else rb.useGravity = true;
    }

    public void ApplyCustomGravity()
    {
        SetGravity(ThrowType);
        if (!rb.useGravity && Thrown)
        {
            rb.AddForce(Gravity);
        }
    }

    /*public void ApplyCustomGravity(float _gravity)
    {
        SetGravity(ThrowType);
        if (!rb.useGravity && Thrown)
        {
            rb.AddForce(_gravity);
        }
    }*/

    public void SwitchMaterial(bool _isSelected)
    {
        if (_isSelected) GetComponent<MeshRenderer>().material = arcManager.GrabMaterial;
        else GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    public void Reset()
    {
        transform.position = defaultTransformPos;
        transform.rotation = defaultTransformRot;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.parent = null;
        Thrown = false;
    }
}
