﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DMG {
    public static class ReflectionHelper {
        public static Type FindType(string fullName) {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetSafeTypes()).FirstOrDefault(t => t.FullName != null && t.FullName.Equals(fullName));
        }

        public static Type FindType(string fullName, string assemblyName) {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetSafeTypes()).FirstOrDefault(t => t.FullName != null && t.FullName.Equals(fullName) && t.Assembly.GetName().Name.Equals(assemblyName));
        }

        public static IEnumerable<Type> GetSafeTypes(this Assembly assembly) {
            try {
                return assembly.GetTypes();
            } catch (ReflectionTypeLoadException e) {
                return e.Types.Where(x => x != null);
            } catch (Exception) {
                return new List<Type>();
            }
        }

        static readonly Dictionary<string, MemberInfo> MemberCache = new Dictionary<string, MemberInfo>();
        public static T GetCachedMember<T>(this Type type, string member) where T : MemberInfo {
            // Use AssemblyQualifiedName and the member name as a unique key to prevent a collision if two types have the same member name
            var key = type.AssemblyQualifiedName + member;
            if (MemberCache.ContainsKey(key)) return MemberCache[key] is T cachedCastedInfo ? cachedCastedInfo : null;

            MemberInfo memberInfo = type.GetMember(member, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).FirstOrDefault();
            MemberCache[key] = memberInfo;

            return memberInfo is T castedInfo ? castedInfo : null;
        }

        public static T GetValue<T>(this Type type, string member, object target = null) =>
            (T) ((type.GetCachedMember<FieldInfo>(member)?.GetValue(target)) ?? (type.GetCachedMember<PropertyInfo>(member)?.GetValue(target, null)));

        public static void SetValue(this Type type, string member, object value, object target = null) {
            type.GetCachedMember<FieldInfo>(member)?.SetValue(target, value);
            type.GetCachedMember<PropertyInfo>(member)?.SetValue(target, value, null);
        }

        public static T CallMethod<T>(this Type type, string method, object target = null, params object[] arguments) => (T) (type.GetCachedMember<MethodInfo>(method)?.Invoke(target, arguments));

        public static void CallMethod(this Type type, string method, object target = null, params object[] arguments) => type.GetCachedMember<MethodInfo>(method)?.Invoke(target, arguments);

        public static T GetValue<T>(this object @object, string member) => @object.GetType().GetValue<T>(member, @object);
        public static void SetValue(this object @object, string member, object value) => @object.GetType().SetValue(member, value, @object);
        public static T CallMethod<T>(this object @object, string member, params object[] arguments) => @object.GetType().CallMethod<T>(member, @object, arguments);
        public static void CallMethod(this object @object, string member, params object[] arguments) => @object.GetType().CallMethod(member, @object, arguments);
    }
}
