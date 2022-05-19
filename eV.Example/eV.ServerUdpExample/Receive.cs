using System.Net;
using System.Net.Sockets;
using System.Text;
namespace eV.ServerUdpExample;

public class Receive
{
    private Socket _socket;
    private SocketAsyncEventArgs _receiveSocketAsyncEventArgs;
    public Receive(Socket socket)
    {
        _socket = socket;
        SocketAsyncEventArgsCompleted socketAsyncEventArgsCompleted = new();
        socketAsyncEventArgsCompleted.ProcessReceiveFrom += ProcessReceiveFrom;
        const int receiveBufferSize = 1024;
        byte[] receiveBuffer = new byte[receiveBufferSize];
        _receiveSocketAsyncEventArgs = new SocketAsyncEventArgs();
        _receiveSocketAsyncEventArgs.Completed += socketAsyncEventArgsCompleted.OnCompleted;
        _receiveSocketAsyncEventArgs.SetBuffer(receiveBuffer, 0, receiveBufferSize);
        _receiveSocketAsyncEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        StartReceiveFrom();
    }

    private void StartReceiveFrom()
    {
        if (!_socket.ReceiveFromAsync(_receiveSocketAsyncEventArgs))
            ProcessReceiveFrom(_receiveSocketAsyncEventArgs);
    }

    private void ProcessReceiveFrom(SocketAsyncEventArgs socketAsyncEventArgs)
    {
        if (socketAsyncEventArgs.SocketError != SocketError.Success)
        {
            Console.WriteLine(socketAsyncEventArgs.SocketError);
            return;
        }
        if (socketAsyncEventArgs.BytesTransferred == 0)
        {
            Console.WriteLine("socketAsyncEventArgs.BytesTransferred");
            return;
        }
        Console.WriteLine(socketAsyncEventArgs.RemoteEndPoint);
        Console.WriteLine(Encoding.Unicode.GetString(socketAsyncEventArgs.Buffer?.Skip(socketAsyncEventArgs.Offset).Take(socketAsyncEventArgs.BytesTransferred).ToArray()));
        StartReceiveFrom();
    }
}
