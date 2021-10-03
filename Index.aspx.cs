using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.UI;

public partial class Index : Page
{
    string SVR_DIR1 = ReadValue("FTP1", "IP");
    string SVR_ID1 = ReadValue("FTP1", "ID");
    string SVR_PW1 = ReadValue("FTP1", "PW");
    string SVR_DIR2 = ReadValue("FTP2", "IP");
    string SVR_ID2 = ReadValue("FTP2", "ID");
    string SVR_PW2 = ReadValue("FTP2", "PW");
    string DOWNLOAD_DIR1 = ReadValue("PATH", "DOWNLOAD_DIR1");
    string DOWNLOAD_DIR2 = ReadValue("PATH", "DOWNLOAD_DIR2");
    string UPLOAD_DIR = ReadValue("PATH", "UPLOAD_DIR");
    string LOG_DIR = ReadValue("PATH", "LOG_DIR");
    Regex regex_filename = new Regex(@"^속보[0-9][0-9]*.jpg$");
    static List<FileList> SvrFile1 = new List<FileList>();
    static List<FileList> SvrFile2 = new List<FileList>();
    static List<FileList> UploadFile = new List<FileList>();

    [DllImport("kernel32.dll")]
    public static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

    public static string ReadValue(string section, string key)
    {
        StringBuilder tmp = new StringBuilder(255);

        try
        {
            //GetPrivateProfileString(section, key, string.Empty, tmp, 255, @"D:\예은\Documents\Visual Studio 2015\Projects\NewsMgr\NewsMgr.ini");
            GetPrivateProfileString(section, key, string.Empty, tmp, 255, @"H:\edisplay-major-news-web\NewsMgr.ini");
            return tmp.ToString();
        }
        catch (Exception)
        {
            return "";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // 리디렉션
        string strRefer = Request.ServerVariables["HTTP_REFERER"];

        if (strRefer == null)
        {
            if (Session["empno"] == null)
                Page.RegisterClientScriptBlock("done", @"<script>alert('잘못된 방식으로 접근 중입니다! (1)');location.href='http://sis.seoul.co.kr/';</script>");
        }
        else if (strRefer.IndexOf("http://sis.seoul.co.kr/") == -1 && strRefer.IndexOf("http://appsvr3.seoul.co.kr/edisplay-major-news-web/") == -1)
        {
            // 조건 1) ".../SSO/go_edisplay_major_news.php"로 하면 Chrome 레퍼에서 못 가져옴 (IE에서는 가능)
            // 조건 2) 파일 전송 시 새로고침(Postback)되기 때문에 꼭 있어야 함             
            Page.RegisterClientScriptBlock("done", @"<script>alert('잘못된 방식으로 접근 중입니다! (2)');location.href='http://sis.seoul.co.kr/';</script>");
        }
        else if (Session["empno"] == null)
        {
            Session["empno"] = Request["userid"];
        }

        if (!IsPostBack)
        {
            if (!Directory.Exists(DOWNLOAD_DIR1))
                Directory.CreateDirectory(DOWNLOAD_DIR1);

            if (!Directory.Exists(DOWNLOAD_DIR2))
                Directory.CreateDirectory(DOWNLOAD_DIR2);

            if (!Directory.Exists(UPLOAD_DIR))
                Directory.CreateDirectory(UPLOAD_DIR);

            if (!Directory.Exists(LOG_DIR))
                Directory.CreateDirectory(LOG_DIR);

            FileError0.Visible = false;
            FileUploadAll.Enabled = false;

            SetSvrFileList();
        }
    }

    // 서버 파일 리스트
    private void SetSvrFileList()
    {
        // 서버1
        try
        {
            DownloadSvrFile(SVR_DIR1, SVR_ID1, SVR_PW1, DOWNLOAD_DIR1, SvrFile1);
            BindSvrFile(SVR_DIR1, SVR_ID1, SVR_PW1, DOWNLOAD_DIR1, SvrFile1, SvrFileRepeater1);

            FileError1.Visible = false;
            FileEmpty1.Visible = (SvrFile1.Count == 0) ? true : false;
            FileSync.Enabled = (SvrFile1.Count == 0 && SvrFile2.Count == 0) ? false : true;
        }
        catch (Exception ex)
        {
            FileError1.Visible = true;
            FileEmpty1.Visible = false;
            FileSync.Enabled = (SvrFile1.Count == 0 && SvrFile2.Count == 0) ? false : true;

            SvrFileRepeater1.DataSource = null;
            SvrFileRepeater1.DataBind();

            SaveLog("(서버1 오류) " + ex.ToString(), "");
        }

        // 서버2
        try
        {
            DownloadSvrFile(SVR_DIR2, SVR_ID2, SVR_PW2, DOWNLOAD_DIR2, SvrFile2);
            BindSvrFile(SVR_DIR2, SVR_ID2, SVR_PW2, DOWNLOAD_DIR2, SvrFile2, SvrFileRepeater2);

            FileError2.Visible = false;
            FileEmpty2.Visible = (SvrFile2.Count == 0) ? true : false;
            FileSync.Enabled = (SvrFile1.Count == 0 && SvrFile2.Count == 0) ? false : true;
        }
        catch (Exception ex)
        {
            FileError2.Visible = true;
            FileEmpty2.Visible = false;
            FileSync.Enabled = (SvrFile1.Count == 0 && SvrFile2.Count == 0) ? false : true;

            SvrFileRepeater2.DataSource = null;
            SvrFileRepeater2.DataBind();

            SaveLog("(서버2 오류) " + ex.ToString(), "");
        }

        FileRefresh.Text = "새로고침(" + DateTime.Now.ToString("HH:mm") + ")";
    }

    // 서버 파일 다운로드
    private void DownloadSvrFile(string svr_dir, string id, string pw, string download_dir, List<FileList> SvrFile)
    {
        SvrFile.Clear();

        foreach (FileInfo info in new DirectoryInfo(download_dir).GetFiles())
            info.Delete();

        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(svr_dir);
        request.Credentials = new NetworkCredential(id, pw);
        request.Method = WebRequestMethods.Ftp.ListDirectory;

        WebResponse response = request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);

        string filename = reader.ReadLine();

        while (filename != null)
        {
            if (regex_filename.IsMatch(filename.ToLower()))
            {
                using (WebClient client = new NoKeepAliveWebClient())
                {
                    client.Credentials = new NetworkCredential(id, pw);
                    client.DownloadFile(svr_dir + filename, download_dir + filename);
                }
            }

            filename = reader.ReadLine();
        }

        reader.Close();
        response.Close();
    }

