//
// EventReference.cs
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
			if (eventType == null)
				throw new ArgumentNullException ("eventType");

			event_type = eventType;
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
