using System.Text;

namespace D64Editor.Core;

/// <summary>
///     Minimal 6502/6510 disassembler for the documented opcode set (no illegal NMOS
///     opcodes), à la other disk editors' disassembly viewers. Bytes are annotated with a running
///     PC so branch targets can be recognized; unrecognized opcodes render as ".byte".
/// </summary>
public static class Disassembler
{
    private enum Mode { Imp, Acc, Imm, Zp, Zpx, Zpy, Abs, Abx, Aby, Ind, Izx, Izy, Rel }

    private readonly record struct Op(uint Code, string Mnemonic, Mode Mode);

    // Documented 6502 opcode set. Organized by helper to keep the table readable.
    private static readonly Dictionary<int, Op> Table = Build();

    private static Dictionary<int, Op> Build()
    {
        var t = new Dictionary<int, Op>();
        void Add(int code, string mn, Mode m) => t[code] = new Op((uint)code, mn, m);

        // LDA
        Add(0xA9, "LDA", Mode.Imm); Add(0xA5, "LDA", Mode.Zp); Add(0xB5, "LDA", Mode.Zpx);
        Add(0xAD, "LDA", Mode.Abs); Add(0xBD, "LDA", Mode.Abx); Add(0xB9, "LDA", Mode.Aby);
        Add(0xA1, "LDA", Mode.Izx); Add(0xB1, "LDA", Mode.Izy);
        // LDX
        Add(0xA2, "LDX", Mode.Imm); Add(0xA6, "LDX", Mode.Zp); Add(0xB6, "LDX", Mode.Zpy);
        Add(0xAE, "LDX", Mode.Abs); Add(0xBE, "LDX", Mode.Aby);
        // LDY
        Add(0xA0, "LDY", Mode.Imm); Add(0xA4, "LDY", Mode.Zp); Add(0xB4, "LDY", Mode.Zpx);
        Add(0xAC, "LDY", Mode.Abs); Add(0xBC, "LDY", Mode.Abx);
        // STA
        Add(0x85, "STA", Mode.Zp); Add(0x95, "STA", Mode.Zpx); Add(0x8D, "STA", Mode.Abs);
        Add(0x9D, "STA", Mode.Abx); Add(0x99, "STA", Mode.Aby); Add(0x81, "STA", Mode.Izx); Add(0x91, "STA", Mode.Izy);
        // STX/STY
        Add(0x86, "STX", Mode.Zp); Add(0x96, "STX", Mode.Zpy); Add(0x8E, "STX", Mode.Abs);
        Add(0x84, "STY", Mode.Zp); Add(0x94, "STY", Mode.Zpx); Add(0x8C, "STY", Mode.Abs);
        // Transfers / stack
        Add(0xAA, "TAX", Mode.Imp); Add(0xA8, "TAY", Mode.Imp); Add(0x8A, "TXA", Mode.Imp);
        Add(0x98, "TYA", Mode.Imp); Add(0xBA, "TSX", Mode.Imp); Add(0x9A, "TXS", Mode.Imp);
        Add(0x48, "PHA", Mode.Imp); Add(0x08, "PHP", Mode.Imp); Add(0x68, "PLA", Mode.Imp); Add(0x28, "PLP", Mode.Imp);
        // Logic
        Add(0x29, "AND", Mode.Imm); Add(0x25, "AND", Mode.Zp); Add(0x35, "AND", Mode.Zpx);
        Add(0x2D, "AND", Mode.Abs); Add(0x3D, "AND", Mode.Abx); Add(0x39, "AND", Mode.Aby);
        Add(0x21, "AND", Mode.Izx); Add(0x31, "AND", Mode.Izy);
        Add(0x09, "ORA", Mode.Imm); Add(0x05, "ORA", Mode.Zp); Add(0x15, "ORA", Mode.Zpx);
        Add(0x0D, "ORA", Mode.Abs); Add(0x1D, "ORA", Mode.Abx); Add(0x19, "ORA", Mode.Aby);
        Add(0x01, "ORA", Mode.Izx); Add(0x11, "ORA", Mode.Izy);
        Add(0x49, "EOR", Mode.Imm); Add(0x45, "EOR", Mode.Zp); Add(0x55, "EOR", Mode.Zpx);
        Add(0x4D, "EOR", Mode.Abs); Add(0x5D, "EOR", Mode.Abx); Add(0x59, "EOR", Mode.Aby);
        Add(0x41, "EOR", Mode.Izx); Add(0x51, "EOR", Mode.Izy);
        // Bit
        Add(0x24, "BIT", Mode.Zp); Add(0x2C, "BIT", Mode.Abs);
        // Add/Subtract/Compare
        Add(0x69, "ADC", Mode.Imm); Add(0x65, "ADC", Mode.Zp); Add(0x75, "ADC", Mode.Zpx);
        Add(0x6D, "ADC", Mode.Abs); Add(0x7D, "ADC", Mode.Abx); Add(0x79, "ADC", Mode.Aby);
        Add(0x61, "ADC", Mode.Izx); Add(0x71, "ADC", Mode.Izy);
        Add(0xE9, "SBC", Mode.Imm); Add(0xE5, "SBC", Mode.Zp); Add(0xF5, "SBC", Mode.Zpx);
        Add(0xED, "SBC", Mode.Abs); Add(0xFD, "SBC", Mode.Abx); Add(0xF9, "SBC", Mode.Aby);
        Add(0xE1, "SBC", Mode.Izx); Add(0xF1, "SBC", Mode.Izy);
        Add(0xC9, "CMP", Mode.Imm); Add(0xC5, "CMP", Mode.Zp); Add(0xD5, "CMP", Mode.Zpx);
        Add(0xCD, "CMP", Mode.Abs); Add(0xDD, "CMP", Mode.Abx); Add(0xD9, "CMP", Mode.Aby);
        Add(0xC1, "CMP", Mode.Izx); Add(0xD1, "CMP", Mode.Izy);
        Add(0xE0, "CPX", Mode.Imm); Add(0xE4, "CPX", Mode.Zp); Add(0xEC, "CPX", Mode.Abs);
        Add(0xC0, "CPY", Mode.Imm); Add(0xC4, "CPY", Mode.Zp); Add(0xCC, "CPY", Mode.Abs);
        // Inc/Dec memory
        Add(0xE6, "INC", Mode.Zp); Add(0xF6, "INC", Mode.Zpx); Add(0xEE, "INC", Mode.Abs); Add(0xFE, "INC", Mode.Abx);
        Add(0xC6, "DEC", Mode.Zp); Add(0xD6, "DEC", Mode.Zpx); Add(0xCE, "DEC", Mode.Abs); Add(0xDE, "DEC", Mode.Abx);
        Add(0xE8, "INX", Mode.Imp); Add(0xC8, "INY", Mode.Imp); Add(0xCA, "DEX", Mode.Imp); Add(0x88, "DEY", Mode.Imp);
        // Shifts
        Add(0x0A, "ASL", Mode.Acc); Add(0x06, "ASL", Mode.Zp); Add(0x16, "ASL", Mode.Zpx);
        Add(0x0E, "ASL", Mode.Abs); Add(0x1E, "ASL", Mode.Abx);
        Add(0x4A, "LSR", Mode.Acc); Add(0x46, "LSR", Mode.Zp); Add(0x56, "LSR", Mode.Zpx);
        Add(0x4E, "LSR", Mode.Abs); Add(0x5E, "LSR", Mode.Abx);
        Add(0x2A, "ROL", Mode.Acc); Add(0x26, "ROL", Mode.Zp); Add(0x36, "ROL", Mode.Zpx);
        Add(0x2E, "ROL", Mode.Abs); Add(0x3E, "ROL", Mode.Abx);
        Add(0x6A, "ROR", Mode.Acc); Add(0x66, "ROR", Mode.Zp); Add(0x76, "ROR", Mode.Zpx);
        Add(0x6E, "ROR", Mode.Abs); Add(0x7E, "ROR", Mode.Abx);
        // Flags
        Add(0x18, "CLC", Mode.Imp); Add(0x38, "SEC", Mode.Imp); Add(0x58, "CLI", Mode.Imp);
        Add(0x78, "SEI", Mode.Imp); Add(0xB8, "CLV", Mode.Imp); Add(0xD8, "CLD", Mode.Imp); Add(0xF8, "SED", Mode.Imp);
        // Branches
        Add(0x10, "BPL", Mode.Rel); Add(0x30, "BMI", Mode.Rel); Add(0x50, "BVC", Mode.Rel); Add(0x70, "BVS", Mode.Rel);
        Add(0x90, "BCC", Mode.Rel); Add(0xB0, "BCS", Mode.Rel); Add(0xD0, "BNE", Mode.Rel); Add(0xF0, "BEQ", Mode.Rel);
        // Jumps/subroutines
        Add(0x4C, "JMP", Mode.Abs); Add(0x6C, "JMP", Mode.Ind);
        Add(0x20, "JSR", Mode.Abs); Add(0x60, "RTS", Mode.Imp); Add(0x40, "RTI", Mode.Imp); Add(0x00, "BRK", Mode.Imp);
        Add(0xEA, "NOP", Mode.Imp);
        return t;
    }

