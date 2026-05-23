using System.Collections.Generic;
using OpalStudio.CustomToolbar.Editor.Core;
using OpalStudio.CustomToolbar.Editor.ToolbarElements.MissingReferences.Data;
using OpalStudio.CustomToolbar.Editor.ToolbarElements.MissingReferences.Window;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpalStudio.CustomToolbar.Editor.ToolbarElements.MissingReferences
{
      sealed internal class ToolbarFindMissingReferences : BaseToolbarElement
      {
            private GUIContent _buttonContent;
            private static bool _isScanning;
            private static int _currentIndex;
            private static List<GameObject> _allObjectsToScan;
            private static Dictionary<GameObject, List<MissingReferenceInfo>> _scanResults;

            protected override string Name => "Find Missing References";
            protected override string Tooltip => "Scan the scene and opens a window with the missing references.";

            private readonly static HashSet<string> _IgnoredComponentTypes = new()
            {
                        "UniversalAdditionalCameraData", "UniversalAdditionalLightData",
            };

            private readonly static HashSet<string> _IgnoredFieldNames = new()
            {
                        "m_VolumeTrigger"
            };

            public override void OnInit()
            {
                  Texture icon = EditorGUIUtility.IconContent("d_Search Icon").image;
                  _buttonContent = new GUIContent(icon, this.Tooltip);

                  EditorApplication.update += UpdateScan;
            }

            public override void OnDrawInToolbar()
            {
                  this.Enabled = !EditorApplication.isPlayingOrWillChangePlaymode && !_isScanning;

                  using (new EditorGUI.DisabledScope(!this.Enabled))
                  {
                        string buttonText = _isScanning ? "Scanning..." : "";
                        GUIContent content = _isScanning ? new GUIContent(buttonText, _buttonContent.image, "Scanning in progress...") : _buttonContent;

                        if (GUILayout.Button(content, ToolbarStyles.CommandButtonStyle, GUILayout.Width(this.Width)))
                        {
                              StartScan();
                        }
                  }
            }

            private static void StartScan()
            {
                  if (_isScanning)
                  {
                        return;
                  }

                  Scene scene = SceneManager.GetActiveScene();
                  GameObject[] allGameObjects = scene.GetRootGameObjects();

                  _allObjectsToScan = new List<GameObject>();

                  foreach (GameObject rootGo in allGameObjects)
                  {
                        CollectAllGameObjects(rootGo, _allObjectsToScan);
                  }

                  _scanResults = new Dictionary<GameObject, List<MissingReferenceInfo>>();
                  _currentIndex = 0;
                  _isScanning = true;
            }

            private static void UpdateScan()
            {
                  if (!_isScanning)
                  {
                        return;
                  }

                  const int objectsPerFrame = 5;
                  int endIndex = Mathf.Min(_currentIndex + objectsPerFrame, _allObjectsToScan.Count);

                  for (int i = _currentIndex; i < endIndex; i++)
                  {
                        float progress = (float)i / _allObjectsToScan.Count;
                        string progressText = $"Scanning objects... ({i + 1}/{_allObjectsToScan.Count})";

                        if (EditorUtility.DisplayCancelableProgressBar("Finding Missing References", progressText, progress))
                        {
                              FinishScan();

                              return;
                        }

                        ScanSingleGameObject(_allObjectsToScan[i], _scanResults);
                  }

                  _currentIndex = endIndex;

                  if (_currentIndex >= _allObjectsToScan.Count)
                  {
                        FinishScan();
                  }
            }

            private static void FinishScan()
            {
                  EditorUtility.ClearProgressBar();
                  _isScanning = false;

                  MissingReferencesWindow.ShowWindow(_scanResults);

                  _allObjectsToScan = null;
                  _scanResults = null;
            }

            private static void CollectAllGameObjects(GameObject parent, List<GameObject> collection)
            {
                  collection.Add(parent);

                  foreach (Transform child in parent.transform)
                  {
                        CollectAllGameObjects(child.gameObject, collection);
                  }
            }

            private static void ScanSingleGameObject(GameObject go, Dictionary<GameObject, List<MissingReferenceInfo>> results)
            {
                  MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();

                  foreach (MonoBehaviour component in components)
                  {
                        if (component == null)
                        {
                              AddResult(go, new MissingReferenceInfo { IsScriptMissing = true }, results);

                              continue;
                        }

                        if (_IgnoredComponentTypes.Contains(component.GetType().Name))
                        {
                              continue;
                        }

                        var serializedObject = new SerializedObject(component);
                        SerializedProperty property = serializedObject.GetIterator();

                        while (property.NextVisible(true))
                        {
                              if (property.propertyType != SerializedPropertyType.ObjectReference || property.objectReferenceValue != null)
                              {
                                    continue;
                              }

                              if (_IgnoredFieldNames.Contains(property.name))
                              {
                                    continue;
                              }

                              AddResult(go, new MissingReferenceInfo
                              {
                                          ComponentName = component.GetType().Name,
                                          FieldName = property.displayName,
                                          IsScriptMissing = false
                              }, results);
                        }
                  }
            }

            private static void AddResult(GameObject go, MissingReferenceInfo info, Dictionary<GameObject, List<MissingReferenceInfo>> results)
            {
                  if (!results.ContainsKey(go))
                  {
                        results[go] = new List<MissingReferenceInfo>();
                  }

                  results[go].Add(info);
            }
      }
}
