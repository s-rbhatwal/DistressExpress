using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TileType
{
    Linear,
    Radial
}
public class motiontransformer : MonoBehaviour
{
    public int DebugBreakFinalEntryCount = 0;
    int DebugBreakCurrentEntryCount = 0;
    public TileType ThisTileType;
    Vector3 ConstraintDirection;
    Vector3 ConstraintNormalDirection;

    public bool DebugDisplaysOn;
    [HideInInspector]
    public bool TrainOnThisRail = false;
    Vector3 CurrentTrainDirectionOnThisRail;
    public GameObject DebugRailActiveIndication;


    // Start is called before the first frame update
    void Start()
    {
        ConstraintDirection = transform.forward;
        ConstraintNormalDirection = transform.up;
    }

    public Vector3 GetConstraintDirection()
    {
        return ConstraintDirection;
    }

    public Vector3 GetConstraintNormalDirection()
    {
        return ConstraintNormalDirection;
    }

    // Update is called once per frame
    void Update()
    {
        if (DebugDisplaysOn)
        {
            DebugRailActiveIndication.SetActive(TrainOnThisRail);
            if (CurrentTrainDirectionOnThisRail != Vector3.zero)
            {
                DebugRailActiveIndication.transform.forward = CurrentTrainDirectionOnThisRail;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        trainscript FoundTrain = other.gameObject.GetComponent<trainscript>();
        if (FoundTrain)
        {
            TrainOnThisRail = false;
            if (FoundTrain.IsTrainOnThisRail(this))//if it has left and NOT entered a another rail, like a freefall
            {
                FoundTrain.SetCurrentRail(null);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {

        trainscript FoundTrain = other.gameObject.GetComponent<trainscript>();
        if (FoundTrain)
        {
            if (FoundTrain.IsTrainOnThisRail(this))//dont do the enter logic, we're already on this rail
            {
                return;
            }

            bool TrainEnteringThisRail = false;
            Vector3 TrainToRailVec = transform.position - FoundTrain.gameObject.transform.position;
            Vector3 TrainDirectionOnThisRail = Vector3.Dot(TrainToRailVec, ConstraintDirection) * ConstraintDirection;
            if (Vector3.Dot(TrainDirectionOnThisRail, FoundTrain.GetCurrentVelocity()) > 0)
            {
                CurrentTrainDirectionOnThisRail = Vector3.Normalize(TrainDirectionOnThisRail);
                TrainEnteringThisRail = true;
            }

            if (TrainEnteringThisRail)
            {
                FoundTrain.SetCurrentRail(this);
                if (DebugBreakFinalEntryCount > 0)
                {
                    DebugBreakCurrentEntryCount++;
                    if (DebugBreakCurrentEntryCount == DebugBreakFinalEntryCount)
                    {
                        Debug.Log("debug motion transformer reached");
                    }
                }

            }
        }
    }


}
