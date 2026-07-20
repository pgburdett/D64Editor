using System.Collections.Generic;
using System.Linq;

namespace D64Editor.Core;

/// <summary>One entry from the standard 1541 drive error table, e.g. code 20 "Header descriptor byte not found".</summary>
public record D64ErrorCode(byte Code, string CbmError, string Category, string Description)
{
    public override string ToString()
    {
        return $"{Code,2}    {CbmError}    {Category,-6} {Description}";
    }
}

/// <summary>
///     Standard 1541 drive error codes, ported from the option-button captions in
///     frmErrorNumbers.frm. Codes 12-14 aren't defined by the 1541's firmware
///     (the original form had placeholder/duplicate captions for those slots too).
/// </summary>
public static class D64ErrorCodes
{
    public static readonly IReadOnlyList<D64ErrorCode> All = new List<D64ErrorCode>
    {
        new(1, "00", "N/A", "No Error"),
        new(2, "20", "READ", "Header descriptor byte not found ($08)"),
        new(3, "21", "R/W", "No sync sequence found"),
        new(4, "22", "READ", "Data descriptor byte not found ($07)"),
        new(5, "23", "READ", "Checksum error in data block"),
        new(6, "24", "READ", "Byte decoding error"),
        new(7, "25", "WRITE", "Write verify error"),
        new(8, "26", "WRITE", "Write protect on"),
        new(9, "27", "SEEK", "Checksum error in header block"),
        new(10, "28", "WRITE", "Write error"),
        new(11, "29", "SEEK", "Disk sector ID mismatch"),
        new(15, "74", "READ", "Drive Not Ready")
    };

    /// <summary>Ported from the check in lblBAM_Click: codes outside this table default to "No Error".</summary>
    public static D64ErrorCode For(byte rawCode)
    {
        return All.FirstOrDefault(e => e.Code == rawCode) ?? All[0];
    }
}