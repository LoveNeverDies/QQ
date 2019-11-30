/* 
 * Newbe.Mahua.HttpApi
 *
 * this is http api document for Newbe.Mahua. You can get some help from http://www.newbe.pro
 *
 * OpenAPI spec version: v1
 * Contact: 472158246@qq.com
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using SwaggerDateConverter = Newbe.Mahua.HttpApiClient.Client.SwaggerDateConverter;

namespace Newbe.Mahua.HttpApiClient.Model
{
    /// <summary>
    /// 发送赞
    /// </summary>
    [DataContract]
    public partial class CqpCQSendLikeV2HttpInput :  IEquatable<CqpCQSendLikeV2HttpInput>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CqpCQSendLikeV2HttpInput" /> class.
        /// </summary>
        /// <param name="qqid">目标QQ.</param>
        /// <param name="times">赞的次数，最多10次.</param>
        public CqpCQSendLikeV2HttpInput(long? qqid = default(long?), int? times = default(int?))
        {
            this.Qqid = qqid;
            this.Times = times;
        }
        
        /// <summary>
        /// 目标QQ
        /// </summary>
        /// <value>目标QQ</value>
        [DataMember(Name="qqid", EmitDefaultValue=false)]
        public long? Qqid { get; set; }

        /// <summary>
        /// 赞的次数，最多10次
        /// </summary>
        /// <value>赞的次数，最多10次</value>
        [DataMember(Name="times", EmitDefaultValue=false)]
        public int? Times { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CqpCQSendLikeV2HttpInput {\n");
            sb.Append("  Qqid: ").Append(Qqid).Append("\n");
            sb.Append("  Times: ").Append(Times).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as CqpCQSendLikeV2HttpInput);
        }

        /// <summary>
        /// Returns true if CqpCQSendLikeV2HttpInput instances are equal
        /// </summary>
        /// <param name="input">Instance of CqpCQSendLikeV2HttpInput to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CqpCQSendLikeV2HttpInput input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Qqid == input.Qqid ||
                    (this.Qqid != null &&
                    this.Qqid.Equals(input.Qqid))
                ) && 
                (
                    this.Times == input.Times ||
                    (this.Times != null &&
                    this.Times.Equals(input.Times))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.Qqid != null)
                    hashCode = hashCode * 59 + this.Qqid.GetHashCode();
                if (this.Times != null)
                    hashCode = hashCode * 59 + this.Times.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
