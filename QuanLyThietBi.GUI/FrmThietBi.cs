using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyThietBi.BLL;
using QuanLyThietBi.Entities;
namespace QuanLyThietBi.GUI
{
    public sealed class FrmThietBi : Form
    {
        private readonly ThietBiService service;
        private readonly Action<Action<string>> initialize;
        private readonly DatabaseSetupService settings;
        private readonly GroupBox grpThongTin = new GroupBox();
        private readonly ComboBox cboLoaiThietBi = new ComboBox();
        private readonly TextBox txtMaTB = new TextBox();
        private readonly TextBox txtTenTB = new TextBox();
        private readonly DateTimePicker dtpNgayMua = new DateTimePicker();
        private readonly TextBox txtSoLuong = new TextBox();
        private readonly TextBox txtDonGia = new TextBox();
        private readonly DataGridView dgvThietBi = new DataGridView();
        private readonly Button btnMoi = new Button();
        private readonly Button btnNhap = new Button();
        private readonly Button btnSua = new Button();
        private readonly Button btnXoa = new Button();
        private readonly Button btnThoat = new Button();
        private readonly Button btnCauHinh = new Button();
        private readonly Button btnTaiLai = new Button();
        private readonly Label lblTrangThai = new Label();
        private readonly Timer timer = new Timer();
        private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");
        private string maDangChon;
        private bool dangGanDuLieu;
        private bool sanSang;
        private bool dangXuLy;
        private string thongBaoXuLy;
        private DateTime batDau;

