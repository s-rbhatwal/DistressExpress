using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugLoadScene : MonoBehaviour
{
    public List<KeyCode> SceneSettingsKey;
    public List<String> SceneSettingsName;

    private trainscript script;

	private void Start()
	{
        script = FindAnyObjectByType<trainscript>();
        script.enabled = false;
    }

	// Update is called once per frame
	void Update()
    {
        for (int i = 0; i < SceneSettingsKey.Count; i++)
        {
            if (Input.GetKeyDown(SceneSettingsKey[i]))
            {
                SceneManager.LoadScene(SceneSettingsName[i]);
            }
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            script.enabled = true;
        }
    }
}
