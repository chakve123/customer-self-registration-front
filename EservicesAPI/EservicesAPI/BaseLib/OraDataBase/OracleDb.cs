using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using BaseLib.Common;
using Oracle.DataAccess.Client;

namespace BaseLib.OraDataBase
{
    public class OracleDb<T> where T : DataProvider, new()
    {
        private string _connStr;

        public string ConnectionString
        {
            get { return _connStr; }
            set { _connStr = value; }
        }

        public delegate void DbRowProcessor(OracleDataReader readerData);
        public delegate void DbRowMultisetProcessor(OracleDataReader readerData, int resultSetIndex);
        public delegate void DbRowParameterProcessor(OracleParameterCollection readerData);


        public OracleDb()
        {
            ConnectionString = DataProviderManager<T>.Provider.ConnectionString;
        }

        public OracleDb(string connStr)
        {
            _connStr = connStr;
        }

        protected void ExecuteNonQuery(OracleConnection conn, OracleCommand command, out string errorMessage, DbRowParameterProcessor readerData = null)
        {
            conn.ConnectionString = _connStr;
            try
            {
                command.Connection = conn;
                conn.Open();
                command.ExecuteNonQuery();
                errorMessage = String.Empty;

                try
                {
                    if (readerData != null)
                        readerData.Invoke(command.Parameters);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                command.Dispose();
                conn.Dispose();
            }
        }

        public void ExecuteNonQuery(OracleCommand command, bool connOpen, out string errorMessage, bool commit = true)
        {
            errorMessage = String.Empty;
            try
            {
                if (command.Connection == null)
                {
                    var conn = new OracleConnection { ConnectionString = _connStr };
                    command.Connection = conn;

                    try
                    {
                        conn.Open();
                        command.Transaction = conn.BeginTransaction();
                    }
                    catch (Exception ex)
                    {
                        command.Transaction.Rollback();
                        command.Dispose();
                        command.Connection.Dispose();
                        errorMessage = ex.Message;
                    }
                }

                if (connOpen)
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        command.Transaction.Rollback();
                        command.Dispose();
                        command.Connection.Dispose();
                        errorMessage = ex.Message;
                    }
                }
                else
                {
                    try
                    {
                        if (commit)
                            command.Transaction.Commit();
                        else
                            command.Transaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        command.Transaction.Rollback();
                        errorMessage = ex.Message;
                    }
                    finally
                    {
                        command.Dispose();
                        command.Connection.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        protected void ExecuteScalar(OracleConnection conn, OracleCommand command, out string errorMessage, out object result)
        {
            result = null;
            conn.ConnectionString = _connStr;
            try
            {
                command.Connection = conn;
                conn.Open();
                result = command.ExecuteScalar();
                errorMessage = String.Empty;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                // Command.Dispose();
                conn.Dispose();
            }
        }

        protected void ProcessEachRow(OracleConnection conn, OracleCommand command, out string errorMessage, DbRowProcessor readerData)
        {
            errorMessage = String.Empty;

            conn.ConnectionString = _connStr;
            command.Connection = conn;

            try
            { conn.Open(); }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            OracleDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                command.Dispose();
                conn.Dispose();
                return;
            }

            while (reader.Read())
            {
                try
                { readerData.Invoke(reader); }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    break;
                }
            }

            try
            {
                reader.Dispose();
                //Command.Dispose();
                conn.Dispose();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        protected void ProcessEachMultisetRow(OracleConnection conn, OracleCommand command, out string errorMessage, DbRowMultisetProcessor readerData)
        {
            errorMessage = String.Empty;

            conn.ConnectionString = _connStr;
            command.Connection = conn;

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            OracleDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                command.Dispose();
                conn.Dispose();
                return;
            }

            int nextResultSet = 0;

            do
            {
                while (reader.Read())
                {
                    try
                    { readerData.Invoke(reader, nextResultSet); }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                        break;
                    }
                }

                nextResultSet++;
            } while (reader.NextResult());

            try
            {
                reader.Dispose();
                //Command.Dispose();
                conn.Dispose();
            }
            catch (Exception ex)
            { errorMessage = ex.Message; }
        }


        public void ExecuteNonQuery(OracleCommand command, out string errorMessage, DbRowParameterProcessor reader = null, List<Thread> threads = null)
        {
            string _errorMessage = string.Empty;
            if (threads != null)
            {
                var thread = new Thread(() =>
                {
                    using (var conn = new OracleConnection())
                    {
                        ExecuteNonQuery(conn, command, out _errorMessage, reader);
                    }
                });

                thread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                thread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
                threads.Add(thread);
                thread.Start();
            }
            else
            {
                using (var conn = new OracleConnection())
                {
                    ExecuteNonQuery(conn, command, out _errorMessage, reader);
                }
            }

            errorMessage = _errorMessage;
        }

        public void SequentialAccess(OracleCommand command, bool connOpen, out string errorMessage, DbRowProcessor readerData = null)
        {
            errorMessage = String.Empty;
            try
            {
                if (command.Connection == null)
                {
                    var conn = new OracleConnection { ConnectionString = _connStr };
                    command.Connection = conn;

                    try
                    {
                        conn.Open();
                        command.Transaction = conn.BeginTransaction();
                    }
                    catch (Exception ex)
                    {
                        command.Transaction.Rollback();
                        command.Dispose();
                        command.Connection.Dispose();
                        errorMessage = ex.Message;
                    }
                }

                if (connOpen)
                {
                    try
                    {
                        OracleDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess);

                        if (readerData != null)
                        {
                            while (reader.Read())
                            {
                                try
                                { readerData.Invoke(reader); }
                                catch (Exception ex)
                                {
                                    errorMessage = ex.Message;
                                    break;
                                }
                            }
                        }

                        reader.Dispose();
                    }
                    catch (Exception ex)
                    {
                        command.Transaction.Rollback();
                        command.Dispose();
                        command.Connection.Dispose();
                        errorMessage = ex.Message;
                    }
                }
                else
                {
                    try
                    {
                        command.Dispose();
                        command.Connection.Dispose();
                    }
                    catch (Exception ex)
                    {
                        command.Transaction.Rollback();
                        errorMessage = ex.Message;
                    }
                    finally
                    {
                        command.Connection.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                if (command.Connection != null)
                    command.Connection.Dispose();
                errorMessage = ex.Message;
            }
        }

        public void SequaltialFlushingReport(OracleCommand command, out string errorMessage)
        {
            string error;
            try
            {
                SequentialAccess(command, true, out error);
                if (!string.IsNullOrEmpty(error)) throw new Exception(error);

                if (command.Parameters["rep_out"].OracleDbType == OracleDbType.Blob)
                    CommonFunctions.SequantialFlushing((Oracle.DataAccess.Types.OracleBlob)command.Parameters["rep_out"].Value);
                else if (command.Parameters["rep_out"].OracleDbType == OracleDbType.Clob)
                    CommonFunctions.SequantialFlushing((Oracle.DataAccess.Types.OracleClob)command.Parameters["rep_out"].Value);
            }
            finally
            {
                SequentialAccess(command, false, out error);
            }

            errorMessage = error;
        }

        public void ExecuteScalar(OracleCommand command, out string errorMessage, out object result)
        {
            using (var conn = new OracleConnection())
            {
                ExecuteScalar(conn, command, out errorMessage, out result);
            }
        }

        public void ProcessEachRow(OracleCommand command, out string errorMessage, DbRowProcessor readerData, List<Thread> threads = null)
        {
            string _errorMessage = string.Empty;
            if (threads != null)
            {
                var thread = new Thread(() =>
                {
                    using (var conn = new OracleConnection())
                    {
                        ProcessEachRow(conn, command, out _errorMessage, readerData);
                    }
                });

                thread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                thread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
                threads.Add(thread);
                thread.Start();
            }
            else
            {
                using (var conn = new OracleConnection())
                {
                    ProcessEachRow(conn, command, out _errorMessage, readerData);
                }
            }

            errorMessage = _errorMessage;
        }

        public void ProcessEachMultisetRow(OracleCommand command, out string errorMessage, DbRowMultisetProcessor readerData)
        {
            using (var conn = new OracleConnection())
            {
                ProcessEachMultisetRow(conn, command, out errorMessage, readerData);
            }
        }

        public static DataRow CreateDataRow(DataTable dt, OracleDataReader dr)
        {
            DataRow datarow = dt.NewRow();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                datarow[i] = dr[i];
            }

            return datarow;
        }

        public static void DrColomnToDtColomn(DataTable dt, OracleDataReader dr)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                dt.Columns.Add(dr.GetName(i), dr.GetFieldType(i));
            }
        }
    }
}
