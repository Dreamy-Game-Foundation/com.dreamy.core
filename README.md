# Dreamy Core

Package thuộc Dreamy Game Studio. Hướng dẫn dưới đây mô tả cấu trúc, cách cài vào project và tích hợp ở root/scene.

## Cài package

Dùng Unity 6000.0 trở lên. Sandbox đã tham chiếu package bằng `file:../LocalPackages/com.dreamy.core`. Project khác dùng Package Manager > + > Install package from disk và chọn package.json, hoặc Git URL của repository nội bộ. Cài cả dependency Dreamy/Git vào manifest của game; version dependency không tự cấu hình registry riêng.

Dependency trực tiếp theo package.json:

Không khai báo dependency trực tiếp; kiểm tra reference asmdef và prefab bên dưới.

## Cấu trúc và asmdef

| Assembly | Reference | Phạm vi |
| --- | --- | --- |
| `Dreamy.Core.Runtime` |  | Runtime |

Trong asmdef của game, thêm assembly chứa API trực tiếp sử dụng. Code bootstrap reference thêm Core/DataConfig/Datasave/Economy theo nhu cầu; code async reference UniTask. Code gọi type sample reference assembly sample. Giữ Editor reference trong asmdef Editor-only.

## Cách dùng ở root

Core cung cấp ServiceLocator, event bus, singleton, state machine, lifecycle, tick và logger. Runtime/ServiceLocator quản lý service; EventBus xử lý sự kiện; Lifecycle nối pause/focus/quit và tick; Singleton và StateMachine là primitive cho component. Core không tự cài service nghiệp vụ.

Trong GameInstaller, tạo service của game rồi đăng ký theo interface. Code tiêu thụ lấy service sau khi bootstrap hoàn tất:

```csharp
using Dreamy.Core;

ServiceLocator.Register<IGameService>(gameService);
var service = ServiceLocator.Get<IGameService>();
// Root thực hiện khi teardown:
ServiceLocator.Unregister<IGameService>();
```

IGameService/gameService là interface và instance do game tự định nghĩa. Root sở hữu instance, dispose nếu cần và unregister lúc kết thúc. View không nên tự tạo lại service. Subscription event phải được tháo theo lifecycle của subscriber.

MonoSingleton dùng cho singleton theo scene; LiveSingleton dành cho object tồn tại qua scene. DreamyLog dùng symbol DREAMY_DEBUG. Core cung cấp contract pooling; backend pooling, SDK, network policy và scene flow thuộc game hoặc package chuyên biệt.
## Sample

Import **Core Demo** qua Package Manager để xem ServiceLocator, EventBus, StateMachine và pooling với backend do host inject. Xem `Samples~/CoreDemo/README.md` để cài component pooling.

## Addressables

Package này không có panel cần đăng ký vào Addressables Group. Việc đặt address của prefab/asset thuộc game hoặc package UI/Assets; không dùng Addressables thay bước đăng ký service/config/save.

## Pooling (1.2.0)

`Dreamy.Core.Pooling.IPoolService` là contract dùng chung; Core không có backend
và không tự đăng ký service. Template cung cấp `LeanPoolService` và `Pools` facade.
Migration: đổi import `Dreamy.Template.Pooling.IPoolService` sang
`Dreamy.Core.Pooling.IPoolService`. `PooledBehaviour` vẫn dùng Lean IPoolable ở Template.

Composition root tạo backend, đăng ký `ServiceLocator.Register<IPoolService>(pool)`
rồi inject `IPoolService` cho consumer. Root shutdown consumer, unregister đúng
instance do mình sở hữu và dispose backend. Không trộn spawn/despawn của service
với API static LeanPool. Mỗi prefab chỉ có một owner Lean pool.

```csharp
using Dreamy.Core.Pooling;
using UnityEngine;

// pool: IPoolService được inject; các prefab/parent/target do consumer sở hữu.
var clone = pool.Spawn(prefab);
var world = pool.Spawn(prefab, position, rotation, parent);
var local = pool.SpawnLocal(prefab, parent, localPosition,
    Quaternion.identity, Vector3.one);
var atTarget = pool.SpawnAt(prefab, target);
var component = pool.Spawn<ParticleSystem>(prefab);
var label = pool.SpawnUi(labelPrefab, canvasTransform, new Vector2(20, 40));
var options = PoolSpawnOptions.World(position, rotation, parent);
options.LocalScale = Vector3.one * 2;
options.Name = "RewardBurst";
options.Configure = go => go.GetComponent<ParticleSystem>().Play();
if (pool.TrySpawn(prefab, options, out var reward))
    pool.Despawn(reward, delay: 2f);
```

`PoolSpawnOptions` mặc định dùng world space; position/rotation/scale không đặt
sẽ lấy từ prefab. `Spawn(prefab)` và overload chỉ truyền parent dùng local pose
của prefab. Scale luôn là local scale và được reset mỗi lần reuse. `SpawnLocal`
mặc định local position zero và rotation identity. `SpawnUi` reset local pose,
scale và anchored position; các field layout khác do prefab/consumer quản lý.

Transform được áp dụng trước OnSpawn; `Configure` chạy sau activation/OnSpawn.
Không dùng callback này để cấu hình dữ liệu mà OnEnable/OnSpawn phải đọc sớm.
Nếu callback ném lỗi, backend trả clone về pool rồi propagate exception.
Component overload yêu cầu component trên root prefab; component thiếu báo lỗi
mà không chiếm capacity. `TrySpawn` trả false khi hết capacity; lỗi input/callback
vẫn throw. `Spawn` trả null khi hết capacity; backend không recycle clone đang dùng.

`Preload(prefab, count, capacity, persistent)` dùng count làm target tổng clone;
capacity 0 là unlimited. Không giảm capacity dưới số clone hiện có. Pool tạo lazily
sống theo service; `Preload(..., persistent: false)` đặt pool trong active scene,
`true` đặt pool dưới service root DontDestroyOnLoad. Active clone vẫn theo scene
của parent/current scene. Khi scene pool bị unload, backend dọn cả clone còn sống
ở scene khác. `Despawn` delay dùng scaled time; return ngay hủy timer cũ. Return
null/clone đã trả là no-op; foreign object bị từ chối. `DespawnAll` chỉ dọn phạm vi
service. Dispose idempotent, hủy timer và destroy toàn bộ pool/clone do nó sở hữu.
