using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiAppночь;
using System.Net;
using System.Net.Sockets;


ServerObject server = new ServerObject();
await server.ListenAsync();

class ServerObject
{
    TcpListener tcpListener = new TcpListener(IPAddress.Any, 8888);
    List<ClientObject> clients = new List<ClientObject>();
    protected internal void RemoveConnection(string id)
    {
        ClientObject? client = clients.FirstOrDefault(c => c.Id == id);
        if (client != null) clients.Remove(client);
        client?.Close();
    }

    protected internal async Task ListenAsync()
    {
        try
        {
            tcpListener.Start();
            Console.WriteLine("Сервер запущен,ожидаем пользователей");

            while (true)
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();

                ClientObject clientObject = new ClientObject(tcpClient, this);
                clients.Add(clientObject);
                Task.Run(clientObject.ProcessAsync);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Disconnect();
        }
    }
    protected internal async Task BroadcastMessageAsync(string message, string id)
    {
        foreach (var client in clients)
        {
            if (client.Id != id)
            {
                await client.Writer.WriteLineAsync(message);
                await client.Writer.FlushAsync();
            }
        }
    }

    protected internal void Disconnect()
    {
        foreach (var clients in clients)
        {
            clients.Close();
        }
        tcpListener.Stop();
    }
}
