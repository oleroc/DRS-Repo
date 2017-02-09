using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InSimDotNet;

namespace DRS_InSim
{
    public partial class Form1
    {
        System.Timers.Timer SQLReconnectTimer = new System.Timers.Timer();
        System.Timers.Timer SaveTimer = new System.Timers.Timer();

        private void SQLReconnectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            {
                SQLRetries++;
                ConnectedToSQL = SqlInfo.StartUp(SQLIPAddress, SQLDatabase, SQLUsername, SQLPassword);
                if (!ConnectedToSQL)
                {
                    insim.Send(255, "SQL connect attempt failed! Attempting to reconnect in ^310 ^8seconds!");
                }
                else
                {
                    insim.Send(255, "SQL connected after ^3" + SQLRetries + " ^8times!");
                    SQLRetries = 0;
                    SQLReconnectTimer.Stop();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // SQL timer
            SQLReconnectTimer.Interval = 10000;
            SQLReconnectTimer.Elapsed += new System.Timers.ElapsedEventHandler(SQLReconnectTimer_Elapsed);

            // Save timer
            SaveTimer.Interval = 3000;
            SaveTimer.Elapsed += new System.Timers.ElapsedEventHandler(Savetimer_Elapsed);
            SaveTimer.Enabled = true;
        }

        private void Savetimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                foreach (var conn in _connections.Values)
                {
                    if (ConnectedToSQL)
                    {
                        try
                        {
                            SqlInfo.UpdateUser(_connections[conn.UCID].UName, _connections[conn.UCID].PName, _connections[conn.UCID].TotalDistance, _connections[conn.UCID].points);

                            ptsFIRST = SqlInfo.showFIRST();
                            ptsSECOND = SqlInfo.showSECOND();
                            ptsTHIRD = SqlInfo.showTHIRD();
                            ptsFORTH = SqlInfo.showFORTH();

                            if (_connections[conn.UCID].OnTrack == false)
                            {
                                // UpdateGui(conn.UCID, true);
                            }
                        }
                        catch (Exception EX)
                        {
                            if (!SqlInfo.IsConnectionStillAlive())
                            {
                                ConnectedToSQL = false;
                                SQLReconnectTimer.Start();
                            }
                            LogTextToFile("sqlerror", "[" + conn.UCID + "] " + (_connections[conn.UCID].PName) + "(" + _connections[conn.UCID].UName + ") conn - Exception: " + EX.Message, false);
                        }
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show("" + f.Message, "AN ERROR OCCURED");
            }
        }
    }
}
