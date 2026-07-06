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
    public TileType ThisTileType;
    public GameObject LinearEndPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        trainscript FoundTrain = other.gameObject.GetComponent<trainscript>();
        switch (ThisTileType)
        {
            case TileType.Linear:
                Vector3 NewDirection = LinearEndPoint.transform.position - transform.position;
                FoundTrain.SetDirectionalConstraint(NewDirection);

                break;

            case TileType.Radial:
                break;
        }
    }
}
