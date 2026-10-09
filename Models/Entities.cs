namespace DairyFarm.Api.Models; 
public class User
{
    public int UserId { get; set; }
    public string FullName { get; set; } = "";
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "User";   // Admin | User
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class Cow
{
    public int CowId { get; set; }
    public string TagNumber { get; set; } = "";
    public string? Name { get; set; }
    public string? Breed { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string CowType { get; set; } = "New";  // New | Old
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string Status { get; set; } = "Active";
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MilkRecord
{
    public int MilkRecordId { get; set; }
    public int CowId { get; set; }
    public DateTime RecordDate { get; set; }
    public string Shift { get; set; } = "Morning"; // Morning | Evening
    public decimal Litres { get; set; }
    public decimal? Fat { get; set; }
    public int EnteredBy { get; set; }
}

public class DoctorVisit
{
    public int VisitId { get; set; }
    public int CowId { get; set; }
    public DateTime VisitDate { get; set; }
    public string DoctorName { get; set; } = "";
    public string? Disease { get; set; }
    public string? Treatment { get; set; }
    public decimal? Cost { get; set; }
    public DateTime? NextVisitDate { get; set; }
    public int EnteredBy { get; set; }
}

public class StockItem
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal CurrentQty { get; set; }
    public decimal ReorderLevel { get; set; }
}

public class LoadUnloadTxn
{
    public int TxnId { get; set; }
    public DateTime TxnDate { get; set; }
    public string Direction { get; set; } = "Load"; // Load | Unload
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public string? VehicleNo { get; set; }
    public string? PartyName { get; set; }
    public string? Remarks { get; set; }
    public int EnteredBy { get; set; }
}

public record LoginRequest(string Username, string Password);
public record RegisterUserRequest(string FullName, string Username, string Password, string Role);
