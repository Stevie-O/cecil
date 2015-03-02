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
using System.Text;
using Mono.Collections.Generic;
using MD = Mono.Cecil.Metadata;

namespace Mono.Cecil {

	public sealed class GenericInstanceType : TypeSpecification, IGenericInstance, IGenericContext {

		Collection<TypeReference> arguments;

		public bool HasGenericArguments {
			get { return !arguments.IsNullOrEmpty (); }
		}

		public Collection<TypeReference> GenericArguments {
			get { return arguments ?? (arguments = new Collection<TypeReference> ()); }
		}

		public override TypeReference DeclaringType {
			get { return ElementType.DeclaringType; }
			set { throw new NotSupportedException (); }
		}

		public override string FullName {
			get {
				return MemberFullName();
			}
		}

		protected override string MemberFullName()
		{
			var name = new StringBuilder(base.MemberFullName());
			this.GenericInstanceFullName(name);
			return name.ToString();
		}

		public override bool IsGenericInstance {
			get { return true; }
		}

		public override bool ContainsGenericParameter {
			get { return this.ContainsGenericParameter () || base.ContainsGenericParameter; }
		}

		IGenericParameterProvider IGenericContext.Type {
			get { return ElementType; }
		}

		IGenericInstance IGenericContext.InstanceType {
			get { return this; }
		}

		public GenericInstanceType (TypeReference type)
			: this(type, null)
		{
		}

		public GenericInstanceType (TypeReference type, ICollection<TypeReference> typeArguments)
			: base(type)
		{
			base.IsValueType = type.IsValueType;
			this.etype = MD.ElementType.GenericInst;
			if (typeArguments != null)
				arguments = new Collection<TypeReference>(typeArguments);
		}

		public override TypeReference ApplyTypeArguments(IGenericContext ctx)
		{
			// TODO: verify this is totally appropriate
			// It *might* be, if you have a situation like this:
			// class Foo<T>() {
			//      List<T> x;
			// }
			// where the type of 'x' is actually a GenericTypeInstance where one of the parameters is a generic type parameter
			if (!ContainsGenericParameter) return this;

			bool any_different = false;
			Collection<TypeReference> new_arguments = new Collection<TypeReference>();
			foreach (TypeReference tr in GenericArguments)
			{
				TypeReference tr_mapped = tr.ApplyTypeArguments(ctx);
				new_arguments.Add(tr_mapped);
				if (tr_mapped != tr) any_different = true;
			}
			// if we didn't replace any of the type arguments, then return ourselves unchanged
			// This lets other code detect that nothing is different and optimize accordingly
			if (!any_different) return this;
			GenericInstanceType mapped_git = new GenericInstanceType(ElementType);
			mapped_git.arguments = new_arguments;
			return mapped_git;
		}

		public override FieldReference[] GetFields()
		{
			return Array.ConvertAll(ElementType.GetFields(), (r) => Mixin.GetRuntimeReference(r, this));
		}

		public override MethodReference[] GetMethods()
		{
			return Array.ConvertAll(ElementType.GetMethods(), (r) => Mixin.GetRuntimeReference(r, this));
		}

		public override PropertyReference[] GetProperties()
		{
			return Array.ConvertAll(ElementType.GetProperties(), (r) => Mixin.GetRuntimeReference(r, this));
		}

		public override EventReference[] GetEvents()
		{
			return Array.ConvertAll(ElementType.GetEvents(), (r) => Mixin.GetRuntimeReference(r, this));
		}

		public override TypeReference[] GetInterfaces()
		{
			TypeReference[] ifaces = base.GetInterfaces();
			bool any_generic = false;
			foreach (TypeReference iface in ifaces)
			{
				if (iface.HasGenericParameters)
				{
					any_generic = true;
					break;
				}
			}
			// If none of the interfaces have generic type arguments, we don't need to do anything fancy
			if (!any_generic) return ifaces;
			TypeReference[] ifaces_concrete = new TypeReference[ifaces.Length];
			for (int i = 0; i < ifaces.Length; i++)
			{
				ifaces_concrete[i] = ifaces[i].ApplyTypeArguments(this);
			}
			return ifaces_concrete;
		}

		public override TypeReference GetBaseType()
		{
			TypeReference genericBaseType = base.GetBaseType();
			// e.g.: class Foo<T> : Bar<T>
			// this will make Foo<int> return Bar<int>
			return genericBaseType.ApplyTypeArguments(this);
		}
	}
}