    private static readonly int[] Lengths = { 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 2, 2 };

    private static readonly Dictionary<ushort, string> Kernal = new()
    {
        [0xFF90] = "SETMSG", [0xFFBA] = "SETLFS", [0xFFBD] = "SETNAM", [0xFFC0] = "OPEN",
        [0xFFC3] = "CLOSE", [0xFFC6] = "CHKIN", [0xFFC9] = "CHKOUT", [0xFFCC] = "CLRCHN",
        [0xFFCF] = "CHRIN", [0xFFD2] = "CHROUT", [0xFFD5] = "LOAD", [0xFFD8] = "SAVE",
        [0xFFE4] = "GETIN", [0xFFB7] = "READST", [0xFF93] = "SECOND", [0xFFAE] = "UNLSN",
        [0xFFB1] = "LISTEN", [0xFFB4] = "TALK", [0xFF8D] = "VECTOR", [0xFFE1] = "STOP",
        [0xFFE7] = "CLALL", [0xFFF3] = "IOBASE", [0xFCE2] = "RESET",
    };

    /// <summary>Disassemble a PRG linearly (default) or by control-flow tracing.</summary>
    public static List<string> Disassemble(byte[] bytes, bool trace = false, IEnumerable<ushort>? extraEntryPoints = null)
    {
        if (bytes is null || bytes.Length < 2) return [];
        var pcBase = (ushort)(bytes[0] | (bytes[1] << 8));
        return trace
            ? DisassembleTrace(bytes, pcBase, extraEntryPoints)
            : DisassembleLinear(bytes, pcBase);
    }

