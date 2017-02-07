using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InSimDotNet;
using InSimDotNet.Packets;
using System.Globalization;
using System.Threading;
using InSimDotNet.Helpers;
using System.IO;

namespace DRS_InSim
{
    public partial class Form1 : Form
    {
        InSim insim = new InSim();

        public string Layoutname = "None";
        public string TrackName = "None";
        public string InSim_Version = "1.1.2b";

        // MySQL Variables
        public SQLInfo SqlInfo = new SQLInfo();
        public bool ConnectedToSQL = false;
        public int SQLRetries = 0;

        // MySQL Connect
        string SQLIPAddress = "127.0.0.1"; // 93.190.143.115
        string SQLDatabase = "lfs";
        string SQLUsername = "root";
        string SQLPassword = "1997andre";

        class Connections
        {
            // NCN fields
            public byte UCID;
            public string UName;
            public string PName;
            public bool IsAdmin;

            // Custom rankings
            public bool IsSuperAdmin;

            // Other
            public bool OnTrack;
            public int TotalDistance;
            public bool KMHoverMPH;
            public int points;

            // Laps
            public int LapsDone;
            public System.TimeSpan LapTime;
            public System.TimeSpan ERaceTime;
            public byte NumStops;
            public string CarName;
            public int Pitstops;
            public bool SentMSG;
        }
        class Players
        {
            public byte UCID;
            public byte PLID;
            public string PName;
            public string CName;

            public int kmh;
            public int mph;
            public string Plate;
        }

        private Dictionary<byte, Connections> _connections = new Dictionary<byte, Connections>();
        private Dictionary<byte, Players> _players = new Dictionary<byte, Players>();

        public Form1()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            InitializeComponent();
            RunInSim();
        }

        void RunInSim()
        {

            // Bind packet events.
            insim.Bind<IS_NCN>(NewConnection);
            insim.Bind<IS_NPL>(NewPlayer);
            insim.Bind<IS_MSO>(MessageReceived);
            insim.Bind<IS_MCI>(MultiCarInfo);
            insim.Bind<IS_CNL>(ConnectionLeave);
            // insim.Bind<IS_CPR>(ClientRenames);
            // insim.Bind<IS_PLL>(PlayerLeave);
            insim.Bind<IS_STA>(OnStateChange);
            // insim.Bind<IS_BTC>(ButtonClicked);
            // insim.Bind<IS_BFN>(ClearButtons);
            // insim.Bind<IS_VTN>(VoteNotify);
            insim.Bind<IS_AXI>(OnAutocrossInformation);
            // insim.Bind<IS_TINY>(OnTinyReceived);
            // insim.Bind<IS_CON>(CarCOntact);
            // insim.Bind<IS_BTT>(ButtonType);
            insim.Bind<IS_LAP>(Laps);
            insim.Bind<IS_RES>(Res);
            insim.Bind<IS_PIT>(Pitstop);
            insim.Bind<IS_HLV>(HotLapValidity);

            // Initialize InSim
            insim.Initialize(new InSimSettings
            {
                Host = "51.254.134.112", // 93.190.143.115
                Port = 29999,
                Admin = "2910693997",
                Prefix = '!',
                Flags = InSimFlags.ISF_MCI | InSimFlags.ISF_MSO_COLS | InSimFlags.ISF_CON | InSimFlags.ISF_RES_0 | InSimFlags.ISF_RES_1 | InSimFlags.ISF_NLP | InSimFlags.ISF_HLV,

                Interval = 500
            });

            insim.Send(new[]
            {
                new IS_TINY { SubT = TinyType.TINY_NCN, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_NPL, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_ISM, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_SST, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_MCI, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_NCI, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_AXI, ReqI = 255 },
                });

            // insim.Send("/cars " + AVAILABLE_CARS);
            insim.Send(255, "^8InSim connected with version ^2" + InSim_Version);

            ConnectedToSQL = SqlInfo.StartUp(SQLIPAddress, SQLDatabase, SQLUsername, SQLPassword);
            if (!ConnectedToSQL)
            {
                insim.Send(255, "SQL connect attempt failed! Attempting to reconnect in ^310 ^8seconds!");
                SQLReconnectTimer.Start();
                // SaveTimer.Start();
            }
            else
            {
                insim.Send(255, "^2Successfully loaded user data from MySQL database");
            }
        }

