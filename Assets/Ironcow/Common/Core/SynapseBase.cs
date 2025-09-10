// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using Ironcow.Synapse.Core;
using Ironcow.Synapse.Sample.Common;

using UnityEngine;
using UnityEngine.UIElements;
#if USE_INPUTSYSTEM
using Ironcow.Synapse.InputSystem;
#endif

namespace Ironcow.Synapse
{
    public class SynapseBase :
#if ODIN_INSPECTOR
        Sirenix.OdinInspector.SerializedMonoBehaviour
#else
        MonoBehaviour
#endif
#if USE_INPUTSYSTEM
        , IDelegate
#endif
    {
#if UNITY_EDITOR
        bool IsUnityNull(object obj) => obj == null || obj.Equals(null);

        protected virtual void OnValidate()
        {
            if(Application.isPlaying) return;
            AutoCaching();
        }

        public void AutoCaching()
        {
            var fields = this.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (!typeof(Component).IsAssignableFrom(field.FieldType))
                {
                    var value = field.GetValue(this);
                    var isNull = IsUnityNull(value);
                    if (!isNull) continue;
                    var components = GetComponentsInChildren(typeof(Transform), false);
                    foreach (var component in components)
                    {
                        if (component.name.ToLower() == field.Name.ToLower())
                        {
                            field.SetValue(this, component.gameObject);
                            break;
                        }
                    }
                }
                else
                {
                    var value = field.GetValue(this);
                    var isNull = IsUnityNull(value);
                    if (!isNull) continue;
                    var components = GetComponentsInChildren(field.FieldType);
                    foreach (var component in components)
                    {
                        if (component.name.ToLower() == field.Name.ToLower())
                        {
                            field.SetValue(this, component);
                            break;
                        }
                    }
                    isNull = IsUnityNull(value);
                    if (!isNull) continue;
                    if (isNull && field.FieldType != typeof(Transform))
                    {
                        if (gameObject.TryGetComponent(field.FieldType, out var component))
                            field.SetValue(this, component);
                    }
                }
            }
        }

        public static new T Instantiate<T>(T origin) where T : Object
        {
            return Instantiate(origin, Vector3.zero, Quaternion.identity, null);
        }

        public static new Object Instantiate(Object origin)
        {
            return Instantiate(origin, Vector3.zero, Quaternion.identity, null);
        }

        public static new T Instantiate<T>(T origin, Transform parent) where T : Object
        {
            return Instantiate(origin, Vector3.zero, Quaternion.identity, parent);
        }

        public static new Object Instantiate(Object origin, Transform parent)
        {
            return Instantiate(origin, Vector3.zero, Quaternion.identity, parent);
        }

        public static new T Instantiate<T>(T origin, Transform parent, bool worldPositionStays) where T : Object
        {
            if (origin is SynapseBehaviour ironcowBehaviour)
            {
                return Register.Instantiate(ironcowBehaviour, parent, worldPositionStays) as T;
            }
            else
            {
                return Object.Instantiate(origin, parent, worldPositionStays) as T;
            }
        }

        public static new Object Instantiate(Object origin, Transform parent, bool worldPositionStays)
        {
            return Instantiate<Object>(origin, parent, worldPositionStays);
        }

        public static new T Instantiate<T>(T origin, Vector3 position, Quaternion rotation) where T : Object
        {
            return Instantiate(origin, position, rotation, null);
        }

        public static new Object Instantiate(Object origin, Vector3 position, Quaternion rotation)
        {
            return Instantiate(origin, Vector3.zero, Quaternion.identity, null);
        }

        public static new Object Instantiate(Object origin, Vector3 position, Quaternion rotation, Transform parent)
        {
            return Instantiate<Object>(origin, Vector3.zero, Quaternion.identity, parent);
        }

        public static new T Instantiate<T>(T origin, Vector3 position, Quaternion rotation, Transform parent) where T : Object
        {
            if (origin is SynapseBehaviour ironcowBehaviour)
            {
                return Register.Instantiate(ironcowBehaviour, position, rotation, parent) as T;
            }
            else
            {
                return Object.Instantiate(origin, position, rotation, parent) as T;
            }
        }

        public static new void Destroy(Object instance)
        {
            if (instance is SynapseBehaviour ironcowBehaviour)
                Register.Release(ironcowBehaviour);
            else if (instance is GameObject gameObject)
                Register.Release(gameObject);
            else
                Object.Destroy(instance);
        }

        public static new void Destroy(Object instance, float t)
        {
            if (instance is SynapseBehaviour ironcowBehaviour)
                Register.Release(ironcowBehaviour, t);
            else if (instance is GameObject gameObject)
                Register.Release(gameObject, t);
            else
                Object.Destroy(instance, t);
        }
#endif
    }
}
