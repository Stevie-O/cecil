//
// GenericFieldReference.cs
//
// Author:
//   Stephen Oberholtzer (stevie@qrpff.net)
//
// Copyright (c) 2015 Stephen Oberholtzer
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Text;
using Mono.Collections.Generic;

namespace Mono.Cecil
{
	/// <summary>
	/// Represents a reference in one module to a MethodDefinition located in another module
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	public sealed class ModuleMethodReference : MethodReference
	{
		ModuleParameterCollection parameters;
		TypeReference return_type;
		
		public override bool HasParameters
		{
			get { return !parameters.IsNullOrEmpty(); }
		}

		public override int ParameterCount
		{
			get
			{
				if (!HasParameters) return 0;
				return parameters.Count;
			}
		}

		public override ParameterReference[] GetParameters()
		{
			if (parameters == null || parameters.Count == 0) return new ParameterReference[0];
			return parameters.ToArray();
		}

		public ModuleMethodReference()
		{
		}

		public ModuleMethodReference(string name, TypeReference returnType)
			: base(name, returnType)
		{
			return_type = returnType;
		}

		public ModuleMethodReference(string name, TypeReference returnType, TypeReference declaringType)
			: this(name, returnType)
		{
			if (declaringType == null) throw new ArgumentNullException("declaringType");
			DeclaringType = declaringType;
		}

		/// <summary>
		/// Prepares to receive parameters from AssemblyReader
		/// </summary>
		/// <param name="numParameters">Hints at the number of parameters that are going to be added.</param>
		/// <returns>An object that can be used to add parameter data to this object.</returns>
		protected internal override IParameterReferenceReceiver ReceiveParameters(int numParameters)
		{
			if (parameters == null)
				parameters = new ModuleParameterCollection(this, numParameters);
			return parameters;
		}

		public override TypeReference ReturnType
		{
			get { return return_type; }
			set { return_type = value; }
		}
	}
}
