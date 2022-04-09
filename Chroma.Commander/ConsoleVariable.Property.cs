using System;
using System.Reflection;
using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    internal partial class ConsoleVariable
    {
        private PropertyInfo _propertyInfo;

        public ConsoleVariable(PropertyInfo propertyInfo, object owner)
        {
            EnsureSupportedType(propertyInfo);
            
            _propertyInfo = propertyInfo;
            _owner = owner;
        }
        
        private T GetProperty<T>()
        {
            try
            {
                return (T)Convert.ChangeType(
                    _propertyInfo.GetValue(_owner),
                    typeof(T)
                );
            }
            catch
            {
                throw new InvalidOperationException("Unable to retrieve field value: invalid type.");
            }
        }

        private void SetProperty(bool value)
        {
            if (_propertyInfo.PropertyType == typeof(bool))
            {
                _propertyInfo.SetValue(_owner, value);
            }
            else throw new ConVarTypeMismatchException(
                Type, 
                ExpressionValue.Type.Boolean
            );
        }

        private void SetProperty(string value)
        {
            if (_propertyInfo.PropertyType == typeof(string))
            {
                _propertyInfo.SetValue(_owner, value);
            }
            else if (_propertyInfo.PropertyType.IsEnum)
            {
                if (Enum.TryParse(_propertyInfo.PropertyType, value, out var enumValue))
                {
                    _propertyInfo.SetValue(_owner, enumValue);
                }
                else
                {
                    throw new ConVarConversionException(typeof(string), _propertyInfo.PropertyType);
                }
            }
            else throw new ConVarTypeMismatchException(
                Type, 
                ExpressionValue.Type.String
            );
        }
        
        private void SetProperty(double value)
        {
            if (_propertyInfo.PropertyType == typeof(double))
            {
                _propertyInfo.SetValue(_owner, value);
            }
            else if (_propertyInfo.PropertyType.IsEnum)
            {
                var enumValue = Convert.ChangeType(value, _propertyInfo.PropertyType.GetEnumUnderlyingType());
                _propertyInfo.SetValue(_owner, enumValue);
            }
            else throw new ConVarTypeMismatchException(
                Type, 
                ExpressionValue.Type.Number
            );
        }
        
        private ExpressionValue.Type GetPropertyExpressionType()
        {
            if (_propertyInfo.PropertyType == typeof(bool))
                return ExpressionValue.Type.Boolean;
            else if (_propertyInfo.PropertyType == typeof(string))
                return ExpressionValue.Type.String;
            else if (_propertyInfo.PropertyType == typeof(double))
                return ExpressionValue.Type.Number;
            else if (_propertyInfo.PropertyType.IsEnum)
                return ExpressionValue.Type.Number;

            throw new InvalidOperationException($"Unsupported property type {_propertyInfo.PropertyType.FullName}");
        }
        
        private void EnsureSupportedType(PropertyInfo prop)
        {
            if (!_supportedTypes.Contains(prop.PropertyType) && !prop.PropertyType.IsEnum)
                throw new InvalidOperationException($"Property type {prop.PropertyType.FullName} is not a supported convar type.");
        }
    }
}