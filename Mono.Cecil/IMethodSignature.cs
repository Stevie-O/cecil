//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

using System.Text;

using Mono.Collections.Generic;

namespace Mono.Cecil {

	public interface IMethodSignature : IMetadataTokenProvider {

		bool HasThis { get; set; }
		bool ExplicitThis { get; set; }
		MethodCallingConvention CallingConvention { get; set; }

		bool HasParameters { get; }
		int ParameterCount { get; }
		ParameterReference[] GetParameters();
		TypeReference ReturnType { get; set; }
	}

	/// <summary>
	/// Combines IMethodSignature with some internal-only methods
	/// </summary>
	internal interface IMethodSignatureInternal : IMethodSignature
	{
		/// <summary>
		/// Prepares to receive parameters from AssemblyReader
		/// </summary>
		/// <param name="numParameters">Hints at the number of parameters that are going to be added.</param>
		/// <returns>An object that can be used to add parameter data to this object.</returns>
		IParameterReferenceReceiver ReceiveParameters(int numParameters);
	}

	static partial class Mixin {

		public static bool HasImplicitThis (this IMethodSignature self)
		{
			return self.HasThis && !self.ExplicitThis;
		}

		public static void MethodSignatureFullName (this IMethodSignature self, StringBuilder builder)
		{
			builder.Append ("(");

			if (self.HasParameters) {
				var parameters = self.GetParameters();
				for (int i = 0; i < parameters.Length; i++) {
					var parameter = parameters [i];
					if (i > 0)
						builder.Append (",");

					if (parameter.ParameterType.IsSentinel)
						builder.Append ("...,");

					builder.Append (parameter.ParameterType.FullName);
				}
			}

			builder.Append (")");
		}
	}
}
