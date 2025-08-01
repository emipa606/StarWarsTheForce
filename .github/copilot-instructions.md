# GitHub Copilot Instructions for Modding in RimWorld: Star Wars - The Force (Continued)

## Mod Overview and Purpose
The "Star Wars - The Force (Continued)" mod aims to enrich the gameplay experience of RimWorld by integrating the iconic elements of the Star Wars universe. The mod introduces an RPG-style leveling system, alignment mechanics, and a variety of force powers, allowing players to develop their characters as Jedi or Sith. This mod continues the original work by Xen and Jecrell, and is no longer being officially updated, but it is open for community enhancement under the given permissions.

## Key Features and Systems
- **RPG-Leveling System**: Gain experience and level up in force powers, enhancing the capabilities of force-sensitive characters.
- **Alignment Mechanics**: Characters can align with the light or dark side of the force, which influences which powers they can access.
- **Force Powers**: A wide range of abilities from both light and dark sides, including iconic powers like Force Push and Force Choke.
- **Force Pools and Meditation**: Manage a character's force energy with force pools and meditation practices.

## Coding Patterns and Conventions
- **Class Design**: The mod uses well-defined classes named clearly by their functionality, such as `CompForceUser`, `ForceAbility`, and `DamageWorker_*` classes for specific force powers.
- **Inheritance**: Many classes extend RimWorld base classes (e.g., `JobDriver_ForceMeditation`, `DamageWorker_*`) to incorporate unique functionalities.
- **Modularization**: Methods are segmented into logical groupings, handling specific tasks such as alignment updates, leveling up powers, and managing force pools.

## XML Integration
- Utilizes XML for defining game object attributes like items, abilities, and pawns.
- The close relationship between XML and C# is essential, as C# scripts often process and modify XML data for dynamic game interactions.

## Harmony Patching
- Extensive use of Harmony for patching existing RimWorld functions to augment the base behavior with features from this mod.
- Files such as `HarmonyPatches_ForceShield.cs` and `HarmonyPatches_Lightsaber.cs` contain methods to inject or override base game functionalities to accommodate the sophisticated mechanics of force powers.

## Suggestions for Copilot
1. **Force Power Implementation**: When creating new force powers, use the existing pattern established in `ForceAbility` and `ForcePower` classes. Leverage the `DamageWorker_*` structure for effects and calculations.
2. **Leveling and Alignment**: For adding new alignment-based effects or traits, ensure alignment updates in `CompForceUser` are consistent and intuitive.
3. **Patching and Compatibility**: Utilize and expand upon the harmony patches framework for seamless integration with other mods. Consider potential conflicts and prepare compatibility methods.
4. **User Experience Enhancements**: Enhance the UI elements like `ITab_Pawn_Force` and `Gizmo_HediffShieldStatus` to provide a clearer understanding of a character’s force status and abilities.
5. **Expand XML Definitions**: When adding new items or powers, ensure XML files are updated accordingly, and C# class definitions can parse and apply XML data efficiently.
6. **Optimization**: Profile existing methods related to force calculation and power effects for performance, especially in a large number of pawns with force abilities.

By following these instructions, you can effectively contribute new features and improvements to this mod, maintaining a cohesive development style consistent with the original structure.
