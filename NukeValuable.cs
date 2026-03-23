using Photon.Pun;
using UnityEngine;

#pragma warning disable CS8618
namespace LegalizeNuclearBombs
{
    public class NukeValuable : Trap
    {
        public Transform center;
        
        public GameObject mesh;
        
        private Material material;
        private static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");
        
        private bool detonated;
        
        private bool emissionActive;
        private bool emissionImpulse;
        
        private float emissionTime;
        
        private ParticleScriptExplosion particleScriptExplosion;
        
        private int hitCount;
        
        public Sound warningSound;
        
        public override void Start()
        {
            base.Start();
            particleScriptExplosion = GetComponent<ParticleScriptExplosion>();
            material = mesh.GetComponent<Renderer>().material;
            
            LegalizeNuclearBombs.Debug($"New nuke valuable spawned | {gameObject}", this);
        }
        
        public override void Update()
        {
            // Debug
            // if (Input.GetKeyDown(KeyCode.N))
            // {
            //     PlayWarningRPC();
            // }
            
            if (!emissionActive) return;
            if (emissionImpulse)
            {
                GameDirector.instance.CameraImpact.ShakeDistance(LegalizeNuclearBombs.configWarningCameraShakeStrength.Value, 1f, 6f, transform.position, .25f);
                emissionImpulse = false;
            }
            LegalizeNuclearBombs.Debug($"emissionColor: {material.GetColor(emissionColor).r}");
            material.SetColor(emissionColor, Color.white * Mathf.Lerp(
                material.GetColor(emissionColor).r,
                Mathf.Clamp(emissionTime, 0f, 1f),
                .35f)
            );
            if (emissionTime > 0f)
            {
                emissionTime -= Time.deltaTime;
                return;
            }
            if (material.GetColor(emissionColor).r > .01f) return;
            material.SetColor(emissionColor, Color.black);
            emissionActive = false;
        }
        
        public void Explode()
        {
            if (detonated) return;
            particleScriptExplosion.Spawn(
                center.position,
                LegalizeNuclearBombs.configExplosionStrength.Value,
                LegalizeNuclearBombs.configPlayerDamage.Value,
                LegalizeNuclearBombs.configEnemyDamage.Value,
                LegalizeNuclearBombs.configExplosionStrength.Value,
                false,
                false,
                LegalizeNuclearBombs.configCameraShakeStrength.Value
                );
            LegalizeNuclearBombs.Debug("KABOOM", this);
            detonated = true;
            Destroy(gameObject); // Prevent spamming the explosion, especially at smaller strength
        }
        
        #region UnityEvents
        
        public void PotentialExplodeLight()
        {
            if (!isLocal) return;
            if (LegalizeNuclearBombs.configHitSensitivity.Value is LegalizeNuclearBombs.HitSensitivity.Light)
                PotentialExplodeHeavy();
        }
        
        public void PotentialExplodeMedium()
        {
            if (!isLocal) return;
            if (LegalizeNuclearBombs.configHitSensitivity.Value is not LegalizeNuclearBombs.HitSensitivity.Heavy)
                PotentialExplodeHeavy();
        }
        
        public void PotentialExplodeHeavy()
        {
            if (!isLocal || LegalizeNuclearBombs.configMaxHitCount.Value <= 0) return;
            if (hitCount >= LegalizeNuclearBombs.configMaxHitCount.Value - 1)
            {
                Explode();
            }
            else
            {
                LegalizeNuclearBombs.Debug($"_hitCount: {hitCount + 1}", this);
                // Play warning sound if almost about to go kaboom
                if (hitCount >= LegalizeNuclearBombs.configMaxHitCount.Value - 2)
                {
                    if (SemiFunc.IsMultiplayer()) photonView.RPC(nameof(PlayWarningRPC), RpcTarget.All);
                    else PlayWarningRPC();
                }
                hitCount++;
            }
        }
        
        #endregion
        
        [PunRPC]
        public void PlayWarningRPC(PhotonMessageInfo info = default)
        {
            if (LegalizeNuclearBombs.configPlayWarningSound.Value)
            {
                warningSound.Volume = LegalizeNuclearBombs.configWarningVolume.Value;
                warningSound.Play(center.position);
            }
            
            if (LegalizeNuclearBombs.configShowWarningVisual.Value)
            {
                emissionTime = 1.5f;
                emissionImpulse = true;
                emissionActive = true;
            }
            LegalizeNuclearBombs.Debug("Played warning, one hit left", this);
        }
    }
}

// "I heard that snare took him two years to make."
