using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Zone;

namespace Controller
{
    public class UIPanelController: MonoBehaviour
    {
        [Header("UI Components")]
        public Toggle showTrail;
        public Slider bulletSpeed;
        public Slider bulletSpawnDelay;
        public Slider airplaneSpeed;
        public Slider airplaneSpawnDelay;
        public Slider timeScale;
        public Toggle pauseGame;
        
        [Header("Objects for change settings")] 
        public ArtaController arta;
        public SpawnerZone airplaneSpawner;

        private float _timeScaleSave = 1.0f;
        
        private void Start()
        { 
            showTrail.onValueChanged.AddListener(delegate {
                TrailToggleChanged(showTrail);
            });
            
            bulletSpeed.onValueChanged.AddListener(delegate {
                BulletSpeedValueChanged(bulletSpeed);
            });
            
            bulletSpawnDelay.onValueChanged.AddListener(delegate {
                BulletSpawnDelayValueChanged(bulletSpawnDelay);
            });
            
            airplaneSpeed.onValueChanged.AddListener(delegate {
                AirplaneSpeedValueChanged(airplaneSpeed);
            });
            
            airplaneSpawnDelay.onValueChanged.AddListener(delegate {
                AirplaneSpawnDelayValueChanged(airplaneSpawnDelay);
            });
            
            timeScale.onValueChanged.AddListener(delegate {
                TimeScaleDelayValueChanged(timeScale);
            });
            
            pauseGame.onValueChanged.AddListener(delegate {
                PauseToggleChanged(pauseGame);
            });
        }

        private void TrailToggleChanged(Toggle toggle)
        {
            arta.bulletObject.showTrail = toggle.isOn;
        }

        private void BulletSpeedValueChanged(Slider slider)
        {
            arta.bulletSpeed = slider.value;
        }
        
        private void BulletSpawnDelayValueChanged(Slider slider)
        {
            arta.spawnDelay = slider.value;
        }

        private void AirplaneSpeedValueChanged(Slider slider)
        {
            airplaneSpawner.objectToSpawn.moveSpeed = slider.value;
        }
        
        private void AirplaneSpawnDelayValueChanged(Slider slider)
        {   
            airplaneSpawner.spawnDelay = slider.value;
        }
        
        private void TimeScaleDelayValueChanged(Slider slider)
        {
            Time.timeScale = slider.value;
            
            if(slider.value == 0.0f)
            {
                pauseGame.isOn = true;
            }
            else
            {
                pauseGame.isOn = false;
                _timeScaleSave = slider.value;
            }
        }
        
        private void PauseToggleChanged(Toggle toggle)
        {
            if (toggle.isOn)
            {
                Time.timeScale = 0.0f;
                timeScale.value = 0.0f;
            }
            else
            {
                Time.timeScale = _timeScaleSave;
            }
        }
    }
}