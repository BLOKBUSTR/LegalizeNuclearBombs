using UnityEngine;

#pragma warning disable CS8618
namespace LegalizeNuclearBombs
{
    public class NukeUraniumCloud : MonoBehaviour
    {
        public ParticleSystem uraniumCloudParticles;
        public Light particlePointLight;
        public HurtCollider hurtCollider;
        public Sound uraniumGeigerLoop;
        
        internal float size;
        internal float duration;
        internal int damage;
        internal float damageRate;
        
        private float geigerLoopTime;
        
        private void Start()
        {
            particlePointLight.range = size * 1.5f;
            
            uraniumCloudParticles.transform.localScale = Vector3.one * size * .2f;
            ParticleSystem.MainModule main = uraniumCloudParticles.main;
            main.startLifetime = new ParticleSystem.MinMaxCurve(duration + 3f, duration + 6f);
            uraniumCloudParticles.Play();
            
            hurtCollider.transform.localScale = Vector3.one * size;
            hurtCollider.playerDamage = damage;
            hurtCollider.playerDamageCooldown = damageRate;
            if (SemiFunc.IsMasterClientOrSingleplayer())
            {
                hurtCollider.enemyDamage = LegalizeNuclearBombs.configEnemyDamage.Value;
                hurtCollider.enemyDamageCooldown = LegalizeNuclearBombs.configUraniumEnemyDamageRate.Value;
            }
            hurtCollider.timer = duration;
            hurtCollider.gameObject.SetActive(true);
            
            geigerLoopTime = duration + 1f;
            
            Destroy(gameObject, duration + 6f);
            LegalizeNuclearBombs.Debug($"Spawned NukeUraniumCloud with total duration {duration + 6f} seconds", this);
        }
        
        private void Update()
        {
            uraniumGeigerLoop.PlayLoop(geigerLoopTime > 0f, 2f, .33f);
            if (geigerLoopTime > 0f) geigerLoopTime -= Time.deltaTime;
            
            if (SemiFunc.PerSecond(1f, this))
            {
                LegalizeNuclearBombs.DebugVerbose($"uraniumGeigerLoop playing: {uraniumGeigerLoop.Source.isPlaying}", this);
            }
        }
    }
}
