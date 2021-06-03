//========= Copyright 2017-2020, HTC Corporation. All rights reserved. ===========
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
#if UNITY_5_4_OR_NEWER
using UnityEditor.Rendering;
#endif
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace HTC.UnityPlugin.Vive3DSP
{
    [InitializeOnLoad]
    public class Vive3DSPVersionCheck : EditorWindow
    {
        [Serializable]
        private struct RepoInfo
        {
            public string tag_name;
            public string body;
            public RepoInfo(string x, string y)
            {
                tag_name = x;
                body = y;
            }
        }

        public const string lastestVersionUrl = "https://api.github.com/repos/Professor3DSound/Vive3DSP/releases/latest";
        public const string pluginUrl = "https://developer.vive.com/resources/knowledgebase/vive-3dsp-sdk-2/";
        public const double versionCheckIntervalMinutes = 30.0;

        private const string nextVersionCheckTimeKey = "Vive3DSP.LastVersionCheckTime";
        private const string fmtIgnoreUpdateKey = "DoNotShowVive3DSPUpdate.v{0}";
        private static string ignoreThisVersionKey = string.Empty;
        private static SerializedObject projectSettingsAsset;
        private static SerializedObject qualitySettingsAsset;

        private static bool completeCheckVersionFlow = false;
        private static UnityWebRequest webReq;
        private static RepoInfo latestRepoInfo = new RepoInfo(string.Empty, string.Empty);
        private static System.Version latestVersion;
        private static Vector2 releaseNoteScrollPosition;
        private static Vector2 settingScrollPosition;
        private static bool showNewVersion;
        private static bool toggleSkipThisVersion = false;
        private static Vive3DSPVersionCheck windowInstance;

        public static bool recommendedWindowOpened { get { return windowInstance != null; } }

        static Vive3DSPVersionCheck()
        {
            EditorApplication.update += CheckVersionAndSettings;
        }

        private static void VersionCheckLog(string msg)
        {
#if VIU_PRINT_FETCH_VERSION_LOG
            using (var outputFile = new StreamWriter("Vive3DSPVersionCheck.log", true))
            {
                outputFile.WriteLine(DateTime.Now.ToString() + " - " + msg + ". Stop fetching until " + UtcDateTimeFromStr(EditorPrefs.GetString(nextVersionCheckTimeKey)).ToLocalTime().ToString());
            }
#endif
        }

        // check vive 3dsp version on github
        private static void CheckVersionAndSettings()
        {
            if (Application.isPlaying)
            {
                EditorApplication.update -= CheckVersionAndSettings;
                return;
            }
            EditorPrefs.SetString(nextVersionCheckTimeKey, UtcDateTimeToStr(DateTime.UtcNow));
            // fetch new version info from github release site
            if ((!completeCheckVersionFlow) && Vive3DSPSettings.autoCheckNewVersion)
            {
                if (webReq == null) // web request not running
                {
                    if (EditorPrefs.HasKey(nextVersionCheckTimeKey) && DateTime.UtcNow < UtcDateTimeFromStr(EditorPrefs.GetString(nextVersionCheckTimeKey)))
                    {
                        VersionCheckLog("Skipped");
                        completeCheckVersionFlow = true;
                        return;
                    }

                    webReq = GetUnityWebRequestAndSend(lastestVersionUrl);
                }

                if (!webReq.isDone)
                {
                    return;
                }

                // On Windows, PlaterSetting is stored at \HKEY_CURRENT_USER\Software\Unity Technologies\Unity Editor 5.x
                EditorPrefs.SetString(nextVersionCheckTimeKey, UtcDateTimeToStr(DateTime.UtcNow.AddMinutes(versionCheckIntervalMinutes)));

                if (UrlSuccess(webReq))
                {
                    latestRepoInfo = JsonUtility.FromJson<RepoInfo>(GetWebText(webReq));
                    VersionCheckLog("Fetched");
                }

                // parse latestVersion and ignoreThisVersionKey
                if (!string.IsNullOrEmpty(latestRepoInfo.tag_name))
                {
                    try
                    {
                        latestVersion = new System.Version(Regex.Replace(latestRepoInfo.tag_name, "[^0-9\\.]", string.Empty));
                        ignoreThisVersionKey = string.Format(fmtIgnoreUpdateKey, latestVersion.ToString());
                    }
                    catch
                    {
                        latestVersion = default(System.Version);
                        ignoreThisVersionKey = string.Empty;
                    }
                }

                webReq.Dispose();
                webReq = null;

                completeCheckVersionFlow = true;
            }
            
            showNewVersion = (!string.IsNullOrEmpty(ignoreThisVersionKey) &&
                              !Vive3DSPProjectSettings.HasIgnoreKey(ignoreThisVersionKey) &&
                              (latestVersion > Vive3DSPAudio.Vive3DSPVersion.Current));

            if (showNewVersion)
            {
                TryOpenRecommendedSettingWindow();
            }

            EditorApplication.update -= CheckVersionAndSettings;
        }

        // Open recommended setting window (with possible new version prompt)
        // won't do any thing if the window is already opened
        public static void TryOpenRecommendedSettingWindow()
        {
            if (recommendedWindowOpened) { return; }

            windowInstance = GetWindow<Vive3DSPVersionCheck>(true, "Vive 3DSP");
            windowInstance.minSize = new Vector2(240f, 400f);
            var rect = windowInstance.position;
            windowInstance.position = new Rect(Mathf.Max(rect.x, 50f), Mathf.Max(rect.y, 50f), rect.width, 400f);
        }

        private static DateTime UtcDateTimeFromStr(string str)
        {
            var utcTicks = default(long);
            if (string.IsNullOrEmpty(str) || !long.TryParse(str, out utcTicks)) { return DateTime.MinValue; }
            return new DateTime(utcTicks, DateTimeKind.Utc);
        }

        private static string UtcDateTimeToStr(DateTime utcDateTime)
        {
            return utcDateTime.Ticks.ToString();
        }

        private static UnityWebRequest GetUnityWebRequestAndSend(string url)
        {
            var webReq = new UnityWebRequest(url);
            if (webReq.downloadHandler == null)
            {
                webReq.downloadHandler = new DownloadHandlerBuffer();
            }
#if UNITY_2017_2_OR_NEWER
            webReq.SendWebRequest();
#endif
            return webReq;
        }

        private static string GetWebText(UnityWebRequest wr)
        {
#if UNITY_5_4_OR_NEWER
            return wr.downloadHandler.text;
#else
            return wr.text;
#endif
        }

        private static bool TryGetWebHeaderValue(UnityWebRequest wr, string headerKey, out string headerValue)
        {
#if UNITY_5_4_OR_NEWER
            headerValue = wr.GetResponseHeader(headerKey);
            return string.IsNullOrEmpty(headerValue);
#else
            if (wr.responseHeaders == null) { headerValue = string.Empty; return false; }
            return wr.responseHeaders.TryGetValue(headerKey, out headerValue);
#endif
        }

        private static bool UrlSuccess(UnityWebRequest wr)
        {
            try
            {
                if (!string.IsNullOrEmpty(wr.error))
                {
                    // API rate limit exceeded, see https://developer.github.com/v3/#rate-limiting
                    Debug.Log("url:" + wr.url);
                    Debug.Log("error:" + wr.error);
                    Debug.Log(GetWebText(wr));

                    string responseHeader;
                    if (TryGetWebHeaderValue(wr, "X-RateLimit-Limit", out responseHeader))
                    {
                        Debug.Log("X-RateLimit-Limit:" + responseHeader);
                    }
                    if (TryGetWebHeaderValue(wr, "X-RateLimit-Remaining", out responseHeader))
                    {
                        Debug.Log("X-RateLimit-Remaining:" + responseHeader);
                    }
                    if (TryGetWebHeaderValue(wr, "X-RateLimit-Reset", out responseHeader))
                    {
                        Debug.Log("X-RateLimit-Reset:" + TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(double.Parse(responseHeader))).ToString());
                    }
                    VersionCheckLog("Failed. Rate limit exceeded");
                    return false;
                }

                if (Regex.IsMatch(GetWebText(wr), "404 not found", RegexOptions.IgnoreCase))
                {
                    Debug.Log("url:" + wr.url);
                    Debug.Log("error:" + wr.error);
                    Debug.Log(GetWebText(wr));
                    VersionCheckLog("Failed. 404 not found");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                VersionCheckLog("Failed. " + e.ToString());
                return false;
            }

            return true;
        }

        public void OnGUI()
        {
            if (showNewVersion)
            {
                EditorGUILayout.HelpBox("New version available:", MessageType.Warning);

                GUILayout.Label("Current version: " + Vive3DSPAudio.Vive3DSPVersion.Current);
                GUILayout.Label("New version: " + latestVersion);

                if (!string.IsNullOrEmpty(latestRepoInfo.body))
                {
                    GUILayout.Label("Release notes:");
                    releaseNoteScrollPosition = GUILayout.BeginScrollView(releaseNoteScrollPosition, GUILayout.Height(250f));
                    EditorGUILayout.HelpBox(latestRepoInfo.body, MessageType.None);
                    GUILayout.EndScrollView();
                }

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(new GUIContent("Get Latest Version", "Goto " + pluginUrl)))
                    {
                        Application.OpenURL(pluginUrl);
                    }

                    GUILayout.FlexibleSpace();

                    toggleSkipThisVersion = GUILayout.Toggle(toggleSkipThisVersion, "Do not prompt for this version again.");
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Close"))
            {
                Close();
            }
        }

        private void OnDestroy()
        {
            if (showNewVersion && toggleSkipThisVersion && !string.IsNullOrEmpty(ignoreThisVersionKey))
            {
                showNewVersion = false;
                Vive3DSPProjectSettings.AddIgnoreKey(ignoreThisVersionKey);
            }

            if (windowInstance == this)
            {
                windowInstance = null;
            }
        }
    }
}