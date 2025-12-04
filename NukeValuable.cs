using Photon.Pun;
using UnityEngine;

// TODO: Fix emissive warning texture overlay implementation when I have the time

namespace LegalizeNuclearBombs
{
    public class NukeValuable : Trap
    {
        #pragma warning disable CS8618
        
        public Transform center;

        // public GameObject mesh;

        // private Material _material;

        // private bool _emissionActive;

        // private float _emissionTime;
        
        private ParticleScriptExplosion _particleScriptExplosion;
        
        private int _hitCount;
        
        public Sound warningSound;
        
        public override void Start()
        {
            base.Start();
            _particleScriptExplosion = GetComponent<ParticleScriptExplosion>();
            // _material = mesh.GetComponent<Renderer>().material;
            
            LegalizeNuclearBombs.Debug($"New nuke valuable spawned ({GetInstanceID()})");
        }

        // public void FixedUpdate()
        // {
        //     if (!_emissionActive) return;
        //     _material.EnableKeyword("_EMISSION");
        //     _emissionTime += Time.fixedDeltaTime;
        //     
        //     if (!(_emissionTime >= 1)) return;
        //     _material.DisableKeyword("_EMISSION");
        //     _emissionActive = false;
        // }
        
        public void Explode()
        {
            _particleScriptExplosion.Spawn(
                center.position,
                LegalizeNuclearBombs.ConfigExplosionStrength.Value,
                LegalizeNuclearBombs.ConfigPlayerDamage.Value,
                LegalizeNuclearBombs.ConfigEnemyDamage.Value,
                LegalizeNuclearBombs.ConfigExplosionStrength.Value,
                false,
                false,
                LegalizeNuclearBombs.ConfigShakeMultiplier.Value
                );
            LegalizeNuclearBombs.Debug($"KABOOM ({GetInstanceID()})");
        }
        
        public void PotentialExplodeLight() // Triggers if ConfigHitSensitivity is 2
        {
            if (!isLocal) return;
            if (LegalizeNuclearBombs.ConfigHitSensitivity.Value > 1) PotentialExplodeHeavy();
        }
        
        public void PotentialExplodeMedium() // Triggers if ConfigHitSensitivity is 1
        {
            if (!isLocal) return;
            if (LegalizeNuclearBombs.ConfigHitSensitivity.Value > 0) PotentialExplodeHeavy();
        }
        
        public void PotentialExplodeHeavy() // Always triggers
        {
            if (!isLocal || LegalizeNuclearBombs.ConfigMaxHitCount.Value <= 0) return;
            if (_hitCount >= LegalizeNuclearBombs.ConfigMaxHitCount.Value - 1)
            {
                Explode();
            }
            else
            {
                LegalizeNuclearBombs.Debug($"_hitCount: {_hitCount + 1} ({GetInstanceID()})");
                // Play warning sound if almost about to go kaboom
                if (_hitCount >= LegalizeNuclearBombs.ConfigMaxHitCount.Value - 2)
                {
                    if (SemiFunc.IsMultiplayer())
                        photonView.RPC(nameof(PlayWarningRPC), RpcTarget.All);
                    else
                        PlayWarningRPC();
                }
                _hitCount++;
            }
        }
        
        // Configs checks are placed here so clients can have their own individual config preferences
        [PunRPC]
        public void PlayWarningRPC(PhotonMessageInfo info = default)
        {
            if (LegalizeNuclearBombs.ConfigPlayWarningSound.Value)
            {
                warningSound.Volume = LegalizeNuclearBombs.ConfigWarningVolume.Value;
                warningSound.Play(center.position);
                LegalizeNuclearBombs.Debug($"Played audio warning; one hit left ({GetInstanceID()})");
            }

            // if (LegalizeNuclearBombs.ConfigShowWarningTextureOverlay.Value)
            // {
            //     _emissionTime = 0f;
            //     _emissionActive = true;
            // }
        }
    }
}

// "I heard that snare took him two years to make."
