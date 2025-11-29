using UnityEngine;

namespace LegalizeNuclearBombs
{
    public class NukeValuable : Trap
    {
        #pragma warning disable CS8618
        private ParticleScriptExplosion _particleScriptExplosion;
        
        private int _hitCount;
        
        public Sound warningSound;
        
        public Transform center;
        
        public override void Start()
        {
            base.Start();
            _particleScriptExplosion = GetComponent<ParticleScriptExplosion>();
            
            LegalizeNuclearBombs.Debug($"New nuke valuable spawned ({GetInstanceID()})");
        }
        
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
            if (!isLocal) return;
            if (_hitCount >= LegalizeNuclearBombs.ConfigMaxHitCount.Value - 1)
            {
                Explode();
            }
            else
            {
                LegalizeNuclearBombs.Debug($"_hitCount: {_hitCount++}");
                // Play warning sound if almost about to go kaboom
                if (_hitCount >= LegalizeNuclearBombs.ConfigMaxHitCount.Value - 2
                    && LegalizeNuclearBombs.ConfigPlayWarningSound.Value)
                {
                    warningSound.Volume = LegalizeNuclearBombs.ConfigWarningVolume.Value;
                    warningSound.Play(center.position);
                    LegalizeNuclearBombs.Debug("Played audio warning; one hit left");
                }
                _hitCount++;
            }
        }
    }
}

// I heard that snare took him two years to make.