    // 서버 파일 바인드
    private void BindSvrFile(string svr_dir, string id, string pw, string download_dir, List<FileList> SvrFile, Repeater repeater)
    {
        foreach (FileInfo info in new DirectoryInfo(download_dir).GetFiles())
        {
            if (regex_filename.IsMatch(info.Name.ToLower()))
            {
                SvrFile.Add(new FileList()
                {
                    name = info.Name,
                    time = GetDateFromSvrFile(svr_dir, id, pw, info.Name),
                    path = download_dir.Split('\\')[download_dir.Split('\\').Length - 2] + "\\" + info.Name
                });
            }
        }

        repeater.DataSource = SvrFile;
        repeater.DataBind();
    }

    // 서버 파일 시간
    private string GetDateFromSvrFile(string svr_dir, string id, string pw, string filename)
    {
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(svr_dir + filename);
        request.Credentials = new NetworkCredential(id, pw);
        request.Method = WebRequestMethods.Ftp.GetDateTimestamp;

        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
        {
            return response.LastModified.ToString();
        }
    }

    // 파일 선택
    protected void FileSelect_DispNone_Click(object sender, EventArgs e)
    {
        try
        {
            DownloadUploadFile();
            BindUploadFile();

            FileError0.Visible = false;
            FileEmpty0.Visible = (UploadFile.Count == 0) ? true : false;
            FileUploadAll.Enabled = (UploadFile.Count == 0) ? false : true;
        }
        catch (Exception ex)
        {
            FileError0.Visible = true;
            FileEmpty0.Visible = false;
            FileUploadAll.Enabled = false;

            UploadFileRepeater.DataSource = null;
            UploadFileRepeater.DataBind();

            SaveLog("(파일 선택 오류) " + ex.ToString(), "");
        }
    }

