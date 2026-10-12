# Core Demo

Import **Core Demo** from Package Manager. The scripts demonstrate ServiceLocator,
EventBus, state machines and the Core 1.2 pooling contract.

For pooling, add `PoolingDemo` to a GameObject, assign its prefab and optional
parent, then call `Initialize(poolService)` from the host composition root.
Wire `SpawnExample()` and `ReturnExample()` to buttons or call them from code.
The component returns its own instance when disabled; it never clears other pools.
Call `Shutdown()` before the host disposes the backend.

Core provides contracts and extensions. The host supplies an `IPoolService`
implementation, such as the Template's LeanPoolService. This sample has no
LeanPool dependency and does not create or register a global backend.
