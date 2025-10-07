using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WMOGroupRenderer))]
public class WMOGroupRendererEditor : Editor
{
    private Dictionary<string, bool> enabledOptions = new Dictionary<string, bool>();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var wmoGroup = ((WMOGroupRenderer)target).WMOGroup;

        if (wmoGroup == null) return;

        RenderProperties(wmoGroup, "");
    }

    private void RenderProperties(object obj, string name)
    {
        if(obj == null)
        {
            EditorGUILayout.LabelField($"Null");
            return;
        }
        foreach (var prop in obj.GetType().GetProperties())
        {
            if(IsSimple(prop))
            {
                EditorGUILayout.LabelField(prop.Name, $"{prop.GetValue(obj)?.ToString() ?? "None"}");
            }
            else if (prop.PropertyType.IsClass)
            {
                name = $"{name}/{prop.Name}";
                if (!enabledOptions.ContainsKey(name))
                {
                    enabledOptions[name] = false;
                }
                enabledOptions[name] = EditorGUILayout.Foldout(enabledOptions[name], prop.Name);
                if (enabledOptions[name])
                {
                    EditorGUI.indentLevel++;
                    RenderProperties(prop.GetValue(obj), name);
                    EditorGUI.indentLevel--;
                }
            }
        }
    }

    static bool IsSimple(PropertyInfo prop)
    {
        var type = prop.PropertyType;

        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime);
    }
}
