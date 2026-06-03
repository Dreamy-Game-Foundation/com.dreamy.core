# com.dreamy.core

Foundation package cho Dreamy Studio Unity projects.

## Requires
- Unity 6000.0+
- UniTask

## Modules

| Module | Namespace | Mô tả |
|---|---|---|
| `ServiceLocator` | `Dreamy.Core` | Type-safe DI, deferred callback |
| `MyEventBus<T>` | `Dreamy.Core` | Struct event bus, zero allocation |
| `BindableProperty<T>` | `Dreamy.Core` | Reactive property + auto-cleanup |
| `TaskRunner` | `Dreamy.Core` | Sequential async task runner |
| `MonoSingleton<T>` | `Dreamy.Core` | Singleton MonoBehaviour |
| `LiveSingleton<T>` | `Dreamy.Core` | Auto-create singleton |
| `AppLifecycle` | `Dreamy.Core` | OnPause / OnFocus / OnQuit events |
| `AppTickService` + `ITickable` | `Dreamy.Core` | Update loop cho non-MonoBehaviour |
| `DreamyLog` | `Dreamy.Core` | Conditional logger (`DREAMY_DEBUG`) |
| `ConnectivityService` | `Dreamy.Core` | Real internet check via ping |
| Extensions | `Dreamy.Core` | Component, Transform, List, Fade, Vector, Math, Number |

## Quick Start

```csharp
// GameInstaller.cs
public class GameInstaller : MonoBehaviour
{
    void Awake()
    {
        ServiceLocator.Register<IConnectivityService>(new ConnectivityService());
        // register other services...
    }
}
```

```csharp
// Define event
public struct CoinCollectedEvent : IEvent { public int Amount; }

// Subscribe
EventBinding<CoinCollectedEvent> _binding;
void OnEnable()  => MyEventBus<CoinCollectedEvent>.Register(_binding = new EventBinding<CoinCollectedEvent>(OnCoinCollected));
void OnDisable() => MyEventBus<CoinCollectedEvent>.Unregister(_binding);

// Raise
MyEventBus<CoinCollectedEvent>.Raise(new CoinCollectedEvent { Amount = 10 });
```

```csharp
// BindableProperty
BindableProperty<int> _score = new(0);
_score.RegisterWithInitValue(v => scoreText.text = v.ToString())
      .UnRegisterOnDestroy(gameObject);
_score.Value = 100; // fires listener automatically
```
