//========= Copyright 2017-2020, HTC Corporation. All rights reserved. ===========

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace HTC.UnityPlugin.Vive3DSP
{
    public static class Vive3DSPSettingsEditor
    {
        private static Vector2 s_scrollValue = Vector2.zero;
        private static float s_warningHeight;
        private static GUIStyle s_labelStyle;
        private static string s_defaultAssetPath;

        public static string defaultAssetPath
        {
            get
            {
                if (s_defaultAssetPath == null)
                {
                    var ms = MonoScript.FromScriptableObject(Vive3DSPSettings.Instance);
                    var path = AssetDatabase.GetAssetPath(ms);
                    path = System.IO.Path.GetDirectoryName(path);
                    s_defaultAssetPath = path.Substring(0, path.Length - "Scripts".Length) + "Resources/" + Vive3DSPSettings.DEFAULT_RESOURCE_PATH + ".asset";
                }

                return s_defaultAssetPath;
            }
        }


        [PreferenceItem("VIVE 3DSP Settings")]
        private static void OnVive3DSPPreferenceGUI()
        {
#if UNITY_2017_1_OR_NEWER
            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.LabelField("Compiling...");
                return;
            }
#endif
            if (s_labelStyle == null)
            {
                s_labelStyle = new GUIStyle(EditorStyles.label);
                s_labelStyle.richText = true;

            }

            s_scrollValue = EditorGUILayout.BeginScrollView(s_scrollValue);
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("<b>VIVE 3DSP v" + Vive3DSPAudio.Vive3DSPVersion.Current + "</b>", s_labelStyle);

            GUILayout.BeginHorizontal();
            
            Vive3DSPSettings.autoCheckNewVersion = EditorGUILayout.ToggleLeft("Auto Check Latest Version", Vive3DSPSettings.autoCheckNewVersion);

            if (EditorGUI.EndChangeCheck())
            {
                var assetPath = AssetDatabase.GetAssetPath(Vive3DSPSettings.Instance);
                if (string.IsNullOrEmpty(assetPath))
                {
                    AssetDatabase.CreateAsset(Vive3DSPSettings.Instance, defaultAssetPath);
                }

                EditorUtility.SetDirty(Vive3DSPSettings.Instance);
            }

            ShowUrlLinkButton(Vive3DSPVersionCheck.pluginUrl, "Get Latest Release");

            ShowUrlLinkButton(Vive3DSPAudio.VIVE_3DSP_FORUM_URL, "Visit VIVE 3DSP Forum");

            GUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        private static void ShowUrlLinkButton(string url, string label = "Get Plugin")
        {
            if (GUILayout.Button(new GUIContent(label, url), GUILayout.ExpandWidth(false)))
            {
                Application.OpenURL(url);
            }
        }
    }
}