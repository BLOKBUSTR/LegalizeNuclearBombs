using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

#pragma warning disable CS8618
namespace LegalizeNuclearBombs
{
    public class ValuableWarhead : MonoBehaviour
    {
        public Transform center;
        public MeshRenderer mesh;
        private Material material;
        private static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");
        public List<ParticleSystem> explosionParticles;
        public GameObject uraniumCloudPrefab;
        
        [Space]
        public PhysGrabObject physGrabObject;
        public PhotonView photonView;
        
        public Sound warningSound;
        public Sound explosionDelaySound;
        
        private float explosionStrength;
        private int playerDamage;
        private bool spawnUraniumCloud;
        private float uraniumCloudSize;
        private float uraniumCloudDuration;
        private int uraniumPlayerDamage;
        private float uraniumPlayerDamageRate;
        
        private int hitCount;
        private bool detonated;
        
        private bool explosionDelayActive;
        private bool explosionDelayImpulse = true;
        private float explosionDelayTime;
        
        private bool emissionActive;
        private bool emissionImpulse;
        private float emissionTime;
        
        private void Start()
        {
            material = mesh.material;
            
            LegalizeNuclearBombs.Debug("New nuke valuable spawned", this);
            
            if (SemiFunc.IsNotMasterClient()) return;
            
            explosionStrength = LegalizeNuclearBombs.configExplosionStrength.Value;
            playerDamage = LegalizeNuclearBombs.configPlayerDamage.Value;
            spawnUraniumCloud = LegalizeNuclearBombs.configSpawnUraniumCloud.Value;
            uraniumCloudSize = LegalizeNuclearBombs.configUraniumCloudSize.Value;
            uraniumCloudDuration = LegalizeNuclearBombs.configUraniumCloudDuration.Value;
            uraniumPlayerDamage = LegalizeNuclearBombs.configUraniumPlayerDamage.Value;
            uraniumPlayerDamageRate = LegalizeNuclearBombs.configUraniumPlayerDamageRate.Value;
            LogInitValues();
            
            if (SemiFunc.IsMultiplayer())
                StartCoroutine(LateStart());
        }
        
        private IEnumerator LateStart()
        {
            yield return new WaitForSeconds(5f);
            
            while (physGrabObject.impactDetector.indestructibleSpawnTimer > 0f)
                yield return new WaitForSeconds(.1f);
            
            photonView.RPC(nameof(SyncValuesRPC), RpcTarget.Others, explosionStrength, playerDamage, spawnUraniumCloud,
                uraniumCloudSize, uraniumCloudDuration, uraniumPlayerDamage, uraniumPlayerDamageRate);
            LegalizeNuclearBombs.Debug("Synced values to clients", this);
        }
        
        [PunRPC]
        private void SyncValuesRPC(float strength, int damage, bool cloud, float cloudSize, float cloudDuration,
            int cloudDamage, float cloudDamageRate, PhotonMessageInfo info = default)
        {
            if (!SemiFunc.MasterOnlyRPC(info)) return;
            
            explosionStrength = strength;
            playerDamage = damage;
            spawnUraniumCloud = cloud;
            uraniumCloudSize = cloudSize;
            uraniumCloudDuration = cloudDuration;
            uraniumPlayerDamage = cloudDamage;
            uraniumPlayerDamageRate = cloudDamageRate;
            
            LogInitValues();
        }
        
        private void LogInitValues() => LegalizeNuclearBombs.Logger.LogDebug(
            "Initialized values:" +
            $"\nexplosionStrength = {explosionStrength}" +
            $"\nplayerDamage = {playerDamage}" +
            $"\nspawnUraniumCloud = {spawnUraniumCloud}" +
            $"\nuraniumCloudSize = {uraniumCloudSize}" +
            $"\nuraniumCloudDuration = {uraniumCloudDuration}" +
            $"\nuraniumPlayerDamage = {uraniumPlayerDamage}" +
            $"\nuraniumPlayerDamageRate = {uraniumPlayerDamageRate}"
        );
        
        private void Update()
        {
            EmissionLogic();
            if (SemiFunc.IsNotMasterClient()) return;
            if (explosionDelayActive) ExplosionDelayLogic();
        }
        
        private void EmissionLogic()
        {
            if (!emissionActive)
            {
                emissionImpulse = true;
                return;
            }
            if (emissionImpulse)
            {
                GameDirector.instance.CameraImpact.ShakeDistance(
                    LegalizeNuclearBombs.configWarningCameraShakeStrength.Value,
                    1f,
                    6f,
                    transform.position,
                    .25f);
                emissionImpulse = false;
            }
            if (LegalizeNuclearBombs.configDebugLogLevel.Value is LegalizeNuclearBombs.LogLevels.Verbose)
                LegalizeNuclearBombs.Logger.LogDebug($"emissionColor: {material.GetColor(emissionColor).r}");
            material.SetColor(emissionColor, Color.white * Mathf.Lerp(
                material.GetColor(emissionColor).r,
                Mathf.Clamp(emissionTime, 0f, 1f),
                .35f)
            );
            if (material.GetColor(emissionColor).r > .01f)
            {
                if (emissionTime > 0f) emissionTime -= Time.deltaTime;
                return;
            }
            
            material.SetColor(emissionColor, Color.black);
            emissionActive = false;
        }
        
