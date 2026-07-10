using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectabletile : MonoBehaviour
{
    public GameObject TileBaseObject;
    Renderer TileBaseRenderer;
    public Material HoverMaterial;
    public Material DefaultMaterial;
    public Material SelectMaterial;
    playertilemover TileMover;

    // Start is called before the first frame update
    void Start()
    {
        TileBaseRenderer = TileBaseObject.GetComponent<Renderer>();
        TileMover = FindFirstObjectByType<playertilemover>();

        transform.position = new Vector3(RoundToNearestTileUnit(TileMover.TileSize, transform.position.x), RoundToNearestTileUnit(TileMover.TileSize, transform.position.y), RoundToNearestTileUnit(TileMover.TileSize, transform.position.z));
    }

    float RoundToNearestTileUnit(int TileUnitSize, float RawNumber)
    {
        int Division = (((int)(RawNumber)) / TileUnitSize);

        float Lower = (Division * TileUnitSize);
        float Higher = ((1 + Division) * (TileUnitSize));

        float LowerDiff = Mathf.Abs(RawNumber - Lower);
        float HigherDiff = Mathf.Abs(RawNumber - Higher);

        if (LowerDiff < HigherDiff)
        {
            return Lower;
        }
        else
        {
            return Higher;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HoverTile()
    {
        TileBaseRenderer.material = HoverMaterial;
    }

    public void PutTileBackDown()
    {
        TileBaseRenderer.material = DefaultMaterial;
    }

    public void SelectTile()
    {
        TileBaseRenderer.material = SelectMaterial;
    }
}
