#nullable enable
namespace WarrantRisk.WinForms;

partial class MainForm
{
    private System.ComponentModel.IContainer? components;
    private TextBox txtSearch = null!, txtMarketPrice = null!;
    private DataGridView dgvWarrants = null!, dgvHistory = null!;
    private Label lblWarrant = null!, lblStrike = null!, lblRatio = null!, lblType = null!, lblPosition = null!, lblTheory = null!, lblHedge = null!, lblStatus = null!;
    private Button btnSave = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        txtSearch = new TextBox(); txtMarketPrice = new TextBox(); dgvWarrants = new DataGridView(); dgvHistory = new DataGridView();
        lblWarrant = new Label(); lblStrike = new Label(); lblRatio = new Label(); lblType = new Label(); lblPosition = new Label(); lblTheory = new Label(); lblHedge = new Label(); lblStatus = new Label(); btnSave = new Button();
        ((System.ComponentModel.ISupportInitialize)dgvWarrants).BeginInit(); ((System.ComponentModel.ISupportInitialize)dgvHistory).BeginInit(); SuspendLayout();
        Text = "權證發行風險監控與避險試算"; StartPosition = FormStartPosition.CenterScreen; MinimumSize = new Size(1100, 700); ClientSize = new Size(1250, 780);
        var searchLabel = new Label { Text = "權證代碼搜尋", Location = new Point(15, 15), AutoSize = true };
        txtSearch.Location = new Point(15, 40); txtSearch.Width = 300; txtSearch.TextChanged += txtSearch_TextChanged;
        dgvWarrants.Location = new Point(15, 80); dgvWarrants.Size = new Size(350, 620); dgvWarrants.ReadOnly = true; dgvWarrants.AllowUserToAddRows = false; dgvWarrants.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgvWarrants.AutoGenerateColumns = true; dgvWarrants.CellClick += dgvWarrants_CellClick;
        var info = new GroupBox { Text = "權證基本資料與即時試算", Location = new Point(390, 15), Size = new Size(820, 270) };
        AddInfo(info, "權證代碼", lblWarrant, 25, 35); AddInfo(info, "履約價格", lblStrike, 25, 75); AddInfo(info, "行使比例", lblRatio, 25, 115); AddInfo(info, "類型", lblType, 25, 155); AddInfo(info, "庫存張數", lblPosition, 25, 195);
        var marketLabel = new Label { Text = "標的股價", Location = new Point(390, 38), AutoSize = true }; txtMarketPrice.Location = new Point(500, 34); txtMarketPrice.Width = 180; txtMarketPrice.TextChanged += txtMarketPrice_TextChanged;
        var theoryTitle = new Label { Text = "理論價值", Location = new Point(390, 85), AutoSize = true }; lblTheory.Location = new Point(500, 83); lblTheory.AutoSize = true; lblTheory.Font = new Font(Font, FontStyle.Bold); lblTheory.Text = "-";
        var hedgeTitle = new Label { Text = "建議避險張數", Location = new Point(390, 130), AutoSize = true }; lblHedge.Location = new Point(500, 128); lblHedge.AutoSize = true; lblHedge.Font = new Font(Font, FontStyle.Bold); lblHedge.Text = "-";
        btnSave.Text = "儲存試算結果"; btnSave.Location = new Point(390, 180); btnSave.Size = new Size(180, 42); btnSave.Click += btnSave_Click; btnSave.Enabled = false;
        info.Controls.AddRange(new Control[] { marketLabel, txtMarketPrice, theoryTitle, lblTheory, hedgeTitle, lblHedge, btnSave });
        var historyGroup = new GroupBox { Text = "最近 10 筆試算紀錄", Location = new Point(390, 305), Size = new Size(820, 395) }; dgvHistory.Dock = DockStyle.Fill; dgvHistory.ReadOnly = true; dgvHistory.AllowUserToAddRows = false; dgvHistory.AutoGenerateColumns = true; historyGroup.Controls.Add(dgvHistory);
        lblStatus.Location = new Point(15, 715); lblStatus.AutoSize = true; lblStatus.ForeColor = Color.DarkGreen;
        Controls.AddRange(new Control[] { searchLabel, txtSearch, dgvWarrants, info, historyGroup, lblStatus }); ((System.ComponentModel.ISupportInitialize)dgvWarrants).EndInit(); ((System.ComponentModel.ISupportInitialize)dgvHistory).EndInit(); ResumeLayout(false); PerformLayout();
    }
    private static void AddInfo(Control parent, string title, Label value, int x, int y) { parent.Controls.Add(new Label { Text = title, Location = new Point(x, y), AutoSize = true }); value.Text = "-"; value.AutoSize = true; value.Location = new Point(x + 100, y); parent.Controls.Add(value); }
}