        #region ' Readers and Writers '
        private bool GetUserAdmin(string Username)
        {//reading admins.ini when connecting to server for InSim admin
            StreamReader CurrentFile = new StreamReader("files/admins.ini");

            string line = null;
            while ((line = CurrentFile.ReadLine()) != null)
            {
                if (line == Username)
                {
                    CurrentFile.Close();
                    return true;
                }
            }
            CurrentFile.Close();
            return false;
        }

        byte GetPlayer(byte UCID)
        {//Get Player from UCID
            byte PLID = 255;
            foreach (var CurrentPlayer in _players.Values) if (CurrentPlayer.UCID == UCID) PLID = CurrentPlayer.PLID;

            return PLID;
        }

        private Connections GetConnection(byte PLID)
        {//Get Connection from PLID
            Players NPL;
            if (_players.TryGetValue(PLID, out NPL)) return _connections[NPL.UCID];
            return null;
        }

        private bool IsConnAdmin(Connections Conn)
        {//general admin check, both Server and InSim
            if (Conn.IsAdmin == true || Conn.IsSuperAdmin == true) return true;
            return false;
        }

        bool TryParseCommand(IS_MSO mso, out string[] args)
        {
            if (mso.UserType == UserType.MSO_PREFIX)
            {
                var message = mso.Msg.Substring(mso.TextStart);
                args = message.Split();
                return args.Length > 0;
            }

            args = null;
            return false;
        }

        private void LogTextToFile(string file, string text, bool AdminMessage = true)
        {

            if (System.IO.File.Exists("files/" + file + ".log") == false) { FileStream CurrentFile = System.IO.File.Create("files/" + file + ".log"); CurrentFile.Close(); }

            StreamReader TextTempData = new StreamReader("files/" + file + ".log");
            string TempText = TextTempData.ReadToEnd();
            TextTempData.Close();

            StreamWriter TextData = new StreamWriter("files/" + file + ".log");
            TextData.WriteLine(TempText + DateTime.Now + ": " + text);
            TextData.Flush();
            TextData.Close();
        }
        #endregion

        // Player joins server
        void NewConnection(InSim insim, IS_NCN NCN)
        {
            try
            {
                _connections.Add(NCN.UCID, new Connections
                {
                    UCID = NCN.UCID,
                    UName = NCN.UName,
                    PName = NCN.PName,
                    IsAdmin = NCN.Admin,

                    IsSuperAdmin = GetUserAdmin(NCN.UName),
                    OnTrack = false,
                    TotalDistance = 0,
                    Pitstops = 0,
                    points = 0
                });

                if (ConnectedToSQL)
                {
                    try
                    {
                        if (SqlInfo.UserExist(NCN.UName))
                        {
                            // SqlInfo.UpdateUser(NCN.UName, true);//Updates the last joined time to the current one

                            string[] LoadedOptions = SqlInfo.LoadUserOptions(NCN.UName);
                            _connections[NCN.UCID].PName = LoadedOptions[0];
                            _connections[NCN.UCID].TotalDistance = Convert.ToInt32(LoadedOptions[1]);
                            _connections[NCN.UCID].points = Convert.ToInt32(LoadedOptions[2]);
                        }
                        else SqlInfo.AddUser(NCN.UName, StringHelper.StripColors(_connections[NCN.UCID].PName), _connections[NCN.UCID].TotalDistance, _connections[NCN.UCID].points);
                    }
                    catch (Exception EX)
                    {
                        if (!SqlInfo.IsConnectionStillAlive())
                        {
                            ConnectedToSQL = false;
                            SQLReconnectTimer.Start();
                        }
                        else Console.WriteLine("NCN(Add/Load)User - " + EX.Message);
                    }
                }

                if (NCN.UName != "")
                {
                    try
                    {
                        // Welcome messages
                        insim.Send(NCN.UCID, "^8Welcome back, " + NCN.PName);
                    }
                    catch (Exception EX)
                    {
                        LogTextToFile("InSim-Errors", "[" + NCN.UCID + "] " + StringHelper.StripColors(NCN.PName) + "(" + NCN.UName + ") NCN - Exception: " + EX, false);
                    }
                }
            }
            catch (Exception e) { LogTextToFile("InSim-Errors", "[" + NCN.UCID + "] " + StringHelper.StripColors(NCN.PName) + "(" + NCN.UName + ") NCN - Exception: " + e, false); }
        }

