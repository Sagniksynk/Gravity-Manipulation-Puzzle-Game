# 🌌 Gravity Shift Protocol

![Unity](https://img.shields.io/badge/Unity-6%20%2F%202022.3%2B-black?style=flat&logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-blue)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Mac-lightgrey)
![Status](https://img.shields.io/badge/Status-Completed-success)

**Gravity Shift Protocol** is a third-person sci-fi puzzle platformer developed in Unity. The core mechanic revolves around **6-Directional Gravity Manipulation**, allowing players to walk on walls and ceilings to solve environmental puzzles.

## 🎮 Gameplay Features

* **Gravity Manipulation System**:
    * Walk on **any surface** (Floor, Walls, Ceiling).
    * **Holographic Preview**: Visualize where gravity will shift before committing.
    * **Head-Pivot Vaulting**: Unique movement mechanic where gravity shifts pivot around the character's head for fluid transitions.
    * **6-DOF Control**: Access all 4 walls + Ceiling (Shift+Up) + Floor Reset (Shift+Down).
* **Third-Person Movement**: Camera-relative movement with smooth rotation and animation blending (Idle -> Run).
* **Objectives**:
    * Collect all **Energy Cubes** scattered across the level.
    * **Time Attack**: Complete the level within 2 minutes.
* **Lose Conditions**:
    * Falling into the void (Free fall detection).
    * Running out of time.
* **In-Game Tutorial**: Interactive step-by-step guide teaching movement and gravity mechanics.

## 🕹️ Controls

| Key | Action |
| :--- | :--- |
| **W, A, S, D** | Move Character |
| **Space** | Jump |
| **Mouse** | Look Around (Orbit Camera) |
| **Arrow Keys** | **Preview Gravity** (Front, Back, Left, Right Walls) |
| **Shift + Up Arrow** | **Preview Ceiling** (180° Flip) |
| **Shift + Down Arrow** | **Preview Floor** (Reset Gravity) |
| **Enter** | **Execute Gravity Shift** |

## 🛠️ Technical Implementation

### Core Scripts
* **`GravityManager.cs`**: Handles the physics calculation for custom gravity directions, holographic preview logic, and the smooth lerping of the player's rotation during gravity shifts. It uses a custom "Head Pivot" system to vault the player onto new surfaces.
* **`PlayerController.cs`**: Custom physics-based movement controller that ignores Unity's default gravity and calculates movement vectors relative to the camera's perspective on the current surface.
* **`ThirdPersonOrbitCam.cs`**: A smart camera system that orbits the player, aligns with the player's local "Up" vector (so the camera flips with you), and includes collision detection to prevent clipping through walls.
* **`GameManager.cs`**: Singleton class managing the game loop, timer, UI updates (TextMeshPro), and win/loss states.

### Physics Logic
Instead of using Unity's global `Physics.gravity`, this project applies a custom force via `Rigidbody.AddForce` (or `linearVelocity` in Unity 6). This allows for dynamic runtime gravity changes without affecting global physics settings for other objects.

## 🚀 How to Play (Installation)

1.  **Download Build**: Go to the [Releases](../../releases) page and download the version for your OS (Windows/Mac).
2.  **Run**: Extract the zip file and run `GravityShift.exe` (Windows) or the `.app` file (Mac).
3.  **Unity Editor**:
    * Clone this repo: `git clone https://github.com/YourUsername/Gravity-Shift-Protocol.git`
    * Open in Unity Hub (Version 2022.3 or Unity 6 recommended).
    * Open `Assets/Scenes/GameLevel`.
    * Press Play.

## 📸 Screenshots

*(Add screenshots of your game here: One standing on the floor, one standing on a wall/ceiling with the hologram visible)*

## 📄 License
This project is for educational purposes as part of a Unity Developer assessment.

---
*Developed by [Your Name]*
