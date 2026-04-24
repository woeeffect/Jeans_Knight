using UnityEditor;
using UnityEngine;

namespace OpalStudio.CustomToolbar.Editor.ToolbarElements
{
      sealed internal class ToolbarTimeSlider : BaseToolbarElement
      {
            private const float _MinTimeScale = 0f;
            private const float _MaxTimeScale = 10f;
            private const string _ToolbarTimeSliderKey = "CustomToolbar.ToolbarTimeSlider.Value";

            private float _currentTimeScale;
            private GUIContent _buttonContent;

            protected override string Name => "Timescale Slider";
            protected override string Tooltip => "Controls Time.timeScale to slow down or speed up the game.";

            public override void OnInit()
            {
                  this.Width = 200;

                  _currentTimeScale = EditorPrefs.GetFloat(_ToolbarTimeSliderKey, 1.0f);
                  Time.timeScale = _currentTimeScale;
                  _buttonContent = new GUIContent("Time", this.Tooltip);
            }

            public override void OnPlayModeStateChanged(PlayModeStateChange state)
            {
                  if (state is PlayModeStateChange.ExitingPlayMode or PlayModeStateChange.EnteredEditMode)
                  {
                        _currentTimeScale = 1.0f;
                        Time.timeScale = _currentTimeScale;

                        EditorPrefs.SetFloat(_ToolbarTimeSliderKey, _currentTimeScale);
                  }

                  this.Enabled = (state == PlayModeStateChange.EnteredPlayMode);
            }

            public override void OnDrawInToolbar()
            {
                  using (new EditorGUI.DisabledScope(!this.Enabled))
                  {
                        EditorGUILayout.LabelField(_buttonContent, GUILayout.Width(35));

                        EditorGUI.BeginChangeCheck();

                        _currentTimeScale = EditorGUILayout.Slider(_currentTimeScale, _MinTimeScale, _MaxTimeScale, GUILayout.Width(this.Width - 40));

                        if (EditorGUI.EndChangeCheck())
                        {
                              Time.timeScale = _currentTimeScale;

                              EditorPrefs.SetFloat(_ToolbarTimeSliderKey, _currentTimeScale);
                        }
                  }
            }
      }
}
