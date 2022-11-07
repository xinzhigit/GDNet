using Microsoft.CSharp;
using Net.Event;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

public static class Fast2BuildMethod
{
    private class Member
    {
        internal string Name;
        internal bool IsPrimitive;
        internal bool IsEnum;
        internal bool IsArray;
        internal bool IsGenericType;
        internal Type Type;
        internal TypeCode TypeCode;
        internal Type ItemType;
        internal Type ItemType1;
    }

    /// <summary>
    /// 动态编译, 在unity开发过程中不需要生成绑定cs文件, 直接运行时编译使用, 当编译apk. app时才进行生成绑定cs文件
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public static bool DynamicBuild(params Type[] types)
    {
        return DynamicBuild(0, types);
    }

    /// <summary>
    /// 动态编译, 在unity开发过程中不需要生成绑定cs文件, 直接运行时编译使用, 当编译apk. app时才进行生成绑定cs文件
    /// </summary>
    /// <param name="compilerOptionsIndex">编译参数, 如果编译失败, 可以选择0-7测试哪个编译成功</param>
    /// <param name="types"></param>
    /// <returns></returns>
    public static bool DynamicBuild(int compilerOptionsIndex, params Type[] types)
    {
        List<string> codes = new List<string>();
        foreach (var type in types)
        {
            var str = BuildNew(type, true, true, new List<string>());
            str.Append(BuildArray(type));
            str.Append(BuildGeneric(type));
            codes.Add(str.ToString());
        }
        CSharpCodeProvider provider = new CSharpCodeProvider();
        CompilerParameters param = new CompilerParameters();
        HashSet<string> dllpaths = new HashSet<string>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assemblie in assemblies)
        {
            var path = assemblie.Location;
            var name = Path.GetFileName(path);
            if (name.Contains("Editor"))
                continue;
            if (name.Contains("mscorlib"))
                continue;
            if (!File.Exists(path))
                continue;
            if (path.Contains("PackageCache"))
                continue;
            dllpaths.Add(path);
        }
        param.ReferencedAssemblies.AddRange(dllpaths.ToArray());
        param.GenerateExecutable = false;
        param.GenerateInMemory = true;
        var options = new string[] { "/langversion:experimental", "/langversion:default", "/langversion:ISO-1", "/langversion:ISO-2", "/langversion:3", "/langversion:4", "/langversion:5", "/langversion:6", "/langversion:7" };
        param.CompilerOptions = options[compilerOptionsIndex];
        CompilerResults cr = provider.CompileAssemblyFromSource(param, codes.ToArray());
        if (cr.Errors.HasErrors)
        {
            NDebug.LogError("编译错误：");
            foreach (CompilerError err in cr.Errors)
                NDebug.LogError(err.ErrorText);
            return false;
        }
        Net.Serialize.NetConvertFast2.Init();
        foreach (var type in types)
            Net.Serialize.NetConvertFast2.AddSerializeType3(type);
        NDebug.Log("编译成功");
        return true;
    }

    public static void Build(Type type, string savePath)
    {
        var str = BuildNew(type, true, true, new List<string>(), savePath);
        var className = type.FullName.Replace(".", "").Replace("+", "");
        File.WriteAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }

    public static void Build(Type type, string savePath, bool serField, bool serProperty, List<string> ignores)
    {
        var str = BuildNew(type, serField, serProperty, ignores, savePath);
        var className = type.FullName.Replace(".", "").Replace("+", "");
        File.WriteAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }

    public static StringBuilder BuildNew(Type type, bool serField, bool serProperty, List<string> ignores, string savePath = null)
    {
        StringBuilder sb = new StringBuilder();
        StringBuilder sb1 = new StringBuilder();
        FieldInfo[] fields;
        if (serField)
            fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        else
            fields = new FieldInfo[0];
        PropertyInfo[] properties;
        if (serProperty)
            properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        else
            properties = new PropertyInfo[0];
        List<Member> members = new List<Member>();
        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                continue;
            if (ignores.Contains(field.Name))
                continue;
            var member = new Member()
            {
                IsArray = field.FieldType.IsArray,
                IsEnum = field.FieldType.IsEnum,
                IsGenericType = field.FieldType.IsGenericType,
                IsPrimitive = Type.GetTypeCode(field.FieldType) != TypeCode.Object,
                Name = field.Name,
                Type = field.FieldType,
                TypeCode = Type.GetTypeCode(field.FieldType)
            };
            if (field.FieldType.IsArray)
            {
                var serType = field.FieldType.GetInterface(typeof(IList<>).FullName);
                var itemType = serType.GetGenericArguments()[0];
                member.ItemType = itemType;
            }
            else if (field.FieldType.GenericTypeArguments.Length == 1)
            {
                Type itemType = field.FieldType.GenericTypeArguments[0];
                member.ItemType = itemType;
            }
            else if (field.FieldType.GenericTypeArguments.Length == 2)
            {
                Type itemType = field.FieldType.GenericTypeArguments[0];
                Type itemType1 = field.FieldType.GenericTypeArguments[1];
                member.ItemType = itemType;
                member.ItemType1 = itemType1;
            }
            members.Add(member);
        }
        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                continue;
            if (!property.CanRead | !property.CanWrite)
                continue;
            if (property.GetIndexParameters().Length > 0)
                continue;
            if (ignores.Contains(property.Name))
                continue;
            var member = new Member()
            {
                IsArray = property.PropertyType.IsArray,
                IsEnum = property.PropertyType.IsEnum,
                IsGenericType = property.PropertyType.IsGenericType,
                IsPrimitive = Type.GetTypeCode(property.PropertyType) != TypeCode.Object,
                Name = property.Name,
                Type = property.PropertyType,
                TypeCode = Type.GetTypeCode(property.PropertyType)
            };
            if (property.PropertyType.IsArray)
            {
                var serType = property.PropertyType.GetInterface(typeof(IList<>).FullName);
                var itemType = serType.GetGenericArguments()[0];
                member.ItemType = itemType;
            }
            else if (property.PropertyType.GenericTypeArguments.Length == 1)
            {
                Type itemType = property.PropertyType.GenericTypeArguments[0];
                member.ItemType = itemType;
            }
            else if (property.PropertyType.GenericTypeArguments.Length == 2)
            {
                Type itemType = property.PropertyType.GenericTypeArguments[0];
                Type itemType1 = property.PropertyType.GenericTypeArguments[1];
                member.ItemType = itemType;
                member.ItemType1 = itemType1;
            }
            members.Add(member);
        }

        var templateText = @"using Net.Serialize;
