using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using System;
using System.IO;
using UnityEditor;

public class SearchableItemSelectWindow : EditorWindow
{
    private string query = "";
    private Vector2 scroll;

    private Dictionary<string, bool> foldouts = new Dictionary<string, bool>();
    private Dictionary<string, string[]> options;
    private Action<string> OnSelect;

    public static void Create(string title, 
                              Dictionary<string, string[]> options,
                              Action<string> OnSelect)
    {
        var window = GetWindow<SearchableItemSelectWindow>(utility: true,
                                                           title: title);
        window.Open(OnSelect, options);
    }

    public void Open(Action<string> OnSelect,
                     Dictionary<string, string[]> options)
    {
        this.OnSelect = OnSelect;
        this.options = options;

        foldouts.Clear();

        Show();
    }

    private void OnGUI()
    {
        if (options == null) Close();

        query = EditorGUILayout.TextField("Search", query);

        scroll = EditorGUILayout.BeginScrollView(scroll);

        var valid = new HashSet<string>(options.Values
                                               .SelectMany(option => option)
                                               .Where(option => option.Contains(query)));

        foreach (var category in options.Where(c => c.Value.Any(v => valid.Contains(v))))
        {
            string heading = category.Key;

            bool show;
            foldouts.TryGetValue(heading, out show);
            show = EditorGUILayout.Foldout(show, heading);
            foldouts[heading] = show;

            if (show)
            {
                foreach (string option in category.Value)
                {
                    if ((query == "" || option.Contains(query))
                     && GUILayout.Button(option, EditorStyles.miniButton))
                    {
                        Close();

                        OnSelect(option);
                    }
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }
}
