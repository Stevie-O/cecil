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
using System.Text;

using Mono.Collections.Generic;

namespace Mono.Cecil {

	public sealed class GenericInstanceMethod : MethodSpecification, IGenericInstance, IGenericContext {

		Collection<TypeReference> arguments;

		public bool HasGenericArguments {
			get { return !arguments.IsNullOrEmpty (); }
		}

		public Collection<TypeReference> GenericArguments {
			get { return arguments ?? (arguments = new Collection<TypeReference> ()); }
		}

		public override bool IsGenericInstance {
			get { return true; }
		}

		IGenericParameterProvider IGenericContext.Method {
			get { return ElementMethod; }
		}

		IGenericParameterProvider IGenericContext.Type {
			get { return ((IGenericContext)ElementMethod).Type; }
		}

		public override bool ContainsGenericParameter {
			get { return this.ContainsGenericParameter () || base.ContainsGenericParameter; }
		}

		protected override string MemberFullName()
		{
			StringBuilder sb = new StringBuilder(base.MemberFullName());
			this.GenericInstanceFullName(sb);
			return sb.ToString();
		}

		public GenericInstanceMethod (MethodReference method)
			: base (method)
		{
		}
	}
}
