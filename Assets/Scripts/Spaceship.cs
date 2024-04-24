using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(Rigidbody))]
public class Spaceship : MonoBehaviour
{
    [Header("Ship Movement Settings")]
    [SerializeField]
    private float yawTorque = 100f;
    [SerializeField]
    private float pitchTorque = 200f;
    [SerializeField]
    private float rollTorque = 100f;
    [SerializeField]
    private float thrust = 100f;
    [SerializeField]
    private float upThrust = 50f;
    [SerializeField]
    private float strafeThrust = 50f;
    private float glide = 0f;
    private float verticalGlide = 0f;
    private float horizontalGlide = 0f;

    [SerializeField, Range(0.001f, 0.999f)]
    private float thrustGlideReduction = 0.999f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float upDownGlideReduction = 0.111f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float leftRightGlideReduction = 0.111f;

    [Header("Boost Settings")]
    [SerializeField]
    private float maxBoostAmount = 2f;
    [SerializeField]
    private float boostDeprecationRate = 0.25f;
    [SerializeField]
    private float boostRechargeRate = 0.5f;
    [SerializeField]
    private float boostMultiplier = 5f;
    [SerializeField]
    private bool boosting = false;
    [SerializeField]
    private float currentBoostAmount;

    [Header("Gun Settings")]
    public GameObject shoot;
    public GameObject gunPoint;


    Rigidbody thisRigidbody;

    //Input Values
    private float thrust1D;  
    private float strafe1D;
    private float upDown1D;   
    private float roll1D;
    private Vector2 pitchYaw;

    void Start()
    {
        thisRigidbody = GetComponent<Rigidbody>();
        currentBoostAmount = maxBoostAmount;
    }

    
    void Update()
    {
        HandleMovement();
        HandleBoosting();
    }

    void HandleMovement()
    {
        //roll
        thisRigidbody.AddRelativeTorque(Vector3.back * roll1D * rollTorque * Time.deltaTime);

        //pitch
        thisRigidbody.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYaw.y, -1f, 1f) * pitchTorque * Time.deltaTime);

        //yaw
        thisRigidbody.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);

        //Thrust
        if(thrust1D > 0.1f || thrust1D < -0.1f)
        {
            float currentThrust;

            if(boosting)
            {
                currentThrust = thrust * boostMultiplier;
            }
            else
            {
                currentThrust = thrust;
            }

            thisRigidbody.AddRelativeForce(Vector3.forward * thrust1D * currentThrust * Time.deltaTime);
            glide = thrust;
        }
        else
        {
            thisRigidbody.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
            glide *= thrustGlideReduction;
        }

        //Up/Down
        if (upDown1D > 0.1f || upDown1D < -0.1f)
        {
            thisRigidbody.AddRelativeForce(Vector3.up * upDown1D * upThrust * Time.fixedDeltaTime);
            verticalGlide = upDown1D * upThrust;
        }
        else
        {
            thisRigidbody.AddRelativeForce(Vector3.up * verticalGlide * Time.fixedDeltaTime);
            verticalGlide *= upDownGlideReduction;
        }

        //Strafe
        if (strafe1D > 0.1f || strafe1D < -0.1f)
        {
            thisRigidbody.AddRelativeForce(Vector3.right * strafe1D * upThrust * Time.deltaTime);
            horizontalGlide = strafe1D * strafeThrust;
        }
        else
        {
            thisRigidbody.AddRelativeForce(Vector3.right * horizontalGlide * Time.deltaTime);
            horizontalGlide *= leftRightGlideReduction;
        }
    }

    void HandleBoosting()
    {
        if(boosting && currentBoostAmount > 0f)
        {
            currentBoostAmount -= boostDeprecationRate;

            if (currentBoostAmount <= 0f)
            {
                boosting = false;
            }
        }
        else
        {
            if(currentBoostAmount < maxBoostAmount)
            {
                currentBoostAmount += boostRechargeRate;
            }
        }
    }

    void FireHandle()
    {
        GameObject thisShoot = Instantiate(shoot, gunPoint.transform.position, gunPoint.transform.rotation);
        
    }

    #region Input Metods

    public void OnThrust(InputAction.CallbackContext context)
    {
        thrust1D = context.ReadValue<float>();
    }

    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe1D = context.ReadValue<float>();
    }

    public void OnUpDown(InputAction.CallbackContext context)
    {
        upDown1D = context.ReadValue<float>();
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        roll1D = context.ReadValue<float>();
    }

    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
    } 

    public void OnBoost(InputAction.CallbackContext context) 
    {
        boosting = context.performed;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        FireHandle();
    }

    #endregion
}
