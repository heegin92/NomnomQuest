// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using Ironcow.Synapse.Resource;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ironcow.Synapse
{
    public partial class ProjectSettingTool
    {
        public SettingMenu OnEnable_EditorDataSetting()
        {
            return new SettingMenu { id = 12, name = "Editor Data Setting", getScritables = Get_EditorDataSetting };
        }

        public ScriptableObject Get_EditorDataSetting()
        {
            DrawEditorButtons();
            return EditorDataSetting.instance;
        }

        partial void CreateProjectFoleders()
        {
            var projectFolder = Path.Combine(Application.dataPath, "_Project");
            if (GUILayout.Button("Create Project Folders"))
            {
                if (!Directory.Exists(projectFolder))
                {
                    Directory.CreateDirectory(Path.Combine(projectFolder));
                }
                var targetFolder = Path.Combine(projectFolder, "Resources");
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                    targetFolder = Path.Combine(targetFolder, "Prefabs");
                    if (!Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }
                }
                if (EditorDataSetting.instance.createAssetPrefabPath == null)
                {
                    EditorDataSetting.instance.createAssetPrefabPath = AssetDatabase.LoadAssetAtPath<DefaultAsset>(targetFolder.Replace(Application.dataPath, "Assets"));
                    AssetDatabase.Refresh();
                }
                targetFolder = Path.Combine(projectFolder, "Scripts");
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }
                if (EditorDataSetting.instance.scriptPath == null)
                {
                    EditorDataSetting.instance.scriptPath = AssetDatabase.LoadAssetAtPath<DefaultAsset>(targetFolder.Replace(Application.dataPath, "Assets"));
                    AssetDatabase.Refresh();
                }
                targetFolder = Path.Combine(projectFolder, "Textures");
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }
                targetFolder = Path.Combine(projectFolder, "Scenes");
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }
                if (EditorDataSetting.instance.scenePath.Count == 0)
                {
                    EditorDataSetting.instance.scenePath.Add(AssetDatabase.LoadAssetAtPath<DefaultAsset>(targetFolder.Replace(Application.dataPath, "Assets")));
                    AssetDatabase.Refresh();
                }
                targetFolder = Path.Combine(projectFolder, "Prefabs");
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }
                if (EditorDataSetting.instance.createEditorPrefabPath == null)
                {
                    EditorDataSetting.instance.createEditorPrefabPath = AssetDatabase.LoadAssetAtPath<DefaultAsset>(targetFolder.Replace(Application.dataPath, "Assets"));
                    AssetDatabase.Refresh();
                }
                targetFolder = Path.Combine(projectFolder, "Settings");
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }
                if (EditorDataSetting.instance.settingSOPath == null)
                {
                    EditorDataSetting.instance.settingSOPath = AssetDatabase.LoadAssetAtPath<DefaultAsset>(targetFolder.Replace(Application.dataPath, "Assets"));
                    AssetDatabase.Refresh();
                }
            }
        }

        // 에디터 버튼들을 그리는 함수
        private void DrawEditorButtons()
        {
            GUILayout.BeginHorizontal();
            CreateProjectFoleders();
            if (GUILayout.Button("Create Partial Scripts"))
            {
                ResourceEditor.CreatePartialScripts();
                CreatePartialDataManager();
            }
            if (GUILayout.Button("Create Manager"))
            {
                ResourceEditor.CreateManagerInstance();
                CreateDataManagerInstance();
                CreateSpawnManagerInstance();
                CreateNetworkManagerInstance();
                CreateSoundManagerInstance();
                CreatePoolManagerInstance();
                CreateUIManagerInstance();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            CreatePoolSettingSO();
            CreatePrefab();
            CreateJoyStick();
            GUILayout.EndHorizontal();
        }

        partial void CreateProjectFoleders();

        partial void CreatePrefab();
        partial void CreatePoolManagerInstance();
        partial void CreateSoundManagerInstance();

        partial void CreateJoyStick();

        partial void CreateUIManagerInstance();

        partial void CreateNetworkManagerInstance();
        partial void CreateSpawnManagerInstance();
        partial void CreateDataManagerInstance();
        partial void CreatePoolSettingSO();
        partial void CreatePartialDataManager();
    }
}
