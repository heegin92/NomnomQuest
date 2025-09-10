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


using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;
using UnityEngine.Networking;

namespace Ironcow.Synapse.Data
{
    [CustomEditor(typeof(DataTreeMapSO))]
    public class DataTreeMapEditor : Editor
    {
        private TreeViewState treeViewState;
        private DataTreeView treeView;
        private DataTreeMapSO map;
        private BaseDataSO selected;
        Editor editor;
        private float height = 650;
        private void OnEnable()
        {
            map = (DataTreeMapSO)target;
            treeViewState ??= new TreeViewState();
            treeView = new DataTreeView(treeViewState, map, asset =>
            {
                // 선택 변경 시 editor 교체
                if (asset == null || asset == selected)
                    return;

                selected = asset;

                if (editor != null)
                {
                    DestroyImmediate(editor);
                    editor = null;
                }
            });
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Height(height));
            DrawTree();
            DrawSO();

            EditorGUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Download Google Sheet")))
            {
                ClearLog();
                DownloadData(sheets);
            }
            if (GUILayout.Button(new GUIContent("Generate Class From Sheet")))
            {
                SheetToClassGenerator.GenerateFromInfo();
                AssetDatabase.Refresh();
                EditorPrefs.SetBool("GenerateClass", true);
            }
            if (GUILayout.Button(new GUIContent("Open Data Tool Setting")))
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
                            DataToolSetting.Release();
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
                    if (DataToolSetting.instance != null)
                    {
                        Selection.activeObject = DataToolSetting.instance;
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
            GUILayout.EndHorizontal();
        }

        void DrawTree()
        {
            var treeRect = GUILayoutUtility.GetRect(140, height, GUILayout.Width(140));
            // 트리 뷰
            treeView.OnGUI(treeRect);

        }

        void DrawSO()
        {
            float spacing = 8f;
            float padding = 4f;
            float totalWidth = EditorGUIUtility.currentViewWidth;

            float rightStartX = treeView.rect.width + spacing + treeView.rect.x;
            float rightWidth = totalWidth - rightStartX - padding;
            var rightRect = new Rect(rightStartX, treeView.rect.y, rightWidth, treeView.rect.height);

            // 인스펙터 영역 (GUILayout 방식으로 처리)
            GUILayout.BeginArea(rightRect);
            {
             
                if (selected != null)
                {
                    if (GUILayout.Button($"Refresh All {selected?.GetType().ToString()}s"))
                    {
                        var sheets = this.sheets.FindAll(obj => obj.className == selected?.GetType().ToString());
                        DownloadData(sheets);
                    }
                    if (GUILayout.Button($"Refresh {selected?.rcode}"))
                    {
                        var sheets = this.sheets.FindAll(obj => obj.className == selected?.GetType().ToString());
                        DownloadData(sheets, selected);
                    }

                    if (editor == null || editor.target != selected)
                    {
                        DestroyImmediate(editor);
                        CreateCachedEditor(selected, null, ref editor);
                    }

                    if (editor != null)
                    {
                        editor.OnInspectorGUI();
                    }
                }
            }
            GUILayout.EndArea();
        }


        public void ClearLog()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

        public async void DownloadData(List<SheetInfoSO> sheets, BaseDataSO data = null)
        {
            foreach (var sheet in sheets)
            {
                var url = $"{DataToolSetting.instance.GSheetUrl}export?format=tsv&gid={sheet.sheetId}";
                var req = UnityWebRequest.Get(url);
                var op = req.SendWebRequest();
                Debug.Log($"{sheet.className}");
                await op;
                var res = req.downloadHandler.text;
                Debug.Log(res);
                sheet.datas = TSVParser.TsvToDic(res);
            }
            if (data != null)
            {
                RefreshData(sheets[0], data);
            }
            else
            {
                ImportDatas(sheets);
            }
        }

        public void RefreshData(SheetInfoSO sheet, BaseDataSO data)
        {
            var dicData = sheet.datas.Find(obj => obj["rcode"] == data.rcode);
            var path = DataToolSetting.DataScriptableObjectPath + "/" + dicData["rcode"] + ".asset";
            var dt = (ScriptableObject)AssetDatabase.LoadAssetAtPath(path, data.GetType());
            dt = TSVParser.DicToSOData(data.GetType(), dt, dicData);

            EditorUtility.SetDirty(dt);
            AssetDatabase.SaveAssets();
        }

        protected void ImportDatas(List<SheetInfoSO> sheets)
        {
            foreach (var sheet in sheets)
            {
                ImportData(sheet);
            }
        }

        protected void ImportData(SheetInfoSO sheet)
        {
            //if (sheet.isUpdate)
            {
                Assembly assembly = typeof(BaseDataSO).Assembly;
                var type = assembly.GetType(sheet.className);
                GetDatas(type, sheet.datas);
            }
        }

        public void GetDatas(Type type, List<Dictionary<string, string>> datas)
        {
            foreach (var data in datas)
            {
                if (!data.ContainsKey("rcode")) return;
                var path = DataToolSetting.DataScriptableObjectPath + "/" + data["rcode"] + ".asset";
                var dt = (ScriptableObject)AssetDatabase.LoadAssetAtPath(path, type);
                if (dt == null)
                {
                    dt = DicToClass(type, data);
                }
                else
                {

                    dt = TSVParser.DicToSOData(type, dt, data);
                }

                EditorUtility.SetDirty(dt);
                AssetDatabase.SaveAssets();
            }
        }

        private List<SheetInfoSO> sheets { get => DataToolSetting.instance.sheets; }

        public ScriptableObject DicToClass(Type type, Dictionary<string, string> data)
        {
            var dt = CreateInstance(type);
            AssetDatabase.CreateAsset(dt, DataToolSetting.DataScriptableObjectPath + "/" + data["rcode"] + ".asset");
            return TSVParser.DicToSOData(type, dt, data);
        }
    }
}
