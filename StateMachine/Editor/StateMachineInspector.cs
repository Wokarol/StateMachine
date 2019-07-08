using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using System.Linq;
using System;
using Wokarol.StateMachineSystem;
using System.Text.RegularExpressions;
using System.Text;

public class StateMachineInspector : EditorWindow
{
    private const string lockedButton = "in lockbutton on.png";
    private const string unlockedButton = "in lockbutton.png";
    private const int historyLenght = 4;

    // Selection
    private bool locked;
    private GameObject[] selectedGameObjects;

    // Cache
    StringBuilder builder = new StringBuilder();

    // Initializes window
    [MenuItem("Tools/State Machine Debbuger")]
    static void Init()
    {
        StateMachineInspector inspector = CreateInstance<StateMachineInspector>();
        inspector.titleContent = new GUIContent("SM Inspector");
        inspector.Show();

    }

    private void OnEnable()
    {
        // Adding selection checks
        Selection.selectionChanged += Repaint;

        UpdateSelection();
    }

    private void OnDisable()
    {
        // Removeing selection checks
        Selection.selectionChanged -= Repaint;
    }

    // Updates selection if window isn't locked
    private void UpdateSelection()
    {
        if (!locked) {
            selectedGameObjects = Selection.gameObjects;
        }
    }


    private void Update() {
        Repaint();
    }

    private void OnGUI()
    {
        UpdateSelection();

        EditorGUILayout.Space();

        if(selectedGameObjects.Length == 0) {
            EditorGUILayout.HelpBox("Nothing selected", MessageType.None);
        }

        // GUI
        foreach (var target in selectedGameObjects) {
            foreach (var component in target.GetComponents<MonoBehaviour>()) {
                Type type = component.GetType();
                var fields = GetAllFieldsFromType(type);
                var stateMachineFields = fields.Where(f => f.FieldType == typeof(StateMachine)).Select(f => new NamedField<StateMachine>(ParseToNiceName(f.Name), f.GetValue(component) as StateMachine));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"{target.name} => {component.GetType().Name}");
                EditorGUI.indentLevel += 1;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                foreach (var sm in stateMachineFields) {
                    builder.Clear();
                    builder.AppendLine($"{sm.Name}");
                    // TODO: Indent?
                    builder.AppendLine($"--------------------------");
                    if (Application.isPlaying) {
                        if(sm.Value != null) {
                            // Current State
                            builder.AppendLine($"Current State: <b>{(sm.Value.CurrentState != null ? sm.Value.CurrentState.Name : "null")}</b>");
                            var historyPopped = new Stack<State>();

                            // History
                            while(sm.Value.History.Count > 0) {
                                if(historyPopped.Count != historyLenght) {
                                    var state = sm.Value.History.Pop();
                                    historyPopped.Push(state);
                                    if(historyPopped.Count == 1) {
                                        builder.AppendLine($"History: \t{state.Name}");
                                    } else {
                                        builder.AppendLine($"\t{state.Name}");
                                    }
                                    
                                } else {
                                    builder.AppendLine($"\t ... ({sm.Value.History.Count}) ...");
                                    break;
                                }
                            }
                            while(historyPopped.Count > 0) {
                                sm.Value.History.Push(historyPopped.Pop());
                            }

                            //builder.AppendLine($"--------------------------");


                        } else {
                            builder.AppendLine($"State machine is null");
                        }
                    } else {
                        builder.AppendLine($"Waiting for playmode");
                    }


                    var style = GUI.skin.GetStyle("HelpBox");
                    style.richText = true;
                    EditorGUILayout.HelpBox(builder.ToString(), MessageType.None);
                    style.richText = false;
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUI.indentLevel -= 1;
            }
        }
        // ===
    }

    string ParseToNiceName(string name)
    {
        name = Regex.Replace(name, @"<(\w+)>k__BackingField", @"$1");
        name = Regex.Replace(name, @"^[a-z]", m => m.ToString().ToUpper());
        name = Regex.Replace(name, @"([a-z])([A-Z])", @"$1 $2");
        name = Regex.Replace(name, @"([a-zA-Z])(\d)", @"$1 $2");
        name = Regex.Replace(name, @"(\d)([a-zA-Z])", @"$1 $2");
        return name;
    }

    FieldInfo[] GetAllFieldsFromType(Type type)
    {
        List<FieldInfo> fields = new List<FieldInfo>();
        Type baseType = null;
        do {
            baseType = type.BaseType;
            var foundFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var foundField in foundFields) {
                if (!fields.Select(f => f.Name).ToList().Contains(foundField.Name))
                    fields.Add(foundField);
            }
            type = baseType;
        } while (baseType != typeof(MonoBehaviour) && baseType != typeof(object));
        return fields.ToArray();
    }

    // Showing button
    private void ShowButton(Rect rect)
    {
        locked = GUI.Toggle(rect, locked, GUIContent.none, "IN LockButton");
    }

    private class NamedField<T>
    {
        public readonly string Name;
        public readonly T Value;

        public NamedField(string name, T value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value;
        }
    }
}
