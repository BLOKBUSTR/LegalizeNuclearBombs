using HarmonyLib;
using LegalizeNuclearBombs.MonoBehaviours;

// ReSharper disable InconsistentNaming
namespace LegalizeNuclearBombs.Patches;

[HarmonyPatch(typeof(ItemDroneIndestructible))]
internal static class ItemDronePatches
{
    // BUG
    internal static void SetBatteryDrain(ItemDrone drone, float drain)
    {
        drone.itemBattery.batteryDrainRate =
            drain <= 1f || drone.itemEquippable.isEquipped || !drone.magnetActive ||
            !drone.magnetTargetPhysGrabObject.GetComponent<ValuableWarhead>()
                ? drone.batteryDrainRate
                : drone.batteryDrainRate * drain;
    }
}

internal static class ItemDroneFeatherPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(ItemDroneFeather.FixedUpdate))]
    private static void FixedUpdatePatch(ItemDroneIndestructible __instance) =>
        ItemDronePatches.SetBatteryDrain(__instance.itemDrone,
            LegalizeNuclearBombs.configFeatherDroneBatteryDrain.Value);
}

internal static class ItemDroneIndestructiblePatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(ItemDroneIndestructible.Update))]
    private static void UpdatePatch(ItemDroneIndestructible __instance) =>
        ItemDronePatches.SetBatteryDrain(__instance.itemDrone,
            LegalizeNuclearBombs.configIndestructibleDroneBatteryDrain.Value);
}

internal static class ItemDroneTorquePatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(ItemDroneTorque.FixedUpdate))]
    private static void FixedUpdatePatch(ItemDroneIndestructible __instance) =>
        ItemDronePatches.SetBatteryDrain(__instance.itemDrone,
            LegalizeNuclearBombs.configRollDroneBatteryDrain.Value);
}

internal static class ItemDroneZeroGravityPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(ItemDroneZeroGravity.FixedUpdate))]
    private static void FixedUpdatePatch(ItemDroneIndestructible __instance) =>
        ItemDronePatches.SetBatteryDrain(__instance.itemDrone,
            LegalizeNuclearBombs.configZeroGravityDroneBatteryDrain.Value);
}
