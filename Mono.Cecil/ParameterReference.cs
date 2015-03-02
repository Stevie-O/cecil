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

namespace Mono.Cecil {

	public abstract class ParameterReference : IMetadataTokenProvider {

		string name;
		internal int index = -1;
		protected TypeReference parameter_type;
		internal MetadataToken token;
		internal IMethodSignature method;

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public int Index {
			get { return index; }
		}

		public int Sequence {
			get {
				if (method == null)
					return -1;

				return method.HasImplicitThis () ? index + 1 : index;
			}
		}

		public TypeReference ParameterType {
			get { return parameter_type; }
			set { parameter_type = value; }
		}

		public MetadataToken MetadataToken {
			get { return token; }
			set { token = value; }
		}

		public IMethodSignature Method {
			get { return method; }
		}

		internal ParameterReference (string name, TypeReference parameterType)
		{
			if (parameterType == null)
				throw new ArgumentNullException ("parameterType");

			this.name = name ?? string.Empty;
			this.parameter_type = parameterType;
		}

		public override string ToString ()
		{
			if (!string.IsNullOrEmpty(Name))
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(Name).Append(' ');
				sb.Append(ParameterType.ToString());
				return sb.ToString();
			}
			return ParameterType.ToString();
		}

		public abstract ParameterDefinition Resolve ();
	}
}
