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

public class CountryListDto
{
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
}

public class ClientTypeListDto
{
    public string ClientType { get; set; } = string.Empty;
    public string ClientTypeName { get; set; } = string.Empty;
}

public class ClientListDto
{
    public string GCIF { get; set; } = string.Empty;
    public string ClntName { get; set; } = string.Empty;
    public string ClntNICNo { get; set; } = string.Empty;
    public string ClntAddr { get; set; } = string.Empty;
    public string ClntPhone { get; set; } = string.Empty;
    public string ClntMobile { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string ClntEmail { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string ClntCode { get; set; } = string.Empty;
    public string CoBrchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string ClntCDSNo { get; set; } = string.Empty;
}