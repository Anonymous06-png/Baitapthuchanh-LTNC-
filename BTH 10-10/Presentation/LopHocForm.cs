using System.Drawing;
using QuanLySinhVien.Business;
using QuanLySinhVien.Entity;
namespace QuanLySinhVien.Presentation;

public sealed class LopHocForm : Form
{
    private readonly LopHocBUL business;
    private readonly TextBox txtMa = new() { Dock = DockStyle.Fill };
    private readonly TextBox txtTen = new() { Dock = DockStyle.Fill };
    private readonly DataGridView grid = new()
    {
        Dock = DockStyle.Fill, ReadOnly = true, MultiSelect = false,
        AllowUserToAddRows = false, AllowUserToDeleteRows = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false
    };
    private readonly Button them = new() { Text = "Thêm", AutoSize = true };
    private readonly Button sua = new() { Text = "Sửa", AutoSize = true, Enabled = false };
    private readonly Button xoa = new() { Text = "Xóa", AutoSize = true, Enabled = false };
    private readonly ErrorProvider errors;
    public LopHocForm(LopHocBUL business)
    {
        this.business = business;
        Text = "Quản lý lớp học"; StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(700, 450); MinimumSize = new Size(550, 350);
        Font = new Font("Segoe UI", 10); AutoScaleMode = AutoScaleMode.Dpi;
        errors = new ErrorProvider { ContainerControl = this, BlinkStyle = ErrorBlinkStyle.NeverBlink };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 4, Padding = new Padding(18) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(layout);
        txtMa.Margin = txtTen.Margin = new Padding(3, 5, 25, 5);
        layout.Controls.Add(new Label { Text = "Mã lớp *", Dock = DockStyle.Fill }, 0, 0); layout.Controls.Add(txtMa, 1, 0);
        layout.Controls.Add(new Label { Text = "Tên lớp *", Dock = DockStyle.Fill }, 0, 1); layout.Controls.Add(txtTen, 1, 1);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill };
        var reset = new Button { Text = "Làm mới", AutoSize = true };
        buttons.Controls.AddRange(new Control[] { them, sua, xoa, reset }); layout.Controls.Add(buttons, 0, 2); layout.SetColumnSpan(buttons, 2);
        layout.Controls.Add(grid, 0, 3); layout.SetColumnSpan(grid, 2);
        Load += (_, _) => RefreshList();
        them.Click += (_, _) => Save(false); sua.Click += (_, _) => Save(true);
        reset.Click += (_, _) => Reset();
        xoa.Click += (_, _) =>
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa lớp này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try { business.DeleteLopHoc(txtMa.Text); RefreshList(); Reset(); }
            catch (InvalidOperationException ex) { MessageBox.Show(ex.Message, "Không thể xóa"); }
        };
        grid.CellClick += (_, e) =>
        {
            if (e.RowIndex < 0 || grid.Rows[e.RowIndex].DataBoundItem is not LopView l) return;
            errors.Clear(); txtMa.Text = l.MaLop; txtTen.Text = l.TenLop; txtMa.ReadOnly = true;
            them.Enabled = false; sua.Enabled = xoa.Enabled = true;
        };
        txtMa.TextChanged += (_, _) => errors.SetError(txtMa, "");
        txtTen.TextChanged += (_, _) => errors.SetError(txtTen, "");
    }
    private void Save(bool editing)
    {
        errors.Clear();
        if (editing && MessageBox.Show("Bạn có chắc muốn sửa tên lớp?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        try
        {
            var l = new LopHoc { MaLop = txtMa.Text, TenLop = txtTen.Text };
            if (editing) business.UpdateLopHoc(l); else business.AddLopHoc(l);
            RefreshList(); Reset();
        }
        catch (BusinessValidationException ex)
        {
            foreach (var err in ex.Errors)
                foreach (var member in err.MemberNames)
                {
                    Control c = member == nameof(LopHoc.MaLop) ? txtMa : txtTen;
                    var previous = errors.GetError(c);
                    errors.SetError(c, previous.Length == 0 ? err.ErrorMessage : previous + Environment.NewLine + err.ErrorMessage);
                }
        }
        catch (InvalidOperationException ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void RefreshList()
    {
        grid.DataSource = business.GetAllLopHoc().Select(l => new LopView { MaLop = l.MaLop, TenLop = l.TenLop, SiSo = l.DanhSachSinhVien.Count }).ToList();
        grid.Columns["MaLop"].HeaderText = "Mã lớp"; grid.Columns["TenLop"].HeaderText = "Tên lớp"; grid.Columns["SiSo"].HeaderText = "Sĩ số";
        grid.ClearSelection();
    }
    private void Reset()
    {
        errors.Clear(); txtMa.ReadOnly = false; txtMa.Clear(); txtTen.Clear();
        them.Enabled = true; sua.Enabled = xoa.Enabled = false; grid.ClearSelection(); txtMa.Focus();
    }
    protected override void Dispose(bool disposing) { if (disposing) errors?.Dispose(); base.Dispose(disposing); }
    private sealed class LopView
    {
        public string MaLop { get; set; } = ""; public string TenLop { get; set; } = ""; public int SiSo { get; set; }
    }
}
