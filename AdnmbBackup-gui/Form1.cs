using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace AdnmbBackup_gui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            if (!Directory.Exists("cache"))
                Directory.CreateDirectory("cache");
            if (!Directory.Exists("output"))
                Directory.CreateDirectory("output");
            if (!Directory.Exists("output\\po"))
                Directory.CreateDirectory("output\\po");
            if (!Directory.Exists("output\\all"))
                Directory.CreateDirectory("output\\all");
            if (!Directory.Exists("po"))
                Directory.CreateDirectory("po");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.coldthunder11.com/index.php/2020/03/19/%e5%a6%82%e4%bd%95%e8%8e%b7%e5%8f%96a%e5%b2%9b%e7%9a%84%e9%a5%bc%e5%b9%b2/");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string id = textBox1.Text;
            if (id == string.Empty) return;
            if (!File.Exists("cookie.txt"))
            {
                MessageBox.Show("请先放好小饼干");
                return;
            }
            string path = Path.Combine("cache", id + ".json");
            string po = Path.Combine("po", id + ".txt");
            if (File.Exists(po))
            {
                string poid = File.ReadAllText(po);
            }
            try
            {
                if (File.Exists(path))
                {
                    var joInCache = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));
                    var ReplyCountInCache = joInCache["ReplyCount"].Value<int>();
                    int pageCountInCache = ReplyCountInCache / 19;
                    if (ReplyCountInCache % pageCountInCache != 0) pageCountInCache++;
                    // remove the Replies in the last page to avoid duplication
                    // what should be mind is that the last page may not be full
                    JArray contentJA = (JArray)joInCache["Replies"];
                    for (int i = 0; i < contentJA.Count; i++)
                    {
                        if (i >= (pageCountInCache - 1) * 19)
                        {
                            contentJA.RemoveAt(i);
                            i--;
                        }
                    }
                    string url = "https://api.nmb.best/Api/thread";
                    var cookie = File.ReadAllText("cookie.txt");
                    CookieContainer cookieContainer = new CookieContainer();
                    cookieContainer.Add(new Cookie("userhash", cookie, "/", "api.nmb.best"));
                    HttpClientHandler handler = new HttpClientHandler() { UseCookies = true };
                    handler.CookieContainer = cookieContainer;
                    HttpClient http = new HttpClient(handler);
                    http.DefaultRequestHeaders.Add("Host", "api.nmb.best");
                    http.DefaultRequestHeaders.Add("Accept", "application/json");
                    http.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                    http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.0.0 Safari/537.36");
                    label4.Text = "正在更新总页数（获取第一页）";
                    var t = http.GetAsync(url + "?id=" + id + "&page=1");
                    t.Wait();
                    var result = t.Result;
                    var t2 = result.Content.ReadAsByteArrayAsync();
                    t2.Wait();
                    var bytes = t2.Result;
                    var str = ReadGzip(bytes);
                    var fpjson = JsonConvert.DeserializeObject<JObject>(str);
                    var replyCount = int.Parse(fpjson["ReplyCount"].ToString());
                    int pageCount = replyCount / 19;
                    if (replyCount % pageCount != 0) pageCount++;
                    for (int page = pageCountInCache; page <= pageCount; page++)
                    {
                        label4.Text = "第" + page + "页";
                        t = http.GetAsync(url + "?id=" + id + "&page=" + page);
                        t.Wait();
                        result = t.Result;
                        t2 = result.Content.ReadAsByteArrayAsync();
                        t2.Wait();
                        bytes = t2.Result;
                        str = ReadGzip(bytes);
                        var jo = JsonConvert.DeserializeObject<JObject>(str);
                        JArray ja = jo["Replies"].ToObject<JArray>();
                        foreach (var item in ja)
                        {
                            if (item["user_hash"].ToString() == "Tips") continue;
                            contentJA.Add(item);
                        }
                    }
                    label4.Text = "完成";
                    fpjson["Replies"].Replace(contentJA);
                    var fjsonstr = JsonConvert.SerializeObject(fpjson, Formatting.Indented);
                    File.WriteAllText(path, fjsonstr);
                }
                else
                {
                    string url = "https://api.nmb.best/Api/thread";
                    var cookie = File.ReadAllText("cookie.txt");
                    CookieContainer cookieContainer = new CookieContainer();
                    cookieContainer.Add(new Cookie("userhash", cookie, "/", "api.nmb.best"));
                    HttpClientHandler handler = new HttpClientHandler() { UseCookies = true };
                    handler.CookieContainer = cookieContainer;
                    HttpClient http = new HttpClient(handler);
                    http.DefaultRequestHeaders.Add("Host", "api.nmb.best");
                    http.DefaultRequestHeaders.Add("Accept", "application/json");
                    http.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                    http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.0.0 Safari/537.36");
                    label4.Text = "正在更新总页数（获取第一页）";
                    var t = http.GetAsync(url + "?id=" + id + "&page=1");
                    t.Wait();
                    var result = t.Result;
                    var t2 = result.Content.ReadAsByteArrayAsync();
                    t2.Wait();
                    var bytes = t2.Result;
                    var str = ReadGzip(bytes);
                    label4.Text = str;
                    var fpjson = JsonConvert.DeserializeObject<JObject>(str);
                    var replyCount = int.Parse(fpjson["ReplyCount"].ToString());
                    int pageCount = replyCount / 19;
                    if (replyCount % pageCount != 0) pageCount++;
                    JArray contentJA = fpjson["Replies"].ToObject<JArray>();
                    for (var page = 2; page <= pageCount; page++)
                    {
                        label4.Text = "正在获取第" + page + "页";
                        t = http.GetAsync(url + "?id=" + id + "&page=" + page);
                        t.Wait();
                        result = t.Result;
                        t2 = result.Content.ReadAsByteArrayAsync();
                        t2.Wait();
                        bytes = t2.Result;
                        str = ReadGzip(bytes);
                        var jo = JsonConvert.DeserializeObject<JObject>(str);
                        JArray ja = jo["Replies"].ToObject<JArray>();
                        var rpcount = ja.Count;
                        for (int j = 0; j < rpcount; j++)
                        {
                            if (ja[j]["user_hash"].ToString() == "Tips") continue;
                            contentJA.Add(ja[j]);
                        }
                    }
                    label4.Text = "完成";
                    fpjson["Replies"].Replace(contentJA);
                    var fjsonstr = JsonConvert.SerializeObject(fpjson, Formatting.Indented);
                    File.WriteAllText(path, fjsonstr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            ConvertToText(id);
            ConvertToTextPoOnly(id);
            ConvertToMarkdown(id);
            ConvertToMarkdownPoOnly(id);
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Ovler-Young/AdnmbBackup-gui");
        }
        static void ConvertToText(string id)
        {
            string path = Path.Combine("cache", id + ".json");
            var jo = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));
            var sb = new StringBuilder();
            sb.Append(jo["user_hash"].ToString()); sb.Append("  "); sb.Append(jo["now"].ToString());
            sb.Append("  No."); sb.Append(jo["id"].ToString()); sb.Append(Environment.NewLine);
            var savepath = Path.Combine("output", id + ".txt");
            if (jo["title"].ToString() != "无标题")
            {
                sb.Append("标题:"); sb.Append(jo["title"].ToString()); sb.Append(Environment.NewLine);
                savepath = Path.Combine("output", id + "_" + jo["title"].ToString() + ".txt");
            }
            sb.Append(ContentProcess(jo["content"].ToString())); sb.Append(Environment.NewLine);
            var ja = jo["Replies"].ToObject<JArray>();
            for (int i = 0; i < ja.Count; i++)
            {
                sb.Append("------------------------------------"); sb.Append(Environment.NewLine);
                sb.Append(ja[i]["user_hash"].ToString()); sb.Append("  "); sb.Append(ja[i]["now"].ToString());
                sb.Append("  No."); sb.Append(ja[i]["id"].ToString()); sb.Append(Environment.NewLine);
                sb.Append(ContentProcess(ja[i]["content"].ToString())); sb.Append(Environment.NewLine);
            }
            File.WriteAllText(savepath, sb.ToString(), System.Text.Encoding.GetEncoding("UTF-8"));
        }
        static void ConvertToTextPoOnly(string id)
        {
            string path = Path.Combine("cache", id + ".json");
            var po_path = path.Replace("cache", "po").Replace("json", "txt");
            var jo = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));
            var sb = new StringBuilder();
            sb.Append(jo["user_hash"].ToString()); sb.Append("  "); sb.Append(jo["now"].ToString());
            sb.Append("  No."); sb.Append(jo["id"].ToString()); sb.Append(Environment.NewLine);
            var savepath = Path.Combine("output", id + "_po_only.txt");
            if (jo["title"].ToString() != "无标题")
            {
                sb.Append("标题:"); sb.Append(jo["title"].ToString()); sb.Append(Environment.NewLine);
                savepath = Path.Combine("output", id + "_" + jo["title"].ToString() + "_po_only.txt");
            }
            sb.Append(ContentProcess(jo["content"].ToString())); sb.Append(Environment.NewLine);
            var ja = jo["Replies"].ToObject<JArray>();
            var poid = new HashSet<string>();
            poid.Add(jo["user_hash"].ToString());
            if (File.Exists(po_path) && File.ReadAllText(po_path) != "")
            {
                // read poid line by line
                var lines = File.ReadAllLines(po_path);
                foreach (var line in lines)
                {
                    poid.Add(line.Split(' ')[0]);
                }
            }
            for (int i = 0; i < ja.Count; i++)
            {
                if (poid.Contains(ja[i]["user_hash"].ToString()))
                {
                    sb.Append("------------------------------------"); sb.Append(Environment.NewLine);
                    sb.Append(ja[i]["user_hash"].ToString()); sb.Append("  "); sb.Append(ja[i]["now"].ToString());
                    sb.Append("No."); sb.Append(ja[i]["id"].ToString()); sb.Append(Environment.NewLine);
                    sb.Append(ContentProcess(ja[i]["content"].ToString())); sb.Append(Environment.NewLine);
                }
            }
            File.WriteAllText(savepath, sb.ToString(), System.Text.Encoding.GetEncoding("UTF-8"));
        }
        static void ConvertToMarkdown(string id)
        {
            string path = Path.Combine("cache", id + ".json");
            var po_path = path.Replace("cache", "po").Replace("json", "txt");
            var jo = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));
            var sb = new StringBuilder();
            var savepath = Path.Combine("output", id + ".md");
            if (jo["title"].ToString() != "无标题")
            {
                sb.Append("# "); sb.Append(jo["title"].ToString()); sb.Append(Environment.NewLine); sb.Append(Environment.NewLine);
                savepath = Path.Combine("output", id + "_" + jo["title"].ToString() + ".md");
            }
            else
            {
                sb.Append("# "); sb.Append(jo["id"].ToString()); sb.Append(Environment.NewLine); sb.Append(Environment.NewLine);
            }
            if (jo["name"].ToString() != "无名氏" && jo["name"].ToString() != "")
            {
                sb.Append("**"); sb.Append(jo["name"].ToString()); sb.Append("**"); sb.Append(Environment.NewLine);
            }
            sb.Append("No."); sb.Append(jo["id"].ToString()); sb.Append("  "); sb.Append(jo["user_hash"].ToString()); sb.Append("  "); sb.Append(jo["now"].ToString()); sb.Append(Environment.NewLine);
            if (jo["img"].ToString() != "")
            {
                sb.Append("![image](https://image.nmb.best/image/"); sb.Append(jo["img"].ToString()); sb.Append(jo["ext"].ToString()); sb.Append(")"); sb.Append(Environment.NewLine);
            }
            sb.Append(ContentProcess(jo["content"].ToString().Replace("<b>", "**").Replace("</b>", "**").Replace("<small>", "`").Replace("</small>", "`"))); sb.Append(Environment.NewLine);
            var ja = jo["Replies"].ToObject<JArray>();
            var poid = new HashSet<string>();
            poid.Add(jo["user_hash"].ToString());
            if (File.Exists(po_path) && File.ReadAllText(po_path) != "")
            {
                // read poid line by line
                var lines = File.ReadAllLines(po_path);
                foreach (var line in lines)
                {
                    poid.Add(line.Split(' ')[0]);
                }
            }
            for (int i = 0; i < ja.Count; i++)
            {
                if (poid.Contains(ja[i]["user_hash"].ToString()))
                {
                    if (ja[i]["title"].ToString() != "无标题")
                    {
                        sb.Append(Environment.NewLine); sb.Append("## "); sb.Append(ja[i]["title"].ToString()); sb.Append(Environment.NewLine); sb.Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append(Environment.NewLine); sb.Append("## No."); sb.Append(ja[i]["id"].ToString()); sb.Append(Environment.NewLine); sb.Append(Environment.NewLine);
                    }
                }
                else
                {
                    if (ja[i]["title"].ToString() != "无标题")
                    {
                        sb.Append(Environment.NewLine); sb.Append("### "); sb.Append(ja[i]["title"].ToString()); sb.Append(Environment.NewLine); sb.Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append(Environment.NewLine); sb.Append("### No."); sb.Append(ja[i]["id"].ToString()); sb.Append(Environment.NewLine); sb.Append(Environment.NewLine);
                    }
                }
                if (ja[i]["name"].ToString() != "无名氏" && ja[i]["name"].ToString() != "")
                {
                    sb.Append("**"); sb.Append(ja[i]["name"].ToString()); sb.Append("**"); sb.Append(Environment.NewLine);
                }
                sb.Append("No."); sb.Append(ja[i]["id"].ToString()); sb.Append("  "); sb.Append(ja[i]["user_hash"].ToString()); sb.Append("  "); sb.Append(ja[i]["now"].ToString()); sb.Append(Environment.NewLine);
                if (ja[i]["img"].ToString() != "")
                {
                    sb.Append("![image](https://image.nmb.best/image/"); sb.Append(ja[i]["img"].ToString()); sb.Append(ja[i]["ext"].ToString()); sb.Append(")"); sb.Append(Environment.NewLine);
                }
                sb.Append(ContentProcess(ja[i]["content"].ToString().Replace("<b>", "**").Replace("</b>", "**").Replace("<small>", "`").Replace("</small>", "`"))); sb.Append(Environment.NewLine);
            }
            File.WriteAllText(savepath, sb.ToString(), System.Text.Encoding.GetEncoding("UTF-8"));
        }
        static void ConvertToMarkdownPoOnly(string id)
        {
            string path = Path.Combine("cache", id + ".json");
            var po_path = path.Replace("cache", "po").Replace("json", "txt");
            var jo = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));
            var sb = new StringBuilder();
            var savepath = Path.Combine("output", id + "_po_only.md");
            if (jo["title"].ToString() != "无标题")
            {
                sb.Append(Environment.NewLine); sb.Append("# "); sb.Append(jo["title"].ToString()); sb.Append(Environment.NewLine); sb.Append(Environment.NewLine);
                savepath = Path.Combine("output", id + "_" + jo["title"].ToString() + "_po_only.md");
            }
            else
            {
                sb.Append(Environment.NewLine); sb.Append("# "); sb.Append(jo["id"].ToString()); sb.Append(Environment.NewLine); sb.Append(Environment.NewLine);
            }
            if (jo["name"].ToString() != "无名氏" && jo["name"].ToString() != "")
            {
                sb.Append("**"); sb.Append(jo["name"].ToString()); sb.Append("**"); sb.Append(Environment.NewLine);
            }
            sb.Append("No."); sb.Append(jo["id"].ToString()); sb.Append("  "); sb.Append(jo["user_hash"].ToString()); sb.Append("  "); sb.Append(jo["now"].ToString()); sb.Append(Environment.NewLine);
            if (jo["img"].ToString() != "")
            {
                sb.Append("![image](https://image.nmb.best/image/"); sb.Append(jo["img"].ToString()); sb.Append(jo["ext"].ToString()); sb.Append(")"); sb.Append(Environment.NewLine);
            }
            sb.Append(ContentProcess(jo["content"].ToString().Replace("<b>", "**").Replace("</b>", "**").Replace("<small>", "`").Replace("</small>", "`"))); sb.Append(Environment.NewLine);
            var ja = jo["Replies"].ToObject<JArray>();
            var poid = new HashSet<string>();
            poid.Add(jo["user_hash"].ToString());
            if (File.Exists(po_path) && File.ReadAllText(po_path) != "")
            {
                // read poid line by line
                var lines = File.ReadAllLines(po_path);
                foreach (var line in lines)
                {
                    poid.Add(line.Split(' ')[0]);
                }
            }
            for (int i = 0; i < ja.Count; i++)
            {
                if (poid.Contains(ja[i]["user_hash"].ToString()))
                {
                    if (ja[i]["title"].ToString() != "无标题")
                    {
                        sb.Append(Environment.NewLine); sb.Append("## "); sb.Append(ja[i]["title"].ToString()); sb.Append(Environment.NewLine); sb.Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append(Environment.NewLine); sb.Append("## No."); sb.Append(ja[i]["id"].ToString()); sb.Append(Environment.NewLine); sb.Append(Environment.NewLine);
                    }
                    if (ja[i]["name"].ToString() != "无名氏" && ja[i]["name"].ToString() != "")
                    {
                        sb.Append("**"); sb.Append(ja[i]["name"].ToString()); sb.Append("**"); sb.Append(Environment.NewLine);
                    }
                    sb.Append(ja[i]["user_hash"].ToString()); sb.Append("  "); sb.Append(ja[i]["now"].ToString());
                    sb.Append("  No."); sb.Append(ja[i]["id"].ToString()); sb.Append(Environment.NewLine);
                    if (ja[i]["img"].ToString() != "")
                    {
                        sb.Append("![image](https://image.nmb.best/image/"); sb.Append(ja[i]["img"].ToString()); sb.Append(ja[i]["ext"].ToString()); sb.Append(")"); sb.Append(Environment.NewLine);
                    }
                    sb.Append(ContentProcess(ja[i]["content"].ToString().Replace("<b>", "**").Replace("</b>", "**").Replace("<small>", "`").Replace("</small>", "`"))); sb.Append(Environment.NewLine);
                }
            }
            File.WriteAllText(savepath, sb.ToString(), System.Text.Encoding.GetEncoding("UTF-8"));
        }
        static string ContentProcess(string content)
        {
            return content.Replace("<font color=\"#789922\">&gt;&gt;", ">>").Replace("</font><br />", Environment.NewLine)
                .Replace("</font>", Environment.NewLine)
                .Replace("<br />\r\n", Environment.NewLine).Replace("<br />\n", Environment.NewLine);
        }
        static string ReadGzip(byte[] bytes)
        {
            string result = string.Empty;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (GZipStream decompressedStream = new GZipStream(ms, CompressionMode.Decompress))
                {
                    using (StreamReader sr = new StreamReader(decompressedStream, Encoding.UTF8))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }
            return result;
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            if (File.Exists("AutoBackupList.txt"))
            {
                if (!File.Exists("cookie.txt"))
                {
                    MessageBox.Show("请先放好小饼干");
                    return;
                }
                int errCount = 0;
                var cookie = File.ReadAllText("cookie.txt");
                var ids = File.ReadAllLines("AutoBackupList.txt");
                foreach (var id in ids)
                {
                    try
                    {
                        string path = Path.Combine("cache", id + ".json");
                        string po = Path.Combine("po", id + ".txt");
                        label2.Text = "正在备份：" + id;
                        if (File.Exists(po))
                        {
                            string poid = File.ReadAllText(po);
                        }
                        try
                        {
                            if (File.Exists(path))
                            {
                                var joInCache = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));
                                var ReplyCountInCache = joInCache["ReplyCount"].Value<int>();
                                int pageCountInCache = ReplyCountInCache / 19;
                                if (ReplyCountInCache % pageCountInCache != 0) pageCountInCache++;
                                // remove the Replies in the last page to avoid duplication
                                // what should be mind is that the last page may not be full
                                JArray contentJA = (JArray)joInCache["Replies"];
                                for (int i = 0; i < contentJA.Count; i++)
                                {
                                    if (i >= (pageCountInCache - 1) * 19)
                                    {
                                        contentJA.RemoveAt(i);
                                        i--;
                                    }
                                }
                                string url = "https://api.nmb.best/Api/thread";
                                CookieContainer cookieContainer = new CookieContainer();
                                cookieContainer.Add(new Cookie("userhash", cookie, "/", "api.nmb.best"));
                                HttpClientHandler handler = new HttpClientHandler() { UseCookies = true };
                                handler.CookieContainer = cookieContainer;
                                HttpClient http = new HttpClient(handler);
                                http.DefaultRequestHeaders.Add("Host", "api.nmb.best");
                                http.DefaultRequestHeaders.Add("Accept", "application/json");
                                http.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                                http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.0.0 Safari/537.36");
                                label4.Text = "正在更新总页数（获取第一页）";
                                var t = http.GetAsync(url + "?id=" + id + "&page=1");
                                t.Wait();
                                var result = t.Result;
                                var t2 = result.Content.ReadAsByteArrayAsync();
                                t2.Wait();
                                var bytes = t2.Result;
                                var str = ReadGzip(bytes);
                                var fpjson = JsonConvert.DeserializeObject<JObject>(str);
                                var replyCount = int.Parse(fpjson["ReplyCount"].ToString());
                                int pageCount = replyCount / 19;
                                if (replyCount % pageCount != 0) pageCount++;
                                label4.Text = "第" + pageCountInCache + "页";
                                for (int page = pageCountInCache; page <= pageCount; page++)
                                {
                                    t = http.GetAsync(url + "?id=" + id + "&page=" + page);
                                    t.Wait();
                                    result = t.Result;
                                    t2 = result.Content.ReadAsByteArrayAsync();
                                    t2.Wait();
                                    bytes = t2.Result;
                                    str = ReadGzip(bytes);
                                    var jo = JsonConvert.DeserializeObject<JObject>(str);
                                    JArray ja = jo["Replies"].ToObject<JArray>();
                                    foreach (var item in ja)
                                    {
                                        if (item["user_hash"].ToString() == "Tips") continue;
                                        contentJA.Add(item);
                                    }
                                    if (page % 10 == 0)
                                    {
                                        label4.Text = "第" + page + "页";
                                    }
                                }
                                label4.Text = "完成";
                                fpjson["Replies"].Replace(contentJA);
                                var fjsonstr = JsonConvert.SerializeObject(fpjson, Formatting.Indented);
                                File.WriteAllText(path, fjsonstr);
                            }
                            else
                            {
                                string url = "https://api.nmb.best/Api/thread";
                                CookieContainer cookieContainer = new CookieContainer();
                                cookieContainer.Add(new Cookie("userhash", cookie, "/", "api.nmb.best"));
                                HttpClientHandler handler = new HttpClientHandler() { UseCookies = true };
                                handler.CookieContainer = cookieContainer;
                                HttpClient http = new HttpClient(handler);
                                http.DefaultRequestHeaders.Add("Host", "api.nmb.best");
                                http.DefaultRequestHeaders.Add("Accept", "application/json");
                                http.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                                http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.0.0 Safari/537.36");
                                label4.Text = "正在更新总页数（获取第一页）";
                                var t = http.GetAsync(url + "?id=" + id + "&page=1");
                                t.Wait();
                                var result = t.Result;
                                var t2 = result.Content.ReadAsByteArrayAsync();
                                t2.Wait();
                                var bytes = t2.Result;
                                var str = ReadGzip(bytes);
                                label4.Text = str;
                                var fpjson = JsonConvert.DeserializeObject<JObject>(str);
                                var replyCount = int.Parse(fpjson["ReplyCount"].ToString());
                                int pageCount = replyCount / 19;
                                if (replyCount % pageCount != 0) pageCount++;
                                JArray contentJA = fpjson["Replies"].ToObject<JArray>();
                                for (var page = 2; page <= pageCount; page++)
                                {
                                    label4.Text = "正在获取第" + page + "页";
                                    t = http.GetAsync(url + "?id=" + id + "&page=" + page);
                                    t.Wait();
                                    result = t.Result;
                                    t2 = result.Content.ReadAsByteArrayAsync();
                                    t2.Wait();
                                    bytes = t2.Result;
                                    str = ReadGzip(bytes);
                                    var jo = JsonConvert.DeserializeObject<JObject>(str);
                                    JArray ja = jo["Replies"].ToObject<JArray>();
                                    var rpcount = ja.Count;
                                    for (int j = 0; j < rpcount; j++)
                                    {
                                        if (ja[j]["user_hash"].ToString() == "Tips") continue;
                                        contentJA.Add(ja[j]);
                                    }
                                }
                                label4.Text = "完成";
                                fpjson["Replies"].Replace(contentJA);
                                var fjsonstr = JsonConvert.SerializeObject(fpjson, Formatting.Indented);
                                File.WriteAllText(path, fjsonstr);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                        ConvertToText(id);
                        ConvertToTextPoOnly(id);
                        ConvertToMarkdown(id);
                        ConvertToMarkdownPoOnly(id);
                    }
                    catch
                    {
                        errCount++;
                    }
                }
                label4.Text = "已完成自动备份，有" + errCount + "个串的备份存在错误";
                label2.Text = "自动备份已完成，可在上方手动输入串号进行备份";
            }
        }
    }
}
