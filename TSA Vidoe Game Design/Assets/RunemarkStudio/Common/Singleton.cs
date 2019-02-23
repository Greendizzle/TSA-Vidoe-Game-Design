using System;

namespace Runemark.Common
{
	public class Singleton<T> where T : class
	{
		static T _instance;
		public static T Instance
		{
			get
			{
				if (_instance == null)
					_instance = (T)Activator.CreateInstance(typeof(T));
				return _instance;
			}
		}
	}
}