using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ImageClassifierApp.Objects.Extensions
{
    public static class EnumExtension
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum paEnum) where TAttribute : System.Attribute
        {
            var type = paEnum.GetType();
            var member = type.GetMember(paEnum.ToString()).First();
            return member.GetCustomAttributes<TAttribute>(true).FirstOrDefault();
        }

        public static void RemoveFlags(this Enum paEnum)
        {
            var enumType = paEnum.GetType();
            Enum.GetNames(enumType).ForEach(memberName =>
            {
                var value = (int)Enum.Parse(paEnum.GetType(), memberName);
                paEnum = (Enum)Enum.ToObject(enumType, (int)(object)paEnum & ~value);
            }).ExecuteQuery();
        }

        public static IEnumerable<TEnum> Enumerate<TEnum>(this Enum paEnum)
        {
            var enumType = paEnum.GetType();
            if (typeof(TEnum) != enumType)
                throw new InvalidEnumArgumentException("Generic parameter is not of enum type.");
            return Enum.GetNames(paEnum.GetType()).Select(memberName => (TEnum)Enum.Parse(enumType, memberName));
        }

        public static string ToLowerString(this Enum paEnum)
        {
            return paEnum.ToString().ToLower();
        }

        public static string GetFormattedName(this Enum paEnum)
        {
            var type = paEnum.GetType();
            var name = Enum.GetName(type, paEnum);
            
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<DisplayAttribute>()
                .SingleOrDefault()?.Name ?? paEnum.ToString();
        }

        public static string InsertSpaceBeforeUpperCase(this string paString)
        {
            var sb = new StringBuilder();
            char previousChar = char.MinValue;
            foreach (char c in paString)
            {
                if (char.IsUpper(c))
                {
                    if (sb.Length != 0 && previousChar != ' ')
                    {
                        sb.Append(' ');
                    }
                }
                sb.Append(c);
                previousChar = c;
            }
            return sb.ToString();
        }
    }
}
