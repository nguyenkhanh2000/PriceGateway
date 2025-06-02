using BaseRedisLib.Interfaces;
using CommonLib.Interfaces;
using StackExchange.Redis;
using StockCore.Redis.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemCore.SharedKernel;
using SystemCore.Temporaries;

namespace BaseRedisLib.Implementations
{
    public class CRedisRepository : CInstance, IRedisRepository
    {
        // const
        public const int __MINUTES_IN_A_MONTH = 1;//43830; //đủ time cho key sống 1 tháng
        public const int __POOL_SIZE = 100;
        public const int __POOL_TIMEOUT_SECONDS = 3;

        // vars
        private readonly IS6GApp _s6GApp;
        private readonly ConnectionMultiplexer _redis;
        private readonly int _databaseNumber = -1;
        private IDatabase _db;
        private static object _locker = new object(); // locker, tranh error voi multi-thread code

        /// <summary>
        /// 2019-02-11 13:39:19 ngocta2
        /// constructor
        /// ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("server1:6379,server2:6379");
        /// https://stackexchange.github.io/StackExchange.Redis/Basics.html
        /// </summary>
        /// <param name="s6GApp"></param>
        /// <param name="redis"></param>
        /// <param name="databaseNumber"></param>
        public CRedisRepository(IS6GApp s6GApp, ConnectionMultiplexer redis, int databaseNumber)
        {
            this._s6GApp = s6GApp;
            this._redis = redis;
            this._databaseNumber = databaseNumber;
            this._db = redis.GetDatabase(databaseNumber, null);
        }

        /// <summary>
        /// destructor
        /// </summary>
        ~CRedisRepository()
        {
            try
            {
                // free                
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogError( ex);
            }
        }

        public object ConnectionMultiplexer
        {
            get { return _redis; }
        }

        public object Subscriber
        {
            get { return _redis.GetSubscriber(); }
        }

        //----------------------------------------------------

        /// <summary>
        /// 2019-02-11 14:02:26 ngocta2
        /// set value vao string key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        //public bool String_Set(string key, string value, int duration = __MINUTES_IN_A_MONTH)
        //{
        //	// debug
        //	TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"key={this._s6GApp.Common.CheckNullString(key)}, value={this._s6GApp.Common.CheckNullString(value)}, duration={duration} mins");

        //	try
        //	{
        //		// convert eDataSingle
        //		EDataSingle eDataSingle = new EDataSingle(value);
        //		string json = this._s6GApp.Common.SerializeObject(eDataSingle);

        //		// log sql before
        //		//this._s6GApp.SqlLogger.LogSqlContext(ec, $"json={this._s6GApp.Common.CheckNullString(json)}");

        //		// exec sql
        //		bool result = this._db.StringSet(key, json, TimeSpan.FromMinutes(duration));

        //		// log sql after
        //		//this._s6GApp.SqlLogger.LogSqlContext(ec, $"json={this._s6GApp.Common.CheckNullString(json)}; result={result}");
        //		this._s6GApp.SqlLogger.LogToFile(null, key, $"json={this._s6GApp.Common.CheckNullString(json)}; result={result}");

        //		return result; // success?
        //	}
        //	catch (Exception ex)
        //	{
        //		this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
        //		return false; // failed
        //	}
        //	finally
        //	{
        //		// log debug
        //		this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
        //	}
        //}

        /// <summary>
        /// 2019-02-11 14:25:28 ngocta2
        /// luu string json xin trong redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public bool String_SetObject(string key, object value, int duration = __MINUTES_IN_A_MONTH)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"key={this._s6GApp.Common.CheckNullString(key)}, value=?, duration={duration} mins");

            try
            {
                // convert eDataSingle
                EDataSingle eDataSingle = new EDataSingle(value);
                string json = this._s6GApp.Common.SerializeObject(eDataSingle);

                // log sql before
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"json={this._s6GApp.Common.CheckNullString(json)}");

                // exec sql
                bool result = this._db.StringSet(key, json, TimeSpan.FromSeconds(duration));

                // log sql after
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"json={this._s6GApp.Common.CheckNullString(json)}; result={result}");
                this._s6GApp.SqlLogger.LogToFile(null, key, $"json={this._s6GApp.Common.CheckNullString(json)}; result={result}");

                return result; // success?
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return false; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// 2019-02-11 14:25:28 ngocta2
        /// luu string json xin trong redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public bool String_SetObject2(string key, object value, int duration = __MINUTES_IN_A_MONTH)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"key={this._s6GApp.Common.CheckNullString(key)}, value=?, duration={duration} mins");

            try
            {
                // convert eDataSingle
                EDataSingle eDataSingle = new EDataSingle(value);
                string json = this._s6GApp.Common.SerializeObject(eDataSingle);

                // log sql before
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"json={this._s6GApp.Common.CheckNullString(json)}");

                // exec sql
                bool result = this._db.StringSet(key, json, TimeSpan.FromSeconds(duration));

