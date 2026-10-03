using System.Drawing;

namespace QuanLySinhVien_;

public sealed class MainForm : Form
{
    private readonly List<LopHoc> _dsLop = new();
    private readonly List<SinhVien> _dsSinhVien = new();

    private TextBox txtMaSV = null!;
    private TextBox txtHoTen = null!;
    private DateTimePicker dtpNgaySinh = null!;
    private RadioButton rdoNam = null!;
    private RadioButton rdoNu = null!;
    private TextBox txtEmail = null!;
    private TextBox txtDienThoai = null!;
    private ComboBox cboLopHoc = null!;
    private NumericUpDown nudDiem = null!;
    private ComboBox cboTrangThai = null!;
    private Button btnThem = null!;
    private Button btnSua = null!;
    private Button btnXoa = null!;
    private Button btnLamMoi = null!;
    private TextBox txtTuKhoa = null!;
    private ComboBox cboLocLop = null!;
    private NumericUpDown nudDiemTu = null!;
    private Button btnTimKiem = null!;
    private Button btnHienThiTatCa = null!;
    private DataGridView dgvSinhVien = null!;
    private Label lblTongSo = null!;

    public MainForm()
    {
        KhoiTaoGiaoDien();
        DangKySuKien();
    }

    private void DangKySuKien()
    {
        Load += MainForm_Load;
        txtMaSV.Leave += TxtMaSV_Leave;
        btnThem.Click += BtnThem_Click;
        btnSua.Click += BtnSua_Click;
        btnXoa.Click += BtnXoa_Click;
        btnLamMoi.Click += (_, _) => LamMoiForm();
        dgvSinhVien.CellClick += DgvSinhVien_CellClick;
        btnTimKiem.Click += (_, _) => TimKiem();
        btnHienThiTatCa.Click += (_, _) => HienThiTatCa();
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        KhoiTaoDuLieuMau();

        cboLopHoc.DataSource = _dsLop.ToList();
        cboLopHoc.DisplayMember = nameof(LopHoc.TenLop);
        cboLopHoc.ValueMember = nameof(LopHoc.MaLop);

        List<LopHoc> danhSachLoc = new()
        {
            new LopHoc { MaLop = string.Empty, TenLop = "Tất cả lớp" }
        };
        danhSachLoc.AddRange(_dsLop);
        cboLocLop.DataSource = danhSachLoc;
        cboLocLop.DisplayMember = nameof(LopHoc.TenLop);
        cboLocLop.ValueMember = nameof(LopHoc.MaLop);

        cboTrangThai.Items.AddRange(new object[]
        {
            "Đang học", "Bảo lưu", "Đã tốt nghiệp"
        });

        HienThiDanhSach(_dsSinhVien);
        LamMoiForm();
    }

    private void KhoiTaoDuLieuMau()
    {
        _dsLop.Clear();
        _dsSinhVien.Clear();

        LopHoc lop1 = new() { MaLop = "KTPM01", TenLop = "Kỹ thuật phần mềm 01" };
        LopHoc lop2 = new() { MaLop = "AI01", TenLop = "Trí tuệ nhân tạo 01" };
        LopHoc lop3 = new() { MaLop = "KHDL01", TenLop = "Khoa học dữ liệu 01" };
        _dsLop.AddRange(new[] { lop1, lop2, lop3 });

        ThemSinhVienMau("SV000123", "Nguyễn Văn An", new DateTime(2006, 8, 15),
            "Nam", "an.nv@vju.ac.vn", "0912345678", 8.5m, "Đang học", lop1);
        ThemSinhVienMau("SV000124", "Trần Minh Anh", new DateTime(2006, 1, 22),
            "Nữ", "anh.tm@vju.ac.vn", "0987654321", 9.0m, "Đang học", lop2);
        ThemSinhVienMau("SV000125", "Lê Hoàng Bình", new DateTime(2006, 5, 9),
            "Nam", "binh.lh@vju.ac.vn", "0355556677", 7.4m, "Đang học", lop1);
        ThemSinhVienMau("SV000126", "Đỗ Thị Hồng", new DateTime(2006, 11, 30),
            "Nữ", "hong.dt@vju.ac.vn", "0777888999", 8.1m, "Đang học", lop3);
    }

    private void ThemSinhVienMau(string maSV, string hoTen, DateTime ngaySinh,
        string gioiTinh, string email, string dienThoai, decimal diem,
        string trangThai, LopHoc lop)
    {
        SinhVien sv = new()
        {
            MaSV = maSV,
            HoTen = hoTen,
            NgaySinh = ngaySinh,
            GioiTinh = gioiTinh,
            Email = email,
            DienThoai = dienThoai,
            Diem = diem,
            TrangThai = trangThai,
            MaLop = lop.MaLop,
            LopHoc = lop
        };
        _dsSinhVien.Add(sv);
        lop.DanhSachSinhVien.Add(sv);
    }

