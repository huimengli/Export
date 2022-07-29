using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Export.Tools
{
    /// <summary>
    /// 工具类
    /// </summary>
    class Item
    {
        /// <summary>
        /// SHA256签名
        /// (不适用于签名中文内容,中文加密和js上的加密不同)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SHA256(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// MD5签名
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string MD5(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var hash = MD5CryptoServiceProvider.Create().ComputeHash(bytes);

            var builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// MD5签名
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string MD5(byte[] data)
        {
            var hash = MD5CryptoServiceProvider.Create().ComputeHash(data);

            var builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// 新建一个UID
        /// </summary>
        /// <returns></returns>
        public static string NewUUID()
        {
            return NewUUID(DateTime.Now.ToJSTime().ToString());
        }

        /// <summary>
        /// 新建一个UID
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string NewUUID(string input)
        {
            input = MD5(input);
            var sha = SHA256(input);
            var uuid = "uuxxxuuy-1xxx-7xxx-yxxx-xxx0xxxy";
            var temp = new StringBuilder();
            var dex = new StringBuilder();
            for (int i = 0; i < uuid.Length; i++)
            {
                var e = uuid[i];
                if (e == 'u')
                {
                    temp.Append(sha[2 * i]);
                    dex.Append(sha[2 * i]);
                }
                else if (e == 'x')
                {
                    temp.Append(input[i]);
                    dex.Append(input[i]);
                }
                else if (e == 'y')
                {
                    temp.Append(MD5(dex.ToString())[i]);
                }
                else
                {
                    temp.Append(e);
                }
            }
            uuid = temp.ToString();
            return uuid;
        }


    }

    /// <summary>
    /// 工具类追加函数
    /// </summary>
    public static class ItemAdd
    {
        /// <summary>
        /// 获取js的时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ToJSTime(this DateTime time)
        {
            var ret = time.ToFileTime();

            ret -= new DateTime(1970, 1, 1, 8, 0, 0).ToFileTime();
            //ret = Math.Floor(ret / 10000);
            ret = ret / 10000;

            return ret;
        }
    }
}
