using System.Collections;
using UnityEngine;

namespace Controller
{
    public class SfxController: MonoBehaviour
    {
        [Header("SFX objects")] 
        public AudioSource audio;
        [Range(1.0f, 5.0f)]
        public float despawnTime = 3.0f;

        private void Start()
        {
            StartCoroutine(DespawnAfterDelay());
        }
        
        private IEnumerator DespawnAfterDelay()
        {
            yield return new WaitForSeconds(despawnTime);

            Destroy(gameObject);
        }
    }
}