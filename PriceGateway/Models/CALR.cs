namespace PriceGateway.Models
{
    /*
     * view-source:http://liveprice2.fpts.com.vn/demo?s=33
        ///  var g_ALR=
        //{
        //"RedirectURL":"http://accounts.fpts.com.vn/?href=liveprice2"
        //,"LoginName":"058C108101"
        //,"ClientCode":"108101"
        //,"ClientName":"DANG THI THUY HOA"
        //,"ReturnCode":"0"
        //,"ReturnMess":"CONFIRM SUCCESS"
        //,"AccountStatus":"0"
        //,"CultureName":"en-US"
        //,"CultureShort":"EN"
        //,"CultureIndex":"1"
        //};

     */
    public class CALR
    {
        public string RedirectURL { get; set; }
        public string LoginName { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMess { get; set; }
        public string AccountStatus { get; set; }


        public string CultureName { get; set; }
        public string CultureShort { get; set; }
        public string CultureIndex { get; set; }
    }
}
