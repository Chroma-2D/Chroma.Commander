using System;
using System.Globalization;
using System.Reflection;
using Chroma.Commander.Expressions;
using Chroma.Commander.Extensions;

namespace Chroma.Commander
{
    internal class ConsoleVariable
    {
        private object _owner;
        private MemberInfo _member;

        internal string ClrTypeFullName => _member.GetClrType().FullName;
        
        public ExpressionValue.Type Type
            => _member.GetCorrespondingExpressionType();

        public bool IsWritable { get; }
        public bool IsReadable { get; }
        public bool IsEnum { get; }

        public ConsoleVariable(FieldInfo field, object owner = null)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            EnsureSupportedType(field);
            
            IsWritable = !field.IsInitOnly;
            IsReadable = true;
            IsEnum = field.FieldType.IsEnum;

            _member = field;
            _owner = owner;
        }

        public ConsoleVariable(PropertyInfo property, object owner = null)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            EnsureSupportedType(property);
            
            IsWritable = property.CanWrite;
            IsReadable = property.CanRead;
            IsEnum = property.PropertyType.IsEnum;

            _member = property;
            _owner = owner;
        }

        public double GetNumber()
        {
            if (!IsReadable)
                throw new ConVarReadException();

            return _member.GetValue<double>(_owner);
        }

        public string GetString()
        {
            if (!IsReadable)
                throw new ConVarReadException();

            return _member.GetValue<string>(_owner);
        }

        public bool GetBoolean()
        {
            if (!IsReadable)
                throw new ConVarReadException();

            return _member.GetValue<bool>(_owner);
        }

        public void Set(bool value)
        {
            if (!IsWritable)
                throw new ConVarWriteException();

            if (_member.IsBoolean())
            {
                _member.SetValue(_owner, value);
            }
            else
            {
                throw new ConVarTypeMismatchException(
                    Type,
                    ExpressionValue.Type.Boolean
                );
            }
        }

        public void Set(string value)
        {
            if (!IsWritable)
                throw new ConVarWriteException();

            if (_member.IsString())
            {
                _member.SetValue(_owner, value);
            }
            else if (_member.IsEnum(out var enumType))
            {
                if (Enum.TryParse(enumType, value, out var enumValue))
                {
                    _member.SetValue(_owner, enumValue);
                }
                else
                {
                    throw new ConVarConversionException(
                        value,
                        ExpressionValue.Type.String,
                        enumType
                    );
                }
            }
            else
            {
                throw new ConVarTypeMismatchException(
                    Type,
                    ExpressionValue.Type.String
                );
            }
        }

        public void Set(double value)
        {
            if (!IsWritable)
                throw new ConVarWriteException();

            if (_member.IsNumerical(out var numericalMemberType))
            {
                try
                {
                    _member.SetValue(
                        _owner,
                        Convert.ChangeType(value, numericalMemberType)
                    );
                }
                catch (OverflowException)
                {
                    throw new ConVarOutOfRangeException(
                        $"'{value}' was too large for the backing type of this variable."
                    );
                }
            }
            else if (_member.IsEnum(out var enumMemberType))
            {
                var enumValue = Convert.ChangeType(
                    value,
                    enumMemberType.GetEnumUnderlyingType()
                );

                _member.SetValue(_owner, enumValue);
            }
            else
            {
                throw new ConVarTypeMismatchException(
                    Type,
                    ExpressionValue.Type.Number
                );
            }
        }

        public override string ToString()
        {
            return Type switch
            {
                ExpressionValue.Type.Boolean => GetBoolean().ToString().ToLower(),
                ExpressionValue.Type.Number => GetNumber().ToString(CultureInfo.InvariantCulture),
                ExpressionValue.Type.String => "\"" + GetString() + "\"",
                _ => throw new InvalidOperationException("Unexpected underlying type.")
            };
        }

        private void EnsureSupportedType(FieldInfo field)
        {
            if (!field.FieldType.IsValidConVarType())
            {
                throw new Exception(
                    $"Field type {field.FieldType.FullName} is not a supported variable type."
                );
            }
        }

        private void EnsureSupportedType(PropertyInfo property)
        {
            if (!property.PropertyType.IsValidConVarType())
            {
                throw new Exception(
                    $"Property type {property.PropertyType.FullName} is not a supported variable type."
                );
            }
        }
    }
}