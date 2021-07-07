TITLE Assignment 1  (a1.asm)

; Author: Abraham Almahmoud
; Last Modified:4/9/2020
; Course number/section:271
; Project Number:1                Due Date: 04/12/2020
; Description: Prompt user to enter three numbers for basic calculations.

INCLUDE Irvine32.inc

r_answer = 1				;Integer used for repeat condition

.data
p_title		BYTE	"Elementary Arithmetic by Abraham Almahmoud", 0
ec_1		BYTE	"**EC: Repeat until the user chooses to quit.", 0
ec_2		BYTE	"**EC: Program verifies the numbers are in descending order.", 0
ec_3		BYTE	"**EC: Handles negative results and computes B-A, C-A, C-B, C-B-A.", 0
ec_4		BYTE	"**EC: Calculate and display the quotients A/B, A/C, B/C.", 0
intro		BYTE	"Enter 3 numbers A > B > C, and I'll show you the sums and differences.", 0
num_A		DWORD	?
num_B		DWORD	?
num_C		DWORD	?
A_p_B		DWORD	?		;Stores value for A+B
A_m_B		DWORD	?		;Stores value for A-B
A_p_C		DWORD	?		;Stores value for A+C
A_m_C		DWORD	?		;Stores value for A-C
B_p_C		DWORD	?		;Stores value for B+C
B_m_C		DWORD	?		;Stores value for B-C
A_p_B_p_C	DWORD	?		;Stores value for A+B+C
A_m_B_m_C	DWORD	?		;Stores value for A-B-C
thousand	DWORD	1000
divided		DWORD	?		;Stores value for the temporary divided value
A_d_B_q		DWORD	?		;Stores value for quotient A/B 
A_d_B_r		DWORD	?		;Stores value for remainder A/B
A_d_C_q		DWORD	?		;Stores value for quotient A/C
A_d_C_r		DWORD	?		;Stores value for remainder A/C
B_d_C_q		DWORD	?		;Stores value for quotient B/C
B_d_C_r		DWORD	?		;Stores value for remainder B/C
prompt_1	BYTE	"First number: ", 0
prompt_2	BYTE	"Second number: ", 0
prompt_3	BYTE	"Three number: ", 0
jle_prompt	BYTE	"ERROR: The numbers are not in descending order!", 0
t_plus		BYTE	" + ", 0
t_minus		BYTE	" - ", 0
t_equal		BYTE	" = ", 0
t_neg		BYTE	"-", 0
t_div		BYTE	" / ", 0
t_point		BYTE	".", 0
r_prompt	BYTE	"Would you like to repeat the program? ENTER '1' to repeat!", 0
r_input		DWORD	?
goodBye		BYTE	"Impressed? Bye!", 0

.code
main PROC

;Introduction
	mov		edx, OFFSET p_title
	call	WriteString
	call	CrLf
	mov		edx, OFFSET ec_1
	call	WriteString
	call	CrLf
	mov		edx, OFFSET ec_2
	call	WriteString
	call	CrLf
	mov		edx, OFFSET ec_3
	call	WriteString
	call	CrLf
repeat_loop:
	call	CrLf
	mov		edx, OFFSET intro
	call	WriteString
	call	CrLf
	call	CrLf

;Get the data
	mov		edx, OFFSET prompt_1
	call	WriteString
	call	ReadInt
	mov		num_A, eax
	mov		edx, OFFSET prompt_2
	call	WriteString
	call	ReadInt
	mov		num_B, eax
	cmp		num_A, eax			
	jle		descend_false		
	mov		edx, OFFSET prompt_3
	call	WriteString
	call	ReadInt
	mov		num_C, eax
	cmp		num_B, eax			
	jle		descend_false
	call	CrLf
	jmp		calc
descend_false:					;numbers are not decending!
	mov		edx, OFFSET jle_prompt
	call	WriteString
	jmp		theEnd

