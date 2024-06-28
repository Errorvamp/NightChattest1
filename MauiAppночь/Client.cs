using MauiAppночь;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Microsoft.Maui.Controls;

internal class  clien
{
    private static async Task Main(string[] args)
    {
        string host = "127.0.0.1";
        int port = 8888;
        using TcpClient client = new TcpClient();
        Console.Write("Введите свое имя: ");
        string? userName = Console.ReadLine();
        Console.WriteLine($"Добро пожаловать, {userName}");
        StreamReader? Reader = null;
        StreamWriter? Writer = null;

        try
        {
            client.Connect(host, port);
            Reader = new StreamReader(client.GetStream());
            Writer = new StreamWriter(client.GetStream());
            if (Writer is null | Reader is null) return;
            Task.Run(() => ReceiveMessageAsync(Reader));
            await SendMessageAsync(Writer);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        Writer?.Close();
        Reader?.Close();

        async Task SendMessageAsync(StreamWriter writer)
        {
            await writer.WriteLineAsync(userName);
            await writer.FlushAsync();//вызывает запись
            Console.WriteLine("Введите сообщение");

            while (true)
            {
                string? message = Console.ReadLine();
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }

        }

        async Task ReceiveMessageAsync(StreamReader reader)
        {
            while (true)
            {
                try
                {
                    string? message = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(message)) continue;
                    Print(message);
                }
                catch
                {
                    break;
                }
            }
        }

        void Print(string message)
        {
            Console.WriteLine(message);
        }
    }
}