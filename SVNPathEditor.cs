using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SVNSyncMon
{
    public partial class SVNPathEditor : Form
    {
        private ListView pathListView;
        private Button addButton;
        private Button removeButton;
        private Button saveButton;
        private Button cancelButton;
        private List<SvnPathInfo> svnPaths;
        private System.Windows.Forms.TextBox svnPathTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label svnPathLabel;
        private string svnPath;

        public SVNPathEditor()
        {
            InitializeComponent();
            LoadSvnPaths();
            LoadSvnPath();
            
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
            this.pathListView = new System.Windows.Forms.ListView();
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.svnPathLabel = new System.Windows.Forms.Label();
            this.svnPathTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pathListView
            // 
            this.pathListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathListView.FullRowSelect = true;
            this.pathListView.GridLines = true;
            this.pathListView.HideSelection = false;
            this.pathListView.Location = new System.Drawing.Point(12, 12);
            this.pathListView.MultiSelect = false;
            this.pathListView.Name = "pathListView";
            this.pathListView.Size = new System.Drawing.Size(560, 300);
            this.pathListView.TabIndex = 0;
            this.pathListView.UseCompatibleStateImageBehavior = false;
            this.pathListView.View = System.Windows.Forms.View.Details;
            this.pathListView.Columns.Add(Localization.GetString("Path"), 300);
            this.pathListView.Columns.Add(Localization.GetString("UpdateInterval"), 100);
            this.pathListView.DoubleClick += new System.EventHandler(this.pathListView_DoubleClick);
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addButton.Location = new System.Drawing.Point(12, 318);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 1;
            this.addButton.Text = Localization.GetString("Add");
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeButton.Location = new System.Drawing.Point(93, 318);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 2;
            this.removeButton.Text = Localization.GetString("Remove");
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.Location = new System.Drawing.Point(416, 318);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 3;
            this.saveButton.Text = Localization.GetString("Save");
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(497, 318);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = Localization.GetString("Cancel");
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // svnPathLabel
            // 
            this.svnPathLabel.AutoSize = true;
            this.svnPathLabel.Location = new System.Drawing.Point(12, 15);
            this.svnPathLabel.Name = "svnPathLabel";
            this.svnPathLabel.Size = new System.Drawing.Size(89, 15);
            this.svnPathLabel.TabIndex = 0;
            this.svnPathLabel.Text = Localization.GetString("SVNPathLabel");
            // 
            // svnPathTextBox
            // 
            this.svnPathTextBox.Location = new System.Drawing.Point(107, 12);
            this.svnPathTextBox.Name = "svnPathTextBox";
            this.svnPathTextBox.Size = new System.Drawing.Size(300, 23);
            this.svnPathTextBox.TabIndex = 1;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(413, 10);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = Localization.GetString("Browse");
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // SVNPathEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 353);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.pathListView);
            this.Controls.Add(this.svnPathLabel);
            this.Controls.Add(this.svnPathTextBox);
            this.Controls.Add(this.browseButton);
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "SVNPathEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = Localization.GetString("Settings");
            this.ResumeLayout(false);
            svnPathLabel.Text = Localization.GetString("SVNPath");
            browseButton.Text = Localization.GetString("Select");
            saveButton.Text = Localization.GetString("Save");
            cancelButton.Text = Localization.GetString("Cancel");
        }

        private void LoadSvnPaths()
        {
            svnPaths = new List<SvnPathInfo>();
            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string configFolderPath = Path.Combine(userProfilePath, ".svnupdator");
            string configFilePath = Path.Combine(configFolderPath, "svnpaths.txt");

            if (File.Exists(configFilePath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(configFilePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length >= 1)
                        {
                            string path = parts[0];
                            int interval = 5; // 기본값 5분
                            
                            if (parts.Length >= 2 && int.TryParse(parts[1], out int parsedInterval))
                            {
                                interval = parsedInterval;
                            }
                            
                            svnPaths.Add(new SvnPathInfo { Path = path, UpdateInterval = interval });
                        }
                    }
                    RefreshListView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Localization.GetString("SVNPathLoadFailed", ex.Message), 
                                  Localization.GetString("Error"), 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Error);
                }
            }
        }

        private void RefreshListView()
        {
            pathListView.Items.Clear();
            foreach (SvnPathInfo pathInfo in svnPaths)
            {
                ListViewItem item = new ListViewItem(pathInfo.Path);
                item.SubItems.Add(pathInfo.UpdateInterval.ToString());
                pathListView.Items.Add(item);
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = Localization.GetString("SelectSVNRepositoryPath");
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.SelectedPath;
                    if (!svnPaths.Any(p => p.Path == path))
                    {
                        // 업데이트 간격 설정 다이얼로그 표시
                        using (var intervalDialog = new UpdateIntervalDialog())
                        {
                            if (intervalDialog.ShowDialog() == DialogResult.OK)
                            {
                                svnPaths.Add(new SvnPathInfo { Path = path, UpdateInterval = intervalDialog.UpdateInterval });
                                RefreshListView();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(Localization.GetString("DuplicatePath"), 
                                      Localization.GetString("Notice"), 
                                      MessageBoxButtons.OK, 
                                      MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void pathListView_DoubleClick(object sender, EventArgs e)
        {
            if (pathListView.SelectedItems.Count > 0)
            {
                int index = pathListView.SelectedItems[0].Index;
                if (index >= 0 && index < svnPaths.Count)
                {
                    using (var intervalDialog = new UpdateIntervalDialog(svnPaths[index].UpdateInterval))
                    {
                        if (intervalDialog.ShowDialog() == DialogResult.OK)
                        {
                            svnPaths[index].UpdateInterval = intervalDialog.UpdateInterval;
                            RefreshListView();
                        }
                    }
                }
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (pathListView.SelectedItems.Count > 0)
            {
                int index = pathListView.SelectedItems[0].Index;
                if (index >= 0 && index < svnPaths.Count)
                {
                    svnPaths.RemoveAt(index);
                    RefreshListView();
                }
            }
            else
            {
                MessageBox.Show(Localization.GetString("SelectPathToRemove"), 
                              Localization.GetString("Notice"), 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Information);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string configFolderPath = Path.Combine(userProfilePath, ".svnupdator");
            string configFilePath = Path.Combine(configFolderPath, "svnpaths.txt");

            try
            {
                if (!Directory.Exists(configFolderPath))
                {
                    Directory.CreateDirectory(configFolderPath);
                }

                // 경로와 업데이트 간격을 함께 저장
                string[] lines = svnPaths.Select(p => $"{p.Path}|{p.UpdateInterval}").ToArray();
                File.WriteAllLines(configFilePath, lines);
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Localization.GetString("SVNPathSaveFailed", ex.Message), 
                              Localization.GetString("Error"), 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = Localization.GetString("ExecutableFiles");
                openFileDialog.Title = Localization.GetString("SelectSVNPath");
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    svnPathTextBox.Text = openFileDialog.FileName;
                }
            }
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
        }

        private void SaveSvnPath()
        {
            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string configFolderPath = Path.Combine(userProfilePath, ".svnupdator");
            string configFilePath = Path.Combine(configFolderPath, "svnpath.txt");

            if (!Directory.Exists(configFolderPath))
            {
                Directory.CreateDirectory(configFolderPath);
            }

            File.WriteAllText(configFilePath, svnPathTextBox.Text);
        }

        public List<SvnPathInfo> GetSvnPaths()
        {
            return svnPaths;
        }
    }

    public class SvnPathInfo
    {
        public string Path { get; set; }
        public int UpdateInterval { get; set; } // 분 단위
    }

    public class UpdateIntervalDialog : Form
    {
        private NumericUpDown intervalUpDown;
        private Button okButton;
        private Button cancelButton;
        private Label label;

        public int UpdateInterval { get; private set; }

        public UpdateIntervalDialog(int currentInterval = 5)
        {
            InitializeComponent();
            UpdateInterval = currentInterval;
            intervalUpDown.Value = currentInterval;
        }

        private void InitializeComponent()
        {
            this.intervalUpDown = new System.Windows.Forms.NumericUpDown();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.intervalUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // intervalUpDown
            // 
            this.intervalUpDown.Location = new System.Drawing.Point(120, 20);
            this.intervalUpDown.Maximum = new decimal(new int[] { 1440, 0, 0, 0 });
            this.intervalUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.intervalUpDown.Name = "intervalUpDown";
            this.intervalUpDown.Size = new System.Drawing.Size(80, 23);
            this.intervalUpDown.TabIndex = 0;
            this.intervalUpDown.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(44, 60);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = Localization.GetString("OK");
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(125, 60);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = Localization.GetString("Cancel");
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(12, 22);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(102, 15);
            this.label.TabIndex = 3;
            this.label.Text = Localization.GetString("UpdateIntervalLabel");
            // 
            // UpdateIntervalDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(234, 101);
            this.Controls.Add(this.label);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.intervalUpDown);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateIntervalDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = Localization.GetString("UpdateIntervalTitle");
            ((System.ComponentModel.ISupportInitialize)(this.intervalUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            UpdateInterval = (int)intervalUpDown.Value;
        }
    }
}