        public FrmThietBi(ThietBiService service, Action<Action<string>> initialize, DatabaseSetupService settings)
        {
            this.service=service; this.initialize=initialize; this.settings=settings;
            TaoGiaoDien();
            LamMoi(); CapNhatNut();
            timer.Interval=1000;
            timer.Tick += (s,e) => { if(dangXuLy) lblTrangThai.Text=thongBaoXuLy+" ("+(int)(DateTime.Now-batDau).TotalSeconds+" giây)"; };
            Shown += async (s,e) => await NapDuLieuAsync();
            FormClosing += XacNhanThoat;
            FormClosed += (s,e) => { timer.Stop(); timer.Dispose(); };
        }
        private void TaoGiaoDien()
        {
            Text="Quản lý thiết bị – "+ConfigurationManager.AppSettings["MaSinhVien"]+" – "+ConfigurationManager.AppSettings["HoTenSinhVien"];
            StartPosition=FormStartPosition.CenterScreen;
            ClientSize=new Size(1120,720); MinimumSize=new Size(1040,650);
            Font=new Font("Segoe UI",10F); AutoScaleMode=AutoScaleMode.Font;
            BackColor=Color.FromArgb(245,247,251);
            var root=new TableLayoutPanel { Dock=DockStyle.Fill, Padding=new Padding(18), ColumnCount=2, RowCount=4 };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,145));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute,64));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute,230));
            root.RowStyles.Add(new RowStyle(SizeType.Percent,100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute,60));
            Controls.Add(root);
            var title=new Label { Text="QUẢN LÝ THIẾT BỊ PHÒNG HỌC", Dock=DockStyle.Fill, TextAlign=ContentAlignment.MiddleCenter,
                Font=new Font("Segoe UI",21,FontStyle.Bold), ForeColor=Color.FromArgb(29,65,132) };
            root.Controls.Add(title,0,0); root.SetColumnSpan(title,2);
            grpThongTin.Text="Thông tin thiết bị"; grpThongTin.Dock=DockStyle.Fill;
            grpThongTin.Padding=new Padding(12); grpThongTin.TabIndex=0;
            root.Controls.Add(grpThongTin,0,1);
            var fields=new TableLayoutPanel { Dock=DockStyle.Fill, ColumnCount=4, RowCount=3 };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,110));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,55));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,100));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,45));
            for(int i=0;i<3;i++) fields.RowStyles.Add(new RowStyle(SizeType.Percent,100F/3));
            grpThongTin.Controls.Add(fields);
            ThemTruong(fields,"Loại thiết bị",cboLoaiThietBi,0,0,0);
            ThemTruong(fields,"Mã thiết bị",txtMaTB,0,1,1);
            ThemTruong(fields,"Tên thiết bị",txtTenTB,0,2,2);
            ThemTruong(fields,"Ngày mua",dtpNgayMua,2,0,3);
            ThemTruong(fields,"Số lượng",txtSoLuong,2,1,4);
            ThemTruong(fields,"Đơn giá",txtDonGia,2,2,5);
            cboLoaiThietBi.Name="cboLoaiThietBi"; cboLoaiThietBi.DropDownStyle=ComboBoxStyle.DropDownList;
            txtMaTB.Name="txtMaTB"; txtMaTB.MaxLength=20;
            txtTenTB.Name="txtTenTB"; txtTenTB.MaxLength=150;
            dtpNgayMua.Name="dtpNgayMua"; dtpNgayMua.Format=DateTimePickerFormat.Custom;
            dtpNgayMua.CustomFormat="dd/MM/yyyy"; dtpNgayMua.MinDate=new DateTime(1753,1,1);
            txtSoLuong.Name="txtSoLuong"; txtSoLuong.MaxLength=16;
            txtDonGia.Name="txtDonGia"; txtDonGia.MaxLength=24;
            var buttons=new TableLayoutPanel { Dock=DockStyle.Fill, ColumnCount=1, RowCount=5, Padding=new Padding(10,10,0,0), TabIndex=1 };
            root.Controls.Add(buttons,1,1);
            Button[] list={btnMoi,btnNhap,btnSua,btnXoa,btnThoat};
            string[] labels={"Mới","Nhập","Sửa","Xóa","Thoát"};
            string[] names={"btnMoi","btnNhap","btnSua","btnXoa","btnThoat"};
            for(int i=0;i<list.Length;i++)
            {
                buttons.RowStyles.Add(new RowStyle(SizeType.Percent,20));
                list[i].Text=labels[i]; list[i].Name=names[i]; list[i].Dock=DockStyle.Fill; list[i].TabIndex=i;
                list[i].BackColor=Color.White; list[i].FlatStyle=FlatStyle.Flat;
                buttons.Controls.Add(list[i],0,i);
            }
            btnNhap.BackColor=Color.FromArgb(36,91,175); btnNhap.ForeColor=Color.White;
            btnMoi.Click += (s,e) => LamMoi();
            btnNhap.Click += async (s,e) => await LuuAsync(false);
            btnSua.Click += async (s,e) => await LuuAsync(true);
            btnXoa.Click += async (s,e) => await XoaAsync();
            btnThoat.Click += (s,e) => Close();
            AcceptButton=btnNhap; CancelButton=btnThoat;
            var groupList=new GroupBox { Text="Danh sách thiết bị", Dock=DockStyle.Fill, Padding=new Padding(10), TabIndex=2 };
            root.Controls.Add(groupList,0,2); root.SetColumnSpan(groupList,2);
            groupList.Controls.Add(dgvThietBi);
            dgvThietBi.Name="dgvThietBi"; dgvThietBi.Dock=DockStyle.Fill;
            dgvThietBi.AutoGenerateColumns=false; dgvThietBi.ReadOnly=true;
            dgvThietBi.AllowUserToAddRows=false; dgvThietBi.AllowUserToDeleteRows=false;
            dgvThietBi.MultiSelect=false; dgvThietBi.SelectionMode=DataGridViewSelectionMode.FullRowSelect;
            dgvThietBi.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
            dgvThietBi.BackgroundColor=Color.White; dgvThietBi.BorderStyle=BorderStyle.None;
            dgvThietBi.RowHeadersVisible=false; dgvThietBi.RowTemplate.Height=33;
            dgvThietBi.EnableHeadersVisualStyles=false; dgvThietBi.ColumnHeadersHeight=38;
            dgvThietBi.ColumnHeadersDefaultCellStyle.BackColor=Color.FromArgb(226,234,247);
            dgvThietBi.ColumnHeadersDefaultCellStyle.Font=new Font(Font,FontStyle.Bold);
            dgvThietBi.AlternatingRowsDefaultCellStyle.BackColor=Color.FromArgb(245,247,250);
            ThemCot("MaTB","Mã TB",75);
            ThemCot("TenTB","Tên thiết bị",200);
            ThemCot("NgayMua","Ngày mua",100,"dd/MM/yyyy");
            ThemCot("SoLuong","Số lượng",75,"N0");
            ThemCot("DonGia","Đơn giá",110,"N2");
            ThemCot("TenLoai","Loại thiết bị",140);
            dgvThietBi.SelectionChanged += (s,e) => ChonDong();
            var footer=new TableLayoutPanel { Dock=DockStyle.Fill, ColumnCount=3, RowCount=1, Padding=new Padding(0,8,0,0), TabIndex=3 };
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,145));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,100));
            root.Controls.Add(footer,0,3); root.SetColumnSpan(footer,2);
            lblTrangThai.Dock=DockStyle.Fill; lblTrangThai.TextAlign=ContentAlignment.MiddleLeft;
            lblTrangThai.Text="Đang chuẩn bị kết nối cơ sở dữ liệu…";
            btnCauHinh.Text="Cấu hình CSDL"; btnCauHinh.Dock=DockStyle.Fill;
            btnTaiLai.Text="Tải lại"; btnTaiLai.Dock=DockStyle.Fill;
            footer.Controls.Add(lblTrangThai,0,0); footer.Controls.Add(btnCauHinh,1,0); footer.Controls.Add(btnTaiLai,2,0);
            btnTaiLai.Click += async (s,e) => await NapDuLieuAsync();
            btnCauHinh.Click += async (s,e) =>
            {
                using(var dialog=new FrmDatabaseSettings(settings))
                    if(dialog.ShowDialog(this)==DialogResult.OK) await NapDuLieuAsync();
            };
        }
        private static void ThemTruong(TableLayoutPanel panel,string label,Control control,int col,int row,int tab)
        {
            panel.Controls.Add(new Label { Text=label+":", Dock=DockStyle.Fill, TextAlign=ContentAlignment.MiddleLeft },col,row);
            control.Anchor=AnchorStyles.Left|AnchorStyles.Right; control.Margin=new Padding(3,8,12,8); control.TabIndex=tab;
            panel.Controls.Add(control,col+1,row);
        }
        private void ThemCot(string property,string label,int width,string format=null)
        {
            var column=new DataGridViewTextBoxColumn { Name=property, DataPropertyName=property, HeaderText=label, FillWeight=width, SortMode=DataGridViewColumnSortMode.NotSortable };
            if(format!=null) { column.DefaultCellStyle.Format=format; column.DefaultCellStyle.FormatProvider=Vi; }
            if(property=="SoLuong" || property=="DonGia") column.DefaultCellStyle.Alignment=DataGridViewContentAlignment.MiddleRight;
            dgvThietBi.Columns.Add(column);
        }
        private void CapNhatNut()
        {
            bool enabled=sanSang && !dangXuLy;
            grpThongTin.Enabled=dgvThietBi.Enabled=enabled;
            btnMoi.Enabled=enabled;
            btnNhap.Enabled=enabled && maDangChon==null;
            btnSua.Enabled=btnXoa.Enabled=enabled && maDangChon!=null;
            btnTaiLai.Enabled=btnCauHinh.Enabled=!dangXuLy;
        }
        private void BatDauXuLy(string message)
        {
            dangXuLy=true; thongBaoXuLy=message; batDau=DateTime.Now;
            lblTrangThai.Text=message; timer.Start(); CapNhatNut();
        }
        private void KetThucXuLy()
        {
            dangXuLy=false;
            if(IsDisposed || Disposing) return;
            timer.Stop(); CapNhatNut();
        }
        private async Task NapDuLieuAsync()
        {
            if(dangXuLy) return;
            sanSang=false; BatDauXuLy("Đang kết nối SQL Server…");
            var progress=new Progress<string>(message =>
            {
                if(!IsDisposed && !Disposing && dangXuLy) { thongBaoXuLy=message; lblTrangThai.Text=message; }
            });
            try
            {
                var data=await Task.Run(() =>
                {
                    initialize(message => ((IProgress<string>)progress).Report(message));
                    return Tuple.Create(service.LayLoaiThietBi(), service.LayDanhSach());
                });
                if(IsDisposed || Disposing) return;
                dangGanDuLieu=true;
                try
                {
                    cboLoaiThietBi.DisplayMember=nameof(LoaiThietBi.TenLoai);
                    cboLoaiThietBi.ValueMember=nameof(LoaiThietBi.MaLoai);
                    cboLoaiThietBi.DataSource=data.Item1;
                }
                finally { dangGanDuLieu=false; }
                GanDanhSach(data.Item2); sanSang=true; LamMoi();
            }
            catch(Exception ex)
            {
                if(IsDisposed || Disposing) return;
                lblTrangThai.Text="Chưa nạp được dữ liệu. Bấm Cấu hình CSDL rồi Tải lại.";
                HienLoi("Không nạp được loại thiết bị và danh sách. Kiểm tra SQL Server/LocalDB và cấu hình kết nối.",ex);
            }
            finally
            {
                KetThucXuLy();
                if(!IsDisposed && !Disposing && sanSang && maDangChon==null) txtMaTB.Focus();
            }
        }
        private void GanDanhSach(List<ThietBiRow> rows)
        {
            dangGanDuLieu=true;
            try
            {
                maDangChon=null; dgvThietBi.DataSource=rows;
                dgvThietBi.ClearSelection(); dgvThietBi.CurrentCell=null;
                lblTrangThai.Text=$"Có {rows.Count} thiết bị. Chọn dòng để sửa/xóa; nhấn Mới để nhập thiết bị mới.";
            }
            finally { dangGanDuLieu=false; }
        }
        private void LamMoi()
        {
            dangGanDuLieu=true;
            try
            {
                maDangChon=null; txtMaTB.Clear(); txtTenTB.Clear(); txtSoLuong.Clear(); txtDonGia.Clear();
                dtpNgayMua.Value=DateTime.Today; cboLoaiThietBi.SelectedIndex=-1;
                txtMaTB.ReadOnly=false; dgvThietBi.ClearSelection(); dgvThietBi.CurrentCell=null;
            }
            finally { dangGanDuLieu=false; }
            CapNhatNut();
            if(!dangXuLy) txtMaTB.Focus();
        }
        private void ChonDong()
        {
            if(dangGanDuLieu || dangXuLy || !sanSang || dgvThietBi.SelectedRows.Count!=1) return;
            var row=dgvThietBi.SelectedRows[0].DataBoundItem as ThietBiRow;
            if(row==null) return;
            maDangChon=row.MaTB; txtMaTB.Text=row.MaTB; txtTenTB.Text=row.TenTB;
            dtpNgayMua.Value=row.NgayMua; txtSoLuong.Text=row.SoLuong.ToString(Vi);
            txtDonGia.Text=row.DonGia.ToString("0.##",Vi); cboLoaiThietBi.SelectedValue=row.MaLoai;
            txtMaTB.ReadOnly=true; CapNhatNut();
        }
        private ThietBiInput DocDuLieu() => new ThietBiInput
        {
            MaTB=txtMaTB.Text, TenTB=txtTenTB.Text, NgayMua=dtpNgayMua.Value,
            SoLuong=txtSoLuong.Text, DonGia=txtDonGia.Text, MaLoai=cboLoaiThietBi.SelectedValue as string
        };
        private async Task LuuAsync(bool sua)
        {
            if(dangXuLy || !sanSang || (sua && maDangChon==null)) return;
            var input=DocDuLieu(); var maGoc=maDangChon;
            await ThucHienGhiAsync(() => { if(sua) service.Sua(maGoc,input); else service.Nhap(input); },
                sua ? "Đã cập nhật thiết bị." : "Đã nhập thiết bị.");
        }
        private async Task XoaAsync()
        {
            if(dangXuLy || !sanSang || maDangChon==null) return;
            var ma=maDangChon;
            if(MessageBox.Show(this,$"Xóa thiết bị {ma} – {txtTenTB.Text}?","Xác nhận xóa",MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)!=DialogResult.Yes) return;
            await ThucHienGhiAsync(() => service.Xoa(ma),"Đã xóa thiết bị.");
        }
        private async Task ThucHienGhiAsync(Action write,string message)
        {
            bool daLuu=false;
            BatDauXuLy("Đang xử lý dữ liệu…");
            try
            {
                await Task.Run(write); daLuu=true;
                var rows=await Task.Run(() => service.LayDanhSach());
                if(IsDisposed || Disposing) return;
                GanDanhSach(rows); LamMoi(); timer.Stop();
                MessageBox.Show(this,message,"Thành công",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch(BusinessException ex)
            {
                if(!IsDisposed && !Disposing)
                {
                    timer.Stop();
                    lblTrangThai.Text="Dữ liệu chưa hợp lệ; thay đổi chưa được lưu.";
                    MessageBox.Show(this,ex.Message,"Kiểm tra dữ liệu",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
            }
            catch(Exception ex)
            {
                if(IsDisposed || Disposing) return;
                if(daLuu)
                {
                    sanSang=false; lblTrangThai.Text="Đã lưu nhưng chưa tải lại được. Bấm Tải lại.";
                    HienLoi("Thay đổi đã lưu vào database nhưng tải lại danh sách thất bại. Không nhập lại để tránh trùng dữ liệu.",ex);
                }
                else
                {
                    lblTrangThai.Text="Thao tác chưa được xác nhận lưu thành công.";
                    HienLoi("Không hoàn tất được thao tác. Kiểm tra kết nối rồi tải lại dữ liệu trước khi thử lại.",ex);
                }
            }
            finally
            {
                KetThucXuLy();
                if(!IsDisposed && !Disposing && sanSang && maDangChon==null) txtMaTB.Focus();
            }
        }
        private void HienLoi(string message,Exception ex)
        {
            timer.Stop();
            MessageBox.Show(this,message+Environment.NewLine+Environment.NewLine+ex.Message,
                "Lỗi",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }
        private void XacNhanThoat(object sender,FormClosingEventArgs e)
        {
            if(MessageBox.Show(this,"Bạn có muốn thoát chương trình?","Xác nhận thoát",MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)!=DialogResult.Yes) e.Cancel=true;
        }
    }
}
