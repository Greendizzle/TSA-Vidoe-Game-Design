using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runemark.Common;
using Runemark.VisualEditor;
using UnityEditor;
using UnityEditorInternal;


namespace Runemark.DialogueSystem
{ 
	public class WeigthReorderableList : ReorderableListGUI 
	{
        protected override bool draggable
        {
            get
            {
                return false;
            }
        }


        public override bool UseAddDropdown { get { return false; } }
		public override string Title { get { return "Pins"; } }

		WeightedBranch _node;

		public WeigthReorderableList(WeightedBranch node) : base (node.Variables.GetAll ())
		{
			_node = node;					
		}
			
		protected override void drawHeaderCallback(Rect rect)
		{
			GUI.Label(rect, Title);
		}

		protected override void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
		{
            int min = 0;
            if (index-1 >= 0)
            {
                var w = _node.Weights[index-1];
                var v = _node.Variables.GetByName(w.VariableName);
                min = v.ConvertedValue<int>() + 1;
            }

			GUI.Label(new Rect(rect.x, rect.y, 100, 20), "Weight: " + min + " - ");

            var weight = _node.Weights[index];
            var variable = _node.Variables.GetByName(weight.VariableName);
            variable.Value = EditorGUI.IntField(new Rect(rect.x+100, rect.y, rect.width-100, 20), variable.ConvertedValue<int>());
		}

		protected override void onAddCallback(ReorderableList list)
		{
            int cnt = _node.Weights.Count;
            int min = 1;
            if (cnt > 0)
            {
                var wd = _node.Weights[cnt-1];
                var v = _node.Variables.GetByName(wd.VariableName);
                min = v.ConvertedValue<int>() + 2;
            }

            var w = new WeightData();
            w.VariableName = "Weight_" + (cnt);
            w.OutputName = w.VariableName;
            
            _node.Variables.Add(w.VariableName, min, "Input");
            _node.PinCollection.AddOutTransition(w.OutputName);
            _node.Weights.Add(w);

			_node.HasChanges = true;
		}

		protected override bool onCanRemoveCallback(UnityEditorInternal.ReorderableList list)
		{
			return true;
		}

		protected override void onRemoveCallback(UnityEditorInternal.ReorderableList list)
		{
			if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete this pin? "+list.index, "Yes", "No"))
			{
                var data = _node.Weights[list.index];
                var output = _node.PinCollection.Get(data.OutputName);
                if (output != null) WireController.Disconnect(output);
                
                _node.Variables.RemoveByName(data.VariableName);
                _node.PinCollection.Remove(data.OutputName);
                _node.Weights.Remove(data);

                _node.HasChanges = true;
			}
		}

		protected override void onSelectCallback(UnityEditorInternal.ReorderableList list)
		{
			
		}
		protected override void onAddDropdownCallback(Rect buttonRect, UnityEditorInternal.ReorderableList list)
		{

		}
        		
	}
}
