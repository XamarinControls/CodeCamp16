using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SQLite.Net;
using SQLite.Net.Async;

namespace sqlitetest1.Interfaces
{
    public interface IDBManager
    {
        /// <summary>
        /// <para>Set to true to prevent any database function from firing. </para>
        /// <para>To update database, need to use global connection object</para>
        /// </summary>
        /// <value><c>true</c> if is migrating handler; otherwise, <c>false</c>.</value>
        bool IsMigrating { get; set; }
        /// <summary>
        /// global Connection object. 
        /// </summary>
        /// <value>The connection.</value>
        SQLiteAsyncConnection Connection { get; }
        /// <summary>
        /// Exposed Exception Handler
        /// </summary>
        /// <value>The DBM anager exception handler.</value>
        Action<Exception> DBManagerExceptionHandler { get; set; }
        /// <summary>
        /// database file name
        /// </summary>
        /// <value>The name of the file.</value>
        string FileName { get; }
        /// <summary>
        /// Database IsInit flag. should be true if database is initialized
        /// </summary>
        /// <value><c>true</c> if is init; otherwise, <c>false</c>.</value>
        bool IsInit { get; }
        /// <summary>
        /// database path
        /// </summary>
        /// <value>The path.</value>
        string Path { get; }
        /// <summary>
        /// <para>execute a sql statment to ensure only one row exist.</para>
        /// <para>First tries an update, then insert in a transaction</para>
        /// <para>If table has more than one row, rows are purged before Insert</para>
        /// </summary>
        /// <example>
        /// 
        /// var row = new DB.OneRow();
        ///	var firstName = "Andrew";
        ///	var LastName = "McCormack";
        ///	var DOB = new System.Random().Next(100, 500);
        ///
        ///	var updateSql = App.dbManager.UpdateStatementBuilder&lt;DB.OneRow&gt;(
        ///		new[] { Helpers.NameOf(() => row.FirstName), Helpers.NameOf(() => row.LastName), Helpers.NameOf(() => row.DOB) },
        ///		new object[] { firstName, LastName, DOB },
        ///		"where FirstName='Andrew'"
        ///	);
        ///
        ///	var insertSql = App.dbManager.InsertStatementBuilder&lt;DB.OneRow&gt;(
        ///		new[] { Helpers.NameOf(() => row.ID), Helpers.NameOf(() => row.FirstName), Helpers.NameOf(() => row.LastName), Helpers.NameOf(() => row.DOB) },
        ///		new object[] { 10, firstName, LastName, DOB }
        ///	);
        /// 
        ///	var ok = await App.dbManager.AllowOneRow<DB.OneRow>(insertSql, updateSql);
        /// 
        /// </example>
        /// <param name="insertSql">insert SQl</param>
        /// <param name="UpdateSql">update SQL</param>
        /// <returns>true/false</returns>
        /// <typeparam name="T">Database type,used if more than one row to clear table</typeparam>
        Task<bool> AllowOneRow<T>(string insertSql, string UpdateSql) where T : class;
        /// <summary>
        /// <para>Primary key or index fields need to be set, or use the ObjectUpdater to set if after it is found</para>
        /// <para>Search for a row based on the query, where clause. if found returns to caller with values before insert or update</para>
        /// <para>Prevents multiple rows from being inserted. It is locked for only one thread</para>
        /// <para>Before Update or Insert, has Action to preform any updates on the object.  use this to set a primary key value if one was missing</para>
        /// <para>from the Call.</para>
        /// <para>If insert, Database Type object has primary key attached</para>
        /// </summary>
        /// <example>
        /// var row = new DB.OneRow();
        /// row.FirstName = "Andrew";
        /// row.LastName = "McCormack";
        /// row.DOB = new System.Random().Next(100, 500);
        /// await App.dbManager.AllowOneRow&lt;DB.OneRow&gt;(row, (x) => x.ID == 0, (arg1, arg2) =>
        /// {
        /// 	if (arg2 != null)
        /// 	{
        /// 		// found row;
        /// 		arg1.ID = arg2.ID;
        /// 	}
        /// 
        /// }, (List&lt;DB.OneRow&gt; arg1, DB.OneRow arg2) => {
        /// 	// Error Handler for more than one row
        /// 	var t = 10;
        /// });
        /// })
        /// </example>        
        /// <param name="obj">Object</param>
        /// <param name="query">Query</param>
        /// <param name="ObjectUpdater">To Update Database object sent in with proper values if doing an update, Parameter(0):=object passed in, Parameter(1):=Query Object from Update</param>
        /// <param name="ErrorHandler">If more than one exist in the table, ErrorHandler is called with Parameter(0):=List&lt;T&gt; Found, Parameter(1):=Query Object from Update</param>
        /// <returns>true/false</returns>
        /// <typeparam name="T">Database type, must be created first with CreateTable</typeparam>
        Task<bool> AllowOneRow<T>(T obj, Func<T, bool> query, Action<T, T> ObjectUpdater, Action<List<T>, T> ErrorHandler) where T : class;
        /// <summary>
        /// All Rows
        /// </summary>
        /// <returns>List of Database object Type</returns>
        /// <typeparam name="T">Database type, must be created first with CreateTable</typeparam>
        Task<List<T>> AllRows<T>() where T : class;
        /// <summary>
        /// All Rows for Database type using Sql statement 
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// </summary>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <param name="sql">Sql statement</param>
        /// <typeparam name="T">Database type, must be created first with CreateTable</typeparam>
        Task<List<T>> AllRows<T>(string sql, bool runSync = false) where T : class;
        /// <summary>
        /// Clears the table. run in a transaction
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// </summary>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <returns>true/false</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task<bool> ClearTable<T>(bool runSync = false) where T : class;
        /// <summary>
        /// Delete transaction query based on Expression
        /// </summary>
        /// <param name="query">Expression (where clause)</param>
        /// <returns>true/false</returns>
        /// <typeparam name="T">Database type, must be created first with CreateTable</typeparam>
        Task<bool> Delete<T>(Expression<Func<T, bool>> query) where T : class;
        /// <summary>
        /// Delete the row
        /// </summary>
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// <param name="obj">Database Object</param>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <returns>true/false</returns>
        /// <typeparam name="T">Database type, must be created first with CreateTable</typeparam>
        Task<bool> Delete<T>(T obj, bool runSync = false) where T : class;
        /// <summary>
        /// <para>Execute Drop table</para>
        /// <para>Drop table: deletes the table. Not the Same ClearTable, which drops and than re-creates table</para>
        /// </summary>
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <returns>The table.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task<bool> DropTable<T>(bool runSync = false) where T : class;
        /// <summary>
        /// Execute a SQL statement and return the rows Affected
        /// </summary>
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <param name="sql">Sql statement</param>
        /// <returns>Rows changed</returns>
        /// <typeparam name="T">Database type, must be created first with CreateTable</typeparam>
        Task<int> Execute(string sql, bool runSync = false);
        /// <summary>
        /// <para>Insert by Executing a Sql Statement. Runs in a Transaction.</para>
        /// <para>Multiple update statements separated by ;</para>
        /// </summary>
        /// <param name="sqlStatment">Sql statment.</param>
        /// <returns>true/false</returns>
        Task<bool> ExecuteInsert(string sqlStatment);
        /// <summary>
        /// <para>Insert by building a sql statement in a transaction.</para>
        /// <para>fields and values array must be same size!</para>
        /// </summary>
        /// <param name="table">table name</param>
        /// <param name="fields">Fields array</param>
        /// <param name="values">Values array</param>
        /// <returns>true/false</returns>
        Task<bool> ExecuteInsert(string table, string[] fields, object[] values);
        /// <summary>
        /// <para>Insert by building a sql statement in a transaction. </para>
        /// <para>fields and values array must be same size!</para>
        /// </summary>
        /// <returns>true/false</returns>
        /// <param name="fields">Fields array</param>
        /// <param name="values">Values array</param>
        /// <typeparam name="T">table type</typeparam>
        Task<bool> ExecuteInsert<T>(string[] fields, object[] values) where T : class;
        /// <summary>
        /// Execute Scalar sql statement
        /// </summary>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <param name="sql">Sql statement</param>
        /// <returns>Integer</returns>
        /// <typeparam name="T">result type</typeparam>
        Task<int> ExecuteScalar(string sql, bool runSync = false);
        /// <summary>
        /// <para>Update by Executing a Sql Statement in a transaction.</para>
        /// <para>Multiple update statements separated by ;</para>
        /// </summary>
        /// <param name="sqlStatment">Sql statment.</param>
        /// <returns>true/false</returns>
        Task<bool> ExecuteUpdate(string sqlStatment);
        /// <summary>
        /// <para>Update by building a sql statement in a tranasction.</para>
        /// <para>fields and values array must be same size!</para>
        /// </summary>
        /// <param name="table">table name</param>
        /// <param name="fields">Fields array</param>
        /// <param name="values">Values array</param>
        /// <param name="whereClause">Where clause (optional)</param>
        /// <returns>true/false</returns>
        Task<bool> ExecuteUpdate(string table, string[] fields, object[] values, string whereClause = null);
        /// <summary>
        /// <para>Update by building a sql statement in a tranasction.</para>
        /// <para>fields and values array must be same size!</para>
        /// </summary>
        /// <param name="fields">Fields array</param>
        /// <param name="values">Values array</param>
        /// <param name="whereClause">Where clause (optional)</param>
        /// <returns>true/false</returns>
        /// <typeparam name="T">Table Type</typeparam>
        Task<bool> ExecuteUpdate<T>(string[] fields, object[] values, string whereClause = null) where T : class;
        /// <summary>
        /// Query, return first row only, no query (where clause)
        /// </summary>
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <returns>object of Database Type</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task<T> FirstRow<T>(bool runSync = false) where T : class;
        /// <summary>
        /// <para>Init the Database Manager</para>
        /// <para>executes if this.IsInit = false</para>
        /// </summary>
        void Init();
        /// <summary>
        /// Insert into the database and set the primary key to the object passed in if has [AutoIncrement] on object
        /// </summary>
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// <param name="runSync">If true, runs Sync, default false. SYNC will BLOCK</param>
        /// <param name="obj">Database object, add primary key value after insert</param>
        /// <typeparam name="T">Database type, must be created first with CreateTable</typeparam>
        Task<bool> Insert<T>(T obj, bool runSync = false) where T : class;
        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <param name="obj">List of Database Objects</param>
        /// <returns>true/false</returns>
        /// <typeparam name="T">Database type, must be created first with CreateTable</typeparam>
        Task<bool> InsertAll<T>(List<T> obj, bool runSync = false) where T : class;
        /// <summary>
        /// <para>Insert Statement Builder. </para>
        /// <para>fields and values array must be same size!</para>
        /// </summary>
        /// <returns>sql statement</returns>
        /// <param name="table">Table name</param>
        /// <param name="fields">Fields array</param>
        /// <param name="values">Values array</param>
        string InsertStatementBuilder(string table, string[] fields, object[] values);
        /// <summary>
        /// Insert Statement Builder. fields and values array must be same size!
        /// </summary>
        /// <returns>sql statement</returns>
        /// <param name="table">Table name</param>
        /// <param name="field">Field name</param>
        /// <param name="value">Value</param>
        string InsertStatementBuilder(string table, string field, object value);
        /// <summary>
        /// <para>Insert Statement Builder. </para>
        /// <para>fields and values array must be same size!</para>
        /// </summary>
        /// <returns>sql statement</returns>
        /// <param name="fields">Fields array</param>
        /// <param name="values">Values array</param>
        /// <typeparam name="T">table type</typeparam>
        string InsertStatementBuilder<T>(string[] fields, object[] values) where T : class;
        /// <summary>
        /// <para>Insert Statement Builder. </para>
        /// </summary>
        /// <returns>sql statement</returns>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <typeparam name="T">table type</typeparam>
        string InsertStatementBuilder<T>(string field, object value) where T : class;
        /// <summary>
        /// All Rows with Query
        /// </summary>
        /// <typeparam name="T">Database type, must be created first with CreateTable</typeparam>
        Task<List<T>> Query<T>(Expression<Func<T, bool>> query) where T : class;
        /// <summary>
        /// Query, return first row only
        /// </summary>
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <param name="query">Query, where clause Expression</param>
        /// <returns>object of Database Type</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task<T> QueryFirst<T>(Expression<Func<T, bool>> query, bool runSync = false) where T : class;
        /// <summary>
        /// <para>Checks if the row exists for update, if no row match tries insert</para>
        /// <para>must have primary key attribute set to value for update to work</para>
        /// <para>If [PrimaryKey, AutoIncrement] attribute and value not set, it will perform an Insert. </para>
        /// <para>To Perform an update, set the PrimaryKey value or constaint values</para>
        /// </summary>
        /// <param name="obj">Database Object</param>
        /// <returns>true/false</returns>
        /// <typeparam name="T">Database type, must be created first with CreateTable</typeparam>
        Task<bool> ReInsert<T>(T obj) where T : class;
        /// <summary>
        /// <para>Checks if the row exists for update, if no row match tries insert</para>
        /// <para>must have primary key attribute set to value for update to work</para>
        /// <para>If [PrimaryKey, AutoIncrement] attribute and value not set, it will perform an Insert. </para>
        /// <para>To Perform an update, set the PrimaryKey value or constaint values</para>
        /// </summary>
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// <param name="obj">Database Object</param>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <returns>true/false</returns>
        /// <typeparam name="T">Database type, must be created first with CreateTable</typeparam>
        Task<bool> ReInsert<T>(T obj, bool runSync = false) where T : class;
        /// <summary>
        /// <para>Reset the database Init Flag.</para>
        /// <para>Must call Init to re-Init.</para>
        /// <para>Call during startup ONLY</para>
        /// </summary>
        void Reset();
        /// <summary>
        /// <para>Reset the database Init Flag and reInit database.</para>
        /// <para>Call during startup ONLY</para>
        /// </summary>
        void ResetDatabaseState();
        /// <summary>
        /// Row count sql, without using Linq. Execute a sql statement to get the row count
        /// </summary>
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <returns>Count</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task<int> RowCount<T>(bool runSync = false) where T : class;
        /// <summary>
        /// <para>Run in a Transaction.</para>
        /// <para>better for large inserts or multiple statement in a batch</para>
        /// </summary>
        /// <param name="transaction">Action&lt;Transaction&gt;</param>
        /// <returns>true/false</returns>
        Task<bool> RunInTransaction(Action<SQLiteConnection> transaction);
        /// <summary>
        /// Update
        /// </summary>
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <param name="obj">true/false</param>
        /// <typeparam name="T">Database type, must be created first with CreateTable</typeparam>
        Task<bool> Update<T>(T obj, bool runSync = false) where T : class;
        /// <summary>
        /// Update all. Must have primary key value set
        /// </summary>
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <param name="obj">List of Database Type Objects</param>
        /// <returns>total updated</returns>
        Task<int> UpdateAll(IEnumerable obj, bool runSync = false);
        /// <summary>
        /// <para>Update Statement Builder. </para>
        /// <para>fields and values array must be same size!</para>
        /// </summary>
        /// <returns>sql statement</returns>
        /// <param name="table">Table name</param>
        /// <param name="fields">Fields array</param>
        /// <param name="values">Values array</param>
        /// <param name="whereClause">Where clause.(optional). include \"where\"</param>
        string UpdateStatementBuilder(string table, string[] fields, object[] values, string whereClause = null);
        /// <summary>
        /// Update Statement Builder. fields and values array must be same size!
        /// </summary>
        /// <returns>sql statement</returns>
        /// <param name="table">Table name</param>
        /// <param name="field">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="whereClause">Where clause (optional). include \"where\"</param>
        string UpdateStatementBuilder(string table, string field, object value, string whereClause = null);
        /// <summary>
        /// <para>Update Statement Builder. </para>
        /// <para>fields and values array must be same size!</para>
        /// </summary>
        /// <returns>sql statement</returns>
        /// <param name="fields">Fields array</param>
        /// <param name="values">Values array</param>
        /// <param name="whereClause">Where clause (optional). include \"where\"</param>
        /// <typeparam name="T">table type</typeparam>
        string UpdateStatementBuilder<T>(string[] fields, object[] values, string whereClause = null) where T : class;
        /// <summary>
        /// <para>Update Statement Builder. </para>
        /// <para>fields and values array must be same size!</para>
        /// </summary>
        /// <returns>sql statement</returns>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="whereClause">Where clause (optional). include \"where\"</param>
        /// <typeparam name="T">table type</typeparam>
        string UpdateStatementBuilder<T>(string field, object value, string whereClause = null) where T : class;
        /// <summary>
        /// <para>Execute Drop table</para>
        /// <para>Drop table: deletes the table. Not the Same ClearTable, which drops and than re-creates table</para>
        /// </summary>
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// <param name="SqlConnection">SqlLite Connection</param>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <param name="ignoreCheck">ignore db.Init check</param>
        /// <returns>The table.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task<bool> DropTable<T>(SQLiteAsyncConnection SqlConnection, bool runSync = false, bool ignoreCheck = false) where T : class;
        /// <summary>
        /// Clears the table. run in a transaction
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// </summary>
        /// <param name="SqlConnection">SqlLite Connection</param>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <param name="ignoreCheck">ignore db.Init check</param>
        /// <returns>true/false</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task<bool> ClearTable<T>(SQLiteAsyncConnection SqlConnection, bool runSync = false, bool ignoreCheck = false) where T : class;
        /// <summary>
        /// <para>Run in a Transaction.</para>
        /// <para>better for large inserts or multiple statement in a batch</para>
        /// </summary>
        /// <param name="SqlConnection">SqlLite Connection</param>
        /// <param name="transaction">Action&lt;Transaction&gt;</param>
        /// <param name="ignoreCheck">ignore db.Init check</param>
        /// <returns>true/false</returns>
        Task<bool> RunInTransaction(SQLiteAsyncConnection SqlConnection, Action<SQLiteConnection> transaction, bool ignoreCheck = false);
        /// <summary>
        /// Sqlite table Exists
        /// </summary>
        /// <param name="SqlConnection">SqlLite Connection</param>
        /// <param name="tableName">Table name.</param>
        /// <returns><c>true</c>, if exists was tabled, <c>false</c> otherwise.</returns>
        bool TableExists(SQLiteAsyncConnection SqlConnection, string tableName);
        /// <summary>
        /// Row count sql, without using Linq. Execute a sql statement to get the row count
        /// </summary>
        /// <code>
        /// If runSync = true, use
        /// ().GetAwaiter().GetResult();
        /// </code>
        /// <param name="SqlConnection">SqlLite Connection</param>
        /// <param name="runSync">If true, runs Sync. default false. SYNC will BLOCK</param>
        /// <param name="ignoreCheck">ignore db.Init check</param>
        /// <returns>Count</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task<Int32> RowCount<T>(SQLiteAsyncConnection SqlConnection, bool runSync = false, bool ignoreCheck = false) where T : class;
    }
}