using Download.pojo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Download
{
    public partial class DownloadWin : Form
    {
        public DownloadWin()
        {
            InitializeComponent();
        }

        private delegate void MyDelegateUI();

        private void RequestHanderGridAdd_Click(object sender, EventArgs e)
        {
            int index = this.RequestHeaderGridView.Rows.Add();
            this.RequestHeaderGridView.Rows[index].Cells[0].Value = "";
        }

        private void DownloadWin_Load(object sender, EventArgs e)
        {
            RequestType.SelectedIndex = 0;//设置该下拉框默认选中第一项。
        }

        private void requestBtn_Click(object sender, EventArgs e)
        {
            String requestType = RequestType.Text;
            String serverUrl = serverUrlText.Text;
            String downloadSkus = downloadTextBox.Text;

            Dictionary<string, string> requestHanders = new Dictionary<string, string>();
            for (int i = 0; i < RequestHeaderGridView.Rows.Count; i++)
            {
                String type = null, name = null;
                if (RequestHeaderGridView.Rows[i].Cells["type"].Value != null)
                {
                    type = RequestHeaderGridView.Rows[i].Cells["type"].Value.ToString();
                }

                if (RequestHeaderGridView.Rows[i].Cells["name"].Value != null)
                {
                    name = RequestHeaderGridView.Rows[i].Cells["name"].Value.ToString();
                }

                if (!String.IsNullOrWhiteSpace(type) && !String.IsNullOrWhiteSpace(name))
                    requestHanders.Add(type, name);
            }


            Task.Factory.StartNew(() =>
            {

                MyDelegateUI requestBtnDelegate = delegate
                {
                    this.requestBtn.Enabled = false;
                };
                this.requestBtn.Invoke(requestBtnDelegate);

                if (String.IsNullOrWhiteSpace(downloadSkus) && String.IsNullOrWhiteSpace(serverUrl))
                {
                    showTips("服务器地址或者文件路径必填一个");
                    return;
                }

                List<Data> sArray = new List<Data>();

                if (!String.IsNullOrWhiteSpace(downloadSkus))
                {

                    string[] tmpArray = downloadSkus.Split(new char[4] { '\r', '\n', ',', '，' });

                    for (int i = 0; i < tmpArray.Length; i++)
                    {
                        if (!String.IsNullOrWhiteSpace(tmpArray[i]))
                        {
                            string tmpData = tmpArray[i].Replace("\\", "");
                            tmpData = tmpData.Replace("/", "");
                            tmpData = tmpData.Replace(":", "");
                            tmpData = tmpData.Replace(".", "");
                            Data data = new Data();
                            data.Datas1 = tmpData;
                            if (!String.IsNullOrWhiteSpace(serverUrl))
                            {
                                data.Url1 = string.Format(serverUrl, tmpArray[i]);
                            }
                            else
                            {
                                data.Url1 = tmpArray[i];
                            }


                            sArray.Add(data);
                        }

                    }

                }
                else
                {
                    Data data = new Data();
                    string tmpData = serverUrl.Replace("\\", "");
                    tmpData = tmpData.Replace("/", "");
                    tmpData = tmpData.Replace(":", "");
                    tmpData = tmpData.Replace(".", "");

                    data.Datas1 = serverUrl;
                    data.Url1 = serverUrl;

                    sArray.Add(data);
                }


                MyDelegateUI downloadProgressBarDelegate = delegate
                {
                    downloadProgressBar.Visible = true;
                    downloadProgressBar.Minimum = 1;
                    downloadProgressBar.Maximum = sArray.Count;
                    downloadProgressBar.Value = 1;
                    downloadProgressBar.Step = 1;
                };
                this.downloadProgressBar.Invoke(downloadProgressBarDelegate);

                String downloadPath = downloadImagePath();
                int index = 0;
                bool success = false;
                foreach (var downloadUrl in sArray)
                {
                    if (downloadUrl != null)
                    {

                        MyDelegateUI downloadProgressBarDelegate2 = delegate
                        {
                            scheduleLabel1.Text = String.Concat("(", ++index, "/", sArray.Count, ")");
                            downloadProgressBar.PerformStep();
                        };
                        this.downloadProgressBar.Invoke(downloadProgressBarDelegate2);


                        success = SavePhotoFromUrl(downloadPath, downloadUrl.Datas1, downloadUrl.Url1, requestType, requestHanders);
                    }
                }


                if (success)
                {
                    MessageBox.Show("下载成功！存储路径：" + downloadPath,
                                       "提示",
                                       MessageBoxButtons.OKCancel,
                                      MessageBoxIcon.Information,
                                     MessageBoxDefaultButton.Button2
                                    );
                }
                MyDelegateUI requestBtnDelegate2 = delegate
                {
                    this.requestBtn.Enabled = true;
                };
                this.requestBtn.Invoke(requestBtnDelegate2);
            });

        }

        /// <summary>
        /// messageBox展示异常信息
        /// </summary>
        /// <param name="tip"></param>
        private void showTips(String tip)
        {
            MessageBox.Show(tip, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
            MyDelegateUI requestBtnDelegate2 = delegate
            {
                this.requestBtn.Enabled = true;
            };
            this.requestBtn.Invoke(requestBtnDelegate2);
        }
        /// <summary>
        /// 获取程序运行 对应系统桌面路径，并生成当前时间为名称的文件夹
        /// </summary>
        /// <returns></returns>
        private String downloadImagePath()
        {
            String deskPath = null;
            deskPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            DateTime dateTime = DateTime.Now;
            deskPath = String.Concat(deskPath, "\\" + (dateTime.ToFileTime().ToString()));
            return deskPath;
        }

        /// <summary>
        /// 从图片地址下载图片到本地磁盘
        /// </summary>
        /// <param name="ToLocalPath">图片本地磁盘地址</param>
        /// <param name="Url">图片网址</param>
        /// <returns></returns>
        public static bool SavePhotoFromUrl(string FilePath, string FileName, string Url, string postMethod, Dictionary<string, string> requestHanders)
        {
            bool Value = false;
            WebResponse response = null;
            Stream stream = null;
            if (!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);

            try
            {
                HttpWebRequest request = null;

                if (Url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    System.Net.ServicePointManager.CheckCertificateRevocationList = false;
                    System.Net.ServicePointManager.DefaultConnectionLimit = 512;
                    System.Net.ServicePointManager.Expect100Continue = false;
                    request = WebRequest.Create(Url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version10;

                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(Url);
                }


                request.Method = postMethod;

                if (requestHanders != null)
                {
                    foreach (KeyValuePair<string, string> kvp in requestHanders)
                    {
                        request.Headers.Add(kvp.Key, kvp.Value);

                    }
                }


                response = request.GetResponse();
                stream = response.GetResponseStream();

                Value = SaveBinaryFile(response, FilePath + "\\" + FileName + getContentType(response.ContentType));
                return Value;
            }
            catch (Exception err)
            {
                string aa = err.ToString();

                MessageBox.Show(aa, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                return false;
            }

        }
        /// <summary>
        /// Save a binary file to disk.
        /// </summary>
        /// <param name="response">The response used to save the file</param>
        // 将二进制文件保存到磁盘
        private static bool SaveBinaryFile(WebResponse response, string FileName)
        {
            bool Value = true;
            byte[] buffer = new byte[1024];

            try
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
                Stream outStream = System.IO.File.Create(FileName);
                Stream inStream = response.GetResponseStream();

                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);

                outStream.Close();
                inStream.Close();
            }
            catch
            {
                Value = false;
            }
            return Value;
        }


        public static String getContentType(String type)
        {
            Dictionary<string, string> contentType = new Dictionary<string, string>();

            contentType.Add(".load", "text/html");
            contentType.Add(".123", "application/vnd.lotus-1-2-3");
            contentType.Add(".3ds", "image/x-3ds");
            contentType.Add(".3g2", "video/3gpp");
            contentType.Add(".3ga", "video/3gpp");
            contentType.Add(".3gp", "video/3gpp");
            contentType.Add(".3gpp", "video/3gpp");
            contentType.Add(".602", "application/x-t602");
            contentType.Add(".669", "audio/x-mod");
            contentType.Add(".7z", "application/x-7z-compressed");
            contentType.Add(".a", "application/x-archive");
            contentType.Add(".aac", "audio/mp4");
            contentType.Add(".abw", "application/x-abiword");
            contentType.Add(".abw.crashed", "application/x-abiword");
            contentType.Add(".abw.gz", "application/x-abiword");
            contentType.Add(".ac3", "audio/ac3");
            contentType.Add(".ace", "application/x-ace");
            contentType.Add(".adb", "text/x-adasrc");
            contentType.Add(".ads", "text/x-adasrc");
            contentType.Add(".afm", "application/x-font-afm");
            contentType.Add(".ag", "image/x-applix-graphics");
            contentType.Add(".ai", "application/illustrator");
            contentType.Add(".aif", "audio/x-aiff");
            contentType.Add(".aifc", "audio/x-aiff");
            contentType.Add(".aiff", "audio/x-aiff");
            contentType.Add(".al", "application/x-perl");
            contentType.Add(".alz", "application/x-alz");
            contentType.Add(".amr", "audio/amr");
            contentType.Add(".ani", "application/x-navi-animation");
            contentType.Add(".anim[1-9j]", "video/x-anim");
            contentType.Add(".anx", "application/annodex");
            contentType.Add(".ape", "audio/x-ape");
            contentType.Add(".arj", "application/x-arj");
            contentType.Add(".arw", "image/x-sony-arw");
            contentType.Add(".as", "application/x-applix-spreadsheet");
            contentType.Add(".asc", "text/plain");
            contentType.Add(".asf", "video/x-ms-asf");
            contentType.Add(".asp", "application/x-asp");
            contentType.Add(".ass", "text/x-ssa");
            contentType.Add(".asx", "audio/x-ms-asx");
            contentType.Add(".atom", "application/atom+xml");
            contentType.Add(".au", "audio/basic");
            contentType.Add(".avi", "video/x-msvideo");
            contentType.Add(".aw", "application/x-applix-word");
            contentType.Add(".awb", "audio/amr-wb");
            contentType.Add(".awk", "application/x-awk");
            contentType.Add(".axa", "audio/annodex");
            contentType.Add(".axv", "video/annodex");
            contentType.Add(".bak", "application/x-trash");
            contentType.Add(".bcpio", "application/x-bcpio");
            contentType.Add(".bdf", "application/x-font-bdf");
            contentType.Add(".bib", "text/x-bibtex");
            contentType.Add(".bin", "application/octet-stream");
            contentType.Add(".blend", "application/x-blender");
            contentType.Add(".blender", "application/x-blender");
            contentType.Add(".bmp", "image/bmp");
            contentType.Add(".bz", "application/x-bzip");
            contentType.Add(".bz2", "application/x-bzip");
            contentType.Add(".c", "text/x-csrc");
            contentType.Add(".c++", "text/x-c++src");
            contentType.Add(".cab", "application/vnd.ms-cab-compressed");
            contentType.Add(".cb7", "application/x-cb7");
            contentType.Add(".cbr", "application/x-cbr");
            contentType.Add(".cbt", "application/x-cbt");
            contentType.Add(".cbz", "application/x-cbz");
            contentType.Add(".cc", "text/x-c++src");
            contentType.Add(".cdf", "application/x-netcdf");
            contentType.Add(".cdr", "application/vnd.corel-draw");
            contentType.Add(".cer", "application/x-x509-ca-cert");
            contentType.Add(".cert", "application/x-x509-ca-cert");
            contentType.Add(".cgm", "image/cgm");
            contentType.Add(".chm", "application/x-chm");
            contentType.Add(".chrt", "application/x-kchart");
            contentType.Add(".class", "application/x-java");
            contentType.Add(".cls", "text/x-tex");
            contentType.Add(".cmake", "text/x-cmake");
            contentType.Add(".cpio", "application/x-cpio");
            contentType.Add(".cpio.gz", "application/x-cpio-compressed");
            contentType.Add(".cpp", "text/x-c++src");
            contentType.Add(".cr2", "image/x-canon-cr2");
            contentType.Add(".crt", "application/x-x509-ca-cert");
            contentType.Add(".crw", "image/x-canon-crw");
            contentType.Add(".cs", "text/x-csharp");
            contentType.Add(".csh", "application/x-csh");
            contentType.Add(".css", "text/css");
            contentType.Add(".cssl", "text/css");
            contentType.Add(".csv", "text/csv");
            contentType.Add(".cue", "application/x-cue");
            contentType.Add(".cur", "image/x-win-bitmap");
            contentType.Add(".cxx", "text/x-c++src");
            contentType.Add(".d", "text/x-dsrc");
            contentType.Add(".dar", "application/x-dar");
            contentType.Add(".dbf", "application/x-dbf");
            contentType.Add(".dc", "application/x-dc-rom");
            contentType.Add(".dcl", "text/x-dcl");
            contentType.Add(".dcm", "application/dicom");
            contentType.Add(".dcr", "image/x-kodak-dcr");
            contentType.Add(".dds", "image/x-dds");
            contentType.Add(".deb", "application/x-deb");
            contentType.Add(".der", "application/x-x509-ca-cert");
            contentType.Add(".desktop", "application/x-desktop");
            contentType.Add(".dia", "application/x-dia-diagram");
            contentType.Add(".diff", "text/x-patch");
            contentType.Add(".divx", "video/x-msvideo");
            contentType.Add(".djv", "image/vnd.djvu");
            contentType.Add(".djvu", "image/vnd.djvu");
            contentType.Add(".dng", "image/x-adobe-dng");
            contentType.Add(".doc", "application/msword");
            contentType.Add(".docbook", "application/docbook+xml");
            contentType.Add(".docm", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            contentType.Add(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            contentType.Add(".dot", "text/vnd.graphviz");
            contentType.Add(".dsl", "text/x-dsl");
            contentType.Add(".dtd", "application/xml-dtd");
            contentType.Add(".dtx", "text/x-tex");
            contentType.Add(".dv", "video/dv");
            contentType.Add(".dvi", "application/x-dvi");
            contentType.Add(".dvi.bz2", "application/x-bzdvi");
            contentType.Add(".dvi.gz", "application/x-gzdvi");
            contentType.Add(".dwg", "image/vnd.dwg");
            contentType.Add(".dxf", "image/vnd.dxf");
            contentType.Add(".e", "text/x-eiffel");
            contentType.Add(".egon", "application/x-egon");
            contentType.Add(".eif", "text/x-eiffel");
            contentType.Add(".el", "text/x-emacs-lisp");
            contentType.Add(".emf", "image/x-emf");
            contentType.Add(".emp", "application/vnd.emusic-emusic_package");
            contentType.Add(".ent", "application/xml-external-parsed-entity");
            contentType.Add(".eps", "image/x-eps");
            contentType.Add(".eps.bz2", "image/x-bzeps");
            contentType.Add(".eps.gz", "image/x-gzeps");
            contentType.Add(".epsf", "image/x-eps");
            contentType.Add(".epsf.bz2", "image/x-bzeps");
            contentType.Add(".epsf.gz", "image/x-gzeps");
            contentType.Add(".epsi", "image/x-eps");
            contentType.Add(".epsi.bz2", "image/x-bzeps");
            contentType.Add(".epsi.gz", "image/x-gzeps");
            contentType.Add(".epub", "application/epub+zip");
            contentType.Add(".erl", "text/x-erlang");
            contentType.Add(".es", "application/ecmascript");
            contentType.Add(".etheme", "application/x-e-theme");
            contentType.Add(".etx", "text/x-setext");
            contentType.Add(".exe", "application/x-ms-dos-executable");
            contentType.Add(".exr", "image/x-exr");
            contentType.Add(".ez", "application/andrew-inset");
            contentType.Add(".f", "text/x-fortran");
            contentType.Add(".f90", "text/x-fortran");
            contentType.Add(".f95", "text/x-fortran");
            contentType.Add(".fb2", "application/x-fictionbook+xml");
            contentType.Add(".fig", "image/x-xfig");
            contentType.Add(".fits", "image/fits");
            contentType.Add(".fl", "application/x-fluid");
            contentType.Add(".flac", "audio/x-flac");
            contentType.Add(".flc", "video/x-flic");
            contentType.Add(".fli", "video/x-flic");
            contentType.Add(".flv", "video/x-flv");
            contentType.Add(".flw", "application/x-kivio");
            contentType.Add(".fo", "text/x-xslfo");
            contentType.Add(".for", "text/x-fortran");
            contentType.Add(".g3", "image/fax-g3");
            contentType.Add(".gb", "application/x-gameboy-rom");
            contentType.Add(".gba", "application/x-gba-rom");
            contentType.Add(".gcrd", "text/directory");
            contentType.Add(".ged", "application/x-gedcom");
            contentType.Add(".gedcom", "application/x-gedcom");
            contentType.Add(".gen", "application/x-genesis-rom");
            contentType.Add(".gf", "application/x-tex-gf");
            contentType.Add(".gg", "application/x-sms-rom");
            contentType.Add(".gif", "image/gif");
            contentType.Add(".glade", "application/x-glade");
            contentType.Add(".gmo", "application/x-gettext-translation");
            contentType.Add(".gnc", "application/x-gnucash");
            contentType.Add(".gnd", "application/gnunet-directory");
            contentType.Add(".gnucash", "application/x-gnucash");
            contentType.Add(".gnumeric", "application/x-gnumeric");
            contentType.Add(".gnuplot", "application/x-gnuplot");
            contentType.Add(".gp", "application/x-gnuplot");
            contentType.Add(".gpg", "application/pgp-encrypted");
            contentType.Add(".gplt", "application/x-gnuplot");
            contentType.Add(".gra", "application/x-graphite");
            contentType.Add(".gsf", "application/x-font-type1");
            contentType.Add(".gsm", "audio/x-gsm");
            contentType.Add(".gtar", "application/x-tar");
            contentType.Add(".gv", "text/vnd.graphviz");
            contentType.Add(".gvp", "text/x-google-video-pointer");
            contentType.Add(".gz", "application/x-gzip");
            contentType.Add(".h", "text/x-chdr");
            contentType.Add(".h++", "text/x-c++hdr");
            contentType.Add(".hdf", "application/x-hdf");
            contentType.Add(".hh", "text/x-c++hdr");
            contentType.Add(".hp", "text/x-c++hdr");
            contentType.Add(".hpgl", "application/vnd.hp-hpgl");
            contentType.Add(".hpp", "text/x-c++hdr");
            contentType.Add(".hs", "text/x-haskell");
            contentType.Add(".htm", "text/html");
            contentType.Add(".html", "text/html");
            contentType.Add(".hwp", "application/x-hwp");
            contentType.Add(".hwt", "application/x-hwt");
            contentType.Add(".hxx", "text/x-c++hdr");
            contentType.Add(".ica", "application/x-ica");
            contentType.Add(".icb", "image/x-tga");
            contentType.Add(".icns", "image/x-icns");
            contentType.Add(".ico", "image/vnd.microsoft.icon");
            contentType.Add(".ics", "text/calendar");
            contentType.Add(".idl", "text/x-idl");
            contentType.Add(".ief", "image/ief");
            contentType.Add(".iff", "image/x-iff");
            contentType.Add(".ilbm", "image/x-ilbm");
            contentType.Add(".ime", "text/x-imelody");
            contentType.Add(".imy", "text/x-imelody");
            contentType.Add(".ins", "text/x-tex");
            contentType.Add(".iptables", "text/x-iptables");
            contentType.Add(".iso", "application/x-cd-image");
            contentType.Add(".iso9660", "application/x-cd-image");
            contentType.Add(".it", "audio/x-it");
            contentType.Add(".j2k", "image/jp2");
            contentType.Add(".jad", "text/vnd.sun.j2me.app-descriptor");
            contentType.Add(".jar", "application/x-java-archive");
            contentType.Add(".java", "text/x-java");
            contentType.Add(".jng", "image/x-jng");
            contentType.Add(".jnlp", "application/x-java-jnlp-file");
            contentType.Add(".jp2", "image/jp2");
            contentType.Add(".jpc", "image/jp2");
            contentType.Add(".jpe", "image/jpeg");
            contentType.Add(".jpeg", "image/jpeg");
            contentType.Add(".jpf", "image/jp2");
            contentType.Add(".jpg", "image/jpeg");
            contentType.Add(".jpr", "application/x-jbuilder-project");
            contentType.Add(".jpx", "image/jp2");
            contentType.Add(".js", "application/javascript");
            contentType.Add(".json", "application/json");
            contentType.Add(".jsonp", "application/jsonp");
            contentType.Add(".k25", "image/x-kodak-k25");
            contentType.Add(".kar", "audio/midi");
            contentType.Add(".karbon", "application/x-karbon");
            contentType.Add(".kdc", "image/x-kodak-kdc");
            contentType.Add(".kdelnk", "application/x-desktop");
            contentType.Add(".kexi", "application/x-kexiproject-sqlite3");
            contentType.Add(".kexic", "application/x-kexi-connectiondata");
            contentType.Add(".kexis", "application/x-kexiproject-shortcut");
            contentType.Add(".kfo", "application/x-kformula");
            contentType.Add(".kil", "application/x-killustrator");
            contentType.Add(".kino", "application/smil");
            contentType.Add(".kml", "application/vnd.google-earth.kml+xml");
            contentType.Add(".kmz", "application/vnd.google-earth.kmz");
            contentType.Add(".kon", "application/x-kontour");
            contentType.Add(".kpm", "application/x-kpovmodeler");
            contentType.Add(".kpr", "application/x-kpresenter");
            contentType.Add(".kpt", "application/x-kpresenter");
            contentType.Add(".kra", "application/x-krita");
            contentType.Add(".ksp", "application/x-kspread");
            contentType.Add(".kud", "application/x-kugar");
            contentType.Add(".kwd", "application/x-kword");
            contentType.Add(".kwt", "application/x-kword");
            contentType.Add(".la", "application/x-shared-library-la");
            contentType.Add(".latex", "text/x-tex");
            contentType.Add(".ldif", "text/x-ldif");
            contentType.Add(".lha", "application/x-lha");
            contentType.Add(".lhs", "text/x-literate-haskell");
            contentType.Add(".lhz", "application/x-lhz");
            contentType.Add(".log", "text/x-log");
            contentType.Add(".ltx", "text/x-tex");
            contentType.Add(".lua", "text/x-lua");
            contentType.Add(".lwo", "image/x-lwo");
            contentType.Add(".lwob", "image/x-lwo");
            contentType.Add(".lws", "image/x-lws");
            contentType.Add(".ly", "text/x-lilypond");
            contentType.Add(".lyx", "application/x-lyx");
            contentType.Add(".lz", "application/x-lzip");
            contentType.Add(".lzh", "application/x-lha");
            contentType.Add(".lzma", "application/x-lzma");
            contentType.Add(".lzo", "application/x-lzop");
            contentType.Add(".m", "text/x-matlab");
            contentType.Add(".m15", "audio/x-mod");
            contentType.Add(".m2t", "video/mpeg");
            contentType.Add(".m3u", "audio/x-mpegurl");
            contentType.Add(".m3u8", "audio/x-mpegurl");
            contentType.Add(".m4", "application/x-m4");
            contentType.Add(".m4a", "audio/mp4");
            contentType.Add(".m4b", "audio/x-m4b");
            contentType.Add(".m4v", "video/mp4");
            contentType.Add(".mab", "application/x-markaby");
            contentType.Add(".man", "application/x-troff-man");
            contentType.Add(".mbox", "application/mbox");
            contentType.Add(".md", "application/x-genesis-rom");
            contentType.Add(".mdb", "application/vnd.ms-access");
            contentType.Add(".mdi", "image/vnd.ms-modi");
            contentType.Add(".me", "text/x-troff-me");
            contentType.Add(".med", "audio/x-mod");
            contentType.Add(".metalink", "application/metalink+xml");
            contentType.Add(".mgp", "application/x-magicpoint");
            contentType.Add(".mid", "audio/midi");
            contentType.Add(".midi", "audio/midi");
            contentType.Add(".mif", "application/x-mif");
            contentType.Add(".minipsf", "audio/x-minipsf");
            contentType.Add(".mka", "audio/x-matroska");
            contentType.Add(".mkv", "video/x-matroska");
            contentType.Add(".ml", "text/x-ocaml");
            contentType.Add(".mli", "text/x-ocaml");
            contentType.Add(".mm", "text/x-troff-mm");
            contentType.Add(".mmf", "application/x-smaf");
            contentType.Add(".mml", "text/mathml");
            contentType.Add(".mng", "video/x-mng");
            contentType.Add(".mo", "application/x-gettext-translation");
            contentType.Add(".mo3", "audio/x-mo3");
            contentType.Add(".moc", "text/x-moc");
            contentType.Add(".mod", "audio/x-mod");
            contentType.Add(".mof", "text/x-mof");
            contentType.Add(".moov", "video/quicktime");
            contentType.Add(".mov", "video/quicktime");
            contentType.Add(".movie", "video/x-sgi-movie");
            contentType.Add(".mp+", "audio/x-musepack");
            contentType.Add(".mp2", "video/mpeg");
            contentType.Add(".mp3", "audio/mpeg");
            contentType.Add(".mp4", "video/mp4");
            contentType.Add(".mpc", "audio/x-musepack");
            contentType.Add(".mpe", "video/mpeg");
            contentType.Add(".mpeg", "video/mpeg");
            contentType.Add(".mpg", "video/mpeg");
            contentType.Add(".mpga", "audio/mpeg");
            contentType.Add(".mpp", "audio/x-musepack");
            contentType.Add(".mrl", "text/x-mrml");
            contentType.Add(".mrml", "text/x-mrml");
            contentType.Add(".mrw", "image/x-minolta-mrw");
            contentType.Add(".ms", "text/x-troff-ms");
            contentType.Add(".msi", "application/x-msi");
            contentType.Add(".msod", "image/x-msod");
            contentType.Add(".msx", "application/x-msx-rom");
            contentType.Add(".mtm", "audio/x-mod");
            contentType.Add(".mup", "text/x-mup");
            contentType.Add(".mxf", "application/mxf");
            contentType.Add(".n64", "application/x-n64-rom");
            contentType.Add(".nb", "application/mathematica");
            contentType.Add(".nc", "application/x-netcdf");
            contentType.Add(".nds", "application/x-nintendo-ds-rom");
            contentType.Add(".nef", "image/x-nikon-nef");
            contentType.Add(".nes", "application/x-nes-rom");
            contentType.Add(".nfo", "text/x-nfo");
            contentType.Add(".not", "text/x-mup");
            contentType.Add(".nsc", "application/x-netshow-channel");
            contentType.Add(".nsv", "video/x-nsv");
            contentType.Add(".o", "application/x-object");
            contentType.Add(".obj", "application/x-tgif");
            contentType.Add(".ocl", "text/x-ocl");
            contentType.Add(".oda", "application/oda");
            contentType.Add(".odb", "application/vnd.oasis.opendocument.database");
            contentType.Add(".odc", "application/vnd.oasis.opendocument.chart");
            contentType.Add(".odf", "application/vnd.oasis.opendocument.formula");
            contentType.Add(".odg", "application/vnd.oasis.opendocument.graphics");
            contentType.Add(".odi", "application/vnd.oasis.opendocument.image");
            contentType.Add(".odm", "application/vnd.oasis.opendocument.text-master");
            contentType.Add(".odp", "application/vnd.oasis.opendocument.presentation");
            contentType.Add(".ods", "application/vnd.oasis.opendocument.spreadsheet");
            contentType.Add(".odt", "application/vnd.oasis.opendocument.text");
            contentType.Add(".oga", "audio/ogg");
            contentType.Add(".ogg", "video/x-theora+ogg");
            contentType.Add(".ogm", "video/x-ogm+ogg");
            contentType.Add(".ogv", "video/ogg");
            contentType.Add(".ogx", "application/ogg");
            contentType.Add(".old", "application/x-trash");
            contentType.Add(".oleo", "application/x-oleo");
            contentType.Add(".opml", "text/x-opml+xml");
            contentType.Add(".ora", "image/openraster");
            contentType.Add(".orf", "image/x-olympus-orf");
            contentType.Add(".otc", "application/vnd.oasis.opendocument.chart-template");
            contentType.Add(".otf", "application/x-font-otf");
            contentType.Add(".otg", "application/vnd.oasis.opendocument.graphics-template");
            contentType.Add(".oth", "application/vnd.oasis.opendocument.text-web");
            contentType.Add(".otp", "application/vnd.oasis.opendocument.presentation-template");
            contentType.Add(".ots", "application/vnd.oasis.opendocument.spreadsheet-template");
            contentType.Add(".ott", "application/vnd.oasis.opendocument.text-template");
            contentType.Add(".owl", "application/rdf+xml");
            contentType.Add(".oxt", "application/vnd.openofficeorg.extension");
            contentType.Add(".p", "text/x-pascal");
            contentType.Add(".p10", "application/pkcs10");
            contentType.Add(".p12", "application/x-pkcs12");
            contentType.Add(".p7b", "application/x-pkcs7-certificates");
            contentType.Add(".p7s", "application/pkcs7-signature");
            contentType.Add(".pack", "application/x-java-pack200");
            contentType.Add(".pak", "application/x-pak");
            contentType.Add(".par2", "application/x-par2");
            contentType.Add(".pas", "text/x-pascal");
            contentType.Add(".patch", "text/x-patch");
            contentType.Add(".pbm", "image/x-portable-bitmap");
            contentType.Add(".pcd", "image/x-photo-cd");
            contentType.Add(".pcf", "application/x-cisco-vpn-settings");
            contentType.Add(".pcf.gz", "application/x-font-pcf");
            contentType.Add(".pcf.z", "application/x-font-pcf");
            contentType.Add(".pcl", "application/vnd.hp-pcl");
            contentType.Add(".pcx", "image/x-pcx");
            contentType.Add(".pdb", "chemical/x-pdb");
            contentType.Add(".pdc", "application/x-aportisdoc");
            contentType.Add(".pdf", "application/pdf");
            contentType.Add(".pdf.bz2", "application/x-bzpdf");
            contentType.Add(".pdf.gz", "application/x-gzpdf");
            contentType.Add(".pef", "image/x-pentax-pef");
            contentType.Add(".pem", "application/x-x509-ca-cert");
            contentType.Add(".perl", "application/x-perl");
            contentType.Add(".pfa", "application/x-font-type1");
            contentType.Add(".pfb", "application/x-font-type1");
            contentType.Add(".pfx", "application/x-pkcs12");
            contentType.Add(".pgm", "image/x-portable-graymap");
            contentType.Add(".pgn", "application/x-chess-pgn");
            contentType.Add(".pgp", "application/pgp-encrypted");
            contentType.Add(".php", "application/x-php");
            contentType.Add(".php3", "application/x-php");
            contentType.Add(".php4", "application/x-php");
            contentType.Add(".pict", "image/x-pict");
            contentType.Add(".pict1", "image/x-pict");
            contentType.Add(".pict2", "image/x-pict");
            contentType.Add(".pickle", "application/python-pickle");
            contentType.Add(".pk", "application/x-tex-pk");
            contentType.Add(".pkipath", "application/pkix-pkipath");
            contentType.Add(".pkr", "application/pgp-keys");
            contentType.Add(".pl", "application/x-perl");
            contentType.Add(".pla", "audio/x-iriver-pla");
            contentType.Add(".pln", "application/x-planperfect");
            contentType.Add(".pls", "audio/x-scpls");
            contentType.Add(".pm", "application/x-perl");
            contentType.Add(".png", "image/png");
            contentType.Add(".pnm", "image/x-portable-anymap");
            contentType.Add(".pntg", "image/x-macpaint");
            contentType.Add(".po", "text/x-gettext-translation");
            contentType.Add(".por", "application/x-spss-por");
            contentType.Add(".pot", "text/x-gettext-translation-template");
            contentType.Add(".ppm", "image/x-portable-pixmap");
            contentType.Add(".pps", "application/vnd.ms-powerpoint");
            contentType.Add(".ppt", "application/vnd.ms-powerpoint");
            contentType.Add(".pptm", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            contentType.Add(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            contentType.Add(".ppz", "application/vnd.ms-powerpoint");
            contentType.Add(".prc", "application/x-palm-database");
            contentType.Add(".ps", "application/postscript");
            contentType.Add(".ps.bz2", "application/x-bzpostscript");
            contentType.Add(".ps.gz", "application/x-gzpostscript");
            contentType.Add(".psd", "image/vnd.adobe.photoshop");
            contentType.Add(".psf", "audio/x-psf");
            contentType.Add(".psf.gz", "application/x-gz-font-linux-psf");
            contentType.Add(".psflib", "audio/x-psflib");
            contentType.Add(".psid", "audio/prs.sid");
            contentType.Add(".psw", "application/x-pocket-word");
            contentType.Add(".pw", "application/x-pw");
            contentType.Add(".py", "text/x-python");
            contentType.Add(".pyc", "application/x-python-bytecode");
            contentType.Add(".pyo", "application/x-python-bytecode");
            contentType.Add(".qif", "image/x-quicktime");
            contentType.Add(".qt", "video/quicktime");
            contentType.Add(".qtif", "image/x-quicktime");
            contentType.Add(".qtl", "application/x-quicktime-media-link");
            contentType.Add(".qtvr", "video/quicktime");
            contentType.Add(".ra", "audio/vnd.rn-realaudio");
            contentType.Add(".raf", "image/x-fuji-raf");
            contentType.Add(".ram", "application/ram");
            contentType.Add(".rar", "application/x-rar");
            contentType.Add(".ras", "image/x-cmu-raster");
            contentType.Add(".raw", "image/x-panasonic-raw");
            contentType.Add(".rax", "audio/vnd.rn-realaudio");
            contentType.Add(".rb", "application/x-ruby");
            contentType.Add(".rdf", "application/rdf+xml");
            contentType.Add(".rdfs", "application/rdf+xml");
            contentType.Add(".reg", "text/x-ms-regedit");
            contentType.Add(".rej", "application/x-reject");
            contentType.Add(".rgb", "image/x-rgb");
            contentType.Add(".rle", "image/rle");
            contentType.Add(".rm", "application/vnd.rn-realmedia");
            contentType.Add(".rmj", "application/vnd.rn-realmedia");
            contentType.Add(".rmm", "application/vnd.rn-realmedia");
            contentType.Add(".rms", "application/vnd.rn-realmedia");
            contentType.Add(".rmvb", "application/vnd.rn-realmedia");
            contentType.Add(".rmx", "application/vnd.rn-realmedia");
            contentType.Add(".roff", "text/troff");
            contentType.Add(".rp", "image/vnd.rn-realpix");
            contentType.Add(".rpm", "application/x-rpm");
            contentType.Add(".rss", "application/rss+xml");
            contentType.Add(".rt", "text/vnd.rn-realtext");
            contentType.Add(".rtf", "application/rtf");
            contentType.Add(".rtx", "text/richtext");
            contentType.Add(".rv", "video/vnd.rn-realvideo");
            contentType.Add(".rvx", "video/vnd.rn-realvideo");
            contentType.Add(".s3m", "audio/x-s3m");
            contentType.Add(".sam", "application/x-amipro");
            contentType.Add(".sami", "application/x-sami");
            contentType.Add(".sav", "application/x-spss-sav");
            contentType.Add(".scm", "text/x-scheme");
            contentType.Add(".sda", "application/vnd.stardivision.draw");
            contentType.Add(".sdc", "application/vnd.stardivision.calc");
            contentType.Add(".sdd", "application/vnd.stardivision.impress");
            contentType.Add(".sdp", "application/sdp");
            contentType.Add(".sds", "application/vnd.stardivision.chart");
            contentType.Add(".sdw", "application/vnd.stardivision.writer");
            contentType.Add(".sgf", "application/x-go-sgf");
            contentType.Add(".sgi", "image/x-sgi");
            contentType.Add(".sgl", "application/vnd.stardivision.writer");
            contentType.Add(".sgm", "text/sgml");
            contentType.Add(".sgml", "text/sgml");
            contentType.Add(".sh", "application/x-shellscript");
            contentType.Add(".shar", "application/x-shar");
            contentType.Add(".shn", "application/x-shorten");
            contentType.Add(".siag", "application/x-siag");
            contentType.Add(".sid", "audio/prs.sid");
            contentType.Add(".sik", "application/x-trash");
            contentType.Add(".sis", "application/vnd.symbian.install");
            contentType.Add(".sisx", "x-epoc/x-sisx-app");
            contentType.Add(".sit", "application/x-stuffit");
            contentType.Add(".siv", "application/sieve");
            contentType.Add(".sk", "image/x-skencil");
            contentType.Add(".sk1", "image/x-skencil");
            contentType.Add(".skr", "application/pgp-keys");
            contentType.Add(".slk", "text/spreadsheet");
            contentType.Add(".smaf", "application/x-smaf");
            contentType.Add(".smc", "application/x-snes-rom");
            contentType.Add(".smd", "application/vnd.stardivision.mail");
            contentType.Add(".smf", "application/vnd.stardivision.math");
            contentType.Add(".smi", "application/x-sami");
            contentType.Add(".smil", "application/smil");
            contentType.Add(".sml", "application/smil");
            contentType.Add(".sms", "application/x-sms-rom");
            contentType.Add(".snd", "audio/basic");
            contentType.Add(".so", "application/x-sharedlib");
            contentType.Add(".spc", "application/x-pkcs7-certificates");
            contentType.Add(".spd", "application/x-font-speedo");
            contentType.Add(".spec", "text/x-rpm-spec");
            contentType.Add(".spl", "application/x-shockwave-flash");
            contentType.Add(".spx", "audio/x-speex");
            contentType.Add(".sql", "text/x-sql");
            contentType.Add(".sr2", "image/x-sony-sr2");
            contentType.Add(".src", "application/x-wais-source");
            contentType.Add(".srf", "image/x-sony-srf");
            contentType.Add(".srt", "application/x-subrip");
            contentType.Add(".ssa", "text/x-ssa");
            contentType.Add(".stc", "application/vnd.sun.xml.calc.template");
            contentType.Add(".std", "application/vnd.sun.xml.draw.template");
            contentType.Add(".sti", "application/vnd.sun.xml.impress.template");
            contentType.Add(".stm", "audio/x-stm");
            contentType.Add(".stw", "application/vnd.sun.xml.writer.template");
            contentType.Add(".sty", "text/x-tex");
            contentType.Add(".sub", "text/x-subviewer");
            contentType.Add(".sun", "image/x-sun-raster");
            contentType.Add(".sv4cpio", "application/x-sv4cpio");
            contentType.Add(".sv4crc", "application/x-sv4crc");
            contentType.Add(".svg", "image/svg+xml");
            contentType.Add(".svgz", "image/svg+xml-compressed");
            contentType.Add(".swf", "application/x-shockwave-flash");
            contentType.Add(".sxc", "application/vnd.sun.xml.calc");
            contentType.Add(".sxd", "application/vnd.sun.xml.draw");
            contentType.Add(".sxg", "application/vnd.sun.xml.writer.global");
            contentType.Add(".sxi", "application/vnd.sun.xml.impress");
            contentType.Add(".sxm", "application/vnd.sun.xml.math");
            contentType.Add(".sxw", "application/vnd.sun.xml.writer");
            contentType.Add(".sylk", "text/spreadsheet");
            contentType.Add(".t", "text/troff");
            contentType.Add(".t2t", "text/x-txt2tags");
            contentType.Add(".tar", "application/x-tar");
            contentType.Add(".tar.bz", "application/x-bzip-compressed-tar");
            contentType.Add(".tar.bz2", "application/x-bzip-compressed-tar");
            contentType.Add(".tar.gz", "application/x-compressed-tar");
            contentType.Add(".tar.lzma", "application/x-lzma-compressed-tar");
            contentType.Add(".tar.lzo", "application/x-tzo");
            contentType.Add(".tar.xz", "application/x-xz-compressed-tar");
            contentType.Add(".tar.z", "application/x-tarz");
            contentType.Add(".tbz", "application/x-bzip-compressed-tar");
            contentType.Add(".tbz2", "application/x-bzip-compressed-tar");
            contentType.Add(".tcl", "text/x-tcl");
            contentType.Add(".tex", "text/x-tex");
            contentType.Add(".texi", "text/x-texinfo");
            contentType.Add(".texinfo", "text/x-texinfo");
            contentType.Add(".tga", "image/x-tga");
            contentType.Add(".tgz", "application/x-compressed-tar");
            contentType.Add(".theme", "application/x-theme");
            contentType.Add(".themepack", "application/x-windows-themepack");
            contentType.Add(".tif", "image/tiff");
            contentType.Add(".tiff", "image/tiff");
            contentType.Add(".tk", "text/x-tcl");
            contentType.Add(".tlz", "application/x-lzma-compressed-tar");
            contentType.Add(".tnef", "application/vnd.ms-tnef");
            contentType.Add(".tnf", "application/vnd.ms-tnef");
            contentType.Add(".toc", "application/x-cdrdao-toc");
            contentType.Add(".torrent", "application/x-bittorrent");
            contentType.Add(".tpic", "image/x-tga");
            contentType.Add(".tr", "text/troff");
            contentType.Add(".ts", "application/x-linguist");
            contentType.Add(".tsv", "text/tab-separated-values");
            contentType.Add(".tta", "audio/x-tta");
            contentType.Add(".ttc", "application/x-font-ttf");
            contentType.Add(".ttf", "application/x-font-ttf");
            contentType.Add(".ttx", "application/x-font-ttx");
            contentType.Add(".txt", "text/plain");
            contentType.Add(".txz", "application/x-xz-compressed-tar");
            contentType.Add(".tzo", "application/x-tzo");
            contentType.Add(".ufraw", "application/x-ufraw");
            contentType.Add(".ui", "application/x-designer");
            contentType.Add(".uil", "text/x-uil");
            contentType.Add(".ult", "audio/x-mod");
            contentType.Add(".uni", "audio/x-mod");
            contentType.Add(".uri", "text/x-uri");
            contentType.Add(".url", "text/x-uri");
            contentType.Add(".ustar", "application/x-ustar");
            contentType.Add(".vala", "text/x-vala");
            contentType.Add(".vapi", "text/x-vala");
            contentType.Add(".vcf", "text/directory");
            contentType.Add(".vcs", "text/calendar");
            contentType.Add(".vct", "text/directory");
            contentType.Add(".vda", "image/x-tga");
            contentType.Add(".vhd", "text/x-vhdl");
            contentType.Add(".vhdl", "text/x-vhdl");
            contentType.Add(".viv", "video/vivo");
            contentType.Add(".vivo", "video/vivo");
            contentType.Add(".vlc", "audio/x-mpegurl");
            contentType.Add(".vob", "video/mpeg");
            contentType.Add(".voc", "audio/x-voc");
            contentType.Add(".vor", "application/vnd.stardivision.writer");
            contentType.Add(".vst", "image/x-tga");
            contentType.Add(".wav", "audio/x-wav");
            contentType.Add(".wax", "audio/x-ms-asx");
            contentType.Add(".wb1", "application/x-quattropro");
            contentType.Add(".wb2", "application/x-quattropro");
            contentType.Add(".wb3", "application/x-quattropro");
            contentType.Add(".wbmp", "image/vnd.wap.wbmp");
            contentType.Add(".wcm", "application/vnd.ms-works");
            contentType.Add(".wdb", "application/vnd.ms-works");
            contentType.Add(".webm", "video/webm");
            contentType.Add(".wk1", "application/vnd.lotus-1-2-3");
            contentType.Add(".wk3", "application/vnd.lotus-1-2-3");
            contentType.Add(".wk4", "application/vnd.lotus-1-2-3");
            contentType.Add(".wks", "application/vnd.ms-works");
            contentType.Add(".wma", "audio/x-ms-wma");
            contentType.Add(".wmf", "image/x-wmf");
            contentType.Add(".wml", "text/vnd.wap.wml");
            contentType.Add(".wmls", "text/vnd.wap.wmlscript");
            contentType.Add(".wmv", "video/x-ms-wmv");
            contentType.Add(".wmx", "audio/x-ms-asx");
            contentType.Add(".wp", "application/vnd.wordperfect");
            contentType.Add(".wp4", "application/vnd.wordperfect");
            contentType.Add(".wp5", "application/vnd.wordperfect");
            contentType.Add(".wp6", "application/vnd.wordperfect");
            contentType.Add(".wpd", "application/vnd.wordperfect");
            contentType.Add(".wpg", "application/x-wpg");
            contentType.Add(".wpl", "application/vnd.ms-wpl");
            contentType.Add(".wpp", "application/vnd.wordperfect");
            contentType.Add(".wps", "application/vnd.ms-works");
            contentType.Add(".wri", "application/x-mswrite");
            contentType.Add(".wrl", "model/vrml");
            contentType.Add(".wv", "audio/x-wavpack");
            contentType.Add(".wvc", "audio/x-wavpack-correction");
            contentType.Add(".wvp", "audio/x-wavpack");
            contentType.Add(".wvx", "audio/x-ms-asx");
            contentType.Add(".x3f", "image/x-sigma-x3f");
            contentType.Add(".xac", "application/x-gnucash");
            contentType.Add(".xbel", "application/x-xbel");
            contentType.Add(".xbl", "application/xml");
            contentType.Add(".xbm", "image/x-xbitmap");
            contentType.Add(".xcf", "image/x-xcf");
            contentType.Add(".xcf.bz2", "image/x-compressed-xcf");
            contentType.Add(".xcf.gz", "image/x-compressed-xcf");
            contentType.Add(".xhtml", "application/xhtml+xml");
            contentType.Add(".xi", "audio/x-xi");
            contentType.Add(".xla", "application/vnd.ms-excel");
            contentType.Add(".xlc", "application/vnd.ms-excel");
            contentType.Add(".xld", "application/vnd.ms-excel");
            contentType.Add(".xlf", "application/x-xliff");
            contentType.Add(".xliff", "application/x-xliff");
            contentType.Add(".xll", "application/vnd.ms-excel");
            contentType.Add(".xlm", "application/vnd.ms-excel");
            contentType.Add(".xls", "application/vnd.ms-excel");
            contentType.Add(".xlsm", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            contentType.Add(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            contentType.Add(".xlt", "application/vnd.ms-excel");
            contentType.Add(".xlw", "application/vnd.ms-excel");
            contentType.Add(".xm", "audio/x-xm");
            contentType.Add(".xmf", "audio/x-xmf");
            contentType.Add(".xmi", "text/x-xmi");
            contentType.Add(".xml", "application/xml");
            contentType.Add(".xpm", "image/x-xpixmap");
            contentType.Add(".xps", "application/vnd.ms-xpsdocument");
            contentType.Add(".xsl", "application/xml");
            contentType.Add(".xslfo", "text/x-xslfo");
            contentType.Add(".xslt", "application/xml");
            contentType.Add(".xspf", "application/xspf+xml");
            contentType.Add(".xul", "application/vnd.mozilla.xul+xml");
            contentType.Add(".xwd", "image/x-xwindowdump");
            contentType.Add(".xyz", "chemical/x-pdb");
            contentType.Add(".xz", "application/x-xz");
            contentType.Add(".w2p", "application/w2p");
            contentType.Add(".z", "application/x-compress");
            contentType.Add(".zabw", "application/x-abiword");
            contentType.Add(".zip", "application/zip");


            foreach (KeyValuePair<string, string> kvp in contentType)
            {

                if (kvp.Value.Equals(type, StringComparison.OrdinalIgnoreCase))
                {
                    return kvp.Key;
                }

            }

            return String.Empty;

        }

        private void RequestHanderGridDelete_Click(object sender, EventArgs e)
        {
            for (int i = this.RequestHeaderGridView.SelectedRows.Count; i > 0; i--)
            {
                if (RequestHeaderGridView.SelectedRows[i - 1].Index != RequestHeaderGridView.Rows.Count - 1)
                    RequestHeaderGridView.Rows.RemoveAt(RequestHeaderGridView.SelectedRows[i - 1].Index);
            }
        }

    }
}
