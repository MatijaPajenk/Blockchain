using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Blockchain {
    public partial class NetChainer : Form {
        public NetChainer() {
            InitializeComponent();
            serverSocket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            connectToServerSocket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            blockchain = new Blockchain();
        }

        private static readonly uint STD_DATA_SIZE = 1024 * 1024;
        private Socket serverSocket;
        private Socket connectToServerSocket;
        private readonly List<Socket> connectedClients = [];
        private readonly List<Socket> serversConnectedTo = [];
        private Blockchain blockchain;
        private bool end = false;
        private static uint DIFF = 2;
        private static readonly int DIFF_INTERVAL = 2;
        private static readonly TimeSpan BLOCK_GEN_INTERVAL = TimeSpan.FromSeconds(10);

        private async void Btn_connect_network_Click(object sender, EventArgs e) {
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
            tbx_node_name.ReadOnly = true;
            lbl_status.Text = $"Online: PORT {serverSocket?.LocalEndPoint?.ToString()?.Split(':')[1]}";
            //MessageBox.Show($"local: {hostSocket?.LocalEndPoint}");



            while(!end) {
                try {
                    var client = await serverSocket!.AcceptAsync();

                    connectedClients.Add(client);
                    //MessageBox.Show($"Client connected: {client.LocalEndPoint}");
                    AppendText(rtb_mining, $"\nClient connected: {client.RemoteEndPoint}\n", Color.Green);
                    _ = HandleClients(client);
                } catch(Exception ex) {
                    MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
                    break;
                }
            }
            serverSocket!.Close();
        }

        private async Task HandleClients(Socket client) {
            while(!end) {
                try {
                    var message = Encoding.UTF8.GetString(await Receive(client));
                    var data = message.Split('|');
                    switch(data[0]) {
                        case "#DS":
                            AppendText(rtb_mining, $"\nClient disconnected: {client.RemoteEndPoint}\n", Color.Goldenrod);
                            await Send(client, Encoding.UTF8.GetBytes("OK"));
                            client.Close();
                            connectedClients.Remove(client);
                            //Invoke((MethodInvoker)delegate {
                            //    lbl_client.Text = (Convert.ToInt32(lbl_client.Text) - 1).ToString();
                            //});
                            return;
                        case "#BC":
                            //MessageBox.Show("Starting blockchain transfer...")
                            var blockCount = Convert.ToInt32(data[1]);
                            //if(blockCount < blockchain.Blocks.Count) {
                            //    await Send(client, Encoding.UTF8.GetBytes("SHORTER"));
                            //    return;
                            //}
                            if(blockchain.Blocks.Count > 0 && blockchain.Difficulty > Convert.ToUInt32(data[2])) {
                                await Send(client, Encoding.UTF8.GetBytes("BETTER"));
                                break;
                            }
                            await Send(client, Encoding.UTF8.GetBytes("OK"));

                            await TransferBlocks(client, blockCount);

                            //MessageBox.Show("All blocks received");

                            break;
                        case "#B":
                            await Send(client, Encoding.UTF8.GetBytes("OK"));
                            await TransferBlock(client);
                            break;
                        case "#DIFF":
                            DIFF = Convert.ToUInt32(data[1]);
                            await Send(client, Encoding.UTF8.GetBytes("OK"));
                            break;
                    }


                } catch(Exception ex) {
                    MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
                    break;
                }
            }
        }

        private async Task TransferBlocks(Socket client, int blockCount) {
            blockchain.Blocks.Clear();
            Invoke((MethodInvoker)delegate {
                rtb_ledger.Clear();
            });
            for(int i = 0; i < blockCount; i++) {
                await TransferBlock(client);
            }

        }

        private async Task TransferBlock(Socket client) {
            try {
                var blockData = Encoding.UTF8.GetString(await Receive(client)).Split('|');

                var timestamp = Convert.ToDateTime(blockData[2]);
                if(DateTime.Now.Subtract(timestamp).TotalSeconds > 60) {
                    MessageBox.Show("Block is invalid because it was created more than 1 minute ago");
                    return;
                }

                // TODO FUCK IT IDK, REJECT IF LOW
                if(Convert.ToUInt32(blockData[4]) < DIFF) {
                    return;
                }

                var block = new Block(
                    index: Convert.ToInt32(blockData[0]),
                    data: Encoding.UTF8.GetBytes(blockData[1]),
                    timestamp: Convert.ToDateTime(blockData[2]),
                    previousHash: Utils.GetByteArrayFromHexString(blockData[3]),
                    difficulty: Convert.ToUInt32(blockData[4]),
                    nonce: Convert.ToUInt32(blockData[5]),
                    miner: blockData[6],
                    hash: Utils.GetByteArrayFromHexString(blockData[7])
                );
                //previousHash = Utils.GetByteArrayFromHexString(blockData[3]);
                blockchain.AddBlock(block);
                //blockchain.ValidateChain();
                blockchain.CalculateCumulativeDifficulty();

                Invoke((MethodInvoker)delegate {
                    AppendText(rtb_ledger, $"\n{block}\n", Color.Green);
                });

                //MessageBox.Show(block.ToString());
            } catch(Exception ex) {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
                return;
            }
            await Send(client, Encoding.UTF8.GetBytes("OK"));
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

            end = true;

            foreach(var server in serversConnectedTo) {
                try {
                    await Send(server, Encoding.UTF8.GetBytes("#DS"));
                    var response = Encoding.UTF8.GetString(await Receive(server));

                    if(response != "OK") {
                        MessageBox.Show("Client not disconnected correctly");
                        return;
                    }

                    //if(socket is not null && socket.Connected) {
                    //    try {
                    //        //server.Shutdown(SocketShutdown.Both);
                    //        socket.Close();
                    //    } catch(Exception ex) {
                    //        MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
                    //    }
                    //}
                } catch { continue; }
            }

            //foreach(var client in connectedClients) {
            //    try {
            //        await Send(client, Encoding.UTF8.GetBytes("#DC"));
            //        var response = Encoding.UTF8.GetString(await Receive(client));

            //        if(response != "OK") {
            //            MessageBox.Show("Remote client not disconnected correctly");
            //            return;
            //        }
            //    } catch { continue; }
            //}

            serverSocket.Close();
        }

        private async void Btn_connect_port_Click(object sender, EventArgs e) {
            if(serverSocket is null) {
                MessageBox.Show("Socket isn't connected");
                return;
            }
            if(tbx_port.Text.Length < 2) {
                MessageBox.Show("Invalid port");
                return;
            }

            var ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Convert.ToInt32(tbx_port.Text));

            if(ipEndPoint.Port.ToString() == serverSocket?.LocalEndPoint?.ToString()?.Split(':')[1]) {
                MessageBox.Show("Can't connect to itself");
                return;
            }

            connectToServerSocket = new Socket(new IPEndPoint(IPAddress.Parse("127.0.0.1"), GetAvailablePort()).AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            if(serversConnectedTo.Contains(connectToServerSocket)) {
                MessageBox.Show($"Already connected to 127.0.0.1:{tbx_port.Text}");
                return;
            }
            try {
                await Task.Run(async () => {
                    try {
                        await connectToServerSocket.ConnectAsync(ipEndPoint);
                        serversConnectedTo.Add(connectToServerSocket);

                    } catch {
                        MessageBox.Show("Invalid address");
                        return;
                    }
                    Invoke((MethodInvoker)delegate {
                        //lbl_client.Text = lbl_client.Text == "Not connected" ? "1" : (Convert.ToInt32(lbl_client.Text) + 1).ToString();
                        AppendText(rtb_mining, $"\nConnected to: {ipEndPoint}\n", Color.Green);
                    });

                    if(blockchain.Blocks.Count > 0) {
                        await SendBlockChainSingle(connectToServerSocket);
                    }
                    //_ = ListenForMessages(connectToServerSocket);
                });
            } catch(Exception ex) {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        private static async Task ListenForMessages(Socket socket) {
            try {
                while(socket.Connected) {
                    var response = Encoding.UTF8.GetString(await Receive(socket));
                    if(response != "OK") {
                        MessageBox.Show("Something went wrong...");
                        return;
                    }
                    //MessageBox.Show(Encoding.UTF8.GetString(response));
                }
            } catch(Exception ex) {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        private static async Task<int> Send(Socket socket, byte[] data) {
            int sent = await socket.SendAsync(data, SocketFlags.None);
            return sent;
        }

        private static async Task<byte[]> Receive(Socket socket) {
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

        private async void Btn_mine_Click(object sender, EventArgs e) {
            await Task.Run(() => MineBlock());
        }

        private async void MineBlock() {
            uint nonce = 0;
            uint check = 100_000;
            //uint diff = INIT_DIFF;
            int i = 0;
            var data = "Test dummy data";
            bool skip;
            var previousHash = blockchain.Blocks.Count > 0 ? blockchain.Blocks.Last().Hash : [];
            while(!end) {
                i++;
                nonce++;
                skip = false;
                var timestamp = DateTime.Now;
                var strData = $"{blockchain.Blocks.Count}{timestamp}{data}{previousHash}{DIFF}{nonce}";
                var byteData = Encoding.UTF8.GetBytes(strData);
                var hash = Utils.ComputeHash(byteData);

                for(int k = 0; k < DIFF; k++) {
                    if(hash[k] != 0) {
                        skip = true;
                        break;
                    }
                }

                if(i == check) {
                    i = 0;
                    if(end)
                        return;
                    AppendText(rtb_mining, $"\n{Utils.GetHexString(hash)}\n", Color.Red);
                }

                if(skip)
                    continue;

                if(end)
                    return;

                AppendText(rtb_mining, $"\n{Utils.GetHexString(hash)}\n", Color.Green);
                var block = new Block(
                    index: blockchain.Blocks.Count,
                    timestamp: timestamp,
                    data: Encoding.UTF8.GetBytes(data),
                    hash: hash,
                    previousHash: previousHash,
                    difficulty: DIFF,
                    nonce: nonce,
                    miner: blockchain.Name);

                AdjustDiff();
                //if(elapsedTime.TotalSeconds < 1) {
                //    DIFF++;
                //} else if(elapsedTime.TotalSeconds > 5) {
                //    DIFF--;
                //}
                //MessageBox.Show($"Diff: {diff}");

                // previousHash = hash;
                blockchain.AddBlock(block);
                blockchain.ValidateChain();
                blockchain.CalculateCumulativeDifficulty();

                if(end)
                    return;
                AppendText(rtb_ledger, $"\n{block}\n", Color.Green);

                //await SendBlockChainToAll();
                await SendBlockToAll(block);

                previousHash = hash;
                nonce = 0;
            }
        }

        private async void AdjustDiff() {
            if(blockchain.Blocks.Count < DIFF_INTERVAL)
                return;
            var prevAdjustmentBlock = blockchain.Blocks[^DIFF_INTERVAL];
            var timeExpected = BLOCK_GEN_INTERVAL.Multiply(DIFF_INTERVAL);
            var timeTaken = blockchain.Blocks[^1].Timestamp.Subtract(prevAdjustmentBlock.Timestamp);

            if(timeTaken < (timeExpected.Divide(2))) {
                DIFF = prevAdjustmentBlock.Difficulty + 1;
            } else if(timeTaken > (timeExpected.Multiply(2))) {
                DIFF = prevAdjustmentBlock.Difficulty - 1;
            } else { return; }

            //MessageBox.Show($"Diff: {DIFF}");
            await SendDiffToAll(DIFF);
        }

        private async Task SendDiffToAll(uint diff) {
            foreach(var socket in serversConnectedTo) {
                await Task.Run(async () => {
                    await Send(socket, Encoding.UTF8.GetBytes($"#DIFF|{diff}"));
                    var response = Encoding.UTF8.GetString(await Receive(socket));
                    if(response != "OK") {
                        MessageBox.Show("Error adjusting diff on network");
                        return;
                    }
                });
            }
        }

        private async Task SendBlockToAll(Block block) {
            foreach(var socket in serversConnectedTo) {
                await Task.Run(async () => await SendBlockSingle(socket, block));
            }
        }

        private static async Task SendBlockSingle(Socket socket, Block block) {
            await Send(socket, Encoding.UTF8.GetBytes("#B"));
            var response = Encoding.UTF8.GetString(await Receive(socket));
            if(response != "OK") {
                MessageBox.Show($"Error transfering block {block.Index}");
                return;
            }

            //MessageBox.Show("Attempting block transfer...");
            await Send(socket, block.ToTransferByte());
            response = Encoding.UTF8.GetString(await Receive(socket));
            if(response == "OK")
                return;

            MessageBox.Show($"Error transfering block {block.Index}");
        }

        private async Task SendBlockChainToAll() {
            foreach(var socket in serversConnectedTo) {
                await Task.Run(async () => await SendBlockChainSingle(socket));
            }
        }

        private async Task SendBlockChainSingle(Socket socket) {
            await Send(socket, Encoding.UTF8.GetBytes($"#BC|{blockchain.Blocks.Count}|{blockchain.Difficulty}"));
            var response = Encoding.UTF8.GetString(await Receive(socket));

            switch(response) {
                case "SHORTER":
                    return;
                case "OK":
                    break;
                case "BETTER":
                    return;
                default:
                    MessageBox.Show("Error transfering blockchain");
                    return;
            }

            foreach(var block in blockchain.Blocks) {
                await Send(socket, block.ToTransferByte());
                response = Encoding.UTF8.GetString(await Receive(socket));

                switch(response) {
                    case "OK":
                        break;
                    default:
                        MessageBox.Show($"Error transfering block {block.Index}");
                        return;
                }
            }

            //MessageBox.Show("All blocks transfered");
        }

        private void AppendText(RichTextBox box, string text, Color color) {
            try {
                Invoke((MethodInvoker)delegate {
                    box.SelectionStart = box.TextLength;
                    box.SelectionLength = 0;
                    box.SelectionColor = color;
                    box.AppendText(text);
                    box.SelectionColor = box.ForeColor;
                    box.ScrollToCaret();
                });
            } catch(Exception ex) {
                MessageBox.Show($"{ex.Message}");
            }
        }

    }
}
