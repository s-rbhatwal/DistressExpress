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
    public LayerMask CollisionCheckTargetLayer;

    BoxCollider RailCollider;


    // Start is called before the first frame update
    void Start()
    {
        RailCollider = GetComponent<BoxCollider>();
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

        //these can change when the player rotates the tile
        ConstraintDirection = transform.forward;
        ConstraintNormalDirection = transform.up;

        if (DebugDisplaysOn)
        {
            DebugRailActiveIndication.SetActive(TrainOnThisRail);
            if (CurrentTrainDirectionOnThisRail != Vector3.zero)
            {
                DebugRailActiveIndication.transform.forward = CurrentTrainDirectionOnThisRail;
            }
        }
        RailCheckTrainEntryExit();
    }

    private void RailCheckTrainEntryExit()
    {
        Collider[] hitColliders = Physics.OverlapBox(RailCollider.bounds.center, RailCollider.bounds.extents , Quaternion.identity, CollisionCheckTargetLayer, QueryTriggerInteraction.Collide);
        trainscript FoundTrain = null;
        foreach (Collider col in hitColliders)
        {
            FoundTrain = col.GetComponent<trainscript>();
            if (FoundTrain != null)
            {
                FoundTrain = col.gameObject.GetComponent<trainscript>();
                break;
            }
        }

        ///////////////////////////
        if (FoundTrain != null && !TrainOnThisRail)
        {
            Vector3 TrainToRailVec = transform.position - FoundTrain.gameObject.transform.position;
            Vector3 TrainDirectionOnThisRail = Vector3.Dot(TrainToRailVec, ConstraintDirection) * ConstraintDirection;
            if (Vector3.Dot(TrainDirectionOnThisRail, FoundTrain.GetCurrentVelocity()) > 0)
            {
                CurrentTrainDirectionOnThisRail = Vector3.Normalize(TrainDirectionOnThisRail);
                TrainOnThisRail = true;//rail entered
                FoundTrain.SetCurrentRail(this);
            }
        }
        else if (FoundTrain == null && TrainOnThisRail)
        {
            TrainOnThisRail = false;
        }

    }


}
