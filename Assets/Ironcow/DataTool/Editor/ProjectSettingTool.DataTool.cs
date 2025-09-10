// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework © 2025 Ironcow Studio
// Distributed via Gumroad under a paid license
// 
// 🔐 This file is part of a licensed product. Redistribution or sharing is prohibited.
// 🔑 A valid license key is required to unlock all features.
// 
// 🌐 For license terms, support, or team licensing, visit:
//     https://ironcowstudio.duckdns.org/ironcowstudio.html
// ─────────────────────────────────────────────────────────────────────────────


using Ironcow.Synapse.Data;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Ironcow.Synapse
{
    public partial class ProjectSettingTool
    {
        [MenuItem("Synapse/Data Tool/Open Locked Inspector For Selection #&D")]
        public static void OpenNewInspector_DataTool()
        {
            // 내부 타입 UnityEditor.InspectorWindow 찾기
            var inspType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            if (inspType == null)
            {
                Debug.LogWarning("InspectorWindow type not found.");
                return;
            }

            // 새 인스펙터 창 생성 및 표시
            var window = ScriptableObject.CreateInstance(inspType) as EditorWindow;
            window.Show();                         // 유사 팝업 / 독립창
            window.Focus();
            window.titleContent = new GUIContent("Data Tool");

            EditorApplication.update += CheckWindow;

            void CheckWindow()
            {
                try
                {
                    if (window == null)
                    {
                        DataTreeMapSO.Release();
                    }
                }
                finally
                {
                    if (window == null)
                        EditorApplication.update -= CheckWindow;
                }
            }
            EditorApplication.delayCall += () =>
            {
                // 고정하고 싶은 대상을 먼저 선택
                if (DataTreeMapSO.instance != null)
                {
                    Selection.activeObject = DataTreeMapSO.instance;
                    // 선택 반영을 위해 한 프레임 양보 (즉시 반영이 필요한 경우 Repaint 호출)
                    window.Repaint();
                }

                // isLocked 프로퍼티 설정 (Unity 버전에 따라 NonPublic일 수 있어 BindingFlags 모두 시도)
                var prop = inspType.GetProperty("isLocked",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(window, true, null);     // 🔒 잠금!
                }
                else
                {
                    Debug.LogWarning("Could not set isLocked on InspectorWindow.");
                }
            };
        }
        public SettingMenu OnEnable_DataToolSetting()
        {
            return new SettingMenu { id = 18, name = "Data Tool Setting", isVisible = () => true, getScritables = Get_DataToolSettingSO };
        }
        public SettingMenu OnEnable_DataTool()
        {
            return new SettingMenu { id = 19, name = "Data Tool", isVisible = () => true, getScritables = Get_DataTool };
        }

        public ScriptableObject Get_DataTool()
        {
            return DataTreeMapSO.instance;
        }

        public ScriptableObject Get_DataToolSettingSO()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Open Data Tool"))
                {
                    OpenNewInspector_DataTool();
                }
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (DataToolSetting.instance.dataScriptableObjectPath == null)
                {
                    if (GUILayout.Button("Craete Data Path"))
                    {
                        var targetFolder = Application.dataPath
#if USE_ADDRESSABLE
                        + "/AddressableDatas/Datas";
#else
                            + "/_Project/Resources/Datas";
#endif
                        if (!Directory.Exists(targetFolder))
                        {
                            Directory.CreateDirectory(targetFolder);
                        }
                        AssetDatabase.Refresh();
                        DataToolSetting.instance.dataScriptableObjectPath = AssetDatabase.LoadAssetAtPath<DefaultAsset>(targetFolder.Replace(Application.dataPath, "Assets"));
                    }
                }
                if (DataToolSetting.instance.thumbnailPath == null)
                {
                    if (GUILayout.Button("Craete Thumbnail Path"))
                    {
                        var targetFolder = Application.dataPath
#if USE_ADDRESSABLE
                        + "/AddressableDatas/Thumbnails";
#else
                            + "/_Project/Resources/Thumbnails";
#endif
                        if (!Directory.Exists(targetFolder))
                        {
                            Directory.CreateDirectory(targetFolder);
                        }
                        AssetDatabase.Refresh();
                        DataToolSetting.instance.thumbnailPath = AssetDatabase.LoadAssetAtPath<DefaultAsset>(targetFolder.Replace(Application.dataPath, "Assets"));
                    }
                }
                if (DataToolSetting.instance.classGeneratePath == null)
                {
                    if (GUILayout.Button("Create Data SO Path"))
                    {
                        var targetFolder = Application.dataPath + "/_Project/Scripts/ScriptableObjects";
                        if (!Directory.Exists(targetFolder))
                        {
                            Directory.CreateDirectory(targetFolder);
                        }
                        AssetDatabase.Refresh();
                        DataToolSetting.instance.classGeneratePath = AssetDatabase.LoadAssetAtPath<DefaultAsset>(targetFolder.Replace(Application.dataPath, "Assets"));
                    }
                }
            }
            return DataToolSetting.instance;
        }

        partial void CreateDataManagerInstance()
        {
            DataEditor.CreateManagerInstance();
        }
            
        partial void CreatePartialDataManager()
        {
            DataEditor.CreatePartialScripts();
        }
    }

}
