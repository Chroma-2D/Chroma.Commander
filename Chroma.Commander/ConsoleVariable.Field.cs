using System;
using System.Reflection;
using Chroma.Commander.Expressions;

namespace Chroma.Commander
{
    internal partial class ConsoleVariable
    {
        private FieldInfo _fieldInfo;

        public ConsoleVariable(FieldInfo fieldInfo, object owner = null)
        {
            EnsureSupportedType(fieldInfo);
            
            _fieldInfo = fieldInfo;
            _owner = owner;
        }

        private T GetField<T>()
        {
            try
            {
                return (T)Convert.ChangeType(
                    _fieldInfo.GetValue(_owner),
                    typeof(T)
                );
            }
            catch
            {
                throw new InvalidOperationException("Unable to retrieve field value: invalid type.");
            }
        }
        
        private void SetField(bool value)
        {
            if (_fieldInfo.FieldType == typeof(bool))
            {
                _fieldInfo.SetValue(_owner, value);
            }
            else throw new ConVarTypeMismatchException(
                Type, 
                ExpressionValue.Type.Boolean
            );
        }

        private void SetField(string value)
        {
            if (_fieldInfo.FieldType == typeof(string))
            {
                _fieldInfo.SetValue(_owner, value);
            }
            else if (_fieldInfo.FieldType.IsEnum)
            {
                if (Enum.TryParse(_fieldInfo.FieldType, value, out var enumValue))
                {
                    _fieldInfo.SetValue(_owner, enumValue);
                }
                else
                {
                    throw new ConVarConversionException(typeof(string), _fieldInfo.FieldType);
                }
            }
            else throw new ConVarTypeMismatchException(
                Type, 
                ExpressionValue.Type.String
            );
        }
        
        private void SetField(double value)
        {
            if (_fieldInfo.FieldType == typeof(double))
            {
                _fieldInfo.SetValue(_owner, value);
            }
            else if (_fieldInfo.FieldType.IsEnum)
            {
                var enumValue = Convert.ChangeType(value, _fieldInfo.FieldType.GetEnumUnderlyingType());
                _fieldInfo.SetValue(_owner, enumValue);
            }
            else throw new ConVarTypeMismatchException(
                Type, 
                ExpressionValue.Type.Number
            );
        }

        private ExpressionValue.Type GetFieldExpressionType()
        {
            if (_fieldInfo.FieldType == typeof(bool))
                return ExpressionValue.Type.Boolean;
            else if (_fieldInfo.FieldType == typeof(string))
                return ExpressionValue.Type.String;
            else if (_fieldInfo.FieldType == typeof(double))
                return ExpressionValue.Type.Number;
            else if (_fieldInfo.FieldType.IsEnum)
                return ExpressionValue.Type.Number;

            throw new InvalidOperationException($"Unsupported field type {_fieldInfo.FieldType.FullName}");
        }
        
        private void EnsureSupportedType(FieldInfo field)
        {
            if (!_supportedTypes.Contains(field.FieldType) && !field.FieldType.IsEnum)
                throw new InvalidOperationException($"Field type {field.FieldType.FullName} is not a supported convar type.");
        }
    }
}