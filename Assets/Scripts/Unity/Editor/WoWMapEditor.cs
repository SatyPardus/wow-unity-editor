using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

public class WoWMapEditor : EditorWindow
{
    [MenuItem("WoW Utilities/Map Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(WoWMapEditor));
    }

    private void OnGUI()
    {
        
    }
}
