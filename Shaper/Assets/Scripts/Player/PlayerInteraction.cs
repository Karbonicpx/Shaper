using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    // Portal interaction
    [SerializeField] private PortalController portalObj;

    private void Update()
    {
        if (portalObj != null)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(portalObj.LoadScene());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        // When player collides with the portal, he get it´s component and loads the scene
        if (collision.gameObject.CompareTag("Portal"))
        {
            portalObj = collision.GetComponent<PortalController>();

            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (portalObj != null) portalObj = null;

    }
}
