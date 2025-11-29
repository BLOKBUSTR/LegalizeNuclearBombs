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
        
        public static ConfigEntry<int> ConfigMaxHitCount;
        public static ConfigEntry<float> ConfigExplosionStrength;
        public static ConfigEntry<int> ConfigPlayerDamage;
        public static ConfigEntry<int> ConfigEnemyDamage;
        public static ConfigEntry<float> ConfigShakeMultiplier;
        
        private static ConfigEntry<bool> _configEnableDebugLogging;
        
        #pragma warning restore CS8618
        
        private void Awake()
        {
            // Config setup
            ConfigMaxHitCount = Config.Bind("1 - Nuke", "MaxHitCount", 3,
                new ConfigDescription("The number of heavy hits the nuke can take until it explodes.",
                    new AcceptableValueRange<int>(0, 10)));
            ConfigExplosionStrength = Config.Bind("1 - Nuke", "ExplosionStrength", 10f,
                new ConfigDescription("The strength of the explosion.",
                    new AcceptableValueRange<float>(0f, 100f)));
            ConfigPlayerDamage = Config.Bind("1 - Nuke", "PlayerDamage", 200,
                new ConfigDescription("The amount of damage dealt to players.",
                    new AcceptableValueRange<int>(0, 1000)));
            ConfigEnemyDamage = Config.Bind("1 - Nuke", "EnemyDamage", 500,
                new ConfigDescription("The amount of damage dealt to enemies.",
                    new AcceptableValueRange<int>(0, 1000)));
            ConfigShakeMultiplier = Config.Bind("1 - Nuke", "ShakeMultiplier", 3f,
                new ConfigDescription("The intensity of the camera shake.",
                    new AcceptableValueRange<float>(0f, 10f)));
            
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