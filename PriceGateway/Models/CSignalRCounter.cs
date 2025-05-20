using System.Collections.Concurrent;

namespace PriceGateway.Models
{
    public class CSignalRCounter
    {
        private DateTime m_ConnectedDateTime = DateTime.Now;
        public const string FORMAT_DATETIME = "yyyy/MM/dd HH:mm:ss";
        public const string SERVERVARIABLES_LOCAL_ADDR = "LOCAL_ADDR";
        public const string SERVERVARIABLES_REMOTE_ADDR = "REMOTE_ADDR";
        public const string SERVERVARIABLES_HTTP_USER_AGENT = "HTTP_USER_AGENT";
        public const string SERVERVARIABLES_HTTP_COOKIE = "HTTP_COOKIE";
        public const string IP_SERVER_PREFIX = "172";

        //---------------------------------------------------------------------------
        // (A) SignalR connected la co info ngay
        public string ConnectedDateTime                 // 2016/08/21 23:44:51
        {
            get { return this.m_ConnectedDateTime.ToString(FORMAT_DATETIME); }
        }
        public string ConnectionID { get; set; }        // 6728041B-C063-494F-997F-6D4C591C9A26
        public string TransportName { get; set; }       // serverSentEvents
        public string InitHubTime { get; set; }         // 5783

        //---------------------------------------------------------------------------
        // (B) ajax pull sau do update thi lay thong tin qua Request.ServerVariables collection
        public string ServerIP { get; set; }            // (LOCAL_ADDR)         172.16.0.51 
        public string ClientPublicIP { get; set; }      // (REMOTE_ADDR)        210.245.49.17 
        public string HttpUserAgent { get; set; }       // (HTTP_USER_AGENT)    Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36
        public string HttpCookie { get; set; }          // (HTTP_COOKIE)        _ga=GA1.3.1935334339.1456366431; top=aHNXIndex_; showhiden=4,20; hideChart=VN100,VNALL,VNMID,VNSML,HNX30TRI,HNXLCAP,HNXSMCAP,HNXFIN,HNXMAN,HNXCON; menu=aHNXIndex; __utma=206370372.872846905.1456719975.1470800184.1471489385.61; __utmz=206370372.1469967227.50.7.utmcsr=ezdiscuss.fpts.com.vn|utmccn=(referral)|utmcmd=referral|utmcct=/; ASP.NET_SessionId=semjozkjsbdmwq1z0aeqmlti; aspfpt_sessiontoken=32442d12-0575-4ad9-a47a-7da786304346

        //---------------------------------------------------------------------------
        // (C) ajax pull xu ly phia tren server thi check AuthenCookie => info cua khach
        public string strRedirectURL { get; set; }      // http://accounts.fpts.com.vn/?href=liveprice2
        public string strLoginName { get; set; }        // 058C108101
        public string strClientCode { get; set; }       // 108101
        public string strClientName { get; set; }       // DANG THI THUY HOA
        public string strSessionNo { get; set; }        // 058C10810120160822164128964499
        public string strReturnCode { get; set; }       // 0
        public string strReturnMess { get; set; }       // CONFIRM SUCCESS
        public string strAccountStatus { get; set; }    // 0
        public string CultureName { get; set; }         // vi-VN/en-US/ja-JP
        public string CultureShort { get; set; }        // VN/EN/JP
        public string CultureIndex { get; set; }        // 0/1/2
        //---------------------------------------------------------------------------
        // (D) ajax pull dung js lay thong tin tai client va send cho server
        public string OS { get; set; }                  // Windows 7
        public string Browser { get; set; }             // Chrome 52 (52.0.2743.116)
        public string Mobile { get; set; }              // false
        public string Screen { get; set; }              // 1920 x 1080
        public string Page { get; set; }                // https://liveprice.fpts.com.vn/?s=51#t=aMarketWatch1470026614275


        //---------------------------------------------------------------------------
        // (E) dung js WebRTC truy van STUN servers lay local ip 
        public string ClientPrivateIP { get; set; }     // 10.26.2.33        
        public string ClientIPv6 { get; set; }          // 2001:cdba::3257:9652

        // ===========================================================================================================================

    }
    public static class SignalRConnections
    {
        public static ConcurrentDictionary<string, CSignalRCounter> Connections = new();
    }
}
