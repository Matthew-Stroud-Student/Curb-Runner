//CREDITS: Pretty Fly Games on YouTube

using NUnit.Framework.Internal.Commands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopDownCarController : MonoBehaviour
{
    [Header("Car settings")]
    public float accelerationFactor = 1.0f;
    public float reverseFactor = 1.0f;
    public float maxSpeed = 20;
    public float initialDrag = 1.0f;
    public float dragFactor = 1.0f;
    public float turnFactor = 3.5f;
    public float driftFactor = 0.95f;
    public float minSpeedBeforeAllowTurning = 3f;

    public Text speedometer;

    //Local variables
    float accelerationInput = 0;
    float steeringInput = 0;

    float rotationAngle = 0;

    float velocityVsUp = 0;

    //Components
    Rigidbody2D carRigidbody2D;
    CarSurfaceHandler carSurfaceHandler;

    //Awake is called when the script instance is being loaded.
    private void Awake()
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
        carSurfaceHandler = GetComponent<CarSurfaceHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rotationAngle = transform.rotation.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Frame-rate independent for physics calculations
    void FixedUpdate()
    {
        ApplyEngineForce();

        KillOrthogonalVelocity();

        ApplySteering();
    }

    void ApplyEngineForce()
    {
        //Calculate how much "forward" we are going in terms of the direction of our velocity
        velocityVsUp = Vector2.Dot(transform.up, carRigidbody2D.velocity);
        speedometer.text = "MPH: " + ((int)((velocityVsUp * 36) / 1.609)).ToString();

        //Limit so we cannot go faster than the max speed in the "forward" direction
        if (velocityVsUp > maxSpeed && accelerationInput > 0)
            return;

        //Limit so we cannot go faster than the 10% of max speed in the "reverse" direction
        if (velocityVsUp < -maxSpeed * 0.1f && accelerationInput < 0)
            return;

        //Limit so we cannot go faster in any direction while acceleration
        if (carRigidbody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
            return;

        //Apply drag depending on speed
        carRigidbody2D.drag = ((Mathf.Abs(velocityVsUp) * (((Mathf.Abs(steeringInput) * 2) * turnFactor) + 1)) * dragFactor + initialDrag);

        //Apply more drag depending on surface
        switch (GetSurface())
        {
            case Surface.SurfaceTypes.Dirt:
                carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 2.0f, Time.fixedDeltaTime * 3);
                break;

            case Surface.SurfaceTypes.Gravel:
                carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 8.0f, Time.fixedDeltaTime * 3);
                break;

            case Surface.SurfaceTypes.Grass:
                carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 5.0f, Time.fixedDeltaTime * 3);
                break;
        }

        //Create a force for the engine
        float forceFactor = accelerationInput > 0 ? accelerationFactor : reverseFactor;
        Vector2 engineForceVector = transform.up * accelerationInput * forceFactor;

        //Apply force and pushes the car forward
        carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        //Limit the cars ability to turn when moving slowly
        float minSpeedBeforeAllowTurningFactor = (carRigidbody2D.velocity.magnitude / minSpeedBeforeAllowTurning);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);

        float currentTurnFactor = turnFactor;

        //Apply more turning depending on surface
        switch (GetSurface())
        {
            case Surface.SurfaceTypes.Dirt:
                currentTurnFactor = 0.65f;
                break;

            case Surface.SurfaceTypes.Gravel:
                currentTurnFactor = 0.45f;
                break;

            case Surface.SurfaceTypes.Grass:
                currentTurnFactor = 0.55f;
                break;
        }

        //Update the rotaion angle based on input
        rotationAngle -= steeringInput * currentTurnFactor * minSpeedBeforeAllowTurningFactor;

        //Apply steering by rotation the car object
        carRigidbody2D.MoveRotation(rotationAngle);
    }

    void KillOrthogonalVelocity()
    {
        //Get forward and right velocity of the car
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

        float currentDriftFactor = driftFactor;

        //Apply more drift depending on surface
        switch (GetSurface())
        {
            case Surface.SurfaceTypes.Dirt:
                currentDriftFactor = 0.60f;
                break;

            case Surface.SurfaceTypes.Gravel:
                currentDriftFactor = 1.00f;
                break;

            case Surface.SurfaceTypes.Grass:
                currentDriftFactor = 0.98f;
                break;
        }

        //Kill the orthogonal velocity (side velocity) based on how much the car should drift.
        carRigidbody2D.velocity = forwardVelocity + rightVelocity * currentDriftFactor;
    }

    float GetLateralVelocity()
    {
        //Returns how fast the car is moving sideways.
        return Vector2.Dot(transform.right, carRigidbody2D.velocity);
    }

    public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;

        //Check if we are moving forward and if the player is hitting the brakes. In that case the tires should screech.
        if (accelerationInput < 0 && velocityVsUp > 0 && Mathf.Abs(GetLateralVelocity()) > 0.15f)
        {
            isBraking = true;
            return true;
        }

        //If we have alot of side movement then the tires should be screeching
        if (Mathf.Abs(GetLateralVelocity()) > 0.2f)
            return true;

        return false;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    public float GetVelocityMagnitude()
    {
        return carRigidbody2D.velocity.magnitude;
    }

    public Surface.SurfaceTypes GetSurface()
    {
        return carSurfaceHandler.GetCurrentSurface();
    }
}
