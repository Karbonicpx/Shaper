using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Management
{
    public class GameManager : MonoBehaviour
    {
        private PlayerInteraction player;
        private float loadDelay = 2f;

        private void Awake()
        {
            player = FindObjectOfType<PlayerInteraction>();
        }


        // When player dies, invokes the reload scene
        private void Update()
        {
            if (player.Dead == true) { Invoke("ReloadScene", loadDelay); }
        }


        // Method to reload scene
        private void ReloadScene()
        {
            var currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}