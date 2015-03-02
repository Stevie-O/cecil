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

	public abstract class PropertyReference : MemberReference {

		TypeReference property_type;

		public TypeReference PropertyType {
			get { return property_type; }
			set { property_type = value; }
		}

		/// <summary>
		/// Gets the set of parameters for this property.
		/// </summary>
		/// <returns>An array of objects derived from ParameterReference (never null).</returns>
		public abstract ParameterReference[] GetParameters();

		public override bool ContainsGenericParameter
		{
			get
			{
				if (PropertyType.ContainsGenericParameter || base.ContainsGenericParameter) return true;
				if (HasParameters)
				{
					foreach (ParameterReference parm in GetParameters())
					{
						if (parm.ParameterType.ContainsGenericParameter) return true;
					}
				}
				return false;
			}
		}

		internal PropertyReference(string name, TypeReference propertyType)
			: base(name)
		{
			if (propertyType == null)
				throw new ArgumentNullException("propertyType");

			property_type = propertyType;
		}

		protected override IMemberDefinition ResolveDefinition ()
		{
			return this.Resolve ();
		}

		public new abstract PropertyDefinition Resolve ();

		public abstract bool HasParameters { get; }

		public override string FullName
		{
			get
			{
				var builder = new StringBuilder();
				builder.Append(PropertyType.ToString());
				builder.Append(' ');
				builder.Append(MemberFullName());
				builder.Append('(');
				if (HasParameters)
				{
					var parameters = GetParameters();
					for (int i = 0; i < parameters.Length; i++)
					{
						if (i > 0)
							builder.Append(',');
						builder.Append(parameters[i].ParameterType.FullName);
					}
				}
				builder.Append(')');
				return builder.ToString();
			}
		}

		/// <summary>
		/// Gets a PropertyReference as it will be seen by the .NET Runtime, with generic type parameters
		/// replaced by their arguments.
		/// </summary>
		/// <returns>(See example)</returns>
		/// <remarks>
		/// For example, given this class:<code>
		/// <![CDATA[
		/// public class Foo<T>
		/// {
		///     public T FooProperty { get; set; }
		///     public T[] FooProperty2 { get; set; }
		/// }
		/// ]]></code>
		/// For PropertyReferences to Foo&lt;int&gt;.FooProperty
		/// * The PropertyType property will be T
		/// * GetRuntimeReference() will return a PropertyReference where PropertyType is int
		/// 
		/// For PropertyReferences to Foo&lt;int&gt;.FooProperty2
		/// * The PropertyType property will be T[]
		/// * GetRuntimeReference() will return a PropertyReference where PropertyType is int[]
		/// 
		/// This method has no effect if the PropertyType does not depend upon a generic parameter.
		/// It also has no effect for a PropertyReference that directly refers to the generic Foo&lt;T&gt;.FooProperty.
		/// </remarks>
		public PropertyReference GetRuntimeReference()
		{
			if (!this.DeclaringType.IsGenericInstance) return this;
			return GetRuntimeReference(this.DeclaringType);
		}

		/// <summary>
		/// Gets a PropertyReference as it will be seen by the .NET Runtime, with generic type parameters
		/// replaced by the arguments in <paramref name="ctx"/>
		/// </summary>
		public virtual PropertyReference GetRuntimeReference(IGenericContext ctx)
		{
			// must only take this shortcut if ConstructedPropertyReference won't be fudging the DeclaringType
			if (ctx == this.DeclaringType && !ContainsGenericParameter) return this;
			return new ConstructedPropertyReference(this, ctx);
		}
	}

	static partial class Mixin
	{
		/// <summary>
		/// Compatible with Converter&lt;PropertyReference, PropertyReference&gt; and thus Array.ConvertAll
		/// </summary>
		/// <param name="r">Property reference</param>
		/// <returns>Result of calling r.GetRuntimeReference()</returns>
		public static PropertyReference GetRuntimeReference(PropertyReference r, IGenericContext ctx)
		{
			return r.GetRuntimeReference(ctx);
		}
	}
}
