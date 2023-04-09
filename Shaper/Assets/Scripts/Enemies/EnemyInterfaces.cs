using UnityEngine;

namespace CustomInterfaces
{
    public class EnemyInterfaces : MonoBehaviour
    {
        public interface IDamageable
        {
            void OnHit(bool condition);
        }
    }
}