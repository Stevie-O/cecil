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

using Mono.Collections.Generic;

namespace Mono.Cecil {

	sealed class ParameterDefinitionCollection : ParameterReferenceCollection<ParameterDefinition> {

		internal ParameterDefinitionCollection (IMethodSignature method) 
			: base(method) 
		{
		}

		internal ParameterDefinitionCollection (IMethodSignature method, int capacity)
			: base (method, capacity)
		{
		}


		protected override ParameterDefinition CreateParameterReference(TypeReference parameterType)
		{
			return new ParameterDefinition(parameterType);
		}
	}
}
