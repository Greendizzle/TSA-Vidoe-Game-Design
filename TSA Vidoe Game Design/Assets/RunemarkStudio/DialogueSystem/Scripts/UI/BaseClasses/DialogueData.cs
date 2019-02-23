using UnityEngine;

namespace Runemark.DialogueSystem
{
	public class DialogueText
	{
        /// <summary>
        /// The name of the actor.
        /// </summary>
        public readonly string ActorName;

		/// <summary>
		/// Text that the actor says.
		/// </summary>
		public readonly string Text;

		/// <summary>
		/// The npc portrait, that appears with the text.
		/// </summary>
		public readonly Sprite Portrait;

		/// <summary>
		/// Audio clip that is played when the text is shows up.
		/// </summary>
		public readonly AudioClip Audio;

		/// <summary>
		/// The time (in seconds) where the audio playback should start.
		/// </summary>
		public readonly float AudioStartTime;

		/// <summary>
		/// The time (in seconds) how long the audio clip should be played.
		/// </summary>
		public readonly float AudioEndTime;

		/// <summary>
		/// The time (in seconds) when the audio clip should be start to play after the text shows up.
		/// </summary>
		public readonly float AudioDelay;

		/// <summary>
		/// The index of the camera this node should used. -1 means main camera.
		/// </summary>
		public readonly int CameraIndex;

        /// <summary>
        /// The name of the skin the system should use.
        /// </summary>
        public readonly string Skin;


		public DialogueText(TextNode node)
		{
            ActorName = node.ActorName;
			Text = node.Text;
			Portrait = node.Portrait;
			Audio = node.Audio;
			AudioStartTime = node.AudioStartTime;
			AudioEndTime = node.AudioEndTime;
			AudioDelay = node.AudioDelay;
			CameraIndex = node.CameraIndex;
            Skin = (node.CustomSkinEnable) ? node.Skin : "";
		}
	}
}