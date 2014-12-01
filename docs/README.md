GDBStub
=======

The finished product of a project at Bob Jones University.  A complete userguide is located in sim2Report



Division of labor: during the GDB stub phase



An implemention of a gbd server for CpS 310 at BJU

Requirements
  
·         Initial connect/handshake [C-level] -- Benjamin (FINISHED)
·		  Implement CPU methods in handshake -- Benjamin (FINISHED)
·         Get/set general purpose registers [C] -- Daniel (FINISHED)
·         Get/set regions of memory [C] -- Daniel (FINISHED)
·         Single step/continue (execution faked at first, of course) [C] -- Benjamin (FINISHED) /Daniel(FINISHED)
·         Load new binary image [B-level] -- 
				gdb will use mov commands instead if you reply with an empty packet.
			
·         Set/clear software breakpoints [A-level] -- Daniel(FINISHED) /Benjamin (FINISHED)
				1110,00010010, immed 19..8, 0111, immed 3..0	
					where immed is ignored by arm hardware but can be used by a debugger
					to store additional information about the breakpoint

All requirements are believed to have been completed.  

GDB stub phase completed back to solo projects.

The simulator can intereact with a gdb client for debugging purposes.

