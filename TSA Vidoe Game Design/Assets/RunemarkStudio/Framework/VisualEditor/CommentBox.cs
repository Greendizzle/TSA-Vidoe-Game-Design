using UnityEngine;


namespace Runemark.VisualEditor
{
	[System.Serializable]
    [HelpURL("http://runemarkstudio.com/dialogue-system-documentation/#comment-box")]
	public class CommentBox : Node
	{
		#region implemented abstract members of Node
		protected override Variable CalculateOutput(string name) { return null; }
		public override string Tooltip { get { 	return "Comment";} }
		#endregion

		public Vector2 Size { get { return _size; } set { _size = value; EditorParameterChanged = true; }}
		[SerializeField] Vector2 _size = new Vector2(200, 100);

		public Color Color { get { return _color; } set { _color = value; EditorParameterChanged = true; }}
		[SerializeField] Color _color = Color.gray;

		protected override void OnInit()
		{
		}

		public void SetPositionNodesUneffected(Vector2 pos)
		{
			base.Position = pos;
		}
	}
}