
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace eV.ClientUdpExample;

public class Send
{
    private Socket _socket;
    private SocketAsyncEventArgs _sendSocketAsyncEventArgs;
    public Send(Socket socket, EndPoint endPoint)
    {
        _socket = socket;
        SocketAsyncEventArgsCompleted socketAsyncEventArgsCompleted = new();
        socketAsyncEventArgsCompleted.ProcessSendTo += ProcessSendTo;

        _sendSocketAsyncEventArgs = new SocketAsyncEventArgs();
        _sendSocketAsyncEventArgs.Completed += socketAsyncEventArgsCompleted.OnCompleted;
        _sendSocketAsyncEventArgs.RemoteEndPoint = endPoint;
    }

    public void send()
    {
        _sendSocketAsyncEventArgs.SetBuffer(Encoding.Unicode.GetBytes("123"));
        if (!_socket.SendToAsync(_sendSocketAsyncEventArgs))
            ProcessSendTo(_sendSocketAsyncEventArgs);
    }
    private void ProcessSendTo(SocketAsyncEventArgs socketAsyncEventArgs)
    {
        if (socketAsyncEventArgs.SocketError != SocketError.Success)
        {
            Console.WriteLine(socketAsyncEventArgs.SocketError);
        }
    }
}
