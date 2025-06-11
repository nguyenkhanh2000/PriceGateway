namespace PriceGateway.Models
{
    public class CClientInfo
    {
        public string OS { get; set; }                  // Windows 7
        public string Browser { get; set; }             // Chrome 52 (52.0.2743.116)
        public string Mobile { get; set; }              // false
        public string Screen { get; set; }              // 1920 x 1080
        public string Page { get; set; }                // https://liveprice.fpts.com.vn/?s=51#t=aMarketWatch1470026614275

        //, 'OS': jscd.os + ' ' + jscd.osVersion
        //, 'Browser': jscd.browser + ' ' + jscd.browserMajorVersion + ' (' + jscd.browserVersion + ')'
        //, 'Mobile': jscd.mobile
        //, 'Screen': jscd.screen
        //, 'Page': vPageType

        // Windows 7	Chrome 52 (52.0.2743.116)	false	1920 x 1080	Admin	document.location.href
    }
}
