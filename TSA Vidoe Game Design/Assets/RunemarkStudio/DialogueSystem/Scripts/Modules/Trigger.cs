using UnityEngine;

namespace Runemark.DialogueSystem
{	
	[System.Serializable]
	public class Trigger
	{
		public enum Modes { Use, TriggerEnter, OnStart, Custom }
		public Modes Mode = Modes.OnStart;

		public float Distance;
		public string PlayerTag;

		public bool OnTriggerEnter(Collider other)
		{
			return Mode == Modes.TriggerEnter && other.tag == PlayerTag;
		}

        /// <summary>
        /// This method is only used for closing the dialogue if the mode is trigger enter
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
		public bool OnTriggerExit(Collider other)
		{
			return Mode == Modes.TriggerEnter && other.tag == PlayerTag;
		}

		public bool Use(Transform other, Transform me)
		{
			return Mode == Modes.Use && other.tag == PlayerTag && Vector3.Distance(other.position, me.position) <= Distance;
		}
	}
}