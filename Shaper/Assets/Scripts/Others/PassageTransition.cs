using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PassageTransition : MonoBehaviour
{
    [SerializeField] private int sceneIndex; // Index that will load the scene
    [SerializeField] private float loadCooldown; // The delay value until the scene is loaded

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(LoadScene());
        collision.gameObject.SetActive(false);

    }


    // Delay scene loading by a fixed cooldown value
    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(loadCooldown);
        SceneManager.LoadScene(sceneIndex);
    }
}