        // Player laps
        void Laps(InSim insim, IS_LAP LAP)
        {
            try
            {
                var conn = GetConnection(LAP.PLID);

                conn.ERaceTime = LAP.ETime;
                conn.LapsDone = LAP.LapsDone;
                conn.LapTime = LAP.LTime;
                conn.NumStops = LAP.NumStops;

                if (conn.SentMSG == false)
                {
                    insim.Send(conn.UCID, "^3[" + TrackName + "] ^8Completed a lap: ^3" + string.Format("{0:00}:{1:00}:{2:00}",
(int)_connections[conn.UCID].LapTime.Minutes,
    _connections[conn.UCID].LapTime.Seconds,
    _connections[conn.UCID].LapTime.Milliseconds) + " ^8- ^3" + conn.CarName);

                    
                }

                conn.SentMSG = false;



            }
            catch (Exception e) { LogTextToFile("InSim-Errors", "[" + LAP.PLID + "] " + " NCN - Exception: " + e, false); }
        }

        // Player laps
        void Res(InSim insim, IS_RES RES)
        {
            try
            {
                var conn = GetConnection(RES.PLID);

            }
            catch (Exception e) { LogTextToFile("InSim-Errors", "[" + RES.PLID + "] " + " NCN - Exception: " + e, false); }
        }

        // New player
        void NewPlayer(InSim insim, IS_NPL NPL)
        {
            try
            {
                var r = GetConnection(NPL.PLID);

                if (_players.ContainsKey(NPL.PLID))
                {
                    // Leaving pits, just update NPL object.
                    _players[NPL.PLID].UCID = NPL.UCID;
                    _players[NPL.PLID].PLID = NPL.PLID;
                    _players[NPL.PLID].PName = NPL.PName;
                    _players[NPL.PLID].CName = NPL.CName;
                    _players[NPL.PLID].Plate = NPL.Plate;
                }
                else
                {
                    // Add new player.
                    _players.Add(NPL.PLID, new Players
                    {
                        UCID = NPL.UCID,
                        PLID = NPL.PLID,
                        PName = NPL.PName,
                        CName = NPL.CName,
                        Plate = NPL.Plate,
                    });
                }

                Connections CurrentConnection = GetConnection(NPL.PLID);

                CurrentConnection.CarName = NPL.CName;
            }
            catch (Exception e) { LogTextToFile("InSim-Errors", "[" + NPL.PLID + "] " + " NCN - Exception: " + e, false); }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            insim.Send(255, "^1InSim is now off");
        }

        void Pitstop(InSim insim, IS_PIT PIT)
        {
            Connections CurrentConnection = GetConnection(PIT.PLID);

            CurrentConnection.Pitstops += 1;
        }