        private void ExplosionDelayLogic()
        {
            explosionDelayTime -= Time.deltaTime;
            if (explosionDelayTime <= 0f)
            {
                SetExplode();
                return;
            }
            if (explosionDelayImpulse)
            {
                if (SemiFunc.IsMultiplayer()) photonView.RPC(nameof(ExplosionDelayRPC), RpcTarget.All);
                else ExplosionDelayRPC();
                explosionDelayImpulse = false;
            }
        }
        
        [PunRPC]
        private void ExplosionDelayRPC(PhotonMessageInfo info = default)
        {
            if (!SemiFunc.MasterOnlyRPC(info)) return;
            
            var volume = LegalizeNuclearBombs.configExplosionDelayVolume.Value;
            if (volume > 0f) explosionDelaySound.Play(center.transform.position, volume);
            material.SetColor(emissionColor, Color.white);
            
            if (GameplayManager.instance.photosensitivity)
            {
                LegalizeNuclearBombs.Debug(
                    "Photosensitivity is enabled, skipping explosion delay visual effects.",
                    this);
                return;
            }
            
            if (LegalizeNuclearBombs.configExplosionDelayCameraGlitch.Value && physGrabObject.grabbedLocal)
            {
                CameraGlitch.Instance.PlayLong();
            }
            foreach (ParticleSystem p in explosionParticles)
            {
                p.Play();
            }
        }
        
        // PhysGrabObjectImpactDetector.onBreakLight
        public void PotentialExplodeLight()
        {
            if (SemiFunc.IsNotMasterClient()) return;
            if (LegalizeNuclearBombs.configHitSensitivity.Value is LegalizeNuclearBombs.HitSensitivity.Light)
                PotentialExplodeHeavy();
        }
        
        // PhysGrabObjectImpactDetector.onBreakMedium
        public void PotentialExplodeMedium()
        {
            if (SemiFunc.IsNotMasterClient()) return;
            if (LegalizeNuclearBombs.configHitSensitivity.Value is not LegalizeNuclearBombs.HitSensitivity.Heavy)
                PotentialExplodeHeavy();
        }
        
        // PhysGrabObjectImpactDetector.onBreakHeavy
        public void PotentialExplodeHeavy()
        {
            if (SemiFunc.IsNotMasterClient() || LegalizeNuclearBombs.configMaxHitCount.Value <= 0) return;
            hitCount++;
            if (hitCount >= LegalizeNuclearBombs.configMaxHitCount.Value)
            {
                if (explosionDelayActive) return;
                
                if (LegalizeNuclearBombs.configExplosionDelayTime.Value <= 0f) SetExplode();
                else
                {
                    explosionDelayTime = LegalizeNuclearBombs.configExplosionDelayTime.Value;
                    explosionDelayActive = true;
                }
            }
            else if (hitCount >= LegalizeNuclearBombs.configMaxHitCount.Value - 1)
            {
                if (SemiFunc.IsMultiplayer()) photonView.RPC(nameof(PlayWarningRPC), RpcTarget.All);
                else PlayWarningRPC();
            }
            LegalizeNuclearBombs.Debug("hitCount: " + hitCount, this);
        }
        
        [PunRPC]
        private void PlayWarningRPC(PhotonMessageInfo info = default)
        {
            if (!SemiFunc.MasterOnlyRPC(info)) return;
            
            var volume = LegalizeNuclearBombs.configWarningVolume.Value;
            if (volume > 0f) warningSound.Play(center.position, volume);
            
            if (LegalizeNuclearBombs.configShowWarningVisual.Value)
            {
                emissionTime = 1.5f;
                emissionImpulse = true;
                emissionActive = true;
            }
            LegalizeNuclearBombs.Debug("Played warning, one hit left", this);
        }
        
        // PhysGrabObjectImpactDetector.onDestroy
        public void SetExplode()
        {
            if (SemiFunc.IsNotMasterClient()) return;
            
            if (SemiFunc.IsMultiplayer())
                photonView.RPC(nameof(SetExplodeRPC), RpcTarget.Others);
            
            Explode();
            // There has to be a better way to do this...
        }
        
        [PunRPC]
        private void SetExplodeRPC(PhotonMessageInfo info = default)
        {
            if (!SemiFunc.MasterOnlyRPC(info)) return;
            
            Explode();
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void Explode()
        {
            if (detonated) return;
            detonated = true;
            
            GetComponent<ParticleScriptExplosion>().Spawn(
                center.position,
                explosionStrength,
                playerDamage,
                LegalizeNuclearBombs.configEnemyDamage.Value,
                LegalizeNuclearBombs.configExplosionStrength.Value,
                false,
                false,
                LegalizeNuclearBombs.configCameraShakeStrength.Value
                );
            
            if (spawnUraniumCloud)
            {
                var cloud = Instantiate(uraniumCloudPrefab, center.transform.position, Quaternion.identity)
                    .GetComponent<NukeUraniumCloud>();
                cloud.size = uraniumCloudSize;
                cloud.duration = uraniumCloudDuration;
                cloud.damage = uraniumPlayerDamage;
                cloud.damageRate = uraniumPlayerDamageRate;
            }
            
            if ((bool)physGrabObject) physGrabObject.impactDetector.DestroyObject();
            // explosionDelaySound.Stop();
        }
    }
}

// "I heard that snare took him two years to make."