    private static List<string> DisassembleLinear(byte[] bytes, ushort pcBase)
    {
        var targets = CollectTargets(bytes, pcBase);
        var lines = new List<string>();
        var pos = 2;
        while (pos < bytes.Length)
        {
            var opPc = (ushort)(pcBase + (pos - 2));
            if (targets.Contains(opPc)) lines.Add($"L{opPc:X4}:");
            EmitInstruction(bytes, pcBase, pos, opPc, targets, lines);
            if (Table.TryGetValue(bytes[pos], out var info)) pos += Lengths[(int)info.Mode];
            else pos += 1;
        }
        return lines;
    }

    private static List<string> DisassembleTrace(byte[] bytes, ushort pcBase, IEnumerable<ushort>? extraEntryPoints)
    {
        var size = bytes.Length - 2;
        var codeStart = new bool[size];
        var codeByte = new bool[size];
        var instrLen = new byte[size];
        var targets = new HashSet<ushort>();
        var work = new Queue<ushort>();

        void Enqueue(ushort a, bool isTarget = true)
        {
            var off = a - pcBase;
            if (off < 0 || off >= size) return;
            if (isTarget) targets.Add(a);
            if (!codeStart[off] && !codeByte[off]) work.Enqueue(a);
        }

        Enqueue(pcBase, true);
        if (extraEntryPoints != null)
            foreach (var ep in extraEntryPoints) Enqueue(ep, true);

        while (work.Count > 0)
        {
            var pc = work.Dequeue();
            var off = pc - pcBase;
            if (off < 0 || off >= size || codeStart[off] || codeByte[off]) continue;
            if (!Table.TryGetValue(bytes[2 + off], out var info))
            {
                codeStart[off] = true; codeByte[off] = true; instrLen[off] = 1;
                continue;
            }
            var len = Lengths[(int)info.Mode];
            if (off + len > size) len = size - off;
            // Don't overwrite an existing instruction start; allow overlap only into data.
            var conflict = false;
            var skipBytes = new List<int>();
            for (var i = 0; i < len; i++)
            {
                if (!codeStart[off + i]) continue;
                // BIT skip trick: BIT zp/abs (2 or 3 bytes) may overlap 1-byte instruction targets.
                if (i >= 1 && info.Mnemonic == "BIT" && (len == 2 || len == 3) && instrLen[off + i] == 1)
                { skipBytes.Add(i); continue; }
                conflict = true; break;
            }
            if (conflict) continue;
            foreach (var i in skipBytes)
            {
                codeStart[off + i] = false;
                instrLen[off + i] = 0;
            }
            codeStart[off] = true;
            instrLen[off] = (byte)len;
            for (var i = 0; i < len; i++) codeByte[off + i] = true;

            var b1 = len > 1 ? bytes[2 + off + 1] : (byte)0;
            var b2 = len > 2 ? bytes[2 + off + 2] : (byte)0;
            var next = (ushort)(pc + len);
            switch (info.Mode)
            {
                case Mode.Rel:
                    var relTarget = (ushort)(pc + 2 + (sbyte)b1);
                    Enqueue(relTarget, true);
                    Enqueue(next, false); // conditional branch falls through
                    break;
                case Mode.Abs when info.Mnemonic == "JSR":
                    var jsrTarget = (ushort)(b1 | (b2 << 8));
                    Enqueue(jsrTarget, true);
                    Enqueue(next, false); // assume return
                    break;
                case Mode.Abs when info.Mnemonic == "JMP":
                    var jmpTarget = (ushort)(b1 | (b2 << 8));
                    Enqueue(jmpTarget, true);
                    break;
                case Mode.Ind when info.Mnemonic == "JMP":
                    // indirect jump - cannot follow statically
                    break;
                default:
                    if (info.Mnemonic is "RTS" or "RTI" or "BRK") { }
                    else Enqueue(next, false);
                    break;
            }
        }

        var lines = new List<string>();
        var p = 0;
        while (p < size)
        {
            var addr = (ushort)(pcBase + p);
            if (codeStart[p])
            {
                if (targets.Contains(addr)) lines.Add($"L{addr:X4}:");
                EmitInstruction(bytes, pcBase, 2 + p, addr, targets, lines);
                var len = Math.Max((byte)1, instrLen[p]);
                for (var i = 1; i < len; i++)
                {
                    var inner = (ushort)(pcBase + p + i);
                    if (targets.Contains(inner)) lines.Add($"L{inner:X4}:  ; skip-trick target (overlapped by BIT)");
                }
                p += len;
            }
            else
            {
                // Emit data bytes in small groups
                var start = p;
                while (p < size && !codeStart[p] && p - start < 8) p++;
                var chunk = new byte[p - start];
                Array.Copy(bytes, 2 + start, chunk, 0, chunk.Length);
                var hex = string.Join(' ', chunk.Select(b => b.ToString("X2")));
                var asc = new string(chunk.Select(b => b >= 32 && b < 127 ? (char)b : '.').ToArray());
                lines.Add($"${(ushort)(pcBase + start):X4}: {hex.PadRight(23)}  .byte {string.Join(", ", chunk.Select(b => $"${b:X2}"))}  ; {asc}");
            }
        }
        return lines;
    }

