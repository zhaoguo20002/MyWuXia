using System;  
using System.Collections.Generic;  
using System.Linq;  
using System.Text;  
using System.Security.Cryptography;  
using System.IO;  

namespace Game  
{  
    /// <summary>  
    /// DES加密解密算法  
    /// </summary>  
    public class DES  
    {  
        /// <summary>  
        /// DES加密算法  
        /// sKey为8位或16位  
        /// </summary>  
        /// <param name="pToEncrypt">需要加密的字符串</param>  
        /// <param name="sKey">密钥</param>  
        /// <returns></returns>  
        public string DesEncrypt(string pToEncrypt, string sKey)  
        {  
            StringBuilder ret = new StringBuilder();  

            try  
            {  
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();  
                byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);  
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);  
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);  
                MemoryStream ms = new MemoryStream();  
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);  
                cs.Write(inputByteArray, 0, inputByteArray.Length);  
                cs.FlushFinalBlock();  

                foreach (byte b in ms.ToArray())  
                {  
                    ret.AppendFormat("{0:X2}", b);  
                }  
                ret.ToString();  
            }  
            catch  
            {  

            }  
            return ret.ToString();  
            //return a;  
        }  
        /// <summary>  
        /// DES解密算法  
        /// sKey为8位或16位  
        /// </summary>  
        /// <param name="pToDecrypt">需要解密的字符串</param>  
        /// <param name="sKey">密钥</param>  
        /// <returns></returns>  
        public string DesDecrypt(string pToDecrypt, string sKey)  
        {    
            MemoryStream ms = new MemoryStream();  

            try  
            {  
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();  
                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];  
                for (int x = 0; x < pToDecrypt.Length / 2; x++)  
                {  
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));  
                    inputByteArray[x] = (byte)i;  
                }  
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);  
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);  

                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);  
                cs.Write(inputByteArray, 0, inputByteArray.Length);  
                cs.FlushFinalBlock();  
                StringBuilder ret = new StringBuilder();  

            }  
            catch  
            {  

            }   

            return System.Text.Encoding.UTF8.GetString(ms.ToArray());  
        }  
    }  

    /// <summary>    
    /// 对称加密解密算法类    
    /// </summary>    
    public class Rijndael  
    {  
        private SymmetricAlgorithm mobjCryptoService;  
        private string Key;  
        /// <summary>    
        /// 对称加密类的构造函数    
        /// </summary>    
        public void SymmetricMethod()  
        {  
            mobjCryptoService = new RijndaelManaged();  
            Key = "Guz(%&hj7x89H$yuBI0456FtmaT5&fvHUFCy76*h%(HilJ$lhj!y6&(*jkP87jH7";  
        }  
        /// <summary>    
        /// 获得密钥    
        /// </summary>    
        /// <returns>密钥</returns>    
        private byte[] GetLegalKey()  
        {  
            string sTemp = Key;  
            mobjCryptoService.GenerateKey();  
            byte[] bytTemp = mobjCryptoService.Key;  
            int KeyLength = bytTemp.Length;  
            if (sTemp.Length > KeyLength)  
                sTemp = sTemp.Substring(0, KeyLength);  
            else if (sTemp.Length < KeyLength)  
                sTemp = sTemp.PadRight(KeyLength, ' ');  
            return ASCIIEncoding.ASCII.GetBytes(sTemp);  
        }  
        /// <summary>    
        /// 获得初始向量IV    
        /// </summary>    
        /// <returns>初试向量IV</returns>    
        private byte[] GetLegalIV()  
        {  
            string sTemp = "aclejaspwejgjdjfasrweojksdf$%#@!^!@&#*ajke^!@#jka";     //key 随便写。。  
            mobjCryptoService.GenerateIV();  
            byte[] bytTemp = mobjCryptoService.IV;  
            int IVLength = bytTemp.Length;  
            if (sTemp.Length > IVLength)  
                sTemp = sTemp.Substring(0, IVLength);  
            else if (sTemp.Length < IVLength)  
                sTemp = sTemp.PadRight(IVLength, ' ');  
            return ASCIIEncoding.ASCII.GetBytes(sTemp);  
        }  
        /// <summary>    
        /// 加密方法    
        /// </summary>    
        /// <param name="Source">待加密的串</param>    
        /// <returns>经过加密的串</returns>    
        public string Encrypto(string Source)  
        {  
            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);  
            MemoryStream ms = new MemoryStream();  
            mobjCryptoService.Key = GetLegalKey();  
            mobjCryptoService.IV = GetLegalIV();  
            ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();  
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);  
            cs.Write(bytIn, 0, bytIn.Length);  
            cs.FlushFinalBlock();  
            ms.Close();  
            byte[] bytOut = ms.ToArray();  
            return Convert.ToBase64String(bytOut);  
        }  
        /// <summary>    
        /// 解密方法    
        /// </summary>    
        /// <param name="Source">待解密的串</param>    
        /// <returns>经过解密的串</returns>    
        public string Decrypto(string Source)  
        {  
            byte[] bytIn = Convert.FromBase64String(Source);  
            MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);  
            mobjCryptoService.Key = GetLegalKey();  
            mobjCryptoService.IV = GetLegalIV();  
            ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();  
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);  
            StreamReader sr = new StreamReader(cs);  
            return sr.ReadToEnd();  
        }  
    }  


}