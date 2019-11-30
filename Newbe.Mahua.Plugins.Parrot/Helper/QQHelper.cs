using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Newbe.Mahua.Plugins.Parrot.Helper
{
    public static class QQHelper
    {
        static IAccessTableHelper accessTableHelper = new AccessTableHelper();
        static ILogHelper logHelper = new LogHelper();
        static IJsonHelper jsonHelper = new JsonHelper();
        static IQueryable<T> Query<T>() where T : IKey<Guid>, new()
        {
            var type = typeof(T);
            string sql = string.Format("SELECT * FROM {0};", type.Name);
            return accessTableHelper.ExecuteDataTable(sql).ToListModel<T>().AsQueryable();
        }

        static bool JudgeValue(string PropertyTypeName, object val)
        {
            bool continueFlag = false;
            switch (PropertyTypeName)
            {
                case "System.String":
                    if (string.IsNullOrWhiteSpace(val.ToString()))
                        continueFlag = true;
                    break;
                case "System.Guid":
                    if (val.ToGuid() == Guid.Empty)
                        continueFlag = true;
                    break;
                case "System.Int32":
                    if (val.ToInt32() == 0)
                        continueFlag = true;
                    break;
                case "System.Int64":
                    if (val.ToInt64() == 0)
                        continueFlag = true;
                    break;
                case "System.DateTime":
                    if (val.ToDateTime() == @"0001/1/1 0:00:00".ToDateTime())
                        continueFlag = true;
                    break;
                default:
                    break;
            }
            return continueFlag;
        }

        /// <summary>
        /// 添加数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="modifyNull">是否处理null值</param>
        public static T Insert<T>(this T entity, bool modifyNull = false) where T : IKey<Guid>, new()
        {
            StringBuilder strKey = new StringBuilder();
            StringBuilder strVal = new StringBuilder();
            StringBuilder strSelect = new StringBuilder();
            var entityType = entity.GetType();
            //开始拼接Insert Sql语句 name是类名（也是表名）
            strKey.AppendFormat("INSERT INTO [{0}] (", entityType.Name);
            PropertyInfo[] propertys = entityType.GetProperties();
            for (int i = 0; i < propertys.Length; i++)
            {
                var item = propertys[i];
                //获取参数名（也是sql字段名）
                var key = item.Name;
                //反射得出参数对应的值
                var val = item.GetValue(entity);
                switch (key)
                {
                    case "GUID":
                        val = Guid.NewGuid();
                        strSelect.AppendFormat("SELECT * FROM [{0}] WHERE [GUID] = '{1}';", entityType.Name, val);
                        break;
                    case "CREATETIME":
                    case "UPDATETIME":
                        val = DateTime.Now;
                        break;
                    default:
                        break;
                }
                //值为空并且modifyNull是false（表示不处理为空的值） 或者参数名是id 则不参与拼接sql
                if ((val == null && modifyNull == false) || key == "ID")
                    continue;
                if (JudgeValue(item.PropertyType.FullName, val))
                    continue;
                strKey.AppendFormat("[{0}]{1}", key, i == propertys.Length - 1 ? string.Empty : ", ");
                strVal.AppendFormat("'{0}'{1}", val, i == propertys.Length - 1 ? string.Empty : ", ");
            }
            string strKeyString = strKey.ToString();
            string strValString = strVal.ToString();
            //判断因为continue造成的 最后结尾带有", "两个字符 而造成拼接成的错误sql 所以需要删除最后两个字符（若有）
            var indexKey = strKeyString.IndexOf(", ", strKeyString.Length - 2);
            var indexVal = strValString.IndexOf(", ", strValString.Length - 2);
            if (indexKey > -1)
                strKey.Remove(strKey.Length - 2, 2);
            if (indexVal > -1)
                strVal.Remove(strVal.Length - 2, 2);
            strKey.Append(") VALUES (").Append(strVal.ToString()).Append(");");
            try
            {
                //执行sql
                if (accessTableHelper.ExecuteNonQuery(strKey.ToString()) == 0)
                {
                    logHelper.Waring(string.Format("Insert错误，SQL语句：{0}", strKey.ToString()));
                }
                entity = accessTableHelper.ExecuteDataTable(strSelect.ToString()).ToListModel<T>(true)[0];
            }
            catch (System.Exception e)
            {
                logHelper.Error(e.Message);
                entity = default(T);
            }
            return entity;
        }
        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="modifyNull"></param>
        /// <returns></returns>
        public static T Update<T>(this T entity, bool modifyNull = false) where T : IKey<Guid>, new()
        {
            StringBuilder strSet = new StringBuilder();
            //StringBuilder strID = new StringBuilder();
            StringBuilder strSelect = new StringBuilder();
            var entityType = entity.GetType();
            //开始拼接Insert Sql语句 name是类名（也是表名）
            strSet.AppendFormat("UPDATE [{0}] SET ", entityType.Name);
            PropertyInfo[] propertys = entityType.GetProperties();
            Guid ID = Guid.Empty;
            for (int i = 0; i < propertys.Length; i++)
            {
                var item = propertys[i];
                //获取参数名（也是sql字段名）
                var key = item.Name;
                //反射得出参数对应的值
                var val = item.GetValue(entity);
                //值为空并且modifyNull是false（表示不处理为空的值） 或者参数名是id 则不参与拼接sql
                if ((val == null && modifyNull == false) || key == "ID")
                {
                    if (key == "GUID")
                    {
                        ID = val.ToGuid();
                        strSelect.AppendFormat("SELECT * FROM [{0}] WHERE [GUID] = '{1}';", entityType.Name, val);
                    }
                    continue;
                }
                if (JudgeValue(item.PropertyType.FullName, val))
                    continue;
                strSet.AppendFormat("[{0}] = '{1}'{2}", key, val, i == propertys.Length - 1 ? string.Empty : ", ");
            }
            string strKeyString = strSet.ToString();
            //判断因为continue造成的 最后结尾带有", "两个字符 而造成拼接成的错误sql 所以需要删除最后两个字符（若有）
            var indexKey = strKeyString.IndexOf(", ", strKeyString.Length - 2);
            if (indexKey > -1)
                strSet.Remove(strSet.Length - 2, 2);
            strSet.AppendFormat(" WHERE [GUID] = '{0}';", ID);
            try
            {
                //执行sql
                if (accessTableHelper.ExecuteNonQuery(strSet.ToString()) == 0)
                {
                    logHelper.Waring("Update错误，SQL语句：{0}", strSet.ToString());
                }
                entity = accessTableHelper.ExecuteDataTable(strSelect.ToString()).ToListModel<T>(true)[0];
            }
            catch (System.Exception e)
            {
                logHelper.Error(e.Message);
                entity = default(T);
            }
            return entity;
        }
        /// <summary>
        /// 删除数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static void Delete<T>(this T entity)where T : IKey<Guid>, new()
        {
            var t = typeof(T);
            try
            {
                //t.GetProperties().GetValue()
                string sql = string.Format("DELETE FROM [{0}] WHERE [ID] = {1};", t.Name, t.GetProperty("ID").GetValue(entity));
                if (accessTableHelper.ExecuteNonQuery(sql.ToString()) == 0)
                {
                    logHelper.Waring("Delete错误，SQL语句：{0}", sql);
                }
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
        }
        public static void Delete<T>(Expression<Func<T, bool>> expression) where T : IKey<Guid>, new()
        {
            //DataContext dbContext = new DataContext(new OleDbConnection(jsonHelper.ReadJsonByString("ConnectionString")));
            //dbContext.Log = Console.Out;
            //Table<QQUSER> qquserTable = dbContext.GetTable<QQUSER>();
            IList<T> res = default(IList<T>);
            var t = typeof(T);
            try
            {
                res = Query<T>().Where(expression).ToList();
                StringBuilder strb = new StringBuilder();
                foreach (var item in res)
                {
                    strb.AppendFormat("DELETE FROM [{0}] WHERE [ID] = {1};", t.Name, t.GetProperty("ID").GetValue(item));
                    if (accessTableHelper.ExecuteNonQuery(strb.ToString()) == 0)
                    {
                        logHelper.Waring("Delete错误，SQL语句：{0}", strb.ToString());
                    }
                    strb.Clear();
                }
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
        }
        public static void Delete<T>(this T entity, Expression<Func<T, bool>> expression) where T : IKey<Guid>, new()
        {
            //DataContext dbContext = new DataContext(new OleDbConnection(jsonHelper.ReadJsonByString("ConnectionString")));
            //dbContext.Log = Console.Out;
            //Table<QQUSER> qquserTable = dbContext.GetTable<QQUSER>();
            IList<T> res = default(IList<T>);
            var t = typeof(T);
            try
            {
                res = Query<T>().Where(expression).ToList();
                StringBuilder strb = new StringBuilder();
                foreach (var item in res)
                {
                    strb.AppendFormat("DELETE FROM [{0}] WHERE [ID] = {1};", t.Name, t.GetProperty("ID").GetValue(item));
                    if (accessTableHelper.ExecuteNonQuery(strb.ToString()) == 0)
                    {
                        logHelper.Waring("Delete错误，SQL语句：{0}", strb.ToString());
                    }
                    strb.Clear();
                }
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
        }
        public static T Get<T>(Expression<Func<T, bool>> expression) where T : IKey<Guid>, new()
        {
            T res = default(T);
            try
            {
                res = Query<T>().FirstOrDefault(expression);
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
            return res;
        }
        public static T Get<T>(this T entity, Expression<Func<T, bool>> expression) where T : IKey<Guid>, new()
        {
            T res = default(T);
            try
            {
                res = Query<T>().FirstOrDefault(expression);
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
            return res;
        }
        public static IList<T> GetAll<T>(Expression<Func<T, bool>> expression) where T : IKey<Guid>, new()
        {
            IList<T> res = default(IList<T>);
            try
            {
                res = Query<T>().Where(expression).ToList();
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
            return res;
        }
        public static IList<T> GetAll<T>(this T entity, Expression<Func<T, bool>> expression) where T : IKey<Guid>, new()
        {
            IList<T> res = default(IList<T>);
            try
            {
                res = Query<T>().Where(expression).ToList();
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
            return res;
        }
    }
}
