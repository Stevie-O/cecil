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

using MD = Mono.Cecil.Metadata;

namespace Mono.Cecil {

	public sealed class PinnedType : TypeSpecification {

		public override bool IsValueType {
			get { return false; }
			set { throw new InvalidOperationException (); }
		}

		public override bool IsPinned {
			get { return true; }
		}

		public PinnedType (TypeReference type)
			: base (type)
		{
			Mixin.CheckType (type);
			this.etype = MD.ElementType.Pinned;
		}

        public override TypeReference ApplyTypeArguments(IGenericContext ctx)
        {
            if (!HasGenericParameters) return this;

            TypeReference constructed_elt = ElementType.ApplyTypeArguments(ctx);
            // if ElementType.ApplyTypeArguments just returned itself, there were no generic parameters to be replaced
            if (constructed_elt == ElementType) return this;
            return new PinnedType(constructed_elt);
        }
	}
}
