#if SERVER
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SQLite;
using Net.Event;
using Net.System;
using System.Collections.Concurrent;

/// <summary>
/// Example2DB数据库管理类
/// 此类由MySqlDataBuild工具生成, 请不要在此类编辑代码! 请定义一个扩展类进行处理
/// MySqlDataBuild工具提供Rpc自动同步到mysql数据库的功能, 提供数据库注释功能
/// MySqlDataBuild工具gitee地址:https://gitee.com/leng_yue/my-sql-data-build
/// </summary>
public partial class Example2DB
{
    public static Example2DB I { get; private set; } = new Example2DB();
    private readonly HashSetSafe<IDataRow> dataRowHandler = new HashSetSafe<IDataRow>();
    private static readonly ConcurrentStack<SQLiteConnection> conns = new ConcurrentStack<SQLiteConnection>();
    public static string connStr = @"Data Source='D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db';";
    public bool DebugSqlBatch { get; set; }

    private static SQLiteConnection CheckConn(SQLiteConnection conn)
    {
        if (conn == null)
        {
            conn = new SQLiteConnection(connStr); //数据库连接
            conn.Open();
        }
        
        if (conn.State != ConnectionState.Open)
        {
            conn.Close();
            conn = new SQLiteConnection(connStr); //数据库连接
            conn.Open();
        }
        return conn;
    }

    public void Init(Action<List<object>> onInit, int connLen = 5)
    {
        InitConnection(connLen);
        List<object> list = new List<object>();

        var configTable = ExecuteReader($"SELECT * FROM config");
        foreach (DataRow row in configTable.Rows)
        {
            var data = new ConfigData();
            data.Init(row);
            list.Add(data);
        }

        var userinfoTable = ExecuteReader($"SELECT * FROM userinfo");
        foreach (DataRow row in userinfoTable.Rows)
        {
            var data = new UserinfoData();
            data.Init(row);
            list.Add(data);
        }

        onInit?.Invoke(list);
    }

    public void InitConnection(int connLen = 5)
    {
        while (conns.TryPop(out var conn))
        {
            conn.Close();
        }
        for (int i = 0; i < connLen; i++)
        {
            conns.Push(CheckConn(null));
        }
    }

    public static DataTableEntity ExecuteReader(string cmdText)
    {
        SQLiteConnection conn1;
        while (!conns.TryPop(out conn1))
        {
            Thread.Sleep(1);
        }
        var conn = CheckConn(conn1);
        var dt = new DataTableEntity();
        try
        {
            using (var cmd = new SQLiteCommand())
            {
                cmd.CommandText = cmdText;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                using (var sdr = cmd.ExecuteReader())
                {
                    dt.Load(sdr);
                }
            }
        }
        catch (Exception ex)
        {
            NDebug.LogError(cmdText + " 错误: " + ex);
        }
        finally
        {
            conns.Push(conn);
        }
        return dt;
    }

    public static async Task<DataTableEntity> ExecuteReaderAsync(string cmdText)
    {
        SQLiteConnection conn1;
        while (!conns.TryPop(out conn1))
        {
            Thread.Sleep(1);
        }
        var conn = CheckConn(conn1);
        var dt = new DataTableEntity();
        try
        {
            using (var cmd = new SQLiteCommand())
            {
                cmd.CommandText = cmdText;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                using (var sdr = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(sdr);
                }
            }
        }
        catch (Exception ex)
        {
            NDebug.LogError(cmdText + " 错误: " + ex);
        }
        finally
        {
            conns.Push(conn);
        }
        return dt;
    }

    /// <summary>
    /// 查询1: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id=1;
    /// <para></para>
    /// 查询2: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id=1 and `index`=1;
    /// <para></para>
    /// 查询3: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id=1 or `index`=1;
    /// <para></para>
    /// 查询4: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id in(1,2,3,4,5);
    /// <para></para>
    /// 查询5: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id not in(1,2,3,4,5);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText"></param>
    /// <returns></returns>
    public static T ExecuteQuery<T>(string cmdText) where T : IDataRow, new()
    {
        var array = ExecuteQueryList<T>(cmdText);
        if (array == null)
            return default;
        return array[0];
    }

    /// <summary>
    /// 查询1: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id=1;
    /// <para></para>
    /// 查询2: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id=1 and `index`=1;
    /// <para></para>
    /// 查询3: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id=1 or `index`=1;
    /// <para></para>
    /// 查询4: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id in(1,2,3,4,5);
    /// <para></para>
    /// 查询5: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id not in(1,2,3,4,5);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText"></param>
    /// <returns></returns>
    public static T[] ExecuteQueryList<T>(string cmdText) where T : IDataRow, new()
    {
        using (var dt = ExecuteReader(cmdText))
        {
            if (dt.Rows.Count == 0)
                return default;
            var datas = new T[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                datas[i] = new T();
                datas[i].Init(dt.Rows[i]);
            }
            return datas;
        }
    }

