using BaseRedisLib.Implementations;
using BaseRedisLib.Interfaces;
using CommonLib.Implementations;
using CommonLib.Interfaces;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PriceGateway.Interfaces;
using StackExchange.Redis;
using StockCore.Redis.MW;
using System;
using System.Text;
using SystemCore.Entities;
using SystemCore.Temporaries;
using static BaseRedisLib.Implementations.CRedisRepository;

namespace PriceGateway.Implementations
{
    /// <summary>
    /// khanhnv
    /// Lớp CPriceHandle được đăng ký với dependency injection container của ASP.NET Core bằng phương thức AddTransient.
    /// Mỗi khi có yêu cầu (request) đến service IPriceHandle, một thể hiện mới CPriceHandle sẽ được tạo ra
    /// Việc sử dụng AddTransient phù hợp khi muốn một đối tượng tạo mới cho mỗi yêu cầu, thay vì chia sẻ đối tượng giữa các yêu cầu
    /// </summary>
    public class CPriceHandle : IPriceHandle
    {
        public readonly IS6GApp _s6GApp;
        private readonly ConnectionMultiplexer _redis;
        private readonly ConnectionMultiplexer _redis_Sentinel;
        private IConfiguration _configuration;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="s6GApp"></param>
        /// <param name="redis"></param>
        /// <param name="redis_Sentinel"></param>
        /// <param name="configuration"></param>
        public CPriceHandle(IS6GApp s6GApp, Lazy<ConnectionMultiplexer> redis, Lazy<ConnectionMultiplexer> redis_Sentinel, IConfiguration configuration)
        {
            this._s6GApp = s6GApp;
            this._redis = redis.Value;
            this._redis_Sentinel = redis_Sentinel.Value;    
            this._configuration = configuration;
        }
        /// <summary>
        /// Api_Get_Full_Quote - khanhnv
        /// </summary>
        /// <param name="Exchange">Sàn</param>
        /// <param name="TypeMsg">Loại Msg</param>
        /// <param name="Board">bảng</param>
        /// <param name="Symbol">mã chứng khoán</param>
        /// <returns></returns>
        public async Task<EResponseResult> fnc_Get_Full_Quote(string Exchange, string TypeMsg, string Board, string Symbol)
        {
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"{EGlobalConfig.__STRING_BEFORE} Exchange={Exchange}, TypeMsg={TypeMsg}, Board={Board}, Symbol={Symbol}", true);
            try
            {
                ConnectRedisMWS5GModel CM = new ConnectRedisMWS5GModel();
                //CM.key = _configuration.GetSection(CMetketConfig.__REDIS_KEY_S5G_LATEST_INVESTING).Value;
                string[] listSymbol = null;
                //Ghép chuỗi key 
                var keyBuilder = new StringBuilder();
                keyBuilder.Append(Exchange); 

                if (!string.IsNullOrEmpty(TypeMsg))
                    keyBuilder.Append(":").Append(TypeMsg);

                if (!string.IsNullOrEmpty(Board))
                    keyBuilder.Append(":").Append(Board);

                CM.key = keyBuilder.ToString();
                CM.DB = 0;
                IRedisRepository _cRedisRepository = new CRedisRepository(_s6GApp, _redis_Sentinel, CM.DB);
                List<dynamic> _lstData = new List<dynamic>();

                //Nếu tồn tại mã - get data theo mã
                if (!string.IsNullOrWhiteSpace(Symbol))
                {
                    //tách chuỗi Symbol và upper
                    listSymbol = Symbol.Split(',').Select(code => code.ToUpper()).ToArray();

                    foreach (var _symbol in listSymbol)
                    {
                        string value = _cRedisRepository.Hash_Get(CM.key, _symbol);
                        if (string.IsNullOrEmpty(value))
                        {
                            return new EResponseResult() { Code = EDalResult.__CODE_SUCCESS, Message = EDalResult.__STRING_SUCCESS, Data = "" };
                        }
                        dynamic data = JsonConvert.DeserializeObject<dynamic>(value);
                        _lstData.Add(data);
                    }
                }
                else //GetAll
                {
                    List<HashKeyRedis> DataRD_Hash = _cRedisRepository.Hash_Get_All(CM.key);

                    foreach (var item in DataRD_Hash)
                    {
                        dynamic cData = JsonConvert.DeserializeObject<dynamic>(item.Value);

                        _lstData.Add(cData);
                    }
                }
                  
                this._s6GApp.SqlLogger.LogSqlContext2("", ec, " ==> Output  " + _lstData.Count);
                return new EResponseResult() { Code = EDalResult.__CODE_SUCCESS, Message = EDalResult.__STRING_SUCCESS, Data = _lstData };
            }
            catch (Exception ex)
            {
                // log error + buffer data
                this._s6GApp.ErrorLogger.LogError(ex);
                EResponseResult RM = new EResponseResult();
                RM.Code = EGlobalConfig.__CODE_ERROR_IN_LAYER_BLL;
                return RM;
            }
        }
    }
}
