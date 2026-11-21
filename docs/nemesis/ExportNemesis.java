// Export disassembly and defined data for Nemesis COM files
import ghidra.app.script.GhidraScript;
import ghidra.program.model.address.Address;
import ghidra.program.model.listing.*;
import ghidra.program.model.mem.MemoryBlock;
import java.io.*;

public class ExportNemesis extends GhidraScript {
    @Override
    public void run() throws Exception {
        Program p = currentProgram;
        String name = p.getName();
        File out = new File("/tmp/opencode/disasm-" + name + ".txt");
        PrintWriter w = new PrintWriter(out);
        w.println("=== " + name + " ===");
        w.println("Language: " + p.getLanguage().getDescription());
        for (MemoryBlock b : p.getMemory().getBlocks()) {
            w.printf("BLOCK %s %08x-%08x%n", b.getName(), b.getStart().getOffset(), b.getEnd().getOffset());
        }
        w.println();
        Listing l = p.getListing();
        for (Instruction i : l.getInstructions(true)) {
            Address a = i.getAddress();
            w.printf("%04x  %-24s %s%n", a.getOffset(), i.getBytesAsString(), i.toString());
        }
        w.println("\n=== DATA ===");
        for (Data d : l.getDefinedData(true)) {
            Address a = d.getAddress();
            w.printf("%04x  %-24s %s%n", a.getOffset(), d.getBytesAsString(), d.toString());
        }
        w.close();
        println("Exported " + out);
    }
}
