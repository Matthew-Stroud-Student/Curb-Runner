using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CarAIHandler : MonoBehaviour
{
    public enum AIMode { followPlayer, followWaypoints };

    [Header("AI settings")]
    public AIMode aiMode;
    public float maxSpeed = 16;
    public bool isAvoidingCars = true;

    //Local variables
    Vector3 targetPosition = Vector3.zero;
    Transform targetTransform = null;

    //Avoidence
    Vector2 avoidenceVectorLerped = Vector3.zero;

    //Waypoints
    WaypointNode currentWaypoint = null;
    WaypointNode previousWaypoint = null;
    WaypointNode[] allWayPoints;

    //Coliders
    CapsuleCollider2D capsuleCollider2D;

    //Components
    TopDownCarController topDownCarController;

    //Awake is called when the script instance is being loaded.
    void Awake()
    {
        topDownCarController = GetComponent<TopDownCarController>();
        allWayPoints = FindObjectsOfType<WaypointNode>();

        capsuleCollider2D = GetComponentInChildren<CapsuleCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Update is called once per frame and is frame dependent
    void FixedUpdate()
    {
        Vector2 inputVector = Vector2.zero;

        switch (aiMode)
        {
            case AIMode.followPlayer:
                FollowPlayer();
                break;

            case AIMode.followWaypoints:
                FollowWaypoints();
                break;
        }

        inputVector.x = TurnTowardTarget();
        inputVector.y = ApplyThrottleOrBrake(inputVector.x);

        //Send the input tp the car controller.
        topDownCarController.SetInputVector(inputVector);
    }

    void FollowPlayer()
    {
        if (targetTransform == null)
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (targetTransform != null)
            targetPosition = targetTransform.position;
    }

    void FollowWaypoints()
    {
        //Pick the closest waypoint if we don't have a waypoint set.
        if (currentWaypoint == null)
        {
            currentWaypoint = FindClosestWayPoint();
            previousWaypoint = currentWaypoint;
        }

        //Set the target on the waypoints position
        if (currentWaypoint != null)
        {
            targetPosition = currentWaypoint.transform.position;

            //Store how close we are to the target
            float distanceToWayPoint = (targetPosition - transform.position).magnitude;

            //Navigate towards nearest point on line
            if (distanceToWayPoint > 5)
            {
                Vector3 nearestPointOnTheWayPointLine = FindNearestPointOnLine(previousWaypoint.transform.position, currentWaypoint.transform.position, transform.position);

                float segments = distanceToWayPoint / 5.0f;

                targetPosition = (targetPosition + nearestPointOnTheWayPointLine * segments) / (segments + 1);

                Debug.DrawLine(transform.position, targetPosition, Color.cyan);
            }

            //Check if we are close enoughh to consider that we have reached the waypoint
            if (distanceToWayPoint <= currentWaypoint.minDistanceToReachWaypoint)
            {
                if (currentWaypoint.maxSpeed > 0)
                    maxSpeed = currentWaypoint.maxSpeed;
                else maxSpeed = 1000;

                //Store the current waypoint as previous before we assign a new current one.
                previousWaypoint = currentWaypoint;

                //If we are close enough then follow to the next waypoint, if there are multiple waypoints then pick one at random.
                currentWaypoint = currentWaypoint.nextWaypointNode[Random.Range(0, currentWaypoint.nextWaypointNode.Length)];
            }
        }
    }

    //Find the closestwaypoint to the AI
    WaypointNode FindClosestWayPoint()
    {
        return allWayPoints
            .OrderBy(t => Vector3.Distance(transform.position, t.transform.position))
            .FirstOrDefault();
    }

    float TurnTowardTarget()
    {
        Vector2 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.Normalize();

        //Aply avoidence to steering
        if (isAvoidingCars)
            AvoidCars(vectorToTarget, out vectorToTarget);

        //Calculate an angle towards the target
        float angleToTarget = Vector2.SignedAngle(transform.up, vectorToTarget);
        angleToTarget *= -1;

        //We want the car to turn as much as possible if the angle is greater than 45 degrees and we want it to smooth out so if the angle is small we want the AI to make smaller turns
        float steerAmount = angleToTarget / 30.0f;

        //Clamp steering to between -1 and 1.
        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);

        return steerAmount;
    }

    float ApplyThrottleOrBrake(float inputX)
    {
        //if we are going too fast then do not accelerate further.
        if (topDownCarController.GetMPH() > maxSpeed)
            return -1;

        //Apply throttle forward based on how much the car wants to turn. If it's a sharp turn this will cause the car to apply less speed forward.
        float throttleAmount = 1.5f - Mathf.Abs(inputX) / 1.0f;

        //Clamp throttle to between -1 and 1.
        throttleAmount = Mathf.Clamp(throttleAmount, -1.0f, 1.0f);

        return throttleAmount;
    }

    //Finds the nearest point on a line.
    Vector2 FindNearestPointOnLine(Vector2 lineStartPosition, Vector2 lineEndPosition, Vector2 point)
    {
        //Get heading as a vector
        Vector2 lineHeadingVector = (lineEndPosition - lineStartPosition);

        //Store the max distance
        float maxDistance = lineHeadingVector.magnitude;
        lineHeadingVector.Normalize();

        //Do projection from the start position to the point
        Vector2 lineVectorStartToPoint = point - lineStartPosition;
        float dotProduct = Vector2.Dot(lineVectorStartToPoint, lineHeadingVector);

        //Clamp the dot product to maxDistance
        dotProduct = Mathf.Clamp(dotProduct, 0f, maxDistance);

        return lineStartPosition + lineHeadingVector * dotProduct;
    }

    //Checks for cars ahead of the car.
    bool IsCarsInFrontOfAICar(out Vector3 position, out Vector3 otherCarRightVector)
    {
        //Disable the cars own collider to avoid having the AI car detect itself.
        capsuleCollider2D.enabled = false;

        //Perform the circle cast in front of the car with a slight offset forward and only in the car layer
        RaycastHit2D raycastHit2d = Physics2D.CircleCast(transform.position + transform.up * 0.18f, 0.2f, transform.up, 1, 1 << LayerMask.NameToLayer("Car"));

        //Enable the colliders again so the car can collide and other cars can detect it.
        capsuleCollider2D.enabled = true;

        if (raycastHit2d.collider != null)
        {
            //Draw a red line showing how long the detection is, make it red since we have detected another car
            Debug.DrawRay(transform.position, transform.up * 1, Color.red);

            position = raycastHit2d.collider.transform.position;
            otherCarRightVector = raycastHit2d.collider.transform.right;

            return true;
        }
        else
        {
            //We didn't detect any other car so draw black line with the distance that we use to check for other cars.
            Debug.DrawRay(transform.position, transform.up * 1, Color.black);
        }

        //No car was detected but we still need assign out values so lset just return zero.
        position = Vector3.zero;
        otherCarRightVector = Vector3.zero;

        return false;
    }

    void AvoidCars(Vector2 vectorToTarget, out Vector2 newVectorToTarget)
    {
        if (IsCarsInFrontOfAICar(out Vector3 otherCarPosition, out Vector3 otherCarRightVector))
        {
            Vector2 avoidenceVector = Vector2.zero;

            //Calculate the reflecting vector if we would hit the other car.
            avoidenceVector = Vector2.Reflect((otherCarPosition - transform.position).normalized, otherCarRightVector);

            float distanceToTarget = (targetPosition - transform.position).magnitude;

            //We want to be able to control how much desure the AI has to drive towards the waypoint vs avoiding the other car.
            //As we get closer to the waypoint the desire to reach the waypoint increases.
            float driveToTargetInfluence = 6.0f / distanceToTarget;

            //Ensure that we limit the value to between 30% and 100% as we always want the AI to desire to reach the waypoint.
            driveToTargetInfluence = Mathf.Clamp(driveToTargetInfluence, 0.30f, 1.0f);

            //The desire to avoid the car is simply the inverse to reach the waypoint
            float avoidenceInfluence = 1.0f - driveToTargetInfluence;

            //Reduce jittering a little bit by using a lerp
            avoidenceVectorLerped = Vector2.Lerp(avoidenceVectorLerped, avoidenceVector, Time.fixedDeltaTime * 4);

            //Calculate a new vector to the target based on the avoidence vector and the desire to reach the waypoint
            newVectorToTarget = vectorToTarget * driveToTargetInfluence + avoidenceVectorLerped * avoidenceInfluence;
            newVectorToTarget.Normalize();

            //Draw the vector which indicates the avoidence vector in green.
            Debug.DrawRay(transform.position, avoidenceVector * 1, Color.green);

            //Draw the vector that the car will actually take in yellow.
            Debug.DrawRay(transform.position, newVectorToTarget * 1, Color.yellow);

            return;
        }

        //We need to assign a default value if we didn't hit any cars before we exit the funcion.
        newVectorToTarget = vectorToTarget;
    }
}