                // log sql after
                //Không ghi Log tránh việc thừa dữ liệu gây đầy ổ lưu trữ
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"json={this._s6GApp.Common.CheckNullString(json)}; result={result}");
                //this._s6GApp.SqlLogger.LogToFile(null, key, $"json={this._s6GApp.Common.CheckNullString(json)}; result={result}");

                return result; // success?
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return false; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// 2022-01-13 Linhnh
        /// luu string json xin trong redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public bool String_SetObject_noTimeOut(string key, object value)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"key={this._s6GApp.Common.CheckNullString(key)}, value=?, duration=-1 mins");

            try
            {
                // convert eDataSingle
                EDataSingle eDataSingle = new EDataSingle(value);
                string json = this._s6GApp.Common.SerializeObject(eDataSingle);

                // log sql before
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"json={this._s6GApp.Common.CheckNullString(json)}");

                // exec sql
                bool result = this._db.StringSet(key, json, null);
                // log sql after

                //Bỏ đoạn ghi log phần này cho bên CDC_Authen_DatT
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"json={this._s6GApp.Common.CheckNullString(json)}; result={result}");
                //this._s6GApp.SqlLogger.LogToFile(null, key, $"json={this._s6GApp.Common.CheckNullString(json)}; result={result}");

                return result; // success?
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return false; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// 2019-02-11 13:46:16 ngocta2
        /// lay value tu string key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string String_Get(string key, int databaseNumber = -1)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"databaseNumber={databaseNumber}; key={this._s6GApp.Common.CheckNullString(key)}");

            try
            {
                string value = "";

                lock (_locker)
                {
                    // change db, select db moi de truy van data
                    if (databaseNumber != -1 && databaseNumber != this._databaseNumber)
                        this._db = this._redis.GetDatabase(databaseNumber, null);

                    // exec sql
                    value = this._db.StringGet(key, CommandFlags.PreferReplica);

                    // change db, restore db cu
                    if (databaseNumber != -1 && databaseNumber != this._databaseNumber)
                        this._db = this._redis.GetDatabase(this._databaseNumber, null);
                }


                this._s6GApp.SqlLogger.LogToFile(null, key, $"databaseNumber={databaseNumber}; key={key}; value={(value == null ? 0 : value.Length)}");

                return value; // success
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return null; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// 2019-02-11 13:46:16 ngocta2
        /// lay value tu string key
        /// Xóa log Redis
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string String_Get2(string key, int databaseNumber = -1)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"databaseNumber={databaseNumber}; key={this._s6GApp.Common.CheckNullString(key)}");

            try
            {
                string value = "";

                lock (_locker)
                {
                    // change db, select db moi de truy van data
                    if (databaseNumber != -1 && databaseNumber != this._databaseNumber)
                        this._db = this._redis.GetDatabase(databaseNumber, null);

                    // exec sql
                    value = this._db.StringGet(key, CommandFlags.PreferReplica);

                    // change db, restore db cu
                    if (databaseNumber != -1 && databaseNumber != this._databaseNumber)
                        this._db = this._redis.GetDatabase(this._databaseNumber, null);
                }


                // log sql after
                // không lưu log tại các lib nữa -> gây ra tình trạng đầy ổ lưu
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"databaseNumber={databaseNumber}; key={key}; value ={this._s6GApp.Common.CheckNullString(value)}");
                //this._s6GApp.SqlLogger.LogToFile(null, key, $"databaseNumber={databaseNumber}; key={key}; value={this._s6GApp.Common.CheckNullString(value)}");

                return value; // success
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return null; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }


        /// <summary>
        /// 2019-02-11 14:02:26 ngocta2
        /// xoa key luu string data
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool String_Remove(string key)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"key={this._s6GApp.Common.CheckNullString(key)}");

            try
            {
                // exec sql
                bool result = this._db.KeyDelete(key);

                // log sql after
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"result={result}");
                //this._s6GApp.SqlLogger.LogToFile(null, key, $"result={result}");

                return result; // success?
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return false; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// 2019-02-11 14:38:16 ngocta2
        /// them row vao sorted set
        /// -------------------------------------------------
        /// 2019-02-26 10:48:40 ngocta2 xay ra truong hop log add row = true het nhung that ra ko thanh cong
        /// log ghi du co data add row 19,20,21,22,23 nhung trong redis chi co 19,23 ... mat 20,21,22
        //2019-02-26 10:23:55.430 +07:00 [INF] =================
        //Source  = BaseSERedisLib.Implementations.CRedisRepository->ZSet_AddRow => 53ac85a6-fdcf-47fc-9e69-155d5effd8ef(30) [3]
        //Data    = ec.Data=zKey=REALTIME:S6G__VN30, dataObject={"t":"2019-02-26 10:19","h":932.41,"l":931.69,"o":932.19,"c":931.8,"v":114560}; zScore=20190226102355428, zValue={"Time":"2019-02-26 10:23:55.429","Data":{"t":"2019-02-26 10:19","h":932.41,"l":931.69,"o":932.19,"c":931.8,"v":114560}}; result=True
        //2019-02-26 10:23:55.431 +07:00 [INF] =================
        //Source  = BaseSERedisLib.Implementations.CRedisRepository->ZSet_AddRow => c5009e4d-3551-4804-be73-dc2be439c1e8(30) [3]
        //Data    = ec.Data=zKey=REALTIME:S6G__VN30, dataObject={"t":"2019-02-26 10:20","h":932.41,"l":931.67,"o":931.84,"c":932.3,"v":271200}; zScore=20190226102355432, zValue={"Time":"2019-02-26 10:23:55.430","Data":{"t":"2019-02-26 10:20","h":932.41,"l":931.67,"o":931.84,"c":932.3,"v":271200}}; result=True
        //2019-02-26 10:23:55.432 +07:00 [INF] =================
        //Source  = BaseSERedisLib.Implementations.CRedisRepository->ZSet_AddRow => 97f47899-8e10-444f-9a32-9d0764593858 (30) [3]
        //Data    = ec.Data=zKey=REALTIME:S6G__VN30, dataObject={"t":"2019-02-26 10:21","h":932.31,"l":931.81,"o":932.28,"c":932.06,"v":236270}; zScore=20190226102355432, zValue={"Time":"2019-02-26 10:23:55.431","Data":{"t":"2019-02-26 10:21","h":932.31,"l":931.81,"o":932.28,"c":932.06,"v":236270}}; result=True
        //2019-02-26 10:23:55.433 +07:00 [INF] =================
        //Source  = BaseSERedisLib.Implementations.CRedisRepository->ZSet_AddRow => e638440b-5b96-4c1d-a951-fa2612bd8cf5(30) [3]
        //Data    = ec.Data=zKey=REALTIME:S6G__VN30, dataObject={"t":"2019-02-26 10:22","h":933.11,"l":931.87,"o":931.87,"c":932.92,"v":161010}; zScore=20190226102355432, zValue={"Time":"2019-02-26 10:23:55.432","Data":{"t":"2019-02-26 10:22","h":933.11,"l":931.87,"o":931.87,"c":932.92,"v":161010}}; result=True
        //2019-02-26 10:23:55.434 +07:00 [INF] =================
        //Source  = BaseSERedisLib.Implementations.CRedisRepository->ZSet_AddRow => 42930182-2ae2-46ba-9166-20f5b06b785f(30) [3]
        //Data    = ec.Data=zKey=REALTIME:S6G__VN30, dataObject={"t":"2019-02-26 10:23","h":932.96,"l":932.62,"o":932.88,"c":932.94,"v":91440}; zScore=20190226102355432, zValue={"Time":"2019-02-26 10:23:55.433","Data":{"t":"2019-02-26 10:23","h":932.96,"l":932.62,"o":932.88,"c":932.94,"v":91440}}; result=True
        /// </summary>
        /// <param name="zKey"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public bool ZSet_AddRow(string zKey, object dataObject, string symbol = null, string dateTimeInput = null)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}, dataObject={this._s6GApp.Common.SerializeObject(dataObject)}");

            try
            {
                //lock (_locker)
                {
                    // setup
                    EDataSingle eDataSingle = new EDataSingle(dataObject);
                    string zValue = this._s6GApp.Common.SerializeObject(eDataSingle);
                    long zScore = this._s6GApp.Common.CreateZScore(); // Convert.ToDouble(DateTime.Now.ToString(EGlobalConfig.DATETIME_REDIS_SCORE));

                    // tinh lai score (Linq)
                    if (symbol != null && dateTimeInput != null)
                        zScore = this._s6GApp.Common.CreateZScore4Insert(zKey, dateTimeInput);

                    //if (symbol == "ACB") this._s6GApp.Common.WriteFile(@"D:\ACB.js", $"(Z) zScore={zScore}; zKey={zKey}; symbol={symbol}; dateTimeInput={dateTimeInput}; dataObject={this._s6GApp.Common.SerializeObject(dataObject)}; ");

                    // log sql before
                    //this._s6GApp.SqlLogger.LogSqlContext(ec, $"zKey={zKey}, zScore={zScore}, zValue={zValue}");

                    // https://stackoverflow.com/questions/39476382/servicestack-redis-client-unknown-reply-on-integer-response-430k
                    // This is typically due to a race condition from sharing the same Redis Client instance across multiple threads. 
                    // You can share a singleton instance of ServiceStack.Redis Thread-safe Client Managers 
                    // but you shouldn't share instances of Redis Client across multiple threads.
                    var result = this._db.SortedSetAdd(zKey, zValue, zScore); // ERROR: Unknown reply on integer response: 58:0 => thu dung "var" thay "bool"
                                                                              //var result = true;// fix tam, debug xong can xoa dong nay 2019-03-25 14:01:18 ngocta2

                    // log sql after
                    //this._s6GApp.SqlLogger.LogSqlContext(ec, $"ec.Data={ec.Data}; zScore={Convert.ToInt64(zScore)}, zValue={zValue}; result={result}");
                    this._s6GApp.SqlLogger.LogToFile(null, zKey, $"zScore={zScore}; zValue={zValue}; result={result}");

                    return result; // success
                }
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return false; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// 2019-02-11 14:47:08 ngocta2
        /// FAST function, dung de import big data, 
        /// chi ghi log chi tiet voi truong hop error de xac dinh error khi dang xu ly data nao
        /// </summary>
        /// <param name="zKey"></param>
        /// <param name="zValue"></param>
        /// <param name="zScore"></param>
        /// <returns></returns>
        public bool ZSet_AddRow(string zKey, string zValue, long zScore, bool checkExistThenSkip = true)
        {
            // debug
            //TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}, zValue={this._s6GApp.Common.CheckNullString(zValue)}, zScore={Convert.ToInt64(zScore)}");

            try
            {
                // exec sql
                bool result = false;

                if (checkExistThenSkip)
                {
                    // checkExistThenSkip=true, phai kiem tra Key/Score da ton tai chua
                    //		+ neu data co ton tai thi ko add row nua
                    //		+ neu data ko ton tai thi add row 
                    SortedSetEntry[] sortedSetEntries = this._db.SortedSetRangeByScoreWithScores(zKey, zScore, zScore);

                    // da co row voi key/score nay roi
                    if (sortedSetEntries.Length > 0)
                        result = false; // da ton tai thi bo qua, return false
                    else
                        result = this._db.SortedSetAdd(zKey, zValue, zScore);// chua ton tai thi add row
                }
                else
                {
                    // checkExistThenSkip=false, khong check ton tai Key/Score, add row luon
                    // fast speed nhung co rui ro la bi double data => chu y
                    result = this._db.SortedSetAdd(zKey, zValue, zScore);
                }

                // log sql after
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"ec.Data={ec.Data}; result={result}");
                this._s6GApp.SqlLogger.LogToFile(null, zKey, $"zScore={zScore}; zValue={zValue}; result={result}");

                return result; // success
            }
            catch (Exception ex)
            {
                TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}, zValue={this._s6GApp.Common.CheckNullString(zValue)}, zScore={Convert.ToInt64(zScore)}");
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                //this._s6GApp.ErrorLogger.LogError(ex);

                return false; // failed
            }
            finally
            {
                // log debug
                //this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// 2019-02-11 14:47:08 ngocta2
        /// FAST function, dung de import big data, 
        /// chi ghi log chi tiet voi truong hop error de xac dinh error khi dang xu ly data nao
        /// </summary>
        /// <param name="zKey"></param>
        /// <param name="zValue"></param>
        /// <param name="zScore"></param>
        /// <returns></returns>
        public bool ZSet_AddRow(string zKey, object dataObject, long zScore, bool checkExistThenSkip = true)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}, dataObject={this._s6GApp.Common.SerializeObject(dataObject)}");

            try
            {
                EDataSingle eDataSingle = new EDataSingle(dataObject);
                string zValue = this._s6GApp.Common.SerializeObject(eDataSingle);
                // exec sql
                bool result = false;

                if (checkExistThenSkip)
                {
                    // checkExistThenSkip=true, phai kiem tra Key/Score da ton tai chua
                    //		+ neu data co ton tai thi ko add row nua
                    //		+ neu data ko ton tai thi add row 
                    SortedSetEntry[] sortedSetEntries = this._db.SortedSetRangeByScoreWithScores(zKey, zScore, zScore);

                    // da co row voi key/score nay roi
                    if (sortedSetEntries.Length > 0)
                        result = false; // da ton tai thi bo qua, return false
                    else
                        result = this._db.SortedSetAdd(zKey, zValue, zScore);// chua ton tai thi add row
                }
                else
                {
                    // checkExistThenSkip=false, khong check ton tai Key/Score, add row luon
                    // fast speed nhung co rui ro la bi double data => chu y
                    result = this._db.SortedSetAdd(zKey, zValue, zScore);
                }

                // log sql after
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"ec.Data={ec.Data}; result={result}");
                this._s6GApp.SqlLogger.LogToFile(null, zKey, $"zScore={zScore}; zValue={zValue}; result={result}");

                return result; // success
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                //this._s6GApp.ErrorLogger.LogError(ex);

                return false; // failed
            }
            finally
            {
                // log debug
                //this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// 2019-02-11 14:47:08 ngocta2
        /// FAST function, dung de import big data, 
        /// chi ghi log chi tiet voi truong hop error de xac dinh error khi dang xu ly data nao
        /// </summary>
        /// <param name="zKey"></param>
        /// <param name="zValue"></param>
        /// <param name="zScore"></param>
        /// <returns></returns>
        public bool ZSet_AddRow2(string zKey, object dataObject, long zScore, bool checkExistThenSkip = true)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}, dataObject={this._s6GApp.Common.SerializeObject(dataObject)}");

            try
            {
                EDataSingle eDataSingle = new EDataSingle(dataObject);
                string zValue = this._s6GApp.Common.SerializeObject(eDataSingle);
                // exec sql
                bool result = false;

                if (checkExistThenSkip)
                {
                    // checkExistThenSkip=true, phai kiem tra Key/Score da ton tai chua
                    //		+ neu data co ton tai thi ko add row nua
                    //		+ neu data ko ton tai thi add row 
                    SortedSetEntry[] sortedSetEntries = this._db.SortedSetRangeByScoreWithScores(zKey, zScore, zScore);

                    // da co row voi key/score nay roi
                    if (sortedSetEntries.Length > 0)
                        result = false; // da ton tai thi bo qua, return false
                    else
                        result = this._db.SortedSetAdd(zKey, zValue, zScore);// chua ton tai thi add row
                }
                else
                {
                    // checkExistThenSkip=false, khong check ton tai Key/Score, add row luon
                    // fast speed nhung co rui ro la bi double data => chu y
                    result = this._db.SortedSetAdd(zKey, zValue, zScore);
                }

                // log sql after
                //Không lưu log
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"ec.Data={ec.Data}; result={result}");
                //this._s6GApp.SqlLogger.LogToFile(null, zKey, $"zScore={zScore}; zValue={zValue}; result={result}");

                return result; // success
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                //this._s6GApp.ErrorLogger.LogError(ex);

                return false; // failed
            }
            finally
            {
                // log debug
                //this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /*
        // dic luu data de truyen vao redisRepo
        Dictionary<string, long> keyValuePairs = new Dictionary<string, long>();
        // loop tinh toan
        foreach (EJsonAData eJsonAData in this._binaryDic[symbol])
        {
            // setup
            EDataSingle eDataSingle = new EDataSingle(eJsonAData.Detail);
            string zValue = this._s6GApp.Common.SerializeObject(eDataSingle);
            long zScore = Convert.ToInt64(DateTime.Now.ToString(EGlobalConfig.DATETIME_REDIS_SCORE));
            keyValuePairs.Add(zValue, zScore);
        }
        // add batch  vao redis
        string zSet = __TEMPLATE_ZSET_DETAIL.Replace("(symbol)", symbol);// "DETAIL:S6G__(symbol)"                    
        this._redisRepository.ZSet_AddRows(zSet, keyValuePairs);
		 */
        /// <summary>
        /// 2019-02-11 14:50:32 ngocta2
        /// them nhieu row 1 luc
        /// https://csharp.hotexamples.com/examples/-/SortedSetEntry/-/php-sortedsetentry-class-examples.html
        /// </summary>
        /// <param name="zKey"></param>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public bool ZSet_AddRows(string zKey, IDictionary<string, long> keyValuePairs)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}, keyValuePairs={this._s6GApp.Common.SerializeObject(keyValuePairs)}");

            try
            {
                // init vars
                long total = keyValuePairs.Count, successCount = 0;
                int i = 0;

                // array luu cac data add batch
                SortedSetEntry[] sortedSetEntries = new SortedSetEntry[keyValuePairs.Count];

                // main                
                foreach (KeyValuePair<string, long> pair in keyValuePairs)
                {
                    sortedSetEntries[i++] = new SortedSetEntry(pair.Key, pair.Value);
                }

                // exec sql                    
                successCount = this._db.SortedSetAdd(zKey, sortedSetEntries);

                // log sql after
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"ec.Data={ec.Data}; successCount={successCount}");
                this._s6GApp.SqlLogger.LogToFile(null, zKey, $"keyValuePairs={this._s6GApp.Common.SerializeObject(keyValuePairs)}; successCount={successCount}");

                return successCount > 0; // success
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return false; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// 2019-02-11 15:01:12 ngocta2
        /// lay ra cac row theo score
        /// </summary>
        /// <param name="zKey"></param>
        /// <param name="fromScore"></param>
        /// <param name="toScore"></param>
        /// <returns></returns>
        public IDictionary<string, double> ZSet_GetRowsByRange(string zKey, long fromScore, long toScore)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}, fromScore={fromScore}, toScore={toScore}");

            try
            {
                // exec sql
                SortedSetEntry[] sortedSetEntries = this._db.SortedSetRangeByScoreWithScores(zKey, fromScore, toScore, Exclude.None, Order.Ascending, 0, -1, CommandFlags.PreferReplica);
                Dictionary<string, double> dic = new Dictionary<string, double>();
                foreach (SortedSetEntry sortedSetEntry in sortedSetEntries)
                    dic.Add(sortedSetEntry.Element, sortedSetEntry.Score);

                // log sql after
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"ec.Data={ec.Data}; dic={this._s6GApp.Common.SerializeObject(dic)}");
                //this._s6GApp.SqlLogger.LogToFile(null, zKey, $"fromScore={fromScore}; toScore={toScore}; dic={this._s6GApp.Common.SerializeObject(dic)}");

                return dic; // success
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return null; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }
        /// <summary>
        /// 2019-02-11 15:01:12 ngocta2
        /// lay ra cac row theo score desc
        /// </summary>
        /// <param name="zKey"></param>
        /// <param name="fromScore"></param>
        /// <param name="toScore"></param>
        /// <returns></returns>
        public IDictionary<string, double> ZSet_GetRowsByRangeDesc(string zKey, long fromScore, long toScore)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}, fromScore={fromScore}, toScore={toScore}");

            try
            {
                //long addscore = fromScore + 1;
                // exec sql
                SortedSetEntry[] sortedSetEntries = this._db.SortedSetRangeByScoreWithScores(zKey, fromScore, toScore, Exclude.Start, Order.Descending, 0, -1, CommandFlags.PreferReplica);
                Dictionary<string, double> dic = new Dictionary<string, double>();
                foreach (SortedSetEntry sortedSetEntry in sortedSetEntries)
                    dic.Add(sortedSetEntry.Element, sortedSetEntry.Score);

                // log sql after
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"ec.Data={ec.Data}; dic={this._s6GApp.Common.SerializeObject(dic)}");
                //this._s6GApp.SqlLogger.LogToFile(null, zKey, $"fromScore={fromScore}; toScore={toScore}; dic={this._s6GApp.Common.SerializeObject(dic)}");

                return dic; // success
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return null; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }
        /// <summary>
        /// 2019-02-11 15:18:29 ngocta2
        /// chi lay value cua row co score to nhat
        /// </summary>
        /// <param name="zKey"></param>
        /// <returns></returns>
        public string ZSet_GetValueWithHighestScore(string zKey)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}");

            try
            {
                // exec sql
                RedisValue[] result = this._db.SortedSetRangeByRank(zKey, 0, 0, Order.Descending, CommandFlags.PreferReplica);

                if (result.Length > 0)
                {
                    return result[0]; // success
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return null; // failed
            }
        }

        /// <summary>
        /// 2019-02-11 15:20:59 ngocta2
        /// lay value cua row co score be nhat
        /// </summary>
        /// <param name="zKey"></param>
        /// <returns></returns>
        public string ZSet_GetValueWithLowestScore(string zKey)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}");

            try
            {
                // exec sql
                RedisValue[] result = this._db.SortedSetRangeByRank(zKey, 0, 0, Order.Ascending, CommandFlags.PreferReplica);

                // log sql after
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"ec.Data={ec.Data}; result={this._s6GApp.Common.SerializeObject(result)}");
                this._s6GApp.SqlLogger.LogToFile(null, zKey, $"result={this._s6GApp.Common.SerializeObject(result)}");

                return result[0]; // success
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return null; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// 2019-02-11 14:54:54 ngocta2
        /// xoa cac row cua sorted set theo score (from, to)
        /// </summary>
        /// <param name="zKey"></param>
        /// <param name="fromScore"></param>
        /// <param name="toScore"></param>
        /// <returns></returns>
        public long ZSet_RemoveRows(string zKey, long fromScore, long toScore)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}, fromScore={fromScore}, toScore={toScore}");

            try
            {
                // exec sql
                long result = this._db.SortedSetRemoveRangeByScore(zKey, fromScore, toScore);

                // log sql after
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"ec.Data={ec.Data}; result={result}");
                this._s6GApp.SqlLogger.LogToFile(null, zKey, $"fromScore={fromScore}; toScore={toScore}; result={result}");

                return result; // success
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return 0; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// 2019-02-11 14:54:54 ngocta2
        /// xoa cac row cua sorted set theo score (from, to)
        /// </summary>
        /// <param name="zKey"></param>
        /// <param name="fromScore"></param>
        /// <param name="toScore"></param>
        /// <returns></returns>
        public long ZSet_RemoveRows2(string zKey, long fromScore, long toScore)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}, fromScore={fromScore}, toScore={toScore}");

            try
            {
                // exec sql
                long result = this._db.SortedSetRemoveRangeByScore(zKey, fromScore, toScore);

                // log sql after
                //Không ghi log gây thừa dữ liệu, đầy ổ lưu
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"ec.Data={ec.Data}; result={result}");
                //this._s6GApp.SqlLogger.LogToFile(null, zKey, $"fromScore={fromScore}; toScore={toScore}; result={result}");

                return result; // success
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return 0; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// 2019-02-11 15:12:40 ngocta2
        /// </summary>
        /// <param name="zKey"></param>
        /// <param name="fromRank"></param>
        /// <param name="toRank"></param>
        /// <returns></returns>
        public IDictionary<string, double> ZSet_GetRowsByRankDesc(string zKey, int fromRank, int toRank)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}, fromRank={fromRank}, toRank={toRank}");

            StockCore.Redis.MW.DataMW MW_D = new StockCore.Redis.MW.DataMW();
            try
            {
                // exec sql

                SortedSetEntry[] sortedSetEntries = this._db.SortedSetRangeByRankWithScores(zKey, fromRank, toRank, Order.Descending, CommandFlags.PreferReplica);
                Dictionary<string, double> dic = new Dictionary<string, double>();
                foreach (SortedSetEntry sortedSetEntry in sortedSetEntries)
                    dic.Add(sortedSetEntry.Element, sortedSetEntry.Score);

                this._s6GApp.SqlLogger.LogToFile(null, zKey, $"fromRank={fromRank}; toRank={toRank}; dic.Count={dic.Count}; result={/*this._s6GApp.Common.SerializeObject(dic)*/""}");

                return dic; // success
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return null; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }
        public IDictionary<string, double> ZSet_GetRowsByRankASC(string zKey, int fromRank, int toRank)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}, fromRank={fromRank}, toRank={toRank}");

            StockCore.Redis.MW.DataMW MW_D = new StockCore.Redis.MW.DataMW();
            try
            {
                // exec sql

                SortedSetEntry[] sortedSetEntries = this._db.SortedSetRangeByRankWithScores(zKey, fromRank, toRank, Order.Ascending, CommandFlags.PreferReplica);
                Dictionary<string, double> dic = new Dictionary<string, double>();
                foreach (SortedSetEntry sortedSetEntry in sortedSetEntries)
                    dic.Add(sortedSetEntry.Element, sortedSetEntry.Score);

                this._s6GApp.SqlLogger.LogToFile(null, zKey, $"fromRank={fromRank}; toRank={toRank}; dic.Count={dic.Count}; result={/*this._s6GApp.Common.SerializeObject(dic)*/""}");

                return dic; // success
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return null; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }
        //private static string EscapeForJson(string s)
        //{
        //	try
        //	{
        //		// 2021-03-09 13:16:12 ngocta2
        //		// redis co row thi lay, ko co row thi tra null
        //		if (s == null)
        //			return null;

        //		string quoted = System.Web.Helpers.Json.Encode(s);
        //		return quoted.Substring(1, quoted.Length - 2);
        //	}
        //	catch (Exception ex)
        //	{
        //		return s;
        //	}

        //}

        /// <summary>
        /// 2019-02-01 10:33:06 ngocta2
        /// 1. get value V, score C cua row co highest score
        /// 2. serialize oldDataObject thanh string SO
        /// 3. compare string SO va V bat dau tu vi tri compareOffset
        /// 4. if SO == V bat dau tu vi tri compareOffset thi : 
        ///     a. delete row co score from C, to C 
        ///         (delete theo C vi neu dung highest score co the luc nay ko dung vi da co them row khac)
        ///     b. add row moi co value = serialzie newDataObject
        ///     
        /// CHU Y: chi ap dung cho value la serialize cua StockCore.TAChart.Entities.EDataBlock
        /// 
        /// 2019-02-11 15:23:08 ngocta2
        /// viet lai cho StackExchange
        /// </summary>
        /// <param name="zKey"></param>
        /// <param name="oldDataObject"></param>
        /// <param name="newDataObject"></param>
        /// <param name="compareOffset"></param>
        /// <returns></returns>
        //public bool ZSet_UpdateRow(string zKey, object oldDataObject, object newDataObject, int compareOffset = 0, int lengthOffset = 0)
        //{
        //    if (compareOffset == 0) compareOffset = 41;
        //    if (lengthOffset == 0) lengthOffset = 23;

        //    // debug
        //    TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}, oldDataObject={this._s6GApp.Common.SerializeObject(oldDataObject)}, newDataObject={this._s6GApp.Common.SerializeObject(newDataObject)}, compareOffset={compareOffset}");

        //    try
        //    {
        //        // setup
        //        bool result = false;
        //        string oldValueInMemoryToCompare = this._s6GApp.Common.SerializeObject(oldDataObject).Substring(0, lengthOffset);
        //        IDictionary<string, double> keyValuePairs = ZSet_GetRowsByRankDesc(zKey, 0, 0);
        //        // ko co data cu de update => insert new
        //        if (keyValuePairs.Count == 0)
        //        {
        //            ZSet_AddRow(zKey, newDataObject);
        //            return true;
        //        }
        //        // so sanh de tim dung la row can update
        //        string oldValueInRedis = (new List<string>(keyValuePairs.Keys))[0];
        //        long oldScoreInRedis = (long)(new List<double>(keyValuePairs.Values))[0];

        //        // log buffer mid
        //        _s6GApp.DebugLogger.WriteBufferMid(ec, $"oldValueInRedis={oldValueInRedis}, oldScoreInRedis={oldScoreInRedis}, oldValueToOverwrite={oldValueInMemoryToCompare}");

        //        string oldValueInRedisToCompare = oldValueInRedis.Substring(compareOffset, lengthOffset);
        //        //oldValueInRedisToCompare = oldValueInRedisToCompare.Substring(0, oldValueInRedisToCompare.Length - 1);

        //        // insert row moi
        //        result = ZSet_AddRow(zKey, newDataObject);

        //        // log buffer mid
        //        _s6GApp.DebugLogger.WriteBufferMid(ec, $"oldValueInMemoryToCompare={oldValueInMemoryToCompare}, oldValueInRedisToCompare={oldValueInRedisToCompare}");

        //        //{"t":"2019-02-01 11:28","o":1059.7,"h":1082.7,"l":1059.7,"c":1082.7,"v":36} => {"t":"2019-02-01 11:28" => index=41,len=23
        //        if (String.CompareOrdinal(oldValueInMemoryToCompare, oldValueInRedisToCompare) == 0)
        //        {
        //            // xoa row theo old score
        //            ZSet_RemoveRows(zKey, oldScoreInRedis, oldScoreInRedis);
        //        }

        //        // insert row moi
        //        //result = ZSet_AddRow(zKey, newDataObject);

        //        // log sql after
        //        this._s6GApp.SqlLogger.LogSqlContext(ec, $"ec.Data={ec.Data} ;result={result}");

        //        return result; // success
        //    }
        //    catch (Exception ex)
        //    {
        //        this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
        //        return false; // failed
        //    }
        //    finally
        //    {
        //        // log debug
        //        this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
        //    }
        //}


        /// <summary>
        /// 2019-03-05 10:54:16 ngocta2
        /// update row theo score
        /// neu da ton tai row co score = zScore thi 
        /// 1. remove row theo zScore
        /// 2. add row theo zKey, zValue, zScore
        /// </summary>
        /// <param name="zKey"></param>
        /// <param name="zValue"></param>
        /// <param name="zScore"></param>
        /// <returns></returns>
        public bool ZSet_UpdateRow(string zKey, string zValue, long zScore)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"zKey={this._s6GApp.Common.CheckNullString(zKey)}, zValue={this._s6GApp.Common.SerializeObject(zValue)}, zScore={zScore}");

            try
            {
                // setup
                long removeResult = 0;
                bool addResult = false;

                // kiem tra zSet da co row nao chua
                //IDictionary<string, double> keyValuePairs = ZSet_GetRowsByRange(zKey, zScore, zScore);
                SortedSetEntry[] sortedSetEntries = this._db.SortedSetRangeByScoreWithScores(zKey, zScore, zScore);
                Dictionary<string, long> keyValuePairs = new Dictionary<string, long>();
                foreach (SortedSetEntry sortedSetEntry in sortedSetEntries)
                    keyValuePairs.Add(sortedSetEntry.Element, Convert.ToInt64(sortedSetEntry.Score));

                // da co row voi score x => xoa row 
                if (keyValuePairs.Count > 0)
                    removeResult = this._db.SortedSetRemoveRangeByScore(zKey, zScore, zScore);

                // add row voi score x
                addResult = this._db.SortedSetAdd(zKey, zValue, zScore);

                // log sql after
                //this._s6GApp.SqlLogger.LogSqlContext(ec, $"zKey={zKey}; zScore={zScore}; keyValuePairs.Count={keyValuePairs.Count}; removeResult={removeResult} ; addResult={addResult}");
                this._s6GApp.SqlLogger.LogToFile(null, zKey, $"zScore={zScore}; zValue={zValue}; keyValuePairs.Count={keyValuePairs.Count}; removeResult={removeResult}; addResult={addResult}");

                return addResult; // success
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return false; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// 2022-06-03 10:30:16 tiepbx
        /// lay value tu hash key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<HashKeyRedis> Hash_Get_All(string key)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"key={this._s6GApp.Common.CheckNullString(key)}");

            try
            {
                // exec sql
                HashEntry[] hashSetEntries = this._db.HashGetAll(key, CommandFlags.PreferReplica);
                List<HashKeyRedis> lstHashKey = new List<HashKeyRedis>(hashSetEntries.Length);


                //Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (HashEntry item in hashSetEntries)
                {
                    HashKeyRedis objEach = new HashKeyRedis();
                    objEach.Key = item.Name;
                    objEach.Value = item.Value;
                    lstHashKey.Add(objEach);
                }

                //dic.Add(item.Name, item.Value);

                //this._s6GApp.SqlLogger.LogToFile(null, key, $"dic.Count={lstHashKey.Count}; result={this._s6GApp.Common.SerializeObject(lstHashKey)}");

                return lstHashKey; // success
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return null; // failed
            }
            finally
            {
                // log debug
                this._s6GApp.DebugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// Linhnh 20220608 Thêm hashSet phục vụ cho việc thêm hash-key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// 
        /// <returns></returns>
        public bool HashSet(string key, string hashField, object value)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"key={this._s6GApp.Common.CheckNullString(key)}");

            try
            {
                EDataSingle eDataSingle = new EDataSingle(value);
                string json = this._s6GApp.Common.SerializeObject(eDataSingle);

                bool result = this._db.HashSet(key, hashField, json);
                return result;
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return false;
            }

        }
        public string Hash_Get(string key, string field)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"key={this._s6GApp.Common.CheckNullString(key)}");
            try
            {
                RedisValue value = this._db.HashGet(key, field);
                if (value.IsNullOrEmpty)
                {
                    return null;
                }
                return value.ToString();
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return null;
            }
        }
        public bool HashDelete(string key, string hashField)
        {
            // debug
            TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"key={this._s6GApp.Common.CheckNullString(key)}");
            try
            {
                bool result = this._db.HashDelete(key, hashField);
                return result;
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                return false;
            }
        }

        public class HashKeyRedis
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
        public bool Key_Exists(string key)
        {
            return this._db.KeyExists(key);
        }
    }
}
