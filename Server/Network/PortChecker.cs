using System;
using System.Net;
using System.Net.Sockets;

public class PortChecker
{
    public static bool IsPortListening(int port)
    {
        TcpListener listener = null;
        try
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            return false; // Port is free
        }
        catch (SocketException)
        {
            return true; // Port is in use
        }
        finally
        {
            listener?.Stop(); // Ensure listener is stopped
        }
    }

    public static void Main()
    {
        int portToCheck = 8080; // Replace with the port you want to check
        bool isListening = IsPortListening(portToCheck);
        Console.WriteLine($"Port {portToCheck} is {(isListening ? "in use" : "free")}");
    }
}