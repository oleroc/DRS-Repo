using System;
using System.Windows.Forms;
using InSimDotNet;
using InSimDotNet.Packets;
using InSimDotNet.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace DRS_InSim
{
    public partial class Form1
    {
        private void MessageReceived(InSim insim, IS_MSO mso)
        {
            try
            {
                // const string TimeFormat = "HH:mm";//ex: 23/03/2003
                {
                    // chatbox.Text += StringHelper.StripColors(mso.Msg.ToString() + " \r\n");

                    if (mso.UserType == UserType.MSO_PREFIX)
                    {
                        string Text = mso.Msg.Substring(mso.TextStart, (mso.Msg.Length - mso.TextStart));
                        string[] command = Text.Split(' ');
                        command[0] = command[0].ToLower();

                        switch (command[0])
                        {
                            case "!stats":

                                if (_connections[mso.UCID].inStats == false)
                                {
                                    // DARK WINDOW
                                    insim.Send(new IS_BTN
                                    {
                                        UCID = mso.UCID,
                                        ReqI = 25,
                                        ClickID = 25,
                                        BStyle = ButtonStyles.ISB_DARK,
                                        H = 87,
                                        W = 111,
                                        T = 43, // up to down
                                        L = 54, // left to right
                                    });

                                    insim.Send(new IS_BTN
                                    {
                                        UCID = mso.UCID,
                                        ReqI = 26,
                                        ClickID = 26,
                                        BStyle = ButtonStyles.ISB_LIGHT,
                                        H = 4,
                                        W = 4,
                                        T = 45, // up to down
                                        L = 56, // left to right
                                        Text = "^7#"
                                    });

                                    insim.Send(new IS_BTN
                                    {
                                        UCID = mso.UCID,
                                        ReqI = 27,
                                        ClickID = 27,
                                        BStyle = ButtonStyles.ISB_LIGHT,
                                        H = 4,
                                        W = 30,
                                        T = 45, // up to down
                                        L = 60, // left to right
                                        Text = "^7-- Name --"
                                    });

                                    insim.Send(new IS_BTN
                                    {
                                        UCID = mso.UCID,
                                        ReqI = 28,
                                        ClickID = 28,
                                        BStyle = ButtonStyles.ISB_LIGHT,
                                        H = 4,
                                        W = 17,
                                        T = 45, // up to down
                                        L = 90, // left to right
                                        Text = "^7-- Best Lap --"
                                    });

                                    insim.Send(new IS_BTN
                                    {
                                        UCID = mso.UCID,
                                        ReqI = 29,
                                        ClickID = 29,
                                        BStyle = ButtonStyles.ISB_LIGHT,
                                        H = 4,
                                        W = 22,
                                        T = 45, // up to down
                                        L = 107, // left to right
                                        Text = "^7-- Vehicle --"
                                    });

                                    insim.Send(new IS_BTN
                                    {
                                        UCID = mso.UCID,
                                        ReqI = 30,
                                        ClickID = 30,
                                        BStyle = ButtonStyles.ISB_LIGHT,
                                        H = 4,
                                        W = 17,
                                        T = 45, // up to down
                                        L = 129, // left to right
                                        Text = "^7-- Plate --"
                                    });

                                    insim.Send(new IS_BTN
                                    {
                                        UCID = mso.UCID,
                                        ReqI = 116,
                                        ClickID = 116,
                                        BStyle = ButtonStyles.ISB_LIGHT,
                                        H = 4,
                                        W = 17,
                                        T = 45, // up to down
                                        L = 146, // left to right
                                        Text = "^7-- Points --"
                                    });

                                    insim.Send(new IS_BTN
                                    {
                                        UCID = mso.UCID,
                                        ReqI = 134,
                                        ClickID = 134,
                                        BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                        H = 10,
                                        W = 107,
                                        T = 118, // up to down
                                        L = 56, // left to right
                                        Text = "^1›^3›^1›^3›^1› ^7CLOSE WINDOW ^1‹^3‹^1‹^3‹^1‹"
                                    });

                                    byte numbers = 1;
                                    byte LocationY = 49;
                                    byte ClickID1 = 31;
                                    byte ClickID2 = 48;
                                    byte ClickID3 = 65;
                                    byte ClickID4 = 82;
                                    byte ClickID5 = 99;
                                    byte ClickID6 = 117;

                                    if (_connections.Count < 16)
                                    {

                                        SortedList<int, string> sorted = new SortedList<int, string>();
                                        

                                        foreach (var o in _connections.Values)
                                        {
                                            sorted.Add(o.points, "points");



                                            {
                                                if (o.UCID != 0)
                                                {
                                                    insim.Send(new IS_BTN
                                                    {
                                                        UCID = mso.UCID,
                                                        ReqI = ClickID1,
                                                        ClickID = ClickID1,
                                                        BStyle = ButtonStyles.ISB_DARK,
                                                        H = 4,
                                                        W = 4,
                                                        T = LocationY, // up to down
                                                        L = 56, // left to right
                                                        Text = "^7" + numbers
                                                    });

                                                    insim.Send(new IS_BTN
                                                    {
                                                        UCID = mso.UCID,
                                                        ReqI = ClickID2,
                                                        ClickID = ClickID2,
                                                        BStyle = ButtonStyles.ISB_DARK,
                                                        H = 4,
                                                        W = 30,
                                                        T = LocationY, // up to down
                                                        L = 60, // left to right
                                                        Text = "^7" + o.PName + " ^7(" + o.UName + ")"
                                                    });

                                                    if (o.CurrentMapHotlap == "" || o.CurrentMapHotlap == "0")
                                                    {
                                                        // Best Lap
                                                        insim.Send(new IS_BTN
                                                        {
                                                            UCID = mso.UCID,
                                                            ReqI = ClickID5,
                                                            ClickID = ClickID5,
                                                            BStyle = ButtonStyles.ISB_DARK,
                                                            H = 4,
                                                            W = 17,
                                                            T = LocationY, // up to down
                                                            L = 90, // left to right
                                                            Text = "^7not set"
                                                        });
                                                    }
                                                    else
                                                    {
                                                        // Best Lap
                                                        insim.Send(new IS_BTN
                                                        {
                                                            UCID = mso.UCID,
                                                            ReqI = ClickID5,
                                                            ClickID = ClickID5,
                                                            BStyle = ButtonStyles.ISB_DARK,
                                                            H = 4,
                                                            W = 17,
                                                            T = LocationY, // up to down
                                                            L = 90, // left to right
                                                            Text = "^7" + Convert.ToString(o.CurrentMapHotlap)
                                                        });
                                                    }


                                                    if (o.OnTrack == true)
                                                    {
                                                        insim.Send(new IS_BTN
                                                        {
                                                            UCID = mso.UCID,
                                                            ReqI = ClickID3,
                                                            ClickID = ClickID3,
                                                            BStyle = ButtonStyles.ISB_DARK,
                                                            H = 4,
                                                            W = 22,
                                                            T = LocationY, // up to down
                                                            L = 107, // left to right
                                                            Text = "^7" + CarHelper.GetFullCarName(o.CarName)
                                                        });
                                                    }
                                                    else
                                                    {
                                                        insim.Send(new IS_BTN
                                                        {
                                                            UCID = mso.UCID,
                                                            ReqI = ClickID3,
                                                            ClickID = ClickID3,
                                                            BStyle = ButtonStyles.ISB_DARK,
                                                            H = 4,
                                                            W = 22,
                                                            T = LocationY, // up to down
                                                            L = 107, // left to right
                                                            Text = "^1Off-track"
                                                        });
                                                    }

                                                    if (o.OnTrack == true)
                                                    {
                                                        if (o.Plate == "")
                                                        {
                                                            insim.Send(new IS_BTN
                                                            {
                                                                UCID = mso.UCID,
                                                                ReqI = ClickID4,
                                                                ClickID = ClickID4,
                                                                BStyle = ButtonStyles.ISB_DARK,
                                                                H = 4,
                                                                W = 17,
                                                                T = LocationY, // up to down
                                                                L = 129, // left to right
                                                                Text = "^7None"
                                                            });
                                                        }
                                                        else
                                                        {
                                                            insim.Send(new IS_BTN
                                                            {
                                                                UCID = mso.UCID,
                                                                ReqI = ClickID4,
                                                                ClickID = ClickID4,
                                                                BStyle = ButtonStyles.ISB_DARK,
                                                                H = 4,
                                                                W = 17,
                                                                T = LocationY, // up to down
                                                                L = 129, // left to right
                                                                Text = "^7" + o.Plate
                                                            });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        insim.Send(new IS_BTN
                                                        {
                                                            UCID = mso.UCID,
                                                            ReqI = ClickID4,
                                                            ClickID = ClickID4,
                                                            BStyle = ButtonStyles.ISB_DARK,
                                                            H = 4,
                                                            W = 17,
                                                            T = LocationY, // up to down
                                                            L = 129, // left to right
                                                            Text = "^1Off-track"
                                                        });
                                                    }

                                                    foreach (var pair in sorted)
                                                    {


                                                        insim.Send(new IS_BTN
                                                        {
                                                            UCID = mso.UCID,
                                                            ReqI = ClickID6,
                                                            ClickID = ClickID6,
                                                            BStyle = ButtonStyles.ISB_DARK,
                                                            H = 4,
                                                            W = 17,
                                                            T = LocationY, // up to down
                                                            L = 146, // left to right
                                                            Text = "^2" + pair
                                                        });
                                                    }


                                                    LocationY += 4;
                                                    ClickID1++;
                                                    ClickID2++;
                                                    ClickID3++;
                                                    ClickID4++;
                                                    ClickID5++;
                                                    ClickID6++;
                                                    numbers++;
                                                    _connections[mso.UCID].inStats = true;
                                                }
                                            }
                                        }
                                    }









                                }

                                break;

                            case "!ap":
                            case "!adminpanel":
                                var conn = _connections[mso.UCID];

                                ptsFIRST = Convert.ToInt32(onepts);
                                ptsSECOND = Convert.ToInt32(twopts);
                                ptsTHIRD = Convert.ToInt32(threepts);
                                ptsFORTH = Convert.ToInt32(fourpts);
                                
                                if (conn.IsAdmin == true)
                                {
                                    if (conn.inAP == false)
                                    {
                                        #region ' buttons '

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 25,
                                            ClickID = 25,
                                            BStyle = ButtonStyles.ISB_DARK,
                                            H = 80,
                                            W = 50,
                                            T = 52, // up to down
                                            L = 75, // left to right
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 26,
                                            ClickID = 26,
                                            BStyle = ButtonStyles.ISB_LIGHT,
                                            H = 9,
                                            W = 48,
                                            T = 53, // up to down
                                            L = 76, // left to right
                                            Text = "^7ADMIN PANELET"
                                        });



                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 27,
                                            ClickID = 27,
                                            BStyle = ButtonStyles.ISB_C4,
                                            H = 5,
                                            W = 48,
                                            T = 65, // up to down
                                            L = 76, // left to right
                                            Text = "^3** Point Administration **"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 28,
                                            ClickID = 28,
                                            BStyle = ButtonStyles.ISB_RIGHT,
                                            H = 4,
                                            W = 13,
                                            T = 73, // up to down
                                            L = 88, // left to right
                                            Text = "^31st. Place:"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 29,
                                            ClickID = 29,
                                            BStyle = ButtonStyles.ISB_RIGHT,
                                            H = 4,
                                            W = 13,
                                            T = 77, // up to down
                                            L = 88, // left to right
                                            Text = "^32nd Place:"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 30,
                                            ClickID = 30,
                                            BStyle = ButtonStyles.ISB_RIGHT,
                                            H = 4,
                                            W = 13,
                                            T = 81, // up to down
                                            L = 88, // left to right
                                            Text = "^33rd Place:"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 31,
                                            ClickID = 31,
                                            BStyle = ButtonStyles.ISB_RIGHT,
                                            H = 4,
                                            W = 13,
                                            T = 85, // up to down
                                            L = 88, // left to right
                                            Text = "^34th Place:"
                                        });






                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 32,
                                            ClickID = 32,
                                            BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                            H = 4,
                                            W = 5,
                                            T = 73, // up to down
                                            L = 102, // left to right
                                            Text = "^7" + ptsFIRST,
                                            TypeIn = 3,
                                            Caption = "^0Amount of points to reward 1st place"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 33,
                                            ClickID = 33,
                                            BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                            H = 4,
                                            W = 5,
                                            T = 77, // up to down
                                            L = 102, // left to right
                                            Text = "^7" + ptsSECOND,
                                            TypeIn = 3,
                                            Caption = "^0Amount of points to reward 2nd place"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 34,
                                            ClickID = 34,
                                            BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                            H = 4,
                                            W = 5,
                                            T = 81, // up to down
                                            L = 102, // left to right
                                            Text = "^7" + ptsTHIRD,
                                            TypeIn = 3,
                                            Caption = "^0Amount of points to reward 3rd place"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 35,
                                            ClickID = 35,
                                            BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                            H = 4,
                                            W = 5,
                                            T = 85, // up to down
                                            L = 102, // left to right
                                            Text = "^7" + ptsFORTH,
                                            TypeIn = 3,
                                            Caption = "^0Amount of points to reward 4th place"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 36,
                                            ClickID = 36,
                                            BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                            H = 4,
                                            W = 9,
                                            T = 73, // up to down
                                            L = 111, // left to right
                                            Text = "^7DEFAULT"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 37,
                                            ClickID = 37,
                                            BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                            H = 4,
                                            W = 9,
                                            T = 85, // up to down
                                            L = 111, // left to right
                                            Text = "^7CLOSE"
                                        });





                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 38,
                                            ClickID = 38,
                                            BStyle = ButtonStyles.ISB_RIGHT,
                                            H = 4,
                                            W = 19,
                                            T = 92, // up to down
                                            L = 82, // left to right
                                            Text = "^3Layout Database:"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 40,
                                            ClickID = 40,
                                            BStyle = ButtonStyles.ISB_RIGHT,
                                            H = 4,
                                            W = 13,
                                            T = 96, // up to down
                                            L = 88, // left to right
                                            Text = "^3Load layout:"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 42,
                                            ClickID = 42,
                                            BStyle = ButtonStyles.ISB_RIGHT,
                                            H = 4,
                                            W = 13,
                                            T = 100, // up to down
                                            L = 88, // left to right
                                            Text = "^3Save layout:"
                                        });






                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 39,
                                            ClickID = 39,
                                            BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                            H = 4,
                                            W = 18,
                                            T = 92, // up to down
                                            L = 102, // left to right
                                            Text = "^7/axlist"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 41,
                                            ClickID = 41,
                                            BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                            H = 4,
                                            W = 8,
                                            T = 96, // up to down
                                            L = 102, // left to right
                                            Text = "^7Load"
                                        });

                                        insim.Send(new IS_BTN
                                        {
                                            UCID = mso.UCID,
                                            ReqI = 43,
                                            ClickID = 43,
                                            BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                                            H = 4,
                                            W = 8,
                                            T = 100, // up to down
                                            L = 102, // left to right
                                            Text = "^7Save"
                                        });




                                        #endregion

                                        conn.inAP = true;
                                    }
                                    else
                                    {
                                        insim.Send(mso.UCID, "^1Du er allerede i admin panelet!");
                                    }
                                }
                                else
                                {
                                    insim.Send(mso.UCID, "^1No access");
                                }

                                break;

                            case "!track":

                                insim.Send(mso.UCID, "^3Track: ^8" + TrackName + " - " + TrackHelper.GetFullTrackName(TrackName));


                            break;

                            case "!test":

                                insim.Send(255, ".CurrentMapHotlap: " + _connections[mso.UCID].CurrentMapHotlap);

                                break;

                            case "!deletepts":
                            case "!deletepoints":
                            case "!delpts":
                                _connections[mso.UCID].points = 0;
                                SqlInfo.deleteownPTS(_connections[mso.UCID].UName);
                                insim.Send(mso.UCID, "^8Your points has reset");
                                UpdateGui(255, true);
                                break;

                            case "!deletedist":
                            case "!deletedistance":
                            case "!deldist":
                                _connections[mso.UCID].TotalDistance = 0;
                                SqlInfo.deleteownDIST(_connections[mso.UCID].UName);
                                insim.Send(mso.UCID, "^8Your total distance has reset");
                                UpdateGui(255, true);
                                break;

                            case "!deleteptsall":
                            case "!deletepointsall":
                            case "!delptsall":
                                if (_connections[mso.UCID].IsAdmin == true)
                                {
                                    foreach (var conns in _connections.Values)
                                    {
                                        conns.points = 0;
                                        SqlInfo.deletePTS();
                                    }

                                    insim.Send(mso.UCID, "^3" + dbCount + " ^8entries of column ^3points ^8deleted.");
                                    UpdateGui(255, true);
                                }
                                else
                                {
                                    insim.Send(mso.UCID, "^1No access");
                                }
                                break;

                            case "!deldistanceall":
                            case "!deldistall":
                            case "!deletedistall":
                            case "!deletedistanceall":
                                if (_connections[mso.UCID].IsAdmin == true)
                                {
                                    foreach (var conns in _connections.Values)
                                    {
                                        conns.TotalDistance = 0;
                                        SqlInfo.deleteDIST();
                                    }

                                    insim.Send(mso.UCID, "^3" + dbCount + " ^8entries of column ^3distance ^8deleted.");
                                    UpdateGui(255, true);
                                }
                                else
                                {
                                    insim.Send(mso.UCID, "^1No access");
                                }
                                break;

                            case "!ac":
                                {//Admin chat
                                    if (mso.UCID == _connections[mso.UCID].UCID)
                                    {
                                        if (!IsConnAdmin(_connections[mso.UCID]))
                                        {
                                            insim.Send(mso.UCID, 0, "You are not an admin");
                                            break;
                                        }
                                        if (command.Length == 1)
                                        {
                                            insim.Send(mso.UCID, 0, "^1Invalid command format. ^2Usage: ^7!ac <text>");
                                            break;
                                        }

                                        string atext = Text.Remove(0, command[0].Length + 1);

                                        foreach (var Conn in _connections.Values)
                                        {
                                            {
                                                if (IsConnAdmin(Conn) && Conn.UName != "")
                                                {
                                                    insim.Send(Conn.UCID, 0, "^3Admin chat: ^7" + _connections[mso.UCID].PName + " ^8(" + _connections[mso.UCID].UName + "):");
                                                    insim.Send(Conn.UCID, 0, "^7" + atext);
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }

                            case "!p":
                            case "!pen":
                            case "!penalty":
                                insim.Send("/p_clear " + _connections[mso.UCID].UName);
                                break;

                            case "!help":
                                insim.Send(mso.UCID, 0, "^3Help commands (temporary list):");
                                insim.Send(mso.UCID, 0, "^7!help ^8- See a list of available commands");



                                // Admin commands
                                foreach (var CurrentConnection in _connections.Values)
                                {
                                    if (CurrentConnection.UCID == mso.UCID)
                                    {
                                        if (IsConnAdmin(CurrentConnection) && CurrentConnection.UName != "")
                                        {
                                            insim.Send(CurrentConnection.UCID, 0, "^3Administrator commands:");
                                            insim.Send(CurrentConnection.UCID, 0, "^7!ac ^8- Talk with the other admins that are online");
                                            insim.Send(CurrentConnection.UCID, 0, "^7!pos ^8- Check your current position in x y z");
                                        }
                                    }
                                }

                                break;

                            default:
                                insim.Send(mso.UCID, 0, "^8Invalid command, type ^2{0}^8 to see available commands", "!help");
                                break;
                        }
                    }
                }
            }
            catch (Exception e) { LogTextToFile("commands", "[" + mso.UCID + "] " + StringHelper.StripColors(_connections[mso.UCID].PName) + "(" + GetConnection(mso.PLID).UName + ") NPL - Exception: " + e, false); }
        }
    }

}
