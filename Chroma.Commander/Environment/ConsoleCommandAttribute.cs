using System;
using Chroma.Commander.Expressions;
using Chroma.Commander.Extensions;

namespace Chroma.Commander.Environment
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ConsoleCommandAttribute : Attribute
    {
        private ExpressionValue[] _defaultArgumentValues;

        public string Trigger { get; }
        public string Description { get; init; } = "description not available";
        public object[] DefaultArguments { get; init; }

        public ExpressionValue[] DefaultArgumentValues
        {
            get
            {
                if (DefaultArguments == null)
                    return null;

                if (_defaultArgumentValues == null)
                {
                    _defaultArgumentValues = new ExpressionValue[DefaultArguments.Length];

                    for (var i = 0; i < DefaultArguments.Length; i++)
                    {
                        var defaultArgument = DefaultArguments[i];

                        try
                        {
                            var type = defaultArgument.GetType();
                            var valType = type.GetCorrespondingExpressionType();

                            switch (valType)
                            {
                                case ExpressionValue.Type.Boolean:
                                {
                                    _defaultArgumentValues[i] = new ExpressionValue(
                                        (bool)defaultArgument
                                    );
                                    break;
                                }

                                case ExpressionValue.Type.Number:
                                {
                                    _defaultArgumentValues[i] = new ExpressionValue(
                                        (double)Convert.ChangeType(defaultArgument, TypeCode.Double)
                                    );
                                    break;
                                }

                                case ExpressionValue.Type.String:
                                {
                                    _defaultArgumentValues[i] = new ExpressionValue(
                                        (string)defaultArgument
                                    );
                                    break;
                                }
                            }
                        }
                        catch (NotSupportedException)
                        {
                            // Ignore any incompatible types.
                        }
                    }
                }

                return _defaultArgumentValues;
            }
        }

        public ConsoleCommandAttribute(string trigger)
        {
            Trigger = trigger;
        }
    }
}