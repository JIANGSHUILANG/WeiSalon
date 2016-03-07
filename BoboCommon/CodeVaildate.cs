using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboCommon
{
    public class CodeVaildate
    {
        #region 验证码



        private const double PI = 3.1415926535897932384626433832795;
        private const double PI2 = 6.283185307179586476925286766559;

        /// <summary>
        /// 创建验证码随机数
        /// </summary>
        /// <returns></returns>
        public string CreateVerifyCode()
        {
            string codeSerial = "2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,m,n,p,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] arr = codeSerial.Split(',');
            string code = "";
            int randValue = -1;
            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < 4; i++)
            {
                randValue = rand.Next(0, arr.Length - 1);
                code += arr[randValue];
            }
            return code;
        }

        /// <summary>
        /// 根据字符串创建图片
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Bitmap CreateImageCode(string code)
        {
            Color[] colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
            string[] fonts = { "Arial", "Georgia" };
            int fSize = 14;
            int fWidth = fSize + 1;
            int imageWidth = (int)(code.Length * fWidth) + 4 + 2 * 2;
            int imageHeight = fSize * 2 + 1;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(imageWidth, imageHeight);
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.White);
            Random rand = new Random();
            //给背景添加随机生成的燥点
            Pen pen = new Pen(Color.LightGray, 0);
            int c = 4 * 10;
            for (int i = 0; i < c; i++)
            {
                int x = rand.Next(image.Width);
                int y = rand.Next(image.Height);
                g.DrawRectangle(pen, x, y, 1, 1);
            }
            int left = 0, top = 0, top1 = 1, top2 = 1;
            int n1 = (imageHeight - 15 - 2 * 2);
            int n2 = n1 / 4;
            top1 = n2;
            top2 = n2 * 2;
            Font f;
            Brush b;
            int cindex, findex;
            //随机字体和颜色的验证码字符
            for (int i = 0; i < code.Length; i++)
            {
                cindex = rand.Next(colors.Length - 1);
                findex = rand.Next(fonts.Length - 1);
                f = new System.Drawing.Font(fonts[findex], fSize, System.Drawing.FontStyle.Bold);
                b = new System.Drawing.SolidBrush(colors[cindex]);
                if (i % 2 == 1)
                {
                    top = top2;
                }
                else
                {
                    top = top1;
                }
                left = i * fWidth;
                g.DrawString(code.Substring(i, 1), f, b, left, top);
            }
            //画一个边框 边框颜色为Color.Gainsboro
            g.DrawRectangle(new Pen(Color.Gainsboro, 0), 0, 0, image.Width - 1, image.Height - 1);
            g.Dispose();
            //产生波形
            image = TwistImage(image, true, 2, 4);
            return image;
        }

        /// <summary>
        /// 正弦曲线Wave扭曲图片
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="nMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        /// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>
        /// <returns></returns>
        public System.Drawing.Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            System.Drawing.Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            // 将位图背景填充为白色
            System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();
            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;
            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI2 * (double)j) / dBaseAxisLen : (PI2 * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);

                    // 取得当前点的颜色
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);

                    System.Drawing.Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                     && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }
            return destBmp;
        }

        /// <summary>
        /// 创建并输出验证码图片
        /// </summary>
        /// <param name="code"></param>
        public byte[] CreateImageOnPage(string code)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Bitmap image = this.CreateImageCode(code);
            try
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            finally
            {
                image.Dispose();
            }
            return ms.ToArray();
        }
        #endregion
    }
}
