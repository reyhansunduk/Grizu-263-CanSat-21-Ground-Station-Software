using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System.Threading;
using System.IO.Ports;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using System.Runtime.InteropServices;


namespace Grizu_263_GCS
{
    public partial class Form1 : Form
    {
        #region global_variables
        string[] ports = SerialPort.GetPortNames();
        string containerr = @"Flight_3205_C.csv";
        string spayload1 = @"Flight_3205_SP1.csv";
        string spayload2 = @"Flight_3205_SP2.csv";
        string global;
        string gc, gsp1, gsp2;
        string[] böl;
        int mqttaktif = 0; 
        string  mqtt_sendSP1, mqtt_sendSP2;
        int counter = 0;
        int simp_aktif = 0;
        MqttClient client;


        string username = "3205";
        string password = "Juehhika290@";
        string topic = "teams/3205";


        bool control = true;
        #endregion

     


        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();

            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
            }
        }
        
        private void button9_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("CMD,3210,SP1,ON,");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("CMD,3210,SP1,OFF,"); 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
                comboBox1.SelectedIndex = 0; 
            }
            comboBox2.Items.Add("4800");
            comboBox2.Items.Add("9600");
            comboBox2.Items.Add("19200");
            
            comboBox2.SelectedIndex = 2;

            try
            {
                client = new MqttClient("cansat.info");
            }
            catch (System.Net.Sockets.SocketException)
            {
                MessageBox.Show("Canım Bu İnternetle MQTT ye Bağlanamazsın", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
                gMapControl1.Position = new GMap.NET.PointLatLng(41.4517, 31.7913);

                gMapControl1.Zoom = 5;
                gMapControl1.MaxZoom = 100;
                gMapControl1.Zoom = 10;
            label3.Parent = pictureBox3;
            label3.BackColor = Color.Transparent;

            label4.Parent = pictureBox3;
            label4.BackColor = Color.Transparent;

            label9.Parent = pictureBox3;
            label9.BackColor = Color.Transparent;

            label10.Parent = pictureBox3;
            label10.BackColor = Color.Transparent;

            label11.Parent = pictureBox3;
            label11.BackColor = Color.Transparent;

            groupBox1.Parent = pictureBox3;
            groupBox1.BackColor = Color.Transparent;

            groupBox2.Parent = pictureBox3;
            groupBox2.BackColor = Color.Transparent;

            groupBox3.Parent = pictureBox3;
            groupBox3.BackColor = Color.Transparent;

            groupBox4.Parent = pictureBox3;
            groupBox4.BackColor = Color.Transparent;

            groupBox5.Parent = pictureBox3;
            groupBox5.BackColor = Color.Transparent;

            pictureBox1.Parent = pictureBox3;
            pictureBox1.BackColor = Color.Transparent;

            groupBox6.Parent = pictureBox3;
            groupBox6.BackColor = Color.Transparent;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            


            timer1.Start();

            if (serialPort1.IsOpen == false)
            {
                if (comboBox1.Text == "")
                    return;
                serialPort1.PortName = comboBox1.Text;
               serialPort1.BaudRate = Convert.ToInt16(comboBox2.Text);
                try
                {
                    serialPort1.Open();
                }
                catch (Exception hata)
                {
                    MessageBox.Show("Hata:" + hata.Message);
                }
                buton_aktif();
            }           
        }

        private void buton_aktif()
        {
            button2.Enabled = true; button3.Enabled = true; button4.Enabled = true; button5.Enabled = true; button6.Enabled = true; button7.Enabled = true; button8.Enabled = true; button9.Enabled = true; button10.Enabled = true;
            button11.Enabled = true; button12.Enabled = true; button13.Enabled = true; button14.Enabled = true; button15.Enabled = true; button16.Enabled = true; button17.Enabled = true; button18.Enabled = true; button19.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {           
            if (simp_aktif == 1)
            {
                StreamReader yaz = new StreamReader(openFileDialog1.FileName);

                string satir = yaz.ReadLine();
                                
                if (counter > 0)
                {
                    for (int i = 0; i < counter; i++)
                        satir = yaz.ReadLine();
                }

                if (satir != null)
                {
                    serialPort1.Write(satir + ",");
                    counter++;
                }
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {            
            openFileDialog1.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mqttaktif = 1;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            
            mqttaktif = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("CMD,3205,SIM,ENABLE,");
        }

        private void button18_Click(object sender, EventArgs e)
        {
            serialPort1.Write("2");
        }

        private void button19_Click(object sender, EventArgs e)
        {
             serialPort1.Write("3");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("CMD,3205,CBN,");
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            global = serialPort1.ReadLine();
            böl = global.Split(',');
            //int i = 0,j=0;
            switch (böl[3])
            {
                case "C":
                    gc = global;
                    Ccontainer();
                    break;
                case "SP1":
                    gsp1 = global;
                    SP1();

                    break;
                case "SP2":
                    gsp2 = global;
                    SP2();
                    break;
                default:
                    break;    
                  
            }
        }

        private void Mqtt_SP1()
        {
            try
            {
                mqtt_sendSP1 = "";
                mqtt_sendSP1 += Cböl[0] + ",";
                mqtt_sendSP1 += Cböl[1] + ",";
                mqtt_sendSP1 += Cböl[2] + ",";
                int j = 3;
                while (j < 8)
                {
                    mqtt_sendSP1 += SP1_böl[j] + ",";
                    j++;
                }

            }
            catch (NullReferenceException)
            {
            } 
        }
        private void Mqtt_SP2()
        {
            mqtt_sendSP2 = "";
            mqtt_sendSP2 += Cböl[0] + ",";
            mqtt_sendSP2 += Cböl[1] + ",";
            mqtt_sendSP2 += Cböl[2] + ",";
            int f = 3;
            while (f < 8)
            {
                mqtt_sendSP2 += SP2_böl[f] + ",";
                f++;
            }
        }

        string[] Cböl;

        private void Ccontainer()
        {
            try
            { 
                Cböl = gc.Split(',');

                int a = 0;
                label4.Text = Cböl[1];
                switch (Cböl[15])
                {
                    case "0":
                        listView6.Items[0].ForeColor = Color.Green;
                        listView6.Items[1].ForeColor = Color.White; listView6.Items[2].ForeColor = Color.White; listView6.Items[3].ForeColor = Color.White; listView6.Items[4].ForeColor = Color.White; listView6.Items[5].ForeColor = Color.White;
                        break;
                    case "1":
                        listView6.Items[0].ForeColor = Color.White; listView6.Items[2].ForeColor = Color.White; listView6.Items[3].ForeColor = Color.White; listView6.Items[4].ForeColor = Color.White; listView6.Items[5].ForeColor = Color.White;
                        listView6.Items[1].ForeColor = Color.Green;
                        break;
                    case "2":
                        listView6.Items[1].ForeColor = Color.White; listView6.Items[0].ForeColor = Color.White; listView6.Items[3].ForeColor = Color.White; listView6.Items[4].ForeColor = Color.White; listView6.Items[5].ForeColor = Color.White;
                        listView6.Items[2].ForeColor = Color.Green;
                        break;
                    case "3":
                        listView6.Items[1].ForeColor = Color.White; listView6.Items[2].ForeColor = Color.White; listView6.Items[0].ForeColor = Color.White; listView6.Items[4].ForeColor = Color.White; listView6.Items[5].ForeColor = Color.White;
                        listView6.Items[3].ForeColor = Color.Green;
                        break;
                    case "4":
                        listView6.Items[1].ForeColor = Color.White; listView6.Items[2].ForeColor = Color.White; listView6.Items[3].ForeColor = Color.White; listView6.Items[0].ForeColor = Color.White; listView6.Items[5].ForeColor = Color.White;
                        listView6.Items[4].ForeColor = Color.Green;
                        break;
                    case "5":
                        listView6.Items[1].ForeColor = Color.White; listView6.Items[2].ForeColor = Color.White; listView6.Items[3].ForeColor = Color.White; listView6.Items[4].ForeColor = Color.White; listView6.Items[0].ForeColor = Color.White;
                        listView6.Items[5].ForeColor = Color.Green;
                        break;
                    default:
                        break;
                }

                switch (Cböl[15])
                {
                    case "0":
                        Cböl[15] = listView6.Items[0].Text; break;
                    case "1":
                        Cböl[15] = listView6.Items[1].Text; break;
                    case "2":
                        Cböl[15] = listView6.Items[2].Text; break;
                    case "3":
                        Cböl[15] = listView6.Items[3].Text; break;
                    case "4":
                        Cböl[15] = listView6.Items[4].Text; break;
                    case "5":
                        Cböl[15] = listView6.Items[5].Text; break;
                    default:
                        break;
                }
                gc = "";
                
                gc += Cböl[0];
                gc += ",";
                gc += Cböl[1];
                gc += ",";
                gc += Cböl[2];
                gc += ",";
                gc += Cböl[3];
                gc += ",";
                gc += Cböl[4];
                gc += ",";
                gc += Cböl[5];
                gc += ",";
                gc += Cböl[6];
                gc += ",";
                gc += Cböl[7];
                gc += ",";
                gc += Cböl[8];
                gc += ",";
                gc += Cböl[9];
                gc += ",";
                gc += Cböl[10];
                gc += ",";
                gc += Cböl[11];
                gc += ",";
                gc += Cböl[12];
                gc += ",";
                gc += Cböl[13];
                gc += ",";
                gc += Cböl[14];
                gc += ",";
                gc += Cböl[15];
                gc += ",";
                gc += Cböl[16];
                gc += ",";
                gc += Cböl[17];
                gc += ",";
                gc += Cböl[18];
                gc += ",";
                gc += "\r";
                                
                File.AppendAllText(containerr, gc);

                listView7.View = View.Details;
                ListViewItem item8 = new ListViewItem(Cböl[0]);
                item8.SubItems.Add(Cböl[1]);
                item8.SubItems.Add(Cböl[3]);
                item8.SubItems.Add(Cböl[2]);
                item8.SubItems.Add(Cböl[4]);
                item8.SubItems.Add(Cböl[5]);
                item8.SubItems.Add(Cböl[6]);
                item8.SubItems.Add(Cböl[7]);
                item8.SubItems.Add(Cböl[8]);
                item8.SubItems.Add(Cböl[9]);
                item8.SubItems.Add(Cböl[10]);
                item8.SubItems.Add(Cböl[11]);
                item8.SubItems.Add(Cböl[12]);
                item8.SubItems.Add(Cböl[13]);
                item8.SubItems.Add(Cböl[14]);
                item8.SubItems.Add(Cböl[15]);
                item8.SubItems.Add(Cböl[16]);
                item8.SubItems.Add(Cböl[17]);
                item8.SubItems.Add(Cböl[18]);
                listView7.Items.Add(item8);

                if (listView7.Items.Count == 8)
                    listView7.Items[0].Remove();

                this.listView7.Items[this.listView7.Items.Count - 1].EnsureVisible();

                listView3.View = View.Details;                     ///////SON TELEMETRİ CONTAINER
                ListViewItem item1 = new ListViewItem(Cböl[0]);
                item1.SubItems.Add(Cböl[1]);
                item1.SubItems.Add(Cböl[2]);
                item1.SubItems.Add(Cböl[4]);
                item1.SubItems.Add(Cböl[7]);
                item1.SubItems.Add(Cböl[8]);
                item1.SubItems.Add(Cböl[9]);
                item1.SubItems.Add(Cböl[15]);
                listView3.Items.Add(item1);

                if (listView3.Items.Count == 2)
                    listView3.Items[0].Remove();

                

                this.listView3.Items[this.listView3.Items.Count - 1].EnsureVisible();

                chart6.Series["ALTITUDE"].Points.AddXY(Cböl[1], Cböl[7]);
                chart6.ChartAreas[0].AxisX.ScaleView.Scroll(System.Windows.Forms.DataVisualization.Charting.ScrollType.Last);
                this.chart5.Series["TEMPERATURE"].Points.AddXY(Cböl[1], Cböl[8]);
                chart5.ChartAreas[0].AxisX.ScaleView.Scroll(System.Windows.Forms.DataVisualization.Charting.ScrollType.Last);
                this.chart7.Series["VOLTAGE"].Points.AddXY(Cböl[1], Cböl[9]);
                chart7.ChartAreas[0].AxisX.ScaleView.Scroll(System.Windows.Forms.DataVisualization.Charting.ScrollType.Last);



                if (mqttaktif == 1)
                {
                    if(control == true)
                    {
                        client.Connect(Guid.NewGuid().ToString(), username, password); 
                        control = false;
                    }
                    
                    MqttClient.MqttMsgPublishEventHandler client_MqttMsgPublishReceived = null;
                    client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

                    if (!String.IsNullOrWhiteSpace(gc))
                    {
                        client.Publish(topic, Encoding.UTF8.GetBytes(gc), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
                       
                    }
                }
            }
            catch (IndexOutOfRangeException )
            {
            }

        }
        string[] SP2_böl;
        private void SP2()
        {
            try
            { 
                SP2_böl = gsp2.Split(',');
                

                File.AppendAllText(spayload2, gsp2);

                listView1.View = View.Details;
                ListViewItem item3 = new ListViewItem(SP2_böl[0]);                            /*    ListView        SP2    */
                item3.SubItems.Add(SP2_böl[1]);
                item3.SubItems.Add(SP2_böl[3]);
                item3.SubItems.Add(SP2_böl[2]);
                item3.SubItems.Add(SP2_böl[4]);
                item3.SubItems.Add(SP2_böl[5]);
                item3.SubItems.Add(SP2_böl[6]);
                item3.SubItems.Add(SP2_böl[7]);
                item3.SubItems.Add(SP2_böl[8]);
                listView1.Items.Add(item3);

                if (listView1.Items.Count == 8)
                    listView1.Items[0].Remove();

                this.listView1.Items[this.listView1.Items.Count - 1].EnsureVisible();

                listView5.View = View.Details;                     ///////SON TELEMETRİ SP2
                ListViewItem item2 = new ListViewItem(böl[2]);
                item2.SubItems.Add(SP2_böl[4]);
                item2.SubItems.Add(SP2_böl[5]);
                item2.SubItems.Add(SP2_böl[6]);
                item2.SubItems.Add(SP2_böl[7]);
                //item2.SubItems.Add(SP2_böl[7]);
                listView5.Items.Add(item2);

                if (listView5.Items.Count == 2)
                    listView5.Items[0].Remove();

                this.listView5.Items[this.listView5.Items.Count - 1].EnsureVisible();

                this.chart4.Series["SP2 ALTITUDE"].Points.AddXY(SP2_böl[1], SP2_böl[4]);
                chart4.ChartAreas[0].AxisX.ScaleView.Scroll(System.Windows.Forms.DataVisualization.Charting.ScrollType.Last);/*    CHART            SP2     */
                this.chart9.Series["SP2 TEMPERATURE"].Points.AddXY(SP2_böl[1], SP2_böl[5]);
                chart9.ChartAreas[0].AxisX.ScaleView.Scroll(System.Windows.Forms.DataVisualization.Charting.ScrollType.Last);
                this.chart10.Series["SP2 ROT RATE"].Points.AddXY(SP2_böl[1], SP2_böl[6]);
                chart10.ChartAreas[0].AxisX.ScaleView.Scroll(System.Windows.Forms.DataVisualization.Charting.ScrollType.Last);
                this.chart11.Series["SP2 VOLTAGE"].Points.AddXY(SP2_böl[1], SP2_böl[7]);
                chart11.ChartAreas[0].AxisX.ScaleView.Scroll(System.Windows.Forms.DataVisualization.Charting.ScrollType.Last);


                if (mqttaktif == 1)
                {
                    Mqtt_SP2();
                       
                        MqttClient.MqttMsgPublishEventHandler client_MqttMsgPublishReceived = null;
                        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

                        client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

                        if (!String.IsNullOrWhiteSpace(mqtt_sendSP2))
                        {
                            client.Publish(topic, Encoding.UTF8.GetBytes(mqtt_sendSP2), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
                        }                           
                }

            }
            catch (IndexOutOfRangeException)
            {
            }
        }
        string[] SP1_böl;
        private void SP1()
        {
            try
            { 
                SP1_böl = gsp1.Split(',');
                

                File.AppendAllText(spayload1, gsp1);

                listView2.View = View.Details;
                ListViewItem item2 = new ListViewItem(SP1_böl[0]);                                   /*    ListView        SP1     */
                item2.SubItems.Add(SP1_böl[1]);
                item2.SubItems.Add(SP1_böl[3]);
                item2.SubItems.Add(SP1_böl[2]);
                item2.SubItems.Add(SP1_böl[4]);
                item2.SubItems.Add(SP1_böl[5]);
                item2.SubItems.Add(SP1_böl[6]);
                item2.SubItems.Add(SP1_böl[7]);
                item2.SubItems.Add(SP1_böl[8]);
                listView2.Items.Add(item2);

                if (listView2.Items.Count == 8)
                    listView2.Items[0].Remove();

                this.listView2.Items[this.listView2.Items.Count - 1].EnsureVisible();

                listView4.View = View.Details;                     ///////SON TELEMETRİ SP1
                ListViewItem item3 = new ListViewItem(SP1_böl[2]);
                item3.SubItems.Add(SP1_böl[4]);
                item3.SubItems.Add(SP1_böl[5]);
                item3.SubItems.Add(SP1_böl[6]);
                item3.SubItems.Add(SP1_böl[7]);
               // item3.SubItems.Add(SP1_böl[7]);
                listView4.Items.Add(item3);

                if (listView4.Items.Count == 2)
                    listView4.Items[0].Remove();

                this.listView4.Items[this.listView4.Items.Count - 1].EnsureVisible();

                this.chart12.Series["SP1 ALTITUDE"].Points.AddXY(SP1_böl[1], SP1_böl[4]);                  /*    CHART            SP1     */
                chart12.ChartAreas[0].AxisX.ScaleView.Scroll(System.Windows.Forms.DataVisualization.Charting.ScrollType.Last);
                this.chart2.Series["SP1 TEMPERATURE"].Points.AddXY(SP1_böl[1], SP1_böl[5]);
                chart2.ChartAreas[0].AxisX.ScaleView.Scroll(System.Windows.Forms.DataVisualization.Charting.ScrollType.Last);
                this.chart3.Series["SP1 ROT RATE"].Points.AddXY(SP1_böl[1], SP1_böl[6]);
                chart3.ChartAreas[0].AxisX.ScaleView.Scroll(System.Windows.Forms.DataVisualization.Charting.ScrollType.Last);
                this.chart8.Series["SP1 VOLTAGE"].Points.AddXY(SP1_böl[1], SP1_böl[7]);
                chart8.ChartAreas[0].AxisX.ScaleView.Scroll(System.Windows.Forms.DataVisualization.Charting.ScrollType.Last);



                if (mqttaktif == 1)
                {
                    Mqtt_SP1();
                    MqttClient.MqttMsgPublishEventHandler client_MqttMsgPublishReceived = null;
                    client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

                    if (!String.IsNullOrWhiteSpace(mqtt_sendSP1))
                    {
                        client.Publish(topic, Encoding.UTF8.GetBytes(mqtt_sendSP1), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
                    }

                }

            }
            catch (IndexOutOfRangeException)
            {
            }
           

        }

        private void button7_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("CMD,3205,CX,ON,");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("CMD,3205,CX,OFF,");
        }

        private void button8_Click(object sender, EventArgs e)
        {           
            serialPort1.WriteLine("CMD,3205,ST," + DateTime.UtcNow.ToString("HH,mm,ss" + ","));  
        }

        private void button5_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("CMD,3205,SIM,ACTIVATE,"); 
        }

        private void button6_Click(object sender, EventArgs e)
        {
             serialPort1.WriteLine("CMD,3205,SIM,DISABLE,");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("CMD,3210,SP2,ON,");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("CMD,3210,SP2,OFF,");
        }

        private void button11_Click(object sender, EventArgs e)
        {           
            simp_aktif = 1;
        }
        private void button1_MouseDown(object sender, MouseEventArgs e) { button1.BackColor = Color.Green; }

        private void button1_MouseLeave(object sender, EventArgs e) { button1.BackColor = Color.White; }

        private void button2_MouseDown(object sender, MouseEventArgs e) { button2.BackColor = Color.Red; }

        private void button2_MouseLeave(object sender, EventArgs e) { button2.BackColor = Color.White; }

        private void button3_MouseDown(object sender, MouseEventArgs e) { button3.BackColor = Color.Green; }

        private void button3_MouseLeave(object sender, EventArgs e) { button3.BackColor = Color.White; }

        private void button13_MouseDown(object sender, MouseEventArgs e) { button13.BackColor = Color.Red; }

        private void button13_MouseLeave(object sender, EventArgs e) { button13.BackColor = Color.White; }

        private void button4_MouseDown(object sender, MouseEventArgs e) { button4.BackColor = Color.Green; }

        private void button4_MouseLeave(object sender, EventArgs e) { button4.BackColor = Color.White; }

        private void button5_MouseDown(object sender, MouseEventArgs e) { button5.BackColor = Color.Green; }

        private void button5_MouseLeave(object sender, EventArgs e) { button5.BackColor = Color.White; }

        private void button11_MouseDown(object sender, MouseEventArgs e) { button11.BackColor = Color.Green; }

        private void button11_MouseLeave(object sender, EventArgs e) { button11.BackColor = Color.White; }

        private void button6_MouseDown(object sender, MouseEventArgs e) { button6.BackColor = Color.Red; }

        private void button6_MouseLeave(object sender, EventArgs e) { button6.BackColor = Color.White; }

        private void button8_MouseDown(object sender, MouseEventArgs e) { button8.BackColor = Color.Green; }

        private void button8_MouseLeave(object sender, EventArgs e) { button8.BackColor = Color.White; }

        private void button12_MouseDown(object sender, MouseEventArgs e) { button12.BackColor = Color.Green; }

        private void button12_MouseLeave(object sender, EventArgs e) { button12.BackColor = Color.White; }

        private void button7_MouseDown(object sender, MouseEventArgs e) { button7.BackColor = Color.Green; }

        private void button7_MouseLeave(object sender, EventArgs e) { button7.BackColor = Color.White; }

        private void button14_MouseDown(object sender, MouseEventArgs e) { button14.BackColor = Color.Red; }

        private void button14_MouseLeave(object sender, EventArgs e) { button14.BackColor = Color.White; }

        private void button9_MouseDown(object sender, MouseEventArgs e) { button9.BackColor = Color.Green; }

        private void button9_MouseLeave(object sender, EventArgs e) { button9.BackColor = Color.White; }

        private void button15_MouseDown(object sender, MouseEventArgs e) { button15.BackColor = Color.Red; }

        private void button15_MouseLeave(object sender, EventArgs e) { button15.BackColor = Color.White; }

        private void button10_MouseDown(object sender, MouseEventArgs e) { button10.BackColor = Color.Green; }

        int i = 0;
        private void label3_Click(object sender, EventArgs e)
        {
            try
            {
                i++;
            if (i == 1)
            {
                   TextWriter tw = new StreamWriter(containerr);
                TextWriter tw1 = new StreamWriter(spayload1);
                TextWriter tw2 = new StreamWriter(spayload2);
                tw.Write(""); tw1.Write(""); tw2.Write("");
                tw.Close(); tw1.Close(); tw2.Close();
            }

            }
            catch (System.IO.IOException)
            {
                MessageBox.Show(".CSV dosyası acık");
            }
                       
        }

        private void button10_MouseLeave(object sender, EventArgs e) { button10.BackColor = Color.White; }

        private void button16_MouseDown(object sender, MouseEventArgs e) {button16.BackColor = Color.Red; }

        private void button16_MouseLeave(object sender, EventArgs e) { button16.BackColor = Color.White; }
    }
}