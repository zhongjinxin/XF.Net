using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XF.Core.CacheManager
{
    public interface ICacheService : IDisposable
    {
        bool Exists(string key);
        void LPush(string key, string val);
        void RPush(string key, string val);
        object ListDequeue(string key);
        T ListDequeue<T>(string key) where T : class;

        /// <summary>
        /// 移除list中的数据，keepIndex为保留的位置到最后一个元素如list 元素为1.2.3.....100
        /// 需要移除前3个数，keepindex应该为4
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keepIndex"></param>ss
        void ListRemove(string key, int keepIndex);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresIn">缓存时长</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间） //new TimeSpan(0, 60, 0);</param>
        /// <returns></returns>
        bool AddObject(string key, object value, int expireSeconds = -1, bool isSliding = false);

        bool Add(string key, string value, int expireSeconds = -1, bool isSliding = false);
        bool Remove(string key);
        void RemoveAll(IEnumerable<string> keys);
        T Get<T>(string key) where T : class;
        string Get(string key);
    }
}
