using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Runemark.Common
{
	public static partial class EditorGUIExtension
	{
		public static Rect NULL_RECT = new Rect(-1000000,-1000000,-1000000,-1000000);

        #region Zoom Area
        private const float kEditorWindowTabHeight = 21.0f;
        private static Matrix4x4 _prevGuiMatrix;

        public static Rect BeginZoomArea(float zoomScale, Rect screenCoordsArea)
        {
            GUI.EndGroup();        // End the group Unity begins automatically for an EditorWindow to clip out the window tab. This allows us to draw outside of the size of the EditorWindow.

            Rect clippedArea = screenCoordsArea.ScaleSizeBy(1.0f / zoomScale, screenCoordsArea.TopLeft());
            clippedArea.y += kEditorWindowTabHeight;
            GUI.BeginGroup(clippedArea);

            _prevGuiMatrix = GUI.matrix;
            Matrix4x4 translation = Matrix4x4.TRS(clippedArea.TopLeft(), Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f));
            GUI.matrix = translation * scale * translation.inverse * GUI.matrix;

            return clippedArea;
        }

        public static void EndZoomArea()
        {
            GUI.matrix = _prevGuiMatrix;
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(0.0f, kEditorWindowTabHeight, Screen.width, Screen.height));
        }
        #endregion

        #region Smart Field
        public static object SmartField(string label, System.Type type, object value)
        {
            return SmartField(NULL_RECT, label, type, value);
        }

        public static object SmartField(Rect rect, System.Type type, object value, GUIStyle style = null) { return SmartField(rect, "", type, value, style); }
        public static object SmartField(Rect rect, string label, System.Type type, object value, GUIStyle style = null) { return SmartField(rect, new GUIContent(label), type, value, style); }
        public static object SmartField(Rect rect, GUIContent label, System.Type type, object value, GUIStyle style = null)
        {
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Text);

            if (type == typeof(string))
            {
                if (style == null) style = EditorStyles.textField;
                if (value == null) value = "";

                if (value.GetType() != typeof(string))
                    value = System.Convert.ChangeType(value, typeof(string));

                if (rect == NULL_RECT)
                    return EditorGUILayout.TextField(label, (string)value, style);
                else
                    return EditorGUI.TextField(rect, label, (string)value, style);
            }

            else if (type == typeof(int))
            {
                if (style == null) style = EditorStyles.numberField;
                if (value == null) value = 0;

                if (value.GetType() != typeof(int))
                    value = System.Convert.ChangeType(value, typeof(int));

                if (rect == NULL_RECT)
                    return EditorGUILayout.IntField(label, (int)value, style);
                else
                    return EditorGUI.IntField(rect, label, (int)value, style);
            }

            else if (type == typeof(float))
            {
                if (style == null) style = EditorStyles.numberField;
                if (value == null) value = 0f;

                if (value.GetType() != typeof(float))
                    value = System.Convert.ChangeType(value, typeof(float));

                if (rect == NULL_RECT)
                    return EditorGUILayout.FloatField(label, (float)value, style);
                else
                    return EditorGUI.FloatField(rect, label, (float)value, style);
            }

            else if (type == typeof(bool))
            {
                if (style == null) style = EditorStyles.label;
                if (value == null) value = false;

                if (value.GetType() != typeof(bool))
                    value = System.Convert.ChangeType(value, typeof(bool));

                if (rect == NULL_RECT)
                    return EditorGUILayout.ToggleLeft(label, (bool)value);
                else
                    return EditorGUI.ToggleLeft(rect, label, (bool)value);
            }

            else if (type == typeof(object))
            {                
                if (rect == NULL_RECT)  GUILayout.Label(label);
                else                    GUI.Label(rect, label);
                return null;
            }
            
            Debug.LogError("No field for " + type);
            return null;
        }
        #endregion

        #region Foldout & Simple BOX

        static GUIStyle _enableFlagStyle;
        static void InitFlagStyle()
        {
            if (_enableFlagStyle == null)
            {
                _enableFlagStyle = new GUIStyle(GUI.skin.label);
                _enableFlagStyle.richText = true;
                _enableFlagStyle.fontStyle = FontStyle.Bold;
            }
        }

        public delegate void BoxRectContent(Rect rect);
        public delegate void BoxContent();
        public static void FoldoutBox(string title, ref bool foldout, int enabled, BoxContent boxContent)
        {
            InitFlagStyle();

            string flag = "";
            if (enabled == 0) flag = "<color=#ff0000ff>Disabled</color>";
            if (enabled == 1) flag = "<color=#008000ff>Enabled</color>";

            GUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            foldout = EditorGUILayout.Foldout(foldout, title);

            var size = _enableFlagStyle.CalcSize(new GUIContent(flag));

            GUILayout.Label(flag, _enableFlagStyle, GUILayout.Width(size.x));
            EditorGUILayout.EndHorizontal();


            if (foldout && boxContent != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical("HelpBox");
                boxContent();
                EditorGUILayout.EndVertical();
                GUILayout.Space(10);
                GUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        public static void SimpleBox(string title, int padding, string style, BoxContent boxContent)
        {
            if (style == "") style = "box";

            EditorGUILayout.BeginVertical(style);

            if (title != "")
            {
                GUILayout.BeginVertical("ObjectPickerGroupHeader");
                GUILayout.Label(title);
                GUILayout.EndVertical();
            }

            GUILayout.Space(padding);

            boxContent();

            GUILayout.Space(padding);
            EditorGUILayout.EndVertical();
        }

        public static void SimpleBox(Rect rect, string title, int padding, string style, BoxRectContent boxContent)
        {
            if (style == "") style = "box";

            GUI.Box(rect, "", style);
            float y = rect.y;

            if (title != "")
            {
                GUI.Box(new Rect(rect.x, y, rect.width, 25), "", "ObjectPickerGroupHeader");
                GUI.Label(new Rect(rect.x, y, rect.width, 25), title);
                y += 25;
            }

            boxContent(new Rect(rect.x, y + padding, rect.width, rect.height - y - 2*padding));
        }


        #endregion

        #region Horizontal Line

        public static void HorizontalLine(float width)
        {
            GUILayout.Box("", GUILayout.Height(1), GUILayout.Width(width));
        }


        #endregion

        #region Curve
        public static void Curve(Rect start, Rect end, Color color, Vector2 direction)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);

            Vector3 startTan = startPos + new Vector3(direction.x, -direction.y, 0) * 50;
            Vector3 endTan = endPos - new Vector3(direction.x, -direction.y, 0) * 50;

            Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 3);
        }
        #endregion

         // Not used, experimental

        #region Display Waveform [Not used]
        /*
                public static Texture2D GenerateWaveform(AudioClip clip)
                {
                    Texture2D tex = Texture2D.whiteTexture;
                    if (clip != null)
                    {
                        int resolution = 60;

                        float[] samples = new float[clip.samples * clip.channels];
                        clip.GetData(samples, 0);

                        float[] waveform = new float[samples.Length / resolution];

                        float max = 0, min = 0;
                        for (int i = 0; i < waveform.Length; i++)
                        {
                            waveform[i] = 0;
                            for (int j = 0; j < resolution; j++)
                            {						
                                waveform[i] += Mathf.Abs(samples[(i * resolution) + j]);
                            }
                            waveform[i] /= resolution;

                            if (waveform[i] > max)
                                max = waveform[i];
                            if (waveform[i] < min)
                                min = waveform[i];
                        }

                        Debug.Log("Resize: " + waveform.Length + ", "  + max + " + "+min);
                        // tex = new Texture2D(waveform.Length, 


                        tex.Resize(waveform.Length, 10*Mathf.CeilToInt(Mathf.Abs(max) + Mathf.Abs(min)));

                        for (int cnt = 0; cnt < waveform.Length; cnt++)
                        {
                            int x = cnt;
                            for (int y = Mathf.RoundToInt(-waveform[cnt] * 10); y < Mathf.RoundToInt(waveform[cnt] * 10); y++)
                            {
                                tex.SetPixel(x, Mathf.RoundToInt(tex.height / 2 + y), Color.cyan);
                            }
                        }

                        tex.wrapMode = TextureWrapMode.Repeat;
                    }

                    Debug.Log("Created : " + tex.width + ","+tex.height);

                    return tex;
                }*/
        #endregion

    }


    public class SmartPopup
    {
        public int Index { get; private set; }
        string[] _elements;
        
        public SmartPopup(List<string> elements, int selectedIndex = 0)
        {
            _elements = elements.ToArray();
            Index = selectedIndex;
        }
        public SmartPopup(string[] elements, int selectedIndex = 0)
        {
            _elements = elements;
            Index = selectedIndex;
        }

        public bool Draw(string label)
        {
            return Draw(EditorGUIExtension.NULL_RECT, label);
        }

        public bool Draw(Rect rect, string label)
        {
            int index = 0;

            if (rect == EditorGUIExtension.NULL_RECT)
               index = EditorGUILayout.Popup(label, Index, _elements);
            else
               index = EditorGUI.Popup(rect, label, Index, _elements);

            if (index != Index)
            {
                Index = index;
                return true;
            }
            return false;
        }
    }
}