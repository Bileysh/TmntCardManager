namespace TmntCardManager.Models;

public class ImportResult
{
    public string ErrorMessage { get; set; }
    public string WarningMessage { get; set; }
    public string SuccessMessage { get; set; }
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
}