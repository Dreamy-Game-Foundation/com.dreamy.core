# Changelog — com.dreamy.core

All notable changes to this package will be documented in this file.
Format based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [1.0.0] - 2026-06-04

### Added
- `ServiceLocator` — type-safe DI container with deferred callback support
- `MyEventBus<T>` — generic struct-based event bus with `SafeInvoke`
- `EventBinding<T>` — typed event binding token
- `BindableProperty<T>` — reactive property with `IUnRegister` auto-cleanup
- `TaskRunner` + `BaseTask` — sequential async task runner
- `MonoSingleton<T>` + `LiveSingleton<T>` — singleton patterns
- `AppLifecycle` — centralized OnPause / OnFocus / OnQuit events
- `AppTickService` + `ITickable` — Update loop for non-MonoBehaviour services
- `DreamyLog` — conditional logger (`DREAMY_DEBUG` define)
- `IConnectivityService` + `ConnectivityService` — real internet check via ping
- Extensions: `ComponentExtensions`, `TransformExtension`, `ListExtensions`, `FadeExtensions`, `VectorExtension`, `MathUtility`, `NumberExtension`
