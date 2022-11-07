using System;
using System.Data;
using System.Threading.Tasks;
using Net.System;
using System.Collections.Generic;
using System.Text;
#if SERVER
using System.Data.SQLite;
#endif

/// <summary>
/// 此类由MySqlDataBuild工具生成, 请不要在此类编辑代码! 请定义一个扩展类进行处理
/// MySqlDataBuild工具提供Rpc自动同步到mysql数据库的功能, 提供数据库注释功能
/// MySqlDataBuild工具gitee地址:https://gitee.com/leng_yue/my-sql-data-build
/// </summary>
public partial class ConfigData : IDataRow
{
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public DataRowState RowState { get; set; }
    private readonly HashSetSafe<int> columns = new HashSetSafe<int>();

    private System.Int64 id;
    /// <summary>{KEYNOTE}</summary>
    public System.Int64 Id
    {
        get { return id; }
        set { this.id = value; }
    }


    private System.String name;
    /// <summary>{NOTE}</summary>
    public System.String Name
    {
        get { return name; }
        set
        {
            if (this.name == value)
                return;
            if(value==null) value = string.Empty;
            this.name = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(1);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            Example2DB.I.Update(this);
#elif CLIENT
            NameCall();
#endif
        }
    }

    /// <summary>{NOTE1}</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncName
    {
        get { return name; }
        set
        {
            if (this.name == value)
                return;
            if(value==null) value = string.Empty;
            this.name = value;
            NameCall();
        }
    }

    /// <summary>{NOTE2}</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncIDName
    {
        get { return name; }
        set
        {
            if (this.name == value)
                return;
            if(value==null) value = string.Empty;
            this.name = value;
            SyncNameCall();
        }
    }

