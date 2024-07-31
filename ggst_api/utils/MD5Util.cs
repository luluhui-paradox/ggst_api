using System.Security.Cryptography;
using System.Text;

namespace ggst_api.utils
{
    public class MD5Util
    {
        public static string ComputeMd5Hash(string input)
        {
            // 创建 MD5 实例
            using (MD5 md5 = MD5.Create())
            {
                // 将输入字符串转换为字节数组
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);

                // 计算哈希值
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // 将字节数组转为十六进制字符串
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // 转换为十六进制格式
                }
                return sb.ToString();
            }
        }
    }
}