;Calculate the required values
calc:
	mov		ebx, num_A			;A + B
	add		ebx, num_B
	mov		A_p_B, ebx

	mov		ebx, num_A			;A - B
	sub		ebx, num_B
	mov		A_m_B, ebx

	mov		ebx, num_A			;A + C
	add		ebx, num_C
	mov		A_p_C, ebx

	mov		ebx, num_A			;A - C
	sub		ebx, num_C
	mov		A_m_C, ebx

	mov		ebx, num_B			;B + C
	add		ebx, num_C
	mov		B_p_C, ebx

	mov		ebx, num_B			;B - C
	sub		ebx, num_C
	mov		B_m_C, ebx

	mov		ebx, A_p_B			;A + B + C
	add		ebx, num_C
	mov		A_p_B_p_C, ebx

	mov		ebx, B_m_C			;A - B - C
	add		ebx, num_A
	mov		A_m_B_m_C, ebx

	fild    num_A				;A / B
	fidiv   num_B
	fimul	thousand			;multiple result by 1000
	frndint						;remove float, convert to int
	fistp	divided				;store temp value
	mov		eax, divided
	cdq
	mov		ebx, thousand
	div		ebx					;divide temp value by 1000
	mov		A_d_B_q, eax		;quotient 
	mov		A_d_B_r, edx		;remainder

	fild    num_A				;A / C
	fidiv   num_C
	fimul	thousand			;multiple result by 1000
	frndint						;remove float, convert to int
	fistp	divided				;store temp value
	mov		eax, divided
	cdq
	mov		ebx, thousand
	div		ebx					;divide temp value by 1000
	mov		A_d_C_q, eax		;quotient 
	mov		A_d_C_r, edx		;remainder

	fild    num_B				;B / C
	fidiv   num_C
	fimul	thousand			;multiple result by 1000
	frndint						;remove float, convert to int
	fistp	divided				;store temp value
	mov		eax, divided		
	cdq
	mov		ebx, thousand
	div		ebx					;divide temp value by 1000
	mov		B_d_C_q, eax		;quotient 
	mov		B_d_C_r, edx		;remainder

	

