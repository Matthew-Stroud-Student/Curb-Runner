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
    public float offTrackTurnFactor = 1.0f;
    public float offTrackDriftFactor = 1.0f;

    public Text speedometer;

    //Local variables
    float accelerationInput = 0;
    float steeringInput = 0;

    float rotationAngle = 0;

    float velocityVsUp = 0;

    float defaultTurnFactor = 0;
    float defaultDriftFactor = 0;

    //Components
    Rigidbody2D carRigidbody2D;
    private ParticleSystem dustParticleSystem;

    //Awake is called when the script instance is being loaded.
    private void Awake()
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultTurnFactor = turnFactor;
        defaultDriftFactor = driftFactor;

        Transform dustParticlesTransform = transform.Find("Dust");

        if(dustParticlesTransform != null )
        {
            dustParticleSystem = dustParticlesTransform.GetComponent<ParticleSystem>();

            if (dustParticleSystem == null )
            {
                Debug.LogWarning("Dust child does not have a ParticleSystem component.");
            }
        }
        else
        {
            Debug.LogWarning("Could not find child object named 'Dust'.");
        }
        
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

//        //Apply drag if there is no accelerationInput so the car stops when the player lets go of the accelerator
//        if (accelerationInput == 0)
//            carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 0.2f, Time.fixedDeltaTime * 1);
//        else carRigidbody2D.drag = 0;

        //Create a force for the engine
        float forceFactor = accelerationInput > 0 ? accelerationFactor : reverseFactor;
        Vector2 engineForceVector = transform.up * accelerationInput * forceFactor;

        //Apply force and pushes the car forward
        carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
        carRigidbody2D.drag = ((Mathf.Abs(velocityVsUp) * (((Mathf.Abs(steeringInput) * 4) * turnFactor) + 1)) * dragFactor + initialDrag) ;
    }

    void ApplySteering()
    {
        //Limit the cars ability to turn when moving slowly
        float minSpeedBeforeAllowTurningFactor = (carRigidbody2D.velocity.magnitude / minSpeedBeforeAllowTurning);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);

        //Update the rotaion angle based on input
        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;

        //Apply steering by rotation the car object
        carRigidbody2D.MoveRotation(rotationAngle);
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

        carRigidbody2D.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Off Track"))
        {
            print("Off Track");
            turnFactor = offTrackTurnFactor;
            driftFactor = offTrackDriftFactor;
            AdjustParticleEmission();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Off Track"))
        {
            // Keep adjusting the emission rate if needed
            AdjustParticleEmission();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Off Track"))
        {
            print("On Track");
            turnFactor = defaultTurnFactor;
            driftFactor = defaultDriftFactor;
            StopParticleEmission();
        }
    }

    // Method to adjust the emission rate based on velocity
    private void AdjustParticleEmission()
    {
        if (dustParticleSystem != null)
        {
            // Get the Emission module of the ParticleSystem
            var emission = dustParticleSystem.emission;

            // Set the rateOverTime based on the velocityVsUp variable (adjust as needed)
            emission.rateOverTime = velocityVsUp * 2;
        }
    }

    // Optionally, stop the particle emission when exiting offroad
    private void StopParticleEmission()
    {
        if (dustParticleSystem != null)
        {
            var emission = dustParticleSystem.emission;
            emission.rateOverTime = 0;
        }
    }
}
