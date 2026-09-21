using System.Net.Http.Json;
using System.Text.Json;
using WarrantRisk.Domain;

namespace WarrantRisk.WinForms;

public partial class MainForm : Form
{
    private readonly HttpClient _http = new() { BaseAddress = new Uri("http://localhost:5080/") };
    private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };
    private List<Warrant> _warrants = [];
    private Warrant? _selected;
    private bool _loading;

    public MainForm() { InitializeComponent(); Shown += async (_, _) => await LoadWarrantsAsync(); }

    private async Task LoadWarrantsAsync()
    {
        try { _loading = true; _warrants = await _http.GetFromJsonAsync<List<Warrant>>("api/warrants", _json) ?? []; BindWarrants(_warrants); lblStatus.Text = $"已載入 {_warrants.Count} 筆權證"; }
        catch (Exception ex) { ShowError("無法連線至 API，請先啟動 WarrantRisk.Api。", ex); }
        finally { _loading = false; }
    }
    private void BindWarrants(IEnumerable<Warrant> rows) { dgvWarrants.DataSource = rows.Select(x => new { 權證代碼 = x.WarrantId, 類型 = x.WarrantType, 履約價 = x.StrikePrice, 行使比例 = x.ConversionRatio, 庫存 = x.PositionQty }).ToList(); }
    private async void txtSearch_TextChanged(object? sender, EventArgs e)
    {
        if (_loading) return;
        try { var key = Uri.EscapeDataString(txtSearch.Text.Trim()); var rows = await _http.GetFromJsonAsync<List<Warrant>>($"api/warrants?keyword={key}", _json) ?? []; BindWarrants(rows); }
        catch (Exception ex) { lblStatus.Text = ex.Message; }
    }
    private async void dgvWarrants_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= dgvWarrants.Rows.Count) return;
        var id = dgvWarrants.Rows[e.RowIndex].Cells[0].Value?.ToString(); _selected = _warrants.FirstOrDefault(x => x.WarrantId == id) ?? await _http.GetFromJsonAsync<Warrant>($"api/warrants/{id}", _json);
        if (_selected is null) return; lblWarrant.Text = _selected.WarrantId; lblStrike.Text = _selected.StrikePrice.ToString("0.0000"); lblRatio.Text = _selected.ConversionRatio.ToString("0.0000"); lblType.Text = _selected.WarrantType; lblPosition.Text = _selected.PositionQty.ToString("N0"); btnSave.Enabled = true; await LoadHistoryAsync(); CalculatePreview();
    }
    private void txtMarketPrice_TextChanged(object? sender, EventArgs e) => CalculatePreview();
    private void CalculatePreview()
    {
        if (_selected is null || !decimal.TryParse(txtMarketPrice.Text, out var price) || price <= 0) { lblTheory.Text = "-"; lblHedge.Text = "-"; return; }
        var r = WarrantCalculator.Calculate(_selected, price); lblTheory.Text = r.TheoryPrice.ToString("N4"); lblHedge.Text = r.HedgeQty.ToString("N2");
    }
    private async void btnSave_Click(object? sender, EventArgs e)
    {
        if (_selected is null || !decimal.TryParse(txtMarketPrice.Text, out var price) || price <= 0) { MessageBox.Show("請輸入大於 0 的標的股價。", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        try { var response = await _http.PostAsJsonAsync($"api/warrants/{_selected.WarrantId}/trials", new TrialRequest(price)); if (!response.IsSuccessStatusCode) { MessageBox.Show(await response.Content.ReadAsStringAsync(), "儲存失敗", MessageBoxButtons.OK, MessageBoxIcon.Error); return; } MessageBox.Show("試算結果已儲存。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information); await LoadHistoryAsync(); }
        catch (Exception ex) { ShowError("儲存失敗。", ex); }
    }
    private async Task LoadHistoryAsync() { if (_selected is null) return; dgvHistory.DataSource = await _http.GetFromJsonAsync<List<TrialLog>>($"api/warrants/{_selected.WarrantId}/trials/recent", _json) ?? []; }
    private static void ShowError(string message, Exception ex) => MessageBox.Show($"{message}\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
