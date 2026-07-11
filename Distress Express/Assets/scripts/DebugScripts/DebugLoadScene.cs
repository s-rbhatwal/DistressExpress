using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugLoadScene : MonoBehaviour
{
    public List<KeyCode> SceneSettingsKey;
    public List<String> SceneSettingsName;

    private trainscript script = null;

	private void Start()
	{
        script = FindAnyObjectByType<trainscript>();
        if(script != null)
        {
			script.enabled = false;
		}
    }

	// Update is called once per frame
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			script.enabled = true;
		}

		for (int i = 0; i < SceneSettingsKey.Count; i++)
        {
            if (Input.GetKeyDown(SceneSettingsKey[i]))
            {
                SceneManager.LoadScene(SceneSettingsName[i]);
            }
        }
    }
}