    // 전송 파일 다운로드
    private void DownloadUploadFile()
    {
        //foreach (FileInfo info in new DirectoryInfo(UPLOAD_DIR).GetFiles())
        //    info.Delete();

        for (int i = 0; i < Request.Files.Count; i++)
        {
            HttpPostedFile postedFile = Request.Files[i];
            string filename = Path.GetFileName(postedFile.FileName);

            if (regex_filename.IsMatch(filename.ToLower()))
                postedFile.SaveAs(UPLOAD_DIR + filename);
        }
    }

    // 전송 파일 바인드
    private void BindUploadFile()
    {
        UploadFile.Clear();

        foreach (FileInfo info in new DirectoryInfo(UPLOAD_DIR).GetFiles())
        {
            if (regex_filename.IsMatch(info.Name.ToLower()))
            {
                UploadFile.Add(new FileList()
                {
                    name = info.Name,
                    path = UPLOAD_DIR.Split('\\')[UPLOAD_DIR.Split('\\').Length - 2] + "\\" + info.Name,
                    state = "wait"
                });
            }
        }

        UploadFileRepeater.DataSource = UploadFile;
        UploadFileRepeater.DataBind();
    }

    // 전송 파일 취소
    protected void SvrFileDelete_Click(object sender, CommandEventArgs e)
    {
        try
        {
            File.Delete(UPLOAD_DIR + e.CommandArgument.ToString());
            UploadFile.RemoveAt(UploadFile.FindIndex(a => a.name == e.CommandArgument.ToString()));

            FileError0.Visible = false;
            FileEmpty0.Visible = (UploadFile.Count == 0) ? true : false;
            FileUploadAll.Enabled = (UploadFile.Count == 0) ? false : true;

            UploadFileRepeater.DataSource = UploadFile;
            UploadFileRepeater.DataBind();
        }
        catch (Exception ex)
        {
            SaveLog("(파일 취소 오류) " + ex.ToString(), "");
        }
    }

    // 전체 전송
    protected void FileUploadAll_Click(object sender, EventArgs e)
    {
        bool success = true;

        for (int i = 0; i < UploadFile.Count; i++)
        {
            // 서버1
            try
            {
                using (WebClient client = new NoKeepAliveWebClient())
                {
                    client.Credentials = new NetworkCredential(SVR_ID1, SVR_PW1);
                    client.UploadFile(SVR_DIR1 + UploadFile[i].name, UPLOAD_DIR + UploadFile[i].name);
                }

                UploadFile[i].state = "success";
                SaveLog("(업로드 성공) " + Session["empno"] + ", " + UploadFile[i].name, UPLOAD_DIR + UploadFile[i].name);
            }
            catch (Exception ex)
            {
                UploadFile[i].state = "fail";
                success = false;
                SaveLog("(업로드 실패) " + Session["empno"] + ", " + UploadFile[i].name + "\n" + ex, "");
            }

            // 서버2
            try
            {
                using (WebClient client = new NoKeepAliveWebClient())
                {
                    client.Credentials = new NetworkCredential(SVR_ID2, SVR_PW2);
                    client.UploadFile(SVR_DIR2 + UploadFile[i].name, UPLOAD_DIR + UploadFile[i].name);
                }

                UploadFile[i].state = (UploadFile[i].state == "fail") ? "fail" : "success";
                SaveLog("(업로드 성공) " + Session["empno"] + ", " + UploadFile[i].name, UPLOAD_DIR + UploadFile[i].name);
            }
            catch (Exception ex)
            {
                UploadFile[i].state = "fail";
                success = false;
                SaveLog("(업로드 실패) " + Session["empno"] + ", " + UploadFile[i].name + "\n" + ex, "");
            }
        }

        // 전송된 파일 삭제
        for (int i = 0; i < UploadFile.Count; i++)
        {
            if (UploadFile[i].state == "success")
            {
                File.Delete(UPLOAD_DIR + UploadFile[i].name);
                UploadFile.RemoveAt(i);
                i--;
            }
        }

        FileError0.Visible = false;
        FileEmpty0.Visible = (UploadFile.Count == 0) ? true : false;
        FileUploadAll.Enabled = (UploadFile.Count == 0) ? false : true;

        UploadFileRepeater.DataSource = UploadFile;
        UploadFileRepeater.DataBind();

        SetSvrFileList();

        if (success)
            Confirm("전송이 완료되었습니다.");
    }

