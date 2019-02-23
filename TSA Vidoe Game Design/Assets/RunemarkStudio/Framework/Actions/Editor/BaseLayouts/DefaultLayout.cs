using UnityEngine;
using Runemark.VisualEditor.Utility;
using System.Collections.Generic;

namespace Runemark.VisualEditor
{
	public class DefaultLayout : NodeLayout 
	{
		protected sealed override float headerHeight { get { return minHeaderHeight; } }
		protected sealed override float bodyHeight { get { return Mathf.Max(minBodyHeight, Mathf.Max(inputs.Count, outputs.Count) * (Pin.PIN_SIZE+10) + 10); } }

		protected float minBodyHeight;
        protected float minHeaderHeight = 30;

		public DefaultLayout(Node node)	: base(node)
		{
			headerColor = BuiltInColors.Default;
			bodyColor = Color.black;

			width = 150f;
			minBodyHeight = 60f;

			showInputLabel = true;
			showOutputLabel = true;
			showTransitionInputLabel = true;
			showTransitionOutputLabel = true;
		}

		protected override void onGUIHeader()
		{		
			GUI.Label(headerRect, Title, titleStyle);
		}

		protected override void onGUIBody()
		{	
			Rect iRect = new Rect(bodyRect.x + 10, bodyRect.y + 10, Pin.PIN_SIZE, Pin.PIN_SIZE);
			Rect oRect = new Rect(bodyRect.x + Rect.width - Pin.PIN_SIZE - 5, bodyRect.y + 10, Pin.PIN_SIZE, Pin.PIN_SIZE);

			// Draw Transitions
			foreach (var t in transitions)
			{
				bool isInput = t.PinType == PinType.TransIn;
				var r = (isInput) ? iRect : oRect;

				DrawPin(r, t, (isInput) ? Vector2.left : Vector2.right, isInput ? !showTransitionInputLabel : !showTransitionOutputLabel);

				if(isInput) iRect.y += iRect.height + 10;
				else oRect.y += oRect.height + 10;
			}

            // Draw inputs
            List<string> pins = new List<string>();
            foreach (var i in inputs)
			{
				DrawPin(iRect, i, Vector2.left, !showInputLabel);
				iRect.y += iRect.height + 10;
                pins.Add(i.Name);
            }

            // Draw input variables
            iRect.width = width - 5 - (Pin.PIN_SIZE + 10) * 2;
            iRect.height = 25;
            var variables = Node.Variables.GetByGroup("Input").FindAll(x => !pins.Contains(x.Name));
            if (inputs.Count == 0)
                iRect.y = bodyRect.y - ((variables.Count % 2 == 0) ? variables.Count / 2 * (iRect.height + 10) - 5 : (variables.Count - 1) / 2 * (iRect.height + 10) + iRect.height / 2);

            foreach (var v in variables)
            {
                DrawInput(iRect, v);
                iRect.y += iRect.height + 10;
            }

            // Draw outputs
            foreach (var o in outputs)
			{
				DrawPin(oRect, o, Vector2.right, !showOutputLabel);
				oRect.y += oRect.height + 10;				
			}	
		}

		protected override void InitStyle()
		{
			base.InitStyle();
			titleStyle.fontSize = 16;
		}
	}
}