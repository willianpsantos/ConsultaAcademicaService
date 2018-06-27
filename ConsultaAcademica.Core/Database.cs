using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Reflection.Emit;

namespace ConsultaAcademica.Core
{
    public enum Sgbd
    {
        SqlServer,
        MySql
    }

    public enum ConnectionStringMode
    {
        Raw,
        AppSettings
    }
    
    public enum DatabaseCommandType
    {
        Text,
        StoredProcedureOrFunction
    }

    public enum DatabaseParameterDirection
    {        
        Input = 1,
        Output = 2,
        InputOutput = 3,
        ReturnValue = 6
    }

    public enum DatabaseDbType
    {
        Undefined = -1,
        AnsiString = 0,       
        Binary = 1,
        Byte = 2,
        Boolean = 3,
        Currency = 4,
        Date = 5,
        DateTime = 6,
        Decimal = 7,
        Double = 8,
        Guid = 9,
        Int16 = 10,
        Int32 = 11,
        Int64 = 12,
        Object = 13,
        SByte = 14,
        Single = 15,
        String = 16,
        Time = 17,
        UInt16 = 18,
        UInt32 = 19,
        UInt64 = 20,
        VarNumeric = 21,
        AnsiStringFixedLength = 22,
        StringFixedLength = 23,
        Xml = 25,
        DateTime2 = 26,
        DateTimeOffset = 27
    }

    public enum DatabaseParameterListMode
    {
        ToInsertStatements,
        ToUpdateStatements,
        OnlyList
    }

    public class DatabaseParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public DatabaseParameterDirection Direction { get; set; }

