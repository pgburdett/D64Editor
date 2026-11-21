# Export Ghidra disassembly of a program to a text file
from ghidra.program.model.address import Address
from ghidra.program.model.listing import Instruction, Data
from ghidra.program.model.mem import MemoryBlock

p = currentProgram
name = p.getName()
out = open('/tmp/opencode/disasm-' + name + '.txt', 'w')
out.write('=== %s ===\n' % name)
out.write('Language: %s\n' % p.getLanguage().getDescription())
for b in p.getMemory().getBlocks():
    out.write('BLOCK %s %08x-%08x\n' % (b.getName(), b.getStart().getOffset(), b.getEnd().getOffset()))
out.write('\n')
l = p.getListing()
for i in l.getInstructions(True):
    out.write('%04x  %-24s %s\n' % (i.getAddress().getOffset(), i.getBytesAsString(), i.toString()))
out.write('\n=== DATA ===\n')
for d in l.getDefinedData(True):
    out.write('%04x  %-24s %s\n' % (d.getAddress().getOffset(), d.getBytesAsString(), d.toString()))
out.close()
print('Exported /tmp/opencode/disasm-' + name + '.txt')
