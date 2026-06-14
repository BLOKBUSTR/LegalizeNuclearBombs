# Changelog

### 3.1.0

- Added a new config setting `IndestructibleDroneBatteryDrain`, which accelerates the rate at which the Indestructible Drone's battery will drain when attached to the Nuke. The battery drain is twice as fast by default.
- Renamed config entry `ExplosionUraniumCloud` → `SpawnUraniumCloud`.

### 3.0.1

- Fixed oversight where config `ExplosionUraniumCloud` was unused. The uranium cloud should now spawn if the host has the setting enabled.
- Upgraded REPOLib version.

### 3.0.0

- Updated for R.E.P.O. 0.4.4+.
- Added a configurable delay before the explosion, after the nuke has taken its last hit.
  - The buildup sound can be replaced with anything of your choosing with loaforcsSoundAPI, as long as it's under 10 seconds. Adjust `ExplosionDelayTime` to fit your custom sound.
  - Also includes particle visuals adopted from the cosmetic boxes, which do not play if Photosensitivity is enabled.
  - Added config options `ExplosionDelayTime`, `ExplosionDelayVolume`, `ExplosionDelayParticles`, and `ExplosionDelayCameraGlitch`.
- Added a uranium cloud that spawns upon explosion. Added a config entry `ExplosionUraniumCloud` to enable/disable it. I want to experiment with more customization for this in the future.
- Fixed a few networking issues:
  - Oversight where explosion strength and player damage were not synced; now added proper networking.
  - The nuke did not destroy itself properly on clients upon explosion.
- Reorganized and optimized some logic.
- Removed config `PlayWarningSound`, since `WarningVolume` can already be used to silence the sound.

### 2.0.0

- Upgraded BepInEx dependency.
- Implemented an emissive visual warning when the nuke has only one hit remaining.
- The nuke now completely destroys itself upon explosion to prevent the explosion from getting spammed every update, especially on lower strengths.
- Changed config entry `HitSensitivity` to use an enum value to make it more readable and intuitive, particularly with REPOConfig.
- README changes:
  - Updated the Configuration section to my new table style;
  - Updated Discord URL to link to this mod's own dedicated thread.

### 1.0.0

- Initial release 🎉
