// ConsoleChatClient -> Program.cs
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Client
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Sunucu IP adresini girin (örn: 10.52.176.122):");
        string ipAddress = Console.ReadLine();

        TcpClient client = new TcpClient();
        try
        {
            await client.ConnectAsync(ipAddress, 8888);
            Console.WriteLine("Sunucuya bağlandı!");
            NetworkStream stream = client.GetStream();

            Console.Write("Lütfen adınızı girin: ");
            string name = Console.ReadLine();
            Console.WriteLine("Sohbete hoş geldin! (Çıkmak için 'exit' yaz)");

            Task receiveTask = Task.Run(async () =>
            {
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;
                    Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                }
            });

            string messageToSend;
            while ((messageToSend = Console.ReadLine()) != "exit")
            {
                string formattedMessage = $"{name}: {messageToSend}";
                byte[] buffer = Encoding.UTF8.GetBytes(formattedMessage);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }
}