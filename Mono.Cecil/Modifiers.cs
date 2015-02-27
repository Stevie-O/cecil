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

	public interface IModifierType {
		TypeReference ModifierType { get; }
		TypeReference ElementType { get; }
	}

	public sealed class OptionalModifierType : TypeSpecification, IModifierType {

		TypeReference modifier_type;

		public TypeReference ModifierType {
			get { return modifier_type; }
			set { modifier_type = value; }
		}

		public override string Name {
			get { return base.Name + Suffix; }
		}

		public override string FullName {
			get { return base.FullName + Suffix; }
		}

		string Suffix {
			get { return " modopt(" + modifier_type + ")"; }
		}

		public override bool IsValueType {
			get { return false; }
			set { throw new InvalidOperationException (); }
		}

		public override bool IsOptionalModifier {
			get { return true; }
		}

		public override bool ContainsGenericParameter {
			get { return modifier_type.ContainsGenericParameter || base.ContainsGenericParameter; }
		}

		public OptionalModifierType (TypeReference modifierType, TypeReference type)
			: base (type)
		{
			if (modifierType == null)
				throw new ArgumentNullException (Mixin.Argument.modifierType.ToString ());
			Mixin.CheckType (type);
			this.modifier_type = modifierType;
			this.etype = MD.ElementType.CModOpt;
		}

		public override TypeReference ApplyTypeArguments(IGenericContext ctx)
		{
			if (!ContainsGenericParameter) return this;

			// 2015-02-24 (SMO): I don't understand how modifier types work, so I hope this is correct
			TypeReference constructed_elt = ElementType.ApplyTypeArguments(ctx);
			TypeReference constructed_mod = ModifierType.ApplyTypeArguments(ctx);

			if (constructed_elt == ElementType && constructed_mod == ModifierType) return this;

			return new OptionalModifierType(constructed_mod, constructed_elt);
		}

	}

	public sealed class RequiredModifierType : TypeSpecification, IModifierType {

		TypeReference modifier_type;

		public TypeReference ModifierType {
			get { return modifier_type; }
			set { modifier_type = value; }
		}

		public override string Name {
			get { return base.Name + Suffix; }
		}

		public override string FullName {
			get { return base.FullName + Suffix; }
		}

		string Suffix {
			get { return " modreq(" + modifier_type + ")"; }
		}

		public override bool IsValueType {
			get { return false; }
			set { throw new InvalidOperationException (); }
		}

		public override bool IsRequiredModifier {
			get { return true; }
		}

		public override bool ContainsGenericParameter {
			get { return modifier_type.ContainsGenericParameter || base.ContainsGenericParameter; }
		}

		public RequiredModifierType (TypeReference modifierType, TypeReference type)
			: base (type)
		{
			if (modifierType == null)
				throw new ArgumentNullException (Mixin.Argument.modifierType.ToString ());
			Mixin.CheckType (type);
			this.modifier_type = modifierType;
			this.etype = MD.ElementType.CModReqD;
		}

		public override TypeReference ApplyTypeArguments(IGenericContext ctx)
		{
			if (!ContainsGenericParameter) return this;

			// 2015-02-24 (SMO): I don't understand how modifier types work, so I hope this is correct
			TypeReference constructed_elt = ElementType.ApplyTypeArguments(ctx);
			TypeReference constructed_mod = ModifierType.ApplyTypeArguments(ctx);

			if (constructed_elt == ElementType && constructed_mod == ModifierType) return this;

            return new RequiredModifierType(constructed_mod, constructed_elt);
        }

	}

}
