using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text.RegularExpressions;
using System.Web.UI;

public partial class Preview : Page
{
    string contents = "";
    string fontFamily = "";
    string fontColor = "";
    int fontSize = 0;
    int fontSpace = 0;
    string posX = "";
    string posY = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        contents = Request["contents"];
        fontFamily = Request["fontFamily"];
        fontColor = Request["fontColor"];
        fontSize = (Request["fontSize"] == "default") ? 42 : Convert.ToInt32(Request["fontSize"]);
        fontSpace = (Request["fontSpace"] == "default") ? -20 : Convert.ToInt32(Request["fontSpace"]);
        posX = Request["posX"];
        posY = Request["posY"];

        try
        {
            MakePreviewImg();
        }
        catch (Exception)
        {
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "alert", "alert('파일 생성 오류! 입력값을 확인해 주세요.');", true);
        }
    }

    private void MakePreviewImg()
    {
        Image image = Image.FromFile(Server.MapPath("bg.jpg"));
        Font font = new Font(fontFamily, fontSize, FontStyle.Regular);
        SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml(fontColor));

        Graphics graphics = Graphics.FromImage(image);
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        string text = contents.Replace(" ", "|");
        string[] textParts = Regex.Split(text, @"\r\n?|\n");

        // 텍스트 시작 위치 구하기
        float x = 0;
        float y = 0;

        if (posX == "center")
        {
            float text_width = 0;

            foreach (string textPart in textParts)
            {
                foreach (char c in textPart)
                {
                    text_width += graphics.MeasureString(c.ToString(), font).Width + fontSpace;

                    // 약물, 숫자, 알파벳 예외처리
                    if (c == '“' || c == '”' || c == '‘' || c == '’' || c == '·' || c == '…' || c == '.' || c == ',' || Regex.IsMatch(Convert.ToString(c), "^[0-9a-zA-Z]*$"))
                        text_width += 8;
                }
            }

            if (text_width + 250 > image.Width)
                x = 220;
            else
                x = (image.Width - text_width) / 2 + 95;
        }
        else
        {
            x = Convert.ToSingle(posX);
        }

        if (posY == "center")
        {
            float text_height = 0;

            foreach (string textPart in textParts)
            {
                foreach (char c in textPart)
                {
                    text_height += graphics.MeasureString(c.ToString(), font).Height;
                    break;
                }
            }

            y = (image.Height - text_height) / 2 + 3;
        }
        else
        {
            y = Convert.ToSingle(posY);
        }

        // 텍스트 쓰기
        PointF point = new PointF(x, y);
        float indent = 0;

        foreach (string textPart in textParts)
        {
            for (int i = 0; i < textPart.Length; i++)
            {
                // 띄어쓰기
                if (textPart[i] == '|')
                    graphics.DrawString(" ", font, brush, new PointF(point.X + indent, point.Y));
                else
                    graphics.DrawString(textPart[i].ToString(), font, brush, new PointF(point.X + indent, point.Y));

                // 자간 
                if (i + 1 < textPart.Length)
                {
                    indent += graphics.MeasureString(textPart[i].ToString(), font).Width + fontSpace;

                    // 약물, 숫자, 알파벳 예외처리
                    if (textPart[i + 1] == '“' || textPart[i + 1] == '”' || textPart[i + 1] == '‘' || textPart[i + 1] == '’'
                        || textPart[i + 1] == '·' || textPart[i + 1] == '…' || textPart[i + 1] == '.' || textPart[i + 1] == ','
                        || textPart[i] == '“' || textPart[i] == '”' || textPart[i] == '‘' || textPart[i] == '’'
                        || textPart[i] == '·' || textPart[i] == '…' || textPart[i] == '.' || textPart[i] == ','
                        || Regex.IsMatch(Convert.ToString(textPart[i + 1]), "^[0-9a-zA-Z]*$") || Regex.IsMatch(Convert.ToString(textPart[i]), "^[0-9a-zA-Z]*$"))
                        indent += 4;
                }
            }
        }

        // 파일 저장하기
        // Get an ImageCodecInfo object that represents the JPEG codec.
        ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
        // for the Quality parameter category.
        Encoder myEncoder = Encoder.Quality;
        // Save the bitmap as a JPEG file with quality level 25.
        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
        // EncoderParameter object in the array.
        EncoderParameters myEncoderParameters = new EncoderParameters(1);
        myEncoderParameters.Param[0] = myEncoderParameter;

        image.Save(Server.MapPath("preview.jpg"), myImageCodecInfo, myEncoderParameters);
        //image.Save(Server.MapPath("preview.jpg"), ImageFormat.Jpeg);
    }

    private ImageCodecInfo GetEncoderInfo(string mimeType)
    {
        ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();

        for (int i = 0; i < info.Length; i++)
        {
            if (info[i].MimeType == mimeType)
                return info[i];
        }

        return null;
    }
}
