using System;
using System.IO;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using System.Security.Cryptography;

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
}
