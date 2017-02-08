using MySql.Data.MySqlClient;
using System;
using InSimDotNet.Helpers;

namespace DRS_InSim
{
    public class SQLInfo
    {
        #region General Stuff
        const string TimeFormat = "HH:mm dd/MM/yyyy";//ex: 23:00 23/03/2003
        MySqlConnection SQL = new MySqlConnection();
        public SQLInfo() { }

        public bool IsConnectionStillAlive()
        {
            try
            {
                if (SQL.State == System.Data.ConnectionState.Open) return true;
                else return false;
            }
            catch { return false; }
        }
        public bool StartUp(string server, string database, string username, string password)
        {
            try
            {
                if (IsConnectionStillAlive()) return true;

                SQL.ConnectionString = "Server=" + server +
                    ";Database=" + database +
                    ";Uid=" + username +
                    ";Pwd=" + password +
                    ";Connect Timeout=10;";
                SQL.Open();

                Query("CREATE TABLE IF NOT EXISTS users(PRIMARY KEY(playername),playername CHAR(30) NOT NULL,nickname CHAR(40) NOT NULL,distance int(10),points int(10));");
                // Query("CREATE TABLE IF NOT EXISTS users(PRIMARY KEY(username),username CHAR(25) NOT NULL,nickname CHAR(40) NOT NULL,distance int(10),points int(10));");
                // Query("CREATE TABLE IF NOT EXISTS admin_settings(PRIMARY KEY(settings),settings CHAR(25) NOT NULL,firstplace byte(3), secondplace byte(3), thirdplace byte(3), forthplace byte(3));");
            }
            catch { return false; }
            return true;
        }
        public int Query(string str)
        {
            MySqlCommand query = new MySqlCommand();
            query.Connection = SQL;
            query.CommandText = str;
            query.Prepare();
            return query.ExecuteNonQuery();
        }
        string RemoveStupidCharacters(string text)
        {
            if (text.Contains("'")) text = text.Replace('\'', '`');
            if (text.Contains("‘")) text = text.Replace('‘', '`');
            if (text.Contains("’")) text = text.Replace('’', '`');
            if (text.Contains("^h")) text = text.Replace("^h", "#");

            return text;
        }
        #endregion
        #region Player Saving Stuff
        public bool UserExist(string playername, string table = "users")
        {
            MySqlCommand query = new MySqlCommand();
            query.Connection = SQL;
            query.CommandText = "SELECT playername FROM " + table + " WHERE playername='" + playername + "' LIMIT 1;";
            query.Prepare();
            MySqlDataReader dr = query.ExecuteReader();

            bool found = false;

            if (dr.Read()) if (dr.GetString(0) != "") found = true;
            dr.Close();

            return found;
        }


        public void AddUser(string playername, string nickname, int distance, int points)
        {
            if (playername == "") return;
            Query("INSERT INTO users VALUES ('" + playername + "', '" + StringHelper.StripColors(nickname) + "', " + distance + ", " + points + ");");
        }
        public void UpdateUser(string playername, string nickname, int distance, int points)
        {
            Query("UPDATE users SET nickname='" + nickname + "', distance=" + distance + ", points=" + points + " WHERE playername='" + playername + "';");
        }
        public string[] LoadUserOptions(string playername)
        {
            string[] options = new string[2];

            MySqlCommand query = new MySqlCommand();
            query.Connection = SQL;
            query.CommandText = "SELECT distance, points FROM users WHERE playername='" + playername + "' LIMIT 1;";
            query.Prepare();
            MySqlDataReader dr = query.ExecuteReader();

            if (dr.Read())
                if (dr.GetString(0) != "")
                {
                    options[0] = dr.GetString(0);
                    options[1] = dr.GetString(1);
                }
            dr.Close();

            return options;
        }
        #endregion
    }
}