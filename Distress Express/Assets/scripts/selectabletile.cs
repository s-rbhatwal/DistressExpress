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

    // Start is called before the first frame update
    void Start()
    {
        TileBaseRenderer = TileBaseObject.GetComponent<Renderer>();
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
