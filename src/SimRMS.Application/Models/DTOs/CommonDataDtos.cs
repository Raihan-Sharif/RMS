namespace SimRMS.Application.Models.DTOs;

public class BranchListDto
{
    public string CompanyCode { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
}

public class CompanyListDto
{
    public string CompanyCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
}

public class TraderListDto
{
    public string ExchangeCode { get; set; } = string.Empty;
    public string TraderCode { get; set; } = string.Empty;
}