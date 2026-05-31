using System.Diagnostics.CodeAnalysis;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

#pragma warning disable CS8618
namespace LegalizeNuclearBombs
{
    [BepInPlugin("BLOKBUSTR.LegalizeNuclearBombs", "LegalizeNuclearBombs", "3.0.0")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class LegalizeNuclearBombs : BaseUnityPlugin
    {
        internal static LegalizeNuclearBombs Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger => Instance._logger;
        private ManualLogSource _logger => base.Logger;
        // internal Harmony? Harmony { get; set; }
        
        public enum HitSensitivity
        {
            Light = 0,
            Medium = 1,
            Heavy = 2
        }
        
        #region ConfigEntries
        
        // Nuke
        public static ConfigEntry<HitSensitivity> configHitSensitivity;
        public static ConfigEntry<int> configMaxHitCount;
        public static ConfigEntry<float> configExplosionStrength;
        public static ConfigEntry<int> configPlayerDamage;
        public static ConfigEntry<int> configEnemyDamage;
        public static ConfigEntry<float> configCameraShakeStrength;
        
        // Explosion Delay
        public static ConfigEntry<float> configExplosionDelayTime;
        public static ConfigEntry<float> configExplosionDelayVolume;
        public static ConfigEntry<bool> configExplosionDelayParticles;
        public static ConfigEntry<bool> configExplosionDelayCameraGlitch;
        
        // Uranium Cloud
        public static ConfigEntry<bool> configExplosionUraniumCloud;
        
        // Break Warning
        public static ConfigEntry<float> configWarningVolume;
        public static ConfigEntry<bool> configShowWarningVisual;
        public static ConfigEntry<float> configWarningCameraShakeStrength;
        
        // Debug
        internal static ConfigEntry<bool> configEnableDebug;
        
        #endregion
        
        private void Awake()
        {
            Instance = this;
            
            gameObject.transform.parent = null;
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            
            RegisterConfig();
            // Patch();
            
            Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
            Debug("Debug logging is enabled.");
        }
        
        private void RegisterConfig()
        {
            // Nuke
            configHitSensitivity = Config.Bind("Nuke", "HitSensitivity", HitSensitivity.Medium,
                "The minimum impact strength that the nuke is sensitive to.");
            configMaxHitCount = Config.Bind("Nuke", "MaxHitCount", 3,
                new ConfigDescription("The number of hits the nuke can take until it explodes. Set to 0 to disable and make it explode only when it loses all value (this will disable the explosion delay!).",
                    new AcceptableValueRange<int>(0, 10)));
            configExplosionStrength = Config.Bind("Nuke", "ExplosionStrength", 12f,
                new ConfigDescription("The strength of the explosion.",
                    new AcceptableValueRange<float>(1f, 25f)));
            configPlayerDamage = Config.Bind("Nuke", "PlayerDamage", 200,
                new ConfigDescription("The amount of damage dealt to players.",
                    new AcceptableValueRange<int>(0, 1000)));
            configEnemyDamage = Config.Bind("Nuke", "EnemyDamage", 400,
                new ConfigDescription("The amount of damage dealt to enemies.",
                    new AcceptableValueRange<int>(0, 1000)));
            configCameraShakeStrength = Config.Bind("Nuke", "CameraShakeStrength", 5f,
                new ConfigDescription("The intensity of the explosion camera shake.",
                    new AcceptableValueRange<float>(0f, 10f)));
            
            // Explosion Delay
            configExplosionDelayTime = Config.Bind("Explosion Delay", "ExplosionDelayTime", 1f,
                new ConfigDescription("Time in seconds that the explosion will be delayed after the nuke has taken its last hit. Can be adjusted to match the length of a custom sound added with loaforcsSoundAPI, as long as it's under 10 seconds. Please do not change if using the default sound.",
                    new AcceptableValueRange<float>(0f, 10f)));
            configExplosionDelayVolume = Config.Bind("Explosion Delay", "ExplosionDelayVolume", .65f,
                new ConfigDescription("The volume of the explosion delay sound.",
                    new AcceptableValueRange<float>(0f, 1f)));
            configExplosionDelayParticles = Config.Bind("Explosion Delay", "ExplosionDelayParticles", true,
                "Whether to play particle effects during the explosion delay.");
            configExplosionDelayCameraGlitch = Config.Bind("Explosion Delay", "ExplosionDelayCameraGlitch", true,
                "Whether to play the camera glitch effect to players holding the nuke when its explosion delay begins.");
            
            // Uranium Cloud
            configExplosionUraniumCloud = Config.Bind("Uranium Cloud", "ExplosionUraniumCloud", true,
                "Whether to spawn a uranium cloud upon explosion.");
            
            // Break Warning
            configWarningVolume = Config.Bind("Break Warning", "WarningVolume", .35f,
                new ConfigDescription("The volume of the warning sound. Set to 0 to disable.",
                    new AcceptableValueRange<float>(0f, 1f)));
            configShowWarningVisual = Config.Bind("Break Warning", "ShowWarningVisual", true,
                "Whether to momentarily show a red glow on the nuke when the break warning triggers.");
            configWarningCameraShakeStrength = Config.Bind("Break Warning", "WarningCameraShakeStrength", 1.5f,
                new ConfigDescription("The intensity of the warning camera shake.",
                    new AcceptableValueRange<float>(0f, 3f)));
            
            // Debug
            configEnableDebug = Config.Bind("Debug", "EnableDebugLogging", false,
                "Whether to enable debug logging.");
        }
        
        public static void Debug(string message, MonoBehaviour? mono = null)
        {
            if (configEnableDebug.Value) Logger.LogDebug((bool)mono ? mono + ": " + message : message);
        }
        
        // public static void Error(string message, MonoBehaviour? mono = null)
        // {
        //     Logger.LogError((bool)mono ? mono + ": " + message : message);
        // }
    }
}
