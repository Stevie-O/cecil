using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mono.Cecil.Rocks
{
	public struct TypeInterfaceMapping
	{
		public TypeReference InterfaceType;
		public MethodReference[] InterfaceMethods;
		public TypeReference TargetType;
		public MethodReference[] TargetMethods;
	}
}
