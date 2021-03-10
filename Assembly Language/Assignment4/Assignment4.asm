TITLE Assignment 4     (Assignement4.asm)

; Author: Abraham Almahmoud
; Last Modified: 05/10/2020
; OSU email address: almahmoa@oregonstate.edu
; Course number/section: 271
; Project Number: 3                Due Date: 05/10/2020
; Description: Output composite numbers within a given range

INCLUDE Irvine32.inc

UPPERLIMIT = 9999
LOWERLIMIT = 1

.data
intro			BYTE	"Composite Numbers	Programmed by Abraham Almahmoud", 10, 0
prompt_1		BYTE	"Enter the number of composite numbers you would like to see. ", 10, 0
prompt_2		BYTE	"I'll accept orders for up to 400 composites.", 10, 10, 0
prompt_3		BYTE	"Enter the number of composites to display [1 .. 9999]: ", 0
prompt_4		BYTE	"Out of range. Try again.", 10, 0
ec_2_prompt		BYTE	"Press any key to continue . . .", 0
space_quad		BYTE	"   ", 0
space_triple	BYTE	"    ", 0
space_double	BYTE	"     ", 0
space_single	BYTE	"      ", 0
num_inc			DWORD	0
newpage			DWORD	240					;use to count lines for page
ten				DWORD	10					;use to count for new line
three			DWORD	3					;use to check all numbers below 3
two				DWORD	2					;use to check if even
one				DWORD	1					;use to prevent divisable by 1
temp			DWORD	?
remainder		DWORD	0
cas_num			DWORD	?
goodbye			BYTE	"Results certified by Abraham Almahmoud. Goodbye.", 0
ec_1			BYTE	"**EC: Align the output columns", 10, 0
ec_2			BYTE	"**EC: Display more composites, but show them one page at a time.", 10, 10, 0

.code
main PROC
	call	introduction
	call	getUserData
	call	showComposites
	call	farewell
	exit	; exit to operating system
main ENDP

introduction	PROC
	mov		edx, OFFSET intro
	call	WriteString
	mov		edx, OFFSET ec_1
	call	WriteString
	mov		edx, OFFSET ec_2
	call	WriteString
	mov		edx, OFFSET prompt_1
	call	WriteString
	mov		edx, OFFSET prompt_2
	call	WriteString
	ret
introduction	ENDP

getUserData		PROC
userdata:
	mov		edx, OFFSET prompt_3
	call	WriteString
	call	ReadDec
;validate
	cmp		eax, UPPERLIMIT
	jg		invalid
	cmp		eax, LOWERLIMIT
	jl		invalid
	jmp		valid
invalid:
	mov		edx, OFFSET prompt_4
	call	WriteString
	jmp		userdata
valid:
	ret
getUserData		ENDP

showComposites	PROC
	mov		ecx, eax
	mov		eax, 1
isComposites:
	jmp		composite_checker
return_loop:
	inc		eax
	loop	isComposites			   
	jmp		exit_composites			    ;jump to end of procedure
composite_checker:
;Less than 3?
	cmp		eax, three
	jle		invalid					    ;check if eax is less than 3
;Even number?
	mov		temp, eax
	cdq
	mov		ebx, two
	div		ebx
	mov		remainder, edx
	cmp		remainder, 0
	mov		eax, temp
	je		valid						;check if eax is even
;Loop to check all numbers leading up
setchecker:
	mov		cas_num, eax
oddchecker:								;check if odd number is a composite
	sub		cas_num, 1
	cmp		cas_num, 1
	je		invalidchecker				;make sure cascading number does not check if divisable by 1
	cdq
	mov		ebx, cas_num
	div		ebx
	mov		remainder, edx
	mov		eax, remainder
	cmp		remainder, 0
	je		validchecker				;if number is divisable by any number between 2-(eax-1), is composite
	mov		eax, temp
	jmp		oddchecker
invalidchecker:
	mov		eax, temp
	jmp		invalid
validchecker:
	mov		eax, temp
	jmp		valid
;OutPut
valid:
	call	WriteDec
	cmp		eax, 1000
	jge		quad_space
	cmp		eax, 100
	jge		triple_space
	cmp		eax, 10
	jge		double_space
	mov		edx, OFFSET space_single
	call	WriteString
	jmp		increment_num
quad_space:
	mov		edx, OFFSET space_quad
	call	WriteString
	jmp		increment_num
triple_space:
	mov		edx, OFFSET space_triple
	call	WriteString
	jmp		increment_num
double_space:
	mov		edx, OFFSET space_double
	call	WriteString
increment_num:
	mov		temp, eax
	inc		num_inc						;track the number of numbers outputted
	mov		eax, num_inc
	cdq
	mov		ebx, ten
	div		ebx
	mov		remainder, edx
	mov		eax, remainder
	cmp		remainder, 0
	je		next_line
incrememnt_eax:
	mov		eax, temp
invalid:
	jmp		return_loop
next_line:								;move to next line if 5 numbers were outputed
	call	CrLf
	mov		eax, num_inc
	cdq
	mov		ebx, newpage
	div		ebx
	mov		remainder, edx
	mov		eax, remainder
	cmp		remainder, 0
	je		next_page
	jmp		incrememnt_eax
next_page:
	mov		edx, OFFSET ec_2_prompt
	call	WaitMsg
	call	CrLf
	jmp		incrememnt_eax
exit_composites:
	ret
showComposites	ENDP

farewell		PROC
	call	CrLf
	call	CrLf
	mov		edx, OFFSET goodbye
	call	WriteString
	ret
farewell		ENDP
END main