    private static void EmitInstruction(byte[] bytes, ushort pcBase, int pos, ushort opPc, HashSet<ushort> targets, List<string> lines)
    {
        var op = bytes[pos];
        if (!Table.TryGetValue(op, out var info))
        {
            lines.Add($"${opPc:X4}: {op:X2}        .byte ${op:X2}");
            return;
        }
        var len = Lengths[(int)info.Mode];
        var operandBytes = new byte[3];
        for (var i = 1; i < len && pos + i < bytes.Length; i++)
            operandBytes[i - 1] = bytes[pos + i];
        var text = Format(info, operandBytes, opPc, len, targets);
        var byteCol = new StringBuilder();
        for (var i = 0; i < len && pos + i < bytes.Length; i++)
            byteCol.Append(bytes[pos + i].ToString("X2")).Append(' ');
        lines.Add($"${opPc:X4}: {byteCol.ToString().PadRight(11)}{text}");
    }

    private static HashSet<ushort> CollectTargets(byte[] bytes, ushort pcBase)
    {
        var targets = new HashSet<ushort>();
        var pos = 2;
        while (pos < bytes.Length)
        {
            var opPc = (ushort)(pcBase + (pos - 2));
            var op = bytes[pos];
            if (!Table.TryGetValue(op, out var info)) { pos++; continue; }
            var len = Lengths[(int)info.Mode];
            if (pos + len > bytes.Length) { pos += len; continue; }
            if (info.Mode == Mode.Rel)
            {
                var target = (ushort)(opPc + 2 + (sbyte)bytes[pos + 1]);
                targets.Add(target);
            }
            else if (info.Mode == Mode.Abs && (info.Mnemonic is "JMP" or "JSR"))
            {
                var target = (ushort)(bytes[pos + 1] | (bytes[pos + 2] << 8));
                if (target >= pcBase && target < pcBase + (bytes.Length - 2))
                    targets.Add(target);
            }
            pos += len;
        }
        return targets;
    }