    // 새로고침
    protected void FileRefresh_Click(object sender, EventArgs e)
    {
        SetSvrFileList();
    }

    // 동기화
    protected void FileSync_Click(object sender, EventArgs e)
    {
        bool success = true;

        // 서버1 → 서버2
        foreach (FileInfo info in new DirectoryInfo(DOWNLOAD_DIR1).GetFiles())
        {
            try
            {
                // 있으면
                if (File.Exists(DOWNLOAD_DIR2 + info.Name))
                {
                    // 시간 비교
                    DateTime time1 = DateTime.Parse(SvrFile1[SvrFile1.FindIndex(a => a.name == info.Name)].time);
                    DateTime time2 = DateTime.Parse(SvrFile2[SvrFile2.FindIndex(a => a.name == info.Name)].time);

                    // 큰 것(최신)으로 덮어쓰기
                    if (time1 >= time2)
                    {
                        using (WebClient client = new NoKeepAliveWebClient())
                        {
                            client.Credentials = new NetworkCredential(SVR_ID1, SVR_PW1);
                            client.UploadFile(SVR_DIR2 + info.Name, DOWNLOAD_DIR1 + info.Name);
                        }

                        SaveLog("(동기화 완료) " + Session["empno"] + ", " + info.Name, DOWNLOAD_DIR1 + info.Name);
                    }
                    else if (time1 < time2)
                    {
                        using (WebClient client = new NoKeepAliveWebClient())
                        {
                            client.Credentials = new NetworkCredential(SVR_ID2, SVR_PW2);
                            client.UploadFile(SVR_DIR1 + info.Name, DOWNLOAD_DIR2 + info.Name);
                        }

                        SaveLog("(동기화 완료) " + Session["empno"] + ", " + info.Name, DOWNLOAD_DIR2 + info.Name);
                    }
                }
                else
                {
                    // 복사
                    using (WebClient client = new NoKeepAliveWebClient())
                    {
                        client.Credentials = new NetworkCredential(SVR_ID1, SVR_PW1);
                        client.UploadFile(SVR_DIR2 + info.Name, DOWNLOAD_DIR1 + info.Name);
                    }

                    SaveLog("(동기화 완료) " + Session["empno"] + ", " + info.Name, DOWNLOAD_DIR1 + info.Name);
                }
            }
            catch (Exception ex)
            {
                success = false;
                SaveLog("(동기화 실패) " + Session["empno"] + ", " + info.Name + "\n" + ex, "");
            }
        }

        // 서버2 → 서버1
        foreach (FileInfo info in new DirectoryInfo(DOWNLOAD_DIR2).GetFiles())
        {
            try
            {
                // 없으면
                if (!File.Exists(DOWNLOAD_DIR1 + info.Name))
                {
                    // 복사
                    using (WebClient client = new NoKeepAliveWebClient())
                    {
                        client.Credentials = new NetworkCredential(SVR_ID2, SVR_PW2);
                        client.UploadFile(SVR_DIR1 + info.Name, DOWNLOAD_DIR2 + info.Name);
                    }

                    SaveLog("(동기화 완료) " + Session["empno"] + ", " + info.Name, DOWNLOAD_DIR2 + info.Name);
                }
            }
            catch (Exception ex)
            {
                success = false;
                SaveLog("(동기화 실패) " + Session["empno"] + ", " + info.Name + "\n" + ex, "");
            }
        }

        FileSync.Text = "동기화(" + DateTime.Now.ToString("HH:mm") + ")";

        if (success)
            Confirm("동기화가 완료되었습니다.");
        else
            Confirm("동기화 실패! 다시 시도해 주세요.");

        SetSvrFileList();
    }

