using BaseRedisLib.Implementations;
using BaseRedisLib.Interfaces;
using CommonLib.Implementations;
using CommonLib.Interfaces;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PriceGateway.BLL;
using PriceGateway.Interfaces;
using PriceGateway.Models;
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

                // Không thêm Board nếu TypeMsg là "M1"
                if (TypeMsg != "M1" && !string.IsNullOrEmpty(Board))
                    keyBuilder.Append(":").Append(Board);

                CM.key = keyBuilder.ToString();
                CM.DB = Int32.Parse(_configuration.GetSection(CConfig.__CONNECTION_REDIS_DB0).Value);
                IRedisRepository _cRedisRepository = new CRedisRepository(_s6GApp, _redis_Sentinel, CM.DB);
                List<dynamic> _lstData = new List<dynamic>();
                // Mapping Symbol đặc biệt khi TypeMsg là M1
                Dictionary<string, string> mapSymbol = new Dictionary<string, string>
                {
                    {"VNINDEX", "001"}, {"VN30", "101"}, {"VNMIDCAP", "102"}, {"VNSMALLCAP", "103"},
                    {"VN100", "104"}, {"VNALLSHARE", "105"}, {"VN50", "106"}, {"VNXALLSHARE", "151"},
                    {"VNX50", "152"}, {"HNXINDEX", "002"}, {"HNXLCAP", "003"}, {"HNXMSCAP", "004"},
                    {"HNX30", "100"}, {"HNXMAN", "200"}, {"HNXCON", "201"}, {"HNXFIN", "202"},
                    {"UPCOM", "301"}, {"UPCOMLARGEINDEX", "310"}, {"UPCOMSMALLINDEX", "320"},
                    {"UPCOMMEDIUMINDEX", "330"}, {"HNX50", "500"}, {"GICS-FINANCIAL", "868"}
                };
                if (TypeMsg == "M1")
                {
                    if (!string.IsNullOrWhiteSpace(Symbol))
                    {
                        //listSymbol = Symbol.Split(',').Select(code => code.Trim()).ToArray();
                        listSymbol = Symbol.Split(',').Select(code => code.ToUpper()).ToArray();
                        foreach (var sym in listSymbol)
                        {
                            if (mapSymbol.TryGetValue(sym, out var mappedSymbol))
                            {
                                string value = _cRedisRepository.Hash_Get(CM.key, mappedSymbol);
                                if (!string.IsNullOrEmpty(value))
                                {
                                    dynamic data = JsonConvert.DeserializeObject<dynamic>(value);
                                    _lstData.Add(data);
                                }
                            }
                        }
                    }
                    else
                    {
                        var allData = _cRedisRepository.Hash_Get_All(CM.key);
                        foreach (var item in allData)
                        {
                            dynamic data = JsonConvert.DeserializeObject<dynamic>(item.Value);
                            _lstData.Add(data);
                        }
                    }
                }
                else
                {
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
                IDatabase redisDb = _redis_Sentinel.GetDatabase(Int32.Parse(_configuration.GetSection(CConfig.__CONNECTION_REDIS_DB0).Value)); 
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

                // 6. Tổng hợp dữ liệu theo cấu trúc mới
                // THAY ĐỔI: Khởi tạo là một List<GroupedPriceResult> thay vì Dictionary
                var finalData = new List<GroupedPriceResult>();

                // Duyệt qua kết quả của từng key, tương ứng với mỗi msgType
                for (int i = 0; i < results.Length; i++)
                {
                    string currentMsgType = msgTypes[i];
                    RedisValue[] valueSet = results[i];

                    var valuesForThisMsgType = new List<object>();

                    // Duyệt qua từng giá trị trả về cho mỗi mã chứng khoán đã yêu cầu
                    for (int j = 0; j < valueSet.Length; j++)
                    {
                        RedisValue value = valueSet[j];
                        if (!value.IsNullOrEmpty)
                        {
                            valuesForThisMsgType.Add(JsonConvert.DeserializeObject(value.ToString()));
                        }
                    }                   
                    // THAY ĐỔI: Tạo một đối tượng GroupedPriceResult và thêm vào danh sách kết quả
                    // Ngay cả khi không có mã nào được tìm thấy, nó vẫn thêm một mục với danh sách rỗng.
                    finalData.Add(new GroupedPriceResult
                    {
                        Key = currentMsgType,
                        Value = valuesForThisMsgType
                    });
                }               

                //// 6. Tổng hợp dữ liệu theo từng msgType
                //// Cấu trúc cuối cùng sẽ là Dictionary<string, List<HashKeyRedis>>
                //var finalData = new Dictionary<string, List<ResPrice>>(); 
                ////D : [key;value]

                //// Duyệt qua kết quả của từng key, tương ứng với mỗi msgType
                //for (int i = 0; i < results.Length; i++)
                //{
                //    string currentMsgType = msgTypes[i];
                //    RedisValue[] valueSet = results[i];

                //    // Tạo một danh sách mới để chứa dữ liệu của riêng msgType này
                //    var dataForThisMsgType = new List<ResPrice>();

                //    // Duyệt qua từng giá trị trả về cho mỗi mã chứng khoán đã yêu cầu
                //    for (int j = 0; j < valueSet.Length; j++)
                //    {
                //        RedisValue value = valueSet[j];
                //        if (!value.IsNullOrEmpty)
                //        {
                //            // Thêm dữ liệu (mã và giá trị) vào danh sách của msgType hiện tại
                //            dataForThisMsgType.Add(new ResPrice
                //            {
                //                Key = liststockCode[j], // Mã chứng khoán
                //                Value = JsonConvert.DeserializeObject(value.ToString())          // Giá trị tương ứng
                //            });
                //        }
                //    }

                //    // Đưa danh sách dữ liệu của msgType này vào Dictionary kết quả.
                //    // Ngay cả khi không có mã nào được tìm thấy, nó vẫn thêm một key với danh sách rỗng.
                //    finalData[currentMsgType] = dataForThisMsgType;
                //}
                this._s6GApp.SqlLogger.LogSqlContext2("", ec, " ==> Output  " + finalData.Count);
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
                CM.DB = Int32.Parse(_configuration.GetSection(CConfig.__CONNECTION_REDIS_DB0).Value); 

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
        public async Task<EResponseResult> fnc_Get_Session(string TypeMsg, string Exchange)
        {
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"{EGlobalConfig.__STRING_BEFORE} TypeMsg={TypeMsg}, Exchange={Exchange}", true);
            try
            {

                List<dynamic> _lstData = new List<dynamic>();
                ConnectRedisMWS5GModel CM = new ConnectRedisMWS5GModel();
                CM.DB = Int32.Parse(_configuration.GetSection(CConfig.__CONNECTION_REDIS_DB0).Value);

                IRedisRepository _cRedisRepository = new CRedisRepository(_s6GApp, _redis_Sentinel, CM.DB);
                if (!string.IsNullOrEmpty(Exchange)) //Get theo exchange
                {
                    string pattern = $"MDDS:{TypeMsg}:{Exchange}";
                    List<HashKeyRedis> DataRD_Hash = _cRedisRepository.Hash_Get_All(pattern);
                    foreach(var item in DataRD_Hash)
                    {
                        dynamic cData = JsonConvert.DeserializeObject<dynamic>(item.Value);
                        _lstData.Add(cData);
                    }
                }
                else //Get all
                {
                    // 1. Định nghĩa pattern để tìm kiếm keys
                    string pattern = $"MDDS:{TypeMsg}:*";

                    // 2. Lấy tất cả các server endpoints để thực hiện SCAN
                    var server = _cRedisRepository.GetServer(); 

                    // 3. Dùng SCAN (thông qua phương thức Keys) để lấy tất cả các key khớp với pattern một cách an toàn
                    // StackExchange.Redis's Keys() method with a pattern uses SCAN behind the scenes.
                    var matchingKeys = server.Keys(database: CM.DB, pattern: pattern);

                    // 4. Lặp qua từng key tìm được và lấy dữ liệu Hash
                    foreach (var key in matchingKeys)
                    {
                        List<HashKeyRedis> DataRD_Hash = _cRedisRepository.Hash_Get_All(key);

                        foreach (var item in DataRD_Hash)
                        {
                            dynamic cData = JsonConvert.DeserializeObject<dynamic>(item.Value);
                            _lstData.Add(cData);
                        }
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
        public async Task<EResponseResult> fnc_Get_Basket(string Exchange)
        {
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"{EGlobalConfig.__STRING_BEFORE}, Exchange={Exchange}", true);
            try
            {
                string exchange = Exchange.ToUpper();

                // Sử dụng int.TryParse để tránh lỗi nếu giá trị config không hợp lệ
                if (!int.TryParse(_configuration.GetSection(CConfig.__CONNECTION_REDIS_DB0).Value, out int dbIndex))
                {
                    // Ghi log hoặc trả về lỗi nếu không đọc được cấu hình DB
                    // Ở đây tạm gán giá trị mặc định là 0
                    dbIndex = 0;
                }

                ConnectRedisMWS5GModel CM = new ConnectRedisMWS5GModel { DB = dbIndex };
                IRedisRepository _cRedisRepository = new CRedisRepository(_s6GApp, _redis_Sentinel, CM.DB);

                // Khai báo _lstData với kiểu cụ thể hơn nếu có thể, ví dụ List<object>
                var _lstData = new List<object>();

                string redisKey = "";

                switch (exchange)
                {
                    case "HSX":
                        redisKey = _configuration.GetSection(CConfig.__KEY_LIST_HSX).Value;
                        break;
                    case "HNX":
                        redisKey = _configuration.GetSection(CConfig.__KEY_LIST_HNX).Value;
                        break;
                    case "FU":
                        redisKey = _configuration.GetSection(CConfig.__KEY_LIST_FU).Value;
                        break;
                    case "CW":
                        redisKey = _configuration.GetSection(CConfig.__KEY_LIST_CW).Value;
                        break;
                    case "ALL":
                        var exchanges = new Dictionary<string, Type>
                        {
                            { "HSX", typeof(Basket_Model_HSX) },
                            { "HNX", typeof(Basket_Model_HNX) },
                            { "FU", typeof(Basket_Model_FU) },
                            { "CW", typeof(string) }
                        };
                        foreach (var ex in exchanges)
                        {
                            string exRedisKey = _configuration.GetSection(ex.Key switch
                            {
                                "HSX" => CConfig.__KEY_LIST_HSX,
                                "HNX" => CConfig.__KEY_LIST_HNX,
                                "FU" => CConfig.__KEY_LIST_FU,
                                "CW" => CConfig.__KEY_LIST_CW,
                                _ => ""
                            }).Value;
                            if (string.IsNullOrEmpty(exRedisKey)) continue;

                            string exDataRedis = _cRedisRepository.String_Get(exRedisKey, CM.DB);
                            if (string.IsNullOrEmpty(exDataRedis)) continue;

                            var exInnerJsonString = JsonConvert.DeserializeObject<string>(exDataRedis);
                            if (ex.Key == "CW")
                            {
                                var jsonObjCW_ALL = JsonConvert.DeserializeObject<RootObject<string>>(exInnerJsonString);
                                var basketDataCW_ALL = jsonObjCW_ALL?.Data?.FirstOrDefault();
                                if (!string.IsNullOrEmpty(basketDataCW_ALL))
                                {
                                    _lstData.Add(new Dictionary<string, string> { { "CW", basketDataCW_ALL } });
                                }
                            }
                            else
                            {
                                var genericType = typeof(RootObject<>).MakeGenericType(ex.Value);
                                var jsonObj = JsonConvert.DeserializeObject(exInnerJsonString, genericType);
                                var dataProperty = genericType.GetProperty("Data")?.GetValue(jsonObj);

                                var firstItem = (dataProperty as System.Collections.IEnumerable)?.Cast<object>().FirstOrDefault();
                                if (firstItem != null) _lstData.Add(firstItem);
                            } 
                        }
                        this._s6GApp.SqlLogger.LogSqlContext2("", ec, " ==> Output  " + _lstData.Count);
                        return new EResponseResult() { Code = EDalResult.__CODE_SUCCESS, Message = EDalResult.__STRING_SUCCESS, Data = _lstData };
                        break;
                }
                if (string.IsNullOrEmpty(redisKey))
                {
                    return new EResponseResult() { Code = EDalResult.__CODE_SUCCESS, Message = EDalResult.__STRING_SUCCESS, Data = "Invalid Exchange" };
                }
                string dataRedis = _cRedisRepository.String_Get(redisKey, CM.DB);
                if (string.IsNullOrEmpty(dataRedis))
                {
                    return new EResponseResult() { Code = EDalResult.__CODE_SUCCESS, Message = EDalResult.__STRING_SUCCESS, Data = string.Empty };
                }
                //Dữ liệu trong Redis đang bị "double-encoded" (JSON trong JSON).
                var innerJsonString = JsonConvert.DeserializeObject<string>(dataRedis);
                switch (exchange)
                {
                    case "HSX":
                        var jsonObjHSX = JsonConvert.DeserializeObject<RootObject<Basket_Model_HSX>>(innerJsonString);
                        var basketDataHSX = jsonObjHSX?.Data?.FirstOrDefault();
                        if (basketDataHSX != null) _lstData.Add(basketDataHSX);
                        break;
                    case "HNX":
                        var jsonObjHNX = JsonConvert.DeserializeObject<RootObject<Basket_Model_HNX>>(innerJsonString);
                        var basketDataHNX = jsonObjHNX?.Data?.FirstOrDefault();
                        if (basketDataHNX != null) _lstData.Add(basketDataHNX);
                        break;
                    case "FU":
                        var jsonObjFU = JsonConvert.DeserializeObject<RootObject<Basket_Model_FU>>(innerJsonString);
                        var basketDataFu = jsonObjFU?.Data?.FirstOrDefault();
                        if (basketDataFu != null) _lstData.Add(basketDataFu);
                        break;
                    case "CW":
                        var jsonObjCW = JsonConvert.DeserializeObject<RootObject<string>>(innerJsonString);
                        var basketDataCW = jsonObjCW?.Data?.FirstOrDefault();
                        //if (basketDataCW != null) _lstData.Add(basketDataCW);
                        if (!string.IsNullOrEmpty(basketDataCW))
                        {
                            _lstData.Add(new Dictionary<string, string> { { "CW", basketDataCW } });
                        }
                        break;
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
        //public async Task<EResponseResult> fnc_Get_Basket(string Exchange)
        //{
        //    try
        //    {
        //        string exchange = Exchange.ToUpper();
        //        ConnectRedisMWS5GModel CM = new ConnectRedisMWS5GModel();
        //        CM.DB = Int32.Parse(_configuration.GetSection(CConfig.__CONNECTION_REDIS_DB0).Value);
        //        IRedisRepository _cRedisRepository = new CRedisRepository(_s6GApp, _redis_Sentinel, CM.DB);
        //        List<dynamic> _lstData = new List<dynamic>();
        //        switch (exchange)
        //        {
        //            case "HSX":
        //                CM.key = _configuration.GetSection(CConfig.__KEY_LIST_HSX).Value;
        //                string DataRedis = _cRedisRepository.String_Get(CM.key, CM.DB);
        //                if (!string.IsNullOrEmpty(DataRedis))
        //                {
        //                    // Deserialize lần thứ nhất (chuỗi bên ngoài)
        //                    var innerJsonString = JsonConvert.DeserializeObject<string>(DataRedis);

        //                    // Deserialize lần thứ hai (chuỗi JSON thực sự)
        //                    var jsonObj = JsonConvert.DeserializeObject<RootObject>(innerJsonString);
        //                    var basketData = jsonObj.Data.FirstOrDefault();
        //                    if (basketData != null)
        //                    {
        //                        _lstData = JsonConvert.SerializeObject(basketData);
        //                    }
        //                }
        //                break;
        //            case "HNX":

        //                break ;
        //            case "FU":

        //                break ;
        //            case "CW":

        //                break ;
        //        }

        //        return new EResponseResult() { Code = EDalResult.__CODE_SUCCESS, Message = EDalResult.__STRING_SUCCESS, Data = _lstData };
        //    }
        //    catch (Exception ex)
        //    {
        //        // log error + buffer data
        //        this._s6GApp.ErrorLogger.LogError(ex);
        //        EResponseResult RM = new EResponseResult();
        //        RM.Code = EGlobalConfig.__CODE_ERROR_IN_LAYER_BLL;
        //        return RM;
        //    }
        //}
    }
}
