# 🧱 Ironcow\.Synapse.Common

> Core module of SynapseFramework
> A collection of foundational systems used across all features, including `SynapseBehaviour`, `ScheduleManager`, `SOSingleton`, and various editor tools.

---

## 📌 Key Features

| Feature Name            | Description                                                                                                  |
| ----------------------- | ------------------------------------------------------------------------------------------------------------ |
| **SynapseBase**         | Automates caching based on MonoBehaviour naming. Eliminates GetComponent calls.                              |
| **SynapseBehaviour**    | Base class for all scripts in SynapseFramework.                                                              |
| **ScheduleManager**     | Automatically manages subscriptions to Update, LateUpdate, and FixedUpdate.                                  |
| **ResourceManager**     | Loads assets with pooling support. Uses `params`-based path configuration instead of direct Resources usage. |
| **SOSingleton**         | Automatically manages ScriptableObject singletons (Editor-only release handling included).                   |
| **ProjectSettingTool**  | Centralized configuration manager with module auto-expansion support.                                        |
| **FrameworkController** | Utility for toggling DefineSymbols for each module.                                                          |
| **Register**            | Automatically registers SynapseBehaviours on instantiation and unregisters on destruction.                   |

---

## 🧹 Usage Examples

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
    // Automatically registered to ScheduleManager
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

## 💪 Installation & Dependencies

* Unity 2022.3 or higher
* Required core module for other SynapseFramework modules
* Initializes automatically without manual configuration

---

## 📂 Key Files

| File Name                | Description                                |
| ------------------------ | ------------------------------------------ |
| `SynapseBehaviour.cs`    | Base MonoBehaviour class with auto-caching |
| `ScheduleManager.cs`     | Manages Unity's lifecycle update events    |
| `ResourceManager.cs`     | Loads and pools resources                  |
| `SOSingleton.cs`         | Manages ScriptableObject singletons        |
| `ProjectSettingTool.cs`  | Global configuration and module control UI |
| `FrameworkController.cs` | DefineSymbol toggle utility                |
| `Register.cs`            | Manages object registration for instances  |

---

## 💡 Tips & Notes

* `ScheduleManager` works in the Editor and auto-initializes.
* `ResourceManager` supports Editor execution and handles path errors gracefully.
* `SOSingleton` auto-creates ScriptableObjects for Editor and persists across recompiles.

---

## 🔄 Update History

* `v1.0.0` (2025.06): Initial release with core utilities (Caching, Scheduling, etc.)

---

## 🤛 FAQ

**Q. Can this module be used independently in another project?**
A. Yes. It is self-contained, but many other modules depend on it, so removal is not recommended.

---

## 📧 Support

* For inquiries, please use the contact form on our publisher profile or official website.
🔗 http://ironcowstudio.duckdns.org/index.php
