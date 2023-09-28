using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.Bullet
{
    public abstract class BulletBase: MonoBehaviour
    {
        [Header("Target Settings")]
        public string targetTag;
        
        [Header("Bullet Settings")]
        public bool showTrail = false;
        [Range(10.0f, 100.0f)]
        public int lineSegments = 100;
        [Range(0.1f, 1.0f)]
        public float lineWidth = 0.4f;
        [Range(10.0f, 60.0f)]
        public float despawnTime = 15.0f;
        
        
        private LineRenderer _lineRenderer;
        
        private List<Vector3> _linePositions;

        public abstract void BeforeDestroy();

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            
            _lineRenderer.positionCount = lineSegments;
            _lineRenderer.startWidth = lineWidth;
            _lineRenderer.endWidth = lineWidth;

            _linePositions = new List<Vector3>();
            
            StartCoroutine(DespawnAfterDelay());
        }

        private void FixedUpdate()
        {
            if (!showTrail) return;
            _linePositions.Add(transform.position);

            if (_linePositions.Count > lineSegments)
            {
                _linePositions.RemoveAt(0);
            }

            _lineRenderer.positionCount = _linePositions.Count;
            _lineRenderer.SetPositions(_linePositions.ToArray());
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag(targetTag)) return;
            
            BeforeDestroy();
            
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }

        private IEnumerator DespawnAfterDelay()
        {
            yield return new WaitForSeconds(despawnTime);

            Destroy(gameObject);
        }
    }
}