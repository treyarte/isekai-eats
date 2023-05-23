using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovement : MonoBehaviour
{
    
    [Header("Car Settings")]
    [Serialize] 
    public float driftFactor = 0.95f;
    [Serialize]
    public float accelerationForce = 10f;
    [Serialize]
    public float turnFactor = 3.5f;
    [Serialize]
    public float maxSpeed = 20f;
    
    [Serialize] 
    private Rigidbody2D rb;
    [Serialize] 
    private Transform _transform;

    private float accelerationInput;
    private float steeringInput;
    private float rotationAngle;
    private float _reverseInput;
    
    private Vector2 _moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyEngineForce()
    {
        //Calculate how much forward we are going in terms of the direction of our velocity
        var velocityVsUp = Vector2.Dot(transform.up, rb.velocity);

        // Limit so we cannot go faster than the max speed in the forward direction
        if (velocityVsUp > maxSpeed && accelerationInput > 0)
        {
            return;
        }
        
        // limit so we cannot go faster that the 50% of max speed in reverse direction
        if (velocityVsUp > -maxSpeed && accelerationInput < 0)
        {
            return;
        }

        //limit so we cannot go faster in any direction while accelerating
        if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
        {
            return;
        }
        
        if (accelerationInput == 0 || accelerationInput == 0 && _reverseInput == 0)
        {
            rb.drag = Mathf.Lerp(rb.drag, 3.0f, Time.fixedDeltaTime * 3);
        }
        else
        {
            rb.drag = 0;
        }

        if (accelerationInput != 0 && _reverseInput != 0)
        {
            return;
        }


        Vector2 engineForceVector = transform.right * ( accelerationForce * accelerationInput);
        
        rb.AddForce(engineForceVector, ForceMode2D.Force);

    }

    public void ApplySteering()
    {
        float minSpeedBeforeAllowTurningFactor = (rb.velocity.magnitude / 8);

        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor); 
            
        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;
        rb.MoveRotation(rotationAngle);

    }

    private void KillOrthogonalVelocity()
    {
        var transformUp = transform.up;
        var transformRight = transform.right;
        
        Vector2 forwardVelocity = transformUp * Vector2.Dot(rb.velocity, transformUp);
        Vector2 rightVelocity = transformRight * Vector2.Dot(rb.velocity, transformRight);

        rb.velocity = rightVelocity + forwardVelocity * driftFactor;
    }

    public void OnAcceleration(InputAction.CallbackContext context)
    {
        accelerationInput = context.ReadValue<float>();
    }

    /// <summary>
    /// TODO Add reverse
    /// </summary>
    /// <param name="context"></param>
    public void OnReverse(InputAction.CallbackContext context)
    {
        _reverseInput = context.ReadValue<float>();
        
        Debug.Log(_reverseInput);
    }
    
    public void OnMovement(InputAction.CallbackContext context)
    {
         Vector2 inputVector = context.ReadValue<Vector2>();
         
         steeringInput = inputVector.x;
         // accelerationInput = inputVector.y;
         
         // Debug.Log(accelerationInput);
    }
    private void FixedUpdate()
    {
        ApplyEngineForce();
        KillOrthogonalVelocity();
        ApplySteering();
    }
}
