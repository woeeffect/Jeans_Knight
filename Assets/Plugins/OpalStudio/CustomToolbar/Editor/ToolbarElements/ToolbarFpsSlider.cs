using UnityEditor;
using UnityEngine;

namespace OpalStudio.CustomToolbar.Editor.ToolbarElements
{
      sealed internal class ToolbarFpsSlider : BaseToolbarElement
      {
            private const int _MinFpsValue = 0;
            private const int _MaxFpsValue = 600;
            private const string _ToolbarFpsSliderKey = "CustomToolbar.ToolbarFpsSlider.Value";

            private int _currentFPS;
            private GUIContent _buttonContent;

            protected override string Name => "FPS Slider";
            protected override string Tooltip => "Controls Application.targetFrameRate. Set to 0 for unlimited FPS.";

            public override void OnInit()
            {
                  this.Width = 200;
                  _buttonContent = new GUIContent("FPS", this.Tooltip);

                  _currentFPS = EditorPrefs.GetInt(_ToolbarFpsSliderKey, 0);

                  Application.targetFrameRate = _currentFPS;
            }

            public override void OnDrawInToolbar()
            {
                  _buttonContent.text = _currentFPS == _MinFpsValue ? "FPS (∞)" : "FPS";

                  EditorGUILayout.LabelField(_buttonContent, GUILayout.Width(25));

                  EditorGUI.BeginChangeCheck();

                  _currentFPS = Mathf.RoundToInt(EditorGUILayout.Slider(_currentFPS, _MinFpsValue, _MaxFpsValue, GUILayout.Width(this.Width - 65)));

                  if (EditorGUI.EndChangeCheck())
                  {
                        Application.targetFrameRate = (_currentFPS == _MinFpsValue) ? -1 : _currentFPS;
                        EditorPrefs.SetInt(_ToolbarFpsSliderKey, _currentFPS);
                  }
            }
      }
}

