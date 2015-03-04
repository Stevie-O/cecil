//
// FieldReference.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2011 Jb Evain
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
			if (fieldType == null)
				throw new ArgumentNullException ("fieldType");

			this.field_type = fieldType;
			this.token = new MetadataToken (TokenType.MemberRef);
		}

		public FieldReference (string name, TypeReference fieldType, TypeReference declaringType)
			: this (name, fieldType)
		{
			if (declaringType == null)
				throw new ArgumentNullException("declaringType");

			this.DeclaringType = declaringType;
		}

        public override MemberDefinitionType MemberType
        {
            get { return MemberDefinitionType.Field; }
        }

        public virtual FieldDefinition Resolve()
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
