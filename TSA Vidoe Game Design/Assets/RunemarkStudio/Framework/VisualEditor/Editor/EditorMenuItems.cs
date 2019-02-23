using UnityEngine;
using UnityEditor;
using Runemark.VisualEditor;
using Runemark.Common;
using Runemark.VisualEditor.Actions;

public class EditorMenuItems 
{
	// Create new graph
	//[MenuItem("Assets/Create/Runemark/Visual Editor/New Graph")]
	static void CreateNewGraph() 
	{	
		var root = AssetCreator.CreateAsset<FunctionGraph>();

		SimpleEvent onEnter = AssetCreator.CreateAsset<SimpleEvent>(root);
		onEnter.Name = "OnEnter";
		onEnter.EditorInit(root, new Vector2(0, -50));
		onEnter.CanCopy = false;
		onEnter.CanDelete = false;
		root.Nodes.Add(onEnter);

		SimpleEvent onExit = AssetCreator.CreateAsset<SimpleEvent>(root);
		onExit.Name = "OnExit";
		onExit.EditorInit(root, new Vector2(0, 50));
		onExit.CanCopy = false;
		onExit.CanDelete = false;
		root.Nodes.Add(onExit);

	}

	// Create custom node window layout
	//[MenuItem("Assets/Create/Runemark/Visual Editor/Node Layout/Default Layout")]
	static void CreateDefaultLayout () { CreateClass.CreateNodeLayout<DefaultLayout>(); }

	//[MenuItem("Assets/Create/Runemark/Visual Editor/Node Layout/Compact Layout")]
	static void CreateCompactLayout () { CreateClass.CreateNodeLayout<CompactLayout>(); }

	//[MenuItem("Assets/Create/Runemark/Visual Editor/Node Layout/Custom Layout")]
	static void CreateCustomLayout () { CreateClass.CreateNodeLayout<NodeLayout>(); }
}
