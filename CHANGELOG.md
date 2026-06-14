# Changelog - com.dreamy.core

All notable changes to this package will be documented in this file.
Format based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [Unreleased]

### Removed

- `BindableProperty<T>`, `IUnRegister`, and automatic unregister helpers

## [1.1.2] - 2026-06-15

### Fixed

- Made nested event raises defer binding mutations until the outermost raise completes
- Reset static service state for play mode sessions without domain reload
- Made deferred service callbacks and AppTick listeners isolate exceptions
- Deferred AppTick registration changes while the tick list is being iterated

## [1.1.1] - 2026-06-07

### Fixed

- Added missing metadata for the StateMachine folder
- Released the package after removing obsolete Pool, Connectivity, and TaskRunner modules

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
