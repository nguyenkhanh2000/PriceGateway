using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemCore.Interfaces;
using static BaseRedisLib.Implementations.CRedisRepository;

namespace BaseRedisLib.Interfaces
{
    public interface IRedisRepository : IInstance
    {
        IServer GetServer();
        // props
        object ConnectionMultiplexer { get; }
        object Subscriber { get; }

        // methods
        string String_Get(string key, int databaseNumber = -1);
        string String_Get2(string key, int databaseNumber = -1);
        bool String_Remove(string key);
        //bool String_Set(string key, string value, int duration);
        bool String_SetObject(string key, object value, int duration);
        bool String_SetObject2(string key, object value, int duration);
        bool String_SetObject_noTimeOut(string key, object value); //LinhNH 2022-01-13
        bool ZSet_AddRow(string zKey, object dataObject, string symbol = null, string dateTimeInput = null);
        //bool ZSet_AddRow_New(string zKey, object dataObject, string symbol = null, string dateTimeInput = null);
        bool ZSet_AddRow(string zKey, string zValue, long zScore, bool checkExistThenSkip = true);
        bool ZSet_AddRow(string zKey, object dataObject, long zScore, bool checkExistThenSkip = true);
        bool ZSet_AddRow2(string zKey, object dataObject, long zScore, bool checkExistThenSkip = true);
        bool ZSet_AddRows(string zKey, IDictionary<string, long> keyValuePairs);
        IDictionary<string, double> ZSet_GetRowsByRange(string zKey, long fromScore, long toScore);
        IDictionary<string, double> ZSet_GetRowsByRangeDesc(string zKey, long fromScore, long toScore);

        IDictionary<string, double> ZSet_GetRowsByRankDesc(string zKey, int fromRank, int toRank);
        IDictionary<string, double> ZSet_GetRowsByRankASC(string zKey, int fromRank, int toRank);
        string ZSet_GetValueWithHighestScore(string zKey);
        string ZSet_GetValueWithLowestScore(string zKey);
        long ZSet_RemoveRows(string zKey, long fromScore, long toScore);
        long ZSet_RemoveRows2(string zKey, long fromScore, long toScore);
        //bool ZSet_UpdateRow(string zKey, object oldDataObject, object newDataObject, int compareOffset = 0, int lengthOffset = 0);
        bool ZSet_UpdateRow(string zKey, string zValue, long zScore);
        List<HashKeyRedis> Hash_Get_All(string key);
        bool HashSet(string key, string hashField, object value);
        string Hash_Get(string key, string field);
        bool HashDelete(string key, string hashField);
        bool Key_Exists(string key);        
    }
}
