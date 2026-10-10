using System.Drawing;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using QuanLySinhVien.Business;
using QuanLySinhVien.Entity;
namespace QuanLySinhVien.Presentation;

// Chỉ tạo control, đọc/hiển thị dữ liệu, bắt sự kiện và gọi Business.
// Không chứa DataList, không truy cập DAL và không tự thêm/sửa/xóa Entity.
public sealed class MainForm : Form
{
    private readonly SinhVienBUL svBUL;
    private readonly LopHocBUL lopBUL;
    private readonly ErrorProvider errors;
    private List<SinhVienView> duLieuDaTai = new();
    private bool dangNapCombo;
    private string? maDaTraCuu;
    private RadioButton rdoTimNguon = null!;
    private RadioButton rdoTimGiaoDien = null!;
    private TextBox txtMaSV = null!;
    private TextBox txtHoTen = null!;
    private DateTimePicker dtpNgaySinh = null!;
    private RadioButton rdoNam = null!;
    private RadioButton rdoNu = null!;
    private TextBox txtEmail = null!;
    private TextBox txtDienThoai = null!;
    private ComboBox cboLopHoc = null!;
    private TextBox txtDiem = null!;
    private ComboBox cboTrangThai = null!;
    private Button btnThem = null!;
    private Button btnSua = null!;
    private Button btnXoa = null!;
    private Button btnLamMoi = null!;
    private Button btnDong = null!;
    private TextBox txtTuKhoa = null!;
    private ComboBox cboLocLop = null!;
    private NumericUpDown nudDiemTu = null!;
    private Button btnTimKiem = null!;
    private Button btnHienThiTatCa = null!;
    private DataGridView dgvSinhVien = null!;
    private Label lblTongSo = null!;


