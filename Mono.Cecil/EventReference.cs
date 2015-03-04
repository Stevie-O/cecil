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

	public abstract class EventReference : MemberReference {

		TypeReference event_type;

		public TypeReference EventType {
			get { return event_type; }
			set { event_type = value; }
		}

		public override string FullName {
			get { return event_type.FullName + " " + MemberFullName (); }
		}

		protected EventReference (string name, TypeReference eventType)
			: base (name)
		{
			Mixin.CheckType (eventType, Mixin.Argument.eventType);
			event_type = eventType;
		}

		protected override IMemberDefinition ResolveDefinition ()
		{
			return this.Resolve ();
		}

        public override MemberDefinitionType MemberType
        {
            get { return MemberDefinitionType.Event; }
        }

		public abstract EventDefinition Resolve ();

		/// <summary>
		/// Gets an EventReference as it will be seen by the .NET Runtime, with generic type parameters
		/// replaced by their arguments.
		/// </summary>
		/// <returns>(See example)</returns>
		/// <remarks>
		/// For example, given this class:<code>
		/// <![CDATA[
		/// public class Foo<T>
		/// {
		///     public event EventHandler<T> FooEvent;
		/// }
		/// ]]></code>
		/// For EventReferences to Foo&lt;CancelEventArgs&gt;.FooEvent (obtained by calling TypeDefinition.Events):
		/// * The EventType property will be EventHandler&lt;T&gt;
		/// * GetRuntimeReference() will return an EventReference where EventType is EventHandler&lt;CancelEventArgs&gt;
		/// 
		/// This method has no effect if the EventType does not depend upon a generic parameter.
		/// It also has no effect for a FieldReference that directly refers to the generic Foo&lt;T&gt;.FooField.
		/// </remarks>
		public EventReference GetRuntimeReference()
		{
			if (!this.DeclaringType.IsGenericInstance) return this;
			return GetRuntimeReference(this.DeclaringType);
		}

		/// <summary>
		/// Gets an EventReference as it will be seen by the .NET Runtime, with generic type parameters
		/// replaced by their arguments from <paramref name="ctx"/>.
		/// </summary>
		public virtual EventReference GetRuntimeReference(IGenericContext ctx)
		{
			if (ctx == this.DeclaringType && !ContainsGenericParameter) return this;
			return new ConstructedEventReference(this, ctx);
		}
	}

	static partial class Mixin
	{
		/// <summary>
		/// Compatible with Converter&lt;EventReference, EventReference&gt; and thus Array.ConvertAll
		/// </summary>
		/// <param name="r">Event reference</param>
		/// <returns>Result of calling r.GetRuntimeReference()</returns>
		public static EventReference GetRuntimeReference(EventReference r, IGenericContext ctx)
		{ 
			return r.GetRuntimeReference(ctx); 
		}
	}
}
