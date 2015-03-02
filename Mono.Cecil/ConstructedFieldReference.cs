//
// GenericFieldReference.cs
//
// Author:
//   Stephen Oberholtzer (stevie@qrpff.net)
//
// Copyright (c) 2015 Stephen Oberholtzer
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
using System.Collections.Generic;
using System.Text;

namespace Mono.Cecil
{
    /// <summary>
    /// Represents a reference to a field that belongs to a constructed type.
    /// </summary>
    /// <remarks>
    /// Example:
    /// <code><![CDATA[
    /// 
    ///public class SomeClass<T>
    ///{
    ///    public T SomeField;
    ///}
    ///]]></code>
    /// A ConstructedFieldReference can refer to <c>SomeClass&lt;int&gt;.SomeField</c>
    /// </remarks>
    public class ConstructedFieldReference : FieldReference
    {
        readonly FieldReference _underlyingFieldReference;

        public ConstructedFieldReference(FieldReference f, IGenericContext ctx)
            : base(f.Name, f.FieldType, f.DeclaringType)
        {
            _underlyingFieldReference = f;
            DeclaringType = (ctx.InstanceType as TypeReference) ?? f.DeclaringType;
            FieldType = f.FieldType.ApplyTypeArguments(ctx);
        }

        public override FieldDefinition Resolve()
        {
            return _underlyingFieldReference.Resolve();
        }

        public override string Name
        {
            get { return _underlyingFieldReference.Name; }
            set { _underlyingFieldReference.Name = value; }
        }

    }
}
