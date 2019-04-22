using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Socket clientSocket;
        bool clientMode = true;

        //AsynchronousClient me;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clientMode = true;
            this.Controls.Remove(panel2);
            this.Controls.Add(panel1);
            AsynchronousClient.console = this.clientConsole;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clientMode = false;
            this.Controls.Remove(panel1);
            this.Controls.Add(panel2);
            AsynchronousServer.console = this.serverConsole;
            if (!AsynchronousServer.runningServer)
            {
                this.serverConsole.Text = "";
                AsynchronousServer.StartListening(); 
            }
        }

        private void connect1(object sender, EventArgs e)
        {
            if (AsynchronousClient.connected)
            {
                showAlert("You are already connected");
            }
            else
            {
                String data = this.textBox1.Text;
                String[] split = data.Split(':');
                if(split.Length == 2)
                {
                    this.clientMode = true;
                    try
                    {
                        AsynchronousClient.keepConnection = true;
                        AsynchronousClient.ipAddress = IPAddress.Parse(split[0]);
                        AsynchronousClient.port = Int32.Parse(split[1]);
                        AsynchronousClient.console = this.clientConsole;
                        AsynchronousClient.StartClient();
                    }catch(System.FormatException)
                    {
                        showAlert("Invalid ip/port");
                    }
                }
                else
                {
                    showAlert("Bad adress, only allowed ip:port combination");
                }
            }
        }

        private void connect2(object sender, EventArgs e)
        {
            if (AsynchronousServer.runningServer)
            {
                showAlert("Server already running");
            }
            else
            {
                String data = this.textBox2.Text;
                if (data.Length > 0)
                {  
                    this.clientMode = false;
                    AsynchronousServer.port = Int32.Parse(data);
                    AsynchronousServer.StartListening();
                }
                else
                {
                    showAlert("Bad adress, only allowed ip:port combination");
                }
            }
        }

        private void send1_Click(object sender, EventArgs e)
        {
            if (AsynchronousClient.client == null)
            {
                showAlert("Client is not connected to server");
            }
            else
            {
                String data = this.input1.Text;
                AsynchronousClient.Send(data);
                this.input1.Text = "";
            }
        }

        private void send2_Click(object sender, EventArgs e)
        {
            if (AsynchronousServer.clients.Count == 0)
            {
                showAlert("No client is connected to server");
            }
            else {
                String data = this.input2.Text;
                AsynchronousServer.Send(data, -1);
                this.input2.Text = "";
            }
        }

        private void disconnect1_Click(object sender, EventArgs e)
        {
            if (AsynchronousClient.connected) {
                AsynchronousClient.keepConnection = false;
                AsynchronousClient.close();
            }
            else
            {
                if (AsynchronousClient.keepConnection)
                {
                    AsynchronousClient.keepConnection = false;
                }
                else
                {
                    showAlert("You are not connected to any server");
                }
            }
        }

        private void disconnect2_Click(object sender, EventArgs e)
        {
            if (AsynchronousServer.runningServer)
            {
                AsynchronousServer.close();
            }
            else
            {
                showAlert("You are not connected to any server");
            }
        }

        private void showAlert(String message, String caption = "Alert")
        {
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result = MessageBox.Show(message, caption, buttons);
        }

        private delegate void SetControlPropertyThreadSafeDelegate(
           System.Windows.Forms.Control control,
           string propertyName,
           object propertyValue);

        public static void SetControlPropertyThreadSafe(
            Control control,
            string propertyName,
            object propertyValue)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new SetControlPropertyThreadSafeDelegate
                (SetControlPropertyThreadSafe),
                new object[] { control, propertyName, propertyValue });
            }
            else
            {
                control.GetType().InvokeMember(
                    propertyName,
                    BindingFlags.SetProperty,
                    null,
                    control,
                    new object[] { propertyValue });
            }
        }

    }
}
