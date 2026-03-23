using System.Diagnostics.CodeAnalysis;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

#pragma warning disable CS8618
namespace LegalizeNuclearBombs
{
    [BepInPlugin("BLOKBUSTR.LegalizeNuclearBombs", "LegalizeNuclearBombs", "2.0.0")]
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
        
        public static ConfigEntry<HitSensitivity> configHitSensitivity;
        public static ConfigEntry<int> configMaxHitCount;
        public static ConfigEntry<float> configExplosionStrength;
        public static ConfigEntry<int> configPlayerDamage;
        public static ConfigEntry<int> configEnemyDamage;
        public static ConfigEntry<float> configCameraShakeStrength;
        
        public static ConfigEntry<bool> configPlayWarningSound;
        public static ConfigEntry<float> configWarningVolume;
        public static ConfigEntry<bool> configShowWarningVisual;
        public static ConfigEntry<float> configWarningCameraShakeStrength;
        
        private static ConfigEntry<bool> configEnableDebug;
        
        private void Awake()
        {
            Instance = this;
            
            gameObject.transform.parent = null;
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            
            RegisterConfig();
            // Patch();
            
            Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
            Debug("Debug logs are enabled.");
        }
        
        private void RegisterConfig()
        {
            // 1 - Nuke
            configHitSensitivity = Config.Bind("1 - Nuke", "HitSensitivity", HitSensitivity.Medium,
                new ConfigDescription("The minimum impact strength that the nuke is sensitive to."));
            configMaxHitCount = Config.Bind("1 - Nuke", "MaxHitCount", 3,
                new ConfigDescription("The number of hits the nuke can take until it explodes. Set to 0 to disable and make it explode only when it loses all value.",
                    new AcceptableValueRange<int>(0, 10)));
            configExplosionStrength = Config.Bind("1 - Nuke", "ExplosionStrength", 15f,
                new ConfigDescription("The strength of the explosion.",
                    new AcceptableValueRange<float>(1f, 25)));
            configPlayerDamage = Config.Bind("1 - Nuke", "PlayerDamage", 200,
                new ConfigDescription("The amount of damage dealt to players.",
                    new AcceptableValueRange<int>(0, 1000)));
            configEnemyDamage = Config.Bind("1 - Nuke", "EnemyDamage", 400,
                new ConfigDescription("The amount of damage dealt to enemies.",
                    new AcceptableValueRange<int>(0, 1000)));
            configCameraShakeStrength = Config.Bind("1 - Nuke", "CameraShakeStrength", 5f,
                new ConfigDescription("The intensity of the explosion camera shake.",
                    new AcceptableValueRange<float>(0f, 10f)));
            
            // 2 - Break Warning
            configPlayWarningSound = Config.Bind("2 - Break Warning", "PlayWarningSound", true,
                new ConfigDescription("Whether to play a fizzing sound as a warning when the nuke has only one hit remaining."));
            configWarningVolume = Config.Bind("2 - Break Warning", "WarningVolume", 0.35f,
                new ConfigDescription("The volume of the warning sound.",
                    new AcceptableValueRange<float>(0f, 1f)));
            configShowWarningVisual = Config.Bind("2 - Break Warning", "ShowWarningVisual", true,
                new ConfigDescription("Whether to momentarily show a red glow on the nuke when it has only one hit remaining."));
            configWarningCameraShakeStrength = Config.Bind("2 - Break Warning", "WarningCameraShakeStrength", 1.5f,
                new ConfigDescription("The intensity of the warning camera shake.",
                    new AcceptableValueRange<float>(0f, 2f)));
            
            // Debug
            configEnableDebug = Config.Bind("Debug", "EnableDebugLogging", false,
                new ConfigDescription("Whether to enable debug logging."));
        }
        
        // internal void Patch()
        // {
        //     Harmony ??= new Harmony(Info.Metadata.GUID);
        //     Harmony.PatchAll();
        // }
        
        // internal void Unpatch()
        // {
        //     Harmony?.UnpatchSelf();
        // }
        
        public static void Debug(string message, MonoBehaviour? monoBehaviour = null)
        {
            if (!configEnableDebug.Value) return;
            var prefix = monoBehaviour == null ? string.Empty : $"{monoBehaviour}: ";
            Logger.LogDebug(prefix + message);
        }
    }
}
