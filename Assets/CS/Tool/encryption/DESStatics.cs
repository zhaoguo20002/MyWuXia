using UnityEngine;

namespace Game  
{  
    public class DESStatics  
    {  

        #region 加密解密  

        public static DES dd = new DES();  

        static string stringKey = "zyzkmqzg"; // 加密密钥  

        public static void Init() {
            string baseKey = "zyzkmqzg";
            string key = TalkingDataGA.GetDeviceId() != null ? TalkingDataGA.GetDeviceId() : "";
            if (key.Length < 8)
            {
                int len = 8 - key.Length;
                int index = 0;
                while (len-- > 0)
                {
                    key += baseKey[index++];  
                }
            }
            else if (key.Length > 8)
            {
                key = key.Substring(0, 8);
            }
            stringKey = key;
        }

        /// <summary>  
        /// 解密  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static string StringDecder(string str, string key = "")  
        {  
            return dd.DesDecrypt(str, string.IsNullOrEmpty(key) ? stringKey : key);  
        }  

        /// <summary>  
        /// 加密  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static string StringEncoder(string str, string key = "")  
        {  
            return dd.DesEncrypt(str, string.IsNullOrEmpty(key) ? stringKey : key);  
        }  

        #endregion   

    }  
}
