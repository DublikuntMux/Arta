using System.Collections;
using Controller;
using UnityEngine;

namespace Zone
{
    public class SpawnerZone : MonoBehaviour
    {
        [Header("Zone Settings")]
        public AirplaneController objectToSpawn;
        [Range(0.1f, 10f)]
        public float spawnDelay;
        public Vector3 spawnZoneSize;
        
        private Coroutine _spawnObjectCoroutine;
    
        private void Start()
        {
            _spawnObjectCoroutine = StartCoroutine(SpawnObjects());
        }
        
        private void OnDisable()
        {
            if (_spawnObjectCoroutine == null) return;
            
            StopCoroutine(_spawnObjectCoroutine);
            _spawnObjectCoroutine = null;
        }

        private IEnumerator SpawnObjects()
        {
            while (true)
            {
                var randomSpawnPosition = transform.position + new Vector3(
                    Random.Range(-spawnZoneSize.x / 2f, spawnZoneSize.x / 2f),
                    Random.Range(-spawnZoneSize.y / 2f, spawnZoneSize.y / 2f),
                    Random.Range(-spawnZoneSize.z / 2f, spawnZoneSize.z / 2f)
                );

                Instantiate(objectToSpawn, randomSpawnPosition, Quaternion.identity);

                yield return new WaitForSeconds(spawnDelay);
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, new Vector3(spawnZoneSize.x, spawnZoneSize.y, spawnZoneSize.z));
        }
    }
}
