# 🧱 Ironcow\.Synapse.Common

> SynapseFramework의 해시 기반 목록
> SynapseBehaviour, ScheduleManager, SOSingleton, 각종 에디터 스크립트 등 모든 시스템의 기반 기능 모음

---

## 📌 주요 기능 요약

| 기능명                     | 설명                                                       |
| ----------------------- | -------------------------------------------------------- |
| **SynapseBase**         | MonoBehaviour 네이버리 기반 캐시 자동화. GetComponent 호출 제거         |
| **SynapseBehaviour**    | Synapse Framework 사용의 기반이 되는 모든 스크립트의 시작점                |
| **ScheduleManager**     | Update, LateUpdate, FixedUpdate를 각 스크립트에서 자동 구독시켜 관리     |
| **ResourceManager**     | Resources를 직접 사용하지 않고 로드 후 푸린 제공. params 기반 경로 설정 지원     |
| **SOSingleton**         | ScriptableObject 싱글턴 자동 관리 시스템 (Editor 전용 Release 처리 포함) |
| **ProjectSettingTool**  | 모든 목록 설정을 한 곳에서 관리 가능, 목록 자동 확장 기능 포함                    |
| **FrameworkController** | 각 목록별 기능 사용 DefineSymbol 설정 톱글 제공                        |
| **Register**            | SynapseBehaviour를 Instantiate할 경우 자동으로 등록. Destroy시 해제   |

---

## 🛉 사용 예시

### ☑️ SynapseBehaviour

```csharp
public class Player : SynapseBehaviour
{
    
}
```

---

### ⏱ ScheduleManager

```csharp
public class Player : SynapseBehaviour
{
    // 자동으로 ScheduleManager에 등록
    public void OnUpdate()
    {

    }
}
```

---

### 📦 ResourceManager

```csharp
public class Player : SynapseBehaviour
{
    public void TestFunction()
    {
        var test = ResourceManager.instance.LoadAsset("Test", ResourceType.UI, ResourceType.Popup);
    }
}
```

---

### 📦 Register

```csharp
public class Player : SynapseBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.TryGetInstance<Enemy>(out var enemy)) // Register.TryGetInstance<T>(obj.gameObject.GetInstanceID(), out target);
        {
            enemy.OnDamage(atk);
        }
    }
}
```

---

## 💪 설치 및 의존성

* Unity 2022.3 이상
* 타 목록이 Common을 기반으로 의존하고 있어, 필수 목록입니다.
* 변경 없이 자동 초기화됩니다.

---

## 📂 내부 주요 파일

| 파일명                      | 설명                                         |
| ------------------------ | ------------------------------------------ |
| `SynapseBehaviour.cs`    | 캐시형 MonoBehaviour 상속 기반                    |
| `ScheduleManager.cs`     | Update 관리 사용자 생명주기 관리 시스템                  |
| `ResourceManager.cs`     | Resources 로드 및 경로 기반 캐시/푸린                 |
| `SOSingleton.cs`         | ScriptableObject 싱글턴 자동 관리                 |
| `ProjectSettingTool.cs`  | 전역 설정 및 목록 초기화 창                           |
| `FrameworkController.cs` | 디파인 심볼 등록 톱글 유틸                            |
| `Register.cs`            | GetInstance, TryGetInstance 구조로 ID 기반 가져오기 |

---

## 💡 핀 & 죄의사항

* `ScheduleManager`는 에디터에서도 작동가능하며 자동 초기화됩니다.
* `ResourceManager`도 에디터에서 작동하고, 경로 오류 시 예제 처리가 포함됩니다.
* `SOSingleton`은 에디터 전용 SO를 자동 생성하고, 컨파일 후에도 유지됩니다.

---

## 🔄 업데이트 날짜

* `v1.0.0` (2025.06): 기본 유틸 구조 완성 (Caching, Schedule 등)

---

## 🤛 FAQ

**Q. 이 목록만 다른 프레임워크에서 사용 가능한가요?**
회수됩니다. 다른 목록에서도 해당 기능을 사용하고 있으며, 삭제는 권장되지 않습니다.

---

## 📧 지원

* 문의 사항이 있으신 경우, 퍼블리셔 프로필 또는 공식 홈페이지의 문의 양식을 이용해 주세요.
🔗 http://ironcowstudio.duckdns.org/index.php