    /// <summary>
    /// 查询1: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id=1;
    /// <para></para>
    /// 查询2: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id=1 and `index`=1;
    /// <para></para>
    /// 查询3: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id=1 or `index`=1;
    /// <para></para>
    /// 查询4: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id in(1,2,3,4,5);
    /// <para></para>
    /// 查询5: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id not in(1,2,3,4,5);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText"></param>
    /// <returns></returns>
    public static async Task<T> ExecuteQueryAsync<T>(string cmdText) where T : IDataRow, new()
    {
        var array = await ExecuteQueryListAsync<T>(cmdText);
        if (array == null)
            return default;
        return array[0];
    }

    /// <summary>
    /// 查询1: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id=1;
    /// <para></para>
    /// 查询2: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id=1 and `index`=1;
    /// <para></para>
    /// 查询3: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id=1 or `index`=1;
    /// <para></para>
    /// 查询4: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id in(1,2,3,4,5);
    /// <para></para>
    /// 查询5: select * from D:\MMORPG\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db where id not in(1,2,3,4,5);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText"></param>
    /// <returns></returns>
    public static async Task<T[]> ExecuteQueryListAsync<T>(string cmdText) where T : IDataRow, new()
    {
        using (var dt = await ExecuteReaderAsync(cmdText))
        {
            if (dt.Rows.Count == 0)
                return default;
            var datas = new T[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                datas[i] = new T();
                datas[i].Init(dt.Rows[i]);
            }
            return datas;
        }
    }

    public static async Task<int> ExecuteNonQuery(string cmdText, List<IDbDataParameter> parameters)
    {
        SQLiteConnection conn1;
        while (!conns.TryPop(out conn1))
        {
            Thread.Sleep(1);
        }
        var conn = CheckConn(conn1);
        var pars = parameters.ToArray();
        return await Task.Run(() =>
        {
            var count = ExecuteNonQuery(conn, cmdText, pars);
            conns.Push(conn);
            return count;
        });
    }

    public static void ExecuteNonQuery(string cmdText, List<IDbDataParameter> parameters, Action<int, Stopwatch> onComplete)
    {
        SQLiteConnection conn1;
        while (!conns.TryPop(out conn1))
        {
            Thread.Sleep(1);
        }
        var conn = CheckConn(conn1);
        var pars = parameters.ToArray();
        if (I.DebugSqlBatch)
            NDebug.Log(cmdText);
        Task.Run(() =>
        {
            var stopwatch = Stopwatch.StartNew();
            var count = ExecuteNonQuery(conn, cmdText, pars);
            conns.Push(conn);
            stopwatch.Stop();
            onComplete(count, stopwatch);
        });
    }

    private static int ExecuteNonQuery(SQLiteConnection conn, string cmdText, IDbDataParameter[] parameters)
    {
        var transaction = conn.BeginTransaction();
        try
        {
            using (SQLiteCommand cmd = new SQLiteCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = cmdText;
                cmd.Connection = conn;
                cmd.CommandTimeout = 1200;
                cmd.Parameters.AddRange(parameters);
                int res = cmd.ExecuteNonQuery();
                transaction.Commit();
                return res;
            }
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            NDebug.LogError(cmdText + " 错误: " + ex);
        }
        return -1;
    }

    public void Update(IDataRow entity)//更新的行,列
    {
        dataRowHandler.Add(entity);
    }

    public bool Executed()//每秒调用一次, 需要自己调用此方法
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            var parms = new List<IDbDataParameter>();
            int count = 0, parmsLen = 0;
            foreach (var row in dataRowHandler)
            {
                switch (row.RowState)
                {
                    case DataRowState.Added:
                        {
                            row.AddedSql(sb, parms, ref parmsLen);
                        }
                        break;
                    case DataRowState.Detached:
                        {
                            row.DeletedSql(sb);
                        }
                        break;
                    case DataRowState.Modified:
                        {
                            row.ModifiedSql(sb, parms, ref parmsLen);
                        }
                        break;
                }
                if (sb.Length + parmsLen >= 2000000)
                {
                    ExecuteNonQuery(sb.ToString(), parms, (count1, stopwatch) =>
                    {
                        NDebug.Log($"sql批处理完成:{count1} 用时:{stopwatch.ElapsedMilliseconds}");
                    });
                    sb.Clear();
                    parms.Clear();
                    count = 0;
                    parmsLen = 0;
                }
                dataRowHandler.Remove(row);
            }
            if (sb.Length > 0)
            {
                ExecuteNonQuery(sb.ToString(), parms, (count1, stopwatch) =>
                {
                    if (count1 > 2000)
                        NDebug.Log($"sql批处理完成:{count1} 用时:{stopwatch.ElapsedMilliseconds}");
                });
            }
        }
        catch (Exception ex)
        {
            NDebug.LogError("SQL异常: " + ex);
        }
        return true;
    }
}
#endif