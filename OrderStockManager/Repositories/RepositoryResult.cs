using Microsoft.AspNet.Identity;
using OrderStockManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace OrderStockManager.Repositories
{
    /// <summary>
    /// リポジトリの結果返却用クラス
    /// </summary>
    /// <typeparam name="T">結果モデル</typeparam>
    // public class RepositoryResult<T> where T : BaseModel
    public class RepositoryResult<T> where T : class
    {
        public RepositoryResult() { }

        public RepositoryResult(HttpStatusCode code, string message = "")
        {
            this.Code = code;
            if (!string.IsNullOrEmpty(message))
                this.message = message;
        }

        public RepositoryResult(int code, string message="")
        {
            this.code = code;
            if (!string.IsNullOrEmpty(message))
                this.message = message;
        }

        public RepositoryResult(HttpStatusCode code, T data)
        {
            this.Code = code;
            this.resultData = data;
        }

        public RepositoryResult(int code, T data)
        {
            this.code = code;
            this.resultData = data;
        }

        /// <summary>
        /// 状態コード(基本HttpStatucCodeを基準)
        /// </summary>
        public int code;

        /// <summary>
        /// 状態メッセージ(任意)
        /// </summary>
        public string message;

        /// <summary>
        /// IdentityResultを格納
        /// </summary>
        public IdentityResult identityResult;

        /// <summary>
        /// 結果情報のオブジェクトを格納
        /// </summary>
        public T resultData;

        /// <summary>
        /// 状態コードをHttpStatusCodeとして扱う
        /// </summary>
        public HttpStatusCode Code
        {
            set => code = (int)value;
            get => (HttpStatusCode)Enum.ToObject(typeof(HttpStatusCode), code);
        }

        /// <summary>
        /// 状態コードをstringとして扱う
        /// </summary>
        public string CodeString
        {
            set => code = (int)Enum.Parse(typeof(HttpStatusCode), value, true);
            get => Enum.GetName(typeof(HttpStatusCode), code);
        }
    }
}
