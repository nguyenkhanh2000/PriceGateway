using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCore.Redis.Open
{
    public class OpenPermitFeeModel
    {
        public string PRODUCTTYPE { get; set; }           // 
        public string STATUS { get; set; }          // 
    }

    public class PermitFeeModel
    {
        public string EzTradeChargeRate { get; set; }
        public string EzTrade { get; set; }
        public string EzTransfer { get; set; }
        public string EzAdvance { get; set; }

        public string EzMargin { get; set; }
        public string EzMortgage { get; set; }
        public string EzOddlot { get; set; }
        public string EzMarginPro { get; set; }
        public string EzFuture { get; set; }
        public string EzTvdt { get; set; }
        public string vTblid { get; set; }
        public string EzSaving { get; set; }
        public string EzMarginPlus { get; set; }
        public string Adomestic { get; set; }

        public string vFeeUP { get; set; }
        public string vFeeUP_CCQ { get; set; }
        public string vFeeLISTED_CP { get; set; }
        public string vFeeHSX_CP { get; set; }
        public string vFeeRate_TP { get; set; }

        public string vFeeLISTED_ETF { get; set; }
        public string vFeeLISTED_CCQ { get; set; }
        public string vFeeHSX_CCQ { get; set; }
        public string vFeeHSX_CQ { get; set; }
        public string vFeeHSX_ETF { get; set; }
        public string vFeeLISTED_CQ { get; set; }
    }

    public class OpenPermitModel
    {
        public string Time { get; set; }          // 
        public PermitModel Data { get; set; }          // 
    }

    public class PermitModel
    {
        public string AID { get; set; }          // 
        public string ATBLID { get; set; }          // 
        public string ACUSTACCOUNT { get; set; }          // 
        public string ATRADETYPE_PERSON { get; set; }          // 
        public string ATRADETYPE_ORG { get; set; }          // 
        public string ATRADE_ONLINE { get; set; }          // 
        public string ATRADE_ODDLOT { get; set; }          // 
        public string ARESULT_FLOOR { get; set; }          // 
        public string ARESULT_SMS { get; set; }          // 
        public string ARESULT_EMAIL { get; set; }          // 
        public string ATBANEWS_FLOORLID { get; set; }          // 
        public string ANEWS_EMAIL { get; set; }          // 
        public string ASAOKE_FLOOR { get; set; }          // 
        public string ASAOKE_EMAIL { get; set; }          // 
        public string ASAOKE_POST { get; set; }          // 
        public string ATAX_ORG { get; set; }          // 
        public string ATAX_FPTS { get; set; }          // 
        public string ATRADE_TRANSFER { get; set; }          // 
        public string ATRADE_MORTGAGE { get; set; }          // 
        public string ATRADE_MARGIN { get; set; }          // 
        public string ABRKID { get; set; }          // 
        public string ASMS_FEE_BALANCE { get; set; }          // 
        public string ASMS_FEE_RESULT { get; set; }          // 
        public string ASMS_FEE_RIGHTS { get; set; }          // 
        public string ASMS_FREE_MARGIN { get; set; }          // 
        public string ASMS_FREE_FPTS { get; set; }          // 
        public string ATRADE_EZMARGINPRO { get; set; }          // 
        public string ATRADETYPE { get; set; }          // 
        public string ARSA_TOKEN { get; set; }          // 
        public string ATRADE_EZFUTURES { get; set; }          // 
        public string ABRKFUID { get; set; }          // 
        public string ACUSTYPE { get; set; }          // 
        public string AFUTUREUQ_DATE { get; set; }          // 
        public string ATRADEFU_DATE { get; set; }          // 
        public string ATRADEFUCLOSE_DATE { get; set; }          // 
        public string ATRANSFERFLAG { get; set; }          // 
        public string ATRADEFU_ORDERTEMPLATE { get; set; }          // 
        public string ATRADE_EZTVDT { get; set; }          // 
        public string ASAOKE_VAT_EMAIL { get; set; }          // 
        public string ASMS_FEE_RESULT_2 { get; set; }          // 
        public string AMODIFYDATE { get; set; }          // 
        public string ATRADE_FSAVINGS { get; set; }
        public string ATRADE_EZTPLUS { get; set; }
    }


    public class OpenFeeModel
    {
        public string Time { get; set; }          // 
        public FeeModel Data { get; set; }          // 
    }

    public class FeeModel
    {
        public string ACLIENTCODE { get; set; }          // 
        public string APACKFEECODE { get; set; }          // 
        public string AFEECDE { get; set; }          // 
        public string AFEETYPE { get; set; }          // 
        public string AEXCHANGE { get; set; }          // 
        public string AAMOUNTFR { get; set; }          // 
        public string AAMOUNTTO { get; set; }          // 
        public string ASTOCKTYPE { get; set; }          // 
        public string AFEEFPTS { get; set; }          // 
        public string AEXCFEE { get; set; }          // 
        public string AFEESUM { get; set; }          // 
        public string ATAX { get; set; }          // 
        public string ACALTYPE { get; set; }          // 
        public string AUNIT { get; set; }          // 
        public string ADATETIME { get; set; }          // 
        public string AMODIFIEDBY { get; set; }          // 
        public string ATAX_DIV { get; set; }          // 
        public string ATAXDIV_MAX { get; set; }          // 
        public string ALASTUPDATE { get; set; }          // 
    }

    public class HashFeeModel
    {
        public string Key { get; set; }
        public FeeModel Value { get; set; }
    }
}
