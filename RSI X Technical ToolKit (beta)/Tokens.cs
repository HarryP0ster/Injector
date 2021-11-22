using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace RSI_X_Desktop
{
    public struct langHolder
    {
        public langHolder(string lS, string lF, string T, bool A)
        {
            langShort = lS;
            langFull = lF;
            token = T;
            isActive = A;
        }
        public string langShort;
        public string langFull;
        public string token;
        public bool isActive;

        static public langHolder Empty => new langHolder();
    };

    class Tokens
    {
        private string Token = "";
        private string HostName = "";
        private Dictionary<string, string> Languages = new Dictionary<string, string>();
        private string roomName = "";

        List<langHolder> langCollectionTarget = new();
        List<langHolder> langCollectionTransl = new();

        //public langHolder GetLangHolder
        //{
        //    get => langCollection;
        //}

        public string GetToken
        { get => Token; }
        public string GetHostName
        { get => HostName; }
        
        public Dictionary<string, string> GetLanguages
        { get => Languages; }

        public List<langHolder> GetTargetLangs
        { get => langCollectionTarget; }
        public List<langHolder> GetTranslLangs
        { get => langCollectionTransl; }

        public string GetRoomName
        { get => roomName; }

        public bool TakeToken(string RoomName)
        {
            //RoomName = "10482357";
            string html = string.Empty;
            HostName    = string.Empty; 
            Token       = string.Empty;
            roomName    = string.Empty;

            Languages.Clear();
            langCollectionTarget = new();
            langCollectionTransl = new();

            WebRequest request = WebRequest.Create("https://secure.rsi.exchange:3222/get_tokens_by_uuid_keyvalue?uuid_=" + RoomName);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to establish connection.");
                return false;
            }

            using (Stream stream = response.GetResponseStream())
            using (StreamReader sreader = new StreamReader(stream))
            {
                html = sreader.ReadToEnd();
            }

            JArray jsonVal = JArray.Parse(html);
            JToken tokVal = jsonVal.First();
            var check = jsonVal.First().Value<JValue>("status");

            if (check.ToString() != "ok") 
                return false;

            foreach (JObject content in tokVal.ElementAt(3).Children<JObject>())
            {
                foreach (JProperty prop in content.Properties())
                {
                    string Token = tokVal.ElementAt(3).First().Value<JValue>(prop.Name).ToString();
                    Languages.Add(prop.Name, Token);

                    //int Del = prop.Name.IndexOf("_");
                    //string shortName = prop.Name.Remove(Del, prop.Name.Length - Del);
                    string shortName = prop.Name.Split('_')[0];
                    langHolder tmp = new()
                    {
                        langFull = prop.Name,
                        langShort = shortName,
                        token = Token,
                        isActive = true,
                    };
                    langCollectionTarget.Add(tmp);
                }
            }

            foreach (JObject content in tokVal.ElementAt(2).Children<JObject>())
            {
                foreach (JProperty prop in content.Properties())
                {
                    string Token = tokVal.ElementAt(2).First().Value<JValue>(prop.Name).ToString();
                    //Languages.Add(prop.Name, Token);

                    //int Del = prop.Name.IndexOf("_");
                    //prop.Name.Remove(Del, prop.Name.Length - Del);

                    string shortName = prop.Name.Split('_')[0];
                    langHolder tmp = new()
                    {
                        langFull = prop.Name,
                        langShort = shortName,
                        token = Token,
                        isActive = true,
                    };
                    langCollectionTransl.Add(tmp);
                }
            }

            HostName    = Languages.ElementAt(0).Key;
            Token       = Languages.ElementAt(0).Value;
            roomName    = tokVal.ElementAt(5).First().ToString();

            if (HostName == "" || Token == "") return false;
            return true;
        }

        public langHolder GetInterpRoomsAt(int ind)
        {
            try
            {
                return langCollectionTransl[ind];
            }
            catch { }
            return new langHolder();
        }

        public langHolder GetTargetRoomsAt(int ind)
        {
            try
            {
                return langCollectionTarget[ind];
            }
            catch { }
            return new langHolder();
        }

        public bool? IsActiveInterpRoomsAt(int ind)
        {
            try
            {
                return langCollectionTransl[ind].isActive;
            }
            catch { }
            return null;

        }

        public bool? IsActiveTargetRoomsAt(int ind)
        {
            try
            {
                return langCollectionTarget[ind].isActive;
            }
            catch { }
            return null;

        }

        public void SetActiveInterpRoomsAt(int ind, bool state)
        {
            try
            {
                var tmp = langCollectionTransl[ind];
                tmp.isActive = state;
                langCollectionTransl[ind] = tmp;
            }
            catch { }
        }

        public void SetActiveTargetRoomsAt(int ind, bool state)
        {
            try
            {
                var tmp = langCollectionTarget[ind];
                tmp.isActive = state;
                langCollectionTarget[ind] = tmp;
            }
            catch { }
        }
    }
}
