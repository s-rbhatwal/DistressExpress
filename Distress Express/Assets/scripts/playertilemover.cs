using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playertilemover : MonoBehaviour
{
    float TileSize = 5.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float HorizStep = 0.0f;
        float VertStep = 0.0f;

        if (Input.GetKeyUp("right") || Input.GetKeyUp("d"))
        {
            HorizStep = 1.0f;
        }
        else if (Input.GetKeyUp("left") || Input.GetKeyUp("a"))
        {
            HorizStep = -1.0f;
        }

        if (Input.GetKeyUp("up") || Input.GetKeyUp("w"))
        {
            VertStep = 1.0f;
        }
        else if (Input.GetKeyUp("down") || Input.GetKeyUp("s"))
        {
            VertStep = -1.0f;
        }

        Vector3 Step = new Vector3(HorizStep, 0, VertStep);
        transform.Translate(Step * TileSize);
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("toooch");
    }
}
