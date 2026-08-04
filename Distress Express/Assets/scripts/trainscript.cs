using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trainscript : MonoBehaviour
{

    public Vector3 InitialAcceleration;
    public Vector3 InitialGravityAcceleration;
    motiontransformer CurrentRail = null;
    DebugLoadScene SceneManager;
    Vector3 CurrentGravityAcceleration;
    Vector3 CurrentAcceleration;
    Vector3 CurrentVelocity;

    Vector3 CurrentDirectionalConstraintUnitVec;

    float AccelerationTimer = 0;
    float CurrentAccelerationDuration = 0;
    public float TrainHeightOverRail = 0;
    BoxCollider TrainCollider;
    public LayerMask CollisionCheckTargetLayer;
    public float RestartAfterStuckWaitTime = 2.0f;
    bool TrainIsStuck = false;
    float TrainStuckTimer = 0.0f; 
    // Start is called before the first frame update
    void Start()
    {
        CurrentAcceleration = InitialAcceleration;
        CurrentGravityAcceleration = InitialGravityAcceleration;
        CurrentAccelerationDuration = 0.5f;
        CurrentDirectionalConstraintUnitVec = Vector3.Normalize(InitialAcceleration);
        CurrentVelocity = new Vector3(0, 0, 0);
        TrainCollider = GetComponent<BoxCollider>();
        SceneManager = FindAnyObjectByType<DebugLoadScene>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldTrainBeUnconstrained())
        {
            SetCurrentRail(null);
        }

        Vector3 NetAccleration = CurrentGravityAcceleration;
        if (AccelerationTimer < CurrentAccelerationDuration)
        {
            NetAccleration += CurrentAcceleration;
            AccelerationTimer += Time.deltaTime;
        }
        CurrentVelocity += NetAccleration * (Time.deltaTime);

        if (CurrentRail)
        {
            switch (CurrentRail.ThisTileType)
            {
                case TileType.Linear:
                    SetDirectionalConstraint(CurrentRail.GetConstraintDirection());
                    break;

                case TileType.Radial:
                    break;
            }

            float VelocityInConstraintDir = Vector3.Dot(CurrentDirectionalConstraintUnitVec, CurrentVelocity);
            CurrentVelocity = VelocityInConstraintDir * CurrentDirectionalConstraintUnitVec;
        }


        transform.position += CurrentVelocity * (Time.deltaTime);

        if (CurrentRail)//constrain position <- check for perp issue
        {
            //we have to do a "closest point on the ray to the object" calculation here. the "ray" is the rail, and the object is the train.
            //we're then going to move the train to that point found on the ray
            Vector3 RayPoint = CurrentRail.gameObject.transform.position;//the point can be on point on the ray, and the ray here is the rail
            Vector3 RayDirectionUnitVec = CurrentRail.GetConstraintDirection();
            Vector3 RayPointToObject = transform.position - RayPoint;
            float RayPointToObject_ProjectedOntoRay = Vector3.Dot(RayPointToObject, RayDirectionUnitVec);
            transform.position = RayPoint + (RayPointToObject_ProjectedOntoRay * RayDirectionUnitVec);//set train to closest point on ray

            //now that the train is on the rail, give it the height offset:
            transform.position += CurrentRail.GetConstraintNormalDirection() * TrainHeightOverRail;
        }

        float almost_zero = 1E-3f;// 1 * 10 ^ -3
        if (CurrentRail && CurrentVelocity.sqrMagnitude <= almost_zero) 
        {
            if (!TrainIsStuck)
            {
                TrainIsStuck = true;
                TrainStuckTimer = 0.0f;
            }
            transform.forward = CurrentRail.GetConstraintDirection();
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
        Collider[] hitColliders = Physics.OverlapBox(TrainCollider.bounds.center, TrainCollider.bounds.extents, Quaternion.identity, CollisionCheckTargetLayer, QueryTriggerInteraction.Collide);

        foreach (Collider col in hitColliders)
        {
            if (col.GetComponent<motiontransformer>()!= null)
            {
                return false;//a rail is touching this train
            }
        }
        return true;
    }

    void SetDirectionalConstraint(Vector3 val)
    {
        CurrentDirectionalConstraintUnitVec = Vector3.Normalize(val);
    }

    public void SetAcceleration(Vector3 val, float accel_duration)
    {
        CurrentAcceleration = val;
        CurrentAccelerationDuration = accel_duration;
    }

    public Vector3 GetCurrentVelocity()
    {
        return CurrentVelocity;
    }

    public void SetCurrentRail(motiontransformer rail)
    {
        if (CurrentRail)
        {
            CurrentRail.TrainOnThisRail = false;
        }

        CurrentRail = rail;
        if (rail)
        {
            rail.TrainOnThisRail = true;
        }
    }

    public bool IsTrainOnThisRail(motiontransformer rail)
    {
        return CurrentRail == rail;
    }
}
