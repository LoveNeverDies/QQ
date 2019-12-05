using Newbe.Mahua.Plugins.Parrot.HelperService;
using Newbe.Mahua.Plugins.Parrot.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Newbe.Mahua.Plugins.Parrot.Helper
{
    public static class EntityHelper
    {
        //static ISqlServerTableHelper sqlServerTableHelper = new SqlServerTableHelper();
        static ISqlServerTableHelper sqlServerTableHelper = new SqlServerTableHelper();
        static ILogHelper logHelper = new LogHelper();
        static IJsonHelper jsonHelper = new JsonHelper();

        #region Lambda表达式转换为SQL WHERE 条件
        public class ToSqlFormat : Attribute
        {
            public string Format { get; set; }
            public ToSqlFormat(string str)
            {
                Format = str;
            }
        }

        /// <summary>
        /// 从表达式获取sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        static string GetSqlFromExpression<T>(Expression<Func<T, bool>> func)
        {
            if (func != null && func.Body is BinaryExpression be)
            {
                return BinarExpressionProvider(be.Left, be.Right, be.NodeType);
            }
            else
            {
                return " ( 1 = 1 ) ";
            }
        }

        /// <summary>
        /// 拆分、拼接sql
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        static string BinarExpressionProvider(Expression left, Expression right, ExpressionType type)
        {
            string sb = "(";
            sb += ExpressionRouter(left);
            sb += ExpressionTypeCast(type);
            string tmpStr = ExpressionRouter(right);
            if (tmpStr == "null")
            {
                if (sb.EndsWith(" = ")) sb = sb.Substring(0, sb.Length - 2) + " is null";
                else if (sb.EndsWith(" <> ")) sb = sb.Substring(0, sb.Length - 2) + " is not null";
            }
            else sb += tmpStr;
            return sb += ")";
        }

        /// <summary>
        /// 拆分、拼接sql
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        static string ExpressionRouter(Expression exp)
        {
            string sb = string.Empty;
            if (exp is BinaryExpression be)
            {
                return BinarExpressionProvider(be.Left, be.Right, be.NodeType);
            }
            else if (exp is MemberExpression me)
            {
                return me.Member.Name;
            }
            else if (exp is NewArrayExpression ae)
            {
                StringBuilder tmpstr = new StringBuilder();
                foreach (Expression ex in ae.Expressions)
                {
                    tmpstr.Append(ExpressionRouter(ex));
                    tmpstr.Append(",");
                }
                return tmpstr.ToString(0, tmpstr.Length - 1);
            }
            else if (exp is MethodCallExpression mce)
            {
                var attributeData = mce.Method.GetCustomAttributes(typeof(ToSqlFormat), false).First();
                return string.Format(((ToSqlFormat)attributeData).Format, ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));
            }
            else if (exp is ConstantExpression ce)
            {
                if (ce.Value == null)
                    return "null";
                else if (ce.Value is ValueType)
                    return ce.Value.ToString();
                else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
                    return string.Format("'{0}'", ce.Value.ToString());
            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);
                return ExpressionRouter(ue.Operand);
            }

            return null;
        }

        /// <summary>
        /// 介绍表达式树节点的节点类型 转换为 sql关键字
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static string ExpressionTypeCast(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.And:
                    return " & ";
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return " = ";
                case ExpressionType.GreaterThan:
                    return " >";
                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";
                case ExpressionType.LessThan:
                    return " < ";
                case ExpressionType.LessThanOrEqual:
                    return " <= ";
                case ExpressionType.NotEqual:
                    return " <> ";
                case ExpressionType.Or:
                    return " | ";
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return " + ";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return " - ";
                case ExpressionType.Divide:
                    return " / ";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return " * ";
                default:
                    return null;
            }
        }

        [ToSqlFormat("{0} IN ({1})")]
        public static bool In<T>(this T obj, T[] array)
        {
            return true;
        }
        [ToSqlFormat("{0} NOT IN ({1})")]
        public static bool NotIn<T>(this T obj, T[] array)
        {
            return true;
        }
        [ToSqlFormat("{0} LIKE {1}")]
        public static bool Like(this string str, string likeStr)
        {
            return true;
        }
        [ToSqlFormat("{0} NOT LIKE {1}")]
        public static bool NotLike(this string str, string likeStr)
        {
            return true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Top1">只返回第一行</param>
        /// <returns></returns>
        static IQueryable<T> Query<T>(bool Top1 = false) where T : IKey<Guid>, new()
        {
            var type = typeof(T);
            return sqlServerTableHelper.ExecuteDataTable(string.Format("SELECT {0}* FROM {1} WHERE 1 = 1 ", Top1 ? "TOP 1 " : string.Empty, type.Name)).ToListModel<T>(Top1).AsQueryable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="Top1">只返回第一行</param>
        /// <returns></returns>
        static IQueryable<T> Query<T>(Expression<Func<T, bool>> expression, bool Top1 = false) where T : IKey<Guid>, new()
        {
            var type = typeof(T);
            return sqlServerTableHelper.ExecuteDataTable(string.Format("SELECT {0}* FROM {1} WHERE {2}", Top1 ? "TOP 1 " : string.Empty, type.Name, GetSqlFromExpression(expression))).ToListModel<T>(Top1).AsQueryable();
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
                    case "ID":
                        if (val.ToGuid() == Guid.Empty)
                            val = Guid.NewGuid();
                        strSelect.AppendFormat("SELECT * FROM [{0}] WHERE [ID] = '{1}';", entityType.Name, val);
                        break;
                    case "CREATETIME":
                    case "UPDATETIME":
                        val = DateTime.Now;
                        break;
                    default:
                        break;
                }
                //值为空并且modifyNull是false（表示不处理为空的值） 或者参数名是id 则不参与拼接sql
                if ((val == null && modifyNull == false))
                    continue;
                if (JudgeValue(item.PropertyType.FullName, val))
                    continue;
                if (item.PropertyType.BaseType.Name == "Enum")
                {
                    //d表示转换为数字
                    val = Enum.Format(item.PropertyType, val, "d");
                }
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
                if (sqlServerTableHelper.ExecuteNonQuery(strKey.ToString()) == 0)
                {
                    logHelper.Waring(string.Format("Insert错误，SQL语句：{0}", strKey.ToString()));
                }
                entity = sqlServerTableHelper.ExecuteDataTable(strSelect.ToString()).ToListModel<T>(true)[0];
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
                    if (key == "ID")
                    {
                        ID = val.ToGuid();
                        strSelect.AppendFormat("SELECT * FROM [{0}] WHERE [ID] = '{1}';", entityType.Name, val);
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
            strSet.AppendFormat(" WHERE [ID] = '{0}';", ID);
            try
            {
                //执行sql
                if (sqlServerTableHelper.ExecuteNonQuery(strSet.ToString()) == 0)
                {
                    logHelper.Waring("Update错误，SQL语句：{0}", strSet.ToString());
                }
                entity = sqlServerTableHelper.ExecuteDataTable(strSelect.ToString()).ToListModel<T>(true)[0];
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
        public static int Delete(this string sql)
        {
            int resCount = 0;
            try
            {
                resCount = sqlServerTableHelper.ExecuteNonQuery(sql.ToString());
                if (resCount == 0)
                {
                    logHelper.Waring("Delete错误，SQL语句：{0}", sql);
                }
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
            return resCount;
        }

        public static int Delete<T>(this T entity) where T : IKey<Guid>, new()
        {
            int resCount = 0;
            var t = typeof(T);
            try
            {
                //t.GetProperties().GetValue()
                string sql = string.Format("DELETE FROM [{0}] WHERE [ID] = {1};", t.Name, t.GetProperty("ID").GetValue(entity));
                resCount = sqlServerTableHelper.ExecuteNonQuery(sql.ToString());
                if (resCount == 0)
                {
                    logHelper.Waring("Delete错误，SQL语句：{0}", sql);
                }
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
            return resCount;
        }

        public static int Delete<T>(Expression<Func<T, bool>> expression) where T : IKey<Guid>, new()
        {
            int resCount = 0;
            int tempCount = 0;
            IList<T> res = default(IList<T>);
            var t = typeof(T);
            try
            {
                res = Query<T>().Where(expression).ToList();
                StringBuilder strb = new StringBuilder();
                foreach (var item in res)
                {
                    strb.AppendFormat("DELETE FROM [{0}] WHERE [ID] = {1};", t.Name, t.GetProperty("ID").GetValue(item));
                    tempCount = sqlServerTableHelper.ExecuteNonQuery(strb.ToString());
                    resCount += tempCount;
                    if (tempCount == 0)
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
            return resCount;
        }

        public static int Delete<T>(this T entity, Expression<Func<T, bool>> expression) where T : IKey<Guid>, new()
        {
            int resCount = 0;
            int tempCount = 0;
            IList<T> res = default(IList<T>);
            var t = typeof(T);
            try
            {
                res = Query<T>().Where(expression).ToList();
                StringBuilder strb = new StringBuilder();
                foreach (var item in res)
                {
                    strb.AppendFormat("DELETE FROM [{0}] WHERE [ID] = {1};", t.Name, t.GetProperty("ID").GetValue(item));
                    tempCount = sqlServerTableHelper.ExecuteNonQuery(strb.ToString());
                    resCount += tempCount;
                    if (tempCount == 0)
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
            return resCount;
        }

        public static T Get<T>(this string sql) where T : IKey<Guid>, new()
        {
            T res = default(T);
            try
            {
                res = sqlServerTableHelper.ExecuteDataTable(sql).ToListModel<T>(true).FirstOrDefault();
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
            return res;
        }

        public static T Get<T>(Expression<Func<T, bool>> expression) where T : IKey<Guid>, new()
        {
            T res = default(T);
            try
            {
                res = Query<T>(expression, true).FirstOrDefault();
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
                res = Query<T>(expression, true).FirstOrDefault();
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
            return res;
        }

        public static IList<T> GetAll<T>(this string sql) where T : IKey<Guid>, new()
        {
            IList<T> res = default(IList<T>);
            try
            {
                res = sqlServerTableHelper.ExecuteDataTable(sql).ToListModel<T>();
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
                res = Query<T>(expression).ToList();
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
                res = Query<T>(expression).ToList();
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
            return res;
        }

    }
}
