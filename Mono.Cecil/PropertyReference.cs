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

		internal PropertyReference (string name, TypeReference propertyType)
			: base (name)
		{
			Mixin.CheckType (propertyType, Mixin.Argument.propertyType);

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
    }
}
