using System;
using System.IO;
using System.Security.Cryptography;
using PSO2SERVER.Crypto;

namespace PSO2SERVER.Packets.Handlers
{
    [PacketHandlerAttr(0x11, 0x0B)]
    public class KeyExchange : PacketHandler
    {
        #region implemented abstract members of PacketHandler

        public override void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size)
        {
            if (context.InputArc4 != null)
                return;

            if (size < 0x80)
                return;

            // Extract the first 0x80 bytes into a separate array
            var cryptedBlob = new byte[0x80];
            var rsaBlob = File.ReadAllBytes(ServerApp.ServerPrivateKeyBlob);

            Array.Copy(data, position, cryptedBlob, 0, 0x80);
            Array.Reverse(cryptedBlob);

            // Print the contents of cryptedBlob in hexadecimal format
            //var info = string.Format("[接收] 接收到的数据 (hex): ");
            //Logger.WriteHex(info, cryptedBlob);

            //// Convert cryptedBlob to a hexadecimal string
            //var hexString = BitConverter.ToString(cryptedBlob).Replace("-", "");

            //// Save the hexadecimal string to a text file
            //File.WriteAllText("cryptedBlob.txt", hexString);

            // FIXME
            if (Client.RsaCsp == null)
            {
                Client.RsaCsp = new RSACryptoServiceProvider();
                Client.RsaCsp.ImportCspBlob(rsaBlob);
            }

            var pkcs = new RSAPKCS1KeyExchangeDeformatter(Client.RsaCsp);
            byte[] decryptedBlob;

            try
            {
                decryptedBlob = pkcs.DecryptKeyExchange(cryptedBlob);
            }
            catch (CryptographicException ex)
            {
                Logger.WriteException("解密RSA密钥交换时发生错误", ex);
                context.Socket.Close();
                return;
            }
            
            // Also a failure.
            if (decryptedBlob.Length < 0x20)
                return;

            // Extract the RC4 key
            var arc4Key = new byte[16];
            Array.Copy(decryptedBlob, 0x10, arc4Key, 0, 0x10);

            // Create three RC4 mungers
            var arc4 = new Arc4Managed {Key = arc4Key};
            context.InputArc4 = arc4.CreateDecryptor();

            arc4 = new Arc4Managed {Key = arc4Key};
            context.OutputArc4 = arc4.CreateEncryptor();

            arc4 = new Arc4Managed {Key = arc4Key};
            var tempDecryptor = arc4.CreateDecryptor();

            // Also, grab the init token for the client
            var decryptedToken = new byte[16];
            tempDecryptor.TransformBlock(decryptedBlob, 0, 0x10, decryptedToken, 0);

            context.SendPacket(0x11, 0xC, 0, decryptedToken);
        }

        #endregion
    }
}