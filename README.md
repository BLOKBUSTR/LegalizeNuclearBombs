# ☢️ LegalizeNuclearBombs ☢️

Adds a custom-made nuclear warhead as a Tall valuable, even bigger than before! Handle with the utmost caution!!

❗ **This mod must be installed on all clients.**

![Screenshot of the nuke valuable in-game](https://raw.githubusercontent.com/BLOKBUSTR/LegalizeNuclearBombs/refs/heads/master/Screenshot.jpg)

## 🔧 Configuration & Customization
This mod is highly configurable! Configs can be updated in-game with [REPOConfig](https://thunderstore.io/c/repo/p/nickklmao/REPOConfig/).

<details>
<summary>Click to expand config list:</summary>

| Category            | ConfigEntry                | Default Value | Description |
|---------------------|----------------------------|:-------------:|-|
| **Nuke**            |
| &#124;              | HitSensitivity             |    Medium     | The minimum impact strength that the nuke is sensitive to. |
| &#124;              | MaxHitCount                |       3       | The number of hits the nuke can take until it explodes. Set to 0 to disable and make it explode only when it loses all value (this will also disable the explosion delay!). |
| &#124;              | ExplosionStrength          |      15f      | The strength of the explosion. |
| &#124;              | PlayerDamage               |      200      | The amount of damage dealt to players. |
| &#124;              | EnemyDamage                |      400      | The amount of damage dealt to enemies. |
| &#124;              | ExplosionUraniumCloud      |     true      | Whether to spawn a uranium cloud upon explosion. |
| ↳                   | CameraShakeStrength        |      5f       | The intensity of the explosion camera shake. |
| **Explosion Delay** |
| &#124;              | ExplosionDelayTime         |      1f       | Time in seconds that the explosion will be delayed after the nuke has taken its last hit. Can be adjusted to match the length of a custom sound added with loaforcsSoundAPI, as long as it's under 10 seconds. Please do not change if using the default sound. |
| &#124;              | ExplosionDelayVolume       |     0.5f      | The volume of the explosion delay sound. |
| &#124;              | ExplosionDelayParticles    |     true      | Whether to play particle effects during the explosion delay. |
| ↳                   | ExplosionDelayCameraGlitch |     true      | Whether to play the camera glitch effect to players holding the nuke when its explosion delay begins. |
| **Break Warning**   |
| &#124;              | WarningVolume              |     0.35f     | The volume of the warning sound. Set to 0 to disable. |
| &#124;              | ShowWarningVisual          |     true      | Whether to momentarily show a red glow on the nuke when the break warning triggers. |
| ↳                   | WarningCameraShakeStrength |     1.5f      | The intensity of the warning camera shake. |
| **Debug**           |
| ↳                   | EnableDebug                |     false     | Whether to enable debug logging. Keep this disabled for normal gameplay |

</details>

## ❤️ Acknowledgements
- [Zehs](https://thunderstore.io/c/repo/p/Zehs/) for creating and maintaining [REPOLib](https://thunderstore.io/c/repo/p/Zehs/REPOLib/);
- [TitanVortex](https://thunderstore.io/c/repo/p/TitanVortex/) for the original [BigNuke](https://thunderstore.io/c/repo/p/TitanVortex/BigNuke/) mod which was one of my favorites, and of which this mod is mostly inspired by;
- [OrigamiCoder](https://thunderstore.io/c/repo/p/OrigamiCoder/), [Vippy](https://thunderstore.io/c/repo/p/Vippy/) and Endershade for playtesting;
- Skrillex and DJ Smokey for more goofy inspiration 🔥

Thank you for playing with this mod! \
Please report any issues to the [Discord Thread](https://discord.com/channels/1344557689979670578/1445947890940903514). Suggestions are also welcome!
