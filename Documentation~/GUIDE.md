# Dreamy Core — Hướng dẫn kiến thức mới

> Tài liệu này dành cho team đang quen với **Coroutine, Singleton, Resources.Load, đặt panel trực tiếp lên Scene**.
> Mỗi phần giải thích **tại sao thay đổi** và **cách dùng ngay**.

---

## Mục lục

1. [UniTask — Thay thế Coroutine](#1-unitask--thay-thế-coroutine)
2. [ServiceLocator — Thay thế Singleton](#2-servicelocator--thay-thế-singleton)
3. [MyEventBus — Giao tiếp không cần tham chiếu trực tiếp](#3-myeventbus--giao-tiếp-không-cần-tham-chiếu-trực-tiếp)
4. [BindableProperty — UI tự động cập nhật](#4-bindableproperty--ui-tự-động-cập-nhật)
5. [StateMachine — Thay thế if/else chồng chất](#5-statemachine--thay-thế-ifelse-chồng-chất)
6. [Object Pool — Thay thế Instantiate/Destroy](#6-object-pool--thay-thế-instantiatedestroy)
7. [Extension Methods — Viết code gọn hơn](#7-extension-methods--viết-code-gọn-hơn)
8. [Addressables — Thay thế Resources.Load](#8-addressables--thay-thế-resourcesload)
9. [WaitFor — Cached coroutine yield](#9-waitfor--cached-coroutine-yield)
10. [AppLifecycle & ITickable](#10-applifecycle--itickable)

---

## 1. UniTask — Thay thế Coroutine

### Tại sao?

| Coroutine | UniTask (async/await) |
|---|---|
| Không trả về giá trị trực tiếp | Trả về giá trị `await LoadAsync<T>()` |
| Khó xử lý lỗi (try/catch không hoạt động trong IEnumerator) | try/catch hoạt động bình thường |
| Không cancel được dễ dàng | CancellationToken hủy sạch |
| Allocate GC mỗi lần gọi | Zero-alloc với `UniTask.Yield()` |
| Không dùng được trong class không phải MonoBehaviour | Dùng được ở bất cứ đâu |

### So sánh nhanh

```csharp
// ❌ Coroutine cũ
IEnumerator LoadAndShow()
{
    yield return new WaitForSeconds(1f);
    var www = UnityWebRequest.Get(url);
    yield return www.SendWebRequest();
    panel.SetActive(true);
}
StartCoroutine(LoadAndShow());

// ✅ UniTask mới
async UniTaskVoid LoadAndShow()
{
    await UniTask.Delay(1000); // 1 giây
    var result = await SomeApiAsync();
    panel.SetActive(true);
}
LoadAndShow().Forget(); // gọi như fire-and-forget
```

### Các từ khóa chính

```csharp
// Chờ một frame
await UniTask.Yield();

// Chờ N giây (milliseconds)
await UniTask.Delay(1000);                       // 1 giây game time
await UniTask.Delay(1000, DelayType.Realtime);   // 1 giây real time (bỏ qua TimeScale)

// Chờ điều kiện
await UniTask.WaitUntil(() => isReady);
await UniTask.WaitWhile(() => isLoading);

// Chạy song song
await UniTask.WhenAll(TaskA(), TaskB(), TaskC());

// Chạy cái nào xong trước
await UniTask.WhenAny(TaskA(), TaskB());
```

### Quy tắc dùng UniTask

```csharp
// 1. Method async trả về UniTask (không cần Forget)
public async UniTask ShowPanelAsync()
{
    // có thể await từ nơi khác
}

// 2. Method "fire and forget" — không ai await
async UniTaskVoid AutoSaveLoop()
{
    while (true)
    {
        await UniTask.Delay(30000);
        SaveGame();
    }
}
// Gọi:
AutoSaveLoop().Forget();

// 3. Cancel khi GameObject bị Destroy (dùng GetCancellationTokenOnDestroy)
async UniTask DoWork(CancellationToken token)
{
    await UniTask.Delay(3000, cancellationToken: token);
    DoSomething();
}
// Trong MonoBehaviour:
DoWork(this.GetCancellationTokenOnDestroy()).Forget();
```

---

## 2. ServiceLocator — Thay thế Singleton

### Vấn đề với Singleton cũ

```csharp
// ❌ Vấn đề: coupling cứng — GameManager biết class cụ thể
AudioManager.Instance.PlaySound("click");
// Nếu muốn swap AudioManager → phải tìm-sửa khắp project
// Không test được (không thể mock)
```

### ServiceLocator là gì?

Một "bảng tra cứu" toàn cục — register service theo **interface**, lấy ra theo **interface**. Game code không bao giờ biết class cụ thể.

```csharp
// ✅ Đăng ký service khi khởi động (trong GameInstaller)
ServiceLocator.Register<IAudioService>(new FMODAudioService());

// ✅ Dùng ở bất cứ đâu — chỉ cần biết interface
var audio = ServiceLocator.Get<IAudioService>();
audio.Play("click");

// ✅ Swap sang implementation khác → chỉ sửa 1 chỗ duy nhất
ServiceLocator.Register<IAudioService>(new JSamAudioService()); // không cần sửa game code
```

### Deferred callback (chưa register vẫn dùng được)

```csharp
// Nếu AudioService chưa ready, callback sẽ được gọi ngay khi nó register
ServiceLocator.Get<IAudioService>(audio => audio.PlayBGM("main_menu"));
```

### Cấu trúc GameInstaller (bootstrap)

```csharp
public class GameInstaller : MonoBehaviour
{
    void Awake()
    {
        // Đăng ký theo thứ tự dependency
        ServiceLocator.Register<IConnectivityService>(new ConnectivityService());
        ServiceLocator.Register<IDatasaveService>(new DatasaveService());
        ServiceLocator.Register<IAudioService>(new JSamAudioService());
        ServiceLocator.Register<IAdService>(new MaxMediationAdapter());
        ServiceLocator.Register<IUIService>(new UIManager());
    }
}
```

---

## 3. MyEventBus — Giao tiếp không cần tham chiếu trực tiếp

### Vấn đề cũ

```csharp
// ❌ Player giữ tham chiếu trực tiếp đến UIManager, ScoreManager, AudioManager...
// → coupling cao, khó maintain
public class Player : MonoBehaviour
{
    [SerializeField] UIManager _ui;
    [SerializeField] ScoreManager _score;
    [SerializeField] AudioManager _audio;

    void Die()
    {
        _ui.ShowGameOver();
        _score.Reset();
        _audio.PlaySFX("die");
    }
}
```

### EventBus giải quyết thế nào?

Player chỉ **phát sự kiện**. UIManager, ScoreManager, AudioManager tự **lắng nghe** — không ai biết nhau.

```csharp
// 1. Định nghĩa event (struct = zero allocation)
public struct PlayerDiedEvent : IEvent
{
    public int FinalScore;
    public Vector3 Position;
}

// 2. Player chỉ raise event — không biết ai lắng nghe
public class Player : MonoBehaviour
{
    void Die()
    {
        MyEventBus<PlayerDiedEvent>.Raise(new PlayerDiedEvent
        {
            FinalScore = _score,
            Position   = transform.position
        });
    }
}

// 3. UIManager tự lắng nghe — không cần Player biết
public class UIManager : MonoBehaviour
{
    EventBinding<PlayerDiedEvent> _binding;

    void OnEnable()
        => MyEventBus<PlayerDiedEvent>.Register(
               _binding = new EventBinding<PlayerDiedEvent>(OnPlayerDied));

    void OnDisable()
        => MyEventBus<PlayerDiedEvent>.Unregister(_binding);

    void OnPlayerDied(PlayerDiedEvent e)
    {
        ShowGameOver(e.FinalScore);
    }
}
```

### Quy tắc đặt tên event

```csharp
// Dùng PascalCase + hậu tố "Event"
public struct CoinCollectedEvent  : IEvent { public int Amount; }
public struct LevelCompletedEvent : IEvent { public int Level; public float Time; }
public struct GamePausedEvent     : IEvent { public bool IsPaused; }
```

---

## 4. BindableProperty — UI tự động cập nhật

### Vấn đề cũ — UI cập nhật thủ công

```csharp
// ❌ Mỗi lần score thay đổi phải nhớ gọi UpdateUI()
_score += 10;
UpdateUI(); // quên gọi → bug!
```

### BindableProperty — UI tự phản ứng

```csharp
public class GameManager : MonoBehaviour
{
    // Khai báo reactive property
    public BindableProperty<int> Score { get; } = new(0);
    public BindableProperty<int> Lives { get; } = new(3);
}

// Trong UI — đăng ký 1 lần, tự động cập nhật mãi mãi
public class HUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _livesText;

    void Start()
    {
        var gm = ServiceLocator.Get<IGameManager>();

        // RegisterWithInitValue: gọi ngay lần đầu + mỗi lần thay đổi
        gm.Score.RegisterWithInitValue(v => _scoreText.text = v.ToString("N0"))
                .UnRegisterOnDestroy(gameObject); // tự cleanup khi HUD bị destroy

        gm.Lives.RegisterWithInitValue(v => _livesText.text = $"x{v}")
                .UnRegisterOnDestroy(gameObject);
    }
}

// Trong gameplay — chỉ cần set Value, UI tự cập nhật
_score.Value += 10;
_lives.Value--;
```

---

## 5. StateMachine — Thay thế if/else chồng chất

### Vấn đề cũ

```csharp
// ❌ Enemy với if/else: khó đọc, khó thêm state mới
void Update()
{
    if (_state == "idle") { /* ... */ }
    else if (_state == "patrol") { /* ... */ }
    else if (_state == "chase") { /* ... */ }
    else if (_state == "attack") { /* ... */ }
    else if (_state == "dead") { /* ... */ }
    // Thêm state mới → phải sửa file này → dễ break code cũ
}
```

### StateMachine của Dreamy Core

```
Enemy (GameObject)
  ├── StateMachine (component)
  ├── IdleState (component — child GO)
  ├── PatrolState (component — child GO)
  ├── ChaseState (component — child GO)
  └── AttackState (component — child GO)
```

```csharp
// Mỗi state = 1 script riêng, rõ ràng, dễ sửa
public class IdleState : BaseState
{
    public override void OnEnter(IStateData data = null)
    {
        GetComponentInParent<Animator>().Play("Idle");
    }

    public override void OnExecute()
    {
        if (DetectPlayer()) Machine.ChangeState<ChaseState>();
    }
}

public class ChaseState : BaseState
{
    public override void OnEnter(IStateData data = null)
    {
        GetComponentInParent<NavMeshAgent>().enabled = true;
    }

    public override void OnExecute()
    {
        Chase();
        if (InAttackRange()) Machine.ChangeState<AttackState>();
        if (LostPlayer())    Machine.ChangeState<IdleState>();
    }

    public override void OnExit() { /* cleanup */ }
}

// Controller: gọn gàng, không if/else
public class EnemyController : MonoBehaviour
{
    void Start() => GetComponent<StateMachine>().Initialize<IdleState>();
}
```

### Truyền data khi chuyển state

```csharp
// Định nghĩa data
public class SpawnData : IStateData { public Vector3 SpawnPoint; }

// Chuyển state kèm data
Machine.ChangeState<SpawnState>(new SpawnData { SpawnPoint = pos });

// Nhận data trong state
public class SpawnState : BaseState
{
    public override void OnEnter(IStateData data = null)
    {
        if (data is SpawnData d) transform.position = d.SpawnPoint;
    }
}
```

---

## 6. Object Pool — Thay thế Instantiate/Destroy

### Vấn đề cũ

```csharp
// ❌ Instantiate/Destroy mỗi viên đạn → GC Spike → giật frame
void Shoot()
{
    var bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    Destroy(bullet, 3f); // sau 3 giây destroy → GC chạy → giật
}
```

### GameObjectPool — Tái sử dụng object

```csharp
// Bullet kế thừa PoolableMonoBehaviour
public class Bullet : PoolableMonoBehaviour
{
    GameObjectPool<Bullet> _pool; // được set bởi GunController

    public void Init(GameObjectPool<Bullet> pool) => _pool = pool;

    public override void OnSpawn()
    {
        // Reset state mỗi lần spawn
        _rigidbody.linearVelocity = Vector3.zero;
    }

    public override void OnDespawn()
    {
        // Cleanup trước khi trả về pool
    }

    void OnCollisionEnter(Collision c)
    {
        // Thay vì Destroy: trả về pool
        _pool.Despawn(this);
    }
}

// GunController tạo và quản lý pool
public class GunController : MonoBehaviour
{
    [SerializeField] Bullet _bulletPrefab;
    GameObjectPool<Bullet> _pool;

    void Awake()
    {
        _pool = new GameObjectPool<Bullet>(_bulletPrefab, parent: transform, defaultCapacity: 30);
        _pool.Preload(10); // pre-warm 10 viên
    }

    public void Shoot()
    {
        var bullet = _pool.Spawn(); // lấy từ pool (không Instantiate)
        bullet.Init(_pool);
        bullet.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
    }
}
```

---

## 7. Extension Methods — Viết code gọn hơn

Extension method là method "thêm vào" class có sẵn mà không cần kế thừa.

### Các extension trong Dreamy Core

```csharp
// --- Transform ---
transform.ResetLocal();                   // position=0, rotation=0, scale=1
transform.DestroyChildren();             // xóa tất cả children

// --- Component ---
var rb = gameObject.GetOrAddComponent<Rigidbody>(); // GetComponent nếu có, AddComponent nếu không
var comp = someComp.OrNull();             // trả về null thật nếu đã bị Destroy (tránh Unity fake-null)

// --- List ---
var items = new List<int> { 1, 2, 3, 4 };
items.Shuffle();                          // xáo trộn
var random = items.GetRandom();          // lấy phần tử ngẫu nhiên
items.ForEach(i => Debug.Log(i));       // duyệt nhanh

// --- Vector ---
Vector3 pos = new Vector3(1, 2, 3);
pos = pos.WithY(0);                      // thay Y = 0, giữ nguyên X và Z
pos = pos.Flat();                        // Y = 0

// --- Number ---
int score = 1500000;
score.ToShortString();                   // "1.5M"
float time = 125f;
time.ToTimerString();                    // "2:05"

// --- LayerMask ---
LayerMask groundMask = ...;
bool hit = groundMask.Contains(gameObject.layer); // check layer

// --- Math ---
float mapped = MathUtility.Remap(value, 0, 100, 0f, 1f); // map range
bool lucky   = MathUtility.Chance(30);                    // 30% xác suất trả về true
```

---

## 8. Addressables — Thay thế Resources.Load

### Vấn đề của Resources.Load

```csharp
// ❌ Resources.Load: tất cả assets trong Resources/ đều bị đưa vào build
// → tăng kích thước app dù không dùng
// → không load async thực sự
// → không stream, không update qua OTA
var sprite = Resources.Load<Sprite>("Icons/coin"); // đồng bộ, block main thread
```

### Addressables giải quyết thế nào?

- Chỉ load asset khi cần, release khi xong → tiết kiệm memory
- Load async thực sự — không block game
- Có thể cập nhật asset qua remote (OTA update)
- Organize bằng label, không phụ thuộc folder structure

### Cách dùng cơ bản

```csharp
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// 1. Đặt Address cho asset trong Unity Inspector (chuột phải → Addressable)
// 2. Load khi cần

// Load 1 asset
var handle = Addressables.LoadAssetAsync<Sprite>("Icons/coin");
var sprite = await handle.Task;
image.sprite = sprite;

// QUAN TRỌNG: Release sau khi dùng xong
Addressables.Release(handle);

// Load và Instantiate
var handle2 = Addressables.InstantiateAsync("Prefabs/Enemy");
var enemy = await handle2.Task;
// Khi destroy: dùng Addressables.ReleaseInstance thay vì Destroy
Addressables.ReleaseInstance(enemy);
```

### Trong com.dreamy.data (package sắp tới) — wrapper gọn hơn

```csharp
// API đơn giản hơn — không cần nhớ Release thủ công
var sprite = await AssetLoader.LoadAsync<Sprite>("Icons/coin");
AssetLoader.Release("Icons/coin"); // hoặc tự release khi scene unload
```

### So sánh tổng quan

| | Resources.Load | Addressables |
|---|---|---|
| Kích thước build | Lớn (tất cả vào build) | Nhỏ hơn (chỉ load khi cần) |
| Thread | Block main thread | Async thực sự |
| Memory | Khó kiểm soát | Release được |
| Remote update | ❌ | ✅ |
| Độ phức tạp | Đơn giản | Cần setup ban đầu |

> **Khuyến nghị**: Dùng Addressables cho prefab, sprite, audio clip. Giữ `Resources/` chỉ cho asset cần thiết ở startup (config, settings).

---

## 9. WaitFor — Cached coroutine yield

```csharp
// ❌ Cũ: tạo object mới mỗi lần → GC
yield return new WaitForSeconds(2f);

// ✅ Mới: lấy từ cache — zero allocation
yield return WaitFor.Seconds(2f);
yield return WaitFor.EndOfFrame;
yield return WaitFor.FixedUpdate;
```

---

## 10. AppLifecycle & ITickable

### AppLifecycle — Tập trung xử lý vòng đời app

```csharp
// Đăng ký ở bất cứ đâu — không cần MonoBehaviour
AppLifecycle.OnPause  += isPaused => AudioListener.pause = isPaused;
AppLifecycle.OnFocus  += hasFocus => { if (!hasFocus) SaveGame(); };
AppLifecycle.OnQuit   += () => SaveGame();
```

### ITickable — Service không phải MonoBehaviour mà cần Update

```csharp
// Service thuần C# cần chạy mỗi frame
public class AchievementService : ITickable
{
    public void Tick(float deltaTime)
    {
        CheckAchievements(deltaTime);
    }
}

// Đăng ký vào AppTickService
var service = new AchievementService();
AppTickService.Register(service);
AppTickService.Unregister(service); // khi không cần nữa
```

---

## Tóm tắt — Checklist thay đổi

| Cũ | Mới |
|---|---|
| `IEnumerator` + `StartCoroutine` | `async UniTask` + `await` |
| `Singleton<T>.Instance` trực tiếp | `ServiceLocator.Get<IService>()` |
| `GetComponent` + serialize nhiều ref | `ServiceLocator` + `MyEventBus` |
| `void Update()` check string state | `StateMachine` + `BaseState` |
| `Instantiate` + `Destroy` | `GameObjectPool<T>.Spawn/Despawn` |
| `Resources.Load<T>()` | `Addressables` (hoặc `AssetLoader`) |
| `new WaitForSeconds(t)` | `WaitFor.Seconds(t)` |
| Script phụ thuộc lẫn nhau (coupling) | `MyEventBus<T>` + `IEvent` struct |
| Cập nhật UI thủ công | `BindableProperty<T>` reactive |
