using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JPCToolLibrary
{
    /// <summary>
    /// sql server 数据库的常用操作
    /// </summary>
    public class SqlHelper
    {
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="connStr">数据库连接串</param>
        /// <returns></returns>
        public static Tuple<bool, string> ConnectionDataBase(string connStr)
        {

            Tuple<bool, string> tuple = new Tuple<bool, string>(false, "");
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connStr;
                    conn.Open();
                    tuple = new Tuple<bool, string>(true, "数据库连接成功");
                }
                return tuple;
            }
            catch (Exception e)
            {
                tuple = new Tuple<bool, string>(true, $"数据库连接出错，错误信息：{e.Message} \r\n {e.StackTrace}");
                return tuple;
            }
        }
        /// <summary>
        /// 执行insertsql语句的ADO.NET操作
        /// 专门为自增长列的插入语句描述
        /// </summary>
        /// <param name="connStr">数据库连接串</param>
        /// <param name="cmdType">命令的类型</param>
        /// <param name="cmdText">执行的SQL脚本或存储过程的名称</param>
        /// <param name="cmdPrams">执行的SQL脚本的参数列表，没有则为NULL</param>
        /// <returns>返回自增长序列</returns>
        public static int ExecuteInsert(string connStr, CommandType cmdType, string cmdText, List<SqlParameter> cmdPrams)
        {
            // 创建连接对象，通往底层的物理数据库通道
            try
            {
                using (SqlConnection conn = new SqlConnection())
                using (SqlCommand cmd = new SqlCommand())
                {
                    conn.ConnectionString = connStr;
                    // 打开连接通道
                    conn.Open();
                    // 命令的发布和处理
                    cmd.Connection = conn;
                    cmd.CommandType = cmdType;
                    cmd.CommandText = cmdText + " select @newidentity = @@IDENTITY;";
                    // 添加参数列表
                    cmd.Parameters.Clear();
                    if (cmdPrams != null)
                    {
                        foreach (SqlParameter parm in cmdPrams)
                        {
                            cmd.Parameters.Add(parm);
                        }
                    }
                    cmd.Parameters.Add(new SqlParameter("@newidentity", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    });
                    // 执行命令
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.NextResult();
                        // 执行结果
                        return (int)cmd.Parameters["@newidentity"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"数据库连接异常:SqlHelper/ExecuteInsert --> {ex.Message}");
            }
        }

        /// <summary>
        /// 执行update和delete等sql语句的ADO.NET操作
        /// </summary>
        /// <param name="connStr">数据库连接串</param>
        /// <param name="cmdType">命令的类型</param>
        /// <param name="cmdText">执行的SQL脚本或存储过程的名称</param>
        /// <param name="cmdPrams">执行的SQL脚本的参数列表，没有则为NULL</param>
        /// <returns>返回的成功与否</returns>
        public static bool ExecuteNonQuery(string connStr, CommandType cmdType, string cmdText, List<SqlParameter> cmdPrams)
        {
            // 创建连接对象，通往底层的物理数据库通道
            try
            {
                using (SqlConnection conn = new SqlConnection())
                using (SqlCommand cmd = new SqlCommand())
                {
                    conn.ConnectionString = connStr;
                    // 打开连接通道
                    conn.Open();
                    cmd.Connection = conn;
                    cmd.CommandType = cmdType;
                    cmd.CommandText = cmdText;
                    // 添加参数列表
                    cmd.Parameters.Clear();
                    if (cmdPrams != null)
                    {
                        foreach (SqlParameter parm in cmdPrams)
                        {
                            cmd.Parameters.Add(parm);
                        }
                    }
                    // 执行命令
                    int i = cmd.ExecuteNonQuery();  // insert  / update / delete 等命令
                    // 执行结果
                    return i > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"数据库连接异常:SqlHelper/ExecuteNonQuery --> {ex.Message}");
            }
        }


        /// <summary>
        /// 执行包含count(*) \ max(..) 等 select 语句
        /// </summary>
        /// <param name="connStr">数据库连接串</param>
        /// <param name="cmdType">命令的类型</param>
        /// <param name="cmdText">执行的SQL脚本或存储过程的名称</param>
        /// <param name="cmdPrams">执行的SQL脚本的参数列表，没有则为NULL</param>
        /// <returns>返回的是第一行第一列的值</returns>
        public static object ExecuteScalar(string connStr, CommandType cmdType, string cmdText, List<SqlParameter> cmdPrams)
        {
            // 创建连接对象，通往底层的物理数据库通道
            try
            {
                using (SqlConnection conn = new SqlConnection())
                using (SqlCommand cmd = new SqlCommand())
                {
                    conn.ConnectionString = connStr;
                    // 打开连接通道
                    conn.Open();
                    // 命令的发布和处理
                    cmd.Connection = conn;
                    cmd.CommandType = cmdType;
                    cmd.CommandText = cmdText;
                    // 添加参数列表
                    cmd.Parameters.Clear();
                    if (cmdPrams != null)
                    {
                        foreach (SqlParameter parm in cmdPrams)
                        {
                            cmd.Parameters.Add(parm);
                        }
                    }
                    // 执行命令
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"数据库连接异常:SqlHelper/ExecuteScalar --> {ex.Message}");
            }
        }

        /// <summary>
        /// Sql Server数据库， 执行select语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">执行sql脚本或存储过程的名称</param>
        /// <param name="cmdPrams">执行的sql脚本的参数列表，没有则为NULL</param>
        /// <returns></returns>
        public static List<T> ExecuteReaderRis<T>(string connStr, CommandType cmdType, string cmdText, List<SqlParameter> cmdPrams) where T : new()
        {
            // 定义一个返回结果
            List<T> result = new List<T>();

            // 创建连接对象，通往底层的物理数据库通道
            try
            {
                using (SqlConnection conn = new SqlConnection())
                using (SqlCommand cmd = new SqlCommand())
                {
                    conn.ConnectionString = connStr;
                    // 打开连接通道
                    conn.Open();
                    // 命令的发布和处理
                    cmd.Connection = conn;
                    cmd.CommandType = cmdType;
                    cmd.CommandText = cmdText;
                    cmd.CommandTimeout = 20;
                    // 添加参数列表
                    cmd.Parameters.Clear();
                    if (cmdPrams != null)
                    {
                        foreach (SqlParameter parm in cmdPrams)
                        {
                            cmd.Parameters.Add(parm);
                        }
                    }
                    // 执行命令
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            #region 构建自动映射器（使用的技术包含泛型和反射）

                            // 实例化泛型类
                            T t = new T();
                            // 获取该泛型对象的属性
                            PropertyInfo[] properties = t.GetType().GetProperties();
                            // 遍历每个属性值
                            foreach (PropertyInfo pi in properties)
                            {
                                // 判断只读器reader中是否包含同名属性值，有则取其值，否则为NULL
                                object val = null;
                                try
                                {
                                    val = reader[pi.Name].ToString();
                                }
                                catch { }
                                // 将reader中的属性值 赋值给泛型对象的属性中
                                if (pi.CanWrite && val != null && val != DBNull.Value)
                                {
                                    pi.SetValue(t, val, null);
                                }
                            }
                            #endregion

                            result.Add(t);
                        }
                        // 返回
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"数据库连接异常:SqlHelper/ExecuteReader<T> --> {ex.Message}");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">执行sql脚本或存储过程的名称</param>
        /// <param name="cmdPrams">执行的sql脚本的参数列表，没有则为NULL</param>
        /// <returns></returns>
        public static List<T> ExecuteReader<T>(string connStr, CommandType cmdType, string cmdText, List<SqlParameter> cmdPrams) where T : new()
        {
            // 定义一个返回结果
            List<T> result = new List<T>();

            // 创建连接对象，通往底层的物理数据库通道
            try
            {
                using (SqlConnection conn = new SqlConnection())
                using (SqlCommand cmd = new SqlCommand())
                {
                    conn.ConnectionString = connStr;
                    // 打开连接通道
                    conn.Open();
                    // 命令的发布和处理
                    cmd.Connection = conn;
                    cmd.CommandType = cmdType;
                    cmd.CommandText = cmdText;
                    cmd.CommandTimeout = 20;
                    // 添加参数列表
                    cmd.Parameters.Clear();
                    if (cmdPrams != null)
                    {
                        foreach (SqlParameter parm in cmdPrams)
                        {
                            cmd.Parameters.Add(parm);
                        }
                    }
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // 执行命令
                        while (reader.Read())
                        {
                            #region 构建自动映射器（使用的技术包含泛型和反射）

                            // 实例化泛型类
                            T t = new T();
                            // 获取该泛型对象的属性
                            PropertyInfo[] properties = t.GetType().GetProperties();
                            // 遍历每个属性值
                            foreach (PropertyInfo pi in properties)
                            {
                                // 判断只读器reader中是否包含同名属性值，有则取其值，否则为NULL
                                object val = null;
                                try
                                {
                                    val = reader[pi.Name];
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception($"SqlHelper/ExecuteReader<T> --> {ex.Message}");
                                }
                                // 将reader中的属性值 赋值给泛型对象的属性中
                                if (pi.CanWrite && val != null && val != DBNull.Value)
                                {
                                    pi.SetValue(t, val, null);
                                }
                            }
                            #endregion

                            result.Add(t);
                        }
                        // 返回
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"数据库连接异常:SqlHelper/ExecuteReader<T> --> {ex.Message}");
            }
        }
        /// <summary>
        /// 将DataTable中的数据存放到数据库中
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="connectString"></param>
        public static void DataTableToSQLServer(DataTable dt, string connectString)
        {
            string connectionString = connectString;

            using (SqlConnection destinationConnection = new SqlConnection(connectionString))
            {
                destinationConnection.Open();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                {
                    try
                    {
                        bulkCopy.DestinationTableName = "Statistical";//要插入的表的表名
                        bulkCopy.BatchSize = dt.Rows.Count;
                        bulkCopy.ColumnMappings.Add("患者编号", "PatientID");//映射字段名 DataTable列名 ,数据库 对应的列名
                        bulkCopy.ColumnMappings.Add("患者姓名", "PatientName");//映射字段名 DataTable列名 ,数据库 对应的列名
                        bulkCopy.ColumnMappings.Add("患者类型", "PatientType");//映射字段名 DataTable列名 ,数据库 对应的列名
                        bulkCopy.ColumnMappings.Add("文件名称", "FilePath");//映射字段名 DataTable列名 ,数据库 对应的列名
                        bulkCopy.ColumnMappings.Add("影像设备", "AETitle");//映射字段名 DataTable列名 ,数据库 对应的列名
                        bulkCopy.ColumnMappings.Add("打印来源", "TerminalName");//映射字段名 DataTable列名 ,数据库 对应的列名
                        bulkCopy.ColumnMappings.Add("接收时间", "StudyTime");//映射字段名 DataTable列名 ,数据库 对应的列名
                        bulkCopy.ColumnMappings.Add("打印时间", "PrintTime");//映射字段名 DataTable列名 ,数据库 对应的列名
                        bulkCopy.ColumnMappings.Add("是否首次打印", "flag");//映射字段名 DataTable列名 ,数据库 对应的列名

                        bulkCopy.WriteToServer(dt);
                        //MessageBox.Show($"已查询到结果{dt.Rows.Count}条数据\r\n插入数据库成功！");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

        }

    }
}
