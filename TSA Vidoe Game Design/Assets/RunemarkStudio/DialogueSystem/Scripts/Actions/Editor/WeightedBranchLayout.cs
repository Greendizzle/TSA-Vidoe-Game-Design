using UnityEngine;
using UnityEditor;
using Runemark.VisualEditor;

namespace Runemark.DialogueSystem
{ 
	[CustomNodeLayout(typeof(WeightedBranch), true)]
	public class WeightedBranchLayout : NodeLayout
	{ 
		protected sealed override float headerHeight { get { return 30; } }
		protected sealed override float bodyHeight 
		{
			get
			{ 
				return Mathf.Max(minBodyHeight, (_node.Weights.Count + 2) * (Pin.PIN_SIZE+10) + 10 + 30); 
			}
		}

		protected float minBodyHeight;

		Pin _iTrans;
		int _lastPinCnt = 0;

        WeightedBranch _node;

		public WeightedBranchLayout(Node node) : base(node)
		{
            _node = (WeightedBranch)node;

            headerColor = new Color(0.27f, 0.43f, 0.53f, 1f);
			bodyColor = new Color(0, 0, 0, 1f);

			width = 175;
			_iTrans = transitions.Find(x => x.PinType == PinType.TransIn);
		}

		protected override void onGUIHeader()
		{		
			GUI.Label(headerRect, Title, titleStyle);
		}

		protected override void onGUIBody()
		{	
			if (_lastPinCnt != transitions.Count)
			{
				_editor.Repaint();
				_lastPinCnt = transitions.Count;
				return;
			}

			Rect r = new Rect(bodyRect.x + 10, bodyRect.y + 10, Pin.PIN_SIZE, Pin.PIN_SIZE);
		
			DrawPin(r, _iTrans, Vector2.left);
			r.y += Pin.PIN_SIZE + 10;

			foreach (var i in inputs)
			{
				DrawPin(r, i, Vector2.left);
				r.y += Pin.PIN_SIZE + 10;
			}

            var weightList = _node.Weights;
			if (weightList != null && weightList.Count > 0)
			{
				Color c = headerColor;
				c.a = c.a / 2;
				EditorGUI.DrawRect(new Rect(bodyRect.x, r.y, width, 25), c);
				GUI.Label(new Rect(bodyRect.x, r.y, width, 25), "Weights:", _answersHeader);
				r.y += 30;

                int cnt = 0;
				foreach (var weight in weightList)
				{
                    Variable variable = _node.Variables.GetByName(weight.VariableName);
                    Pin pin = _node.PinCollection.Get(weight.OutputName);
                                      
					float x = r.x + r.width + 5;
					float w = width - (2*r.width + 30);

					int min = 0;
					int max = variable.ConvertedValue<int>();

					if (cnt > 0)
					{
                        var wd = _node.Weights[cnt - 1];
                        Variable v = _node.Variables.GetByName(wd.VariableName);                     
						min = v.ConvertedValue<int>() + 1;
					}

					GUI.Label(new Rect(x, r.y - 2, w, 20), min + " - " + max, _textStyle);

					x += w+5;
					DrawPin(new Rect(x, r.y, r.width, r.height), pin, Vector2.right, true);
					r.y += r.height + 10;

                    cnt++;
				}
			}



		}



		GUIStyle _textStyle;
		GUIStyle _answersHeader;
		protected override void InitStyle()
		{
			base.InitStyle();
			titleStyle.fontSize = 16;

			Color textColor = new Color(.8f, .8f, .8f, 1f);

			_textStyle = new GUIStyle(GUI.skin.label);
			_textStyle.normal.textColor = textColor;
			_textStyle.wordWrap = true;

			_answersHeader = new GUIStyle(_textStyle);
			_answersHeader.alignment = TextAnchor.MiddleLeft;
			_answersHeader.padding = new RectOffset(10, 1, 1, 1);
		}	
	} 




	[CustomEditor(typeof(WeightedBranch))]
	public class WeightedBranchInspector : NodeInspector
	{
		WeigthReorderableList _list;

		void OnEnable()
		{			
			WeightedBranch myTarget = (WeightedBranch)target;
			_list = new WeigthReorderableList(myTarget);
		}        

        public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			WeightedBranch myTarget = (WeightedBranch)target;
			_list.Draw();


			if (EditorGUI.EndChangeCheck())
				myTarget.HasChanges = true;
		}
	}
} 