    // 확인창
    private void Confirm(string msg)
    {
        ScriptManager.RegisterClientScriptBlock(this, GetType(), "alert", "alert('" + msg + "');", true);
    }

    // 로그
    private void SaveLog(string msg, string filepath)
    {
        FileStream fs = null;
        StreamWriter sw = null;

        try
        {
            fs = new FileStream(LOG_DIR + "NewsMgr_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Append);

            sw = new StreamWriter(fs);
            sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + msg);

            sw.Close();
            fs.Close();

            // 파일 저장
            if (filepath != "")
            {
                if (msg.Contains("업로드"))
                    File.Copy(filepath, LOG_DIR + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Session["empno"] + "_업로드_" + Path.GetFileName(filepath));
                else
                    File.Copy(filepath, LOG_DIR + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Session["empno"] + "_동기화_" + Path.GetFileName(filepath));
            }
        }
        catch (Exception)
        {
            if (sw != null)
                sw.Close();

            if (fs != null)
                fs.Close();
        }
    }

    // 파일 생성 완료
    protected void SendPreviewBtn_Click(object sender, EventArgs e)
    {
        try
        {
            //foreach (FileInfo info in new DirectoryInfo(UPLOAD_DIR).GetFiles())
            //    info.Delete();

            // 파일 이동
            if (FileName1.Checked)
                File.Copy(Server.MapPath("preview.jpg"), UPLOAD_DIR + FileName1.Value + ".jpg", true);
            else if (FileName2.Checked)
                File.Copy(Server.MapPath("preview.jpg"), UPLOAD_DIR + FileName2.Value + ".jpg", true);
            else if (FileName3.Checked)
                File.Copy(Server.MapPath("preview.jpg"), UPLOAD_DIR + FileName3.Value + ".jpg", true);
            else if (FileName4.Checked)
                File.Copy(Server.MapPath("preview.jpg"), UPLOAD_DIR + FileName4.Value + ".jpg", true);
            else if (FileName5.Checked)
                File.Copy(Server.MapPath("preview.jpg"), UPLOAD_DIR + FileName5.Value + ".jpg", true);
            else if (FileName6.Checked)
                File.Copy(Server.MapPath("preview.jpg"), UPLOAD_DIR + FileName6.Value + ".jpg", true);
            else if (FileName7.Checked)
                File.Copy(Server.MapPath("preview.jpg"), UPLOAD_DIR + FileName7.Value + ".jpg", true);
            else if (FileName8.Checked)
                File.Copy(Server.MapPath("preview.jpg"), UPLOAD_DIR + FileName8.Value + ".jpg", true);

            BindUploadFile();

            FileError0.Visible = false;
            FileEmpty0.Visible = (UploadFile.Count == 0) ? true : false;
            FileUploadAll.Enabled = (UploadFile.Count == 0) ? false : true;
        }
        catch (Exception ex)
        {
            FileError0.Visible = true;
            FileEmpty0.Visible = false;
            FileUploadAll.Enabled = false;

            UploadFileRepeater.DataSource = null;
            UploadFileRepeater.DataBind();

            SaveLog("(파일 생성 오류) " + ex.ToString(), "");
        }
    }
}
