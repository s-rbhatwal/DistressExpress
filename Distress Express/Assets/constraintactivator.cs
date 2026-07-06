using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class constraintactivator : MonoBehaviour
{
    public GameObject RailConstraint;
    // Start is called before the first frame update
    void Start()
    {
        RailConstraint.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerExit(Collider other)
    {
        trainscript FoundTrain = other.gameObject.GetComponent<trainscript>();
        if (FoundTrain)
        {
            RailConstraint.SetActive(true);
        }
    }
}
