using HarmonyLib;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace LegalizeNuclearBombs.Patches
{
    [HarmonyPatch(typeof(ItemDroneIndestructible))]
    internal static class ItemDroneIndestructiblePatch
    {
        [HarmonyPostfix, HarmonyPatch(nameof(ItemDroneIndestructible.Update))]
        private static void UpdatePatch(ItemDroneIndestructible __instance)
        {
            var drain = LegalizeNuclearBombs.configIndestructibleDroneBatteryDrain.Value;
            
            if (drain <= 1f || __instance.itemEquippable.isEquipped || !__instance.itemDrone.magnetActive ||
                !__instance.itemDrone.magnetTargetPhysGrabObject.GetComponent<NukeValuable>())
                return;
            
            __instance.itemDrone.itemBattery.batteryLife -= drain * Time.deltaTime;
            
            // if (SemiFunc.PerSecond(.5f, __instance))
            //     LegalizeNuclearBombs.Debug($"Attached to NukeValuable, accelerated battery drain by {drain}",
            //         __instance);
        }
    }
}
