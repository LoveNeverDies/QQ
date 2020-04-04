using Newbe.Mahua.Plugins.Parrot.HelperService;
using Newbe.Mahua.Plugins.Parrot.Model.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Newbe.Mahua.Plugins.Parrot.Helper
{
    /// <summary>
    /// Lambda表达式转换为SQL WHERE 条件
    /// </summary>
    namespace MaiCore
    {
        public class SqlParaModel
        {
            /// <summary>
            /// 
            /// </summary>
            public string name { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public object value { set; get; }
        }
        /// <summary>
        ///
        /// </summary>
        public class LambdaToSqlHelper
        {
            /// <summary>
            /// NodeType枚举
            /// </summary>
            private enum EnumNodeType
            {
                /// <summary>
                /// 二元运算符
                /// </summary>
                [Description("二元运算符")]
                BinaryOperator = 1,

                /// <summary>
                /// 一元运算符
                /// </summary>
                [Description("一元运算符")]
                UndryOperator = 2,

                /// <summary>
                /// 常量表达式
                /// </summary>
                [Description("常量表达式")]
                Constant = 3,

                /// <summary>
                /// 成员（变量）
                /// </summary>
                [Description("成员（变量）")]
                MemberAccess = 4,

                /// <summary>
                /// 函数
                /// </summary>
                [Description("函数")]
                Call = 5,

                /// <summary>
                /// 未知
                /// </summary>
                [Description("未知")]
                Unknown = -99,

                /// <summary>
                /// 不支持
                /// </summary>
                [Description("不支持")]
                NotSupported = -98
            }

            /// <summary>
            /// 判断表达式类型
            /// </summary>
            /// <param name="exp">lambda表达式</param>
            /// <returns></returns>
            private static EnumNodeType CheckExpressionType(Expression exp)
            {
                switch (exp.NodeType)
                {
                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                    case ExpressionType.Equal:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.LessThan:
                    case ExpressionType.NotEqual:
                        return EnumNodeType.BinaryOperator;
                    case ExpressionType.Constant:
                        return EnumNodeType.Constant;
                    case ExpressionType.MemberAccess:
                        return EnumNodeType.MemberAccess;
                    case ExpressionType.Call:
                        return EnumNodeType.Call;
                    case ExpressionType.Not:
                    case ExpressionType.Convert:
                        return EnumNodeType.UndryOperator;
                    default:
                        return EnumNodeType.Unknown;
                }
            }

            /// <summary>
            /// 表达式类型转换
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            private static string ExpressionTypeCast(ExpressionType type)
            {
                switch (type)
                {
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        return " and ";
                    case ExpressionType.Equal:
                        return " = ";
                    case ExpressionType.GreaterThan:
                        return " > ";
                    case ExpressionType.GreaterThanOrEqual:
                        return " >= ";
                    case ExpressionType.LessThan:
                        return " < ";
                    case ExpressionType.LessThanOrEqual:
                        return " <= ";
                    case ExpressionType.NotEqual:
                        return " <> ";
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        return " or ";
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

            private static string BinarExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
            {
                BinaryExpression be = exp as BinaryExpression;
                Expression left = be.Left;
                Expression right = be.Right;
                ExpressionType type = be.NodeType;
                string sb = "(";
                //先处理左边
                sb += ExpressionRouter(left, listSqlParaModel);
                sb += ExpressionTypeCast(type);
                //再处理右边
                string sbTmp = ExpressionRouter(right, listSqlParaModel);
                if (sbTmp == "null")
                {
                    if (sb.EndsWith(" = "))
                        sb = sb.Substring(0, sb.Length - 2) + " is null";
                    else if (sb.EndsWith(" <> "))
                        sb = sb.Substring(0, sb.Length - 2) + " is not null";
                }
                else
                    sb += sbTmp;
                return sb += ")";
            }

            private static string ConstantExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
            {
                ConstantExpression ce = exp as ConstantExpression;
                if (ce.Value == null)
                {
                    return "null";
                }
                else if (ce.Value is ValueType)
                {
                    GetSqlParaModel(listSqlParaModel, GetValueType(ce.Value));
                    return "@para" + listSqlParaModel.Count;
                }
                else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
                {
                    GetSqlParaModel(listSqlParaModel, GetValueType(ce.Value));
                    return "@para" + listSqlParaModel.Count;
                }
                return "";
            }

            private static string LambdaExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
            {
                LambdaExpression le = exp as LambdaExpression;
                return ExpressionRouter(le.Body, listSqlParaModel);
            }

            private static string MemberExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
            {
                if (!exp.ToString().StartsWith("value"))
                {
                    MemberExpression me = exp as MemberExpression;
                    if (me.Member.Name == "Now")
                    {
                        GetSqlParaModel(listSqlParaModel, DateTime.Now);
                        return "@para" + listSqlParaModel.Count;
                    }
                    return me.Member.Name;
                }
                else
                {
                    var result = Expression.Lambda(exp).Compile().DynamicInvoke();
                    if (result == null)
                    {
                        return "null";
                    }
                    else if (result is ValueType)
                    {
                        GetSqlParaModel(listSqlParaModel, GetValueType(result));
                        return "@para" + listSqlParaModel.Count;
                    }
                    else if (result is string || result is DateTime || result is char)
                    {
                        GetSqlParaModel(listSqlParaModel, GetValueType(result));
                        return "@para" + listSqlParaModel.Count;
                    }
                    else if (result is int[])
                    {
                        var rl = result as int[];
                        StringBuilder sbTmp = new StringBuilder();
                        foreach (var r in rl)
                        {
                            GetSqlParaModel(listSqlParaModel, r.ToString().ToInt32());
                            sbTmp.Append("@para" + listSqlParaModel.Count + ",");
                        }
                        return sbTmp.ToString().Substring(0, sbTmp.ToString().Length - 1);
                    }
                    else if (result is string[])
                    {
                        var rl = result as string[];
                        StringBuilder sbTmp = new StringBuilder();
                        foreach (var r in rl)
                        {
                            GetSqlParaModel(listSqlParaModel, r.ToString());
                            sbTmp.Append("@para" + listSqlParaModel.Count + ",");
                        }
                        return sbTmp.ToString().Substring(0, sbTmp.ToString().Length - 1);
                    }
                }
                return "";
            }

            private static string MethodCallExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
            {
                MethodCallExpression mce = exp as MethodCallExpression;
                if (mce.Method.Name == "Contains")
                {
                    if (mce.Object == null)
                    {
                        return string.Format("{0} in ({1})", ExpressionRouter(mce.Arguments[1], listSqlParaModel), ExpressionRouter(mce.Arguments[0], listSqlParaModel));
                    }
                    else
                    {
                        if (mce.Object.NodeType == ExpressionType.MemberAccess)
                        {
                            //w => w.name.Contains("1")
                            var _name = ExpressionRouter(mce.Object, listSqlParaModel);
                            var _value = ExpressionRouter(mce.Arguments[0], listSqlParaModel);
                            var index = listSqlParaModel.Count - 1;//_value.RetainNumber().ToInt32() - 1;
                            listSqlParaModel[index].value = string.Format("%{0}%", listSqlParaModel[index].value);
                            return string.Format("{0} like {1}", _name, _value);
                        }
                    }
                }
                else if (mce.Method.Name == "OrderBy")
                {
                    return string.Format("{0} asc", ExpressionRouter(mce.Arguments[1], listSqlParaModel));
                }
                else if (mce.Method.Name == "OrderByDescending")
                {
                    return string.Format("{0} desc", ExpressionRouter(mce.Arguments[1], listSqlParaModel));
                }
                else if (mce.Method.Name == "ThenBy")
                {
                    return string.Format("{0},{1} asc", MethodCallExpressionProvider(mce.Arguments[0], listSqlParaModel), ExpressionRouter(mce.Arguments[1], listSqlParaModel));
                }
                else if (mce.Method.Name == "ThenByDescending")
                {
                    return string.Format("{0},{1} desc", MethodCallExpressionProvider(mce.Arguments[0], listSqlParaModel), ExpressionRouter(mce.Arguments[1], listSqlParaModel));
                }
                else if (mce.Method.Name == "Like")
                {
                    return string.Format("({0} like {1})", ExpressionRouter(mce.Arguments[0], listSqlParaModel), ExpressionRouter(mce.Arguments[1], listSqlParaModel).Replace("‘", ""));
                }
                else if (mce.Method.Name == "NotLike")
                {
                    return string.Format("({0} not like ‘%{1}%‘)", ExpressionRouter(mce.Arguments[0], listSqlParaModel), ExpressionRouter(mce.Arguments[1], listSqlParaModel).Replace("‘", ""));
                }
                else if (mce.Method.Name == "In")
                {
                    return string.Format("{0} in ({1})", ExpressionRouter(mce.Arguments[0], listSqlParaModel), ExpressionRouter(mce.Arguments[1], listSqlParaModel));
                }
                else if (mce.Method.Name == "NotIn")
                {
                    return string.Format("{0} not in ({1})", ExpressionRouter(mce.Arguments[0], listSqlParaModel), ExpressionRouter(mce.Arguments[1], listSqlParaModel));
                }
                return "";
            }

            private static string NewArrayExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
            {
                NewArrayExpression ae = exp as NewArrayExpression;
                StringBuilder sbTmp = new StringBuilder();
                foreach (Expression ex in ae.Expressions)
                {
                    sbTmp.Append(ExpressionRouter(ex, listSqlParaModel));
                    sbTmp.Append(",");
                }
                return sbTmp.ToString(0, sbTmp.Length - 1);
            }

            private static string ParameterExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
            {
                ParameterExpression pe = exp as ParameterExpression;
                return pe.Type.Name;
            }

            private static string UnaryExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
            {
                UnaryExpression ue = exp as UnaryExpression;
                var result = ExpressionRouter(ue.Operand, listSqlParaModel);
                ExpressionType type = exp.NodeType;
                if (type == ExpressionType.Not)
                {
                    if (result.Contains(" in "))
                    {
                        result = result.Replace(" in ", " not in ");
                    }
                    if (result.Contains(" like "))
                    {
                        result = result.Replace(" like ", " not like ");
                    }
                }
                return result;
            }

            /// <summary>
            /// 路由计算
            /// </summary>
            /// <param name="exp"></param>
            /// <param name="listSqlParaModel"></param>
            /// <returns></returns>
            private static string ExpressionRouter(Expression exp, List<SqlParaModel> listSqlParaModel)
            {
                var nodeType = exp.NodeType;
                if (exp is BinaryExpression)    //表示具有二进制运算符的表达式
                {
                    return BinarExpressionProvider(exp, listSqlParaModel);
                }
                else if (exp is ConstantExpression) //表示具有常数值的表达式
                {
                    return ConstantExpressionProvider(exp, listSqlParaModel);
                }
                else if (exp is LambdaExpression)   //介绍 lambda 表达式。 它捕获一个类似于 .NET 方法主体的代码块
                {
                    return LambdaExpressionProvider(exp, listSqlParaModel);
                }
                else if (exp is MemberExpression)   //表示访问字段或属性
                {
                    return MemberExpressionProvider(exp, listSqlParaModel);
                }
                else if (exp is MethodCallExpression)   //表示对静态方法或实例方法的调用
                {
                    return MethodCallExpressionProvider(exp, listSqlParaModel);
                }
                else if (exp is NewArrayExpression) //表示创建一个新数组，并可能初始化该新数组的元素
                {
                    return NewArrayExpressionProvider(exp, listSqlParaModel);
                }
                else if (exp is ParameterExpression)    //表示一个命名的参数表达式。
                {
                    return ParameterExpressionProvider(exp, listSqlParaModel);
                }
                else if (exp is UnaryExpression)    //表示具有一元运算符的表达式
                {
                    return UnaryExpressionProvider(exp, listSqlParaModel);
                }
                return null;
            }

            /// <summary>
            /// 值类型转换
            /// </summary>
            /// <param name="_value"></param>
            /// <returns></returns>
            private static object GetValueType(object _value)
            {
                var _type = _value.GetType().Name;
                switch (_type)
                {
                    case "Decimal ": return _value.ToDecimal();
                    case "Int32": return _value.ToInt32();
                    case "DateTime": return _value.ToDateTime();
                    case "String": return _value.ToString();
                    case "Char": return _value.ToChar();
                    case "Boolean": return _value.ToBoolean();
                    default: return _value;
                }
            }

            /// <summary>
            /// sql参数
            /// </summary>
            /// <param name="listSqlParaModel"></param>
            /// <param name="val"></param>
            private static void GetSqlParaModel(List<SqlParaModel> listSqlParaModel, object val)
            {
                SqlParaModel p = new SqlParaModel();
                p.name = "para" + (listSqlParaModel.Count + 1);
                p.value = val;
                listSqlParaModel.Add(p);
            }

            /// <summary>
            /// lambda表达式转换sql
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="where"></param>
            /// <param name="listSqlParaModel"></param>
            /// <returns></returns>
            public static string GetWhereSql<T>(Expression<Func<T, bool>> where, List<SqlParaModel> listSqlParaModel) where T : IKey<Guid>, new()
            {
                string result = string.Empty;
                if (where != null)
                {
                    Expression exp = where.Body as Expression;
                    result = ExpressionRouter(exp, listSqlParaModel);
                }
                if (result != string.Empty)
                {
                    result = " WHERE " + result;
                }
                return result;
            }

            /// <summary>
            /// lambda表达式转换sql
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="orderBy"></param>
            /// <returns></returns>
            public static string GetOrderBySql<T>(Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderBy) where T : class
            {
                string result = string.Empty;
                if (orderBy != null && orderBy.Body is MethodCallExpression)
                {
                    MethodCallExpression exp = orderBy.Body as MethodCallExpression;
                    List<SqlParaModel> listSqlParaModel = new List<SqlParaModel>();
                    result = MethodCallExpressionProvider(exp, listSqlParaModel);
                }
                if (result != string.Empty)
                {
                    result = " order by " + result;
                }
                return result;
            }

            /// <summary>
            /// lambda表达式转换sql
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="fields"></param>
            /// <returns></returns>
            public static string GetQueryField<T>(Expression<Func<T, object>> fields)
            {
                StringBuilder sbSelectFields = new StringBuilder();
                if (fields.Body is NewExpression)
                {
                    NewExpression ne = fields.Body as NewExpression;
                    for (var i = 0; i < ne.Members.Count; i++)
                    {
                        sbSelectFields.Append(ne.Members[i].Name + ",");
                    }
                }
                else if (fields.Body is ParameterExpression)
                {
                    sbSelectFields.Append("*");
                }
                else
                {
                    sbSelectFields.Append("*");
                }
                if (sbSelectFields.Length > 1)
                {
                    sbSelectFields = sbSelectFields.Remove(sbSelectFields.Length - 1, 1);
                }
                return sbSelectFields.ToString();
            }

        }
    }
    public static class EntityHelper
    {
        static ISQLServerTableHelper sqlServerTableHelper = new SQLServerTableHelper();
        static ILogHelper logHelper = new LogHelper();
        static IJsonHelper jsonHelper = new JsonHelper();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Top1">只返回第一行</param>
        /// <returns></returns>
        static IQueryable<T> Query<T>(bool Top1 = false) where T : IKey<Guid>, new()
        {
            var type = typeof(T);
            return sqlServerTableHelper.ExecuteDataTable(string.Format("SELECT {0}* FROM {1};", Top1 ? "TOP 1 " : string.Empty, type.Name)).ToListModel<T>().AsQueryable();
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
            List<MaiCore.SqlParaModel> sqlParas = new List<MaiCore.SqlParaModel>();
            var sql = MaiCore.LambdaToSqlHelper.GetWhereSql(expression, sqlParas);
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParas.ForEach(p =>
            {
                sqlParameters.Add(new SqlParameter("@" + p.name, p.value));
            });
            var type = typeof(T);
            return sqlServerTableHelper.ExecuteDataTable(string.Format("SELECT {0}* FROM {1}{2};", Top1 ? "TOP 1 " : string.Empty, type.Name, sql), sqlParameters.ToArray()).ToListModel<T>().AsQueryable();
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
                    if (val.ToDateTime() == DateTime.MinValue)
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
                        val = DateTime.Now;
                        break;
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
                entity = sqlServerTableHelper.ExecuteDataTable(strSelect.ToString()).ToListModel<T>().FirstOrDefault();
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
                if (key == "UPDATETIME")
                {
                    val = DateTime.Now;
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
                entity = sqlServerTableHelper.ExecuteDataTable(strSelect.ToString()).ToListModel<T>().FirstOrDefault();
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
            try
            {
                resCount = Delete(expression);
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
            return resCount;
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
                res = GetAll(expression);
                //res = Query<T>(expression).ToList();
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
            return res;
        }

        /// <summary>
        /// 增删改
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int ExecuteSQL(this string sql, params SqlParameter[] parameters)
        {
            int resCount = 0;
            try
            {
                resCount = sqlServerTableHelper.ExecuteNonQuery(sql, parameters);
                if (resCount == 0)
                {
                    logHelper.Waring("{1}方法返回影响行数为0，SQL语句为：{0}", sql, nameof(ExecuteSQL));
                }
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
            return resCount;
        }

        /// <summary>
        /// 增删改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IList<T> ExecuteSQL<T>(this string sql, params SqlParameter[] parameters) where T : IKey<Guid>, new()
        {
            IList<T> res = default(IList<T>);
            try
            {
                res = sqlServerTableHelper.ExecuteDataTable(sql, parameters).ToListModel<T>();
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
            return res;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IList<T> SearchSQL<T>(this string sql, params SqlParameter[] parameters) where T : IKey<Guid>, new()
        {
            IList<T> res = default(IList<T>);
            try
            {
                res = sqlServerTableHelper.ExecuteDataTable(sql, parameters).ToListModel<T>();
            }
            catch (Exception e)
            {
                logHelper.Error(e.Message);
            }
            return res;
        }
    }
}
