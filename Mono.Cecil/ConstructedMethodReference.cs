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
using Mono.Collections.Generic;

namespace Mono.Cecil
{
    /// <summary>
    /// Represents a reference to a constructed method.
    /// </summary>
    /// <remarks>
    /// Example:
    /// <code><![CDATA[
    ///public class SomeClass<T>
    ///{
    ///    public T DoSomething(T item) { return item; }
    ///    public U DoSomething2<U>(T item, U item2) { return item2; }
    ///}
    ///public class AnotherClass
    ///{
    ///     public T DoSomething3<T>(T item) { return item; }
    ///}
    ///]]></code>
    /// A ConstructedMethodReference can refer to things such as:
    /// <list type="bullet">
    /// <item><c>SomeClass&lt;int&gt;.DoSomething</c></item>
    /// <item><c>SomeClass&lt;int&gt;.DoSomething2&lt;string&gt;</c></item>
    /// <item><c>AnotherClass.DoSomething3&lt;int&gt;</c></item>
    /// </list>
    /// </remarks>
    public sealed class ConstructedMethodReference : MethodSpecification, IGenericInstance
    {
        readonly IGenericContext _ctx;

        public ConstructedMethodReference(MethodReference r, IGenericContext ctx)
            : base(r)
        {
            _ctx = ctx;
        }

        public override MethodReference[] GetOverrides()
        {
            return Array.ConvertAll(ElementMethod.GetOverrides(), (r) => Mixin.GetRuntimeReference(r, _ctx));
        }

        public override TypeReference DeclaringType
        {
            get { return (_ctx.InstanceType as TypeReference) ?? ElementMethod.DeclaringType; }
            set { throw new NotSupportedException(); }
        }

        public override MethodDefinition Resolve()
        {
            return ElementMethod.Resolve();
        }

        public override ParameterReference[] GetParameters()
        {
            MethodReference realMethod = ElementMethod;

            ParameterReference[] baseParams = realMethod.GetParameters();
            if (baseParams.Length == 0) return baseParams;
            ModuleParameterCollection mpc = new ModuleParameterCollection(this, baseParams.Length);
            foreach (ParameterReference p in baseParams)
            {
                ParameterReference p2 = new ModuleParameterReference(p.ParameterType.ApplyTypeArguments(_ctx));
                mpc.Add(p2);
            }
            return mpc.ToArray();
        }

        public override TypeReference ReturnType
        {
            get
            {
                TypeReference returnType = ElementMethod.ReturnType;
                if (returnType != null) returnType = returnType.ApplyTypeArguments(_ctx);
                return returnType;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public bool HasGenericArguments
        {
            get
            {
                var impl = ElementMethod as IGenericInstance;
                return impl != null && impl.HasGenericArguments;
            }
        }

        public Collection<TypeReference> GenericArguments
        {
            get
            {
                var impl = ElementMethod as IGenericInstance;
                return impl != null ? impl.GenericArguments : null;
            }
        }

        protected override string MemberFullName()
        {
            string baseName = base.MemberFullName();
            IGenericInstance gen_meth = ElementMethod as IGenericInstance;
            if (gen_meth != null)
            {
                StringBuilder sb = new StringBuilder(baseName);
                gen_meth.GenericInstanceFullName(sb);
                return sb.ToString();
            }
            if (ElementMethod.HasGenericParameters)
            {
                StringBuilder sb = new StringBuilder(baseName);
                ElementMethod.GenericParametersFullName(sb);
                return sb.ToString();
            }
            return baseName;
       }

    }

}
