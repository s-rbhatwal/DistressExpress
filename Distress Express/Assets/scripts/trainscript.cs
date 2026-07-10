using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trainscript : MonoBehaviour
{

    public Vector3 InitialAcceleration;
    public Vector3 InitialGravityAcceleration;
    motiontransformer CurrentRail = null;

    Vector3 CurrentGravityAcceleration;
    Vector3 CurrentAcceleration;
    Vector3 CurrentVelocity;

    Vector3 CurrentDirectionalConstraintUnitVec;

    float AccelerationTimer = 0;
    float CurrentAccelerationDuration = 0;
    public float TrainHeightOverRail = 0;
    // Start is called before the first frame update
    void Start()
    {
        CurrentAcceleration = InitialAcceleration;
        CurrentGravityAcceleration = InitialGravityAcceleration;
        CurrentAccelerationDuration = 0.5f;
        CurrentDirectionalConstraintUnitVec = Vector3.Normalize(InitialAcceleration);
        CurrentVelocity = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
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

        transform.forward = Vector3.Normalize(CurrentVelocity);

        transform.position += CurrentVelocity * (Time.deltaTime);

        if (CurrentRail)//constrain position
        {
            Vector3 TrainToRailVec = CurrentRail.gameObject.transform.position - transform.position;
            Vector3 PerpVecTrainToRail = Vector3.Dot(TrainToRailVec, CurrentRail.GetConstraintNormalDirection()) * CurrentRail.GetConstraintNormalDirection();//the vector in the direction of the rail's normal from the rail to the train 
            transform.position += PerpVecTrainToRail;
            transform.position -= Vector3.Normalize(PerpVecTrainToRail) * TrainHeightOverRail;
        }
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
