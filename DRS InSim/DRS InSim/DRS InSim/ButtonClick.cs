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
using InSimDotNet.Helpers;
using InSimDotNet.Packets;

namespace DRS_InSim
{
    public partial class Form1
    {
        private void BTC_ClientClickedButton(IS_BTC BTC)
        {
            var conn = _connections[BTC.UCID];

            try
            {
                switch (BTC.ClickID)
                {
                    case 36:

                        if (conn.inAP == true)
                        {
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 25);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 26);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 27);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 28);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 29);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 30);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 31);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 32);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 33);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 34);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 35);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 36);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 37);

                            onepts = Convert.ToString(4);
                            twopts = Convert.ToString(3);
                            threepts = Convert.ToString(2);
                            fourpts = Convert.ToString(1);

                            SqlInfo.updateptsFIRST(Convert.ToInt32(4));
                            SqlInfo.updateptsSECOND(Convert.ToInt32(3));
                            SqlInfo.updateptsTHIRD(Convert.ToInt32(2));
                            SqlInfo.updateptsFORTH(Convert.ToInt32(1));

                            insim.Send(BTC.UCID, "^2All values has been set to default");
                            conn.inAP = false;
                        }
                        
                        break;

                    case 37:

                        if (conn.inAP == true)
                        {
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 25);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 26);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 27);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 28);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 29);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 30);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 31);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 32);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 33);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 34);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 35);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 36);
                            deleteBtn(BTC.UCID, BTC.ReqI, true, 37);
                            conn.inAP = false;
                        }

                        break;
                }
            }
            catch (Exception e) { LogTextToFile("error", "[" + BTC.UCID + "] " + StringHelper.StripColors(_connections[BTC.UCID].PName) + "(" + _connections[BTC.UCID].UName + ") - BTC - Exception: " + e, false); }
        }
    }
}
