using UnityEngine;
using Runemark.VisualEditor.Utility;
using UnityEditor;
using Runemark.Common;
using System.Collections.Generic;

namespace Runemark.VisualEditor
{
	public class CompactLayout : NodeLayout 
	{
		protected sealed override float headerHeight { get { return Mathf.Max(minHeaderHeight, Mathf.Max(inputs.Count, outputs.Count) * (Pin.PIN_SIZE+10) + 10); } }
		protected sealed override float bodyHeight { get { return 0; } }

		protected float minHeaderHeight;

		public CompactLayout(Node node) : base(node)
		{
			headerColor = BuiltInColors.Default;
			bodyColor = Color.black;

			width = 150f;
			minHeaderHeight = 40f;

			showInputLabel = false;
			showOutputLabel = false;
			showTransitionInputLabel = false;
			showTransitionOutputLabel = false;
		}

		protected override void onGUIHeader()
		{		
			GUI.Label(headerRect, Title, titleStyle);

			float centerY = headerRect.y + headerRect.height / 2;

            int iCount = Node.PinCollection.Count(PinType.Input) + Node.PinCollection.Count(PinType.TransIn);
            int oCount = Node.PinCollection.Count() - iCount;


            Rect iRect = new Rect(headerRect.x + 10, 
				centerY - ((iCount % 2 == 0) ? iCount / 2 * (Pin.PIN_SIZE + 10) - 5: (iCount - 1)/2 * (Pin.PIN_SIZE+10) + Pin.PIN_SIZE / 2 ), 
								  Pin.PIN_SIZE, Pin.PIN_SIZE);
			Rect oRect = new Rect(headerRect.x + Rect.width - Pin.PIN_SIZE - 5, 
				centerY - ((oCount % 2 == 0) ? oCount / 2 * (Pin.PIN_SIZE + 10) - 5 : (oCount - 1)/2 * (Pin.PIN_SIZE+10) + Pin.PIN_SIZE / 2  ), 
								  Pin.PIN_SIZE, Pin.PIN_SIZE);

			// Draw Transitions
			foreach (var t in transitions)
			{
				bool isInput = t.PinType == PinType.TransIn;

				var r = (isInput) ? iRect : oRect;

				DrawPin(r, t, (isInput) ? Vector2.left : Vector2.right, (isInput) ? !showTransitionInputLabel : !showTransitionOutputLabel);

				if(isInput) iRect.y += iRect.height + 10;
				else oRect.y += oRect.height + 10;
			}



            // Draw inputs
            List<string> pins = new List<string>();
            for (int iCnt = 0; iCnt < inputs.Count; iCnt++)
            {
                DrawPin(iRect, inputs[iCnt], Vector2.left, !showInputLabel);
                iRect.y += iRect.height + 10;
                pins.Add(inputs[iCnt].Name);
            }

            // Draw input variables
            iRect.width = width - 5 - (Pin.PIN_SIZE + 10) * 2;
            iRect.height = 25;
            var variables = Node.Variables.GetByGroup("Input").FindAll(x => !pins.Contains(x.Name));
            if (inputs.Count == 0)
                iRect.y = centerY - ((variables.Count % 2 == 0) ? variables.Count / 2 * (iRect.height + 10) - 5 : (variables.Count - 1) / 2 * (iRect.height + 10) + iRect.height / 2);   

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

		protected override void onGUIBody(){}

		protected override void InitStyle()
		{
			base.InitStyle();
			titleStyle.fontSize = 18;
		}
	}
}