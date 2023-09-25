using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    public class AirplaneController : MonoBehaviour
    {
        [Header("Airplane Settings")]
        public Vector3 targetPoint;
        [Range(10.0f, 1000.0f)]
        public float moveSpeed;
        public AudioClip[] randomSound;

        private Rigidbody _rb;
        private AudioSource _audioSource;
        
        private Vector3 _realTargetPoint;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
            
            _realTargetPoint = new Vector3(
                Random.Range(targetPoint.x - 30f, targetPoint.x + 30f),
                Random.Range(targetPoint.y - 30f, targetPoint.y + 30f),
                Random.Range(targetPoint.z - 30f, targetPoint.z + 30f)
            );
            
            if (randomSound.Length > 0)
            {
                var randomIndex = Random.Range(0, randomSound.Length);
                _audioSource.clip = randomSound[randomIndex];
                _audioSource.Play();
            }
        }

        private void Update()
        {
            var position = transform.position;
            var direction = (targetPoint - position).normalized;

            _rb.velocity = direction * moveSpeed;

            var distanceToTarget = Vector3.Distance(position, _realTargetPoint);
            if (distanceToTarget < 0.1f)
            {
                _rb.velocity = Vector3.zero;
            }
        }
    }
}
