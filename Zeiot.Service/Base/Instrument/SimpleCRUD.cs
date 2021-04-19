using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using Dapper;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Zeiot.Service.Base.Instrument
{
    /// <summary>
    /// Main class for Dapper.SimpleCRUD extensions
    /// </summary>
    public static partial class SimpleCRUD
    {

        static SimpleCRUD()
        {
            SetDialect(_dialect);
        }
        /// <summary>
        /// 数据库地址
        /// </summary>
        public  static string sqlconnct = "";
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static int sqltype = 0;

        private static Dialect _dialect = Dialect.SQLServer;
        private static string _encapsulation;
        private static string _getIdentitySql;
        private static string _getPagedListSql;

        private static readonly ConcurrentDictionary<Type, string> TableNames = new ConcurrentDictionary<Type, string>();
        private static readonly ConcurrentDictionary<string, string> ColumnNames = new ConcurrentDictionary<string, string>();

        private static ITableNameResolver _tableNameResolver = new TableNameResolver();
        private static IColumnNameResolver _columnNameResolver = new ColumnNameResolver();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqltype">0 sql server  1 mysql</param>
        /// <param name="ConnectionString"></param>
        public static void SetConnectionString(string ConnectionString, int _sqltype )
        {

            sqltype = _sqltype;
            if (sqltype == 1)
            {
                _dialect = Dialect.MySQL;
                SetDialect(_dialect);
            }


            if (!string.IsNullOrEmpty(ConnectionString))
                sqlconnct = ConnectionString;
            else
            {
                if (sqltype == 0)
                {
                    sqlconnct = ConfigurationManager.ConnectionStrings["SqlServerConnection"].ConnectionString;
                }
                else
                {
                    sqlconnct = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
                }
            }


        }
        /// <summary>
        /// 返回当前数据库类型
        /// </summary>
        /// <returns></returns>
        public static string GetDialect()
        {
            return _dialect.ToString();
        }

        /// <summary>
        /// 设置数据库类型
        /// </summary>
        /// <param name="dialect"></param>
        public static void SetDialect(Dialect dialect)
        {
            switch (dialect)
            {
                case Dialect.PostgreSQL:
                    _dialect = Dialect.PostgreSQL;
                    _encapsulation = "\"{0}\"";
                    _getIdentitySql = string.Format("SELECT LASTVAL() AS id");
                    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({PageNumber}-1) * {RowsPerPage})";
                    break;

                case Dialect.MySQL:
                    _dialect = Dialect.MySQL;
                    _encapsulation = "{0}";
                    _getIdentitySql = string.Format("SELECT LAST_INSERT_ID() AS id");
                    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {Offset},{RowsPerPage}";
                    break;
                default:
                    _dialect = Dialect.SQLServer;
                    _encapsulation = "[{0}]";
                    _getIdentitySql = string.Format("SELECT CAST(SCOPE_IDENTITY()  AS BIGINT) AS [id]");
                    _getPagedListSql = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {OrderBy}) AS PagedNumber, {SelectColumns} FROM {TableName} {WhereClause}) AS u WHERE PagedNUMBER BETWEEN (({PageNumber}-1) * {RowsPerPage} + 1) AND ({PageNumber} * {RowsPerPage})";
                    break;
            }
        }




        /// <summary>
        /// 设置表名解析
        /// </summary>
        /// <param name="resolver">The resolver to use when requesting the format of a table name</param>
        public static void SetTableNameResolver(ITableNameResolver resolver)
        {
            _tableNameResolver = resolver;
        }

        /// <summary>
        /// 设置列名解析
        /// </summary>
        /// <param name="resolver">The resolver to use when requesting the format of a column name</param>
        public static void SetColumnNameResolver(IColumnNameResolver resolver)
        {
            _columnNameResolver = resolver;
        }
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Returns a single entity by a single id from table T.</returns>
        public static T Get<T>(this IDbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any() || idProps.Count != 1)
                throw new ArgumentException("Get<T> only supports an entity with a [Key] or Id property");

            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            sb.Append("Select ");
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            sb.AppendFormat(" from {0} where ", name);

            sb.AppendFormat("{0} = @{1}", GetColumnName(idProps[0]), idProps[0].Name);

            var dynParms = new DynamicParameters();
            if (idProps.First().PropertyType == typeof(int))
                dynParms.Add("@" + idProps.First().Name, int.Parse(id.ToString()));
            else
                dynParms.Add("@" + idProps.First().Name, id);

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("Get<{0}>: {1} with Id: {2}", currenttype, sb, id));
            return connection.Query<T>(sb.ToString(), dynParms, transaction, true, commandTimeout).FirstOrDefault();
        }
        /// <summary>
        /// <parameters>By default queries the table matching the class name</parameters>
        /// <parameters>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</parameters>
        /// <parameters>conditions is an SQL where clause and/or order by clause ex: "where name='bob'" or "where age>=@Age"</parameters>
        /// <parameters>parameters is an anonymous type to pass in named parameter values: new { Age = 15 }</parameters>
        /// <parameters>Supports transaction and command timeout</parameters>
        /// <parameters>Returns a list of entities that match where conditions</parameters>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Gets a list of entities with optional SQL where conditions</returns>
        public static IEnumerable<T> GetList<T>(this IDbConnection connection, string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.Append("Select ");
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            sb.AppendFormat(" from {0}", name);

            if (!string.IsNullOrEmpty(conditions))
            {
                sb.Append(" where ");
                conditions = ReplaceFist(conditions, "where");
                sb.Append(conditions.Replace("--", "——"));
            }
            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("GetList<{0}>: {1}", currenttype, sb));

            return connection.Query<T>(sb.ToString(), parameters, transaction, true, commandTimeout);
        }


        /// <summary>
        /// <parameters>By default queries the table matching the class name</parameters>
        /// <parameters>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</parameters>
        /// <parameters>conditions is an SQL where clause ex: "where name='bob'" or "where age>=@Age" - not required </parameters>
        /// <parameters>orderby is a column or list of columns to order by ex: "lastname, age desc" - not required - default is by primary key</parameters>
        /// <parameters>parameters is an anonymous type to pass in named parameter values: new { Age = 15 }</parameters>
        /// <parameters>Supports transaction and command timeout</parameters>
        /// <parameters>Returns a list of entities that match where conditions</parameters>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="pageNumber"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="conditions"></param>
        /// <param name="orderby"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Gets a paged list of entities with optional exact match where conditions</returns>
        public static IEnumerable<T> GetListPaged<T>(this IDbConnection connection, int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (string.IsNullOrEmpty(_getPagedListSql))
                throw new Exception("GetListPage is not supported with the current SQL Dialect");

            if (pageNumber < 1)
                throw new Exception("Page must be greater than 0");

            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();
            //if (!idProps.Any())
            //    throw new ArgumentException("Entity must have at least one [Key] property");

            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            var query = _getPagedListSql;
            if (string.IsNullOrEmpty(orderby)&& idProps.Count>0)
            {
                orderby = GetColumnName(idProps.First());
            }

            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            query = query.Replace("{SelectColumns}", sb.ToString());
            query = query.Replace("{TableName}", name);
            query = query.Replace("{PageNumber}", pageNumber.ToString());
            query = query.Replace("{RowsPerPage}", rowsPerPage.ToString());
            query = query.Replace("{OrderBy}", orderby);
            query = query.Replace("{WhereClause}", conditions);
            query = query.Replace("{Offset}", ((pageNumber - 1) * rowsPerPage).ToString());

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("GetListPaged<{0}>: {1}", currenttype, query));

            return connection.Query<T>(query, parameters, transaction, true, commandTimeout);
        }



  

        /// <summary>
        /// <parameters>By default queries the table matching the class name</parameters>
        /// <parameters>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</parameters>
        /// <parameters>Returns a number of records entity by a single id from table T</parameters>
        /// <parameters>Supports transaction and command timeout</parameters>
        /// <parameters>conditions is an SQL where clause ex: "where name='bob'" or "where age>=@Age" - not required </parameters>
        /// <parameters>parameters is an anonymous type to pass in named parameter values: new { Age = 15 }</parameters>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Returns a count of records.</returns>
        public static int RecordCount<T>(this IDbConnection connection, string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            sb.Append("Select count(1)");
            sb.AppendFormat(" from {0}", name);
            if (!string.IsNullOrEmpty(conditions))
            {
                sb.Append(" where ");
                conditions = ReplaceFist(conditions, "where");
                sb.Append(conditions.Replace("--", "——"));
            }
            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("RecordCount<{0}>: {1}", currenttype, sb));

            return connection.ExecuteScalar<int>(sb.ToString(), parameters, transaction, commandTimeout);
        }

        /// <summary>
        /// <parameters>By default queries the table matching the class name</parameters>
        /// <parameters>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</parameters>
        /// <parameters>Returns a number of records entity by a single id from table T</parameters>
        /// <parameters>Supports transaction and command timeout</parameters>
        /// <parameters>conditions is an SQL where clause ex: "where name='bob'" or "where age>=@Age" - not required </parameters>
        /// <parameters>parameters is an anonymous type to pass in named parameter values: new { Age = 15 }</parameters>
        /// </summary>       
        /// <param name="connection"></param>          
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Returns a count of records.</returns>
        public static int RecordCount(this IDbConnection connection, string sql = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
           
            var sb = new StringBuilder();
            sb.Append("Select count(1)");
            sb.AppendFormat(" from ({0}) as totalTable", sql);              

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("RecordCount<{0}>: {1}", sql, sb));

            return connection.ExecuteScalar<int>(sb.ToString(), parameters, transaction, commandTimeout);
        }
   
        //build update statement based on list on an entity
        private static void BuildUpdateSet<T>(T entityToUpdate, StringBuilder sb)
        {
            var nonIdProps = GetUpdateableProperties(entityToUpdate).ToArray();

            for (var i = 0; i < nonIdProps.Length; i++)
            {
                var property = nonIdProps[i];

                sb.AppendFormat("{0} = @{1}", GetColumnName(property), property.Name);
                if (i < nonIdProps.Length - 1)
                    sb.AppendFormat(", ");
            }
        }

        //build select clause based on list of properties skipping ones with the IgnoreSelect and NotMapped attribute
        private static void BuildSelect(StringBuilder sb, IEnumerable<PropertyInfo> props)
        {
            var propertyInfos = props as IList<PropertyInfo> ?? props.ToList();
            var addedAny = false;
            for (var i = 0; i < propertyInfos.Count(); i++)
            {
                var property = propertyInfos.ElementAt(i);

                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreSelectAttribute).Name || attr.GetType().Name == typeof(NotMappedAttribute).Name)) continue;

                if (addedAny)
                    sb.Append(",");
                sb.Append(GetColumnName(property));
                //if there is a custom column name add an "as customcolumnname" to the item so it maps properly
                if (property.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(ColumnAttribute).Name) != null)
                    sb.Append(" as " + Encapsulate(property.Name));
                addedAny = true;
            }
        }



        //Get all properties in an entity
        private static IEnumerable<PropertyInfo> GetAllProperties<T>(T entity) where T : class
        {
            if (entity == null) return new PropertyInfo[0];
            return entity.GetType().GetProperties();
        }

        //Get all properties that are not decorated with the Editable(false) attribute
        private static IEnumerable<PropertyInfo> GetScaffoldableProperties<T>()
        {
            IEnumerable<PropertyInfo> props = typeof(T).GetProperties();

            props = props.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(EditableAttribute).Name && !IsEditable(p)) == false);


            return props.Where(p => p.PropertyType.IsSimpleType() || IsEditable(p));
        }

        //Determine if the Attribute has an AllowEdit key and return its boolean state
        //fake the funk and try to mimick EditableAttribute in System.ComponentModel.DataAnnotations 
        //This allows use of the DataAnnotations property in the model and have the SimpleCRUD engine just figure it out without a reference
        private static bool IsEditable(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == typeof(EditableAttribute).Name);
                if (write != null)
                {
                    return write.AllowEdit;
                }
            }
            return false;
        }

        #region 自定义方法
        /// <summary>
        /// 添加 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityToInsert"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The ID (primary key) of the newly inserted record if it is identity using the int? type, otherwise null</returns>
        public static int Insert<TEntity>(this IDbConnection connection, TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
           return Insert<int, TEntity>(connection, entityToInsert, transaction, commandTimeout);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityToInsert"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The ID (primary key) of the newly inserted record if it is identity using the defined type, otherwise null</returns>
        private static TKey Insert<TKey, TEntity>(this IDbConnection connection, TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {

            var name = GetTableName(entityToInsert);
            var sb = new StringBuilder();
            sb.AppendFormat("insert into {0}", name);
            sb.Append(" (");
            BuildInsertParameters<TEntity>(sb);
            sb.Append(") ");
            sb.Append("values");
            sb.Append(" (");
            BuildInsertValues<TEntity>(sb);
            sb.Append(")");

            if (IsIdentity(entityToInsert))
            {
                sb.Append(";" + _getIdentitySql);
            }
            else
            {
                sb.Append("; select @@ROWCOUNT as id ");
            }
            if (IsRequiredNull(entityToInsert))
            {
                dynamic i = 0;
                return (TKey)i;
            }
            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("Insert: {0}", sb));
            var r = connection.Query(sb.ToString(), entityToInsert, transaction, true, commandTimeout);
            return (TKey)r.First().id;
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityToUpdate"></param>
        /// <param name="conditions">更新条件</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of affected records</returns>
        public static int Update<TEntity>(this IDbConnection connection, TEntity entityToUpdate, string conditions="", IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var idProps = GetIdProperties(entityToUpdate).ToList();

            if (!idProps.Any() && string.IsNullOrEmpty(conditions))
                return 0;
             

            var name = GetTableName(entityToUpdate);

            var sb = new StringBuilder();
            sb.AppendFormat("update {0}", name);

            sb.AppendFormat(" set ");
            BuildUpdateSet(entityToUpdate, sb);
            sb.Append(" where ");
            if (conditions != null && conditions.Length > 0)
            {
                conditions = ReplaceFist(conditions,"where");
                sb.Append(conditions.Replace("--", "——"));
            }
            else
            {
                BuildWhere<TEntity>(sb, idProps, entityToUpdate);
            }
            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("Update: {0}", sb));

            return connection.Execute(sb.ToString(), entityToUpdate, transaction, commandTimeout);
        }
        /// <summary>
        /// 根据主键删除（只能有一个主键）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of records affected</returns>
        public static int Delete<T>(this IDbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();
            if (!idProps.Any() || idProps.Count != 1)
                return 0;

            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.AppendFormat("Delete from {0} where ", name);
            sb.AppendFormat("{0} = @{1}", GetColumnName(idProps[0]), idProps[0].Name);
            var dynParms = new DynamicParameters();
            if (idProps.First().PropertyType == typeof(int))
                dynParms.Add("@" + idProps.First().Name, int.Parse( id.ToString()));
            else
                dynParms.Add("@" + idProps.First().Name, id);

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("Delete<{0}> {1}", currenttype, sb));

            return connection.Execute(sb.ToString(), dynParms, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of records affected</returns>
        public static int DeleteList<T>(this IDbConnection connection, string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (string.IsNullOrEmpty(conditions))
                return 0;

            var currenttype = typeof(T);
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.AppendFormat("Delete from {0}", name);
            sb.Append(" where ");
            conditions = ReplaceFist(conditions, "where");
            sb.Append(conditions.Replace("--", "——"));

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("DeleteList<{0}> {1}", currenttype, sb));

            return connection.Execute(sb.ToString(), parameters, transaction, commandTimeout);
        }
        /// <summary>
        /// 替换相匹配的首个字符串 自动转换为小写字母
        /// </summary>
        /// <param name="conditions">原始字符串</param>
        /// <param name="oldstr">要替换的字符串</param>
        /// <param name="newstr">新的字符串 默认为空</param>
        /// <param name="islower">是否转换为小写字母 默认转换</param>
        /// <returns></returns>
        private static string ReplaceFist(string conditions, string oldstr,string newstr="",bool islower=true)
        {
            if (islower)
            {
                Regex r = new Regex(oldstr.ToLower());
                return r.Replace(conditions.ToLower(), newstr.ToLower(), 1);
            }
            else
            {
                Regex r = new Regex(oldstr);
                return r.Replace(conditions, newstr, 1);
            }
        }
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static string GetTableName(object entity)
        {
            var type = entity.GetType();
            return GetTableName(type);
        }
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetTableName(Type type)
        {
            string tableName;

            if (TableNames.TryGetValue(type, out tableName))
            {
                return tableName;
            }

            tableName = _tableNameResolver.ResolveTableName(type);

            TableNames.AddOrUpdate(type, tableName, (t, v) => tableName);

            return tableName;
        }


        /// <summary>
        /// 根据表结构拼接字段名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sb"></param>
        /// <param name="keytype"></param>
        private static void BuildInsertParameters<T>(StringBuilder sb)
        {
            var props = GetScaffoldableProperties<T>().ToArray();

            for (var i = 0; i < props.Count(); i++)
            {
                var property = props.ElementAt(i);

                if (IsIdentity(property))
                    continue;
                sb.Append(GetColumnName(property));
                if (i < props.Count() - 1)
                    sb.Append(", ");
            }
            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);
        }
        /// <summary>
        /// 根据字段结构获取字段名称
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static string GetColumnName(PropertyInfo propertyInfo)
        {
            string columnName, key = string.Format("{0}.{1}", propertyInfo.DeclaringType, propertyInfo.Name);

            if (ColumnNames.TryGetValue(key, out columnName))
            {
                return columnName;
            }

            columnName = _columnNameResolver.ResolveColumnName(propertyInfo);

            ColumnNames.AddOrUpdate(key, columnName, (t, v) => columnName);
            return columnName;
        }
        /// <summary>
        /// 根据表结构拼接字段值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sb"></param>
        /// <param name="keytype"></param>
        private static void BuildInsertValues<T>(StringBuilder sb)
        {
            var props = GetScaffoldableProperties<T>().ToArray();
            for (var i = 0; i < props.Count(); i++)
            {
                var property = props.ElementAt(i);
                if (IsIdentity(property))
                    continue;
                sb.AppendFormat("@{0}", property.Name);
                if (i < props.Count() - 1)
                    sb.Append(", ");
            }
            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);

        }
        /// <summary>
        /// 获取where参数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sb"></param>
        /// <param name="idProps"></param>
        /// <param name="whereConditions"></param>
        private static void BuildWhere<TEntity>(StringBuilder sb, IEnumerable<PropertyInfo> idProps, object whereConditions = null)
        {
            var propertyInfos = idProps.ToArray();
            for (var i = 0; i < propertyInfos.Count(); i++)
            {
                var useIsNull = false;
                var propertyToUse = propertyInfos.ElementAt(i);
                var sourceProperties = GetScaffoldableProperties<TEntity>().ToArray();
                for (var x = 0; x < sourceProperties.Count(); x++)
                {
                    if (sourceProperties.ElementAt(x).Name == propertyToUse.Name)
                    {
                        if (whereConditions != null && propertyToUse.CanRead && (propertyToUse.GetValue(whereConditions, null) == null || propertyToUse.GetValue(whereConditions, null) == DBNull.Value))
                        {
                            useIsNull = true;
                        }
                        propertyToUse = sourceProperties.ElementAt(x);
                        break;
                    }
                }
                sb.AppendFormat(
                    useIsNull ? "{0} is null" : "{0} = @{1}",
                    GetColumnName(propertyToUse),
                    propertyToUse.Name);

                if (i < propertyInfos.Count() - 1)
                    sb.AppendFormat(" and ");
            }
        }

        /// <summary>
        /// 是否有为空的必填项
        /// </summary>
        /// <returns></returns>
        private static bool IsRequiredNull<T>(T entityToInsert)
        {
            IEnumerable<PropertyInfo> pilist = entityToInsert.GetType().GetProperties().Where(o => o.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(RequiredAttribute).Name));
            if (pilist.Count() > 0)
            {
                if (pilist.Where(o => o.GetValue(entityToInsert) == null).Count() > 0)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 是否有必填
        /// </summary>
        /// <returns></returns>
        private static bool IsRequired<T>(T entityToInsert)
        {
            IEnumerable<PropertyInfo> pilist = entityToInsert.GetType().GetProperties().Where(o => o.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(RequiredAttribute).Name));
            if (pilist.Count() > 0)
                return true;
            return false;
        }
        /// <summary>
        /// 是否必填
        /// </summary>
        /// <returns></returns>
        private static bool IsRequired(PropertyInfo pi)
        {
            if (pi.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(RequiredAttribute).Name))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否有主键
        /// </summary>
        /// <returns></returns>
        private static bool IsKey<T>(T entityToInsert)
        {
            IEnumerable<PropertyInfo> pilist = entityToInsert.GetType().GetProperties().Where(o => o.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name));
            if (pilist.Count() > 0)
                return true;
            return false;
        }
        /// <summary>
        /// 是否主键
        /// </summary>
        /// <returns></returns>
        private static bool IsKey(PropertyInfo pi)
        {
            if (pi.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否有自增主键
        /// </summary>
        /// <returns></returns>
        private static bool IsIdentity<T>(T entityToInsert)
        {
            IEnumerable<PropertyInfo> pilist = entityToInsert.GetType().GetProperties().Where(o => o.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name));
            foreach (PropertyInfo pi in pilist)
            {
                try
                {
                    object[] objArray = pi.GetCustomAttributes(false);
                    if (objArray.Length > 1)
                    {
                        if (objArray[1] is Model.Base.Property&& (objArray[1] as Model.Base.Property).Value == "Identity")
                        {
                            return true;
                        }
                    }
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// 是否自增主键
        /// </summary>
        /// <returns></returns>
        private static bool IsIdentity(PropertyInfo pi)
        {
            try
            {
                if (pi.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name))
                {
                    object[] objArray = pi.GetCustomAttributes(false);
                    if (objArray.Length > 1)
                    {
                    
                        if (objArray[1] is Model.Base.Property && (objArray[1] as Model.Base.Property).Value == "Identity")
                        {
                          return true;
                        }
                    }
                }
            }
            catch
            {

            }
            return false;
        }

      
        #endregion

        //Determine if the Attribute has an IsReadOnly key and return its boolean state
        //fake the funk and try to mimick ReadOnlyAttribute in System.ComponentModel 
        //This allows use of the DataAnnotations property in the model and have the SimpleCRUD engine just figure it out without a reference
        private static bool IsReadOnly(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == typeof(ReadOnlyAttribute).Name);
                if (write != null)
                {
                    return write.IsReadOnly;
                }
            }
            return false;
        }

        //Get all properties that are:
        //Not named Id
        //Not marked with the Key attribute
        //Not marked ReadOnly
        //Not marked IgnoreInsert
        //Not marked NotMapped
        private static IEnumerable<PropertyInfo> GetUpdateableProperties<T>(T entity)
        {
            var updateableProperties = GetScaffoldableProperties<T>();
            //remove ones with ID
            updateableProperties = updateableProperties.Where(p => !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            //remove ones with key attribute
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name) == false);
            //remove ones that are readonly
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => (attr.GetType().Name == typeof(ReadOnlyAttribute).Name) && IsReadOnly(p)) == false);
            //remove ones with IgnoreUpdate attribute
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreUpdateAttribute).Name) == false);
            //remove ones that are not mapped
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(NotMappedAttribute).Name) == false);

            return updateableProperties;
        }

        //Get all properties that are named Id or have the Key attribute
        //For Inserts and updates we have a whole entity so this method is used
        private static IEnumerable<PropertyInfo> GetIdProperties(object entity)
        {
            var type = entity.GetType();
            return GetIdProperties(type);
        }

        //Get all properties that are named Id or have the Key attribute
        //For Get(id) and Delete(id) we don't have an entity, just the type so this method is used
        private static IEnumerable<PropertyInfo> GetIdProperties(Type type)
        {
            var tp = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)).ToList();
            return tp.Any() ? tp : type.GetProperties().Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        }



  

        private static string Encapsulate(string databaseword)
        {
            return string.Format(_encapsulation, databaseword);
        }
        /// <summary>
        /// Generates a guid based on the current date/time
        /// http://stackoverflow.com/questions/1752004/sequential-guid-generator-c-sharp
        /// </summary>
        /// <returns></returns>
        public static Guid SequentialGuid()
        {
            var tempGuid = Guid.NewGuid();
            var bytes = tempGuid.ToByteArray();
            var time = DateTime.Now;
            bytes[3] = (byte)time.Year;
            bytes[2] = (byte)time.Month;
            bytes[1] = (byte)time.Day;
            bytes[0] = (byte)time.Hour;
            bytes[5] = (byte)time.Minute;
            bytes[4] = (byte)time.Second;
            return new Guid(bytes);
        }

        /// <summary>
        /// Database server dialects
        /// </summary>
        public enum Dialect
        {
            SQLServer,
            PostgreSQL,
            MySQL,
        }

        public interface ITableNameResolver
        {
            string ResolveTableName(Type type);
        }

        public interface IColumnNameResolver
        {
            string ResolveColumnName(PropertyInfo propertyInfo);
        }

        public class TableNameResolver : ITableNameResolver
        {
            public virtual string ResolveTableName(Type type)
            {
                var tableName = Encapsulate(type.Name);

                var tableattr = type.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(TableAttribute).Name) as dynamic;
                if (tableattr != null)
                {
                    tableName = Encapsulate(tableattr.Name);
                    try
                    {
                        if (!String.IsNullOrEmpty(tableattr.Schema))
                        {
                            string schemaName = Encapsulate(tableattr.Schema);
                            tableName = String.Format("{0}.{1}", schemaName, tableName);
                        }
                    }
                    catch (RuntimeBinderException)
                    {
                        //Schema doesn't exist on this attribute.
                    }
                }

                return tableName;
            }
        }

        public class ColumnNameResolver : IColumnNameResolver
        {
            public virtual string ResolveColumnName(PropertyInfo propertyInfo)
            {
                var columnName = Encapsulate(propertyInfo.Name);

                var columnattr = propertyInfo.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(ColumnAttribute).Name) as dynamic;
                if (columnattr != null)
                {
                    columnName = Encapsulate(columnattr.Name);
                    if (Debugger.IsAttached)
                        Trace.WriteLine(String.Format("Column name for type overridden from {0} to {1}", propertyInfo.Name, columnName));
                }
                return columnName;
            }
        }
    }

    //=================================================调用类==================================================================
    
    /// <summary>
    /// Optional Table attribute.
    /// You can use the System.ComponentModel.DataAnnotations version in its place to specify the table name of a poco
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// Optional Table attribute.
        /// </summary>
        /// <param name="tableName"></param>
        public TableAttribute(string tableName)
        {
            Name = tableName;
        }
        /// <summary>
        /// Name of the table
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Name of the schema
        /// </summary>
        public string Schema { get; set; }
    }

    /// <summary>
    /// Optional Column attribute.
    /// You can use the System.ComponentModel.DataAnnotations version in its place to specify the table name of a poco
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Optional Column attribute.
        /// </summary>
        /// <param name="columnName"></param>
        public ColumnAttribute(string columnName)
        {
            Name = columnName;
        }
        /// <summary>
        /// Name of the column
        /// </summary>
        public string Name { get; private set; }
    }

    /// <summary>
    /// Optional Key attribute.
    /// You can use the System.ComponentModel.DataAnnotations version in its place to specify the Primary Key of a poco
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
    }

    /// <summary>
    /// Optional NotMapped attribute.
    /// You can use the System.ComponentModel.DataAnnotations version in its place to specify that the property is not mapped
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotMappedAttribute : Attribute
    {
    }

    /// <summary>
    /// Optional Key attribute.
    /// You can use the System.ComponentModel.DataAnnotations version in its place to specify a required property of a poco
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAttribute : Attribute
    {
    }

    /// <summary>
    /// Optional Editable attribute.
    /// You can use the System.ComponentModel.DataAnnotations version in its place to specify the properties that are editable
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableAttribute : Attribute
    {
        /// <summary>
        /// Optional Editable attribute.
        /// </summary>
        /// <param name="iseditable"></param>
        public EditableAttribute(bool iseditable)
        {
            AllowEdit = iseditable;
        }
        /// <summary>
        /// Does this property persist to the database?
        /// </summary>
        public bool AllowEdit { get; private set; }
    }

    /// <summary>
    /// Optional Readonly attribute.
    /// You can use the System.ComponentModel version in its place to specify the properties that are editable
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ReadOnlyAttribute : Attribute
    {
        /// <summary>
        /// Optional ReadOnly attribute.
        /// </summary>
        /// <param name="isReadOnly"></param>
        public ReadOnlyAttribute(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
        }
        /// <summary>
        /// Does this property persist to the database?
        /// </summary>
        public bool IsReadOnly { get; private set; }
    }

    /// <summary>
    /// Optional IgnoreSelect attribute.
    /// Custom for Dapper.SimpleCRUD to exclude a property from Select methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreSelectAttribute : Attribute
    {
    }

    /// <summary>
    /// Optional IgnoreInsert attribute.
    /// Custom for Dapper.SimpleCRUD to exclude a property from Insert methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreInsertAttribute : Attribute
    {
    }

    /// <summary>
    /// Optional IgnoreUpdate attribute.
    /// Custom for Dapper.SimpleCRUD to exclude a property from Update methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreUpdateAttribute : Attribute
    {
    }

}

internal static class TypeExtension
{
    //You can't insert or update complex types. Lets filter them out.
    public static bool IsSimpleType(this Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        type = underlyingType ?? type;
        var simpleTypes = new List<Type>
                               {
                                   typeof(byte),
                                   typeof(sbyte),
                                   typeof(short),
                                   typeof(ushort),
                                   typeof(int),
                                   typeof(uint),
                                   typeof(long),
                                   typeof(ulong),
                                   typeof(float),
                                   typeof(double),
                                   typeof(decimal),
                                   typeof(bool),
                                   typeof(string),
                                   typeof(char),
                                   typeof(Guid),
                                   typeof(DateTime),
                                   typeof(DateTimeOffset),
                                   typeof(byte[])
                               };
        return simpleTypes.Contains(type) || type.IsEnum;
    }
}
