//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace Mono.Cecil.Rocks {

#if INSIDE_ROCKS
	public
#endif
	static class TypeDefinitionRocks {

		public static TypeInterfaceMapping GetInterfaceMapping(this TypeReference self, TypeReference interfaceType)
		{
			if (self == null)
				throw new ArgumentNullException("self");

			TypeDefinition interfaceDef = interfaceType.Resolve();

			if (interfaceDef == null) throw new InvalidOperationException("Cannot resolve interfaceType to a TypeDefinition!");

			// Okay, pre-flight check.  These tests are based on the MSDN documentation for Type.GetInterfaceMap()

			if (interfaceType == null)
				throw new ArgumentNullException("interfaceType");

			if (self.IsGenericParameter)
				throw new InvalidOperationException("Cannot get interface mappings for generic type parameters");

			if (!interfaceDef.IsInterface)
				throw new ArgumentException("interfaceType argument does not refer to an interface");

			// not really sure how this works out
			if (interfaceDef.HasGenericParameters && self.IsArray)
				throw new ArgumentException("operation not valid for generic interfaces and array types");

			bool interface_valid = false;
			foreach (TypeReference intf in self.GetInterfaces())
			{
				if (intf.Module == null) continue;
				if (intf.Resolve() == interfaceDef) 
				{
					interface_valid = true;
					break;
				}
			}
			if (!interface_valid)
				throw new ArgumentException("This type does not implement the specified interface");

			TypeInterfaceMapping tim = new TypeInterfaceMapping();
			tim.InterfaceType = interfaceType;
			tim.TargetType = self;
			InterfaceMethodMapper imf = new InterfaceMethodMapper();
			imf.FindMappings(self, interfaceType);
			tim.InterfaceMethods =  imf.imethodlist.ToArray();
			tim.TargetMethods = imf.tmethodlist.ToArray();
			return tim;
		}

		public static IEnumerable<MethodDefinition> GetConstructors (this TypeDefinition self)
		{
			if (self == null)
				throw new ArgumentNullException ("self");

			if (!self.HasMethods)
				return Empty<MethodDefinition>.Array;

			return self.Methods.Where (method => method.IsConstructor);
		}

		public static MethodDefinition GetStaticConstructor (this TypeDefinition self)
		{
			if (self == null)
				throw new ArgumentNullException ("self");

			if (!self.HasMethods)
				return null;

			return self.GetConstructors ().FirstOrDefault (ctor => ctor.IsStatic);
		}

		public static FieldDefinition GetField(this TypeDefinition self, string name)
		{
			if (self == null)
				throw new ArgumentNullException("self");

			if (!self.HasFields)
				return null;

			return self.Fields.FirstOrDefault(delegate(FieldDefinition d) { return d.Name == name; });
		}

		public static PropertyDefinition GetProperty(this TypeDefinition self, string name)
		{
			if (self == null)
				throw new ArgumentNullException("self");

			if (!self.HasProperties)
				return null;

			return self.Properties.FirstOrDefault(delegate(PropertyDefinition d) { return d.Name == name; });
		}

		public static IEnumerable<MethodDefinition> GetMethods(this TypeDefinition self)
		{
			if (self == null)
				throw new ArgumentNullException ("self");

			if (!self.HasMethods)
				return Empty<MethodDefinition>.Array;

			return self.Methods.Where (method => !method.IsConstructor);
		}

		public static TypeReference GetEnumUnderlyingType (this TypeDefinition self)
		{
			if (self == null)
				throw new ArgumentNullException ("self");
			if (!self.IsEnum)
				throw new ArgumentException ();

			return Mixin.GetEnumUnderlyingType (self);
		}
	}
}