        void HotLapValidity(InSim insim, IS_HLV HLV)
        {
            Connections CurrentConnection = GetConnection(HLV.PLID);

            if (HLV.HLVC == HlvcFlags.Wall)
            {
                if (CurrentConnection.SentMSG == false)
                {
                    insim.Send(CurrentConnection.UCID, "^3CAR CONTACT WITH WALL, HOTLAP INVALIDATED.");
                    CurrentConnection.SentMSG = true;
                }
            }

            if (HLV.HLVC == HlvcFlags.Ground)
            {
                if (CurrentConnection.SentMSG == false)
                {
                    insim.Send(CurrentConnection.UCID, "^3CAR CONTACT WITH GRASS, HOTLAP INVALIDATED.");
                    CurrentConnection.SentMSG = true;
                }
            }
        }

        private void OnStateChange(InSim insim, IS_STA STA)
        {
            try
            {
                if (TrackName != STA.Track)
                {
                    TrackName = STA.Track;
                    insim.Send(new IS_TINY { SubT = TinyType.TINY_AXI, ReqI = 255 });
                }
            }
            catch (Exception EX) { LogTextToFile("packetError", "STA - " + EX.Message); }
        }

        private void OnAutocrossInformation(InSim insim, IS_AXI AXI)
        {
            try
            {
                if (AXI.NumO != 0)
                {
                    Layoutname = AXI.LName;
                    if (AXI.ReqI == 0) insim.Send("Layout ^1" + Layoutname + " ^3loaded");
                }
            }
            catch (Exception EX) { LogTextToFile("packetError", "AXI - " + EX.Message); }
        }

        private void OnTinyReceived(InSim insim, IS_TINY TINY)
        {
            if (TINY.SubT == TinyType.TINY_AXC)
            {
                try
                {
                    if (Layoutname != "None") insim.Send("Layout ^1" + Layoutname + " ^3cleared");
                    Layoutname = "None";
                }
                catch (Exception EX) { LogTextToFile("packetError", "AXC - " + EX.Message); }
            }
        }

        void ConnectionLeave(InSim insim, IS_CNL CNL)
        {
            try
            {
                if (ConnectedToSQL)
                {
                    try { SqlInfo.UpdateUser(_connections[CNL.UCID].UName, StringHelper.StripColors(_connections[CNL.UCID].PName), _connections[CNL.UCID].TotalDistance, _connections[CNL.UCID].points); }
                    catch (Exception EX)
                    {
                        if (!SqlInfo.IsConnectionStillAlive())
                        {
                            ConnectedToSQL = false;
                            SQLReconnectTimer.Start();
                        }
                        else { LogTextToFile("error", "CNL - Exception: " + EX, false); }
                }
                }
            }
            catch (Exception e) { LogTextToFile("error", "CNL - Exception: " + e, false); }
        }

        // MCI - Multi Car Info
        private void MultiCarInfo(InSim insim, IS_MCI mci)
        {
            try
            {
                {
                    foreach (CompCar car in mci.Info)
                    {
                        Connections conn = GetConnection(car.PLID);
                        if (conn.OnTrack == true && conn.UCID != 0)
                        {
                            int Sped = Convert.ToInt32(MathHelper.SpeedToKph(car.Speed));

                            decimal SpeedMS = (int)(((car.Speed / 32768f) * 100f) / 2);
                            decimal Speed = (int)((car.Speed * (100f / 32768f)) * 3.6f);

                            int kmh = car.Speed / 91;
                            int mph = car.Speed / 146;
                            var X = car.X / 65536;
                            var Y = car.Y / 65536;
                            var Z = car.Z / 65536;
                            var angle = car.AngVel / 30;
                            string anglenew = "";
                            // int Angle = AbsoluteAngleDifference(car.Direction, car.Heading);

                            _players[car.PLID].kmh = kmh;
                            _players[car.PLID].mph = mph;

                            conn.TotalDistance += Convert.ToInt32(SpeedMS);
                            // anglenew = angle.ToString().Replace("-", "");
                        }
                    }
                }
            }
            catch (Exception e) { LogTextToFile("error", "MCI - Exception: " + e, false); }
        }
    }
}
