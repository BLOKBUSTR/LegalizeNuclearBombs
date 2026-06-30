using HarmonyLib;

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
            __instance.itemDrone.itemBattery.batteryDrainRate =
                drain <= 1f || __instance.itemEquippable.isEquipped || !__instance.itemDrone.magnetActive ||
                !__instance.itemDrone.magnetTargetPhysGrabObject.GetComponent<ValuableWarhead>()
                    ? __instance.itemDrone.batteryDrainRate
                    : __instance.itemDrone.batteryDrainRate * drain;
        }
    }
}
