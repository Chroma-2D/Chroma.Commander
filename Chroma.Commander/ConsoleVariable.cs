using System;
using System.Collections.Generic;
using System.Globalization;
using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    internal partial class ConsoleVariable
    {
        private static HashSet<Type> _supportedTypes = new()
        {
            typeof(bool),
            typeof(string),
            typeof(double)
        };
        
        private object _owner;

        public ExpressionValue.Type Type
        {
            get
            {
                if (_fieldInfo != null)
                    return GetFieldExpressionType();
                else if (_propertyInfo != null)
                    return GetPropertyExpressionType();
                
                throw new InvalidOperationException("Console variable is in an invalid state.");
            }
        }

        public bool RepresentsEnum
        {
            get
            {
                if (_fieldInfo != null)
                    return _fieldInfo.FieldType.IsEnum;
                else if (_propertyInfo != null)
                    return _propertyInfo.PropertyType.IsEnum;
                
                throw new InvalidOperationException("Console variable is in an invalid state.");
            }
        }

        public double GetNumber()
        {
            if (_fieldInfo != null)
            {
                return GetField<double>();
            }
            else if (_propertyInfo != null)
            {
                if (!_propertyInfo.CanRead)
                    throw new ConVarReadException();
                
                return GetProperty<double>();
            }
            else
            {
                throw new InvalidOperationException("Console variable is in an invalid state.");
            }
        }
        
        public string GetString()
        {
            if (_fieldInfo != null)
            {
                return GetField<string>();
            }
            else if (_propertyInfo != null)
            {
                if (!_propertyInfo.CanRead)
                    throw new ConVarReadException();
                
                return GetProperty<string>();
            }
            else
            {
                throw new InvalidOperationException("Console variable is in an invalid state.");
            }
        }
        
        public bool GetBoolean()
        {
            if (_fieldInfo != null)
            {
                return GetField<bool>();
            }
            else if (_propertyInfo != null)
            {
                if (!_propertyInfo.CanRead)
                    throw new ConVarReadException();
                
                return GetProperty<bool>();
            }
            else
            {
                throw new InvalidOperationException("Console variable is in an invalid state.");
            }
        }

        public void Set(bool value)
        {
            if (_fieldInfo != null)
            {
                if (_fieldInfo.IsInitOnly)
                    throw new ConVarWriteException();

                SetField(value);
            }
            else if (_propertyInfo != null)
            {
                if (!_propertyInfo.CanWrite)
                    throw new ConVarWriteException();
                
                SetProperty(value);
            }
            else
            {
                throw new InvalidOperationException("Console variable is in an invalid state.");
            }
        }

        public void Set(string value)
        {
            if (_fieldInfo != null)
            {
                if (_fieldInfo.IsInitOnly)
                    throw new ConVarWriteException();

                SetField(value);
            }
            else if (_propertyInfo != null)
            {
                if (!_propertyInfo.CanWrite)
                    throw new ConVarWriteException();

                SetProperty(value);
            }
            else
            {
                throw new InvalidOperationException("Console variable is in an invalid state.");
            }
        }

        public void Set(double value)
        {
            if (_fieldInfo != null)
            {
                if (_fieldInfo.IsInitOnly)
                    throw new ConVarWriteException();

                SetField(value);
            }
            else if (_propertyInfo != null)
            {
                if (!_propertyInfo.CanWrite)
                    throw new ConVarWriteException();

                SetProperty(value);
            }
            else
            {
                throw new InvalidOperationException("Console variable is in an invalid state.");
            }
        }

        public override string ToString()
        {
            return Type switch
            {
                ExpressionValue.Type.Boolean => GetBoolean().ToString().ToLower(),
                ExpressionValue.Type.Number => GetNumber().ToString(CultureInfo.InvariantCulture),
                ExpressionValue.Type.String => GetString(),
                _ => throw new InvalidOperationException("Console variable is in an invalid state.")
            };
        }
    }
}