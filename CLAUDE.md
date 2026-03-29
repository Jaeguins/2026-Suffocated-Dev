# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A 2D action game built with **Unity 6000.3.11f1** using the **Universal Render Pipeline (URP)**. The project is in early development, migrated from a Unity template.

## Unity Version & Key Packages

- **Unity:** 6000.3.11f1
- **Render Pipeline:** URP 2D (com.unity.render-pipelines.universal v17.3.0)
- **Input:** New Input System (com.unity.inputsystem v1.19.0)
- **Serialization:** Newtonsoft.Json (com.unity.nuget.newtonsoft-json v3.2.2)
- **2D Tools:** 2D Animation, Aseprite importer, Tilemap, SpriteShape

## Architecture

### System Initialization

`SystemHolder` is the central singleton manager, initialized automatically before any scene loads via `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]`. It owns and exposes all game subsystems. Access via `SystemHolder.Get()`.

### Save System

`GameSaveManager` (`Assets/Scripts/System/GameSave/`) handles JSON persistence to `Application.persistentDataPath`. It is generic — use `Write<T>()` and `Read<T>()` with a relative path. New saveable data types should be passed through `SystemHolder.SaveManager`.

### Input

Input bindings are defined in `Assets/InputSystem_Actions.inputactions`. The Player action map includes: Move, Look, Attack, Interact (hold), Crouch, Jump, Sprint, Previous, Next — supporting keyboard and gamepad.

## Scripts Location

All game code lives under `Assets/Scripts/`. System-level managers go in `Assets/Scripts/System/`.

## Development Notes

- **No automated test runner or build CLI** — use the Unity Editor for building and running tests via the Test Runner window (com.unity.test-framework is installed).
- Scenes: `SampleScene` is the main scene; `SceneTest` is for development/testing.
- New scenes should be created from the URP 2D scene template (`Assets/Settings/Scenes/URP2DSceneTemplate.unity`).
- `Assets/UIs/` is reserved for UI components (currently empty).
