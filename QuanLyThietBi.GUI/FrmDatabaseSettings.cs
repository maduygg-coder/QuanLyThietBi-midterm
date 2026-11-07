using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyThietBi.BLL;
using QuanLyThietBi.Entities;
namespace QuanLyThietBi.GUI
{
    public sealed class FrmDatabaseSettings : Form
    {
        private readonly DatabaseSetupService service;
        private readonly TextBox txtServer = new TextBox();
        private readonly TextBox txtDatabase = new TextBox();
        private readonly Button btnConnect = new Button();
        private readonly Button btnCancel = new Button();
        private readonly Label lblInfo = new Label();
        private bool busy;
        public FrmDatabaseSettings(DatabaseSetupService service)
        {
            this.service = service;
            Text = "Cấu hình kết nối SQL Server";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;
            ClientSize = new Size(640, 350);
            Font = new Font("Segoe UI", 10F);
            var layout = new TableLayoutPanel { Dock=DockStyle.Fill, Padding=new Padding(18), ColumnCount=2, RowCount=5 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            Controls.Add(layout);
            layout.Controls.Add(new Label { Text="Máy chủ SQL:", Dock=DockStyle.Fill, TextAlign=ContentAlignment.MiddleLeft },0,0);
            txtServer.Dock=DockStyle.Fill; txtServer.MaxLength=255; txtServer.TabIndex=0;
            layout.Controls.Add(txtServer,1,0);
            layout.Controls.Add(new Label { Text="Tên database:", Dock=DockStyle.Fill, TextAlign=ContentAlignment.MiddleLeft },0,1);
            txtDatabase.Dock=DockStyle.Fill; txtDatabase.MaxLength=128; txtDatabase.TabIndex=1;
            layout.Controls.Add(txtDatabase,1,1);
            var guide = new Label
            {
                Text = "LocalDB: (localdb)\\MSSQLLocalDB\r\nSQL Express: .\\SQLEXPRESS\r\nMáy chưa có LocalDB: mở Visual Studio Installer → Modify → Individual components → SQL Server Express LocalDB.",
                Dock = DockStyle.Fill
            };
            layout.Controls.Add(guide,0,2); layout.SetColumnSpan(guide,2);
            lblInfo.Text="Giữ cách đăng nhập trong cấu hình hiện có (mặc định tài khoản Windows). Kiểm tra máy chủ thành công mới lưu cấu hình; database sẽ được tạo khi nạp dữ liệu.";
            lblInfo.Dock=DockStyle.Fill;
            layout.Controls.Add(lblInfo,0,3); layout.SetColumnSpan(lblInfo,2);
            var actions=new FlowLayoutPanel { Dock=DockStyle.Fill, FlowDirection=FlowDirection.RightToLeft };
            btnConnect.Text="Kết nối và lưu"; btnConnect.AutoSize=true; btnConnect.TabIndex=2;
            btnCancel.Text="Đóng"; btnCancel.AutoSize=true; btnCancel.DialogResult=DialogResult.Cancel; btnCancel.TabIndex=3;
            actions.Controls.Add(btnCancel); actions.Controls.Add(btnConnect);
            layout.Controls.Add(actions,0,4); layout.SetColumnSpan(actions,2);
            AcceptButton=btnConnect; CancelButton=btnCancel;
            btnConnect.Click += async (s,e) => await ConnectAsync();
            Load += (s,e) =>
            {
                try
                {
                    var current=service.Read();
                    txtServer.Text=current.Server; txtDatabase.Text=current.Database;
                }
                catch(Exception ex) { lblInfo.Text=ex.Message; }
            };
            FormClosing += (s,e) => { if(busy) e.Cancel=true; };
        }
        private async Task ConnectAsync()
        {
            if(busy) return;
            var input = new DatabaseConnectionOptions { Server=txtServer.Text, Database=txtDatabase.Text };
            busy=true; btnConnect.Enabled=btnCancel.Enabled=false; txtServer.Enabled=txtDatabase.Enabled=false;
            lblInfo.Text="Đang kiểm tra máy chủ SQL Server…";
            try
            {
                await Task.Run(() => service.TestAndSave(input));
                busy=false; DialogResult=DialogResult.OK; Close();
            }
            catch(Exception ex)
            {
                lblInfo.Text="Chưa lưu cấu hình. Kiểm tra tên máy chủ và việc cài SQL Server/LocalDB.";
                MessageBox.Show(this,ex.Message,"Không kết nối được",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            finally
            {
                busy=false;
                if(!IsDisposed) btnConnect.Enabled=btnCancel.Enabled=txtServer.Enabled=txtDatabase.Enabled=true;
            }
        }
    }
}
