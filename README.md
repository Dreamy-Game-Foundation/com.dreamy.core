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

MonoSingleton dùng cho singleton theo scene; LiveSingleton dành cho object tồn tại qua scene. DreamyLog dùng symbol DREAMY_DEBUG. Pooling, SDK, network policy và scene flow thuộc game hoặc package chuyên biệt.
## Sample

Manifest hiện không khai báo sample để import qua Package Manager.

## Addressables

Package này không có panel cần đăng ký vào Addressables Group. Việc đặt address của prefab/asset thuộc game hoặc package UI/Assets; không dùng Addressables thay bước đăng ký service/config/save.
