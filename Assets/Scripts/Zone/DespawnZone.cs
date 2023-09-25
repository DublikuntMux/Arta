using UnityEngine;

namespace Zone
{
    public class DespawnZone: MonoBehaviour
    {
        [Header("Zone Settings")]
        public string destroyTag;
        public Vector3 zoneSize;
        
        private readonly Collider[] _collidersBuffer = new Collider[10];

        private void Update()
        {
            var colliderCount = Physics.OverlapBoxNonAlloc(transform.position, zoneSize / 2, _collidersBuffer);

            for (var i = 0; i < colliderCount; i++)
            {
                if (_collidersBuffer[i].gameObject.CompareTag(destroyTag))
                {
                    Destroy(_collidersBuffer[i].gameObject);
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, zoneSize);
        }
    }
}
