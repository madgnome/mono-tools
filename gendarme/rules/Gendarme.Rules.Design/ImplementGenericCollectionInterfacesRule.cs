//
// Gendarme.Rules.Design.ImplementGenericCollectionInterfacesRule
//
// Authors:
//	Daniel Abramov <ex@vingrad.ru>
//
// Copyright (C) 2008 Daniel Abramov
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

using Mono.Cecil;

using Gendarme.Framework;
using Gendarme.Framework.Rocks;
using Gendarme.Framework.Helpers;

namespace Gendarme.Rules.Design {

	[Problem ("This type implements non-generic IEnumerable interface but does not implement IEnumerable<T> interface that will make your collection type-safe.")]
	[Solution ("Implement one of generic collection interfaces such as IEnumerable<T>, ICollection<T> or IList<T>.")]
	public class ImplementGenericCollectionInterfacesRule : Rule, ITypeRule {

		public RuleResult CheckType (TypeDefinition type)
		{
			// rule applies only to 2.0+ assemblies
			TargetRuntime runtime = type.Module.Assembly.Runtime;
			if (runtime == TargetRuntime.NET_1_0 || runtime == TargetRuntime.NET_1_1)
				return RuleResult.DoesNotApply;
			
			// rule applies only to public types
			if (!type.IsPublic && !type.IsNestedPublic)
				return RuleResult.DoesNotApply;
			
			// rule does not apply to enums, interfaces and generated code
			if (type.IsEnum || type.IsInterface || type.IsGeneratedCode ())
				return RuleResult.DoesNotApply;

			// rule only applies if the type implements IEnumerable
			if (!type.Implements ("System.Collections.IEnumerable"))
				return RuleResult.DoesNotApply;
		
			// rule does not apply to the types implementing IDictionary
			if (type.Implements ("System.Collections.IDictionary"))
				return RuleResult.DoesNotApply;

			// the type should implement IEnumerable<T> too
			if (!type.Implements ("System.Collections.Generic.IEnumerable`1"))
				Runner.Report (type, Severity.Medium, Confidence.High, "Implement generic collection interface.");

			return Runner.CurrentRuleResult;
		}
	}
}