;Display the results
	mov		eax, num_A			;A + B
	call	WriteDec
	mov		edx, OFFSET t_plus
	call	WriteString
	mov		eax, num_B
	call	WriteDec
	mov		edx, OFFSET t_equal
	call	WriteString
	mov		eax, A_p_B
	call	WriteDec
	call	CrLf

	mov		eax, num_A			;A - B
	call	WriteDec
	mov		edx, OFFSET t_minus
	call	WriteString
	mov		eax, num_B
	call	WriteDec
	mov		edx, OFFSET t_equal
	call	WriteString
	mov		eax, A_m_B
	call	WriteDec
	call	CrLf

	mov		eax, num_A			;A + C
	call	WriteDec
	mov		edx, OFFSET t_plus
	call	WriteString
	mov		eax, num_C
	call	WriteDec
	mov		edx, OFFSET t_equal
	call	WriteString
	mov		eax, A_p_C
	call	WriteDec
	call	CrLf

	mov		eax, num_A			;A - C
	call	WriteDec
	mov		edx, OFFSET t_minus
	call	WriteString
	mov		eax, num_C
	call	WriteDec
	mov		edx, OFFSET t_equal
	call	WriteString
	mov		eax, A_m_C
	call	WriteDec
	call	CrLf

	mov		eax, num_B			;B + C
	call	WriteDec
	mov		edx, OFFSET t_plus
	call	WriteString
	mov		eax, num_C
	call	WriteDec
	mov		edx, OFFSET t_equal
	call	WriteString
	mov		eax, B_p_C
	call	WriteDec
	call	CrLf
	
	mov		eax, num_B			;B - C
	call	WriteDec
	mov		edx, OFFSET t_minus
	call	WriteString
	mov		eax, num_C
	call	WriteDec
	mov		edx, OFFSET t_equal
	call	WriteString
	mov		eax, B_m_C
	call	WriteDec
	call	CrLf
	
	mov		eax, num_A			;A + B + C
	call	WriteDec
	mov		edx, OFFSET t_plus
	call	WriteString
	mov		eax, num_B
	call	WriteDec
	mov		edx, OFFSET t_plus
	call	WriteString
	mov		eax, num_C
	call	WriteDec
	mov		edx, OFFSET t_equal
	call	WriteString
	mov		eax, A_p_B_p_C
	call	WriteDec
	call	CrLf

	mov		eax, num_B			;B - A
	call	WriteDec
	mov		edx, OFFSET t_minus
	call	WriteString
	mov		eax, num_A
	call	WriteDec
	mov		edx, OFFSET t_equal
	call	WriteString
	mov		edx, OFFSET t_neg
	call	WriteString
	mov		eax, A_m_B
	call	WriteDec
	call	CrLf

	mov		eax, num_C			;C - A
	call	WriteDec
	mov		edx, OFFSET t_minus
	call	WriteString
	mov		eax, num_A
	call	WriteDec
	mov		edx, OFFSET t_equal
	call	WriteString
	mov		edx, OFFSET t_neg
	call	WriteString
	mov		eax, A_m_C
	call	WriteDec
	call	CrLf

	mov		eax, num_C			;C - B
	call	WriteDec
	mov		edx, OFFSET t_minus
	call	WriteString
	mov		eax, num_B
	call	WriteDec
	mov		edx, OFFSET t_equal
	call	WriteString
	mov		edx, OFFSET t_neg
	call	WriteString
	mov		eax, B_m_C
	call	WriteDec
	call	CrLf

	mov		eax, num_C			;C - B - A
	call	WriteDec
	mov		edx, OFFSET t_minus
	call	WriteString
	mov		eax, num_B
	call	WriteDec
	mov		edx, OFFSET t_minus
	call	WriteString
	mov		eax, num_A
	call	WriteDec
	mov		edx, OFFSET t_equal
	call	WriteString
	mov		edx, OFFSET t_neg
	call	WriteString
	mov		eax, A_m_B_m_C
	call	WriteDec
	call	CrLf

	mov		eax, num_A			;A / B
	call	WriteDec
	mov		edx, OFFSET t_div
	call	WriteString
	mov		eax, num_B
	call	WriteDec
	mov		edx, OFFSET t_equal
	call	WriteString
	mov		eax, A_d_B_q
	call	WriteDec
	mov		edx, OFFSET t_point
	call	WriteString
	mov		eax, A_d_B_r
	call	WriteDec
	call	CrLf

	mov		eax, num_A			;A / C
	call	WriteDec
	mov		edx, OFFSET t_div
	call	WriteString
	mov		eax, num_C
	call	WriteDec
	mov		edx, OFFSET t_equal
	call	WriteString
	mov		eax, A_d_C_q
	call	WriteDec
	mov		edx, OFFSET t_point
	call	WriteString
	mov		eax, A_d_C_r
	call	WriteDec
	call	CrLf

	mov		eax, num_B			;B / C
	call	WriteDec
	mov		edx, OFFSET t_div
	call	WriteString
	mov		eax, num_C
	call	WriteDec
	mov		edx, OFFSET t_equal
	call	WriteString
	mov		eax, B_d_C_q
	call	WriteDec
	mov		edx, OFFSET t_point
	call	WriteString
	mov		eax, B_d_C_r
	call	WriteDec
	call	CrLf
	call	CrLf

;Say Goodbye
	mov		edx, OFFSET r_prompt
	call	WriteString
	call	CrLf
	call	ReadInt
	mov		r_input, eax
	cmp		eax, r_answer		;compare r_input
	je		repeat_loop			;repeat program if equal
	call	CrLf
	mov		edx, OFFSET goodBye
	call	WriteString
	call	CrLf
theEnd:
	exit	; exit to operating system
main ENDP

END main
