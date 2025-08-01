using DNAS.Application.Common.Interface;
using DNAS.Domian.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DNAS.Application.Common.Implementation
{
	internal class Encryption(IOptions<AppConfig> appConfig, IHttpContextAccessor haccess) : IEncryption
	{
		private readonly string UserId = haccess.HttpContext?.User.FindFirstValue("UserId")!;

		public string AesEncrypt(string PlainText)
		{
			try
			{
				using (var aes = Aes.Create())
				{
					string secret = appConfig.Value.EnSheKeValue; //encryption secret
					if (!string.IsNullOrEmpty(UserId))
					{
						secret = secret.Substring(0, secret.Length - UserId.Length);
						secret = string.Concat(secret, UserId);
					}
					aes.Key = CreateAesKey(secret);
					aes.Mode = CipherMode.CBC;
					aes.Padding = PaddingMode.PKCS7;

					aes.GenerateIV(); // Generate a new IV for each encryption
					var iv = aes.IV;

					using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
					using (var ms = new MemoryStream())
					{
						ms.Write(iv, 0, iv.Length); // Prepend IV to the encrypted data

						using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
						{
							var plainBytes = Encoding.UTF8.GetBytes(PlainText);
							cs.Write(plainBytes, 0, plainBytes.Length);
							cs.FlushFinalBlock();
						}

						return Convert.ToBase64String(ms.ToArray()).Replace("+", "%2b").Replace("=", "%3D");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return string.Empty;
			}
		}

		public string AesDecrypt(string CipherText)
		{
			try
			{
				var cipherBytes = Convert.FromBase64String(CipherText.Replace("%2b", "+").Replace("%3D", "="));

				using (var aes = Aes.Create())
				{
					string secret = appConfig.Value.EnSheKeValue; //encryption secret
					if (!string.IsNullOrEmpty(UserId))
					{
						secret = secret.Substring(0, secret.Length - UserId.Length);
						secret = string.Concat(secret, UserId);
					}
					aes.Key = CreateAesKey(secret);
					aes.Mode = CipherMode.CBC;
					aes.Padding = PaddingMode.PKCS7;

					var iv = new byte[16]; // AES block size is 16 bytes
					Array.Copy(cipherBytes, 0, iv, 0, iv.Length);

					var actualCipher = new byte[cipherBytes.Length - iv.Length];
					Array.Copy(cipherBytes, iv.Length, actualCipher, 0, actualCipher.Length);

					aes.IV = iv;

					using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
					using (var ms = new MemoryStream(actualCipher))
					using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
					{
						using (var sr = new StreamReader(cs, Encoding.UTF8))
						{
							return sr.ReadToEnd();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return string.Empty;
			}
		}

        public string AesEncryptForEmail(string PlainText)
        {
            try
            {
                using (var aes = Aes.Create())
                {
                    string secret = appConfig.Value.EnSheKeValue; //encryption secret
                    
                    aes.Key = CreateAesKey(secret);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    aes.GenerateIV(); // Generate a new IV for each encryption
                    var iv = aes.IV;

                    using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(iv, 0, iv.Length); // Prepend IV to the encrypted data

                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            var plainBytes = Encoding.UTF8.GetBytes(PlainText);
                            cs.Write(plainBytes, 0, plainBytes.Length);
                            cs.FlushFinalBlock();
                        }

                        return Convert.ToBase64String(ms.ToArray()).Replace("+", "%2b").Replace("=", "%3D");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public string AesDecryptForEmail(string CipherText)
        {
            try
            {
                var cipherBytes = Convert.FromBase64String(CipherText.Replace("%2b", "+").Replace("%3D", "="));

                using (var aes = Aes.Create())
                {
                    string secret = appConfig.Value.EnSheKeValue; //encryption secret
                    
                    aes.Key = CreateAesKey(secret);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    var iv = new byte[16]; // AES block size is 16 bytes
                    Array.Copy(cipherBytes, 0, iv, 0, iv.Length);

                    var actualCipher = new byte[cipherBytes.Length - iv.Length];
                    Array.Copy(cipherBytes, iv.Length, actualCipher, 0, actualCipher.Length);

                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream(actualCipher))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs, Encoding.UTF8))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public string AesEncrypt(string PlainText, string enckey)
		{
			try
			{
				using (var aes = Aes.Create())
				{
					aes.Key = CreateAesKey(enckey);
					aes.Mode = CipherMode.CBC;
					aes.Padding = PaddingMode.PKCS7;

					aes.GenerateIV(); // Generate a new IV for each encryption
					var iv = aes.IV;

					using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
					using (var ms = new MemoryStream())
					{
						ms.Write(iv, 0, iv.Length); // Prepend IV to the encrypted data

						using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
						{
							var plainBytes = Encoding.UTF8.GetBytes(PlainText);
							cs.Write(plainBytes, 0, plainBytes.Length);
							cs.FlushFinalBlock();
						}

						return Convert.ToBase64String(ms.ToArray()).Replace("+", "%2b").Replace("=", "%3D");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return string.Empty;
			}
		}

		public string AesDecrypt(string CipherText, string enckey)
		{
			try
			{
				var cipherBytes = Convert.FromBase64String(CipherText.Replace("%2b", "+").Replace("%3D", "="));
				using (var aes = Aes.Create())
				{
					aes.Key = CreateAesKey(enckey);
					aes.Mode = CipherMode.CBC;
					aes.Padding = PaddingMode.PKCS7;

					var iv = new byte[16]; // AES block size is 16 bytes
					Array.Copy(cipherBytes, 0, iv, 0, iv.Length);

					var actualCipher = new byte[cipherBytes.Length - iv.Length];
					Array.Copy(cipherBytes, iv.Length, actualCipher, 0, actualCipher.Length);

					aes.IV = iv;

					using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
					using (var ms = new MemoryStream(actualCipher))
					using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
					{
						using (var sr = new StreamReader(cs, Encoding.UTF8))
						{
							return sr.ReadToEnd();
						}
					}
				}
			}
			catch (Exception exp)
			{
				Console.WriteLine(exp.Message);
				return string.Empty;
			}
		}


		private static byte[] CreateAesKey(string inputString)

		{
			return Encoding.UTF8.GetByteCount(inputString) == 32 ? Encoding.UTF8.GetBytes(inputString) : SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString));
		}


		public string DecryptStringAES(string CifherText)
		{

			var keybytes = Encoding.UTF8.GetBytes(appConfig.Value.EnSheKeValue);
			var iv = Encoding.UTF8.GetBytes(appConfig.Value.EnSheKeValue);

			var encrypted = Convert.FromBase64String(CifherText);
			var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
			return string.Format(decriptedFromJavascript);
		}
		public string DecryptStringAES(string CifherText, string key)
		{

			var keybytes = Encoding.UTF8.GetBytes(key);
			var iv = Encoding.UTF8.GetBytes(key);

			var encrypted = Convert.FromBase64String(CifherText);
			var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
			return string.Format(decriptedFromJavascript);
		}
		private static string DecryptStringFromBytes(byte[] CifherText, byte[] key, byte[] iv)
		{
			// Check arguments.
			if (CifherText == null || CifherText.Length <= 0)
			{
				throw new ArgumentNullException(nameof(CifherText));
			}
			if (key == null || key.Length <= 0)
			{
				throw new ArgumentNullException(nameof(key));
			}
			if (iv == null || iv.Length <= 0)
			{
				throw new ArgumentNullException(nameof(key));
			}

			// Declare the string used to hold
			// the decrypted text.
			string plaintext = string.Empty;

			// Create an RijndaelManaged object
			// with the specified key and IV.
			using (RijndaelManaged rijAlg = new())
			{
				//Settings
				rijAlg.Mode = CipherMode.CBC;
				rijAlg.Padding = PaddingMode.PKCS7;
				rijAlg.FeedbackSize = 128;

				rijAlg.Key = key;
				rijAlg.IV = iv;

				// Create a decrytor to perform the stream transform.
				var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
				try
				{
					// Create the streams used for decryption.
					using (var msDecrypt = new MemoryStream(CifherText))
					{
						using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
						{

							using (var srDecrypt = new StreamReader(csDecrypt))
							{
								// Read the decrypted bytes from the decrypting stream
								// and place them in a string.
								plaintext = srDecrypt.ReadToEnd();

							}

						}
					}
				}
				catch
				{
					plaintext = "keyError";
				}
			}

			return plaintext;
		}

	}
}
