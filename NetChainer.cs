using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Blockchain {
    public partial class NetChainer : Form {
        public NetChainer() {
            InitializeComponent();
        }

        private static readonly uint STD_DATA_SIZE = 1024 * 1024;
        private Socket serverSocket;
        private Socket connectToServerSocket;
        private List<Socket> connectedClients = new List<Socket>();
        private List<Socket> serversConnectedTo = new List<Socket>();
        private Blockchain blockchain;

        private async void btn_connect_network_Click(object sender, EventArgs e) {
            if(tbx_node_name.Text.Length == 0) {
                MessageBox.Show("Must enter node name!");
                return;
            }

            blockchain = new Blockchain(tbx_node_name.Text);

            try {
                var ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), GetAvailablePort());
                serverSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(ipEndPoint);
                serverSocket.Listen(10);
            } catch(Exception ex) {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
                return;
            }

            btn_connect_network.Enabled = false;
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

            if(serversConnectedTo.Contains(connectToServerSocket)) {
                MessageBox.Show($"Already connected to 127.0.0.1:{tbx_port.Text}");
                return;
            }
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

        private async void btn_mine_Click(object sender, EventArgs e) {
            await Task.Run(() => MineBlock());
        }

        private void MineBlock() {
            uint nonce = 0;
            uint check = 100_000;
            uint diff = 2;
            int i = 0;
            var data = "Test dummy data";
            bool skip;
            var startTime = DateTime.Now;
            while(true) {
                i++;
                nonce++;
                skip = false;
                var previousHash = blockchain.Blocks.Count == 0 ?
                    Array.Empty<byte>() : blockchain.Blocks[blockchain.Blocks.Count - 1].Hash;
                var timestamp = DateTime.Now;
                var strData = $"{blockchain.Blocks.Count}{timestamp}{data}{previousHash}{diff}{nonce}";
                var byteData = Encoding.UTF8.GetBytes(strData);
                var hash = Utils.ComputeHash(byteData);

                for(int k = 0; k < diff; k++) {
                    if(hash[k] != 0) {
                        skip = true;
                        break;
                    }
                }

                if(i == check) {
                    i = 0;
                    AppendText(rtb_mining, $"\n{Utils.GetHexString(hash)}\n", Color.Red);
                }

                if(skip)
                    continue;

                var elapsedTime = DateTime.Now - startTime;

                AppendText(rtb_mining, $"\n{Utils.GetHexString(hash)}\n", Color.Green);
                var block = new Block(
                    index: blockchain.Blocks.Count,
                    timestamp: timestamp,
                    data: Encoding.UTF8.GetBytes(data),
                    hash: hash,
                    previousHash: previousHash,
                    difficulty: diff,
                    nonce: nonce,
                    miner: blockchain.Name);

                if(elapsedTime.TotalSeconds < 1) {
                    diff++;
                } else if(elapsedTime.TotalSeconds > 5) {
                    diff--;
                }
                MessageBox.Show($"Diff: {diff}");

                blockchain.Blocks.Add(block);
                AppendText(rtb_ledger, $"\n{block}\n", Color.Green);

                if(blockchain.Blocks.Count == 10)
                    break;

                nonce = 0;
                startTime = DateTime.Now;
            }
        }

        public void AppendText(RichTextBox box, string text, Color color) {
            this.Invoke((MethodInvoker)delegate {
                box.SelectionStart = box.TextLength;
                box.SelectionLength = 0;
                box.SelectionColor = color;
                box.AppendText(text);
                box.SelectionColor = box.ForeColor;
                box.ScrollToCaret();
            });
        }

    }
}
