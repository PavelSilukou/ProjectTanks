using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;

public class NetworkManager {

    TcpClient client = new TcpClient();

    NetworkStream serverStream;

    ConcurrentQueue<string> recvQueue = new ConcurrentQueue<string>();
    ConcurrentQueue<string> sendQueue = new ConcurrentQueue<string>();

    Thread clientSendThr;
    Thread clientRecvThr;
    //Thread clientProcessThr;

    private string ip;
    private int port;

    private volatile bool stopFlag = false;

    public NetworkManager()
    {

    }

    public NetworkManager(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
        //this.gameManager = gameManager;
    }

    public string GetMessage()
    {
        if (recvQueue.Count > 0)
        {
            string recvMessage = recvQueue.Dequeue();
            return recvMessage;
        }
        else
        {
            return "none";
        }
    }

    public void PutMessage(string sendMessage)
    {
        sendQueue.Enqueue(sendMessage);
    }

    private void ClientSend()
    {
        while (!stopFlag)
        {
            if (sendQueue.Count > 0)
            {
                byte[] message = new byte[128];
                string data = sendQueue.Dequeue();

                message = Encoding.ASCII.GetBytes(data);
                serverStream.Write(message, 0, message.Length);
                serverStream.Flush();
            }
        }
    }

    private void ClientReceive()
    {
        while (!stopFlag)
        {
            byte[] message = new byte[128];
            int bytes;

            while(serverStream.DataAvailable)
            {
                bytes = serverStream.Read(message, 0, message.Length);
                string recvMessage = System.Text.Encoding.ASCII.GetString(message, 0, bytes);
                recvQueue.Enqueue(recvMessage);
            }
        }
    }

    /*private void ClientProcess()
    {
        while (!stopFlag)
        {
            if (recvQueue.Count > 0)
            {
                string message = recvQueue.Dequeue();
                gameManager.ProcessMessage(message);
            }
        }
    }*/

    public void SetConnection()
    {
        client.Connect(this.ip, this.port);
        serverStream = client.GetStream();

        clientSendThr = new Thread(new ThreadStart(ClientSend));
        clientRecvThr = new Thread(new ThreadStart(ClientReceive));
        //clientProcessThr = new Thread(new ThreadStart(ClientProcess));
        clientSendThr.Start();
        clientRecvThr.Start();
        //clientProcessThr.Start();
    }

    public void CloseConnection()
    {
        stopFlag = true;
        client.Close();
    }
}

public class ConcurrentQueue<T>{

    private readonly object syncLock = new object();
    private Queue<T> queue;

    public ConcurrentQueue()
    {
        this.queue = new Queue<T>();
    }

    public int Count
    {
        get 
        {
            lock (syncLock)
            {
                return this.queue.Count;
            }
        }
    }

    public void Enqueue(T obj)
    {
        lock (syncLock)
        {
            queue.Enqueue(obj);
        }
    }

    public T Dequeue()
    {
        lock (syncLock)
        {
            return queue.Dequeue();
        }
    }

}
