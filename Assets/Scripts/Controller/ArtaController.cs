using System.Collections;
using UnityEngine;

namespace Controller
{
    public class ArtaController : MonoBehaviour
    {
        [Header("Target Settings")]
        public string targetGroupTag;
        
        [Header("Bullet Settings")]
        public BulletController bulletObject;
        public Vector3 bulletSpawnPoint;
        [Range(10.0f, 2000.0f)]
        public float bulletSpeed;
        [Range(0.1f, 10f)]
        public float spawnDelay;

        private Coroutine _spawnBulletCoroutine;

        private void Start()
        {
            _spawnBulletCoroutine = StartCoroutine(SpawnBullet());
        }

        private void OnDisable()
        {
            if (_spawnBulletCoroutine == null) return;
            
            StopCoroutine(_spawnBulletCoroutine);
            _spawnBulletCoroutine = null;
        }
        
        private IEnumerator SpawnBullet()
        {
            while (true)
            {
                var prefabInstance = GameObject.FindGameObjectWithTag(targetGroupTag);

                if (prefabInstance != null)
                    ShootBullet(prefabInstance);

                yield return new WaitForSeconds(spawnDelay);
            }
        }
        
        private void ShootBullet(GameObject target)
        {
            var spawnPoint = transform.position + bulletSpawnPoint;
            var position = target.GetComponent<Collider>().bounds.center;
            var toTarget = position - spawnPoint ;
            
            var timeToTarget = toTarget.magnitude / bulletSpeed;
            
            var targetPosition = position + target.GetComponent<Rigidbody>().velocity * timeToTarget;
            
            var displacement = targetPosition - spawnPoint;
            
            var bullet = Instantiate(bulletObject, spawnPoint, Quaternion.identity);
            var bulletRb = bullet.GetComponent<Rigidbody>();
            
            var velocity = Vector3.Normalize(displacement) * bulletSpeed;
            bulletRb.velocity = velocity;
            
            var bulletRotation = Quaternion.LookRotation(velocity);
            bullet.transform.rotation = bulletRotation;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + bulletSpawnPoint, 0.4f);
        }
    }
}
