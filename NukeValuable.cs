using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

#pragma warning disable CS8618
namespace LegalizeNuclearBombs
{
    public class NukeValuable : MonoBehaviour
    {
        public Transform center;
        public GameObject mesh;
        private Material material;
        private static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");
        private PhysGrabObject physGrabObject;
        private PhotonView photonView;
        
        public GameObject uraniumCloudPrefab;
        
        public Sound warningSound;
        public Sound explosionDelaySound;
        
        private int hitCount;
        private bool detonated;
        
        public List<ParticleSystem> explosionParticles;
        private bool explosionDelayActive;
        private bool explosionDelayImpulse = true;
        private float explosionDelayTime;
        
        private bool emissionActive;
        private bool emissionImpulse;
        private float emissionTime;
        
        private void Start()
        {
            material = mesh.GetComponent<MeshRenderer>().material;
            physGrabObject = GetComponent<PhysGrabObject>();
            photonView = GetComponent<PhotonView>();
            LegalizeNuclearBombs.Debug("New nuke valuable spawned", this);
            
            // if (LegalizeNuclearBombs.configEnableDebug.Value)
            //     foreach (AudioClip a in explosionDelaySound.Sounds)
            //         LegalizeNuclearBombs.Logger.LogDebug(this + ": " + a.name + " | " + a.length);
        }
        
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
            LegalizeNuclearBombs.Debug($"emissionColor: {material.GetColor(emissionColor).r}");
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
            
            if (GameplayManager.instance.photosensitivity) return;
            
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
            if (hitCount >= LegalizeNuclearBombs.configMaxHitCount.Value - 1)
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
            
            var explosionStrength = LegalizeNuclearBombs.configExplosionStrength.Value;
            var playerDamage = LegalizeNuclearBombs.configPlayerDamage.Value;
            
            if (SemiFunc.IsMultiplayer())
                photonView.RPC(nameof(SetExplodeRPC), RpcTarget.Others, explosionStrength, playerDamage);
            
            Explode(explosionStrength, playerDamage);
        }
        
        [PunRPC]
        private void SetExplodeRPC(float explosionStrength, int playerDamage, PhotonMessageInfo info = default)
        {
            if (!SemiFunc.MasterOnlyRPC(info)) return;
            
            Explode(explosionStrength, playerDamage);
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void Explode(float explosionStrength, int playerDamage)
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
            
            Instantiate(uraniumCloudPrefab, center.transform.position, Quaternion.identity).GetComponent<UraniumScript>();
            
            if ((bool)physGrabObject) physGrabObject.impactDetector.DestroyObject();
            // explosionDelaySound.Stop();
            
            LegalizeNuclearBombs.Debug(
                $"Explode (explosionStrength: {explosionStrength}, playerDamage = {playerDamage})",
                this);
        }
    }
}

// "I heard that snare took him two years to make."
