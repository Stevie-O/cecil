using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mono.Cecil.Rocks
{
	/// <summary>
	/// Used to perform the equivalent of the .NET GetInterfaceMapping() method
	/// </summary>
	public class InterfaceMethodMapper
	{
		// more info here: https://groups.google.com/forum/#!topic/mono-cecil/s8BPJcK1u08

		public List<MethodReference> imethodlist = new List<MethodReference>();
		public List<MethodReference> tmethodlist = new List<MethodReference>();

		public static MethodReference FindInterfaceImplementation(TypeReference type, MethodReference imethod, bool allowExplicit)
		{
			MethodReference implicit_impl = null;

			// TODO: rewrite this so we don't call GetParameters() for every single method in the interface

			MethodReference[] tmethods = type.GetMethods();

			foreach (MethodReference tmethod in tmethods)
			{
				MethodDefinition tmethoddef = tmethod.Resolve();
				// I can't think of any circumstance where we could get here and have tmethod.Resolve() return null.
				if (tmethoddef.IsPublic)
				{
					if (implicit_impl == null && MetadataResolver.MethodsMatch(imethod, tmethod, true))
						implicit_impl = tmethod;
				}
				else if (allowExplicit && tmethoddef.IsPrivate && tmethoddef.HasOverrides)
				{
					MethodReference[] overrides = tmethod.GetOverrides();
					MethodReference overridden_method = overrides[0];
					// Nasty kludge until I can figure out what to do about overrides of methods belonging to generic interfaces
					if (overridden_method.ContainsGenericParameter)
						overridden_method = overridden_method.GetRuntimeReference(overridden_method.DeclaringType);
					if (MetadataResolver.AreSame(overridden_method.DeclaringType, imethod.DeclaringType)
						&& MetadataResolver.MethodsMatch(overridden_method, imethod, true)
						)
					{
						return tmethod;
					}
				}
			}

			if (implicit_impl != null) return implicit_impl;

			TypeReference baseType = type.GetBaseType();
			if (baseType != null)
				return FindInterfaceImplementation(baseType, imethod, allowExplicit);

			return null;
		}

		void GetMethodMappings(TypeReference type, TypeReference iface)
		{
			// if (!iface.HasMethods) return; // Hey, it can happen! Look at System.Web.SessionState.IRequiresSessionState
			MethodReference[] imethods = iface.GetMethods();
			MethodReference[] tmethods = new MethodReference[imethods.Length];
			for (int i = 0; i < imethods.Length; i++)
			{
				MethodReference imethod = imethods[i];
				MethodReference tmethod = FindInterfaceImplementation(type, imethod, true);
				tmethods[i] = tmethod;
			}
			imethodlist.AddRange(imethods);
			tmethodlist.AddRange(tmethods);
		}

		public void FindMappings(TypeReference type, TypeReference iface)
		{
			GetMethodMappings(type, iface);

			// Testing with .NET 2.0 reveals that Type.GetInterfaceMap() does not map base interface methods
			// if the specified interface inherits from any other interfaces
			// Tested with Dictionary<,> and IDictionary<,>
			/*
			if (iface.HasInterfaces)
			{
				foreach (TypeReference base_if in iface.Interfaces)
				{
					FindMappings(type, base_if.CheckedResolve());
				}
			}*/
		}
	}

}
