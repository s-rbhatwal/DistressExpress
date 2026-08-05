using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public enum TileType
{
    Linear,
    Radial
}
public class motiontransformer : MonoBehaviour
{
    public bool DebugPrintCollisionInfo = false;
    public TileType ThisTileType;
    Vector3 ConstraintDirection;
    Vector3 ConstraintNormalDirection;

    public bool DebugDisplaysOn;
    [HideInInspector]
    public GameObject DebugRailActiveIndication;
    public LayerMask CollisionCheckTargetLayer;

    BoxCollider BoxRailCollider;
    SphereCollider SphereRailCollider;
    trainscript TrainTouchingThisRail;


    // Start is called before the first frame update
    void Start()
    {
        BoxRailCollider = GetComponent<BoxCollider>();//linear rail
        SphereRailCollider = GetComponent<SphereCollider>();//quarter circle rail
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
            if (TrainTouchingThisRail != null)
            {
                DebugRailActiveIndication.SetActive(true);
            }
            else
            {
                DebugRailActiveIndication.SetActive(false);

            }
            /*if (DebugDisplayCurrentTrainDirectionOnThisRail != Vector3.zero)
            {
                DebugRailActiveIndication.transform.forward = DebugDisplayCurrentTrainDirectionOnThisRail;
            }*/
        }
        RailCheckTrainEntryExit();
    }

    public Vector3 GetCircleCenter()
    {
        if (ThisTileType == TileType.Radial)
        {
            return transform.position;
        }
        return Vector3.zero;
    }

    trainscript RailCheckTrainFound()
    {
        trainscript ReturnFoundTrain = null;
        switch (ThisTileType)
        {
            case (TileType.Linear):
                Collider[]  CollidersHittingBox = Physics.OverlapBox(BoxRailCollider.bounds.center, BoxRailCollider.bounds.extents, Quaternion.identity, CollisionCheckTargetLayer, QueryTriggerInteraction.Collide);

                foreach (Collider col in CollidersHittingBox)
                {
                    trainscript FoundTrain = col.GetComponent<trainscript>();
                    if (FoundTrain != null)
                    {
                        if (DebugPrintCollisionInfo) 
                        {
                            //UnityEngine.Debug.Log("Train colliding with this rail"); 
                        }
                        //the dot product check stops the train from entering from under the rail
                        Vector3 RailToTrain = FoundTrain.transform.position - transform.position;
                        if (Vector3.Dot(RailToTrain, ConstraintNormalDirection) >= 0)
                        {
                            if (DebugPrintCollisionInfo)
                            {
                               // UnityEngine.Debug.Log("Position check passed");
                            }
                            ReturnFoundTrain = col.gameObject.GetComponent<trainscript>();
                        }
                        break;
                    }
                }
                break;

            case (TileType.Radial):
                Collider[] CollidersHittingSphere = Physics.OverlapSphere(SphereRailCollider.center, SphereRailCollider.radius);
                foreach (Collider col in CollidersHittingSphere)
                {
                    trainscript FoundTrain = col.GetComponent<trainscript>();
                    if (FoundTrain != null)
                    {
                        if (DebugPrintCollisionInfo)
                        {
                            //UnityEngine.Debug.Log("Train colliding with this rail"); 
                        }
                        ReturnFoundTrain = col.gameObject.GetComponent<trainscript>();
                        break;
                    }
                }
                break;
        }


        return ReturnFoundTrain;
    }
    private void RailCheckTrainEntryExit()
    {
        trainscript FoundTrain = RailCheckTrainFound();
        ///////////////////////////
        if (FoundTrain != null && TrainTouchingThisRail == null)// On Train Enter
        {
            if (DebugPrintCollisionInfo)
            {
                UnityEngine.Debug.Log("entered rail");
            }
            FoundTrain.IncrementTouchingRailCount();
            TrainTouchingThisRail = FoundTrain;
            FoundTrain.SetCurrentRail(this);
        }
        else if (FoundTrain == null && TrainTouchingThisRail != null)//On Train Exit:
        {
            if (DebugPrintCollisionInfo)
            {
                UnityEngine.Debug.Log("exited rail");
            }
            TrainTouchingThisRail.DecrementTouchingRailCount();
            TrainTouchingThisRail = null;
            //dont set current rail to null, because that's the trains job, and it should only happen when the train's TouchingRailCount = 0
        }

    }


}
