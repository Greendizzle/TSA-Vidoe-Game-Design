using System.Collections.Generic;
using UnityEngine;

namespace Runemark.VisualEditor
{
	public interface INodeCollection
	{
		#region EditorStuff
		bool HasChanges { get; set; }

		Vector2 ZoomOffset { get; set; }
		string SelectedVariableID { get; set; }
		List<CommentBox> CommentBoxes { get; set; }
		#endregion

		NodeCollection Nodes { get; }
		FunctionGraph Root { get; }

        System.Type GetType();
	}
}