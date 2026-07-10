using UnityEngine;
using UnityEngine.SceneManagement;

public class TempWinTrigger : MonoBehaviour
{
    public string SceneName = string.Empty;

	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			SceneManager.LoadScene(SceneName);
		}
	}
}