        public DatabaseDbType DbType { get; set; }
    }

    public class Database : IDisposable
    {
        #region Constantes

        private const string SQL_SERVER_PARAMETER_CHAR = "@";
        private const string MYSQL_PARAMETER_CHAR = "@";
        private const int CONNECTION_TIMEOUT = 600;

        private string[] TRUE_STRING = new string[] { "S", "s", "Y", "y", "TRUE", "true" };
        private string[] FALSE_STRING = new string[] { "N", "n", "FALSE", "false" };

        public const string CONNECTION_STRING_NAME_APP_SETTINGS = "database.connection.string.name";

        #endregion

        #region Propriedades

        private IDbConnection Connection { get; set; }

        private IDbTransaction Transaction { get; set; }

        protected string ConnectionString { get; set; }

        protected string DatabaseName { get; set; }

        protected Sgbd Sgbd { get; set; }

        protected CommandType CommandType { get; set; }

        protected string CommandText { get; set; }

        protected List<DatabaseParameter> Parameters { get; set; }

        public bool OpenConnectionBeforeExecuted { get; set; }

        public bool CloseConnectionAfterExecuted { get; set; }

        public bool UseTransaction { get; set; }

        public bool AutoCommit { get; set; }

        public bool UseDataReaderToGetData { get; set; }

        #endregion

        #region Construtores

        public Database()
        {
            Sgbd = Sgbd.SqlServer;
            ConnectionString = this.GetConnectionStringByConfig();
            Parameters = new List<DatabaseParameter>();

            OpenConnectionBeforeExecuted = true;
            CloseConnectionAfterExecuted = true;
            UseTransaction = false;
            AutoCommit = true;
            UseDataReaderToGetData = true;
        }

        public Database(string connectionString)
        {
            Sgbd = Sgbd.SqlServer;
            ConnectionString = connectionString;
            Parameters = new List<DatabaseParameter>();

            OpenConnectionBeforeExecuted = true;
            CloseConnectionAfterExecuted = true;
            UseTransaction = false;
            AutoCommit = true;
            UseDataReaderToGetData = true;
        }

        public Database(Sgbd sgbd)
        {
            Sgbd = sgbd;
            ConnectionString = this.GetConnectionStringByConfig();
            Parameters = new List<DatabaseParameter>();

            OpenConnectionBeforeExecuted = true;
            CloseConnectionAfterExecuted = true;
            UseTransaction = false;
            AutoCommit = true;
            UseDataReaderToGetData = true;
        }

        public Database(Sgbd sgbd, string connectionString)
        {
            Sgbd = sgbd;
            ConnectionString = connectionString;
            Parameters = new List<DatabaseParameter>();

            CloseConnectionAfterExecuted = true;
            OpenConnectionBeforeExecuted = true;
            UseTransaction = false;
            AutoCommit = true;
            UseDataReaderToGetData = true;
        }

        #endregion

        #region Métodos Protegidos

        protected string GetConnectionStringByConfig(string settings = CONNECTION_STRING_NAME_APP_SETTINGS)
        {
            string sgbd = "";

            switch(Sgbd)
            {
                case Sgbd.MySql:
                    sgbd = "mysql.";
                    break;
                case Sgbd.SqlServer:
                    sgbd = "sqlserver.";
                    break;
            }

            var name = ConfigurationManager.AppSettings[sgbd + settings];
            var connectionString = ConfigurationManager.ConnectionStrings[name].ConnectionString;            
            return connectionString;
        }

        protected void InitializeConnection()
        {   
            if(string.IsNullOrEmpty(ConnectionString) || string.IsNullOrWhiteSpace(ConnectionString))
            {
                ConnectionString = GetConnectionStringByConfig();
            }

            switch (Sgbd)
            {
                case Sgbd.SqlServer:
                    if(Connection != null && Connection is SqlConnection)
                    {
                        Connection.ConnectionString = ConnectionString;
                        return;
                    }

                    Connection = new SqlConnection(ConnectionString);
                    break;

                case Sgbd.MySql:
                    if (Connection != null && Connection is MySqlConnection)
                    {
                        Connection.ConnectionString = ConnectionString;
                        return;
                    }

                    Connection = new MySqlConnection(ConnectionString);
                    break;
            }
        }

        protected IDbCommand CreateCommand()
        {
            var command = Connection.CreateCommand();
            command.CommandType = CommandType;
            command.CommandText = CommandText;
            command.CommandTimeout = CONNECTION_TIMEOUT;

            if (Parameters.Count == 0)
            {
                return command;
            }

            foreach (var item in Parameters)
            {
                var parameter = command.CreateParameter();

                parameter.ParameterName = item.Name;
                parameter.Value = item.Value ?? DBNull.Value;
                parameter.Direction = (ParameterDirection)item.Direction;

                if (item.DbType != DatabaseDbType.Undefined)
                {
                    parameter.DbType = (DbType)item.DbType;
                }

                command.Parameters.Add(parameter);
            }

            return command;
        }

        protected IDbDataAdapter CreateDataAdapter(IDbCommand command)
        {
            switch(Sgbd)
            {
                case Sgbd.MySql:
                    return new MySqlDataAdapter( (MySqlCommand) command );
                case Sgbd.SqlServer:
                    return new SqlDataAdapter( (SqlCommand) command );
                default:
                    return null;
            }
        }

        protected bool VerifyIfParameterAdded(string name)
        {
            var paramChar = Sgbd == Sgbd.SqlServer
                              ? SQL_SERVER_PARAMETER_CHAR
                              : Sgbd == Sgbd.MySql
                                  ? MYSQL_PARAMETER_CHAR
                                  : string.Empty;

            var regex = $"^({paramChar})?({name})$";
            var count = Parameters.Where(x => Regex.IsMatch(x.Name, regex, RegexOptions.IgnoreCase)).Count();
            return count > 0;
        }

        protected void InternalAddParameter(DatabaseParameterDirection direction, string name, object value)
        {
            if (VerifyIfParameterAdded(name))
            {
                return;
            }

            var parameter = new DatabaseParameter()
            {
                Name = name,
                Value = value,
                Direction = direction,
                DbType = DatabaseDbType.Undefined
            };

            Parameters.Add(parameter);
        }

        protected void InternalAddParameter(DatabaseParameterDirection direction, string name, object value, DatabaseDbType dbType)
        {
            if (VerifyIfParameterAdded(name))
            {
                return;
            }

            var parameter = new DatabaseParameter()
            {
                Name = name,
                Value = value,
                Direction = direction,
                DbType = dbType
            };

            Parameters.Add(parameter);
        }

        protected IDictionary<string, object> InternalExecute()
        {
            InitializeConnection();

            var command = CreateCommand();

            try
            {
                if (OpenConnectionBeforeExecuted)
                {
                    OpenConnection();
                }

                if (UseTransaction)
                {
                    BeginTransaction();
                    command.Transaction = Transaction;
                }

                command.ExecuteNonQuery();
                var returns = new Dictionary<string, object>(command.Parameters.Count);

                if (UseTransaction && AutoCommit)
                {
                    Commit();
                }

                foreach (DbParameter item in command.Parameters)
                {
                    switch (item.Direction)
                    {
                        case ParameterDirection.InputOutput:
                        case ParameterDirection.Output:
                        case ParameterDirection.ReturnValue:
                            returns.Add(item.ParameterName, item.Value);
                            break;
                    }
                }

                return returns;
            }
            catch (Exception ex)
            {
                if (UseTransaction && AutoCommit)
                {
                    Rollback();
                }

                throw;
            }
            finally
            {
                if (CloseConnectionAfterExecuted)
                {
                    CloseConnection();
                    command.Dispose();
                }
            }
        }

        protected T InvokeParseMethod<T>(object valueToConvert)
        {
            if (valueToConvert == null)
            {
                return default(T);
            }

            string strValue = valueToConvert.ToString();

            if (string.IsNullOrEmpty(strValue) || string.IsNullOrWhiteSpace(strValue))
            {
                return default(T);
            }
            
            var type = typeof(T);

            var method = type.GetMethod(
                "Parse",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy,
                null,
                new Type[] { typeof(string) },
                new ParameterModifier[] { new ParameterModifier(1) }
            );

            var converted = method.Invoke(null, new object[] { valueToConvert.ToString() });
            return (T) converted;
        }
                
        
        protected object ParseValue(object value, Type typeToConvert)
        {
            if (typeToConvert == typeof(String))
            {
                return value.ToString();
            }

            if (typeToConvert == typeof(Char))
            {
                return Char.Parse(value.ToString());
            }

            if (typeToConvert == typeof(Boolean))
            {
                var valueType = value.GetType();

                if (valueType == typeof(Int16) ||
                     valueType == typeof(Int32) ||
                      valueType == typeof(Int64) ||
                       valueType == typeof(UInt16) ||
                        valueType == typeof(UInt32) ||
                         valueType == typeof(UInt64))
                {
                    return value.ToString().Equals("1");
                }

                if(TRUE_STRING.Contains(value.ToString()))
                {
                    return true;
                }

                if(value.GetType() == typeof(Boolean))
                {
                    return value;
                }

                return false;
            }

            if (typeToConvert == typeof(Byte))
            {
                return Byte.Parse(value.ToString());
            }

            if (typeToConvert == typeof(SByte))
            {
                return SByte.Parse(value.ToString());
            }

            if (typeToConvert == typeof(Int16))
            {
                return Int16.Parse(value.ToString());
            }

            if (typeToConvert == typeof(Int32))
            {
                return Int32.Parse(value.ToString());
            }

            if(typeToConvert == typeof(Int64))
            {
                return Int64.Parse(value.ToString());
            }

            if (typeToConvert == typeof(UInt16))
            {
                return UInt16.Parse(value.ToString());
            }

            if (typeToConvert == typeof(UInt32))
            {
                return UInt32.Parse(value.ToString());
            }

            if (typeToConvert == typeof(UInt64))
            {
                return UInt64.Parse(value.ToString());
            }
            
            if (typeToConvert == typeof(Double))
            {
                return Double.Parse(value.ToString());
            }

            if (typeToConvert == typeof(Decimal))
            {
                return Decimal.Parse(value.ToString());
            }
            
            if (typeToConvert == typeof(DateTime))
            {
                return DateTime.Parse(value.ToString());
            }

            return null;
        }

        protected object ConvertDataReader(DbDataReader reader, Type type, PropertyInfo[] properties)
        {
            if(properties == null)
            {
                properties = type.GetProperties();
            }

            var instance = type.Assembly.CreateInstance(type.FullName);

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes<Column>();

                if (attributes == null || attributes.Count() == 0)
                {
                    continue;
                }

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);

                    if (string.IsNullOrEmpty(columnName))
                    {
                        continue;
                    }

                    var columnAttribute = attributes.Where(
                                              x => x.Name.Equals(
                                                  columnName,
                                                  StringComparison.InvariantCultureIgnoreCase
                                              )
                                          ).FirstOrDefault();

                    if (columnAttribute == null)
                    {
                        continue;
                    }

                    var dbValue = default(object);

                    try
                    {
                        dbValue = reader.GetValue(i);
                    }
                    catch(Exception)
                    {
                        dbValue = null;
                    }

                    var propertyType = property.PropertyType;
                    var underlyingType = Nullable.GetUnderlyingType(propertyType);

                    if(propertyType.IsEnum)
                    {
                        var enumFields = propertyType.GetFields();
                        
                        foreach(var field in enumFields)
                        {
                            var enumCustomAttributes = field.GetCustomAttributes(typeof(EnumValueAttribute), false);

                            if(enumCustomAttributes == null || enumCustomAttributes.Count() == 0)
                            {
                                continue;
                            }

                            foreach(EnumValueAttribute attr in enumCustomAttributes)
                            {
                                if(attr.Value.Equals(dbValue))
                                {
                                    var rawValue = field.GetRawConstantValue();
                                    property.SetValue(instance, rawValue);
                                }
                            }
                        }

                        continue;
                    }

                    object value = null;

                    if (dbValue == DBNull.Value || dbValue == null)
                    {
                        if (underlyingType != null && underlyingType.Equals(typeof(String)))
                        {
                            value = "";
                        }
                        else if (propertyType.Equals(typeof(String)))
                        {
                            value = "";
                        }
                        else
                        {
                            continue;
                        }
                    }

                    

                    if (underlyingType != null)
                    {
                        value = ParseValue(dbValue, underlyingType);
                    }
                    else
                    {
                        value = ParseValue(dbValue, propertyType);
                    }

                    property.SetValue(instance, value);
                }
            }

            return instance;
        }

        protected object ConvertDataRow(DataRow row, Type type, PropertyInfo[] properties)
        {
            if (properties == null)
            {
                properties = type.GetProperties();
            }

            var instance = type.Assembly.CreateInstance(type.FullName);

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes<Column>();

                if (attributes == null || attributes.Count() == 0)
                {
                    continue;
                }

                for (var i = 0; i < row.Table.Columns.Count; i++)
                {
                    var columnName = row.Table.Columns[i].ColumnName;

                    if (string.IsNullOrEmpty(columnName))
                    {
                        continue;
                    }

                    var columnAttribute = attributes.Where(
                                              x => x.Name.Equals(
                                                  columnName,
                                                  StringComparison.InvariantCultureIgnoreCase
                                              )
                                          ).FirstOrDefault();

                    if (columnAttribute == null)
                    {
                        continue;
                    }

                    var dbValue = default(object);

                    try
                    {
                        dbValue = row[columnName];
                    }
                    catch (Exception ex)
                    {
                        dbValue = null;
                    }

                    var propertyType = property.PropertyType;
                    var underlyingType = Nullable.GetUnderlyingType(propertyType);

                    if (propertyType.IsEnum)
                    {
                        var enumFields = propertyType.GetFields();

                        foreach (var field in enumFields)
                        {
                            var enumCustomAttributes = field.GetCustomAttributes(typeof(EnumValueAttribute), false);

                            if (enumCustomAttributes == null || enumCustomAttributes.Count() == 0)
                            {
                                continue;
                            }

                            foreach (EnumValueAttribute attr in enumCustomAttributes)
                            {
                                if (attr.Value.Equals(dbValue))
                                {
                                    var rawValue = field.GetRawConstantValue();
                                    property.SetValue(instance, rawValue);
                                }
                            }
                        }

                        continue;
                    }

                    if (dbValue == DBNull.Value || dbValue == null)
                    {
                        continue;
                    }

                    object value = null;

                    if (underlyingType != null)
                    {
                        value = ParseValue(dbValue, underlyingType);
                    }
                    else
                    {
                        value = ParseValue(dbValue, propertyType);
                    }

                    property.SetValue(instance, value);
                }
            }

            return instance;
        }

        protected List<T> GetListUsingDataReader<T>(IDbCommand command)
        {
            var reader = (DbDataReader)command.ExecuteReader();

            if (!reader.HasRows)
            {
                return null;
            }

            var type = typeof(T);
            var list = new List<T>();
            var properties = type.GetProperties();

            while (reader.Read())
            {
                var instance = ConvertDataReader(reader, type, properties);
                list.Add((T)instance);
            }

            reader.Close();
            return list;
        }

        protected List<T> GetListUsingDataSet<T>(IDbCommand command)
        {
            var dataAdapter = CreateDataAdapter(command);
            var dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            var type = typeof(T);
            var list = new List<T>();
            var properties = type.GetProperties();

            if(dataSet.Tables == null || dataSet.Tables.Count == 0)
            {
                return new List<T>();
            }

            var table = dataSet.Tables[0];

            if(table.Rows == null || table.Rows.Count == 0)
            {
                return new List<T>();
            }

            foreach(DataRow row in table.Rows)
            {
                var instance = ConvertDataRow(row, type, properties);
                list.Add((T)instance);
            }

            table.Dispose();
            dataSet.Dispose();
            return list;
        }


        protected T InternalScalarValue<T>()
        {
            InitializeConnection();

            var command = CreateCommand();

            try
            {
                if (OpenConnectionBeforeExecuted)
                {
                    OpenConnection();
                }

                object value = command.ExecuteScalar();
                var type = typeof(T);
                var underlyingType = Nullable.GetUnderlyingType(type);

                if (value == null || value == DBNull.Value)
                {
                    return default(T);
                }

                return (underlyingType == null)
                         ? (T)ParseValue(value, type)
                         : (T)ParseValue(value, underlyingType);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (CloseConnectionAfterExecuted)
                {
                    CloseConnection();
                    command.Dispose();
                }
            }
        }

        protected List<T> InternalToList<T>()
        {
            InitializeConnection();

            var command = CreateCommand();

            try
            {
                if (OpenConnectionBeforeExecuted)
                {
                    OpenConnection();
                }

                if(UseDataReaderToGetData)
                {
                    return GetListUsingDataReader<T>(command);
                }
                else
                {
                    return GetListUsingDataSet<T>(command);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (CloseConnectionAfterExecuted)
                {
                    CloseConnection();
                    command.Dispose();
                }
            }
        }

        protected T[] InternalToArray<T>()
        {
            var list = ToList<T>();

            if(list == null)
            {
                return new T[] { };
            }

            return list.ToArray();
        }

        protected T InternalSingle<T>()
        {
            InitializeConnection();

            var command = CreateCommand();

            try
            {
                if (OpenConnectionBeforeExecuted)
                {
                    OpenConnection();
                }

                var reader = (DbDataReader)command.ExecuteReader();

                if (!reader.HasRows)
                {
                    return default(T);
                }

                var type = typeof(T);                
                var properties = type.GetProperties();

                if(!reader.Read())
                {
                    return default(T);
                }

                var instance = ConvertDataReader(reader, type, properties);
                reader.Close();
                return (T)instance;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (CloseConnectionAfterExecuted)
                {
                    CloseConnection();
                    command.Dispose();
                }
            }
        }

        protected DataSet ToDataSet()
        {
            InitializeConnection();

            var command = CreateCommand();

            try
            {
                if (OpenConnectionBeforeExecuted)
                {
                    OpenConnection();
                }

                var dataAdapter = CreateDataAdapter(command);
                var dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                return dataSet;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (CloseConnectionAfterExecuted)
                {
                    CloseConnection();
                    command.Dispose();
                }
            }
        }

        protected string GetColumn(Type type, string name, string table = "")
        {
            var property = type.GetProperty(name);

            if (property == null)
            {
                return name;
            }

            var attributes = property.GetCustomAttributes<Column>();

            if (attributes == null || attributes.Count() == 0)
            {
                return "";
            }

            Column attribute = null;

            if (string.IsNullOrEmpty(table))
            {
                attribute = attributes.FirstOrDefault();
                return attribute?.Name;
            }

            attribute = attributes.Where(x => x.Table.Equals(table, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            return attribute?.Name;
        }

        protected void AddFilters(Type type, IDictionary<string, object> filters)
        {
            var tuple = new Tuple<string, string, object>("aluno", "=", 1);

            string sql = " WHERE ";
            var first = filters.First();

            var columnName = GetColumn(type, first.Key);
            var paramName = NormalizeParameterName(columnName);
            var value = first.Value;
            var valueType = value.GetType();
            KeyValuePair<string, object> keyPair;

            if (valueType == typeof(string))
            {
                sql += $"{columnName} = {paramName} ";
                AddParameterInput(paramName, value);
            }
            else if(valueType == typeof(KeyValuePair<string , object>))
            {
                keyPair = (KeyValuePair<string, object>)value;
                sql += $"{columnName} {keyPair.Key} {paramName}";
                AddParameterInput(paramName, keyPair.Value);
            }
            
            foreach (var item in filters)
            {
                if(item.Key == first.Key)
                {
                    continue;
                }

                value = item.Value;
                valueType = value.GetType();
                columnName = GetColumn(type, item.Key);
                paramName = NormalizeParameterName(columnName);
                
                if (valueType == typeof(string))
                {
                    sql += $"{columnName} = {paramName} ";
                    AddParameterInput(paramName, value);
                }
                else if (valueType == typeof(KeyValuePair<string, object>))
                {
                    keyPair = (KeyValuePair<string, object>)value;
                    sql += $"{columnName} {keyPair.Key} {paramName}";
                    AddParameterInput(paramName, keyPair.Value);
                }
            }

            CommandText += sql;
        }

        protected void AddFilters<T>(IDictionary<string, object> filters)
        {
            var type = typeof(T);
            AddFilters(type, filters);
        }
                
        #endregion

        #region Métodos Públicos

        public Database SetConnectionString(string connectionStringOrSettingName, ConnectionStringMode mode = ConnectionStringMode.AppSettings)
        {
            switch(mode)
            {
                case ConnectionStringMode.Raw:
                    ConnectionString = connectionStringOrSettingName;
                    break;

                case ConnectionStringMode.AppSettings:
                    ConnectionString = GetConnectionStringByConfig(connectionStringOrSettingName);
                    break;
            }

            return this;
        }

        public Database SetConnection(IDbConnection connection)
        {
            Connection = connection;
            return this;
        }

        public Database SetTransaction(IDbTransaction transaction)
        {
            Transaction = transaction;
            return this;
        }

        public Database SetSgbd(Sgbd sgbd)
        {
            Sgbd = sgbd;
            ConnectionString = GetConnectionStringByConfig();
            Connection?.Dispose();

            return this;
        }

        public Database SetCommandType(DatabaseCommandType type)
        {
            switch(type)
            {
                case DatabaseCommandType.Text:
                    CommandType = CommandType.Text;
                    break;

                case DatabaseCommandType.StoredProcedureOrFunction:
                    CommandType = CommandType.StoredProcedure;
                    break;
            }

            return this;
        }

        public Database SetCommand(string command)
        {
            CommandText = command;
            return this;
        }

        public Database AddParameterInput(string name, object value)
        {
            InternalAddParameter(DatabaseParameterDirection.Input, name, value);
            return this;
        }

        public Database AddParameterInput(string name, object value, DatabaseDbType dbType)
        {
            InternalAddParameter(DatabaseParameterDirection.Input, name, value, dbType);
            return this;
        }

        public Database AddParameterOutput(string name)
        {
            InternalAddParameter(DatabaseParameterDirection.Output, name, null);
            return this;
        }

        public Database AddParameterOutput(string name, DatabaseDbType dbType)
        {
            InternalAddParameter(DatabaseParameterDirection.Output, name, null, dbType);
            return this;
        }

        public Database AddParameterInputOutput(string name, object value)
        {
            InternalAddParameter(DatabaseParameterDirection.InputOutput, name, value);
            return this;
        }

        public Database AddParameterInputOutput(string name, object value, DatabaseDbType dbType)
        {
            InternalAddParameter(DatabaseParameterDirection.InputOutput, name, value, dbType);
            return this;
        }

        public Database AddParameterReturnValue(string name)
        {
            InternalAddParameter(DatabaseParameterDirection.ReturnValue, name, null);
            return this;
        }

        public Database AddParameterReturnValue(string name, DatabaseDbType dbType)
        {
            InternalAddParameter(DatabaseParameterDirection.ReturnValue, name, null, dbType);
            return this;
        }

        public Database AddParameter(DatabaseParameter parameter)
        {
            if (VerifyIfParameterAdded(parameter.Name))
            {
                return this;
            }

            Parameters.Add(parameter);
            return this;
        }

        public Database AddParameters<T>(T entity, DatabaseParameterListMode mode, string table = "")
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            var columns = new List<String>();

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes<Column>();

                if (attributes == null || attributes.Count() == 0)
                {
                    continue;
                }

                Column attribute = null;

                if (string.IsNullOrEmpty(table))
                {
                    attribute = attributes.FirstOrDefault();
                }
                else
                {
                    attribute = attributes.Where(x => x.Table.Equals(table, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                }

                if (attribute == null)
                {
                    continue;
                }

                if (mode == DatabaseParameterListMode.ToInsertStatements && !attribute.Insert)
                {
                    continue;
                }

                if (mode == DatabaseParameterListMode.ToUpdateStatements && !attribute.Update)
                {
                    continue;
                }

                AddParameterInput(NormalizeParameterName(attribute.Name), property.GetValue(entity));
            }

            return this;
        }

        public string NormalizeParameterName(string param)
        {
            var paramChar = Sgbd == Sgbd.SqlServer
                              ? SQL_SERVER_PARAMETER_CHAR
                              : Sgbd == Sgbd.MySql
                                  ? MYSQL_PARAMETER_CHAR
                                  : string.Empty;

            if (param.StartsWith(paramChar))
            {
                return param;
            }

            return $"{paramChar}{param}";
        }

        public Database OpenConnection()
        {
            if(Connection == null)
            {
                InitializeConnection();
            }

            if(Connection.State == ConnectionState.Open)
            {
                return this;
            }

            Connection.Open();
            return this;
        }

        public Database CloseConnection()
        {
            if(Connection == null)
            {
                return this;
            }

            if(Connection.State == ConnectionState.Closed)
            {
                return this;
            }

            Connection.Close();
            return this;
        }

        public Database BeginTransaction()
        {
            if(Connection == null)
            {
                InitializeConnection();
            }

            if(Connection.State == ConnectionState.Closed)
            {
                return this;
            }

            switch(Sgbd)
            {
                case Sgbd.MySql:
                    if(Transaction != null && Transaction is MySqlTransaction)
                    {
                        return this;
                    }
                    break;

                case Sgbd.SqlServer:
                    if (Transaction != null && Transaction is SqlTransaction)
                    {
                        return this;
                    }
                    break;
            }

            Transaction = Connection.BeginTransaction();
            return this;
        }

        public Database Commit()
        {            
            Transaction?.Commit();
            Transaction?.Dispose();
            Transaction = null;

            return this;
        }

        public Database Rollback()
        {
            Transaction?.Rollback();
            Transaction?.Dispose();
            Transaction = null;

            return this;
        }

        public Database ClearParameters()
        {
            Parameters.Clear();
            System.GC.Collect();

            return this;
        }

        public IDictionary<string, object> Execute()
        {
            UseTransaction = false;
            AutoCommit = true;
            return InternalExecute();
        }

        public IDictionary<string, object> Execute(bool useTransaction)
        {
            UseTransaction = useTransaction;
            AutoCommit = true;
            return InternalExecute();
        }

        public IDictionary<string, object> Execute(bool useTransaction, bool autoCommit)
        {
            UseTransaction = useTransaction;
            AutoCommit = autoCommit;
            return InternalExecute();
        }

        public T ScalarValue<T>()
        {
            return InternalScalarValue<T>();
        }

        public List<T> ToList<T>()
        {
            return InternalToList<T>();
        }

        public T[] ToArray<T>()
        {
            return InternalToArray<T>();
        }

        public T Single<T>()
        {
            return InternalSingle<T>();
        }

        public string[] GetColumns<T>(DatabaseParameterListMode mode, string table = "")
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            var columns = new List<String>();

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes<Column>();

                if (attributes == null || attributes.Count() == 0)
                {
                    continue;
                }

                Column attribute = null;

                if (string.IsNullOrEmpty(table))
                {
                    attribute = attributes.FirstOrDefault();
                }
                else
                {
                    attribute = attributes.Where(x => x.Table.Equals(table, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                }

                if (attribute == null)
                {
                    continue;
                }

                if (mode == DatabaseParameterListMode.ToInsertStatements && !attribute.Insert)
                {
                    continue;
                }

                if (mode == DatabaseParameterListMode.ToUpdateStatements && !attribute.Update)
                {
                    continue;
                }

                columns.Add(attribute.Name);
            }

            return columns.ToArray();
        }

        public string[] GetParameters<T>(DatabaseParameterListMode mode, string table = "")
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            var columns = new List<String>();

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes<Column>();

                if (attributes == null || attributes.Count() == 0)
                {
                    continue;
                }

                Column attribute = null;

                if (string.IsNullOrEmpty(table))
                {
                    attribute = attributes.FirstOrDefault();
                }
                else
                {
                    attribute = attributes.Where(x => x.Table.Equals(table, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                }

                if (attribute == null)
                {
                    continue;
                }

                if (mode == DatabaseParameterListMode.ToInsertStatements && !attribute.Insert)
                {
                    continue;
                }

                if (mode == DatabaseParameterListMode.ToUpdateStatements && !attribute.Update)
                {
                    continue;
                }

                string column = "";

                switch(mode)
                {
                    case DatabaseParameterListMode.ToInsertStatements:
                    case DatabaseParameterListMode.OnlyList:
                        column = NormalizeParameterName(attribute.Name);
                        break;
                    case DatabaseParameterListMode.ToUpdateStatements:
                        column = $"{attribute.Name} = {NormalizeParameterName(attribute.Name)}";
                        break;
                }

                columns.Add(column);
            }

            return columns.ToArray();
        }

        public void Dispose()
        {
            CloseConnection();
            Connection?.Dispose();
            Transaction?.Dispose();
        }

        #endregion
    }
}
