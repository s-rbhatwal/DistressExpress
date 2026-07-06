using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trainscript : MonoBehaviour
{

    public Vector3 InitialAcceleration;
    public Vector3 InitialGravityAcceleration;
    public bool StartWithDirectionalConstraintOn = true;

    Vector3 CurrentGravityAcceleration;
    Vector3 CurrentAcceleration;
    Vector3 CurrentVelocity;

    Vector3 CurrentDirectionalConstraintUnitVec;
    bool DirectionalConstaintOn = false;

    float AccelerationTimer = 0;
    float CurrentAccelerationDuration = 0;
    // Start is called before the first frame update
    void Start()
    {
        CurrentAcceleration = InitialAcceleration;
        CurrentGravityAcceleration = InitialGravityAcceleration;
        CurrentAccelerationDuration = 0.5f;
        CurrentDirectionalConstraintUnitVec = Vector3.Normalize(InitialAcceleration);
        CurrentVelocity = new Vector3(0, 0, 0);
        DirectionalConstaintOn = StartWithDirectionalConstraintOn;
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

        if (DirectionalConstaintOn)
        {
            float VelocityInConstraintDir = Vector3.Dot(CurrentDirectionalConstraintUnitVec, CurrentVelocity);
            CurrentVelocity = VelocityInConstraintDir * CurrentDirectionalConstraintUnitVec;
        }

        transform.position += CurrentVelocity * (Time.deltaTime);
    }

    public void SetDirectionalConstraint(Vector3 val)
    {
        DirectionalConstaintOn = true;
        CurrentDirectionalConstraintUnitVec = Vector3.Normalize(val);
    }

    public void TurnOffDirectionalConstraint()
    {
        DirectionalConstaintOn = false;
    }

    public void SetAcceleration(Vector3 val, float accel_duration)
    {
        CurrentAcceleration = val;
        CurrentAccelerationDuration = accel_duration;
    }

}
