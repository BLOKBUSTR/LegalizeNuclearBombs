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
        
        public void PotentialExplode()
        {
            if (!isLocal) return;
            if (_hitCount >= LegalizeNuclearBombs.ConfigMaxHitCount.Value)
            {
                Explode();
            }
            else
            {
                // Play warning sound if almost about to go kaboom
                if (_hitCount >= LegalizeNuclearBombs.ConfigMaxHitCount.Value - 1)
                {
                    warningSound.Play(center.position);
                }
                _hitCount++;
            }
        }
    }
}
