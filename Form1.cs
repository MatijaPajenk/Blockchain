using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Blockchain {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private static readonly uint STD_DATA_SIZE = 1024 * 1024;
        private Socket serverSocket;
        private Socket connectToServerSocket;
        private List<Socket> connectedClients = new List<Socket>();
        private List<Socket> serversConnectedTo = new List<Socket>();

        private async void btn_connect_network_Click(object sender, EventArgs e) {
            var ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), GetAvailablePort());
            serverSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(10);

            lbl_status.Text = $"Online: PORT {serverSocket?.LocalEndPoint?.ToString()?.Split(':')[1]}";
            //MessageBox.Show($"local: {hostSocket?.LocalEndPoint}");

            while(true) {
                try {
                    var client = await serverSocket.AcceptAsync();
                    connectedClients.Add(client);
                    //MessageBox.Show($"Client connected: {client.LocalEndPoint
                    AppendText(rtb_mining, $"\nClient connected: {client.RemoteEndPoint}\n", Color.Green);
                    _ = HandleClients(client);
                } catch(Exception ex) {
                    MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
                    break;
                }
            }
            serverSocket.Close();
        }


        private static void AppendText(RichTextBox box, string text, Color color) {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        private async Task HandleClients(Socket client) {
            while(true) {
                try {
                    //TODO implement logic
                    var message = Encoding.UTF8.GetString(await Receive(client));
                    var data = message.Split('|');
                    switch(data[0]) {
                        case "#D":
                            await Send(client, Encoding.UTF8.GetBytes("OK"));
                            AppendText(rtb_mining, $"\nClient disconnected: {client.RemoteEndPoint}\n", Color.Goldenrod);
                            client.Close();
                            connectedClients.Remove(client);
                            return;
                    }


                } catch(Exception ex) {
                    MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
                    break;
                }
            }

            //try {
            //    client.Shutdown(SocketShutdown.Both);
            //    client.Close();
            //} catch(Exception ex) {
            //    MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            //}
        }



        private static int GetAvailablePort() {
            var temp = new TcpListener(IPAddress.Loopback, 0);
            temp.Start();
            int port = ((IPEndPoint)temp.LocalEndpoint).Port;
            temp.Stop();
            return port;
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if(serverSocket is null)
                return;

            foreach(var server in serversConnectedTo) {
                try {
                    await Send(server, Encoding.UTF8.GetBytes("#D"));
                    var response = Encoding.UTF8.GetString(await Receive(server));

                    if(response != "OK") {
                        MessageBox.Show("Client not disconnected correctly");
                        return;
                    }

                    if(server is not null && server.Connected) {
                        try {
                            server.Shutdown(SocketShutdown.Both);
                            server.Close();
                        } catch(Exception ex) {
                            MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
                        }
                    }
                } catch { continue; }
            }

            serverSocket.Close();
        }

        private async void btn_connect_port_Click(object sender, EventArgs e) {
            if(serverSocket is null) {
                MessageBox.Show("Socket isn't connected");
                return;
            }
            if(tbx_port.Text.Length < 2) {
                MessageBox.Show("Invalid port");
                return;
            }

            var ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Convert.ToInt32(tbx_port.Text));
            connectToServerSocket = new Socket(new IPEndPoint(IPAddress.Parse("127.0.0.1"), GetAvailablePort()).AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serversConnectedTo.Add(connectToServerSocket);
            try {
                await Task.Run(async () => {
                    await connectToServerSocket.ConnectAsync(ipEndPoint);

                    _ = ListenForMessages(connectToServerSocket);
                });
            } catch(Exception ex) {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        private async Task ListenForMessages(Socket socket) {
            try {
                while(socket.Connected) {
                    //TODO implemnt logic
                    break;
                }
            } catch(Exception ex) {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            }


        }

        private async Task<int> Send(Socket socket, byte[] data) {
            int sent = await socket.SendAsync(data, SocketFlags.None);
            return sent;
        }

        private async Task<byte[]> Receive(Socket socket) {
            var buffer = new byte[STD_DATA_SIZE];
            try {
                var received = await socket.ReceiveAsync(buffer, SocketFlags.None);
                var newBuffer = new byte[received];
                Array.Copy(buffer, newBuffer, received);
                return newBuffer;
            } catch(Exception ex) {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
                return Array.Empty<byte>();
            }
        }
    }
}
