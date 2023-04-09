using System;
using UnityEngine;
using CustomInterfaces;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        // Portal interaction
        [SerializeField] private PortalController portalObj;
        [SerializeField] private bool isDead = false;

        public bool Dead
        {
            get { return isDead; }
            set { isDead = value; }
        }

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

            // Checking if the object has the damageable (Enemy) Interface
            var damageable = collision.gameObject.GetComponent<EnemyInterfaces.IDamageable>();

            // If it has, than executes the interface
            if (damageable != null) { damageable.OnHit(Dead = true); }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (portalObj != null) portalObj = null;

        }
    }
}