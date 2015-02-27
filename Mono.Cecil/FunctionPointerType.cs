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
using MD = Mono.Cecil.Metadata;

namespace Mono.Cecil {

	public sealed class FunctionPointerType : TypeSpecification, IMethodSignature, IMethodSignatureInternal {

		readonly MethodReference function;

		public bool HasThis {
			get { return function.HasThis; }
			set { function.HasThis = value; }
		}

		public bool ExplicitThis {
			get { return function.ExplicitThis; }
			set { function.ExplicitThis = value; }
		}

		public MethodCallingConvention CallingConvention {
			get { return function.CallingConvention; }
			set { function.CallingConvention = value; }
		}

		public bool HasParameters {
			get { return function.HasParameters; }
		}

		public int ParameterCount {
			get { return function.ParameterCount; }
		}

		public ParameterReference[] GetParameters() {
			return function.GetParameters();
		}

		public TypeReference ReturnType {
			get { return function.ReturnType; }
			set { function.ReturnType = value; }
		}

		public override string Name {
			get { return function.Name; }
			set { throw new InvalidOperationException (); }
		}

		public override string Namespace {
			get { return string.Empty; }
			set { throw new InvalidOperationException (); }
		}

		public override ModuleDefinition Module {
			get { return ReturnType.Module; }
		}

		public override IMetadataScope Scope {
			get { return function.ReturnType.Scope; }
			set { throw new InvalidOperationException (); }
		}

		public override bool IsFunctionPointer {
			get { return true; }
		}

		public override bool ContainsGenericParameter {
			get { return function.ContainsGenericParameter; }
		}

		public override string FullName {
			get {
				var signature = new StringBuilder ();
				signature.Append (function.Name);
				signature.Append (" ");
				signature.Append (function.ReturnType.FullName);
				signature.Append (" *");
				this.MethodSignatureFullName (signature);
				return signature.ToString ();
			}
		}

		public FunctionPointerType ()
			: base (null)
		{
			this.function = new ModuleMethodReference ();
			this.function.Name = "method";
			this.etype = MD.ElementType.FnPtr;
		}

		public override TypeDefinition Resolve ()
		{
			return null;
		}

		public override TypeReference GetElementType ()
		{
			return this;
		}

		// TODO: how should ApplyTypeArguments behave here?
		
		/// <summary>
		/// Prepares to receive parameters from AssemblyReader/Import
		/// </summary>
		/// <param name="numParameters">Hints at the number of parameters that are going to be added.</param>
		/// <returns>An object that can be used to add parameter data to this object.</returns>
		IParameterReferenceReceiver IMethodSignatureInternal.ReceiveParameters(int numParameters)
		{
			return ReceiveParameters(numParameters);
		}

		internal IParameterReferenceReceiver ReceiveParameters(int numParameters)
		{
			return function.ReceiveParameters(numParameters);
		}

	}
}
