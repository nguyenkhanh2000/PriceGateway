using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCore.Redis.Report
{
    public class ReportRedisModel
    {
        public string ClientCode { get; set; }
    }

    public class ReportCashModel
    {
        public string Time { get; set; }
        public string PurchasingPowerTotal { get; set; }            //Số dư có thể giao dịch
        public string CashAmount { get; set; }                      //Số dư tiền ban đầu
        public string AdvanceAmount { get; set; }                   //Tiền có thể ứng trước
        public string RemainingSecurities { get; set; }             //Sức mua từ CK còn lại
        public string PendingBuyCash { get; set; }                  //Tiền treo mua chờ khớp
        public string MatchedBuyCash { get; set; }                  //Tiền treo mua đã khớp
        public string TransferringAmount { get; set; }              //Tiền đang chuyển
        public string RemainingCashAmount { get; set; }             //Tiền mặt còn lại
        public string RemainingDebt { get; set; }                   //Dư nợ ký quỹ 
        public string DebtInterest { get; set; }                    //Lãi vay ký quỹ
        public string ReceivableCashTotal { get; set; }             //Tổng tiền chờ về
        public string ReceivableCashT0 { get; set; }                //Tiền chờ về T0
        public string ReceivableCashT1 { get; set; }                //Tiền chờ về T1
        public string ReceivableCashT2 { get; set; }                //Tiền chờ về T2
        public string ReceivableCashDevidend { get; set; }          //Tiền cổ tức chờ về
        public string ReceivableCashOther { get; set; }             //Tiền chờ về khác
        public string Fees { get; set; }                            //Phí chờ thu khác 
        public string NetAssetValueMoney { get; set; }              //Giá trị tiền ròng

        // test
        public string ReceivableMatureCW { get; set; }              //Tiền cổ tức chờ về
        public string ReceivableCashOddlot { get; set; }
        public string RemainingSecuritiesleveragedbuyingpower { get; set; }             //Sức mua từ CK còn lại
    }

    public class ReportStockModel
    {
        public string Time { get; set; }
        public string StockCode { get; set; }                           //Mã
        public string AvailableOrderSecurities { get; set; }            //SL có thể đặt bán
        public string PendingSellIntraday { get; set; }                 //SL bán chờ khớp
        public string MarketPrice { get; set; }                         //Giá thị trường
        public string AveragePrice { get; set; }                        //Giá TB tạm tính
        public string ProfitLossRate { get; set; }                      //% Lãi/lỗ dự kiến
        public string TotalAmount { get; set; }                         //Tổng KL
        public string MarketValue { get; set; }                         //Giá trị thị trường
        public string RootValue { get; set; }                           //Giá trị gốc 
        public string ProfitLossValue { get; set; }                     //Giá trị lãi/lỗ dự kiến
        public string TradingreadyAvailable { get; set; }               //Chứng khoán có sẵn 
        public string MatchedSell { get; set; }                         //Bán T0
        public string MatchedBuyT0 { get; set; }                        //Mua T0
        public string ReceivableT1 { get; set; }                        //Mua T1
        public string ReceivableT2 { get; set; }                        //Mua T2
        public string WaitingReceiveRight { get; set; }                 //Quyền chờ về
        public string LimitAmount { get; set; }                         //Hạn chế
        public string DividendBonus { get; set; }                       //CP thưởng/Cổ tức bằng cổ phiếu

        //test
        public string ClientCode { get; set; }          // 058C547797
        public string AvailableOrderSecuritiesMar { get; set; }          // 
        public string AvailableOrderSecuritiesTotal { get; set; }           //
        public string PendingSellintradayMar { get; set; }           // 
        public string PendingSellintradayTotal { get; set; }           // 
        public string OldAveragePrice { get; set; }           // 
        public string TradingReadyAvailableMarMor { get; set; }           // 
        public string TradingReadyAvailableTotal { get; set; }           // 
        public string MatchedSellintraday { get; set; }           // 
        public string MatchedSellintradayMar { get; set; }           // 
        public string MatchedSellintradayTotal { get; set; }           // 
        public string MatchedBuyintraday { get; set; }           //
        public string ReceivableSecuritiesT1 { get; set; }           // 
        public string ReceivableSecuritiesT2 { get; set; }           // 
        public string WaitingReceiveRightSecurities { get; set; }           // 
        public string MortgageAtBank { get; set; }           // 
        public string TransferRestricted { get; set; }           // 
        public string DividendBonusShare { get; set; }           // 
    }

    public class HashReportStockModel
    {
        public string Key { get; set; }
        public dynamic Value { get; set; }
    }
}
