using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;

public interface IDataRow 
{
    DataRowState RowState { get; set; }

    void Init(DataRow row);

    void AddedSql(StringBuilder sb, List<IDbDataParameter> parms, ref int parmsLen);

    void ModifiedSql(StringBuilder sb, List<IDbDataParameter> parms, ref int parmsLen);

    void DeletedSql(StringBuilder sb);
}

public class DataColumnEntity
{
    public int referenceCount;
    public object value;
    public bool IsStringOrDateTime;
    public bool IsBuffer;
    public bool updateData;
}

public class DataRowEntity
{
    public DataRowState state;
    public string tableName;
    public IDataRow Row;
    public string columnName;
    public object columnValue;
    public string primaryKey;
    public object primaryValue;
    public ConcurrentDictionary<string, DataColumnEntity> columns = new ConcurrentDictionary<string, DataColumnEntity>();
    public int referenceCount;
    public bool updateData;

    public DataRowEntity() { }

    public DataRowEntity Update(string tableName, DataRowState state, string columnName, object columnValue, IDataRow row, string primaryKey, object primaryValue)
    {
        this.tableName = tableName;
        if(this.state != DataRowState.Added)
            this.state = state;
        this.Row = row;
        this.columnName = columnName;
        this.columnValue = columnValue;
        this.primaryKey = primaryKey;
        this.primaryValue = primaryValue;
        if (!columns.TryGetValue(columnName, out var column))
            columns.TryAdd(columnName, column = new DataColumnEntity() { IsBuffer = columnValue is byte[], IsStringOrDateTime = columnValue is string | columnValue is DateTime });;
        if (column.referenceCount < 100)
            column.referenceCount++;
        column.updateData = true;
        column.value = columnValue;
        if (referenceCount < 100)
            referenceCount++;
        updateData = true;
        return this;
    }
}

public class DataTableEntity : DataTable
{
    private readonly object syncRoot = new object(); 
    public new DataTableNewRowEventHandler TableNewRow;
    public DataRow AddRow(params object[] pars)
    {
        lock (syncRoot) 
        {
            var row = Rows.Add(pars);
            row.AcceptChanges();
            TableNewRow?.Invoke(this, new DataTableNewRowEventArgs(row));
            return row;
        }
    }

    public void DeleteRow(DataRow dataRow) 
    {
        lock (syncRoot)
        {
            if (dataRow.RowState == DataRowState.Deleted)
                return;
            dataRow.Delete();
            dataRow.AcceptChanges();
        }
    }

    public new DataRow[] Select(string filterExpression) 
    {
        lock (syncRoot)
        {
            var rows = base.Select(filterExpression);
            return rows;
        }
    }

    public DataRow GetRow(int i)
    {
        return Rows[i];
    }
}

public class DBEntity
{
    public string table;
    public DataRow row;
    public int index;
    public object value;
    public int id;
    public string name;
    public string idName;
    public int type;
    public object asynObj;
    public bool isComplete;
    public Action<DataRow[]> action;
}
