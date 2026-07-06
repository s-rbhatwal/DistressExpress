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
    public GameObject LinearEndPointA;
    public GameObject LinearEndPointB;
    Collider ColliderPointA;
    Collider ColliderPointB;
    // Start is called before the first frame update
    void Start()
    {
        ColliderPointA = LinearEndPointA.GetComponent<Collider>();
        ColliderPointB = LinearEndPointB.GetComponent<Collider>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (DebugBreakFinalEntryCount > 0)
        {
            if (DebugBreakCurrentEntryCount == DebugBreakFinalEntryCount)
            {
                Debug.Log("debug motion transformer reached");
            }
            DebugBreakCurrentEntryCount ++;
        }

        trainscript FoundTrain = other.gameObject.GetComponent<trainscript>();
        switch (ThisTileType)
        {
            case TileType.Linear:
                Vector3 NewDirection = LinearEndPointB.transform.position - LinearEndPointA.transform.position;
                FoundTrain.SetDirectionalConstraint(NewDirection);

                break;

            case TileType.Radial:
                break;
        }
    }
}
