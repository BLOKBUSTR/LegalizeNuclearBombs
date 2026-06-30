using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using REPOLib;
using REPOLib.Modules;
using UnityEngine;
using Object = UnityEngine.Object;

#pragma warning disable CS8618 // Resharper disable InconsistentNaming
namespace LegalizeNuclearBombs
{
    [BepInPlugin("BLOKBUSTR.LegalizeNuclearBombs", "LegalizeNuclearBombs", "3.1.0")]
    [BepInDependency("REPOLib")]
    public class LegalizeNuclearBombs : BaseUnityPlugin
    {
        internal static LegalizeNuclearBombs Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger => Instance._logger;
        private ManualLogSource _logger => base.Logger;
        internal Harmony? Harmony { get; set; }
        
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
        public static ConfigEntry<bool> configSpawnUraniumCloud;
        public static ConfigEntry<float> configUraniumCloudSize;
        public static ConfigEntry<float> configUraniumCloudDuration;
        public static ConfigEntry<int> configUraniumPlayerDamage;
        public static ConfigEntry<float> configUraniumPlayerDamageRate;
        public static ConfigEntry<int> configUraniumEnemyDamage;
        public static ConfigEntry<float> configUraniumEnemyDamageRate;
        
        // Break Warning
        public static ConfigEntry<float> configWarningVolume;
        public static ConfigEntry<bool> configShowWarningVisual;
        public static ConfigEntry<float> configWarningCameraShakeStrength;
        
        // Items
        public static ConfigEntry<float> configIndestructibleDroneBatteryDrain;
        
        // Debug
        internal enum LogLevels { Disabled, Enabled, Verbose }
        internal static ConfigEntry<LogLevels> configDebugLogLevel;
        
        #endregion
        
        private void Awake()
        {
            Instance = this;
            
            gameObject.transform.parent = null;
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            
            RegisterConfig();
            Patch();
            
            try
            {
                Logger.LogInfo("Loading AssetBundle...");
                var path = Path.Combine(Path.GetDirectoryName(Info.Location)!, "LegalizeNuclearBombs");
                BundleLoader.LoadBundle(path, assetBundle =>
                {
                    foreach (var a in assetBundle.GetAllAssetNames()) Logger.LogDebug(a);
                    
                    var prefab = assetBundle.LoadAsset<GameObject>("Nuke Uranium Cloud");
                    Logger.LogDebug($"{prefab}");
                    if ((bool)prefab) Utilities.FixAudioMixerGroups(prefab);
                    else Logger.LogError("Nuke Uranium Cloud prefab not found");
                }, true);
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to fix Audio Mixer Group on prefab Nuke Uranium Cloud with error: " + e.Message);
            }
            
            Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
            if (configDebugLogLevel.Value is LogLevels.Enabled) Logger.LogDebug("Debug logging is enabled.");
            if (configDebugLogLevel.Value is LogLevels.Verbose) Logger.LogDebug("Verbose debug logging is enabled.");
        }
        
