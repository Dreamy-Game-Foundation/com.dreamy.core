# com.dreamy.core

Foundation package for internal Dreamy Studio Unity projects.

This package is intended for private team use through Unity Package Manager Git URLs. OpenUPM is not required.

## Requirements

- Unity 6000.0+
- UniTask installed in the consuming project before this package

Unity does not reliably resolve Git URL dependencies declared inside another Git package. For internal team projects, add prerequisites directly to the consuming project's `Packages/manifest.json`, then add `com.dreamy.core`.

## Internal Install

Use a tagged private Git repo for stable imports:

```json
{
  "dependencies": {
    "com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
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
| `BindableProperty<T>` | `Dreamy.Core` | Reactive property with auto-cleanup |
| `TaskRunner` | `Dreamy.Core` | Sequential async task runner |
| `StateMachine` | `Dreamy.Core` | MonoBehaviour finite state machine |
| `GameObjectPool<T>` | `Dreamy.Core` | Unity ObjectPool wrapper for prefab components |
| `MonoSingleton<T>` | `Dreamy.Core` | Scene-local singleton |
| `LiveSingleton<T>` | `Dreamy.Core` | Auto-created DontDestroyOnLoad singleton |
| `RegulatorSingleton<T>` | `Dreamy.Core` | Duplicate-tolerant singleton |
| `AppLifecycle` | `Dreamy.Core` | Pause, focus, and quit events |
| `AppTickService` + `ITickable` | `Dreamy.Core` | Update loop for non-MonoBehaviour services |
| `DreamyLog` | `Dreamy.Core` | Conditional logger (`DREAMY_DEBUG`) |
| `ConnectivityService` | `Dreamy.Core` | Internet connectivity check |
| Extensions | `Dreamy.Core` | Component, Transform, List, Fade, Vector, Math, Number |

## Quick Start

```csharp
public class GameInstaller : MonoBehaviour
{
    private void Awake()
    {
        ServiceLocator.Register<IConnectivityService>(new ConnectivityService());
    }
}
```

```csharp
public struct CoinCollectedEvent : IEvent
{
    public int Amount;
}

private EventBinding<CoinCollectedEvent> binding;

private void OnEnable()
{
    binding = new EventBinding<CoinCollectedEvent>(OnCoinCollected);
    MyEventBus<CoinCollectedEvent>.Register(binding);
}

private void OnDisable()
{
    MyEventBus<CoinCollectedEvent>.Unregister(binding);
}

private void CollectCoin()
{
    MyEventBus<CoinCollectedEvent>.Raise(new CoinCollectedEvent { Amount = 10 });
}
```

```csharp
private readonly BindableProperty<int> score = new(0);

private void Awake()
{
    score.RegisterWithInitValue(value => scoreText.text = value.ToString())
        .UnRegisterOnDestroy(gameObject);
}
```
