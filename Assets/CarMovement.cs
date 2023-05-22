using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovement : MonoBehaviour
{
    [Serialize]
    public float moveSpeed = 20f;
    [Serialize]
    public float accelerationForce = 400f;
    [Serialize]
    public float turnFactor = 3.5f;
    [Serialize] 
    private Rigidbody2D rb;
    [Serialize] 
    private Transform _transform;

    private float accelerationInput;
    private float steeringInput;
    private float rotationAngle;
    
    private Vector2 _moveDirection;
    private bool _desireAccelerate;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void MoveCar()
    {
        if (_desireAccelerate)
        {
            // Calculate the desired velocity.
            Vector2 desiredVelocity = _moveDirection;

            // Apply the desired velocity to the rigidbody over time using a Lerp() function.
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.right, Time.deltaTime * moveSpeed);
            
        }
    }

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _desireAccelerate = true;
            
        }

        if (context.canceled)
        {
            _desireAccelerate = false;
            
        }
        
        
        
    }

    public void ApplyEngineForce()
    {
        if (_desireAccelerate)
        {
            Vector2 engineForceVector = transform.right * ( accelerationForce * Time.fixedDeltaTime);
            
            rb.AddForce(engineForceVector, ForceMode2D.Force);
        }
        
    }

    public void ApplySteering()
    {
        rotationAngle -= steeringInput * turnFactor;
        rb.MoveRotation(rotationAngle);

    }
    
    public void OnMovement(InputAction.CallbackContext context)
    {
        steeringInput = context.ReadValue<Vector2>().x;
        _moveDirection = context.ReadValue<Vector2>();
    }
    private void FixedUpdate()
    {
        ApplyEngineForce();
        ApplySteering();
    }
}
