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
using System.IO;
using System.Linq;
using System.Reflection;

using Ironcow.Synapse;
using Ironcow.Synapse.BT;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

[CustomPropertyDrawer(typeof(BTRunner))]
public class BTRunnerDrawer : PropertyDrawer
{
    BTRunner instance;
    static bool isOpened = true;
    List<(MethodInfo, Type)> allMethods = new();
    SerializedProperty property;
    BTSaveData data;
    bool isCreateNewBT;
    string newDataName = "";
    ReorderableList drawMenu;
    bool isMethodInit = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (this.property == null)
        {
            this.property = property;
        }
        if (instance == null)
        {
            instance = (BTRunner)this.fieldInfo.GetValue(property.serializedObject.targetObject);
            if (instance == null)
            {
                instance = new BTRunner();
                this.fieldInfo.SetValue(property.serializedObject.targetObject, instance);
            }
        }
        if (allMethods.Count == 0 && !isMethodInit)
        {
            isMethodInit = true;
            var actionClasses = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttribute<BTActionsAttribute>() != null);

            foreach (var type in actionClasses)
            {
                foreach (var m in BTActionsBase.CachedMethods(type))
                {
                    if (type == typeof(BTActionsBase)) continue;
                    allMethods.Add((m, type));
                }
            }
        }
        if (data == null && !string.IsNullOrEmpty(instance.lastData))
        {
            LoadData(instance.lastData);
        }

        EditorGUILayout.BeginHorizontal();

        if (isCreateNewBT)
        {
            newDataName = EditorGUILayout.TextField(newDataName);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("생성", GUILayout.Width(40)))
            {
                if (AssetDatabase.LoadAssetAtPath<BTSaveData>(Path.Combine(BTEditor.SavePath, newDataName)) != null)
                {
                    EditorUtility.DisplayDialog("경고", "이미 파일이 존재합니다. 다른 이름으로 변경해주세요", "확인");
                }
                else
                {
                    CreateData(newDataName);
                    if (data != null)
                        instance.lastData = data.name;
                    newDataName = "";
                    isCreateNewBT = false;
                }
            }
            if (GUILayout.Button("취소", GUILayout.Width(40)))
            {
                isCreateNewBT = false;
            }
        }
        else
        {
            if (EditorGUILayout.DropdownButton(new GUIContent(data == null ? "Select BT Data" : data.name), FocusType.Passive))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("<None>"), false, () =>
                {
                    data = null;
                    instance.root = new RootNode();
                    instance.lastData = "";
                });

                var items = AssetDatabase.FindAssets("t:BTSaveData")
                            .Select(guid => AssetDatabase.LoadAssetAtPath<BTSaveData>(AssetDatabase.GUIDToAssetPath(guid)));

                foreach (var asset in items)
                {
                    menu.AddItem(new GUIContent(asset.name), asset == this.data, (dt) =>
                    {
                        SaveData();
                        data = (BTSaveData)dt;
                        instance.root = JsonUtility.FromJson<RootNode>(data.data);
                        instance.lastData = data.name;
                        drawMenu = null; // Reset the draw menu to refresh the display
                    }, asset);
                }

                menu.ShowAsContext();
            }
            if (GUILayout.Button("New BTData", GUILayout.Width(100)))
            {
                isCreateNewBT = true;
            }
        }

        EditorGUILayout.EndHorizontal();
        isOpened = EditorGUI.Foldout(position, isOpened, "BTRunner");
        if (isOpened)
        {
            GUILayout.Space(5);
            if (data != null)
            {
                GUILayout.Space(10);
                if (instance.root == null) instance.root = new RootNode();
                EditorGUILayout.BeginVertical();
                DrawNode(instance.root, GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandHeight(true)));
                EditorGUILayout.EndVertical();
            }
        }
        if (data == null)
        {
            instance.root = new RootNode();
        }
        property.serializedObject.ApplyModifiedProperties();
    }

    public void SaveData()
    {
        if (Application.isPlaying || data == null) return;
        if (instance.root != null)
            data.SaveData(instance.root);
        EditorUtility.SetDirty(data);
    }

    public void CreateData(string dataName)
    {
        var path = Path.Combine(BTEditor.SavePath, $"{dataName}.asset");
        if (!AssetDatabase.IsValidFolder(BTEditor.SavePath))
        {
            AssetDatabase.CreateFolder(BTEditor.ParentPath, "BTSaveData");
        }
        var dt = ScriptableObject.CreateInstance<BTSaveData>();
        AssetDatabase.CreateAsset(dt, path);
        Debug.Log("Create Complete");
        data = AssetDatabase.LoadAssetAtPath<BTSaveData>(path);
    }

    bool isDialog = false;
    public void LoadData(string lastData)
    {
        if (isDialog) return;
        if (BTEditor.instance.savePath == null)
        {
            Selection.activeObject = BTEditor.instance;
            isDialog = EditorUtility.DisplayDialog("경고", "BTEditor의 SavePath를 설정해주세요", "ok");
            return;
        }
        var name = $"{lastData}.asset";
        var path = Path.Combine(BTEditor.SavePath, name);
        data = AssetDatabase.LoadAssetAtPath<BTSaveData>(path);
        if (data == null)
        {
            CreateData(name);
        }

        if (!Application.isPlaying)
            instance.root = JsonUtility.FromJson<RootNode>(data.data);
        if (instance.root == null)
            instance.root = new RootNode();
    }

    public ReorderableList DrawNode(BTNode node, Rect position)
    {
        if (node == null) return null;
        ReorderableList rl = null;
        var type = node.GetType().UnderlyingSystemType;
        EditorGUI.indentLevel++;

        if (type == typeof(SelectorNode))
        {
            var nd = (SelectorNode)node;
            rl = SetList(type.Name.ToString(), nd, nd.childs, type, position);
        }
        else if (type == typeof(SequenceNode))
        {
            var nd = (SequenceNode)node;
            rl = SetList(type.Name.ToString(), nd, nd.childs, type, position);
        }
        else if (type == typeof(RootNode))
        {
            var nd = (RootNode)node;
            rl = SetList(type.Name.ToString(), nd, new List<BTNode>() { nd.node }, type, position);
            EditorGUILayout.Space(rl.GetHeight());
        }
        else if (type == typeof(ActionNode))
        {
            DrawAction(type.Name.ToString(), (ActionNode)node, position);
        }
        EditorGUI.indentLevel--;

        if (Event.current.type == EventType.MouseDown && position.Contains(Event.current.mousePosition))
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData("DraggedNode", node);
            DragAndDrop.objectReferences = new UnityEngine.Object[] { };
            DragAndDrop.StartDrag("Dragging BTNode");
            Event.current.Use();
        }
        if (Event.current.type == EventType.DragUpdated && position.Contains(Event.current.mousePosition))
        {
            var dragged = DragAndDrop.GetGenericData("DraggedNode") as BTNode;
            if (dragged != null && dragged != node && node is CompositeNode)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Event.current.Use();
            }
        }
        if (Event.current.type == EventType.DragPerform && position.Contains(Event.current.mousePosition))
        {
            var dragged = DragAndDrop.GetGenericData("DraggedNode") as BTNode;
            if (dragged != null && dragged != node && node is CompositeNode compNode)
            {
                RemoveFromParent(dragged);
                compNode.childs.Add(dragged);
                SaveData();
                GUI.changed = true;
                DragAndDrop.AcceptDrag();
                Event.current.Use();
            }
        }
        return rl;
    }

    void RemoveFromParent(BTNode node)
    {
        void Search(BTNode parent)
        {
            if (parent is CompositeNode comp)
            {
                if (comp.childs.Contains(node))
                {
                    comp.childs.Remove(node);
                    return;
                }
                foreach (var child in comp.childs)
                    Search(child);
            }
        }
        Search(instance.root.node);
    }

    public float DrawAction(string label, ActionNode node, Rect position)
    {
        int width = 60;
        EditorGUI.LabelField(new Rect(position.x, position.y, width, position.height), label.Replace("Node", ""));

        var content = new GUIContent(string.IsNullOrEmpty(node.funcName) ? "<none>" : $"{node.className}/{node.funcName}");

        if (EditorGUI.DropdownButton(new Rect(position.x + width, position.y, position.width - width, position.height), content, FocusType.Passive))
        {
            GenericMenu menuClass = new GenericMenu();
            foreach (var method in allMethods)
            {
                if (method.Item1.Name == "__Sample" && method.Item1.DeclaringType == typeof(BTActionsBase))
                    continue;
                var isEquip = node.onUpdate != null && node.onUpdate.Method == method.Item1;
                string menuLabel = $"{method.Item2.Name}/{method.Item1.Name}";
                menuClass.AddItem(new GUIContent(menuLabel), isEquip, (dt) =>
                {
                    var target = property.serializedObject.targetObject;
                    node.funcName = method.Item1.Name;
                    node.targetClassName = method.Item2.FullName;
                    SaveData();
                }, method.Item1.Name);
            }
            menuClass.ShowAsContext();
        }
        return 30;
    }

    public ReorderableList SetList(string label, BTNode node, List<BTNode> nodes, Type type, Rect position)
    {
        ReorderableList rlist = new ReorderableList(nodes, type, false, true, true, true);

        rlist.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, label.Replace("Node", ""));
        };

        rlist.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var list = DrawNode(nodes[index], rect);
            if (list != null)
            {
                rlist.elementHeight += list.GetHeight();
            }
        };

        rlist.elementHeightCallback += index =>
        {
            return GetHeight(nodes[index]);
        };

        rlist.onAddCallback = lt =>
        {
            var type = node.GetType().UnderlyingSystemType;
            RootNode rn = null;
            if (type == typeof(RootNode))
            {
                rn = (RootNode)node;
            }
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Selector"), false, (nm) =>
            {
                if (rn != null) rn.node = new SelectorNode(rn);
                else nodes.Add(new SelectorNode(node));
                SaveData();
            }, "Selector");
            menu.AddItem(new GUIContent("Sequence"), false, (nm) =>
            {
                if (rn != null) rn.node = new SequenceNode(rn);
                else nodes.Add(new SequenceNode(node));
                SaveData();
            }, "Sequence");
            menu.AddItem(new GUIContent("Action"), false, (nm) =>
            {
                if (rn != null) rn.node = new ActionNode(rn);
                else nodes.Add(new ActionNode(node));
                SaveData();
            }, "Action");
            menu.ShowAsContext();
        };

        rlist.onRemoveCallback = lt =>
        {
            nodes.RemoveAt(nodes.Count - 1);
            SaveData();
        };

        rlist.DoList(position);
        return rlist;
    }

    public float GetHeight(List<BTNode> nodes)
    {
        var height = 0f;
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] == null) break;
            var type = nodes[i].GetType().UnderlyingSystemType;
            if (type.IsSubclassOf(typeof(CompositeNode)))
            {
                var nd = (CompositeNode)nodes[i];
                height += nd.childs.Count == 0 ? 70 : GetHeight(nd.childs) + 60;
            }
            else if (type == typeof(ActionNode))
            {
                height += 15;
            }
        }
        return height == 0 ? 20 : height;
    }

    public float GetHeight(BTNode node)
    {
        if (node == null) return 20;
        var type = node.GetType().UnderlyingSystemType;
        if (type.IsSubclassOf(typeof(CompositeNode)))
        {
            var nd = (CompositeNode)node;
            return nd.childs.Count == 0 ? 70 : GetHeight(nd.childs) + 60;
        }
        else if (type == typeof(ActionNode))
        {
            return 15;
        }
        return 20;
    }
}
