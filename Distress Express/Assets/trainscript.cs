using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trainscript : MonoBehaviour
{

    public float Speed;
    public Vector3 InitialAcceleration;

    Vector3 CurrentAcceleration;
    Vector3 CurrentVelocity;

    Vector3 CurrentDirectionalConstraintUnitVec;
    bool DirectionalConstaintOn = true;

    // Start is called before the first frame update
    void Start()
    {
        CurrentAcceleration = InitialAcceleration;
        CurrentDirectionalConstraintUnitVec = Vector3.Normalize(InitialAcceleration);
        CurrentVelocity = new Vector3(0, 0, 0);
        DirectionalConstaintOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentVelocity += CurrentAcceleration * (Time.deltaTime);

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

    public void SetAcceleration(Vector3 val)
    {
        CurrentAcceleration = val;
    }

}
