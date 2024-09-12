using System;
using System.IO;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using System.Security.Cryptography;
using PSO2SERVER;

public static class KeyLoader
{
    public static RSACryptoServiceProvider LoadPrivateKeyFromPem(string filePath)
    {
        using (var reader = new StreamReader(filePath))
        {
            var pemReader = new PemReader(reader);
            var privateKeyParams = (RsaPrivateCrtKeyParameters)pemReader.ReadObject();

            var rsaParams = new RSAParameters
            {
                Modulus = privateKeyParams.Modulus.ToByteArrayUnsigned(),
                Exponent = privateKeyParams.PublicExponent.ToByteArrayUnsigned(),
                P = privateKeyParams.P.ToByteArrayUnsigned(),
                Q = privateKeyParams.Q.ToByteArrayUnsigned(),
                DP = privateKeyParams.DP.ToByteArrayUnsigned(),
                DQ = privateKeyParams.DQ.ToByteArrayUnsigned(),
                InverseQ = privateKeyParams.QInv.ToByteArrayUnsigned(),
                D = privateKeyParams.Exponent.ToByteArrayUnsigned()
            };

            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParams);
            return rsa;
        }
    }

    public static RSACryptoServiceProvider LoadPublicKeyFromPem(string filePath)
    {
        using (var reader = new StreamReader(filePath))
        {
            var pemReader = new PemReader(reader);
            var publicKeyParams = (RsaKeyParameters)pemReader.ReadObject();

            var rsaParams = new RSAParameters
            {
                Modulus = publicKeyParams.Modulus.ToByteArrayUnsigned(),
                Exponent = publicKeyParams.Exponent.ToByteArrayUnsigned()
            };

            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParams);
            return rsa;
        }
    }

    public static void SaveKeyToFile(string filePath, byte[] keyBlob)
    {
        using (FileStream outFile = File.Create(filePath))
        {
            outFile.Write(keyBlob, 0, keyBlob.Length);
        }
    }

    public static void ProcessKeyFiles(string pemFile, string blobFile, bool isPrivateKey, bool isGenerating)
    {
        if (File.Exists(pemFile) && !File.Exists(blobFile) && isGenerating)
        {
            Logger.Write("[KEY] 发现{0}文件, 正在生成新的{1}密钥 {2}...", pemFile, isPrivateKey ? "私有" : "公共", blobFile);
            RSACryptoServiceProvider rsa = isPrivateKey ? LoadPrivateKeyFromPem(pemFile) : LoadPublicKeyFromPem(pemFile);
            byte[] cspBlob = rsa.ExportCspBlob(isPrivateKey);
            SaveKeyToFile(blobFile, cspBlob);
        }
    }

    public static void GenerateAndSaveKeyIfNotExists(RSACryptoServiceProvider rsa, string keyBlobFile, bool isPrivateKey)
    {
        if (!File.Exists(keyBlobFile))
        {
            Logger.WriteWarning("[KEY] 未找到 {0} 文件, 正在生成新的{1}密钥...", keyBlobFile, isPrivateKey ? "私有" : "公共");
            byte[] cspBlob = rsa.ExportCspBlob(isPrivateKey);
            SaveKeyToFile(keyBlobFile, cspBlob);
        }
    }
}