    private static string Format(Op info, byte[] operand, ushort pc, int len, HashSet<ushort> targets)
    {
        string S(ushort a) => targets.Contains(a) ? $"L{a:X4}" : $"${a:X4}";
        string KernalComment(ushort a) => Kernal.TryGetValue(a, out var n) ? $"  ; {n}" : string.Empty;
        switch (info.Mode)
        {
            case Mode.Imp:
            case Mode.Acc:
                return info.Mode == Mode.Acc ? $"{info.Mnemonic} A" : info.Mnemonic;
            case Mode.Imm:
                return $"{info.Mnemonic} #${operand[0]:X2}";
            case Mode.Zp:
                return $"{info.Mnemonic} ${operand[0]:X2}";
            case Mode.Zpx:
                return $"{info.Mnemonic} ${operand[0]:X2},X";
            case Mode.Zpy:
                return $"{info.Mnemonic} ${operand[0]:X2},Y";
            case Mode.Abs:
                var abs = (ushort)(operand[0] | (operand[1] << 8));
                return $"{info.Mnemonic} {S(abs)}{KernalComment(abs)}";
            case Mode.Abx:
                return $"{info.Mnemonic} ${((ushort)(operand[0] | (operand[1] << 8))):X4},X";
            case Mode.Aby:
                return $"{info.Mnemonic} ${((ushort)(operand[0] | (operand[1] << 8))):X4},Y";
            case Mode.Ind:
                return $"{info.Mnemonic} (${((ushort)(operand[0] | (operand[1] << 8))):X4})";
            case Mode.Izx:
                return $"{info.Mnemonic} (${operand[0]:X2},X)";
            case Mode.Izy:
                return $"{info.Mnemonic} (${operand[0]:X2}),Y";
            case Mode.Rel:
                var target = (ushort)(pc + 2 + (sbyte)operand[0]);
                return $"{info.Mnemonic} {S(target)}";
            default:
                return info.Mnemonic;
        }
    }
}
