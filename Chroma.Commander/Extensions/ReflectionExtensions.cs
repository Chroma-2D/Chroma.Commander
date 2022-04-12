using System;
using System.Reflection;
using Chroma.Commander.Expressions;

namespace Chroma.Commander.Extensions
{
    internal static class ReflectionExtensions
    {
        public static bool IsNumerical(this Type type)
        {
            return type == typeof(double)
                   || type == typeof(float)
                   || type == typeof(int)
                   || type == typeof(uint)
                   || type == typeof(long)
                   || type == typeof(ulong)
                   || type == typeof(short)
                   || type == typeof(ushort)
                   || type == typeof(byte)
                   || type == typeof(sbyte);
        }

        public static bool IsValidConVarType(this Type type)
        {
            return type.IsNumerical()
                   || type == typeof(string)
                   || type == typeof(bool)
                   || type.IsEnum;
        }

        public static ExpressionValue.Type GetCorrespondingExpressionType(this Type type)
        {
            if (type == typeof(bool))
            {
                return ExpressionValue.Type.Boolean;
            }
            else if (type == typeof(string))
            {
                return ExpressionValue.Type.String;
            }
            else if (type.IsNumerical()
                     || type.IsEnum)
            {
                return ExpressionValue.Type.Number;
            }

            throw new NotSupportedException($"Type '{type.FullName}' is not a supported expression value type.");
        }

        public static ExpressionValue.Type GetCorrespondingExpressionType(this MemberInfo mi)
        {
            if (mi is FieldInfo fi)
            {
                try
                {
                    return GetCorrespondingExpressionType(fi.FieldType);
                }
                catch (Exception e)
                {
                    throw new NotSupportedException(
                        $"Field '{fi.Name}' is of an unsupported type '{fi.FieldType.FullName}'.",
                        e
                    );
                }
            }
            else if (mi is PropertyInfo pi)
            {
                try
                {
                    return GetCorrespondingExpressionType(pi.PropertyType);
                }
                catch (Exception e)
                {
                    throw new NotSupportedException(
                        $"Property '{pi.Name}' is of an unsupported type '{pi.PropertyType.FullName}'.",
                        e
                    );
                }
            }

            throw new NotSupportedException(
                $"{mi.MemberType} '{mi.Name}' is not a supported member type."
            );
        }

        public static Type GetClrType(this MemberInfo mi)
        {
            if (mi is FieldInfo fi)
            {
                return fi.FieldType;
            }
            else if (mi is PropertyInfo pi)
            {
                return pi.PropertyType;
            }

            throw new NotSupportedException(
                $"{mi.MemberType} '{mi.Name}' is not a supported member type."
            );
        }

        public static T GetValue<T>(this MemberInfo mi, object owner)
        {
            if (mi is FieldInfo fi)
            {
                return (T)Convert.ChangeType(
                    fi.GetValue(owner),
                    typeof(T)
                );
            }
            else if (mi is PropertyInfo pi)
            {
                return (T)Convert.ChangeType(
                    pi.GetValue(owner),
                    typeof(T)
                );
            }

            throw new NotSupportedException(
                $"{mi.MemberType} '{mi.Name}' is not a supported member type."
            );
        }

        public static void SetValue(this MemberInfo mi, object owner, object value)
        {
            if (mi is FieldInfo fi)
            {
                fi.SetValue(owner, value);
            }
            else if (mi is PropertyInfo pi)
            {
                pi.SetValue(owner, value);
            }
            else
            {
                throw new NotSupportedException(
                    $"{mi.MemberType} '{mi.Name}' is not a supported member type."
                );
            }
        }

        public static bool IsNumerical(this MemberInfo mi, out Type type)
        {
            if (mi is FieldInfo fi)
            {
                type = fi.FieldType;
                return fi.FieldType.IsNumerical();
            }
            else if (mi is PropertyInfo pi)
            {
                type = pi.PropertyType;
                return pi.PropertyType.IsNumerical();
            }
            else
            {
                throw new NotSupportedException(
                    $"{mi.MemberType} '{mi.Name}' is not a supported member type."
                );
            }
        }

        public static bool IsEnum(this MemberInfo mi, out Type type)
        {
            if (mi is FieldInfo fi)
            {
                type = fi.FieldType;
                return fi.FieldType.IsEnum;
            }
            else if (mi is PropertyInfo pi)
            {
                type = pi.PropertyType;
                return pi.PropertyType.IsEnum;
            }
            else
            {
                throw new NotSupportedException(
                    $"{mi.MemberType} '{mi.Name}' is not a supported member type."
                );
            }
        }

        public static bool IsBoolean(this MemberInfo mi)
        {
            if (mi is FieldInfo fi)
            {
                return fi.FieldType == typeof(bool);
            }
            else if (mi is PropertyInfo pi)
            {
                return pi.PropertyType == typeof(bool);
            }
            else
            {
                throw new NotSupportedException(
                    $"{mi.MemberType} '{mi.Name}' is not a supported member type."
                );
            }
        }

        public static bool IsString(this MemberInfo mi)
        {
            if (mi is FieldInfo fi)
            {
                return fi.FieldType == typeof(string);
            }
            else if (mi is PropertyInfo pi)
            {
                return pi.PropertyType == typeof(string);
            }
            else
            {
                throw new NotSupportedException(
                    $"{mi.MemberType} '{mi.Name}' is not a supported member type."
                );
            }
        }
    }
}