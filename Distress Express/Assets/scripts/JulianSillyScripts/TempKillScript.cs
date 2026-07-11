using UnityEngine;
using UnityEngine.SceneManagement;

public class TempKillScript : MonoBehaviour
{
    [SerializeField] private float m_YKillLevel = -15;
    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < m_YKillLevel)
        {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
    }
}
