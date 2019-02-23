using System;
using System.Collections.Generic;

namespace Runemark.VisualEditor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class GenericArguments : Attribute
	{
		public readonly Type[] Types;
		public GenericArguments(params Type[] args)
		{
			Types = args;
		}
	}
}