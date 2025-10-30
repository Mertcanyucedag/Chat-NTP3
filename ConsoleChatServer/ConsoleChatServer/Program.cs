// ConsoleChatServer -> Program.cs
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Server
{
    private static List<TcpClient> clients = new List<TcpClient>();

    static async Task Main(string[] args)
    {
        TcpListener server = new TcpListener(IPAddress.Any, 8888);
        try
        {
            server.Start();
            Console.WriteLine("Sunucu başlatıldı. Port 8888 dinleniyor...");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                clients.Add(client);
                Console.WriteLine("Yeni bir istemci bağlandı!");
                Task.Run(() => HandleClient(client));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
        }
        finally
        {
            server.Stop();
        }
    }

    private static async void HandleClient(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Alınan: {message}");
                await BroadcastMessage(message, client);
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Bir istemcinin bağlantısı koptu!");
            clients.Remove(client);
            client.Close();
        }
    }

    private static async Task BroadcastMessage(string message, TcpClient sender)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        foreach (var client in clients)
        {
            if (client != sender)
            {
                await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }
}