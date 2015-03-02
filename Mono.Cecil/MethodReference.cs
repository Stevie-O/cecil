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

	public abstract class MethodReference : MemberReference, IMethodSignature, IMethodSignatureInternal, IGenericParameterProvider, IGenericContext {

		bool has_this;
		bool explicit_this;
		MethodCallingConvention calling_convention;
		internal Collection<GenericParameter> generic_parameters;

		public virtual bool HasThis {
			get { return has_this; }
			set { has_this = value; }
		}

		public virtual bool ExplicitThis {
			get { return explicit_this; }
			set { explicit_this = value; }
		}

		public virtual MethodCallingConvention CallingConvention {
			get { return calling_convention; }
			set { calling_convention = value; }
		}

		public abstract bool HasParameters { get; }

		/// <summary>
		/// Provides a fast way to get the number of parameters in the method.
		/// </summary>
		public abstract int ParameterCount { get; }

		public abstract ParameterReference[] GetParameters();

		IGenericParameterProvider IGenericContext.Type {
			get {
				var declaring_type = this.DeclaringType;
				var instance = declaring_type as GenericInstanceType;
				if (instance != null)
					return instance.ElementType;

				return declaring_type;
			}
		}

		IGenericParameterProvider IGenericContext.Method {
			get { return this; }
		}

		IGenericInstance IGenericContext.InstanceType {
			get { return DeclaringType as IGenericInstance; }
		}

		IGenericInstance IGenericContext.InstanceMethod {
			get { return this as IGenericInstance; }
		}

		GenericParameterType IGenericParameterProvider.GenericParameterType {
			get { return GenericParameterType.Method; }
		}

		public virtual bool HasGenericParameters {
			get { return !generic_parameters.IsNullOrEmpty (); }
		}

		public virtual Collection<GenericParameter> GenericParameters {
			get {
				if (generic_parameters != null)
					return generic_parameters;

				return generic_parameters = new GenericParameterCollection (this);
			}
		}

		public abstract TypeReference ReturnType { get; set; }

		public override string FullName {
			get {
				var builder = new StringBuilder ();
				if (ReturnType != null) builder.Append (ReturnType.FullName).Append(" ");
				builder.Append (MemberFullName ());
				this.MethodSignatureFullName (builder);
				return builder.ToString ();
			}
		}

		public virtual bool IsGenericInstance {
			get { return false; }
		}

		public override bool ContainsGenericParameter {
			get {
				if (this.ReturnType.ContainsGenericParameter || base.ContainsGenericParameter)
					return true;

				if (!HasParameters)
					return false;

				var parameters = this.GetParameters();

				for (int i = 0; i < parameters.Length; i++)
					if (parameters[i].ParameterType.ContainsGenericParameter)
						return true;

				return false;
			}
		}

		internal MethodReference ()
		{
			this.token = new MetadataToken (TokenType.MemberRef);
		}

		public MethodReference (string name)
			: base (name)
		{
			this.token = new MetadataToken (TokenType.MemberRef);
		}

		public MethodReference (string name, TypeReference declaringType)
			: this (name)
		{
			Mixin.CheckType (declaringType, Mixin.Argument.declaringType);

			this.DeclaringType = declaringType;
		}

		public virtual MethodReference GetElementMethod ()
		{
			return this;
		}

		protected override IMemberDefinition ResolveDefinition ()
		{
			return this.Resolve ();
		}

		public new virtual MethodDefinition Resolve ()
		{
			var module = this.Module;
			if (module == null)
				throw new NotSupportedException ();

			return module.Resolve (this);
		}

		/// <summary>
		/// Prepares to receive parameters from AssemblyReader
		/// </summary>
		/// <param name="numParameters">Hints at the number of parameters that are going to be added.</param>
		/// <returns>An object that can be used to add parameter data to this object.</returns>
		IParameterReferenceReceiver IMethodSignatureInternal.ReceiveParameters(int numParameters)
		{
			return ReceiveParameters(numParameters);
		}

		protected internal abstract IParameterReferenceReceiver ReceiveParameters(int numParameters);

		/// <summary>
		/// Gets a MethodReference as it will be seen by the .NET Runtime, with generic type parameters
		/// replaced by their arguments.
		/// </summary>
		/// <returns>A MethodReference that describes a type as viewed by code executing in the .NET runtime.</returns>
		/// <remarks>
		/// </remarks>
		public virtual MethodReference GetRuntimeReference()
		{
			if (!(this.IsGenericInstance || this.DeclaringType.IsGenericInstance)) return this;
			return GetRuntimeReference(this);
		}

		/// <summary>
		/// Gets a MethodReference as it will be seen by the .NET Runtime, with generic type parameters
		/// replaced by the arguments in <param name="ctx" />.
		/// </summary>
		public virtual MethodReference GetRuntimeReference(IGenericContext ctx)
		{
			if (ctx == this && !ContainsGenericParameter) return this;
			return new ConstructedMethodReference(this, ctx);
		}

        /// <summary>
        /// Gets the override list for this method
        /// </summary>
        /// <returns></returns>
        public virtual MethodReference[] GetOverrides()
        {
            return Resolve().Overrides.ToArray();
        }
	}

	static partial class Mixin {

		public static bool IsVarArg (this IMethodSignature self)
		{
			return (self.CallingConvention & MethodCallingConvention.VarArg) != 0;
		}

		public static int GetSentinelPosition (this IMethodSignature self)
		{
			if (!self.HasParameters)
				return -1;

			var parameters = self.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
				if (parameters [i].ParameterType.IsSentinel)
					return i;

			return -1;
		}

		/// <summary>
		/// Compatible with Converter&lt;MethodReference, MethodReference&gt; and thus Array.ConvertAll
		/// </summary>
		/// <param name="r">Method reference</param>
		/// <returns>Result of calling r.GetRuntimeReference()</returns>
		public static MethodReference GetRuntimeReference(MethodReference r, IGenericContext ctx)
		{ 
			return r.GetRuntimeReference(ctx);
		}
	}
}
