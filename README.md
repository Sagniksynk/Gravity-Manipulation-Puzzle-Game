# 🌌 Gravity Shift Protocol

![Unity](https://img.shields.io/badge/Unity-6%20%2F%202022.3%2B-black?style=flat&logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-blue)
![Architecture](https://img.shields.io/badge/Architecture-Event--Driven-orange)
![Status](https://img.shields.io/badge/Status-Completed-success)

**Gravity Shift Protocol** is a third-person sci-fi puzzle platformer developed in Unity. The core mechanic revolves around **6-Directional Gravity Manipulation**, allowing players to walk on walls and ceilings to solve environmental puzzles.

This project demonstrates **high-performance C# scripting**, utilizing advanced design patterns to ensure decoupled architecture and minimal Garbage Collection overhead.

## 🎮 Gameplay Features

* **Gravity Manipulation System**:
    * Walk on **any surface** (Floor, Walls, Ceiling).
    * **Holographic Preview**: Visualize where gravity will shift before committing.
    * **Head-Pivot Vaulting**: Unique movement mechanic where gravity shifts pivot around the character's head for fluid transitions.
* **Third-Person Movement**: Camera-relative movement with smooth rotation and animation blending.
* **Objectives**:
    * Collect all **Energy Cubes** scattered across the level.
    * **Time Attack**: Complete the level within 2 minutes.
* **Lose Conditions**:
    * Falling into the void (Raycast-based free fall detection).
    * Running out of time.
* **In-Game Tutorial**: Interactive, state-machine based guide teaching movement mechanics.

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

## 🛠️ Technical Architecture & Optimizations

This project focuses on **Clean Code** principles and **Performance Optimization**.

### 1. Design Patterns
* **Observer Pattern (Events)**: A static `GameEvents` class handles communication between systems.
    * *Benefit*: Decouples `Collectible.cs` from `GameManager.cs`. Collectibles simply fire an event when picked up, without needing references to the Game Loop.
* **Singleton Pattern**: Implemented on `GameManager` with duplicate-destruction logic to ensure a single source of truth for game state.
* **Dirty Flag Pattern**: UI elements (Timer, Score) only update when data actually changes, rather than rewriting string text every frame.

### 2. Performance Optimizations
* **Garbage Collection (GC) Reduction**: 
    * Eliminated string allocation in `Update()` loops.
    * Cached references to all major components (`Rigidbody`, `Transform`, `Animator`) to avoid `GetComponent` calls during runtime.
* **Animator Hashing**: 
    * Replaced string-based references (e.g., `anim.SetBool("IsGrounded")`) with `Animator.StringToHash`. This significantly reduces CPU overhead during animation updates.
* **Physics Throttling**:
    * Heavy calculations, such as the "Free Fall Raycast Check," are offloaded to Coroutines running at reduced intervals (5Hz) rather than every frame (60Hz+), saving valuable CPU cycles.

### 3. Core Scripts Overview
* **`GameEvents.cs`**: Pure C# static class managing Actions/Events for messaging.
* **`GravityManager.cs`**: Handles physics calculation for custom gravity directions, holographic previews, and the smooth lerping of player rotation (Quaternion math).
* **`PlayerController.cs`**: Physics-based movement controller that ignores Unity's default gravity, using calculated vectors relative to the custom camera perspective.
* **`ThirdPersonOrbitCam.cs`**: Smart camera system that aligns with the player's local "Up" vector to prevent gimbal lock when walking on ceilings.
* **`TutorialManager.cs`**: A coroutine-based state machine that guides the player through inputs and waits for specific triggers before advancing.

## 🚀 How to Play (Installation)

1.  **Download Build**: Go to the [Releases](../../releases) page and download the version for your OS (Windows/Mac).
2.  **Run**: Extract the zip file and run `GravityShift.exe`.
3.  **Unity Editor**:
    * Clone this repo: `git clone https://github.com/Sagniksynk/Gravity-Manipulation-Puzzle-Game.git`
    * Open in Unity Hub (Unity 6 or 2022.3 LTS).
    * Open `Assets/Scenes/SampleScene`.
    * Press Play.

## 📄 License
This project is for educational purposes as part of a Unity Developer assessment.

---
*Developed by [Sagnik Dasgupta]*
