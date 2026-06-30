# ☢️ LegalizeNuclearBombs ☢️

[![Follow BLOKBUSTR](https://img.shields.io/badge/fanlink-follow%20blokbustr-ad6fcc?style=for-the-badge)](https://fanlink.tv/blokbustr)

Adds a custom-made nuclear warhead as a Tall valuable, even bigger than before! Handle with the utmost caution!!

❗ **This mod must be installed on all clients.**

![Screenshot of the nuke valuable in-game](https://raw.githubusercontent.com/BLOKBUSTR/LegalizeNuclearBombs/refs/heads/master/Screenshot.jpg)

## 🔧 Configuration
This mod is highly configurable! Configs can be updated in-game with [REPOConfig](https://thunderstore.io/c/repo/p/nickklmao/REPOConfig/).
I also highly recommend using [MenuLibExtras](https://thunderstore.io/c/repo/p/Jettcodey/MenuLibExtras) for finer control.

<details>
<summary>Click to expand config list:</summary>

| Category            | ConfigEntry                     | Default Value | Description |
|---------------------|---------------------------------|:-------------:|-------------|
| **Nuke**            |
| &#124;              | HitSensitivity                  |    Medium     | The minimum impact strength that the nuke is sensitive to. |
| &#124;              | MaxHitCount                     |       3       | The number of hits the nuke can take until it explodes. Set to 0 to disable and make it explode only when it loses all value (this will also disable the explosion delay). |
| &#124;              | ExplosionStrength               |      15f      | The strength of the explosion. |
| &#124;              | PlayerDamage                    |      150      | The amount of damage dealt to players. |
| &#124;              | EnemyDamage                     |      300      | The amount of damage dealt to enemies. |
| ↳                   | CameraShakeStrength             |      5f       | The intensity of the explosion camera shake. |
| **Explosion Delay** |
| &#124;              | ExplosionDelayTime              |      1f       | Time in seconds that the explosion will be delayed after the nuke has taken its last hit. Can be adjusted to match the length of a custom sound added with loaforcsSoundAPI, as long as it's under 10 seconds. Please do not change if using the default sound. |
| &#124;              | ExplosionDelayVolume            |     0.5f      | The volume of the explosion delay sound. |
| &#124;              | ExplosionDelayParticles         |     true      | Whether to play particle effects during the explosion delay. |
| ↳                   | ExplosionDelayCameraGlitch      |     true      | Whether to play the camera glitch effect to players holding the nuke when its explosion delay begins. |
| **Uranium Cloud**   |
| &#124;              | SpawnUraniumCloud               |     true      | Whether to spawn a uranium cloud upon explosion. |
| &#124;              | UraniumCloudSize                |      15f      | The size of the uranium cloud, including its damage range. |
| &#124;              | UraniumCloudDuration            |      12f      | The duration that the uranium cloud will linger for. The HurtCollider will disappear once the duration expires, but particles will linger for several seconds longer. |
| &#124;              | UraniumPlayerDamage             |       5       | The amount of damage dealt to players who are inside the uranium cloud. |
| &#124;              | UraniumPlayerDamageRate         |     1.5f      | The rate per second to damage the player. |
| &#124;              | UraniumEnemyDamage              |       5       | The amount of damage dealt to enemies that are inside the uranium cloud. |
| ↳                   | UraniumEnemyDamageRate          |      2f       | The rate per second to damage enemies. |
| **Break Warning**   |
| &#124;              | WarningVolume                   |     0.35f     | The volume of the warning sound. Set to 0 to disable. |
| &#124;              | ShowWarningVisual               |     true      | Whether to momentarily show a red glow on the nuke when the break warning triggers. |
| ↳                   | WarningCameraShakeStrength      |     1.5f      | The intensity of the warning camera shake. |
| **Items**           |
| ↳                   | IndestructibleDroneBatteryDrain |      2f       | The amount at which to accelerate the Indestructible Drone's battery drain when attached to the Nuke. Set to 0 to disable. |
| **Debug**           |
| ↳                   | EnableDebug                     |     false     | The debug logging level to use. Keep this disabled for normal gameplay |

</details>

## ⚠️ Compatibility

No known incompatibilities.

<details><summary>Click to expand the list of methods patched by this mod:</summary>

- `ItemDroneIndestructible.Update`

</details>

## ❤️ Acknowledgements
- [Zehs](https://thunderstore.io/c/repo/p/Zehs/) for creating and maintaining [REPOLib](https://thunderstore.io/c/repo/p/Zehs/REPOLib/);
- [TitanVortex](https://thunderstore.io/c/repo/p/TitanVortex/) for the original [BigNuke](https://thunderstore.io/c/repo/p/TitanVortex/BigNuke/) mod which was one of my favorites, and of which this mod is mostly inspired by;
- [EvryFlare](https://linktr.ee/evryflare), [OrigamiCoder](https://thunderstore.io/c/repo/p/OrigamiCoder/), [Vippy](https://thunderstore.io/c/repo/p/Vippy/) and Endershade for playtesting;
- Skrillex and DJ Smokey for more goofy inspiration 🔥

Thank you for playing with this mod! \
Please report any issues to the [Discord Thread](https://discord.com/channels/1344557689979670578/1445947890940903514). Suggestions are also welcome!
