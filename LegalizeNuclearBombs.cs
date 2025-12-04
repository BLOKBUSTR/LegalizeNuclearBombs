using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace LegalizeNuclearBombs
{
    [BepInPlugin("BLOKBUSTR.LegalizeNuclearBombs", "LegalizeNuclearBombs", "1.0.0")]
    public class LegalizeNuclearBombs : BaseUnityPlugin
    {
        internal static LegalizeNuclearBombs Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger => Instance._logger;
        private ManualLogSource _logger => base.Logger;
        // internal Harmony? Harmony { get; set; }
        
        #pragma warning disable CS8618

        public static ConfigEntry<int> ConfigHitSensitivity;
        public static ConfigEntry<int> ConfigMaxHitCount;
        public static ConfigEntry<float> ConfigExplosionStrength;
        public static ConfigEntry<int> ConfigPlayerDamage;
        public static ConfigEntry<int> ConfigEnemyDamage;
        public static ConfigEntry<float> ConfigShakeMultiplier;

        public static ConfigEntry<bool> ConfigPlayWarningSound;
        public static ConfigEntry<float> ConfigWarningVolume;
        // public static ConfigEntry<bool> ConfigShowWarningTextureOverlay;
        
        private static ConfigEntry<bool> _configEnableDebugLogging;
        
        private void Awake()
        {
            // Config setup
            // 1 - Nuke
            ConfigHitSensitivity = Config.Bind("1 - Nuke", "HitSensitivity", 1,
                new ConfigDescription("How sensitive the nuke is to impacts. The higher the value, the higher the sensitivity.",
                    new AcceptableValueRange<int>(0, 2)));
            ConfigMaxHitCount = Config.Bind("1 - Nuke", "MaxHitCount", 3,
                new ConfigDescription("The number of hits the nuke can take until it explodes. Set to 0 to disable and make it explode only when it loses all value.",
                    new AcceptableValueRange<int>(0, 10)));
            ConfigExplosionStrength = Config.Bind("1 - Nuke", "ExplosionStrength", 15f,
                new ConfigDescription("The strength of the explosion.",
                    new AcceptableValueRange<float>(0f, 25)));
            ConfigPlayerDamage = Config.Bind("1 - Nuke", "PlayerDamage", 200,
                new ConfigDescription("The amount of damage dealt to players.",
                    new AcceptableValueRange<int>(0, 1000)));
            ConfigEnemyDamage = Config.Bind("1 - Nuke", "EnemyDamage", 400,
                new ConfigDescription("The amount of damage dealt to enemies.",
                    new AcceptableValueRange<int>(0, 1000)));
            ConfigShakeMultiplier = Config.Bind("1 - Nuke", "ShakeMultiplier", 3f,
                new ConfigDescription("The intensity of the camera shake.",
                    new AcceptableValueRange<float>(0f, 10f)));
            
            // 2 - Break Warning
            ConfigPlayWarningSound = Config.Bind("2 - Break Warning", "PlayWarningSound", true,
                new ConfigDescription("Whether to play a fizzing sound as a warning when the nuke has only one hit remaining."));
            ConfigWarningVolume = Config.Bind("2 - Break Warning", "WarningVolume", 0.3f,
                new ConfigDescription("The volume of the warning sound.",
                    new AcceptableValueRange<float>(0f, 1f)));
            // ConfigShowWarningTextureOverlay = Config.Bind("2 - Break Warning", "ShowWarningTextureOverlay", true,
            //     new ConfigDescription("Whether to momentarily show a red glowing overlay when the nuke has only one hit remaining."));
            
            // Debug
            _configEnableDebugLogging = Config.Bind("Debug", "EnableDebugLogging", false,
                new ConfigDescription("Whether to enable debug logging."));
            
            Instance = this;
            
            // Prevent the plugin from being deleted
            gameObject.transform.parent = null;
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            
            // Patch();
            
            Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
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
        
        // private void Update()
        // {
        //     // Code that runs every frame goes here
        // }

        public static void Debug(string message)
        {
            if (!_configEnableDebugLogging.Value) return;
            Logger.LogDebug(message);
        }
    }
}
