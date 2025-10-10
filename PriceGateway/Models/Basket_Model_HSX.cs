namespace PriceGateway.Models
{
    public class Basket_Model_HSX
    {
        public string All { get; set; }
        public string VNINDEX { get; set; }
        public string VN30 { get; set; }
        public string VN100 { get; set; }
        public string VNALL { get; set; }
        public string VNMID { get; set; }
        public string VNSML { get; set; }
        public string VNXALL { get; set; }
        public string ETF { get; set; }
    }
    public class Basket_Model_HNX
    {
        public string HNXUpcomIndex { get; set; }
        public string HNXIndex { get; set; }
        public string HNX30 { get; set; }
        public string BOND { get; set; } 
    }
    public class Basket_Model_FU
    {
        public string HNXDSAll { get; set; } 
    }

    public class RootObject<T>
    {
        public string Time { get; set; }
        public List<T> Data { get; set; }
    }
}
