using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Unity.VisualScripting;

public class TrainSpeedTracker : MonoBehaviour
{
	private List<Tuple<float,float>> m_TrainSpeeds = new List<Tuple<float, float>>();
	[SerializeField] private trainscript m_TrainScript;

	[SerializeField] private float m_TimeToRecord;

	private float m_RecordTimer;
	private float m_Time = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
		m_Time += Time.deltaTime;
		m_RecordTimer -= Time.deltaTime;

		if(m_RecordTimer <= 0.0f)
		{
			m_RecordTimer = m_TimeToRecord;
			m_TrainSpeeds.Add(new Tuple<float,float>(m_Time, m_TrainScript.GetCurrentVelocity().magnitude));
			Debug.Log("Speed: " + m_TrainScript.GetCurrentVelocity().magnitude);
		}
    }

	private void OnApplicationQuit()
	{
		PrintData();
	}

	void PrintData()
	{
		string filePath = getPath();

		//This is the writer, it writes to the filepath
		StreamWriter writer = System.IO.File.CreateText(filePath);

		//This is writing the line of the type, name, damage... etc... (I set these)
		writer.WriteLine("Initial Jerk,Initial Jerk Duration");
		writer.WriteLine(m_TrainScript.InitialJerk.magnitude.ToString
			() + "," + m_TrainScript.InitialJerkDuration.ToString());
		writer.WriteLine("Time,Speed");
		//This loops through everything in the inventory and sets the file to these.
		for (int i = 0; i < m_TrainSpeeds.Count; ++i)
		{
			writer.WriteLine(m_TrainSpeeds[i].Item1 + "," + m_TrainSpeeds[i].Item2);
		}

		//This closes the file
		writer.Close();
	}

	private string getPath()
	{
#if UNITY_EDITOR
		return Application.dataPath + "/CSV/" + "TrainSpeed.csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath+"Saved_Inventory.csv";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+"Saved_Inventory.csv";
#else
        return Application.dataPath +"/"+"Saved_Inventory.csv";
#endif
	}
}
