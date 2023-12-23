namespace Blockchain {
    partial class NetChainer {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && ( components != null )) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            label1 = new Label();
            label2 = new Label();
            tbx_node_name = new TextBox();
            lbl_status = new Label();
            btn_connect_network = new Button();
            btn_mine = new Button();
            tbx_port = new TextBox();
            btn_connect_port = new Button();
            rtb_ledger = new RichTextBox();
            rtb_mining = new RichTextBox();
            lbl_client = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 27);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(118, 28);
            label1.TabIndex = 0;
            label1.Text = "Node name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(53, 60);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(69, 28);
            label2.TabIndex = 1;
            label2.Text = "Status:";
            // 
            // tbx_node_name
            // 
            tbx_node_name.Location = new Point(114, 22);
            tbx_node_name.Margin = new Padding(4);
            tbx_node_name.Name = "tbx_node_name";
            tbx_node_name.Size = new Size(127, 34);
            tbx_node_name.TabIndex = 2;
            // 
            // lbl_status
            // 
            lbl_status.AutoSize = true;
            lbl_status.Location = new Point(114, 60);
            lbl_status.Margin = new Padding(4, 0, 4, 0);
            lbl_status.Name = "lbl_status";
            lbl_status.Size = new Size(115, 28);
            lbl_status.TabIndex = 3;
            lbl_status.Text = "placeholder";
            // 
            // btn_connect_network
            // 
            btn_connect_network.BackColor = SystemColors.ButtonHighlight;
            btn_connect_network.Location = new Point(280, 21);
            btn_connect_network.Margin = new Padding(4);
            btn_connect_network.Name = "btn_connect_network";
            btn_connect_network.Size = new Size(96, 32);
            btn_connect_network.TabIndex = 4;
            btn_connect_network.Text = "Connect";
            btn_connect_network.UseVisualStyleBackColor = false;
            btn_connect_network.Click += btn_connect_network_Click;
            // 
            // btn_mine
            // 
            btn_mine.Location = new Point(394, 21);
            btn_mine.Margin = new Padding(4);
            btn_mine.Name = "btn_mine";
            btn_mine.Size = new Size(96, 32);
            btn_mine.TabIndex = 5;
            btn_mine.Text = "Mine";
            btn_mine.UseVisualStyleBackColor = true;
            btn_mine.Click += btn_mine_Click;
            // 
            // tbx_port
            // 
            tbx_port.Location = new Point(598, 21);
            tbx_port.Margin = new Padding(4);
            tbx_port.Name = "tbx_port";
            tbx_port.Size = new Size(127, 34);
            tbx_port.TabIndex = 6;
            // 
            // btn_connect_port
            // 
            btn_connect_port.Location = new Point(733, 19);
            btn_connect_port.Margin = new Padding(4);
            btn_connect_port.Name = "btn_connect_port";
            btn_connect_port.Size = new Size(122, 32);
            btn_connect_port.TabIndex = 7;
            btn_connect_port.Text = "Connect port";
            btn_connect_port.UseVisualStyleBackColor = true;
            btn_connect_port.Click += btn_connect_port_Click;
            // 
            // rtb_ledger
            // 
            rtb_ledger.Location = new Point(27, 103);
            rtb_ledger.Name = "rtb_ledger";
            rtb_ledger.Size = new Size(410, 441);
            rtb_ledger.TabIndex = 8;
            rtb_ledger.Text = "Blockchain ledger...";
            // 
            // rtb_mining
            // 
            rtb_mining.Location = new Point(462, 103);
            rtb_mining.Name = "rtb_mining";
            rtb_mining.Size = new Size(393, 441);
            rtb_mining.TabIndex = 9;
            rtb_mining.Text = "Mining (every 100 000th hash)...";
            // 
            // lbl_client
            // 
            lbl_client.AutoSize = true;
            lbl_client.Location = new Point(600, 65);
            lbl_client.Name = "lbl_client";
            lbl_client.Size = new Size(142, 28);
            lbl_client.TabIndex = 10;
            lbl_client.Text = "Not connected";
            // 
            // NetChainer
            // 
            AutoScaleDimensions = new SizeF(11F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(885, 566);
            Controls.Add(lbl_client);
            Controls.Add(rtb_mining);
            Controls.Add(rtb_ledger);
            Controls.Add(btn_connect_port);
            Controls.Add(tbx_port);
            Controls.Add(btn_mine);
            Controls.Add(btn_connect_network);
            Controls.Add(lbl_status);
            Controls.Add(tbx_node_name);
            Controls.Add(label2);
            Controls.Add(label1);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            Name = "NetChainer";
            Text = "NetChainer";
            FormClosing += Form1_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox tbx_node_name;
        private Label lbl_status;
        private Button btn_connect_network;
        private Button btn_mine;
        private TextBox tbx_port;
        private Button btn_connect_port;
        private RichTextBox rtb_ledger;
        private RichTextBox rtb_mining;
        private Label lbl_client;
    }
}
