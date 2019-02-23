using UnityEngine;
using Runemark.VisualEditor;
using Runemark.VisualEditor.Actions;
using Runemark.VisualEditor.Utility;
using UnityEditor;
using Runemark.Common;

namespace Runemark.VisualEditor
{
    [CustomNodeLayout(typeof(Constant))]
    public class ConstantLayout : CompactLayout
    {
        protected override string Title { get { return ""; } }

        Constant _node { get { return ConvertNode<Constant>(); } }
        public ConstantLayout(Constant node) : base(node)
        {
            headerColor = new Color(0, 0, 0, .8f);
            width = 200;
            showOutputLabel = false;

            node.OnTypeChanged += InitFieldStyle;
        }
        protected override void InitFieldStyle()
        {
            base.InitFieldStyle();
            var background = VisualEditorGUIStyle.GetTexture(BuiltInColors.GetDark(_node.Type), new Color(0, 0, 0, 0), true, true, true, true);
            fieldStyle.normal.background = background.Texture;
            fieldStyle.active.background = background.Texture;
            fieldStyle.focused.background = background.Texture;
            fieldStyle.hover.background = background.Texture;
        }
    }

    [CustomEditor(typeof(Constant))]
    public class ConstantNodeInspector : NodeInspector
    {
        protected override void onGUI()
        {
            Constant myTarget = (Constant)target;
        }
    }
}