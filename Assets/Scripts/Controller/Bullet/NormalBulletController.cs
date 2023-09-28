using UnityEngine;

namespace Controller.Bullet
{
    [RequireComponent(typeof(LineRenderer))]
    public class NormalBulletController: BulletBase
    {
        [Header("SFX")]
        public AudioClip[] explosionSound;
        public GameObject[] explosionParticle;
        public SfxController sfxObject;

        public override void BeforeDestroy()
        {
            if (explosionSound.Length > 0)
            {
                var sfx = Instantiate(sfxObject, gameObject.transform.position, Quaternion.identity);
                var randomSound = Random.Range(0, explosionSound.Length);
                
                sfx.audio.clip = explosionSound[randomSound];
                sfx.audio.Play();
            }

            if (explosionParticle.Length > 0)
            {
                var randomParticle = Random.Range(0, explosionParticle.Length);
                var particleSystemInstance = Instantiate(explosionParticle[randomParticle], gameObject.transform.position, Quaternion.identity);
                
                var particle = particleSystemInstance.GetComponent<ParticleSystem>();
                particle.Play();
            }
        }
    }
}