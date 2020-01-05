using Newbe.Mahua.Plugins.Parrot.HelperService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Newbe.Mahua.Plugins.Parrot.Helper
{
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }
    }

    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 字段名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字段描述
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public ColumnType Type { get; set; }

        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否是外键 虽然还没有实现这个功能 但是字段先保留
        /// </summary>
        public bool IsForeignKey { get; set; }

        /// <summary>
        /// 可以为空
        /// </summary>
        public bool CanBeNull { get { return true; } set { this.CanBeNull = value; } }
    }

    public enum ColumnType
    {
        /// <summary>
        /// GUID
        /// </summary>
        UNIQUEIDENTIFIER,
        /// <summary>
        /// 时间类型
        /// </summary>
        DATETIME,
        /// <summary>
        /// LONG类型
        /// </summary>
        BIGINT,
        /// <summary>
        /// 字符串类型 长度为100
        /// </summary>
        NVARCHAR100,
        /// <summary>
        /// 字符串类型
        /// </summary>
        TEXT,
        /// <summary>
        /// INT类型
        /// </summary>
        INT,
        /// <summary>
        /// BIT类型
        /// </summary>
        BIT
    }

    class TableColumnModel
    {
        /// <summary>
        /// 字段名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字段描述
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否是外键
        /// </summary>
        public bool IsForeignKey { get; set; }

        /// <summary>
        /// 可以为空
        /// </summary>
        public string CanBeNull { get; set; }
    }

    public class GenerateTableHelper
    {
        ISQLServerTableHelper helper = null;
        StringBuilder strbuilderSQL = null;
        StringBuilder strbuilderDescribe = null;
        public GenerateTableHelper()
        {
            helper = new SQLServerTableHelper();
            strbuilderSQL = new StringBuilder();
            strbuilderDescribe = new StringBuilder();
        }
        string GetColumnType(ColumnType type)
        {
            string res = string.Empty;
            switch (type)
            {
                case ColumnType.UNIQUEIDENTIFIER:
                    res = "UNIQUEIDENTIFIER";
                    break;
                case ColumnType.DATETIME:
                    res = "DATETIME";
                    break;
                case ColumnType.BIGINT:
                    res = "BIGINT";
                    break;
                case ColumnType.NVARCHAR100:
                    res = "NVARCHAR(100)";
                    break;
                case ColumnType.INT:
                    res = "INT";
                    break;
                case ColumnType.BIT:
                    res = "BIT";
                    break;
                default:
                    res = "TEXT";
                    break;
            }
            return res;
        }

        /// <summary>
        /// 如果用户没有给字段添加TYPE属性，则反射字段的类型处理
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetEntityType(string type)
        {
            string res = string.Empty;
            switch (type)
            {
                case "DateTime":
                    res = "DATETIME";
                    break;
                case "Int32":
                case "Enum":
                    res = "INT";
                    break;
                case "Int64":
                    res = "BIGINT";
                    break;
                case "Guid":
                    res = "UNIQUEIDENTIFIER";
                    break;
                case "Boolean":
                    res = "BIT";
                    break;
                default:
                    res = "TEXT";
                    break;
            }
            return res;
        }

        IList<TableColumnModel> GetTable<T>()
        {
            Type t = typeof(T);
            PropertyInfo[] propertyInfos = t.GetProperties();
            IList<TableColumnModel> tableEntities = new List<TableColumnModel>();
            propertyInfos.ToList().ForEach(property =>
            {
                IList<CustomAttributeData> list = property.GetCustomAttributesData();
                if (list.Count == 1)
                {
                    tableEntities.Add(GetTableEntity(list[0].NamedArguments, property));
                }
                else
                {

                }
            });
            return tableEntities;
        }

        /// <summary>
        /// 构造sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public GenerateTableHelper StructureSQL<T>()
        {
            var res = GetTable<T>();
            /*
             CREATE TABLE [dbo].[QQUSER] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [CREATETIME]        DATETIME         NOT NULL,
    [UPDATETIME]        DATETIME         NOT NULL,
    [BREAK]             NVARCHAR (50)    NULL,
    [QQUSER_QQID]       BIGINT           NOT NULL,
    [QQUSER_QQQID]      BIGINT           NOT NULL,
    [QQUSER_EXPERIENCE] BIGINT           NOT NULL,
    CONSTRAINT [PK__QQUSER__3214EC072AD5732C] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'QQ号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'QQUSER', @level2type = N'COLUMN', @level2name = N'QQUSER_QQID';
             */
            object tableName = new object();
            try
            {
                tableName = typeof(T).GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            }
            catch
            {
                throw new Exception("当前类没有添加Table属性");
            }
            strbuilderSQL.AppendLine("CREATE TABLE [dbo].[{0}] (", tableName);
            for (int i = 0; i < res.Count; i++)
            {
                var item = res[i];
                strbuilderSQL.AppendLine("[{0}] {1} {2} {3}{4}", item.Name, item.Type, item.CanBeNull, item.IsPrimaryKey ? "PRIMARY KEY" : string.Empty, i == res.Count - 1 ? ")" : ",");
                //不支持开头加GO 在后面加分号吧
                strbuilderDescribe.AppendLine(@"
                EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'{0}', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'{1}', @level2type = N'COLUMN', @level2name = N'{2}'{3}", item.Describe, tableName, item.Name, ";");
            }
            strbuilderSQL.AppendLine(strbuilderDescribe.ToString());
            strbuilderDescribe.Clear();
            return this;
        }

        /// <summary>
        /// 删除数据库
        /// 删除数据库
        /// </summary>
        /// <typeparam name="T">表名</typeparam>
        /// <returns></returns>
        public GenerateTableHelper DropTableSQL<T>()
        {
            var tableName = typeof(T).GetCustomAttributesData()[0].NamedArguments[0].TypedValue.Value;
            strbuilderSQL.AppendFormat(@"
            IF Exists(SELECT TOP 1 * FROM sysObjects WHERE Id=OBJECT_ID(N'{0}') AND xtype='U')
            DROP TABLE [dbo].[{0}]", tableName);
            SubmitSQLServer();
            return this;
        }

        /// <summary>
        /// 然后提交
        /// </summary>
        public void SubmitSQLServer()
        {
            helper.ExecuteNonQuery(strbuilderSQL.ToString());
            strbuilderSQL.Clear();
            strbuilderDescribe.Clear();
        }
        TableColumnModel GetTableEntity(IList<CustomAttributeNamedArgument> attributeNamedArguments, PropertyInfo property)
        {
            TableColumnModel res = new TableColumnModel();
            attributeNamedArguments.ToList().ForEach(arg =>
            {
                switch (arg.MemberInfo.Name)
                {
                    case "Type":
                        res.Type = GetColumnType((ColumnType)arg.TypedValue.Value);
                        break;
                    case "CanBeNull":
                        res.CanBeNull = arg.TypedValue.Value.ToBoolean() ? "NULL" : "NOT NULL";
                        break;
                    case "Describe":
                        res.Describe = arg.TypedValue.Value.ToString();
                        break;
                    case "IsPrimaryKey":
                        res.IsPrimaryKey = arg.TypedValue.Value.ToBoolean();
                        break;
                    case "IsForeignKey":
                        res.IsForeignKey = arg.TypedValue.Value.ToBoolean();
                        break;
                    case "Name":
                        res.Name = arg.TypedValue.Value.ToString();
                        break;
                    default:
                        break;
                }
            });
            if (string.IsNullOrWhiteSpace(res.Name))
            {
                res.Name = property.Name;
            }
            if (string.IsNullOrWhiteSpace(res.Describe))
            {
                res.Describe = property.Name;
            }
            if (string.IsNullOrWhiteSpace(res.Type))
            {
                res.Type = GetEntityType(property.PropertyType.Name);
            }
            if (string.IsNullOrWhiteSpace(res.CanBeNull))
            {
                res.CanBeNull = "NULL";
            }
            return res;
        }
    }
}
