# Pokémon: Survival Depths

![Unity](https://img.shields.io/badge/Unity-2022.x%20%2F%206.x-black?style=flat&logo=unity)
![C#](https://img.shields.io/badge/C%23-Unity-blue?style=flat&logo=c-sharp)
![Status](https://img.shields.io/badge/Status-Coursework_Completed-green)
![License](https://img.shields.io/badge/License-Academic-lightgrey)

> A Pokémon-themed roguelike dungeon crawler built in Unity for the COMP3218 Game Design and Development module. The game leverages automated combat mechanics, procedural level components, and dynamic choice-driven upgrade loops.

---

## Gameplay Overview

**Pokémon: Survival Depths** places players in a high-stakes, wave-based survival arena. The core gameplay strategy revolves entirely around tactical movement, precise positioning, and optimized environmental awareness.

<img width="1297" height="841" alt="image" src="https://github.com/user-attachments/assets/45be2147-54dd-4cd0-8ebc-cf447a9c4ece" />

### Core Loop Mechanics:
*   **Automatic Combat:** Attack execution triggers automatically on set intervals or upon close enemy contact, allowing players to focus intensely on dodging projectiles and spatial tracking.
*   **Procedural Design Elements:** Enemy spawns, level structures, and reward selections utilize procedural distribution rules rather than human placement.
*   **Self-Contained Runs:** While the progression loop features a definitive end wave and map milestones rather than an endless format, dying resets temporary character buffs back to the run's baseline status.
*   **Strategic Retrieval:** Dying inside a specific map resets only the upgrades earned within that environment, preserving the core builds secured from previously completed tiers.

---

## Playable Characters

Players choose their starting Pokémon during the initial character selection phase, each possessing a signature utility behavior designed to counter specific scaling enemy crowds.

| Pokémon | Base Archetype | Unique Ability / Tactical Trait |
| :--- | :--- | :--- |
| **Squirtle** | Crowd Control | Inflicts a localized crowd-control status slowing enemy movement vectors upon hit. |
| **Charmander** | Damage over Time | Applies a stackable burn modifier dealing persistent damage-over-time metrics. |
| **Pikachu** | Area of Effect | Chain-lightning functionality that jumps across multiple nearby targets upon contact. |

---

## System Architecture & Balancing

### The Progression Curve
Character pacing balances against environmental difficulty scaling via an exponential experience curve. The baseline required experience points ($XP$) to advance to the next level is governed mathematically by the formula:

$$XP_{\text{required}} = 10 \times \text{Level}^{1.5}$$

The calibrated exponent of $1.5$ guarantees that leveling intervals remain engaging without causing rapid endgame power spikes or tedious mid-game progression bottlenecks. Upon filling the threshold bar, the engine fully pauses active background combat logic to let players safely select one of three randomized upgrades.

### Biome Modifier Scaling
The environment features structural difficulty tiers, translating abstract design principles into concrete engine-enforced hazards:

| Difficulty Tier | Biome Archetype | Active System Modifiers / Traps |
| :--- | :--- | :--- |
| **Easy** | Forest | Baseline settings; introductory wave setups without active environmental traps. |
| **Medium** | Desert | Global scaling applied: Enemy health values scale up, and basic unit velocities increase. |

---

## How to Play

1. Download the latest pre-compiled build archive from the [**GitHub Releases**](https://github.com/TJKNlghT/Pokemon-Survival-Depths/releases) page.
2. Extract the downloaded `.zip` folder fully to your local machine.
3. Launch the game executable (e.g., `Pokemon_Survival_Depths.exe`) inside the uncompressed directory. No installation processes are required.

---

## Task Allocation & Contribution Sheet

This framework was co-developed as an academic project. The comprehensive division of workload was formally tracked and submitted in the official `COMP3218 - CW2 - Student Task Allocation Sheet.pdf`:

| Task Category | Specific Description | Wong Jin Xuan (%) | Ng Kin Yew Dexter (%) | Key Technical Challenges Resolved |
| :--- | :--- | :---: | :---: | :--- |
| **A. Quality** | Presentation, Graphics, Asset Pipeline & Info Design | **80%** | 20% | Handled licensing limits on premium asset stores; optimized 2D layout framing. |
| **B. Fulfilling Brief** | Level Design, Traps, Goals, Risks & Audio Management | 50% | 50% | Balanced dynamic unit drop rates (100% XP drop, 25-30% Potion chance). |
| **B. Fulfilling Brief** | Interactive Control Systems & Game Play Tutorial Layout | 50% | 50% | Fixed Unity interface hierarchy compilation conflicts regarding Prefab UI panels. |
| **C. Design Element** | Core Dynamics, Engine State Linking & Integration | 30% | **70%** | Resolved entity state transitions; handled system-level Unity project overwrites. |
| **D. Feedback Work** | System UI Elements, Audio FX Hooks, Multi-Map Framework | 50% | 50% | Coded automated script optimizations (projectile lifespans) to eliminate entity lag. |

---

## Technical Limitations & Known Exploits

Because this system was developed within constrained academic parameters, several known engine and design quirks exist within the current build:

*   **Stat State Desynchronization:** If a player achieves a high level within Map 2 and triggers a retry condition, the system sets their active baseline back to Level 1 while keeping Map 1 modifiers active.
*   **NavMesh Geometry Exploit:** The pathfinding logic fails to parse specific non-walkable tile bounds correctly. If a player shifts their character coordinate transform onto black obstacle layers, enemy AI paths fall into an infinite idle state loop.
*   **Boss Special Actions:** While bosses feature complete introductory visual cuts, they utilize standard high-stat attack actions rather than unique scripted spell patterns due to development time constraints.

---

## Planned Enhancements

*   **Cursor Directional Aiming:** Re-orienting combat patterns to rely explicitly on screen mouse pointers for targeted precision instead of snapping directly to directional keyboard inputs.
*   **Knockback Diminishing Returns:** Integrating absolute stun/knockback limits to prevent players from permanently locking high-tier bosses via high attack-speed configurations.
*   **Expanded Volumetric Tracking:** Implementing multi-floor layout evaluation patterns, variable camera tracking bounds, and an expanded inventory statistics database ($\text{Armor}$, $\text{Crit Rate}$, $\text{Pierce}$).

---

## Course Integration
*   **Module:** COMP3218 – Game Design and Development (Coursework 1)
*   **Institution:** University of Southampton Malaysia (UoSM)
