using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;

namespace OrderStockManager
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // アプリケーションのスタートアップで実行するコードです
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

#if GLOBAL_DEBUG
            System.Diagnostics.Debug.WriteLine("Global : アプリケーションの開始");
            this.Application["Count"] = 0;
#endif
        }

        void Application_End(object sender, EventArgs e)
        {
            // アプリケーションのシャットダウンで実行するコードです

#if GLOBAL_DEBUG
            System.Diagnostics.Debug.WriteLine("Global : アプリケーションの終了");
#endif
        }

        void Application_Error(object sender, EventArgs e)
        {
            // ハンドルされていないエラーが発生したときに実行するコードです

#if GLOBAL_DEBUG
            // 発生した例外をクリアしてプログラムコードを続行
            System.Diagnostics.Debug.WriteLine("Global : 例外の発生");
            // Application_Errorイベントハンドラでは続けて表示するWebページを指定する必要がある。
            // 表示するWebページはWeb.Configファイルで指定することもできる。
            // this.Server.Transfer("Default.aspx");
#endif
        }

        void Session_Start(object sender, EventArgs e)
        {
            // 新規セッションを開始したときに実行するコードです

#if GLOBAL_DEBUG
            System.Diagnostics.Debug.WriteLine("Global : セッションの開始");
            this.Application.Lock();
            this.Application["Count"] = (int)this.Application["Count"] + 1;
            this.Application.UnLock();
            System.Diagnostics.Debug.WriteLine("Global : セッション数 : " + this.Application["Count"].ToString());
#endif
        }

        void Session_End(object sender, EventArgs e)
        {
            // セッションが終了したときに実行するコードです

#if GLOBAL_DEBUG
            // メモ: Web.config ファイル内で sessionstate モードが InProc に設定されているときのみ、
            // Session_End イベントが発生します。session モードが StateServer か、または SQLServer に
            // 設定されている場合、イベントは発生しません。
            System.Diagnostics.Debug.WriteLine("セッションの終了");
            this.Application.Lock();
            this.Application["Count"] = (int)this.Application["Count"] - 1;
            this.Application.UnLock();
            System.Diagnostics.Debug.WriteLine("Global : セッション数 : " + this.Application["Count"].ToString());
#endif
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
#if GLOBAL_DEBUG
            System.Diagnostics.Debug.WriteLine("Global : 要求の開始");
#endif
        }

        void Application_EndRequest(object sender, EventArgs e)
        {
#if GLOBAL_DEBUG
            System.Diagnostics.Debug.WriteLine("Global : 要求の終了");
#endif
        }

        void Application_AuthenticateRequest(object sender, EventArgs e)
        {
#if GLOBAL_DEBUG
            System.Diagnostics.Debug.WriteLine("Global : 認証要求の発生");
#endif
        }
    }
}
