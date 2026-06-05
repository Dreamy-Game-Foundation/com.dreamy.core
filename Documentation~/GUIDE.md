# Dreamy Core Guide

`com.dreamy.core` is the low-level foundation package. It should stay small, stable, and dependency-light.

## What Belongs In Core

- `ServiceLocator`
- `MyEventBus<T>` and `IEvent`
- `BindableProperty<T>`
- `StateMachine`
- Singleton base classes
- `AppLifecycle`
- `AppTickService` and `ITickable`
- `DreamyLog`
- Generic extensions that do not pull optional packages

## What Does Not Belong In Core

- Pooling implementation: use LeanPool in the game template or a gameplay package.
- Connectivity checks: put project policy in the game template or a networking package.
- UniTask helpers: put async UI/data helpers in the package that already needs UniTask.
- DOTween helpers: put tween presets in `com.dreamy.ui` or the game template.
- SDK adapters: put Ads/IAP/Firebase adapters in dedicated packages.

## ServiceLocator

Register concrete services behind interfaces in the game composition root.

```csharp
ServiceLocator.Register<IAudioService>(new JsamAudioService());
var audio = ServiceLocator.Get<IAudioService>();
ServiceLocator.Unregister<IAudioService>();
```

## EventBus

Use struct events for fire-and-forget communication.

```csharp
public struct GameStartEvent : IEvent
{
    public int Level;
}

private EventBinding<GameStartEvent> binding;

private void OnEnable()
{
    binding = new EventBinding<GameStartEvent>(OnGameStart);
    MyEventBus<GameStartEvent>.Register(binding);
}

private void OnDisable()
{
    MyEventBus<GameStartEvent>.Unregister(binding);
}
```

## BindableProperty

Use `BindableProperty<T>` for simple reactive state.

```csharp
private readonly BindableProperty<int> score = new(0);

private void Awake()
{
    score.RegisterWithInitValue(value => scoreText.text = value.ToString())
        .UnRegisterOnDestroy(gameObject);
}
```

## StateMachine

Use `StateMachine` for small MonoBehaviour state flows. If state machine usage grows into gameplay architecture, move it to `com.dreamy.gameplay`.

```csharp
public class IdleState : BaseState
{
    public override void OnEnter(IStateData data = null)
    {
        DreamyLog.Log("Idle");
    }
}
```

## Dependency Rule

`com.dreamy.core` should not depend on UniTask, LeanPool, DOTween, Addressables, DreamySDK, Firebase, or other optional packages.
