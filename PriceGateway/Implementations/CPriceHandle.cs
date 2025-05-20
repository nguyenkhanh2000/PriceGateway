using BaseRedisLib.Implementations;
using BaseRedisLib.Interfaces;
using CommonLib.Implementations;
using CommonLib.Interfaces;
using Microsoft.Extensions.Configuration;
using PriceGateway.Interfaces;
using StackExchange.Redis;
using StockCore.Redis.MW;
using SystemCore.Entities;
using static BaseRedisLib.Implementations.CRedisRepository;

namespace PriceGateway.Implementations
{
    public class CPriceHandle : IPriceHandle
    {
        public readonly IS6GApp _s6GApp;
        private readonly ConnectionMultiplexer _redis;
        private IConfiguration _configuration;
        public CPriceHandle(IS6GApp s6GApp, Lazy<ConnectionMultiplexer> redis, IConfiguration configuration)
        {
            this._s6GApp = s6GApp;
            this._redis = redis.Value;
            this._configuration = configuration;
        }
        public async Task<EResponseResult> fnc_Get_Full_Quote(string Exchange)
        {
            try
            {
                ConnectRedisMWS5GModel CM = new ConnectRedisMWS5GModel();
                //CM.key = _configuration.GetSection(CMetketConfig.__REDIS_KEY_S5G_LATEST_INVESTING).Value;
                CM.key = "FULL_ROW_QUOTE";
                CM.DB = 6;
                CM.connectString = "10.26.7.84:6379";
                var connectRedis = ConnectionMultiplexer.Connect(CM.connectString);

                IRedisRepository _cRedisRepository = new CRedisRepository(_s6GApp, _redis, CM.DB);

                List<HashKeyRedis> DataRD_Hash = _cRedisRepository.Hash_Get_All(CM.key);


                EResponseResult RM = new EResponseResult();
                RM.Code = EGlobalConfig.__CODE_ERROR_IN_LAYER_BLL;
                return RM;
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
