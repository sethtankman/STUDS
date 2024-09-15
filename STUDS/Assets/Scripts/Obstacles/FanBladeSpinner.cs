using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanBladeSpinner : MonoBehaviour
{
    // The object to orbit
    public Transform target;

    // Speed of orbit (target speed for spinning)
    public float maxSpeed = 750f;

    // Current speed of the fan blades
    private float currentSpeed = 0f;

    // How fast the fan spins up or slows down
    public float acceleration = 50f;
    public float deceleration = 50f;

    // Fan states
    public enum FanState { Stopped, SpinningUp, ConstantSpeed, SlowingDown }
    public FanState currentState = FanState.ConstantSpeed;

    private void Update()
    {
        if (target != null)
        {
            // Handle fan speed based on state
            switch (currentState)
            {
                case FanState.Stopped:
                    currentSpeed = 0f;
                    break;
                case FanState.SpinningUp:
                    SpinUp();
                    break;
                case FanState.ConstantSpeed:
                    MaintainSpeed();
                    break;
                case FanState.SlowingDown:
                    SlowDown();
                    break;
            }

            // Rotate the fan blades based on the current speed
            transform.RotateAround(target.position, Vector3.up, currentSpeed * Time.deltaTime);
        }
    }

    // Method to start spinning up the fan
    public void StartSpinUp()
    {
        currentState = FanState.SpinningUp;
    }

    // Method to slow down the fan
    public void StartSlowDown()
    {
        currentState = FanState.SlowingDown;
    }

    // Method to instantly stop the fan
    public void StopFan()
    {
        currentState = FanState.Stopped;
        currentSpeed = 0f;
    }

    // Spin up the fan blades to max speed
    private void SpinUp()
    {
        currentSpeed += acceleration * Time.deltaTime;
        if (currentSpeed >= maxSpeed)
        {
            currentSpeed = maxSpeed;
            currentState = FanState.ConstantSpeed;
        }
    }

    // Maintain constant speed
    private void MaintainSpeed()
    {
        currentSpeed = maxSpeed;
    }

    // Slow down the fan blades to a stop
    private void SlowDown()
    {
        currentSpeed -= deceleration * Time.deltaTime;
        if (currentSpeed <= 0f)
        {
            currentSpeed = 0f;
            currentState = FanState.Stopped;
        }
    }
}