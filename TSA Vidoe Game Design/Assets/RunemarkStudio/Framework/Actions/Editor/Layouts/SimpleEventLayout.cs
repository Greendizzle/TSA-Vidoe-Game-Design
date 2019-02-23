using Runemark.VisualEditor.Actions;
using UnityEngine;
using UnityEditor;
using Runemark.VisualEditor.Utility;


namespace Runemark.VisualEditor
{
	[CustomNodeLayout(typeof(SimpleEvent), true)]
	public class SimpleEventLayout : DefaultLayout 
	{
		public SimpleEventLayout(Node node) : base(node)
		{
			headerColor = new Color(0.43f, 0.10f, 0.10f, 1f);
			bodyColor = new Color(0,0,0,1f);

			minBodyHeight = 40;
			showTransitionInputLabel = false;
			showTransitionOutputLabel = false;
		}	
	}


	[CustomEditor(typeof(SimpleEvent))]
	public class SimpleEventLayoutInspector : NodeInspector
	{
        protected override bool NameEditable
        {
            get
            {
                SimpleEvent myTarget = (SimpleEvent)target;
                return myTarget.CanCopy;
            }
        }
        

        protected override void onGUI()
		{
            SimpleEvent myTarget = (SimpleEvent)target;
		}        

    }
}

