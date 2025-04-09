using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Microsoft.Win32;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace SVNSyncMon
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // 아이콘 생성 및 저장
            Icon customIcon = CreateCustomIcon();
            
            // 메인 폼 생성 및 표시
            SVNConsole mainForm = new SVNConsole();
            mainForm.Show();
            
            // 시스템 트레이 아이콘 생성
            NotifyIcon trayIcon = new NotifyIcon();
            trayIcon.Icon = customIcon;
            trayIcon.Text = Localization.GetString("AppTitle");
            trayIcon.Visible = true;
            
            // 컨텍스트 메뉴 생성
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            
            // 시작 프로그램 등록 메뉴 추가
            ToolStripMenuItem startupMenuItem = new ToolStripMenuItem(Localization.GetString("StartupRegister"));
            startupMenuItem.Click += (s, e) => {
                SetStartup(true);
                startupMenuItem.Checked = IsStartupEnabled();
            };
            startupMenuItem.Checked = IsStartupEnabled();
            contextMenu.Items.Add(startupMenuItem);
            
            // 구분선 추가
            contextMenu.Items.Add(new ToolStripSeparator());
            
            ToolStripMenuItem showMenuItem = new ToolStripMenuItem(Localization.GetString("ShowSVNConsole"));
            showMenuItem.Click += (s, e) => mainForm.Show();
            contextMenu.Items.Add(showMenuItem);
            
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem(Localization.GetString("Exit"));
            exitMenuItem.Click += (s, e) => {
                trayIcon.Visible = false;
                Application.Exit();
            };
            contextMenu.Items.Add(exitMenuItem);
            
            trayIcon.ContextMenuStrip = contextMenu;
            
            // 더블 클릭 이벤트 처리
            trayIcon.DoubleClick += (s, e) => mainForm.Show();
            
            Application.Run();
        }
        
        // Windows 시작 시 자동 실행 설정
        private static void SetStartup(bool enable)
        {
            try
            {
                string appPath = Application.ExecutablePath;
                string appName = Path.GetFileNameWithoutExtension(appPath);
                
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (enable)
                    {
                        key.SetValue(appName, appPath);
                    }
                    else
                    {
                        key.DeleteValue(appName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(Localization.GetString("StartupError"), ex.Message),
                    Localization.GetString("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        
        // Windows 시작 시 자동 실행 여부 확인
        private static bool IsStartupEnabled()
        {
            try
            {
                string appPath = Application.ExecutablePath;
                string appName = Path.GetFileNameWithoutExtension(appPath);
                
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false))
                {
                    string value = key.GetValue(appName) as string;
                    return value != null && value.Equals(appPath, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }
        
        private static Icon CreateCustomIcon()
        {
            try
            {
                // 리소스에서 아이콘 로드
                using (Stream stream = typeof(Program).Assembly.GetManifestResourceStream("SVNSyncMon.favicon.ico"))
                {
                    if (stream != null)
                    {
                        return new Icon(stream);
                    }
                }
                
                // 리소스 로드 실패 시 기본 아이콘 생성
                using (Bitmap bitmap = new Bitmap(32, 32))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.Transparent);
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        // 파란색 물결 그리기
                        using (GraphicsPath path1 = new GraphicsPath())
                        {
                            path1.AddArc(2, 2, 28, 28, 0, 180);
                            using (Pen pen1 = new Pen(Color.FromArgb(0, 157, 255), 3))
                            {
                                g.DrawPath(pen1, path1);
                            }
                        }

                        using (GraphicsPath path2 = new GraphicsPath())
                        {
                            path2.AddArc(6, 6, 20, 20, 0, 180);
                            using (Pen pen2 = new Pen(Color.FromArgb(0, 120, 215), 2))
                            {
                                g.DrawPath(pen2, path2);
                            }
                        }

                        using (GraphicsPath path3 = new GraphicsPath())
                        {
                            path3.AddArc(10, 10, 12, 12, 0, 180);
                            using (Pen pen3 = new Pen(Color.FromArgb(0, 90, 180), 2))
                            {
                                g.DrawPath(pen3, path3);
                            }
                        }

                        // SVN 텍스트 그리기
                        using (Font font = new Font("Arial", 8, FontStyle.Bold))
                        {
                            using (SolidBrush brush = new SolidBrush(Color.FromArgb(0, 120, 215)))
                            {
                                string text = "SVN";
                                SizeF textSize = g.MeasureString(text, font);
                                float x = (32 - textSize.Width) / 2;
                                float y = 18;
                                g.DrawString(text, font, brush, x, y);
                            }
                        }
                    }

                    // 아이콘 생성
                    return Icon.FromHandle(bitmap.GetHicon());
                }
            }
            catch (Exception)
            {
                // 오류 발생 시 기본 아이콘 반환
                return SystemIcons.Application;
            }
        }
    }
} 