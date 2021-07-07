TITLE Assignment 3     (a3.asm)

; Author: Abraham Almahmoud
; Last Modified: 05/02/2020
; Course number/section: 271
; Project Number: 3                Due Date: 05/03/2020
; Description: Take a sum of inputed negative numbers and output the maximum, minimum, sum, and roudned average.

INCLUDE Irvine32.inc

UPPERLIMIT_A = 88
LOWERLIMIT_A = 55
UPPERLIMIT_B = 40
LOWERLIMIT_B = 1

.data
intro			BYTE	"Welcome to the Integer Accumulator by Abraham Almahmoud", 10, 0
prompt_1		BYTE	"What is your name? ", 0
input_name		BYTE	33 DUP(0)
prompt_2		BYTE	"Hello there, ", 0
prompt_3		BYTE	"Please enter number in [-88, -55] or [-40, -1].", 10, 0
prompt_4		BYTE	"Enter a non-negative number when you are finished to see results.", 10, 0
prompt_5a		BYTE	"Enter number ", 0
prompt_5b		BYTE	": ", 0
num_1			DWORD	?
num_2			DWORD	?
num_3			DWORD	?
prompt_6		BYTE	"Number Invalid!", 10, 0
prompt_7a		BYTE	"You entered ", 0
valid_num		DWORD	0
prompt_7b		BYTE	" valid numbers.", 10, 0
prompt_8		BYTE	"The maximum valid number is -", 0
max_num			DWORD	88
prompt_9		BYTE	"The minimum valid number is -", 0
min_num			DWORD	1
prompt_10		BYTE	"The sum of your valid numbers is -", 0
sum_num			DWORD	?
prompt_11		BYTE	"The rounded average is -",0
thousand		DWORD	1000
avg_num			DWORD	?
remainder		DWORD	?
goodbye			BYTE	"We have to stop meeting like this. Farewell, ", 0
ec_1			BYTE	"**EC: Number the lines during user input.", 10, "      Increment the line number only for valid number entries.", 10, 0



.code
main PROC
;Introduction
	mov		edx, OFFSET intro
	call	WriteString
	mov		edx, OFFSET ec_1
	call	WriteString
	call	CrLf
	mov		edx, OFFSET prompt_1
	call	WriteString
	mov		edx, OFFSET input_name
	mov		ecx, 32
	call	ReadString
	mov		edx, OFFSET prompt_2
	call	WriteString
	mov		edx, OFFSET	input_name
	call	WriteString
	call	CrLf

;Instructions
	mov		edx, OFFSET prompt_3
	call	WriteString
	mov		edx, OFFSET prompt_4
	call	WriteString
	jmp		enter_num

;Repeatedly prompt the user to enter a number.
invalid_num:
	mov		edx, OFFSET	prompt_6
	call	WriteString
enter_num:
	mov		edx, OFFSET prompt_5a
	call	WriteString
	mov		eax, valid_num
	add		eax, 1
	call	WriteDec
	mov		edx, OFFSET prompt_5b
	call	WriteString
	call	ReadInt
	cmp		eax, 0
	jns		output_num
	mov		ebx, -1
	mul		ebx

;Validate the user input to be in [-88, -55] or [-40, -1] (inclusive). Actual value check is the positive equivalent
validate_num:
	cmp		eax, UPPERLIMIT_A
	jg		invalid_num
	cmp		eax, LOWERLIMIT_A
	jl		limit_two
	jmp		inputOK
limit_two:
	cmp		eax, UPPERLIMIT_B
	jg		invalid_num
	cmp		eax, LOWERLIMIT_B
	jl		invalid_num
inputOK:
;initialize accumulator and loop control
	inc		valid_num	;count valid_num(s)
	cmp		eax, max_num
	jg		min_check
	mov		max_num, eax
min_check:
	cmp		eax, min_num
	jl		pass_num
	mov		min_num, eax
pass_num:
	add		sum_num,eax		;accumulator
	jmp		enter_num

;Display:
;Output the number of validated numbers entered
output_num:
	mov		edx, OFFSET prompt_7a
	call	WriteString
	mov		eax, valid_num
	call	WriteDec
	mov		edx, OFFSET prompt_7b
	call	WriteString
	call	CrLf
	cmp		valid_num, 0
	je		farewell

;Output the maximum, minimum, sum, and the rounded average of negative numbers entered
	mov		edx, OFFSET prompt_8
	call	WriteString
	mov		eax, max_num
	call	WriteDec
	call	CrLf
	mov		edx, OFFSET prompt_9
	call	WriteString
	mov		eax, min_num
	call	WriteDec
	call	CrLf
	mov		edx, OFFSET prompt_10
	call	WriteString
	mov		eax, sum_num
	call	WriteDec
	call	CrLf
;Calculate the (rounded integer) average of the valid numbers.
	fild    sum_num				;A / B
	fidiv   valid_num
	fimul	thousand			;multiple result by 1000
	frndint						;remove float, convert to int
	fistp	avg_num				;store temp value
	mov		eax, avg_num
	cdq
	mov		ebx, thousand
	div		ebx					;divide temp value by 1000
	mov		avg_num, eax		;quotient 
	mov		remainder, edx		;remainder
	cmp		remainder, 510		;check if remainder is greater than 0.51
	jl		round_down
	add		avg_num, 1
round_down:	
	mov		edx, OFFSET prompt_11
	call	WriteString
	mov		eax, avg_num
	call	WriteDec
	call	CrLf

;Farewell
farewell:
	mov		edx, OFFSET goodbye
	call	WriteString
	mov		edx, OFFSET input_name
	call	WriteString
	call	CrLf

	exit	; exit to operating system
main ENDP

; (insert additional procedures here)

END main
