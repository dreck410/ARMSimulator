GDBStub
=======

Attempt at implementing a gbd server for CpS 310 at BJU

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

Important notes.  

The Mov command will give you a address in Hex, a length of data in hex and then hex data to be loaded. 

Command Line Options:
"--load fileName.exe" loads a file into RAM
"--debug" makes the program listen on port 8080 for a connection from a gdb stub
"--mem XXX" where XXX is any positive number less than 1 GB sets the size of memory to XXX
"--test" runs the test cases.