using Net.System;
using System;
using System.Collections.Generic;

namespace Binding
{
    public struct {TYPENAME}Bind : ISerialize<{TYPE}>, ISerialize
    {
        public void Write({TYPE} value, Segment stream)
        {
            int pos = stream.Position;
            stream.Position += {SIZE};
            byte[] bits = new byte[{SIZE}];
{Split}
            if ({Condition})
            {
                NetConvertBase.SetBit(ref bits[{BITPOS}], {FIELDINDEX}, true);
                stream.Write(value.{FIELDNAME});
            }
{Split}
			if({Condition})
			{
				NetConvertBase.SetBit(ref bits[{BITPOS}], {FIELDINDEX}, true);
				var bind = new {BINDTYPE}();
				bind.Write(value.{FIELDNAME}, stream);
			}
{Split}
			if({Condition})
			{
				NetConvertBase.SetBit(ref bits[{BITPOS}], {FIELDINDEX}, true);
				var bind = new DictionaryBind<{KEYTYPE}, {VALUETYPE}>();
				bind.Write(value.{FIELDNAME}, stream, new {BINDTYPE}());
			}
{Split}
            int pos1 = stream.Position;
            stream.Position = pos;
            stream.Write(bits, 0, {SIZE});
            stream.Position = pos1;
        }
		
		public {TYPE} Read(Segment stream)
		{
			byte[] bits = stream.Read({SIZE});
			var value = new {TYPE}();
{Split}
			if(NetConvertBase.GetBit(bits[{BITPOS}], {FIELDINDEX}))
				value.{FIELDNAME} = stream.{READTYPE}();
{Split}
			if(NetConvertBase.GetBit(bits[{BITPOS}], {FIELDINDEX}))
			{
				var bind = new {BINDTYPE}();
				value.{FIELDNAME} = bind.Read(stream);
			}
{Split}
			if(NetConvertBase.GetBit(bits[{BITPOS}], {FIELDINDEX}))
			{
				var bind = new DictionaryBind<{KEYTYPE}, {VALUETYPE}>();
				value.{FIELDNAME} = bind.Read(stream, new {BINDTYPE}());
			}
{Split}
			return value;
		}

