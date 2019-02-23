using Runemark.Common;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace Runemark.DialogueSystem.UI
{
    [CustomEditor(typeof(UIElementAnswer), true)]
    public class DialogueSystemUIAnswerEditor : DialogueSystemUIElementEditor
    {
        protected override void onGUI()
        {
            EditorGUIExtension.SimpleBox("UI Elements", 5, "", delegate ()
            {
                DrawGUI();
            });
        }

        protected virtual void DrawGUI() { }
    }

    [CustomEditor(typeof(UIElementAnswerText), true)]
    public class DialogueSystemUITextAnswerEditor : DialogueSystemUIAnswerEditor
    {
        protected override void DrawGUI()
        {            
            var myTarget = (UIElementAnswerText)target;

            myTarget.IndexUI = (Text)EditorGUILayout.ObjectField("Index", myTarget.IndexUI, typeof(Text), true);
            myTarget.TextUI = (Text)EditorGUILayout.ObjectField("Text", myTarget.TextUI, typeof(Text), true);
        }
    }

    [CustomEditor(typeof(UIElementAnswerIcon), true)]
    public class DialogueSystemUIIconAnswerEditor : DialogueSystemUIAnswerEditor
    {
        UIElementAnswerIcon _myTarget;
        ReorderableList _iconList;

        private void OnEnable()
        {
            if(_myTarget == null) _myTarget = (UIElementAnswerIcon)target;

            _iconList = new ReorderableList(_myTarget.Icons, typeof(IconData), true, true, true, true);
            _iconList.elementHeight = 50;
            _iconList.drawHeaderCallback = DrawIconListHeader;
            _iconList.drawElementCallback = DrawIconListElement;
        }

        protected override void DrawGUI()
        {
            if (_myTarget == null) _myTarget = (UIElementAnswerIcon)target;
            _myTarget.Image = (Image)EditorGUILayout.ObjectField("Image", _myTarget.Image, typeof(Image), true);

            var lib = _myTarget.GetComponentInParent<IconLibrary>();
            if (lib == null)
                _iconList.DoLayoutList();
            else
            {
                _myTarget.Icons.Clear();
                EditorGUILayout.HelpBox("This element uses Icons from an IconLibrary parent component", MessageType.Info);
                if (GUILayout.Button("Select Icon Library"))
                {
                    Selection.activeGameObject = lib.gameObject;
                }
            }
        }



        void DrawIconListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Icons");
        }
        void DrawIconListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            Rect r = new Rect(rect.x + 5, rect.y + 5, rect.height - 10, rect.height - 10);
            _myTarget.Icons[index].Icon = (Sprite)EditorGUI.ObjectField(r, _myTarget.Icons[index].Icon, typeof(Sprite), true);

            r = new Rect(rect.x + rect.height + 10, rect.y + (rect.height - 20) / 2, rect.width - rect.height - 15, 20);
            _myTarget.Icons[index].Name = EditorGUI.TextField(r, _myTarget.Icons[index].Name);

             
        }
    }
}