    public MainForm(SinhVienBUL svBUL, LopHocBUL lopBUL)
    {
        this.svBUL = svBUL; this.lopBUL = lopBUL;
        KhoiTaoGiaoDien();
        errors = new ErrorProvider { ContainerControl = this, BlinkStyle = ErrorBlinkStyle.NeverBlink };
        DangKySuKien();
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing) errors?.Dispose();
        base.Dispose(disposing);
    }
    private void DangKySuKien()
    {
        Load += (_, _) => { NapComboLop(); cboTrangThai.Items.AddRange(new object[] { "Đang học", "Bảo lưu", "Đã tốt nghiệp" }); TaiDanhSachLop(); LamMoiForm(); };
        txtMaSV.Leave += (_, _) => TraCuuMa();
        txtMaSV.Enter += (_, _) => txtMaSV.SelectAll();
        txtMaSV.KeyDown += (_, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            SelectNextControl(txtMaSV, true, true, true, true);
        };
        btnDong.Click += (_, _) => Close();
        FormClosing += (_, e) =>
        {
            if (e.CloseReason != CloseReason.UserClosing) return;
            e.Cancel = MessageBox.Show("Bạn có muốn thoát không? Dữ liệu DataList của lần chạy này sẽ không được lưu.",
                "Xác nhận đóng", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes;
        };
        btnThem.Click += (_, _) => LuuSinhVien(false);
        btnSua.Click += (_, _) => LuuSinhVien(true);
        btnXoa.Click += (_, _) => XoaSinhVien();
        btnLamMoi.Click += (_, _) => LamMoiForm();
        btnTimKiem.Click += (_, _) => TimKiem();
        btnHienThiTatCa.Click += (_, _) => HienThiTatCa();
        dgvSinhVien.CellClick += (_, e) =>
        {
            if (e.RowIndex < 0 || dgvSinhVien.Rows[e.RowIndex].DataBoundItem is not SinhVienView row) return;
            var sv = svBUL.GetSinhVienByMaSV(row.MaSV);
            if (sv != null) HienThiSinhVienLenForm(sv);
        };
        cboLocLop.SelectedIndexChanged += (_, _) => { if (!dangNapCombo) TaiDanhSachLop(); };
        // SelectedIndexChanged như mẫu, có guard khi đang nạp DataSource.
        // Chọn lớp bằng tay hoặc khi tra cứu sinh viên đều đồng bộ danh sách lớp.
        cboLopHoc.SelectedIndexChanged += (_, _) =>
        {
            if (!dangNapCombo && cboLopHoc.SelectedValue is string ma && ma.Length > 0)
                cboLocLop.SelectedValue = ma;
        };
        txtMaSV.TextChanged += (_, _) => errors.SetError(txtMaSV, "");
        txtHoTen.TextChanged += (_, _) => errors.SetError(txtHoTen, "");
        txtEmail.TextChanged += (_, _) => errors.SetError(txtEmail, "");
        txtDienThoai.TextChanged += (_, _) => errors.SetError(txtDienThoai, "");
        txtDiem.TextChanged += (_, _) => errors.SetError(txtDiem, "");
        dtpNgaySinh.ValueChanged += (_, _) => errors.SetError(dtpNgaySinh, "");
        rdoNam.CheckedChanged += (_, _) => errors.SetError(rdoNu, "");
        rdoNu.CheckedChanged += (_, _) => errors.SetError(rdoNu, "");
        cboLopHoc.SelectedIndexChanged += (_, _) => errors.SetError(cboLopHoc, "");
        cboTrangThai.SelectedIndexChanged += (_, _) => errors.SetError(cboTrangThai, "");
        dtpNgaySinh.ShowCheckBox = true;
        txtDiem.PlaceholderText = "0–10";
        txtTuKhoa.TabIndex = 0; cboLocLop.TabIndex = 1; nudDiemTu.TabIndex = 2;
        btnTimKiem.TabIndex = 3; btnHienThiTatCa.TabIndex = 4;
    }
    private void NapComboLop()
    {
        string selectedFilter = MaLopLoc();
        string? selectedEdit = cboLopHoc.SelectedValue as string;
        dangNapCombo = true;
        try
        {
            var lops = lopBUL.GetAllLopHoc();
            cboLopHoc.DisplayMember = nameof(LopHoc.TenLop); cboLopHoc.ValueMember = nameof(LopHoc.MaLop);
            cboLopHoc.DataSource = lops;
            cboLopHoc.SelectedIndex = -1;
            if (selectedEdit != null && lops.Any(l => l.MaLop == selectedEdit)) cboLopHoc.SelectedValue = selectedEdit;
            var filters = new List<LopHoc> { new() { MaLop = "", TenLop = "Tất cả lớp" } };
            filters.AddRange(lops);
            cboLocLop.DisplayMember = nameof(LopHoc.TenLop); cboLocLop.ValueMember = nameof(LopHoc.MaLop);
            cboLocLop.DataSource = filters;
            cboLocLop.SelectedValue = filters.Any(l => l.MaLop == selectedFilter) ? selectedFilter : "";
        }
        finally { dangNapCombo = false; }
    }
    private string MaLopLoc() => (cboLocLop.SelectedItem as LopHoc)?.MaLop ?? "";
    private void TaiDanhSachLop()
    {
        string ma = MaLopLoc();
        var list = ma.Length == 0 ? svBUL.GetAllSinhVien() : svBUL.GetSinhVienByMaLop(ma);
        duLieuDaTai = list.Select(ToView).ToList();
        HienThiDanhSach(duLieuDaTai);
    }
    private void TraCuuMa()
    {
        if (txtMaSV.ReadOnly) return;
        string ma = txtMaSV.Text.Trim().ToUpperInvariant();
        if (maDaTraCuu == ma) return;
        maDaTraCuu = ma;
        var sv = svBUL.GetSinhVienByMaSV(ma);
        if (sv != null)
        {
            // Không ghi đè bản nháp khi người dùng đang thêm và đổi mã sang mã có sẵn.
            if (CoThongTinDangNhap() && MessageBox.Show(
                "Mã sinh viên này đã tồn tại. Bạn có muốn bỏ thông tin đang nhập để xem sinh viên đó không?",
                "Xác nhận chuyển sinh viên", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                errors.SetError(txtMaSV, "Mã đã tồn tại. Nhập mã khác để thêm; thông tin đang nhập vẫn được giữ.");
                return;
            }
            HienThiSinhVienLenForm(sv);
        }
        else
        {
            // Mã mới hoặc mã trống: giữ nguyên bản nháp. Không gọi XoaThongTinChiTiet.
            // Form ban đầu đã được xóa ở LamMoiForm; chỉ Làm mới hoặc lưu thành công mới xóa form.
            errors.SetError(txtMaSV, "");
            btnThem.Enabled = true;
            btnSua.Enabled = btnXoa.Enabled = false;
        }
    }
    private bool CoThongTinDangNhap() =>
        !string.IsNullOrWhiteSpace(txtHoTen.Text)
        || !string.IsNullOrWhiteSpace(txtEmail.Text)
        || !string.IsNullOrWhiteSpace(txtDienThoai.Text)
        || !string.IsNullOrWhiteSpace(txtDiem.Text)
        || dtpNgaySinh.Checked
        || rdoNam.Checked || rdoNu.Checked
        || cboLopHoc.SelectedIndex >= 0
        || cboTrangThai.SelectedIndex >= 0;
    private bool DocSinhVienTuForm(out SinhVien sv)
    {
        sv = new SinhVien
        {
            MaSV = txtMaSV.Text, HoTen = txtHoTen.Text,
            NgaySinh = dtpNgaySinh.Checked ? dtpNgaySinh.Value.Date : DateTime.MinValue,
            GioiTinh = rdoNam.Checked ? "Nam" : rdoNu.Checked ? "Nữ" : "",
            Email = txtEmail.Text, SoDienThoai = txtDienThoai.Text,
            MaLop = (cboLopHoc.SelectedItem as LopHoc)?.MaLop ?? "",
            TrangThai = cboTrangThai.Text
        };
        if (string.IsNullOrWhiteSpace(txtDiem.Text)) return true; // BUL trả lỗi Required cho Diem.
        string text = txtDiem.Text.Trim().Replace(',', '.');
        if (!decimal.TryParse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var diem))
        {
            return false; // LuuSinhVien gộp lỗi chuyển đổi UI với các lỗi Business khác.
        }
        sv.Diem = diem;
        return true;
    }
    private void LuuSinhVien(bool sua)
    {
        errors.Clear();
        bool diemDocDuoc = DocSinhVienTuForm(out var sv);
        var loi = svBUL.KiemTraDuLieu(sv, sua);
        if (!diemDocDuoc)
        {
            loi.RemoveAll(e => e.MemberNames.Contains(nameof(SinhVien.Diem)));
            loi.Add(new ValidationResult("Điểm phải là số, ví dụ 8.5 hoặc 8,5.", new[] { nameof(SinhVien.Diem) }));
        }
        if (loi.Count > 0) { HienThiLoi(new BusinessValidationException(loi)); return; }
        if (sua && MessageBox.Show("Bạn có chắc muốn sửa sinh viên này?", "Xác nhận sửa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try
        {
            if (sua) svBUL.UpdateSinhVien(sv); else svBUL.AddSinhVien(sv);
            TaiDanhSachLop();
            MessageBox.Show(sua ? "Sửa thành công." : "Thêm thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LamMoiForm();
        }
        catch (BusinessValidationException ex) { HienThiLoi(ex); }
        catch (InvalidOperationException ex) { ThongBaoCanhBao(ex.Message); }
    }
    private void HienThiLoi(BusinessValidationException ex)
    {
        var controls = new Dictionary<string, Control>
        {
            [nameof(SinhVien.MaSV)] = txtMaSV, [nameof(SinhVien.HoTen)] = txtHoTen,
            [nameof(SinhVien.NgaySinh)] = dtpNgaySinh, [nameof(SinhVien.GioiTinh)] = rdoNu,
            [nameof(SinhVien.Email)] = txtEmail, [nameof(SinhVien.SoDienThoai)] = txtDienThoai,
            [nameof(SinhVien.Diem)] = txtDiem, [nameof(SinhVien.MaLop)] = cboLopHoc,
            [nameof(SinhVien.TrangThai)] = cboTrangThai
        };
        Control? first = null;
        foreach (var error in ex.Errors)
        {
            foreach (string member in error.MemberNames)
                if (controls.TryGetValue(member, out var c))
                {
                    string old = errors.GetError(c);
                    errors.SetError(c, old.Length == 0 ? error.ErrorMessage : old + Environment.NewLine + error.ErrorMessage);
                    first ??= c;
                }
        }
        first?.Focus();
        lblTongSo.Text = "Thông tin chưa hợp lệ — di chuột vào dấu lỗi cạnh ô nhập để xem.";
    }
    private void XoaSinhVien()
    {
        if (MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        try { svBUL.DeleteSinhVien(txtMaSV.Text); TaiDanhSachLop(); LamMoiForm(); }
        catch (InvalidOperationException ex) { ThongBaoCanhBao(ex.Message); }
    }
    private void HienThiSinhVienLenForm(SinhVien sv)
    {
        errors.Clear();
        txtMaSV.Text = sv.MaSV; txtHoTen.Text = sv.HoTen;
        dtpNgaySinh.Value = sv.NgaySinh; dtpNgaySinh.Checked = true;
        rdoNam.Checked = sv.GioiTinh == "Nam"; rdoNu.Checked = sv.GioiTinh == "Nữ";
        txtEmail.Text = sv.Email; txtDienThoai.Text = sv.SoDienThoai;
        txtDiem.Text = sv.Diem?.ToString(CultureInfo.CurrentCulture) ?? "";
        cboLopHoc.SelectedValue = sv.MaLop; cboTrangThai.SelectedItem = sv.TrangThai;
        txtMaSV.ReadOnly = true; btnThem.Enabled = false; btnSua.Enabled = btnXoa.Enabled = true;
    }
    private void XoaThongTinChiTiet()
    {
        errors.Clear(); txtHoTen.Clear(); txtEmail.Clear(); txtDienThoai.Clear(); txtDiem.Clear();
        dtpNgaySinh.Value = DateTime.Today.AddYears(-18); dtpNgaySinh.Checked = false;
        rdoNam.Checked = rdoNu.Checked = false;
        cboLopHoc.SelectedIndex = cboTrangThai.SelectedIndex = -1;
    }
    private void LamMoiForm()
    {
        maDaTraCuu = null;
        txtMaSV.ReadOnly = false; txtMaSV.Clear(); XoaThongTinChiTiet();
        btnThem.Enabled = true; btnSua.Enabled = btnXoa.Enabled = false;
        dgvSinhVien.ClearSelection();
        BeginInvoke(new Action(() => txtMaSV.Focus()));
    }
    private void TimKiem()
    {
        if (rdoTimNguon.Checked)
        {
            duLieuDaTai = svBUL.TimKiemTaiNguon(MaLopLoc(), txtTuKhoa.Text, nudDiemTu.Value).Select(ToView).ToList();
            HienThiDanhSach(duLieuDaTai);
        }
        else
        {
            // Chỉ lọc DTO đã tải; không gọi BUL/DAL ở nhánh tìm trên giao diện.
            string keyword = txtTuKhoa.Text.Trim();
            HienThiDanhSach(duLieuDaTai.Where(v => v.Diem >= nudDiemTu.Value &&
                (v.MaSV.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ||
                 v.HoTen.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ||
                 v.Email.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ||
                 v.SoDienThoai.Contains(keyword, StringComparison.OrdinalIgnoreCase))));
        }
    }
    private void HienThiTatCa()
    {
        txtTuKhoa.Clear(); nudDiemTu.Value = 0;
        dangNapCombo = true; cboLocLop.SelectedValue = ""; dangNapCombo = false;
        TaiDanhSachLop();
    }
    private void QuanLyLop()
    {
        using var form = new LopHocForm(lopBUL);
        form.ShowDialog(this);
        NapComboLop(); TaiDanhSachLop(); LamMoiForm();
    }
    private static SinhVienView ToView(SinhVien sv) => new()
    {
        MaSV = sv.MaSV, HoTen = sv.HoTen, NgaySinh = sv.NgaySinh.ToString("dd/MM/yyyy"),
        GioiTinh = sv.GioiTinh, Email = sv.Email, SoDienThoai = sv.SoDienThoai,
        Diem = sv.Diem ?? 0, Lop = sv.LopHoc?.TenLop ?? sv.MaLop, TrangThai = sv.TrangThai
    };
    private void HienThiDanhSach(IEnumerable<SinhVienView> list)
    {
        var rows = list.ToList(); dgvSinhVien.DataSource = rows;
        var headers = new Dictionary<string, string> { ["MaSV"] = "Mã SV", ["HoTen"] = "Họ và tên", ["NgaySinh"] = "Ngày sinh", ["GioiTinh"] = "Giới tính", ["Email"] = "Email", ["SoDienThoai"] = "Điện thoại", ["Diem"] = "Điểm", ["Lop"] = "Lớp", ["TrangThai"] = "Trạng thái" };
        foreach (var pair in headers) if (dgvSinhVien.Columns.Contains(pair.Key)) dgvSinhVien.Columns[pair.Key].HeaderText = pair.Value;
        dgvSinhVien.ClearSelection(); lblTongSo.Text = $"Hiển thị: {rows.Count} sinh viên";
    }
    private static void ThongBaoCanhBao(string message) => MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    private void KhoiTaoGiaoDien()
    {
        Text = "Quản lý sinh viên — Bài thực hành 3 (Bài 1 + Bài 2)";
        AutoScaleMode = AutoScaleMode.Dpi;
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
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));
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
        var header = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, TabIndex = 3, TabStop = false };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        header.Controls.Add(tieuDe, 0, 0);
        var btnLop = TaoNut("Quản lý lớp", Color.FromArgb(43, 108, 170), 0);
        btnLop.Width = 150; btnLop.Anchor = AnchorStyles.Right;
        btnLop.Click += (_, _) => QuanLyLop();
        header.Controls.Add(btnLop, 1, 0);
        root.Controls.Add(header, 0, 0);

        GroupBox khungThongTin = TaoKhung("Thông tin sinh viên");
        khungThongTin.TabIndex = 0;
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
            CustomFormat = "dd/MM/yyyy", TabIndex = 3, Margin = new Padding(5, 7, 22, 7)
        };
        rdoNam = new RadioButton { Text = "Nam", Checked = true, AutoSize = true, TabIndex = 0 };
        rdoNu = new RadioButton { Text = "Nữ", AutoSize = true, TabIndex = 1 };
        FlowLayoutPanel gioiTinh = new() { Dock = DockStyle.Fill, TabIndex = 4, TabStop = false, Padding = new Padding(4, 8, 0, 0) };
        gioiTinh.Controls.AddRange(new Control[] { rdoNam, rdoNu });
        txtDiem = TaoTextBox(5);
        txtEmail = TaoTextBox(6);
        txtDienThoai = TaoTextBox(7);
        cboTrangThai = TaoComboBox(8);

        ThemTruong(form, "Mã sinh viên *", txtMaSV, 0, 0);
        ThemTruong(form, "Họ và tên *", txtHoTen, 2, 0);
        ThemTruong(form, "Lớp học *", cboLopHoc, 4, 0);
        ThemTruong(form, "Ngày sinh", dtpNgaySinh, 0, 1);
        ThemTruong(form, "Giới tính", gioiTinh, 2, 1);
        ThemTruong(form, "Điểm *", txtDiem, 4, 1);
        ThemTruong(form, "Email *", txtEmail, 0, 2);
        ThemTruong(form, "Điện thoại *", txtDienThoai, 2, 2);
        ThemTruong(form, "Trạng thái", cboTrangThai, 4, 2);

        btnThem = TaoNut("Thêm", Color.FromArgb(45, 145, 108), 0);
        btnSua = TaoNut("Sửa", Color.FromArgb(43, 108, 170), 1);
        btnXoa = TaoNut("Xóa", Color.FromArgb(201, 75, 88), 2);
        btnLamMoi = TaoNut("Làm mới", Color.FromArgb(106, 121, 139), 3);
        btnDong = TaoNut("Đóng", Color.FromArgb(93, 113, 134), 4);
        FlowLayoutPanel nut = new()
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft,
            TabIndex = 9, TabStop = false, WrapContents = false, Padding = new Padding(0, 3, 8, 0)
        };
        nut.Controls.AddRange(new Control[] { btnDong, btnLamMoi, btnXoa, btnSua, btnThem });
        form.Controls.Add(nut, 0, 3);
        form.SetColumnSpan(nut, 6);

        GroupBox khungTim = TaoKhung(string.Empty);
        khungTim.TabIndex = 1;
        root.Controls.Add(khungTim, 0, 2);
        var searchLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        searchLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        searchLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        khungTim.Controls.Add(searchLayout);
        var modes = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        rdoTimNguon = new RadioButton { Text = "Tìm tại nguồn DataList (qua Business → DAL)", Checked = true, AutoSize = true };
        rdoTimGiaoDien = new RadioButton { Text = "Tìm trên danh sách đã tải lên giao diện", AutoSize = true };
        modes.Controls.AddRange(new Control[] { rdoTimNguon, rdoTimGiaoDien });
        searchLayout.Controls.Add(modes, 0, 0);
        FlowLayoutPanel tim = new()
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false, AutoScroll = true
        };
        searchLayout.Controls.Add(tim, 0, 1);
        txtTuKhoa = new TextBox { Width = 220, PlaceholderText = "Mã, tên, email, điện thoại", Margin = new Padding(4, 7, 12, 4) };
        cboLocLop = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(4, 7, 12, 4) };
        nudDiemTu = TaoSo(0); nudDiemTu.Width = 70; nudDiemTu.Dock = DockStyle.None;
        btnTimKiem = TaoNut("Tìm kiếm", Color.FromArgb(43, 108, 170), 0);
        btnHienThiTatCa = TaoNut("Hiển thị tất cả", Color.FromArgb(93, 113, 134), 0); btnHienThiTatCa.Width = 145;
        tim.Controls.AddRange(new Control[]
        {
            TaoNhanNgang("Từ khóa"), txtTuKhoa, TaoNhanNgang("Lọc lớp"), cboLocLop,
            TaoNhanNgang("Điểm từ"), nudDiemTu, btnTimKiem, btnHienThiTatCa
        });

        GroupBox khungDanhSach = TaoKhung("Danh sách sinh viên");
        khungDanhSach.TabIndex = 2;
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
        Dock = DockStyle.Fill, TabIndex = tabIndex, Margin = new Padding(5, 7, 22, 7)
    };

    private static ComboBox TaoComboBox(int tabIndex) => new()
    {
        Dock = DockStyle.Fill, TabIndex = tabIndex, DropDownStyle = ComboBoxStyle.DropDownList,
        Margin = new Padding(5, 7, 22, 7)
    };

    private static NumericUpDown TaoSo(int tabIndex) => new()
    {
        Dock = DockStyle.Fill, TabIndex = tabIndex, Minimum = 0, Maximum = 10,
        DecimalPlaces = 1, Increment = 0.1m, Margin = new Padding(5, 7, 22, 7)
    };

    private static Label TaoNhan(string text) => new()
    {
        Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, TabStop = false,
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
        public string SoDienThoai { get; set; } = string.Empty;
        public decimal Diem { get; set; }
        public string Lop { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
    }
}
