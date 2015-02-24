//
// PropertyReference.cs
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

		internal PropertyReference (string name, TypeReference propertyType)
			: base (name)
		{
			if (propertyType == null)
				throw new ArgumentNullException ("propertyType");

			property_type = propertyType;
		}

		public abstract PropertyDefinition Resolve ();

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
    }
}