        public void WriteValue(object value, Segment stream)
        {
            Write(({TYPE})value, stream);
        }

        public object ReadValue(Segment stream)
        {
            return Read(stream);
        }
    }
}
";
        var typeName = type.FullName.Replace(".", "").Replace("+", "");
        var fullName = type.FullName;
        templateText = templateText.Replace("{TYPENAME}", typeName);
        templateText = templateText.Replace("{TYPE}", fullName);
        templateText = templateText.Replace("{SIZE}", $"{((members.Count - 1) / 8) + 1}");

        var templateTexts = templateText.Split(new string[] { "{Split}" }, 0);

        sb.Append(templateTexts[0]);

        for (int i = 0; i < members.Count; i++)
        {
            int bitInx1 = i % 8;
            int bitPos = i / 8;
            var typecode = Type.GetTypeCode(members[i].Type);
            if (typecode != TypeCode.Object)
            {
                var templateText1 = templateTexts[1];
                templateText1 = templateText1.Replace("{BITPOS}", $"{bitPos}");
                templateText1 = templateText1.Replace("{FIELDINDEX}", $"{++bitInx1}");
                templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                if (typecode == TypeCode.String)
                    templateText1 = templateText1.Replace("{Condition}", $"!string.IsNullOrEmpty(value.{members[i].Name})");
                else if (typecode == TypeCode.Boolean)
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != false");
                else if (typecode == TypeCode.DateTime)
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != default");
                else
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != 0");
                sb.Append(templateText1);

                var templateText2 = templateTexts[5];
                templateText2 = templateText2.Replace("{BITPOS}", $"{bitPos}");
                templateText2 = templateText2.Replace("{FIELDINDEX}", $"{bitInx1}");
                templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                if(members[i].IsEnum)
                    templateText2 = templateText2.Replace("{READTYPE}", $"ReadEnum<{members[i].Type.FullName.Replace("+", ".")}>");
                else
                    templateText2 = templateText2.Replace("{READTYPE}", $"Read{typecode}");
                sb1.Append(templateText2);
            }
            else if (members[i].IsArray)
            {
                typecode = Type.GetTypeCode(members[i].ItemType);
                if (typecode != TypeCode.Object)
                {
                    var templateText1 = templateTexts[1];
                    templateText1 = templateText1.Replace("{BITPOS}", $"{bitPos}");
                    templateText1 = templateText1.Replace("{FIELDINDEX}", $"{++bitInx1}");
                    templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != null");
                    sb.Append(templateText1);

                    var templateText2 = templateTexts[5];
                    templateText2 = templateText2.Replace("{BITPOS}", $"{bitPos}");
                    templateText2 = templateText2.Replace("{FIELDINDEX}", $"{bitInx1}");
                    templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                    templateText2 = templateText2.Replace("{READTYPE}", $"Read{typecode}Array");
                    sb1.Append(templateText2);
                }
                else
                {
                    var templateText1 = templateTexts[2];
                    templateText1 = templateText1.Replace("{BITPOS}", $"{bitPos}");
                    templateText1 = templateText1.Replace("{FIELDINDEX}", $"{++bitInx1}");
                    templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                    if (members[i].Type.IsValueType)
                        templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != default");
                    else
                        templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != null");
                    var local = members[i].ItemType.FullName.Replace(".", "").Replace("+", "") + "ArrayBind";
                    templateText1 = templateText1.Replace("{BINDTYPE}", $"{local}");
                    sb.Append(templateText1);

                    var templateText2 = templateTexts[6];
                    templateText2 = templateText2.Replace("{BITPOS}", $"{bitPos}");
                    templateText2 = templateText2.Replace("{FIELDINDEX}", $"{bitInx1}");
                    templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                    templateText2 = templateText2.Replace("{BINDTYPE}", $"{local}");
                    sb1.Append(templateText2);
                }
            }
            else if (members[i].IsGenericType)
            {
                if (members[i].ItemType1 == null)//List<T>
                {
                    typecode = Type.GetTypeCode(members[i].ItemType);
                    if (typecode != TypeCode.Object)
                    {
                        var templateText1 = templateTexts[1];
                        templateText1 = templateText1.Replace("{BITPOS}", $"{bitPos}");
                        templateText1 = templateText1.Replace("{FIELDINDEX}", $"{++bitInx1}");
                        templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                        templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != null");
                        sb.Append(templateText1);

                        var templateText2 = templateTexts[5];
                        templateText2 = templateText2.Replace("{BITPOS}", $"{bitPos}");
                        templateText2 = templateText2.Replace("{FIELDINDEX}", $"{bitInx1}");
                        templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                        templateText2 = templateText2.Replace("{READTYPE}", $"Read{typecode}List");
                        sb1.Append(templateText2);
                    }
                    else
                    {
                        var templateText1 = templateTexts[2];
                        templateText1 = templateText1.Replace("{BITPOS}", $"{bitPos}");
                        templateText1 = templateText1.Replace("{FIELDINDEX}", $"{++bitInx1}");
                        templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                        if (members[i].Type.IsValueType)
                            templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != default");
                        else
                            templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != null");
                        var local = members[i].ItemType.FullName.Replace(".", "").Replace("+", "") + "GenericBind";
                        templateText1 = templateText1.Replace("{BINDTYPE}", $"{local}");
                        sb.Append(templateText1);

                        var templateText2 = templateTexts[6];
                        templateText2 = templateText2.Replace("{BITPOS}", $"{bitPos}");
                        templateText2 = templateText2.Replace("{FIELDINDEX}", $"{bitInx1}");
                        templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                        templateText2 = templateText2.Replace("{BINDTYPE}", $"{local}");
                        sb1.Append(templateText2);
                    }
                }
                else //Dic
                {
                    var templateText1 = templateTexts[3];
                    templateText1 = templateText1.Replace("{BITPOS}", $"{bitPos}");
                    templateText1 = templateText1.Replace("{FIELDINDEX}", $"{++bitInx1}");
                    templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != null");

                    var key = members[i].ItemType.FullName;
                    string bindType;
                    string value;
                    if (members[i].ItemType1.IsArray)
                    {
                        var serType = members[i].ItemType1.GetInterface(typeof(IList<>).FullName);
                        var type1 = serType.GetGenericArguments()[0];
                        typecode = Type.GetTypeCode(type1);
                        if (typecode == TypeCode.Object)
                        {
                            value = $"{type1.FullName}[]";
                            bindType = type1.FullName.Replace(".", "").Replace("+", "") + "ArrayBind";
                        }
                        else
                        {
                            value = $"{type1.FullName}[]";
                            bindType = $"BaseArrayBind<{type1.FullName}>";
                        }
                    }
                    else if (members[i].ItemType1.IsGenericType)
                    {
                        var type1 = members[i].ItemType1.GenericTypeArguments[0];
                        typecode = Type.GetTypeCode(type1);
                        if (typecode == TypeCode.Object)
                        {
                            value = $"List<{type1.FullName}>";
                            bindType = type1.FullName.Replace(".", "").Replace("+", "") + "GenericBind";
                        }
                        else
                        {
                            value = $"List<{type1.FullName}>";
                            bindType = $"BaseListBind<{type1.FullName}>";
                        }
                    }
                    else
                    {
                        typecode = Type.GetTypeCode(members[i].ItemType1);
                        if (typecode == TypeCode.Object)
                        {
                            value = members[i].ItemType1.FullName.Replace("+", ".");
                            bindType = members[i].ItemType1.FullName.Replace(".", "").Replace("+", "") + "Bind";
                        }
                        else
                        {
                            value = members[i].ItemType1.FullName;
                            bindType = $"BaseBind<{value}>";
                        }
                    }
                    templateText1 = templateText1.Replace("{KEYTYPE}", $"{key}");
                    templateText1 = templateText1.Replace("{VALUETYPE}", $"{value}");
                    templateText1 = templateText1.Replace("{BINDTYPE}", $"{bindType}");
                    sb.Append(templateText1);

                    var templateText2 = templateTexts[7];
                    templateText2 = templateText2.Replace("{BITPOS}", $"{bitPos}");
                    templateText2 = templateText2.Replace("{FIELDINDEX}", $"{bitInx1}");
                    templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                    templateText2 = templateText2.Replace("{KEYTYPE}", $"{key}");
                    templateText2 = templateText2.Replace("{VALUETYPE}", $"{value}");
                    templateText2 = templateText2.Replace("{BINDTYPE}", $"{bindType}");
                    sb1.Append(templateText2);

                    var text = BuildDictionary(members[i].Type, out var className1);
                    File.WriteAllText(savePath + $"//{className1}.cs", text);
                }
            }
            else
            {
                var templateText1 = templateTexts[2];
                templateText1 = templateText1.Replace("{BITPOS}", $"{bitPos}");
                templateText1 = templateText1.Replace("{FIELDINDEX}", $"{++bitInx1}");
                templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");

                if(members[i].Type.IsValueType)
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != default");
                else
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != null");
                var local = members[i].Type.FullName.Replace(".", "").Replace("+", "") + "Bind";
                templateText1 = templateText1.Replace("{BINDTYPE}", $"{local}");
                sb.Append(templateText1);

                var templateText2 = templateTexts[6];
                templateText2 = templateText2.Replace("{BITPOS}", $"{bitPos}");
                templateText2 = templateText2.Replace("{FIELDINDEX}", $"{bitInx1}");
                templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                templateText2 = templateText2.Replace("{BINDTYPE}", $"{local}");
                sb1.Append(templateText2);
            }
        }

        sb.Append(templateTexts[4]);
        sb.Append(sb1.ToString());
        sb.Append(templateTexts[8]);

        return sb;
    }

    public static void BuildBindingType(List<Type> types, string savePath)
    {
        StringBuilder str = new StringBuilder();
        str.AppendLine("using System;");
        str.AppendLine("using System.Collections.Generic;");
        str.AppendLine("using Net.Serialize;");
        str.AppendLine("");
        str.AppendLine("namespace Binding");
        str.AppendLine("{");
        str.AppendLine("    public class BindingType : IBindingType");
        str.AppendLine("    {");
        str.AppendLine("        public Type[] TYPES { get; } = new Type[]");
        str.AppendLine("        {");
        foreach (var item in types)
        {
            if(item.IsGenericType & item.GenericTypeArguments.Length == 2)
                str.AppendLine($"\t\t\ttypeof(Dictionary<{item.GenericTypeArguments[0].FullName},{item.GenericTypeArguments[1].FullName}>),");
            else
                str.AppendLine($"\t\t\ttypeof({item.FullName}),");
        }
        str.AppendLine("        };");
        str.AppendLine("    }");
        str.AppendLine("}");
        File.WriteAllText(savePath + $"//BindingType.cs", str.ToString());
    }

    public static void BuildArray(Type type, string savePath)
    {
        var str = BuildArray(type);
        var className = type.FullName.Replace(".", "").Replace("+", "");
        File.AppendAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }

    public static StringBuilder BuildArray(Type type)
    {
        StringBuilder sb = new StringBuilder();
        var templateText = @"
namespace Binding
{
	public struct {TYPENAME}ArrayBind : ISerialize<{TYPE}[]>, ISerialize
	{
		public void Write({TYPE}[] value, Segment stream)
		{
			int count = value.Length;
			stream.Write(count);
			if (count == 0) return;
			var bind = new {BINDTYPE}();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public {TYPE}[] Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new {TYPE}[count];
			if (count == 0) return value;
			var bind = new {BINDTYPE}();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write(({TYPE}[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}";
        var typeName = type.FullName.Replace(".", "").Replace("+", "");
        var fullName = type.FullName;
        templateText = templateText.Replace("{TYPENAME}", typeName);
        templateText = templateText.Replace("{TYPE}", fullName);
        
        var local = typeName + "Bind";
        templateText = templateText.Replace("{BINDTYPE}", $"{local}");

        sb.Append(templateText);
        return sb;
    }

    public static void BuildGeneric(Type type, string savePath)
    {
        var str = BuildGeneric(type);
        var className = type.FullName.Replace(".", "").Replace("+", "");
        File.AppendAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }

    public static StringBuilder BuildGeneric(Type type)
    {
        StringBuilder sb = new StringBuilder();
        var templateText = @"
namespace Binding
{
	public struct {TYPENAME}GenericBind : ISerialize<List<{TYPE}>>, ISerialize
	{
		public void Write(List<{TYPE}> value, Segment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new {BINDTYPE}();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<{TYPE}> Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new List<{TYPE}>(count);
			if (count == 0) return value;
			var bind = new {BINDTYPE}();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<{TYPE}>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}";
        var typeName = type.FullName.Replace(".", "").Replace("+", "");
        var fullName = type.FullName;
        templateText = templateText.Replace("{TYPENAME}", typeName);
        templateText = templateText.Replace("{TYPE}", fullName);

        var local = typeName + "Bind";
        templateText = templateText.Replace("{BINDTYPE}", $"{local}");

        sb.Append(templateText);
        return sb;
    }

    public static string BuildDictionary(Type type, out string fileTypeName)
    {
        var text =
@"using Binding;
using Net.Serialize;
using Net.System;
using System.Collections.Generic;
public struct Dictionary_{TKeyName}_{TValueName}_Bind : ISerialize<Dictionary<{TKey}, {TValue}>>, ISerialize
{
    public void Write(Dictionary<{TKey}, {TValue}> value, Segment stream)
    {
        int count = value.Count;
        stream.Write(count);
        if (count == 0) return;
        foreach (var value1 in value)
        {
            stream.Write(value1.Key);
            var bind = new {BindTypeName}();
            bind.Write(value1.Value, stream);
        }
    }

    public Dictionary<{TKey}, {TValue}> Read(Segment stream)
    {
        var count = stream.ReadInt32();
        var value = new Dictionary<{TKey}, {TValue}>();
        if (count == 0) return value;
        for (int i = 0; i < count; i++)
        {
            var key = stream.Read{READ}();
            var bind = new {BindTypeName}();
            var value1 = bind.Read(stream);
            value.Add(key, value1);
        }
        return value;
    }

    public void WriteValue(object value, Segment stream)
    {
        Write((Dictionary<{TKey}, {TValue}>)value, stream);
    }

    public object ReadValue(Segment stream)
    {
        return Read(stream);
    }
}";
        TypeCode typecode;
        var args = type.GenericTypeArguments;
        string bindType;
        string value;
        string typeBindName;
        string keyRead;
        if (args[1].IsArray)
        {
            var serType = args[1].GetInterface(typeof(IList<>).FullName);
            var type1 = serType.GetGenericArguments()[0];
            typecode = Type.GetTypeCode(type1);
            if (typecode == TypeCode.Object)
            {
                value = $"{type1.FullName}[]";
                bindType = type1.FullName.Replace(".", "").Replace("+", "") + "ArrayBind";
                typeBindName = bindType;
            }
            else
            {
                value = $"{type1.FullName}[]";
                bindType = $"BaseArrayBind<{type1.FullName}>";
                typeBindName = type1.FullName.Replace(".", "").Replace("+", "") + "ArrayBind";
            }
            keyRead = $"{Type.GetTypeCode(args[0])}Array";
        }
        else if (args[1].IsGenericType)
        {
            var type1 = args[1].GenericTypeArguments[0];
            typecode = Type.GetTypeCode(type1);
            if (typecode == TypeCode.Object)
            {
                value = $"List<{type1.FullName}>";
                bindType = type1.FullName.Replace(".", "").Replace("+", "") + "GenericBind";
                typeBindName = bindType;
            }
            else
            {
                value = $"List<{type1.FullName}>";
                bindType = $"BaseListBind<{type1.FullName}>";
                typeBindName = type1.FullName.Replace(".", "").Replace("+", "") + "GenericBind";
            }
            keyRead = $"{Type.GetTypeCode(args[0])}List";
        }
        else
        {
            typecode = Type.GetTypeCode(args[1]);
            if (typecode == TypeCode.Object)
            {
                value = args[1].FullName.Replace("+", ".");
                bindType = args[1].FullName.Replace(".", "").Replace("+", "") + "Bind";
                typeBindName = bindType;
            }
            else
            {
                value = args[1].FullName;
                bindType = $"BaseBind<{value}>";
                typeBindName = value.Replace(".", "").Replace("+", "");
            }
            keyRead = $"{Type.GetTypeCode(args[0])}";
        }
        text = text.Replace("{TKeyName}", $"{args[0].FullName.Replace(".", "").Replace("+", "")}");
        text = text.Replace("{TValueName}", $"{typeBindName}");
        text = text.Replace("{TKey}", $"{args[0].FullName}");
        text = text.Replace("{TValue}", $"{value}");
        text = text.Replace("{BindTypeName}", $"{bindType}");
        text = text.Replace("{READ}", $"{keyRead}");
        fileTypeName = $"Dictionary_{args[0].FullName.Replace(".", "")}_{typeBindName}_Bind"; ;
        return text;
    }
}