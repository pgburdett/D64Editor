using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>Disassembly viewer for PRG files and raw disk sectors/blocks.</summary>
public partial class DisassemblyWindow : Window
{
    private string _rawListing = string.Empty;
    private byte[] _fileBytes = Array.Empty<byte>();
    private D64Image? _disk;
    private string _fileName = string.Empty;

    public DisassemblyWindow()
    {
        InitializeComponent();
        PetsciiDisplay.Changed += ApplyCharset;
        RefreshBtn.Click += (_, _) => RefreshListing();
        TraceBox.IsCheckedChanged += (_, _) => RefreshListing();
        ModeBox.SelectionChanged += (_, _) => UpdateModeVisibility();
        UpdateModeVisibility();
    }

    public DisassemblyWindow(string fileName, byte[] bytes, D64Image? disk = null) : this()
    {
        _fileName = fileName;
        _fileBytes = bytes ?? Array.Empty<byte>();
        _disk = disk;
        Title = $"Disassembly — {fileName}";
        if (_disk is null)
        {
            // No disk context: only File mode makes sense.
            ModeBox.IsEnabled = false;
        }
        RefreshListing();
    }

    /// <summary>Open directly in sector/block mode for a disk image.</summary>
    public DisassemblyWindow(D64Image disk, int track, int sector) : this()
    {
        _disk = disk;
        _fileName = $"T{track}/S{sector}";
        Title = $"Disassembly — {_fileName}";
        ModeBox.SelectedIndex = 1;
        TrackBox.Text = track.ToString();
        SectorBox.Text = sector.ToString();
        CountBox.Text = "1";
        UpdateModeVisibility();
        RefreshListing();
    }

    private void UpdateModeVisibility()
    {
        var mode = (ModeBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "File";
        var isTs = mode == "Track/Sector";
        var isBlock = mode == "Block";
        TLabel.IsVisible = TrackBox.IsVisible = isTs;
        SLabel.IsVisible = SectorBox.IsVisible = isTs;
        BLabel.IsVisible = BlockBox.IsVisible = isBlock;
        CLabel.IsVisible = CountBox.IsVisible = isTs || isBlock;
        if ((isTs || isBlock) && _disk is null)
        {
            ModeBox.SelectedIndex = 0;
        }
    }

    private void RefreshListing()
    {
        try
        {
            var trace = TraceBox.IsChecked == true;
            var entries = ParseEntries(EntryBox.Text);
            var (bytes, label) = GetBytes();
            if (bytes.Length == 0)
            {
                _rawListing = "(no data)";
                TitleLabel.Text = label;
                ApplyCharset();
                return;
            }
            var lines = Disassembler.Disassemble(bytes, trace, entries.Count > 0 ? entries : null);
            var load = bytes.Length > 1 ? (bytes[0] | (bytes[1] << 8)).ToString("X4") : "----";
            var ep = entries.Count > 0 ? $", entries={string.Join(",", entries.Select(e => $"${e:X4}"))}" : "";
            TitleLabel.Text = $"{label} — {lines.Count} line(s) @ ${load}{(trace ? ", trace" : "")}{ep}";
            _rawListing = lines.Count == 0 ? "(nothing to disassemble)" : string.Join('\n', lines);
        }
        catch (Exception ex)
        {
            _rawListing = "Error: " + ex.Message;
            TitleLabel.Text = "Disassembly error";
        }
        ApplyCharset();
    }

    private (byte[] bytes, string label) GetBytes()
    {
        var mode = (ModeBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "File";
        if (mode == "File" || _disk is null)
            return (_fileBytes, _fileName.Length == 0 ? "File" : _fileName);

        var count = Math.Clamp(ParseInt(CountBox.Text, 1), 1, 64);
        if (mode == "Track/Sector")
        {
            var t0 = ParseInt(TrackBox.Text, 18);
            var s0 = ParseInt(SectorBox.Text, 0);
            var t = t0; var s = s0;
            var raw = new List<byte>();
            for (var i = 0; i < count; i++)
            {
                var block = D64Image.TrackSectorToBlock(t, s);
                raw.AddRange(_disk.ReadBlock(block));
                s++;
                if (s >= D64Image.SectorsPerTrack(t)) { s = 0; t++; if (t > _disk.TrackCount) break; }
            }
            var label = count == 1 ? $"T{t0}/S{s0}" : $"T{t0}/S{s0} +{count}";
            return (WithBase(raw.ToArray(), 0x0000), label);
        }
        else // Block
        {
            var b = Math.Clamp(ParseInt(BlockBox.Text, 0), 0, Math.Max(0, _disk.TotalBlocks - 1));
            var raw = new List<byte>();
            for (var i = 0; i < count && b + i < _disk.TotalBlocks; i++)
                raw.AddRange(_disk.ReadBlock(b + i));
            var label = count == 1 ? $"Block {b}" : $"Blocks {b}-{b + count - 1}";
            return (WithBase(raw.ToArray(), 0x0000), label);
        }
    }

    private static List<ushort> ParseEntries(string? text)
    {
        var result = new List<ushort>();
        if (string.IsNullOrWhiteSpace(text)) return result;
        foreach (var part in text.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var s = part;
            if (s.StartsWith("$")) s = s[1..];
            else if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) s = s[2..];
            if (ushort.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var v))
                result.Add(v);
            else if (ushort.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v))
                result.Add(v);
        }
        return result;
    }

    private static int ParseInt(string? text, int fallback)
    {
        if (string.IsNullOrWhiteSpace(text)) return fallback;
        var s = text.Trim();
        if (s.StartsWith("$")) return int.TryParse(s[1..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var h) ? h : fallback;
        if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) return int.TryParse(s[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var x) ? x : fallback;
        return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : fallback;
    }

    private static byte[] WithBase(byte[] raw, ushort baseAddr)
    {
        var data = new byte[2 + raw.Length];
        data[0] = (byte)(baseAddr & 0xFF);
        data[1] = (byte)(baseAddr >> 8);
        Array.Copy(raw, 0, data, 2, raw.Length);
        return data;
    }

    private void ApplyCharset() => ListingBox.Text = PetsciiDisplay.ToPuaString(_rawListing);
}
