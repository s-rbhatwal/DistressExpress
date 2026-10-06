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
    Vector3 ConstraintForwardDirection;
    Vector3 ConstraintNormalDirection;
    Vector3 ConstraintRightDirection;


    public bool DebugDisplaysOn;
    [HideInInspector]
    public GameObject DebugRailActiveIndication;
    public LayerMask CollisionCheckTargetLayer;
    public GameObject SelectableTile;
    selectabletile RailTile;

    BoxCollider BoxRailCollider;
    SphereCollider SphereRailCollider;
    trainscript TrainTouchingThisRail;

    // Start is called before the first frame update
    void Start()
    {
        BoxRailCollider = GetComponent<BoxCollider>();//linear rail
        SphereRailCollider = GetComponent<SphereCollider>();//quarter circle rail
        RailTile = SelectableTile.GetComponent<selectabletile>();
    }

    public Vector3 GetConstraintForwardDirection()
    {
        return ConstraintForwardDirection;
    }

    public Vector3 GetConstraintNormalDirection()
    {
        return ConstraintNormalDirection;
    }

    // Update is called once per frame
    void Update()
    {

        //these can change when the player rotates the tile
        ConstraintForwardDirection = transform.forward;
        ConstraintNormalDirection = transform.up;
        ConstraintRightDirection = transform.right;

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
            return SphereRailCollider.bounds.center;//should be the same as transform.position
        }
        return Vector3.zero;
    }

    public float GetCircleRadius()
    {
        if (ThisTileType == TileType.Radial)
        {
            return SphereRailCollider.bounds.extents.x;
        }
        return 0;
    }

    trainscript RailCheckTrainFound()
    {
        trainscript ReturnFoundTrain = null;
        switch (ThisTileType)
        {
            case (TileType.Linear):

                //VERY important note about the following line: 
                //BoxRailCollider.bounds.center and BoxRailCollider.bounds.extents are simply the the AXIS ALIGNED bounding box of the collider.
                //so, if the box collider is rotated instead of axis aligned, BoxRailCollider.bounds.center and BoxRailCollider.bounds.extents are actually the dimensions of tight fitting axis aligned box that _contains_ the non-axis-aligned collider
                Collider[] CollidersHittingBox = Physics.OverlapBox(BoxRailCollider.bounds.center, BoxRailCollider.bounds.extents, Quaternion.identity, CollisionCheckTargetLayer, QueryTriggerInteraction.Collide);

                if (DebugPrintCollisionInfo)
                {
                    UnityEngine.Debug.Log("Train colliding with this rail");
                }

                foreach (Collider col in CollidersHittingBox)
                {
                    trainscript FoundTrain = col.GetComponent<trainscript>();
                    if (FoundTrain != null)
                    {
                        if (DebugPrintCollisionInfo)
                        {
                            UnityEngine.Debug.Log("Train colliding with this rail");
                        }
                        //the dot product check stops the train from entering from under the rail
                        Vector3 RailToTrain = FoundTrain.transform.position - transform.position;
                        if (Vector3.Dot(RailToTrain, ConstraintNormalDirection) >= 0)
                        {
                            if (DebugPrintCollisionInfo)
                            {
                                UnityEngine.Debug.Log("Position check passed");
                            }
                            ReturnFoundTrain = col.gameObject.GetComponent<trainscript>();
                        }
                        break;
                    }
                }
                break;

            case (TileType.Radial):
                //SphereRailCollider.bounds here is the tight fitting box that _contains_ the sphere
                Collider[] CollidersHittingSphere = Physics.OverlapSphere(SphereRailCollider.bounds.center, SphereRailCollider.bounds.extents.x);
                foreach (Collider col in CollidersHittingSphere)
                {
                    trainscript FoundTrain = col.GetComponent<trainscript>();
                    if (FoundTrain != null)
                    {
                        if (DebugPrintCollisionInfo)
                        {
                            UnityEngine.Debug.Log("Train colliding with this sphere");
                        }
                        Vector3 CircleCenterToTrain = FoundTrain.transform.position - GetCircleCenter();
                        if ((Vector3.Dot(CircleCenterToTrain, GetConstraintForwardDirection()) >= 0) && (Vector3.Dot(CircleCenterToTrain, ConstraintRightDirection) >= 0))
                        {
                            ReturnFoundTrain = col.gameObject.GetComponent<trainscript>();
                            if (DebugPrintCollisionInfo)
                            {
                                UnityEngine.Debug.Log("Train colliding with this curved rail");
                            }
                        }
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
            float VelDotRail = Vector3.Dot(ConstraintForwardDirection, FoundTrain.GetCurrentVelocity());
            bool TrainMovingForward = (VelDotRail >= 0);
            bool TrainAtStandStill = (VelDotRail == 0);
            OnTrainEnterRail(TrainMovingForward, TrainAtStandStill, FoundTrain);
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

    private void OnTrainEnterRail(bool TrainMovingForward, bool TrainAtStandStill, trainscript FoundTrain)
    {
        if (TrainAtStandStill == false && Mathf.Abs(RailTile.TrainVelocityIncrease) > 0)
        {
            Vector3 TrainMoveDirection = ConstraintForwardDirection;
            if (!TrainMovingForward) //this means its moving backward
            {
                TrainMoveDirection *= -1.0f;
            }
            FoundTrain.SingleFrameAcclerateByVal(RailTile.TrainVelocityIncrease * TrainMoveDirection);
        }

    }


}
