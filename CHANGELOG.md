# Changelog - com.dreamy.core

All notable changes to this package will be documented in this file.
Format based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [1.1.0] - 2026-06-05

### Added

- `StateMachine`, `BaseState`, `IState`, `IStateData`, and `NullState`
- Core demo sample scripts under `Samples~/CoreDemo`

### Changed

- Documented internal Git URL installation flow for team projects
- Marked package as an internal library package
- Removed UniTask as a core dependency

### Removed

- `TaskRunner` and `BaseTask`
- `IConnectivityService`, `ConnectivityService`, and `ConnectivityChangedEvent`
- `GameObjectPool<T>`, `IPoolable`, and `PoolableMonoBehaviour`
- UniTask-based `FadeExtensions`

## [1.0.0] - 2026-06-04

### Added

- `ServiceLocator` - type-safe DI container with deferred callback support
- `MyEventBus<T>` - generic struct-based event bus
- `EventBinding<T>` - typed event binding token
- `BindableProperty<T>` - reactive property with `IUnRegister` auto-cleanup
- `MonoSingleton<T>` + `LiveSingleton<T>` - singleton patterns
- `AppLifecycle` - centralized OnPause / OnFocus / OnQuit events
- `AppTickService` + `ITickable` - Update loop for non-MonoBehaviour services
- `DreamyLog` - conditional logger (`DREAMY_DEBUG` define)
- Extensions: `ComponentExtensions`, `TransformExtension`, `ListExtensions`, `VectorExtension`, `MathUtility`, `NumberExtension`
