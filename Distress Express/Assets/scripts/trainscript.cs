using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class trainscript : MonoBehaviour
{

    public Vector3 InitialJerk;
    public Vector3 InitialGravityAcceleration;

    motiontransformer CurrentRail = null;
    DebugLoadScene SceneManager;
    Vector3 CurrentJerk;
    Vector3 CurrentGravityAcceleration;
    Vector3 CurrentAcceleration;
    Vector3 CurrentVelocity;
    int TouchingRailsCount = 0;//how many rails is the train currently touching? //will be 2 at the most

    float TimeSinceCurrentJerkStarted = 0;
    float CurrentJerkDuration = 0;
    public float TrainHeightOverRail = 0;
    BoxCollider TrainCollider;
    public LayerMask CollisionCheckTargetLayer;
    public float RestartAfterStuckWaitTime = 2.0f;
    bool TrainIsStuck = false;
    float TrainStuckTimer = 0.0f; 
    // Start is called before the first frame update
    void Start()
    {
        CurrentGravityAcceleration = InitialGravityAcceleration;
        SetJerk(InitialJerk, 0.5f);
        CurrentVelocity = new Vector3(0, 0, 0);
        TrainCollider = GetComponent<BoxCollider>();
        SceneManager = FindAnyObjectByType<DebugLoadScene>();
    }

    public void IncrementTouchingRailCount()
    {
        TouchingRailsCount++;
    }

    public void DecrementTouchingRailCount()
    {
        TouchingRailsCount--;
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldTrainBeUnconstrained())
        {
            //UnityEngine.Debug.Log("train currently derailed");
            SetCurrentRail(null);
        }
        Vector3 NetAccleration = CurrentGravityAcceleration;
        if (TimeSinceCurrentJerkStarted < CurrentJerkDuration)
        {
            CurrentAcceleration += (CurrentJerk * (Time.deltaTime));
            TimeSinceCurrentJerkStarted += Time.deltaTime;
            NetAccleration += CurrentAcceleration; //abit of trickery here: we're only accelerating if we're also jerking. this way we're not stuck with constant acceleration.
        }

        CurrentVelocity += NetAccleration * (Time.deltaTime);

        if (CurrentRail)//constain velocity onto rail direction
        {
            switch (CurrentRail.ThisTileType)
            {
                case TileType.Linear:
                    float VelocityInConstraintDir = Vector3.Dot(CurrentRail.GetConstraintForwardDirection(), CurrentVelocity);
                    CurrentVelocity = VelocityInConstraintDir * CurrentRail.GetConstraintForwardDirection();
                    break;

                case TileType.Radial:
                    Vector3 UnitVecCircleCenterToTrain = Vector3.Normalize(transform.position - CurrentRail.GetCircleCenter());
                    float VelocityInRadiusDir  = Vector3.Dot(UnitVecCircleCenterToTrain, CurrentVelocity);
                    CurrentVelocity -= (VelocityInRadiusDir * UnitVecCircleCenterToTrain);
                    float VelocityInNormalDir = Vector3.Dot(CurrentRail.GetConstraintNormalDirection(), CurrentVelocity);
                    CurrentVelocity -= (VelocityInNormalDir * CurrentRail.GetConstraintNormalDirection());
                    break;
            }


        }


        transform.position += CurrentVelocity * (Time.deltaTime);

        if (CurrentRail)//constrain position onto rail
        {
            switch (CurrentRail.ThisTileType)
            {
                case TileType.Linear:
                    //we have to do a "closest point on the ray to the object" calculation here. the "ray" is the rail, and the object is the train.
                    //we're then going to move the train to that point found on the ray
                    Vector3 RayPoint = CurrentRail.gameObject.transform.position;//the point can be on point on the ray, and the ray here is the rail
                    Vector3 RayDirectionUnitVec = CurrentRail.GetConstraintForwardDirection();
                    Vector3 RayPointToObject = transform.position - RayPoint;
                    float RayPointToObject_ProjectedOntoRay = Vector3.Dot(RayPointToObject, RayDirectionUnitVec);
                    transform.position = RayPoint + (RayPointToObject_ProjectedOntoRay * RayDirectionUnitVec);//set train to closest point on ray
                    break;

                case TileType.Radial:
                    Vector3 SphereCenterToTrain = Vector3.Normalize(transform.position - CurrentRail.GetCircleCenter());
                    Vector3 Flattened_SphereCenterToTrain = Vector3.ProjectOnPlane(SphereCenterToTrain, CurrentRail.GetConstraintNormalDirection());
                    transform.position = CurrentRail.GetCircleCenter() + (CurrentRail.GetCircleRadius() * Vector3.Normalize(Flattened_SphereCenterToTrain));
                    break;
            }


            //now that the train is on the rail, give it the height offset:
            transform.position += CurrentRail.GetConstraintNormalDirection() * TrainHeightOverRail;

            UnityEngine.Debug.Log("Train velocity is " + CurrentVelocity.magnitude);
        }




        float almost_zero = 1E-3f;// 1 * 10 ^ -3
        if (CurrentRail && CurrentVelocity.sqrMagnitude <= almost_zero) 
        {
            if (!TrainIsStuck)
            {
                TrainIsStuck = true;
                TrainStuckTimer = 0.0f;
            }
            transform.forward = CurrentRail.GetConstraintForwardDirection();
        }
        else 
        {
            TrainIsStuck = false;
            transform.forward = Vector3.Normalize(CurrentVelocity);
        }

        if (TrainIsStuck)
        {
            TrainStuckTimer += Time.deltaTime;
            if (TrainStuckTimer >= RestartAfterStuckWaitTime)
            {
                SceneManager.ReloadScene();
            }
        }

    }

    bool ShouldTrainBeUnconstrained()
    {
        /*Collider[] hitColliders = Physics.OverlapBox(TrainCollider.bounds.center, TrainCollider.bounds.extents, Quaternion.identity, CollisionCheckTargetLayer, QueryTriggerInteraction.Collide);

        foreach (Collider col in hitColliders)
        {
            if (col.GetComponent<motiontransformer>()!= null)
            {
                return false;//a rail is touching this train
            }
        }*/
        if (TouchingRailsCount == 0)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    public void SetJerk(Vector3 val, float duration)
    {
        TimeSinceCurrentJerkStarted = 0.0f;
        CurrentJerk = val;
        CurrentJerkDuration = duration;
    }

    public Vector3 GetCurrentVelocity()
    {
        return CurrentVelocity;
    }

    public void SetCurrentRail(motiontransformer rail)
    {
        CurrentRail = rail;
    }

    public bool IsTrainOnThisRail(motiontransformer rail)
    {
        return CurrentRail == rail;
    }
}