    private void TxtMaSV_Leave(object? sender, EventArgs e)
    {
        string maSV = txtMaSV.Text.Trim().ToUpperInvariant();
        txtMaSV.Text = maSV;
        if (maSV.Length == 0) return;

        SinhVien? sv = TimTheoMa(maSV);
        if (sv != null)
        {
            HienThiSinhVienLenForm(sv);
            ChuyenTrangThaiDangSua();
        }
        else
        {
            XoaThongTinChiTiet();
            txtMaSV.Text = maSV;
            txtMaSV.ReadOnly = false;
            btnThem.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }
    }

    private void BtnThem_Click(object? sender, EventArgs e)
    {
        SinhVien sv = DocSinhVienTuForm();
        if (!KiemTraSinhVien(sv)) return;
        if (TimTheoMa(sv.MaSV) != null)
        {
            ThongBaoCanhBao("Mã sinh viên đã tồn tại.");
            txtMaSV.Focus();
            return;
        }

        _dsSinhVien.Add(sv);
        sv.LopHoc!.DanhSachSinhVien.Add(sv);
        HienThiDanhSach(_dsSinhVien);
        MessageBox.Show("Thêm sinh viên thành công.", "Thông báo",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        LamMoiForm();
    }

    private void BtnSua_Click(object? sender, EventArgs e)
    {
        SinhVien? svCu = TimTheoMa(txtMaSV.Text.Trim());
        if (svCu == null)
        {
            ThongBaoCanhBao("Không tìm thấy sinh viên cần sửa.");
            return;
        }

        SinhVien svMoi = DocSinhVienTuForm();
        if (!KiemTraSinhVien(svMoi)) return;

        DialogResult xacNhan = MessageBox.Show(
            $"Bạn có chắc muốn sửa thông tin của {svCu.HoTen}?",
            "Xác nhận sửa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (xacNhan != DialogResult.Yes) return;

        if (svCu.LopHoc != svMoi.LopHoc)
        {
            svCu.LopHoc?.DanhSachSinhVien.Remove(svCu);
            svMoi.LopHoc!.DanhSachSinhVien.Add(svCu);
        }

        svCu.HoTen = svMoi.HoTen;
        svCu.NgaySinh = svMoi.NgaySinh;
        svCu.GioiTinh = svMoi.GioiTinh;
        svCu.Email = svMoi.Email;
        svCu.DienThoai = svMoi.DienThoai;
        svCu.Diem = svMoi.Diem;
        svCu.TrangThai = svMoi.TrangThai;
        svCu.MaLop = svMoi.MaLop;
        svCu.LopHoc = svMoi.LopHoc;

        HienThiDanhSach(_dsSinhVien);
        MessageBox.Show("Sửa sinh viên thành công.", "Thông báo",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        LamMoiForm();
    }

    private void BtnXoa_Click(object? sender, EventArgs e)
    {
        SinhVien? sv = TimTheoMa(txtMaSV.Text.Trim());
        if (sv == null)
        {
            ThongBaoCanhBao("Không tìm thấy sinh viên cần xóa.");
            return;
        }

        DialogResult xacNhan = MessageBox.Show(
            $"Bạn có chắc muốn xóa sinh viên {sv.HoTen} ({sv.MaSV})?",
            "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (xacNhan != DialogResult.Yes) return;

        sv.LopHoc?.DanhSachSinhVien.Remove(sv);
        _dsSinhVien.Remove(sv);
        HienThiDanhSach(_dsSinhVien);
        MessageBox.Show("Xóa sinh viên thành công.", "Thông báo",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        LamMoiForm();
    }

    private void DgvSinhVien_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        object? giaTri = dgvSinhVien.Rows[e.RowIndex].Cells[nameof(SinhVienView.MaSV)].Value;
        if (giaTri == null) return;

        SinhVien? sv = TimTheoMa(giaTri.ToString() ?? string.Empty);
        if (sv == null) return;
        HienThiSinhVienLenForm(sv);
        ChuyenTrangThaiDangSua();
    }

    private SinhVien DocSinhVienTuForm()
    {
        LopHoc? lop = cboLopHoc.SelectedItem as LopHoc;
        return new SinhVien
        {
            MaSV = txtMaSV.Text.Trim().ToUpperInvariant(),
            HoTen = txtHoTen.Text.Trim(),
            NgaySinh = dtpNgaySinh.Value.Date,
            GioiTinh = rdoNam.Checked ? "Nam" : rdoNu.Checked ? "Nữ" : string.Empty,
            Email = txtEmail.Text.Trim(),
            DienThoai = txtDienThoai.Text.Trim(),
            Diem = nudDiem.Value,
            TrangThai = cboTrangThai.Text,
            MaLop = lop?.MaLop ?? string.Empty,
            LopHoc = lop
        };
    }

    private bool KiemTraSinhVien(SinhVien sv)
    {
        if (sv.KiemTraHopLe(out List<string> loi)) return true;
        MessageBox.Show(string.Join(Environment.NewLine, loi), "Dữ liệu không hợp lệ",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
    }

    private SinhVien? TimTheoMa(string maSV) =>
        _dsSinhVien.FirstOrDefault(sv =>
            sv.MaSV.Equals(maSV, StringComparison.OrdinalIgnoreCase));

    private void HienThiSinhVienLenForm(SinhVien sv)
    {
        txtMaSV.Text = sv.MaSV;
        txtHoTen.Text = sv.HoTen;
        dtpNgaySinh.Value = sv.NgaySinh;
        rdoNam.Checked = sv.GioiTinh == "Nam";
        rdoNu.Checked = sv.GioiTinh == "Nữ";
        txtEmail.Text = sv.Email;
        txtDienThoai.Text = sv.DienThoai;
        nudDiem.Value = Math.Clamp(sv.Diem, nudDiem.Minimum, nudDiem.Maximum);
        cboLopHoc.SelectedValue = sv.MaLop;
        cboTrangThai.SelectedItem = sv.TrangThai;
    }

    private void ChuyenTrangThaiDangSua()
    {
        txtMaSV.ReadOnly = true;
        btnThem.Enabled = false;
        btnSua.Enabled = true;
        btnXoa.Enabled = true;
    }

    private void LamMoiForm()
    {
        txtMaSV.ReadOnly = false;
        txtMaSV.Clear();
        XoaThongTinChiTiet();
        btnThem.Enabled = true;
        btnSua.Enabled = false;
        btnXoa.Enabled = false;
        dgvSinhVien.ClearSelection();
        BeginInvoke(() => txtMaSV.Focus());
    }

    private void XoaThongTinChiTiet()
    {
        txtHoTen.Clear();
        dtpNgaySinh.Value = DateTime.Today.AddYears(-18);
        rdoNam.Checked = true;
        rdoNu.Checked = false;
        txtEmail.Clear();
        txtDienThoai.Clear();
        nudDiem.Value = 0;
        if (cboLopHoc.Items.Count > 0) cboLopHoc.SelectedIndex = 0;
        if (cboTrangThai.Items.Count > 0) cboTrangThai.SelectedIndex = 0;
    }

    private void TimKiem()
    {
        string tuKhoa = txtTuKhoa.Text.Trim();
        string maLop = (cboLocLop.SelectedItem as LopHoc)?.MaLop ?? string.Empty;
        decimal diemTu = nudDiemTu.Value;

        IEnumerable<SinhVien> ketQua = _dsSinhVien.Where(sv =>
            (tuKhoa.Length == 0
             || sv.MaSV.Contains(tuKhoa, StringComparison.CurrentCultureIgnoreCase)
             || sv.HoTen.Contains(tuKhoa, StringComparison.CurrentCultureIgnoreCase)
             || sv.Email.Contains(tuKhoa, StringComparison.CurrentCultureIgnoreCase)
             || sv.DienThoai.Contains(tuKhoa, StringComparison.CurrentCultureIgnoreCase))
            && (maLop.Length == 0 || sv.MaLop == maLop)
            && sv.Diem >= diemTu);

        HienThiDanhSach(ketQua);
    }

    private void HienThiTatCa()
    {
        txtTuKhoa.Clear();
        nudDiemTu.Value = 0;
        if (cboLocLop.Items.Count > 0) cboLocLop.SelectedIndex = 0;
        HienThiDanhSach(_dsSinhVien);
    }

    private void HienThiDanhSach(IEnumerable<SinhVien> danhSach)
    {
        List<SinhVienView> duLieu = danhSach.Select(sv => new SinhVienView
        {
            MaSV = sv.MaSV,
            HoTen = sv.HoTen,
            NgaySinh = sv.NgaySinh.ToString("dd/MM/yyyy"),
            GioiTinh = sv.GioiTinh,
            Email = sv.Email,
            DienThoai = sv.DienThoai,
            Diem = sv.Diem,
            Lop = sv.LopHoc?.TenLop ?? string.Empty,
            TrangThai = sv.TrangThai
        }).ToList();

        dgvSinhVien.DataSource = null;
        dgvSinhVien.DataSource = duLieu;
        DatTieuDeCot();
        dgvSinhVien.ClearSelection();
        lblTongSo.Text = $"Tổng số: {duLieu.Count} sinh viên";
    }

    private void DatTieuDeCot()
    {
        if (dgvSinhVien.Columns.Count == 0) return;
        dgvSinhVien.Columns[nameof(SinhVienView.MaSV)].HeaderText = "Mã SV";
        dgvSinhVien.Columns[nameof(SinhVienView.HoTen)].HeaderText = "Họ và tên";
        dgvSinhVien.Columns[nameof(SinhVienView.NgaySinh)].HeaderText = "Ngày sinh";
        dgvSinhVien.Columns[nameof(SinhVienView.GioiTinh)].HeaderText = "Giới tính";
        dgvSinhVien.Columns[nameof(SinhVienView.DienThoai)].HeaderText = "Điện thoại";
        dgvSinhVien.Columns[nameof(SinhVienView.Diem)].HeaderText = "Điểm";
        dgvSinhVien.Columns[nameof(SinhVienView.Lop)].HeaderText = "Lớp";
        dgvSinhVien.Columns[nameof(SinhVienView.TrangThai)].HeaderText = "Trạng thái";
    }

    private static void ThongBaoCanhBao(string noiDung) =>
        MessageBox.Show(noiDung, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private void KhoiTaoGiaoDien()
    {
        Text = "Ứng dụng quản lý sinh viên";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1380, 820);
        MinimumSize = new Size(1100, 700);
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(244, 247, 251);

        TableLayoutPanel root = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(22, 12, 22, 18)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 260));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 82));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(root);

        Label tieuDe = new()
        {
            Text = "QUẢN LÝ SINH VIÊN",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 21F, FontStyle.Bold),
            ForeColor = Color.FromArgb(28, 68, 106)
        };
        root.Controls.Add(tieuDe, 0, 0);

        GroupBox khungThongTin = TaoKhung("Thông tin sinh viên");
        root.Controls.Add(khungThongTin, 0, 1);
        TableLayoutPanel form = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 6,
            RowCount = 4,
            Padding = new Padding(10, 12, 10, 5)
        };
        float[] rongCot = { 11, 22, 11, 22, 11, 23 };
        foreach (float rong in rongCot) form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, rong));
        form.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        form.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        form.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        form.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        khungThongTin.Controls.Add(form);

        txtMaSV = TaoTextBox(0);
        txtHoTen = TaoTextBox(1);
        cboLopHoc = TaoComboBox(2);
        dtpNgaySinh = new DateTimePicker
        {
            Dock = DockStyle.Fill, Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy", TabIndex = 3, Margin = new Padding(5, 7, 12, 7)
        };
        rdoNam = new RadioButton { Text = "Nam", Checked = true, AutoSize = true, TabIndex = 4 };
        rdoNu = new RadioButton { Text = "Nữ", AutoSize = true, TabIndex = 5 };
        FlowLayoutPanel gioiTinh = new() { Dock = DockStyle.Fill, Padding = new Padding(4, 8, 0, 0) };
        gioiTinh.Controls.AddRange(new Control[] { rdoNam, rdoNu });
        nudDiem = TaoSo(6);
        txtEmail = TaoTextBox(7);
        txtDienThoai = TaoTextBox(8);
        cboTrangThai = TaoComboBox(9);

        ThemTruong(form, "Mã sinh viên *", txtMaSV, 0, 0);
        ThemTruong(form, "Họ và tên *", txtHoTen, 2, 0);
        ThemTruong(form, "Lớp học *", cboLopHoc, 4, 0);
        ThemTruong(form, "Ngày sinh", dtpNgaySinh, 0, 1);
        ThemTruong(form, "Giới tính", gioiTinh, 2, 1);
        ThemTruong(form, "Điểm *", nudDiem, 4, 1);
        ThemTruong(form, "Email *", txtEmail, 0, 2);
        ThemTruong(form, "Điện thoại *", txtDienThoai, 2, 2);
        ThemTruong(form, "Trạng thái", cboTrangThai, 4, 2);

        btnThem = TaoNut("Thêm", Color.FromArgb(45, 145, 108), 10);
        btnSua = TaoNut("Sửa", Color.FromArgb(43, 108, 170), 11);
        btnXoa = TaoNut("Xóa", Color.FromArgb(201, 75, 88), 12);
        btnLamMoi = TaoNut("Làm mới", Color.FromArgb(106, 121, 139), 13);
        FlowLayoutPanel nut = new()
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false, Padding = new Padding(0, 3, 8, 0)
        };
        nut.Controls.AddRange(new Control[] { btnLamMoi, btnXoa, btnSua, btnThem });
        form.Controls.Add(nut, 0, 3);
        form.SetColumnSpan(nut, 6);

        GroupBox khungTim = TaoKhung(string.Empty);
        root.Controls.Add(khungTim, 0, 2);
        FlowLayoutPanel tim = new()
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false, Padding = new Padding(10, 13, 5, 5), AutoScroll = true
        };
        khungTim.Controls.Add(tim);
        txtTuKhoa = new TextBox { Width = 275, PlaceholderText = "Mã, họ tên, email hoặc điện thoại", Margin = new Padding(4, 7, 18, 4) };
        cboLocLop = new ComboBox { Width = 230, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(4, 7, 18, 4) };
        nudDiemTu = TaoSo(0); nudDiemTu.Width = 95; nudDiemTu.Dock = DockStyle.None;
        btnTimKiem = TaoNut("Tìm kiếm", Color.FromArgb(43, 108, 170), 0);
        btnHienThiTatCa = TaoNut("Hiển thị tất cả", Color.FromArgb(93, 113, 134), 0); btnHienThiTatCa.Width = 145;
        tim.Controls.AddRange(new Control[]
        {
            TaoNhanNgang("Từ khóa"), txtTuKhoa, TaoNhanNgang("Lớp"), cboLocLop,
            TaoNhanNgang("Điểm từ"), nudDiemTu, btnTimKiem, btnHienThiTatCa
        });

        GroupBox khungDanhSach = TaoKhung("Danh sách sinh viên");
        root.Controls.Add(khungDanhSach, 0, 3);
        TableLayoutPanel bangLayout = new() { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Padding = new Padding(8) };
        bangLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        bangLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        khungDanhSach.Controls.Add(bangLayout);
        lblTongSo = new Label
        {
            Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(43, 65, 82)
        };
        bangLayout.Controls.Add(lblTongSo, 0, 0);
        dgvSinhVien = new DataGridView
        {
            Dock = DockStyle.Fill, ReadOnly = true, MultiSelect = false,
            AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false, BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None, AutoGenerateColumns = true
        };
        dgvSinhVien.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(226, 237, 246);
        dgvSinhVien.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        dgvSinhVien.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 232, 247);
        dgvSinhVien.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 55, 75);
        dgvSinhVien.EnableHeadersVisualStyles = false;
        dgvSinhVien.RowTemplate.Height = 34;
        bangLayout.Controls.Add(dgvSinhVien, 0, 1);
    }

    private static GroupBox TaoKhung(string text) => new()
    {
        Text = text, Dock = DockStyle.Fill, Padding = new Padding(10),
        Font = new Font("Segoe UI", 11F, FontStyle.Bold), BackColor = Color.White
    };

    private static TextBox TaoTextBox(int tabIndex) => new()
    {
        Dock = DockStyle.Fill, TabIndex = tabIndex, Margin = new Padding(5, 7, 12, 7)
    };

    private static ComboBox TaoComboBox(int tabIndex) => new()
    {
        Dock = DockStyle.Fill, TabIndex = tabIndex, DropDownStyle = ComboBoxStyle.DropDownList,
        Margin = new Padding(5, 7, 12, 7)
    };

    private static NumericUpDown TaoSo(int tabIndex) => new()
    {
        Dock = DockStyle.Fill, TabIndex = tabIndex, Minimum = 0, Maximum = 10,
        DecimalPlaces = 1, Increment = 0.1m, Margin = new Padding(5, 7, 12, 7)
    };

    private static Label TaoNhan(string text) => new()
    {
        Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight,
        Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
    };

    private static Label TaoNhanNgang(string text) => new()
    {
        Text = text, AutoSize = true, Margin = new Padding(8, 10, 4, 0),
        Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
    };

    private static void ThemTruong(TableLayoutPanel layout, string nhan, Control control, int cot, int dong)
    {
        layout.Controls.Add(TaoNhan(nhan), cot, dong);
        layout.Controls.Add(control, cot + 1, dong);
    }

    private static Button TaoNut(string text, Color mau, int tabIndex) => new()
    {
        Text = text, Width = 105, Height = 38, BackColor = mau, ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, TabIndex = tabIndex,
        Margin = new Padding(5), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
    };

    private sealed class SinhVienView
    {
        public string MaSV { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string NgaySinh { get; set; } = string.Empty;
        public string GioiTinh { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
        public decimal Diem { get; set; }
        public string Lop { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
    }
}
