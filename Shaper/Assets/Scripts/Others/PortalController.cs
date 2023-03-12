using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    [SerializeField] private int levelIndex; // Index that will load the scene
    public int Level
    {
        get { return levelIndex; }
    }

    private float loadCooldown = 2f; // The delay value until the scene is loaded


    // Delay scene loading by a fixed cooldown value
    public IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(loadCooldown);
        SceneManager.LoadScene(levelIndex);
    }
}
