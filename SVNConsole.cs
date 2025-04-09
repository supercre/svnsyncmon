using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SVNSyncMon
{
    public partial class SVNConsole : Form
    {
        private List<SvnPathInfo> svnPaths;
        private CancellationTokenSource cancellationTokenSource;
        private Task updateTask;
        private string svnPath;

        public SVNConsole()
        {
            InitializeComponent();
            InitializeSvnPaths();
            StartUpdateTask();
            
            // 아이콘 설정
            try
            {
                string iconPath = Path.Combine(Application.StartupPath, "favicon.ico");
                if (File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
            }
            catch (Exception)
            {
                // 오류 발생 시 기본 아이콘 사용
            }
        }

        private void InitializeComponent()
        {
            mainPanel = new TableLayoutPanel();
            logBox = new TextBox();
            svnPathPanel = new Panel();
            svnPathLabel = new Label();
            svnPathTextBox = new TextBox();
            svnPathSelectButton = new Button();
            menuStrip = new MenuStrip();
            settingsMenuItem = new ToolStripMenuItem();
            editPathsMenuItem = new ToolStripMenuItem();
            languageMenuItem = new ToolStripMenuItem();
            koreanMenuItem = new ToolStripMenuItem();
            englishMenuItem = new ToolStripMenuItem();
            updateAllButton = new Button();
            clearLogButton = new Button();
            mainPanel.SuspendLayout();
            svnPathPanel.SuspendLayout();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.ColumnCount = 1;
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.Controls.Add(logBox, 0, 0);
            mainPanel.Controls.Add(svnPathPanel, 0, 1);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 35);
            mainPanel.Margin = new Padding(4, 5, 4, 5);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 2;
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 90F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            mainPanel.Size = new Size(1143, 965);
            mainPanel.TabIndex = 0;
            // 
            // logBox
            // 
            logBox.BackColor = Color.Black;
            logBox.Dock = DockStyle.Fill;
            logBox.Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point);
            logBox.ForeColor = Color.White;
            logBox.Location = new Point(4, 5);
            logBox.Margin = new Padding(4, 5, 4, 5);
            logBox.Multiline = true;
            logBox.Name = "logBox";
            logBox.ReadOnly = true;
            logBox.ScrollBars = ScrollBars.Vertical;
            logBox.Size = new Size(1135, 858);
            logBox.TabIndex = 0;
            // 
            // svnPathPanel
            // 
            svnPathPanel.Controls.Add(clearLogButton);
            svnPathPanel.Controls.Add(updateAllButton);
            svnPathPanel.Controls.Add(svnPathLabel);
            svnPathPanel.Controls.Add(svnPathTextBox);
            svnPathPanel.Controls.Add(svnPathSelectButton);
            svnPathPanel.Dock = DockStyle.Fill;
            svnPathPanel.Location = new Point(4, 873);
            svnPathPanel.Margin = new Padding(4, 5, 4, 5);
            svnPathPanel.Name = "svnPathPanel";
            svnPathPanel.Size = new Size(1135, 87);
            svnPathPanel.TabIndex = 1;
            // 
            // svnPathLabel
            // 
            svnPathLabel.AutoSize = true;
            svnPathLabel.Location = new Point(4, 10);
            svnPathLabel.Margin = new Padding(4, 0, 4, 0);
            svnPathLabel.Name = "svnPathLabel";
            svnPathLabel.Size = new Size(135, 25);
            svnPathLabel.TabIndex = 0;
            svnPathLabel.Text = Localization.GetString("SVNPathLabel");
            // 
            // svnPathTextBox
            // 
            svnPathTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            svnPathTextBox.Location = new Point(140, 5);
            svnPathTextBox.Margin = new Padding(4, 5, 4, 5);
            svnPathTextBox.Name = "svnPathTextBox";
            svnPathTextBox.ReadOnly = true;
            svnPathTextBox.Size = new Size(875, 31);
            svnPathTextBox.TabIndex = 1;
            // 
            // svnPathSelectButton
            // 
            svnPathSelectButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            svnPathSelectButton.Location = new Point(1024, 3);
            svnPathSelectButton.Margin = new Padding(4, 5, 4, 5);
            svnPathSelectButton.Name = "svnPathSelectButton";
            svnPathSelectButton.Size = new Size(107, 38);
            svnPathSelectButton.TabIndex = 3;
            svnPathSelectButton.Text = Localization.GetString("SelectButton");
            svnPathSelectButton.UseVisualStyleBackColor = true;
            svnPathSelectButton.Click += svnPathSelectButton_Click;
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new Size(24, 24);
            menuStrip.Items.AddRange(new ToolStripItem[] { settingsMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new Padding(9, 3, 0, 3);
            menuStrip.Size = new Size(1143, 35);
            menuStrip.TabIndex = 1;
            menuStrip.Text = "menuStrip";
            // 
            // settingsMenuItem
            // 
            settingsMenuItem.DropDownItems.AddRange(new ToolStripItem[] { editPathsMenuItem, languageMenuItem });
            settingsMenuItem.Name = "settingsMenuItem";
            settingsMenuItem.Size = new Size(64, 29);
            settingsMenuItem.Text = Localization.GetString("FileMenu");
            // 
            // editPathsMenuItem
            // 
            editPathsMenuItem.Name = "editPathsMenuItem";
            editPathsMenuItem.Size = new Size(251, 34);
            editPathsMenuItem.Text = Localization.GetString("EditPathsMenu");
            editPathsMenuItem.Click += editPathsMenuItem_Click;
            // 
            // languageMenuItem
            // 
            languageMenuItem.Text = Localization.GetString("Language");
            koreanMenuItem.Text = Localization.GetString("Korean");
            englishMenuItem.Text = Localization.GetString("English");
            var germanMenuItem = new ToolStripMenuItem(Localization.GetString("German"));
            var japaneseMenuItem = new ToolStripMenuItem(Localization.GetString("Japanese"));
            var chineseSimplifiedMenuItem = new ToolStripMenuItem(Localization.GetString("ChineseSimplified"));
            var chineseTraditionalMenuItem = new ToolStripMenuItem(Localization.GetString("ChineseTraditional"));

            // 현재 언어에 따라 체크 표시 설정
            string currentLanguage = Localization.CurrentLanguage;
            koreanMenuItem.Checked = currentLanguage == "ko";
            englishMenuItem.Checked = currentLanguage == "en";
            germanMenuItem.Checked = currentLanguage == "de";
            japaneseMenuItem.Checked = currentLanguage == "ja";
            chineseSimplifiedMenuItem.Checked = currentLanguage == "zh-CN";
            chineseTraditionalMenuItem.Checked = currentLanguage == "zh-TW";

            koreanMenuItem.Click += (s, e) => ChangeLanguage("ko");
            englishMenuItem.Click += (s, e) => ChangeLanguage("en");
            germanMenuItem.Click += (s, e) => ChangeLanguage("de");
            japaneseMenuItem.Click += (s, e) => ChangeLanguage("ja");
            chineseSimplifiedMenuItem.Click += (s, e) => ChangeLanguage("zh-CN");
            chineseTraditionalMenuItem.Click += (s, e) => ChangeLanguage("zh-TW");
            languageMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 
                koreanMenuItem, 
                englishMenuItem, 
                germanMenuItem, 
                japaneseMenuItem, 
                chineseSimplifiedMenuItem, 
                chineseTraditionalMenuItem 
            });
            settingsMenuItem.DropDownItems.Add(languageMenuItem);
            // 
            // updateAllButton
            // 
            updateAllButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            updateAllButton.Location = new Point(4, 49);
            updateAllButton.Margin = new Padding(4, 5, 4, 5);
            updateAllButton.Name = "updateAllButton";
            updateAllButton.Size = new Size(302, 38);
            updateAllButton.TabIndex = 4;
            updateAllButton.Text = Localization.GetString("UpdateAllNow");
            updateAllButton.UseVisualStyleBackColor = true;
            updateAllButton.Click += updateAllButton_Click;
            // 
            // clearLogButton
            // 
            clearLogButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            clearLogButton.Location = new Point(310, 49);
            clearLogButton.Margin = new Padding(4, 5, 4, 5);
            clearLogButton.Name = "clearLogButton";
            clearLogButton.Size = new Size(150, 38);
            clearLogButton.TabIndex = 5;
            clearLogButton.Text = Localization.GetString("ClearLog");
            clearLogButton.UseVisualStyleBackColor = true;
            clearLogButton.Click += clearLogButton_Click;
            // 
            // SVNConsole
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1143, 1000);
            Controls.Add(mainPanel);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            Margin = new Padding(4, 5, 4, 5);
            MinimumSize = new Size(562, 463);
            Name = "SVNConsole";
            StartPosition = FormStartPosition.CenterScreen;
            Text = Localization.GetString("AppTitle");
            FormClosing += SVNConsole_FormClosing;
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            svnPathPanel.ResumeLayout(false);
            svnPathPanel.PerformLayout();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem settingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editPathsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageMenuItem;
        private System.Windows.Forms.ToolStripMenuItem koreanMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishMenuItem;
        private System.Windows.Forms.Panel svnPathPanel;
        private System.Windows.Forms.Label svnPathLabel;
        private System.Windows.Forms.TextBox svnPathTextBox;
        private System.Windows.Forms.Button svnPathSelectButton;
        private System.Windows.Forms.Button updateAllButton;
        private System.Windows.Forms.Button clearLogButton;

        private void DisplaySvnPathsInfo()
        {
            foreach (var pathInfo in svnPaths)
            {
                DateTime nextUpdateTime = DateTime.Now.AddMinutes(pathInfo.UpdateInterval);
                logBox.AppendText($"- {pathInfo.Path} ({Localization.GetString("UpdateInterval")}: {pathInfo.UpdateInterval}{Localization.GetString("Minutes")}){Environment.NewLine}");
                logBox.AppendText($"  {Localization.GetString("NextUpdateTime")}: {nextUpdateTime:yyyy-MM-dd HH:mm:ss}{Environment.NewLine}");
            }
            logBox.AppendText(Environment.NewLine);
        }

        private void AppendLog(string message)
        {
            if (logBox.Lines.Length > 5000)
            {
                logBox.Clear();
                logBox.AppendText(Localization.GetString("LogExceeded") + Environment.NewLine);
                DisplaySvnPathsInfo();
            }
            logBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }

        private void InitializeSvnPaths()
        {
            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string configFolderPath = Path.Combine(userProfilePath, ".svnupdator");
            string configFilePath = Path.Combine(configFolderPath, "svnpaths.txt");

            svnPaths = new List<SvnPathInfo>();

            if (File.Exists(configFilePath))
            {
                string[] lines = File.ReadAllLines(configFilePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int interval))
                    {
                        svnPaths.Add(new SvnPathInfo { Path = parts[0], UpdateInterval = interval });
                    }
                }

                AppendLog(Localization.GetString("SVNPathsUpdated", svnPaths.Count));
                DisplaySvnPathsInfo();
            }
            else
            {
                AppendLog(Localization.GetString("SVNPathsConfigNotFound"));
            }

            LoadSvnPath();
        }

        private void LoadSvnPath()
        {
            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string configFolderPath = Path.Combine(userProfilePath, ".svnupdator");
            string configFilePath = Path.Combine(configFolderPath, "svnpath.txt");

            if (File.Exists(configFilePath))
            {
                svnPath = File.ReadAllText(configFilePath);
                svnPathTextBox.Text = svnPath;
            }
            else
            {
                // 환경 변수 PATH에서 svn.exe 찾기
                svnPath = FindSvnInPath();
                svnPathTextBox.Text = svnPath;
                SaveSvnPath();
            }
        }

        private string FindSvnInPath()
        {
            string pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrEmpty(pathEnv))
                return "svn";

            string[] paths = pathEnv.Split(Path.PathSeparator);
            foreach (string path in paths)
            {
                string svnPath = Path.Combine(path, "svn.exe");
                if (File.Exists(svnPath))
                    return svnPath;
            }

            return "svn"; // 기본값
        }

        private void SaveSvnPath()
        {
            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string configFolderPath = Path.Combine(userProfilePath, ".svnupdator");
            string configFilePath = Path.Combine(configFolderPath, "svnpath.txt");

            // 기존 경로 읽기
            string oldPath = "";
            if (File.Exists(configFilePath))
            {
                oldPath = File.ReadAllText(configFilePath);
            }

            // 새 경로 저장
            if (!Directory.Exists(configFolderPath))
            {
                Directory.CreateDirectory(configFolderPath);
            }

            string newPath = svnPathTextBox.Text;
            File.WriteAllText(configFilePath, newPath);
            svnPath = newPath;

            // 경로가 변경되었고, 기존 경로가 존재하면 삭제
            if (!string.IsNullOrEmpty(oldPath) && oldPath != newPath && File.Exists(oldPath))
            {
                try
                {
                    File.Delete(oldPath);
                    AppendLog(Localization.GetString("OldPathDeleted", oldPath));
                }
                catch (Exception ex)
                {
                    AppendLog(Localization.GetString("OldPathDeleteFailed", ex.Message));
                }
            }

            AppendLog(Localization.GetString("SVNPathSaved"));
        }

        private void StartUpdateTask()
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            
            updateTask = Task.Run(async () =>
            {
                // 각 저장소의 마지막 업데이트 시간을 추적
                var lastUpdateTimes = svnPaths.ToDictionary(p => p.Path, _ => DateTime.MinValue);
                
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        var now = DateTime.Now;
                        foreach (var pathInfo in svnPaths.ToList())
                        {
                            if (cancellationTokenSource.Token.IsCancellationRequested)
                                break;

                            // 마지막 업데이트로부터 지정된 간격이 지났는지 확인
                            var lastUpdate = lastUpdateTimes[pathInfo.Path];
                            var nextUpdate = lastUpdate.AddMinutes(pathInfo.UpdateInterval);
                            
                            if (lastUpdate == DateTime.MinValue || now >= nextUpdate)
                            {
                                try
                                {
                                    if (!IsDisposed && !cancellationTokenSource.Token.IsCancellationRequested)
                                    {
                                        await InvokeAsync(() =>
                                        {
                                            AppendLog(Localization.GetString("UpdateStarted", pathInfo.Path));
                                        });
                                    }
                                    
                                    await ExecuteSvnUpdate(pathInfo.Path);
                                    lastUpdateTimes[pathInfo.Path] = now;  // 업데이트 시간 기록
                                    
                                    if (!IsDisposed && !cancellationTokenSource.Token.IsCancellationRequested)
                                    {
                                        await InvokeAsync(() =>
                                        {
                                            AppendLog(Localization.GetString("UpdateCompleted", pathInfo.Path));
                                        });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (!IsDisposed && !cancellationTokenSource.Token.IsCancellationRequested)
                                    {
                                        AppendLog(Localization.GetString("UpdateFailed", pathInfo.Path, ex.Message));
                                    }
                                }
                            }
                        }
                        
                        // 1초 대기 후 다시 검사
                        await Task.Delay(1000, cancellationTokenSource.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (!IsDisposed && !cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            AppendLog(Localization.GetString("TaskError", ex.Message));
                        }
                        await Task.Delay(5000, cancellationTokenSource.Token);
                    }
                }
            }, cancellationTokenSource.Token);
        }

        private Task InvokeAsync(Action action)
        {
            if (IsDisposed)
                return Task.CompletedTask;

            if (InvokeRequired)
                return Task.Run(() => Invoke(action));
            else
            {
                action();
                return Task.CompletedTask;
            }
        }

        private void SVNConsole_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private async void RestartUpdateTask()
        {
            // 기존 태스크 취소
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                try
                {
                    await Task.WhenAny(updateTask, Task.Delay(1000)); // 최대 1초 대기
                }
                catch (Exception)
                {
                    // 태스크 취소 중 발생하는 예외 무시
                }
            }
            
            // 새 태스크 시작
            StartUpdateTask();
        }

        private void editPathsMenuItem_Click(object sender, EventArgs e)
        {
            SVNPathEditor pathEditor = new SVNPathEditor();
            if (pathEditor.ShowDialog() == DialogResult.OK)
            {
                // 경로 목록 업데이트
                svnPaths = pathEditor.GetSvnPaths();
                AppendLog(Localization.GetString("SVNPathsUpdated", svnPaths.Count));
                DisplaySvnPathsInfo();
                
                // 업데이트 태스크 재시작
                RestartUpdateTask();
            }
        }

        private async Task ExecuteSvnUpdate(string path)
        {
            if (string.IsNullOrEmpty(svnPath))
            {
                await InvokeAsync(() =>
                {
                    AppendLog(Localization.GetString("SVNPathNotFound"));
                });
                return;
            }

            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = svnPath,
                    Arguments = $"update \"{path}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = System.Diagnostics.Process.Start(startInfo))
                {
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    if (process.ExitCode == 0)
                    {
                        await InvokeAsync(() =>
                        {
                            AppendLog(Localization.GetString("SVNUpdateSuccess", output));
                            AppendLog(Localization.GetString("UpdateCompleteTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                            
                            // 다음 업데이트 시각 계산 및 표시
                            var pathInfo = svnPaths.FirstOrDefault(p => p.Path == path);
                            if (pathInfo != null)
                            {
                                DateTime nextUpdateTime = DateTime.Now.AddMinutes(pathInfo.UpdateInterval);
                                AppendLog(Localization.GetString("NextUpdateTime", nextUpdateTime.ToString("yyyy-MM-dd HH:mm:ss")));
                            }
                        });
                    }
                    else
                    {
                        await InvokeAsync(() =>
                        {
                            AppendLog(Localization.GetString("SVNUpdateFailed", error));
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                await InvokeAsync(() =>
                {
                    AppendLog(Localization.GetString("SVNUpdateError", ex.Message));
                });
            }
        }

        private void svnPathSelectButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = Localization.GetString("SVNExecutableFilter");
                openFileDialog.Title = Localization.GetString("SelectSVNExecutable");
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    svnPathTextBox.Text = openFileDialog.FileName;
                    SaveSvnPath();
                }
            }
        }

        private void svnPathTextBox_TextChanged(object sender, EventArgs e)
        {
            // 텍스트 박스가 읽기 전용이므로 이 이벤트 핸들러는 제거
        }

        private async void updateAllButton_Click(object sender, EventArgs e)
        {
            updateAllButton.Enabled = false;
            try
            {
                foreach (var pathInfo in svnPaths)
                {
                    await ExecuteSvnUpdate(pathInfo.Path);
                }
            }
            finally
            {
                updateAllButton.Enabled = true;
            }
        }

        private void clearLogButton_Click(object sender, EventArgs e)
        {
            logBox.Clear();
            logBox.AppendText(Localization.GetString("LogCleared") + Environment.NewLine);
            DisplaySvnPathsInfo();
        }

        private void ChangeLanguage(string language)
        {
            Localization.CurrentLanguage = language;
            UpdateUITexts();

            // 언어 메뉴 항목의 체크 표시 업데이트
            foreach (ToolStripMenuItem item in languageMenuItem.DropDownItems)
            {
                item.Checked = (item.Text == Localization.GetString(language == "ko" ? "Korean" :
                                                                   language == "en" ? "English" :
                                                                   language == "de" ? "German" :
                                                                   language == "ja" ? "Japanese" :
                                                                   language == "zh-CN" ? "ChineseSimplified" :
                                                                   "ChineseTraditional"));
            }
        }

        private void UpdateUITexts()
        {
            this.Text = Localization.GetString("AppTitle");
            settingsMenuItem.Text = Localization.GetString("FileMenu");
            editPathsMenuItem.Text = Localization.GetString("EditPathsMenu");
            languageMenuItem.Text = Localization.GetString("Language");
            svnPathLabel.Text = Localization.GetString("SVNPathLabel");
            svnPathSelectButton.Text = Localization.GetString("SelectButton");
            updateAllButton.Text = Localization.GetString("UpdateAllNow");
            clearLogButton.Text = Localization.GetString("ClearLog");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
} 