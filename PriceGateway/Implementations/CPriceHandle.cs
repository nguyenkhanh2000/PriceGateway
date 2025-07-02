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
                        if (!string.IsNullOrEmpty(value))
                        {
                            dynamic data = JsonConvert.DeserializeObject<dynamic>(value);
                            _lstData.Add(data);
                        }
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Symbol"></param>
        /// <param name="Board"></param>
        /// <returns></returns>
        public async Task<EResponseResult> fnc_Get_Full_Price(string Symbol, string Board)
        {
            // Bắt đầu ghi log và context thực thi
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"{EGlobalConfig.__STRING_BEFORE} Symbol={Symbol}, Board={Board}", true);
            try
            {
                // 1. Chuẩn bị các mã chứng khoán (fields) cần lấy từ tham số Symbol
                string[] liststockCode = Symbol.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(code => code.Trim().ToUpper())
                                               .ToArray();

                if (liststockCode.Length == 0)
                {
                    return new EResponseResult() { Code = EDalResult.__CODE_SUCCESS, Message = EDalResult.__STRING_SUCCESS, Data = new Dictionary<string, List<HashKeyRedis>>() };
                }

                var fieldsToFetch = liststockCode.Select(code => (RedisValue)code).ToArray();

                // 2. Chuẩn bị các key Redis (ứng với các message type) cần truy vấn
                var msgTypes = new List<string> { "D", "F", "ME", "MT", "X_KL", "X_TP" };
                var redisKeys = msgTypes.Select(mt => (RedisKey)$"MDDS:{mt}:{Board}").ToList();

                // 3. Khởi tạo kết nối và tạo batch (pipeline)
                IDatabase redisDb = _redis_Sentinel.GetDatabase(9); 
                IBatch batch = redisDb.CreateBatch();

                // 4. Thêm các lệnh HMGET (HashGetAsync) vào batch
                var allTasks = new List<Task<RedisValue[]>>();
                foreach (var key in redisKeys)
                {
                    allTasks.Add(batch.HashGetAsync(key, fieldsToFetch));
                }

                batch.Execute();

                // 5. Chờ và nhận tất cả kết quả trả về
                RedisValue[][] results = await Task.WhenAll(allTasks);

                // 6. Tổng hợp dữ liệu theo từng msgType
                // Cấu trúc cuối cùng sẽ là Dictionary<string, List<HashKeyRedis>>
                var finalData = new Dictionary<string, List<ResPrice>>();

                // Duyệt qua kết quả của từng key, tương ứng với mỗi msgType
                for (int i = 0; i < results.Length; i++)
                {
                    string currentMsgType = msgTypes[i];
                    RedisValue[] valueSet = results[i];

                    // Tạo một danh sách mới để chứa dữ liệu của riêng msgType này
                    var dataForThisMsgType = new List<ResPrice>();

                    // Duyệt qua từng giá trị trả về cho mỗi mã chứng khoán đã yêu cầu
                    for (int j = 0; j < valueSet.Length; j++)
                    {
                        RedisValue value = valueSet[j];
                        if (!value.IsNullOrEmpty)
                        {
                            // Thêm dữ liệu (mã và giá trị) vào danh sách của msgType hiện tại
                            dataForThisMsgType.Add(new ResPrice
                            {
                                Key = liststockCode[j], // Mã chứng khoán
                                Value = JsonConvert.DeserializeObject(value.ToString())          // Giá trị tương ứng
                            });
                        }
                    }

                    // Đưa danh sách dữ liệu của msgType này vào Dictionary kết quả.
                    // Ngay cả khi không có mã nào được tìm thấy, nó vẫn thêm một key với danh sách rỗng.
                    finalData[currentMsgType] = dataForThisMsgType;
                }

                // 7. Trả về kết quả thành công với dữ liệu đã được nhóm theo msgType
                return new EResponseResult() { Code = EDalResult.__CODE_SUCCESS, Message = EDalResult.__STRING_SUCCESS, Data = finalData };
            }
            catch (Exception ex)
            {
                // Ghi log lỗi và trả về phản hồi lỗi
                this._s6GApp.ErrorLogger.LogError(ex);
                var RM = new EResponseResult
                {
                    Code = EGlobalConfig.__CODE_ERROR_IN_LAYER_BLL,
                    Message = ex.Message
                };
                return RM;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public async Task<EResponseResult> fnc_Get_String_Seq(string keyName)
        {
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"{EGlobalConfig.__STRING_BEFORE} keyName={keyName}", true);
            try
            {
                ConnectRedisMWS5GModel CM = new ConnectRedisMWS5GModel();

                CM.key = keyName;
                CM.DB = 0;

                IRedisRepository _cRedisRepository = new CRedisRepository(_s6GApp, _redis_Sentinel, CM.DB);

                string DataRedis = _cRedisRepository.String_Get(CM.key, CM.DB);
                if (string.IsNullOrEmpty(DataRedis))
                {
                    return new EResponseResult() { Code = EDalResult.__CODE_SUCCESS, Message = EDalResult.__STRING_SUCCESS, Data = string.Empty };
                }

                //convert to json
                dynamic Seq_data = JsonConvert.DeserializeObject<dynamic>(DataRedis);

                //object Seq = JsonConvert.DeserializeObject<object>(Seq_data);
                this._s6GApp.SqlLogger.LogSqlContext2("", ec, " ==> Output  " + DataRedis.Length);

                return new EResponseResult() { Code = EDalResult.__CODE_SUCCESS, Message = EDalResult.__STRING_SUCCESS, Data = Seq_data };
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