    /// <summary>{NOTE3}</summary>
    public void NameCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2HashProto.NAME, name);
    }

	/// <summary>{NOTE4}</summary>
    public void SyncNameCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2HashProto.NAME, id, name);
    }

    [Net.Share.Rpc(hash = (ushort)Example2HashProto.NAME)]
    private void NameCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(ConfigData));收集Rpc
    {
        Name = value;
        OnName?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnName;

    private System.Int64 number;
    /// <summary>{NOTE}</summary>
    public System.Int64 Number
    {
        get { return number; }
        set
        {
            if (this.number == value)
                return;
            
            this.number = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(2);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            Example2DB.I.Update(this);
#elif CLIENT
            NumberCall();
#endif
        }
    }

    /// <summary>{NOTE1}</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int64 SyncNumber
    {
        get { return number; }
        set
        {
            if (this.number == value)
                return;
            
            this.number = value;
            NumberCall();
        }
    }

    /// <summary>{NOTE2}</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int64 SyncIDNumber
    {
        get { return number; }
        set
        {
            if (this.number == value)
                return;
            
            this.number = value;
            SyncNumberCall();
        }
    }

    /// <summary>{NOTE3}</summary>
    public void NumberCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2HashProto.NUMBER, number);
    }

	/// <summary>{NOTE4}</summary>
    public void SyncNumberCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2HashProto.NUMBER, id, number);
    }

    [Net.Share.Rpc(hash = (ushort)Example2HashProto.NUMBER)]
    private void NumberCall(System.Int64 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(ConfigData));收集Rpc
    {
        Number = value;
        OnNumber?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnNumber;


    public ConfigData() { }

#if SERVER
    public ConfigData(params object[] parms)
    {
        NewTableRow(parms);
    }
    public void NewTableRow()
    {
        for (int i = 0; i < 3; i++)
        {
            var obj = this[i];
            if (obj == null)
                continue;
            var defaultVal = GetDefaultValue(obj.GetType());
            if (obj.Equals(defaultVal))
                continue;
            columns.Add(i);
        }
        RowState = DataRowState.Added;
        Example2DB.I.Update(this);
    }
    public object GetDefaultValue(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
    public void NewTableRow(params object[] parms)
    {
        if (parms == null)
            return;
        if (parms.Length == 0)
            return;
        for (int i = 0; i < parms.Length; i++)
        {
            this[i] = parms[i];
            columns.Add(i);
        }
        RowState = DataRowState.Added;
        Example2DB.I.Update(this);
    }
    public string GetCellName(int index)
    {
        switch (index)
        {

            case 0:
                return "id";

            case 1:
                return "name";

            case 2:
                return "number";

        }
        throw new Exception("错误");
    }
    public object this[int index]
    {
        get
        {
            switch (index)
            {

                case 0:
                    return this.id;

                case 1:
                    return this.name;

                case 2:
                    return this.number;

            }
            throw new Exception("错误");
        }
        set
        {
            switch (index)
            {

                case 0:
                    this.id = (System.Int64)value;
                    break;

                case 1:
                    this.name = (System.String)value;
                    break;

                case 2:
                    this.number = (System.Int64)value;
                    break;

            }
        }
    }

    public void Delete()
    {
        RowState = DataRowState.Detached;
        Example2DB.I.Update(this);
    }

    /// <summary>
    /// 查询1: Query("`id`=1");
    /// <para></para>
    /// 查询2: Query("`id`=1 and `index`=1");
    /// <para></para>
    /// 查询3: Query("`id`=1 or `index`=1");
    /// </summary>
    /// <param name="filterExpression"></param>
    /// <returns></returns>
    public static ConfigData Query(string filterExpression)
    {
        var cmdText = $"select * from config where {filterExpression}; ";
        var data = Example2DB.ExecuteQuery<ConfigData>(cmdText);
        return data;
    }
    /// <summary>
    /// 查询1: Query("`id`=1");
    /// <para></para>
    /// 查询2: Query("`id`=1 and `index`=1");
    /// <para></para>
    /// 查询3: Query("`id`=1 or `index`=1");
    /// </summary>
    /// <param name="filterExpression"></param>
    /// <returns></returns>
    public static async Task<ConfigData> QueryAsync(string filterExpression)
    {
        var cmdText = $"select * from config where {filterExpression}; ";
        var data = await Example2DB.ExecuteQueryAsync<ConfigData>(cmdText);
        return data;
    }
    public static ConfigData[] QueryList(string filterExpression)
    {
        var cmdText = $"select * from config where {filterExpression}; ";
        var datas = Example2DB.ExecuteQueryList<ConfigData>(cmdText);
        return datas;
    }
    public static async Task<ConfigData[]> QueryListAsync(string filterExpression)
    {
        var cmdText = $"select * from config where {filterExpression}; ";
        var datas = await Example2DB.ExecuteQueryListAsync<ConfigData>(cmdText);
        return datas;
    }
    public void Update()
    {
        if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached | RowState == DataRowState.Added | RowState == 0) return;

        for (int i = 0; i < 3; i++)
        {
            columns.Add(i);
        }
        RowState = DataRowState.Modified;
        Example2DB.I.Update(this);

    }
#endif

    public void Init(DataRow row)
    {
        RowState = DataRowState.Unchanged;

        if (row[0] is System.Int64 id)
            this.id = id;

        if (row[1] is System.String name)
            this.name = name;

        if (row[2] is System.Int64 number)
            this.number = number;

    }

    public void AddedSql(StringBuilder sb, List<IDbDataParameter> parms, ref int parmsLen)
    {
#if SERVER
        string cmdText = "REPLACE INTO config SET ";
        foreach (var column in columns)
        {
            var name = GetCellName(column);
            var value = this[column];
            if (value is string | value is DateTime)
                cmdText += $"`{name}`='{value}',";
            else if (value is byte[] buffer)
            {
                var count = parms.Count;
                cmdText += $"`{name}`=@buffer{count},";
                parms.Add(new SQLiteParameter($"@buffer{count}", buffer));
                parmsLen += buffer.Length;
            }
            else cmdText += $"`{name}`={value},";
            columns.Remove(column);
        }
        cmdText = cmdText.TrimEnd(',');
        cmdText += "; ";
        sb.Append(cmdText);
        RowState = DataRowState.Unchanged;
#endif
    }

    public void ModifiedSql(StringBuilder sb, List<IDbDataParameter> parms, ref int parmsLen)
    {
#if SERVER
        if (RowState == DataRowState.Detached | RowState == DataRowState.Deleted | RowState == DataRowState.Added | RowState == 0)
            return;
        string cmdText = $"UPDATE config SET ";
        foreach (var column in columns)
        {
            var name = GetCellName(column);
            var value = this[column];
            if (value is string | value is DateTime)
                cmdText += $"`{name}`='{value}',";
            else if (value is byte[] buffer)
            {
                var count = parms.Count;
                cmdText += $"`{name}`=@buffer{count},";
                parms.Add(new SQLiteParameter($"@buffer{count}", buffer));
                parmsLen += buffer.Length;
            }
            else cmdText += $"`{name}`={value},";
            columns.Remove(column);
        }
        cmdText = cmdText.TrimEnd(',');
        cmdText += $" WHERE `id`={id}; ";
        sb.Append(cmdText);
        RowState = DataRowState.Unchanged;
#endif
    }

    public void DeletedSql(StringBuilder sb)
    {
#if SERVER
        if (RowState == DataRowState.Deleted)
            return;
        string cmdText = $"DELETE FROM config WHERE `id` = {id}; ";
        sb.Append(cmdText);
        RowState = DataRowState.Deleted;
#endif
    }
}