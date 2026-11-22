// Decompile all functions in the current program to C-like pseudocode.
// @category Nemesis
import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.address.Address;
import ghidra.program.model.listing.*;
import java.io.*;

public class DecompileNemesis extends GhidraScript {
    @Override
    public void run() throws Exception {
        Program p = currentProgram;
        String name = p.getName().replace(".COM", "");
        File out = new File("/tmp/opencode/decomp-" + name + ".c");
        PrintWriter w = new PrintWriter(out);
        w.println("/* Decompiled " + p.getName() + " - Ghidra Z80 decompiler */");
        DecompInterface di = new DecompInterface();
        di.openProgram(p);
        int ok = 0, fail = 0;
        for (Function f : p.getFunctionManager().getFunctions(true)) {
            Address a = f.getEntryPoint();
            DecompileResults r = di.decompileFunction(f, 60, monitor);
            if (r.decompileCompleted()) {
                w.println("\n/* === " + a + " : " + f.getName() + " === */");
                w.println(r.getDecompiledFunction().getC());
                ok++;
            } else {
                w.println("\n/* FAILED " + a + " : " + r.getErrorMessage() + " */");
                fail++;
            }
        }
        w.close();
        di.dispose();
        println("Decompiled " + ok + " functions (" + fail + " failed) to " + out);
    }
}
