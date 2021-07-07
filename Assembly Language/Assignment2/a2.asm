TITLE Assignment 2     (a2.asm)

; Author: Abraham Almahmoud
; Last Modified:4/18/2020
; Course number/section: 271
; Project Number: 2                 Due Date: 04/19/2020
; Description: A program that will calculate Fibonacci numbers.

INCLUDE Irvine32.inc

UPPERLIMIT = 46
LOWERLIMIT = 1

.data
p_title			BYTE	"Fibonacci Numbers", 10,  0
p_author		BYTE	"Programmed by Abraham Almahmoud", 10, 0
prompt_1		BYTE	"What's Your Name? ", 0
prompt_2		BYTE	"Hello, ", 0
prompt_3		BYTE	"Enter the number of Fibonacci terms to be displayed", 10, 0
prompt_4		BYTE	"Give the number as an integer in the range [1 .. 46].", 10, 10, 0
prompt_5		BYTE	"How many Fibonacci terms do you want? ", 0
prompt_6		BYTE	"Out of range. Enter a number in [1 .. 46]", 10, 0
goodbye_1		BYTE	"Results certified by Abraham Almahmoud.",10, 0
goodbye_2		BYTE	"Goodbye, ", 0
goodbye_3		BYTE	".", 0
input_name		BYTE	33 DUP(0)
input_num		DWORD	?
num_a			DWORD	1
num_b			DWORD	0
num_c			DWORD	0
num_inc			DWORD	0
space5			BYTE	"     ", 0
space6			BYTE	"      ", 0
space7			BYTE	"       ", 0
space8			BYTE	"        ", 0
space9			BYTE	"         ", 0
space10			BYTE	"          ", 0
space11			BYTE	"           ", 0
space12			BYTE	"            ", 0
space13			BYTE	"             ", 0
space14			BYTE	"              ", 0
ec_1			BYTE	"**EC: Display the numbers in aligned columns.", 10, 10, 0

.code
main PROC
;Introduction
	mov		edx, OFFSET p_title
	call	WriteString
	mov		edx, OFFSET p_author
	call	WriteString
	mov		edx, OFFSET ec_1
	call	WriteString
	mov		edx, OFFSET	prompt_1
	call	WriteString
	mov		edx, OFFSET input_name
	mov		ecx, 32
	call	ReadString
	mov		edx, OFFSET prompt_2
	call	WriteString
	mov		edx, OFFSET	input_name
	call	WriteString
	call	CrLf
;User Instructions
	mov		edx, OFFSET	prompt_3
	call	WriteString
	mov		edx, OFFSET	prompt_4
	call	WriteString
	jmp		user_data
;Get User Data
invalid_num:							;text that outputs if number input was invalid
	mov		edx, OFFSET prompt_6
	call	WriteString
user_data:								;prompt input for fibonacci terms
	mov		edx, OFFSET	prompt_5
	call	WriteString
	call	ReadInt
	mov		input_num, eax
;validate the input
	cmp		eax, UPPERLIMIT
	jg		invalid_num
	cmp		eax, LOWERLIMIT
	jl		invalid_num
;Display Fibs
	call	CrLf
	mov		ecx, input_num
fibonacci_loop:
	jmp fibonacci
return_loop:							;return from conditional statment to continue the loop
	loop	fibonacci_loop
	jmp		goodbye						;ecx has reached 0, jump to farewell
fibonacci:								;fibonacci number generator
	mov		eax, num_a
	call	WriteDec
;check which magnitude of 10 the current fibonacci number is at to detrimine the space distance needed for allignment
	cmp		num_a, 1000000000
	jge		insert_space5
	cmp		num_a, 100000000
	jge		insert_space6
	cmp		num_a, 10000000
	jge		insert_space7
	cmp		num_a, 1000000
	jge		insert_space8
	cmp		num_a, 100000
	jge		insert_space9
	cmp		num_a, 10000
	jge		insert_space10
	cmp		num_a, 1000
	jge		insert_space11
	cmp		num_a, 100
	jge		insert_space12
	cmp		num_a, 10
	jge		insert_space13
	mov		edx, OFFSET space14
	jmp		write_space
;write the appropriate space needed for the current fibonacci number for allignment
insert_space5:
	mov		edx, OFFSET space5
	jmp		write_space
insert_space6:
	mov		edx, OFFSET space6
	jmp		write_space
insert_space7:
	mov		edx, OFFSET space7
	jmp		write_space
insert_space8:
	mov		edx, OFFSET space8
	jmp		write_space
insert_space9:
	mov		edx, OFFSET space9
	jmp		write_space
insert_space10:
	mov		edx, OFFSET space10
	jmp		write_space
insert_space11:
	mov		edx, OFFSET space11
	jmp		write_space
insert_space12:
	mov		edx, OFFSET space12
	jmp		write_space
insert_space13:
	mov		edx, OFFSET space13
	jmp		write_space
write_space:
	call	WriteString
	inc		num_inc						;track the number of numbers outputted
	cmp		num_inc, 5
	je		next_line
	jmp		math
next_line:								;move to next line if 5 numbers were outputed
	call	CrLf
	mov		num_inc, 0
math:									;the math for generating the next fibonacci number			
	mov		ebx, num_b
	mov		num_c, ebx
	mov		ebx, num_a
	mov		num_b, ebx
	mov		ebx, num_a
	add		ebx, num_c
	mov		num_a, ebx
	jmp		return_loop
;Farewell
goodbye:
	call	CrLf
	call	CrLf
	mov		edx, OFFSET goodbye_1
	call	WriteString
	mov		edx, OFFSET goodbye_2
	call	WriteString
	mov		edx, OFFSET	input_name
	call	WriteString
	mov		edx, OFFSET goodbye_3
	call	WriteString
	exit	; exit to operating system
main ENDP

END main