        private void RegisterConfig()
        {
            // Nuke
            configHitSensitivity = Config.Bind("Nuke", "HitSensitivity", HitSensitivity.Medium,
                "The minimum impact strength that the nuke is sensitive to.");
            configMaxHitCount = Config.Bind("Nuke", "MaxHitCount", 3,
                new ConfigDescription("The number of hits the nuke can take until it explodes. Set to 0 to disable and make it explode only when it loses all value (this will disable the explosion delay).",
                    new AcceptableValueRange<int>(0, 10)));
            configExplosionStrength = Config.Bind("Nuke", "ExplosionStrength", 5f,
                new ConfigDescription("The strength of the explosion.",
                    new AcceptableValueRange<float>(1f, 15f)));
            configPlayerDamage = Config.Bind("Nuke", "PlayerDamage", 150,
                new ConfigDescription("The amount of damage dealt to players.",
                    new AcceptableValueRange<int>(0, 1000)));
            configEnemyDamage = Config.Bind("Nuke", "EnemyDamage", 200,
                new ConfigDescription("The amount of damage dealt to enemies.",
                    new AcceptableValueRange<int>(0, 1000)));
            configCameraShakeStrength = Config.Bind("Nuke", "CameraShakeStrength", 5f,
                new ConfigDescription("The intensity of the explosion camera shake.",
                    new AcceptableValueRange<float>(0f, 10f)));
            
            // Explosion Delay
            configExplosionDelayTime = Config.Bind("Explosion Delay", "ExplosionDelayTime", 1f,
                // ReSharper disable once StringLiteralTypo
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
            configSpawnUraniumCloud = Config.Bind("Uranium Cloud", "SpawnUraniumCloud", true,
                "Whether to spawn a uranium cloud upon explosion.");
            configUraniumCloudSize = Config.Bind("Uranium Cloud", "UraniumCloudSize", 20f,
                new ConfigDescription("The size of the uranium cloud, including its damage range.",
                    new AcceptableValueRange<float>(5f, 45f)));
            configUraniumCloudDuration = Config.Bind("Uranium Cloud", "UraniumCloudDuration", 15f,
                new ConfigDescription("The duration that the uranium cloud will linger for. The HurtCollider will disappear once the duration expires, but particles will linger for several seconds longer.",
                    new AcceptableValueRange<float>(5f, 45f)));
            configUraniumPlayerDamage = Config.Bind("Uranium Cloud", "UraniumPlayerDamage", 5,
                new ConfigDescription("The amount of damage dealt to players who are inside the uranium cloud.",
                    new AcceptableValueRange<int>(0, 25)));
            configUraniumPlayerDamageRate = Config.Bind("Uranium Cloud", "UraniumPlayerDamageRate", 1.5f,
                new ConfigDescription("The rate per second to damage the player.",
                    new AcceptableValueRange<float>(.25f, 5f)));
            configUraniumEnemyDamage = Config.Bind("Uranium Cloud", "UraniumEnemyDamage", 5,
                new ConfigDescription("The amount of damage dealt to enemies that are inside the uranium cloud.",
                    new AcceptableValueRange<int>(0, 25)));
            configUraniumEnemyDamageRate = Config.Bind("Uranium Cloud", "UraniumEnemyDamageRate", 2f,
                new ConfigDescription("The rate per second to damage enemies.",
                    new AcceptableValueRange<float>(.25f, 5f)));
            
            // Break Warning
            configWarningVolume = Config.Bind("Break Warning", "WarningVolume", .35f,
                new ConfigDescription("The volume of the warning sound. Set to 0 to disable.",
                    new AcceptableValueRange<float>(0f, 1f)));
            configShowWarningVisual = Config.Bind("Break Warning", "ShowWarningVisual", true,
                "Whether to momentarily show a red glow on the nuke when the break warning triggers.");
            configWarningCameraShakeStrength = Config.Bind("Break Warning", "WarningCameraShakeStrength", 1.5f,
                new ConfigDescription("The intensity of the warning camera shake.",
                    new AcceptableValueRange<float>(0f, 3f)));
            
            // Items
            configIndestructibleDroneBatteryDrain = Config.Bind("Items", "IndestructibleDroneBatteryDrain", 2f,
                new ConfigDescription("The multiplier to accelerate the Indestructible Drone's battery drain when attached to the Nuke. Set to 1 for vanilla behavior.",
                    new AcceptableValueRange<float>(1f, 15f)));
            
            // Debug
            configDebugLogLevel = Config.Bind("Debug", "DebugLogLevel", LogLevels.Disabled,
                "The debug logging level to use. Keep this disabled during normal gameplay.");
        }
        
        private void Patch()
        {
            Harmony ??= new Harmony(Info.Metadata.GUID);
            Harmony.PatchAll();
        }
        
        // Consideration: compact these into Debug methods into one
        public static void Debug(string message, Object obj)
        {
            if (configDebugLogLevel.Value is LogLevels.Enabled)
                Logger.LogDebug((bool)obj ? $"{obj} ({obj.GetInstanceID()}): {message}" : message);
        }
        
        public static void DebugVerbose(string message, Object obj)
        {
            if (configDebugLogLevel.Value is LogLevels.Verbose)
                Logger.LogDebug((bool)obj ? $"{obj} ({obj.GetInstanceID()}): {message}" : message);
        }
        
        // public static void Error(string message, Object? obj = null)
        // {
        //     Logger.LogError((bool)obj ? $"{obj} ({obj!.GetInstanceID()}): {message}" : message);
        // }
    }
}
