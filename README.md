# com.dreamy.core

Foundation package for internal Dreamy Studio Unity projects.

This package is intended for private team use through Unity Package Manager Git URLs. OpenUPM is not required.

## Requirements

- Unity 6000.0+

`com.dreamy.core` has no required third-party runtime package dependencies.

## Internal Install

Use a tagged private Git repo for stable imports:

```json
{
  "dependencies": {
    "com.dreamy.core": "https://github.com/Dreamy-Game-Foundation/com.dreamy.core.git#v1.1.0"
  }
}
```

For local development inside this project, keep the embedded package under `Packages/com.dreamy.core`.

## Modules

| Module | Namespace | Description |
|---|---|---|
| `ServiceLocator` | `Dreamy.Core` | Type-safe service registry with deferred callbacks |
| `MyEventBus<T>` | `Dreamy.Core` | Struct event bus |
| `StateMachine` | `Dreamy.Core` | MonoBehaviour finite state machine |
| `MonoSingleton<T>` | `Dreamy.Core` | Scene-local singleton |
| `LiveSingleton<T>` | `Dreamy.Core` | Auto-created DontDestroyOnLoad singleton |
| `RegulatorSingleton<T>` | `Dreamy.Core` | Duplicate-tolerant singleton |
| `AppLifecycle` | `Dreamy.Core` | Pause, focus, and quit events |
| `AppTickService` + `ITickable` | `Dreamy.Core` | Update loop for non-MonoBehaviour services |
| `DreamyLog` | `Dreamy.Core` | Conditional logger (`DREAMY_DEBUG`) |
| Extensions | `Dreamy.Core` | Component, Transform, List, Vector, Math, Number |

## Boundary Notes

`com.dreamy.core` should stay small and stable. New systems should only be added here when they are foundation primitives needed by multiple Dreamy packages or multiple shipped games.

- Pooling should use LeanPool in the game template or a dedicated gameplay package, not core.
- Connectivity checks belong in the game template or a networking package because they contain policy choices such as URL, interval, and timeout.
- UniTask-based async helpers should live in UI/data/template packages that already need UniTask.

The game template should own default service registration, SDK wiring, manifest examples, scene structure, and sample implementation.
