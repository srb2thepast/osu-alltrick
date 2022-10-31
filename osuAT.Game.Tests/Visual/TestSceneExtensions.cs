using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using osu.Framework.Testing;
using osu.Framework.Extensions.IEnumerableExtensions;
using NUnit.Framework.Internal;
using NUnit.Framework;
using osu.Framework.Testing.Drawables.Steps;
using osuTK.Graphics;
using System.Runtime.CompilerServices;

namespace osuAT.Game.Tests.Visual
{
    public static class TestSceneExtensions
    {
        // from osu.Framework.Testing.TestBrowser.finishload()
        /// <summary>
        /// Manually adds every attributed test.
        /// Should only be used when this TestScene is not wrapped by a TestBrowser.
        /// </summary>
        /// <remarks>TestCaseSource is not currently supported.</remarks>
        public static void ManuallyAddAttributeTests(this TestScene scene)
        {
            bool hadTestAttributeTest = false;
            foreach (var m in scene.GetType().GetMethods())
            {
                string name = m.Name;

                if (name == nameof(TestScene.TestConstructor) || m.GetCustomAttribute(typeof(IgnoreAttribute), false) != null)
                    continue;

                if (name.StartsWith("Test", StringComparison.Ordinal))
                    name = name.Substring(4);

                int runCount = 1;

                if (m.GetCustomAttribute(typeof(RepeatAttribute), false) != null)
                {
                    object count = m.GetCustomAttributesData().Single(a => a.AttributeType == typeof(RepeatAttribute)).ConstructorArguments.Single().Value;
                    Debug.Assert(count != null);

                    runCount += (int)count;
                }

                for (int i = 0; i < runCount; i++)
                {
                    string repeatSuffix = i > 0 ? $" ({i + 1})" : string.Empty;

                    var methodWrapper = new MethodWrapper(m.GetType(), m);

                    if (methodWrapper.GetCustomAttributes<TestAttribute>(false).SingleOrDefault() != null)
                    {
                        var parameters = m.GetParameters();

                        if (parameters.Length > 0)
                        {
                            var valueMatrix = new List<List<object>>();

                            foreach (var p in methodWrapper.GetParameters())
                            {
                                var valueAttrib = p.GetCustomAttributes<ValuesAttribute>(false).SingleOrDefault();
                                if (valueAttrib == null)
                                    throw new ArgumentException($"Parameter is present on a {nameof(TestAttribute)} method without values specification.", p.ParameterInfo.Name);

                                List<object> choices = new List<object>();

                                foreach (object choice in valueAttrib.GetData(p))
                                    choices.Add(choice);

                                valueMatrix.Add(choices);
                            }

                            foreach (var combination in valueMatrix.CartesianProduct())
                            {
                                hadTestAttributeTest = true;
                                scene.AddLabel($"{name}({string.Join(", ", combination)}){repeatSuffix}");
                                handleTestMethod(m, combination.ToArray());
                            }
                        }
                        else
                        {
                            hadTestAttributeTest = true;
                            scene.AddLabel($"{name}{repeatSuffix}");
                            handleTestMethod(m);
                        }
                    }

                    foreach (var tc in m.GetCustomAttributes(typeof(TestCaseAttribute), false).OfType<TestCaseAttribute>())
                    {
                        hadTestAttributeTest = true;
                        scene.AddLabel($"{name}({string.Join(", ", tc.Arguments)}){repeatSuffix}");

                        handleTestMethod(m, tc.Arguments);
                    }

                    /* TestCaseSource is not currently supported.
                    foreach (var tcs in m.GetCustomAttributes(typeof(TestCaseSourceAttribute), false).OfType<TestCaseSourceAttribute>())
                    {
                        IEnumerable sourceValue = getTestCaseSourceValue(m, tcs);

                        if (sourceValue == null)
                        {
                            Debug.Assert(tcs.SourceName != null);
                            throw new InvalidOperationException($"The value of the source member {tcs.SourceName} must be non-null.");
                        }

                        foreach (object argument in sourceValue)
                        {
                            hadTestAttributeTest = true;

                            if (argument is IEnumerable argumentsEnumerable)
                            {
                                object[] arguments = argumentsEnumerable.Cast<object>().ToArray();

                                AddLabel($"{name}({string.Join(", ", arguments)}){repeatSuffix}");
                                handleTestMethod(m, arguments);
                            }
                            else
                            {
                                AddLabel($"{name}({argument}){repeatSuffix}");
                                handleTestMethod(m, argument);
                            }
                        }
                    }

                    */
                }
            }

            // even if no [Test] or [TestCase] methods were found, [SetUp] steps should be added.
            if (!hadTestAttributeTest)
                addSetUpSteps();

            void addSetUpSteps()
            {
                var setUpMethods = ReflectionUtils.GetMethodsWithAttribute(scene.GetType(), typeof(SetUpAttribute), true)
                                                  .Where(m => m.Name != nameof(TestScene.SetUpTestForNUnit));

                if (setUpMethods.Any())
                {
                    scene.AddStep(new SingleStepButton(true)
                    {
                        Text = "[SetUp]",
                        LightColour = Color4.Teal,
                        Action = () => setUpMethods.ForEach(s => s.Invoke(scene, null))
                    });
                }

            }

            void handleTestMethod(MethodInfo methodInfo, params object[] arguments)
            {
                addSetUpSteps();
                methodInfo.Invoke(scene, arguments);
            }
        }

    }
}
