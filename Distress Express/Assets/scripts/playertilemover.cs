using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum TileMoverState
{
    TileUnselected,
    TileSelected
}
public class playertilemover : MonoBehaviour
{
    public int TileSize = 5;
    selectabletile FocusedTile;
    TileMoverState State;

    public LayerMask CollisionCheckTargetLayer;
    BoxCollider TileMoverCollider;
    // Start is called before the first frame update
    void Start()
    {
        TileMoverCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        float HorizStep = 0.0f;
        float VertStep = 0.0f;

        if (Input.GetKeyUp("space") && FocusedTile != null)
        {

            if (State != TileMoverState.TileSelected)//not yet selected
            {
                FocusedTile.SelectTile();
                State = TileMoverState.TileSelected;
            }
            else //already selected
            {
                FocusedTile.PutTileBackDown();
                State = TileMoverState.TileUnselected;
            }
        }

        if (Input.GetKeyDown("right") || Input.GetKeyDown("d"))
        {
            HorizStep = 1.0f;
        }
        else if (Input.GetKeyDown("left") || Input.GetKeyDown("a"))
        {
            HorizStep = -1.0f;
        }

        if (Input.GetKeyDown("up") || Input.GetKeyDown("w"))
        {
            VertStep = 1.0f;
        }
        else if (Input.GetKeyDown("down") || Input.GetKeyDown("s"))
        {
            VertStep = -1.0f;
        }

        Vector3 PotentialFuturePosition = transform.position +( new Vector3(HorizStep, 0, VertStep) * (float)(TileSize));

        Vector3 FinalPosition = UpdateSelectedTile(PotentialFuturePosition);

        transform.position = FinalPosition;
        if (State == TileMoverState.TileSelected)
        {
            FocusedTile.transform.position = FinalPosition;
            if (Input.GetKeyDown("q"))
            {
                FocusedTile.transform.Rotate(0, 90, 0);
            }
            if (Input.GetKeyDown("e"))
            {
                FocusedTile.transform.Rotate(0, -90, 0);
            }
        }
    }

    Vector3 UpdateSelectedTile(Vector3 PotentialFuturePosition)
    {
        Collider[] hitColliders = Physics.OverlapBox(PotentialFuturePosition, TileMoverCollider.bounds.extents, Quaternion.identity, CollisionCheckTargetLayer, QueryTriggerInteraction.Collide);

        selectabletile FoundTile = null;
        foreach (Collider col in hitColliders) 
        {
            FoundTile = col.GetComponent<selectabletile>();
            if (FoundTile != null)
            {
                break;
            }
        }

        if (State == TileMoverState.TileSelected)
        {
            if (FoundTile)
            {
                return transform.position;//dont move to the tile's spot ! there is a tile here already! therefore,  dont use PotentialFuturePosition
            }
        }
        else 
        {
            if (FoundTile != FocusedTile)
            {
                if (FocusedTile != null)
                {
                    FocusedTile.PutTileBackDown();
                }
                FocusedTile = FoundTile;
                if (FocusedTile != null)
                {
                    FocusedTile.HoverTile();
                }
            }
        }
        return PotentialFuturePosition;

    }


}
