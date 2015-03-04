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

namespace Mono.Cecil {

	public class FieldReference : MemberReference {

		TypeReference field_type;

		public TypeReference FieldType {
			get { return field_type; }
			set { field_type = value; }
		}

		public override string FullName {
			get { return field_type.FullName + " " + MemberFullName (); }
		}

		public override bool ContainsGenericParameter {
			get { return field_type.ContainsGenericParameter || base.ContainsGenericParameter; }
		}

		internal FieldReference ()
		{
			this.token = new MetadataToken (TokenType.MemberRef);
		}

		public FieldReference (string name, TypeReference fieldType)
			: base (name)
		{
			Mixin.CheckType (fieldType, Mixin.Argument.fieldType);

			this.field_type = fieldType;
			this.token = new MetadataToken (TokenType.MemberRef);
		}

		public FieldReference (string name, TypeReference fieldType, TypeReference declaringType)
			: this (name, fieldType)
		{
			Mixin.CheckType (declaringType, Mixin.Argument.declaringType);

			this.DeclaringType = declaringType;
		}

        public override MemberDefinitionType MemberType
        {
            get { return MemberDefinitionType.Field; }
        }

		protected override IMemberDefinition ResolveDefinition ()
		{
			return this.Resolve ();
		}

		public new virtual FieldDefinition Resolve ()
		{
			var module = this.Module;
			if (module == null)
				throw new NotSupportedException ();

			return module.Resolve (this);
		}

		/// <summary>
		/// Gets a FieldReference as it will be seen by the .NET Runtime, with generic type parameters
		/// replaced by their arguments.
		/// </summary>
		/// <returns>(See example)</returns>
		/// <remarks>
		/// For example, given this class:<code>
		/// <![CDATA[
		/// public class Foo<T>
		/// {
		///     public T FooField;
		///     public T[] FooField2;
		/// }
		/// ]]></code>
		/// For FieldReferences to Foo&lt;int&gt;.FooField (obtained by parsing IL, or TypeDefinition.Fields):
		/// * The FieldType property will be T
		/// * GetRuntimeReference() will return a FieldReference where FieldType is int
		/// 
		/// For FieldReferences to Foo&lt;int&gt;.FooField2 (obtained by parsing IL, or TypeDefinition.Fields):
		/// * The FieldType property will be T[]
		/// * GetRuntimeReference() will return a FieldReference where FieldType is int[]
		/// 
		/// This method has no effect if the FieldReference that directly refers to the generic Foo&lt;T&gt;.FooField.
		/// </remarks>
		public FieldReference GetRuntimeReference()
		{
			if (!this.DeclaringType.IsGenericInstance) return this;
			return GetRuntimeReference(this.DeclaringType);
		}

		/// <summary>
		/// Gets a FieldReference as it will be seen by the .NET Runtime, with generic type parameters
		/// replaced by their arguments from ctx.
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		public virtual FieldReference GetRuntimeReference(IGenericContext ctx)
		{
			if (ctx == this.DeclaringType && !ContainsGenericParameter) return this;
			return new ConstructedFieldReference(this, ctx);
		}
	}

	static partial class Mixin
	{
		/// <summary>
		/// Compatible with Converter&lt;FieldReference, FieldReference&gt; and thus Array.ConvertAll
		/// </summary>
		/// <param name="r">Field reference</param>
		/// <returns>Result of calling r.GetRuntimeReference()</returns>
		public static FieldReference GetRuntimeReference(FieldReference r, IGenericContext useContext) 
		{ 
			return r.GetRuntimeReference(); 
		}
	}
}
