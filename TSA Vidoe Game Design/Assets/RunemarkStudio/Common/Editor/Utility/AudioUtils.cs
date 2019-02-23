using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;


namespace Runemark.Common.Utility
{
    /// <summary>
    /// This class is used to play audio in the editor.
    /// </summary>
	public class AudioUtils
	{
		public static void Play(AudioClip clip, int start)
		{
            Stop();
			Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
			Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
			MethodInfo method = audioUtilClass.GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[]{ typeof(AudioClip), typeof(int) }, null);
			method.Invoke(null, new object[]{ clip, start });

			if (start > 0)
			{
				method = audioUtilClass.GetMethod("SetClipSamplePosition", BindingFlags.Static | BindingFlags.Public);
				method.Invoke(null, new object[]{ clip, start });
			}

		}

		public static void Stop()
		{
			Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
			Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
			MethodInfo method = audioUtilClass.GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public, null, new System.Type[]{ }, null);
			method.Invoke(null, new object[]{});
		}



	}
}