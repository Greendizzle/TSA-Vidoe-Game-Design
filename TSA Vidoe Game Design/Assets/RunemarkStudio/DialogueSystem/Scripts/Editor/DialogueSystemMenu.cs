using UnityEngine;
using UnityEditor;
using Runemark.Common;
using Runemark.VisualEditor.Actions;

namespace Runemark.DialogueSystem
{
	public static class DialogueSystemMenu 
	{
		// Create new graph
		[MenuItem("Assets/Create/Runemark/Dialogue System/New Dialogue")]
		static void CreateNewGraph() 
		{	
			var root = AssetCreator.CreateAsset<DialogueGraph>();
            string name = root.Name;
            root.Name = "DialogueRoot";
            root.EditorInit(null, Vector2.zero);
            root.Name = name;

            ConversationFlow c = new ConversationFlow();        
            SimpleEvent onOpen = AssetCreator.CreateAsset<SimpleEvent>(root);
			onOpen.Name = c.EVENT_NAME;
			onOpen.EditorInit(root, new Vector2(0, -50));
			onOpen.CanCopy = false;
			onOpen.CanDelete = false;
			root.Nodes.Add(onOpen);

            AmbientDialogue a = new AmbientDialogue();
            SimpleEvent onBark = AssetCreator.CreateAsset<SimpleEvent>(root);
			onBark.Name = a.EVENT_NAME;
			onBark.EditorInit(root, new Vector2(0, 50));
			onBark.CanCopy = false;
			onBark.CanDelete = false;
			root.Nodes.Add(onBark);
		}
	}
}