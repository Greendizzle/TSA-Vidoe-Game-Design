using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runemark.Common;
using Runemark.VisualEditor;
using UnityEditor;
using UnityEditorInternal;


namespace Runemark.DialogueSystem
{ 
	public class AnswerReorderableList : ReorderableListGUI 
	{
		public override bool UseAddDropdown { get { return true; } }
		public override string Title { get { return "Answers"; } }

		TextNode _node;

		public AnswerReorderableList(TextNode node) : base (node.Answers)
		{
			_node = node;
		}			
		protected override void drawHeaderCallback(Rect rect)
		{
			GUI.Label(rect, Title);
		}
		protected override void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
		{		
			var element = _node.Answers[index];

            Pin input = _node.PinCollection.Get(element.InputName);
            Variable variable = _node.Variables.GetByName(element.VariableName);

            // TIME
            if (element.Type == AnswerType.Time)
            {
                GUI.Label(new Rect(rect.x, rect.y, 50, 20), "Time");
                Rect fieldRect = new Rect(rect.x + 55, rect.y, rect.width - 55, 20);

                if (input.HasConnection)
                    GUI.Label(fieldRect, "Calculated");
                else
                    variable.Value = EditorGUI.FloatField(fieldRect, variable.ConvertedValue<float>());
			}
			else if (element.Type == AnswerType.Answer)
                variable.Value = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, 20), variable.ConvertedValue<string>());
		}
		protected override void onRemoveCallback(UnityEditorInternal.ReorderableList list)
		{
			if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete this answer?", "Yes", "No"))
			{
                var element = _node.Answers[list.index];
                var input = _node.PinCollection.Get(element.InputName);               
                var output = _node.PinCollection.Get(element.OutputName);

                if(input != null) WireController.Disconnect(input);
                if (output != null) WireController.Disconnect(output);

                _node.PinCollection.Remove(element.InputName);
                _node.PinCollection.Remove(element.OutputName);
                _node.Variables.RemoveByName(element.VariableName);
                ReorderableList.defaultBehaviours.DoRemoveButton(list);


			}
		}
		protected override void onSelectCallback(UnityEditorInternal.ReorderableList list)
		{
			
		}
		protected override void onAddDropdownCallback(Rect buttonRect, UnityEditorInternal.ReorderableList list)
		{
			var menu = new GenericMenu();         
			menu.AddItem(new GUIContent("Answer"), false, AddAnswer, AnswerType.Answer);
			if(_node.Answers.Find(x => x.Type == AnswerType.Time) == null)
                menu.AddItem(new GUIContent("Time"), false, AddAnswer, AnswerType.Time);		

			menu.ShowAsContext();
		}   
		protected override void onReorderCallback(ReorderableList list)
		{
			_node.HasChanges = true;
		}

		void AddAnswer(object type)
		{
            int index = _node.Answers.Count;
            AnswerData data = new AnswerData();
            data.InputName = "AnswerInput_" + index;
            data.OutputName = "AnswerOutput_" + index;
            data.VariableName = "AnswerVariableInput_" + index;
            data.Type = (AnswerType)type;

            _node.PinCollection.AddOutTransition(data.OutputName);

            switch (data.Type)
			{
                case AnswerType.Time:
                    _node.PinCollection.AddInput(data.InputName, typeof(float));
                    _node.Variables.Add(data.VariableName, 0f, "Input");
					break;

				case AnswerType.Answer:
                    _node.PinCollection.AddInput(data.InputName, typeof(bool));
                    _node.Variables.Add(data.VariableName, "", "Input");
					break;
			}

            _node.Answers.Add(data);
		}

        //unused
        protected override void onAddCallback(UnityEditorInternal.ReorderableList list) { }
        protected override bool onCanRemoveCallback(UnityEditorInternal.ReorderableList list)
        {
            return true;
        }
    }
}
